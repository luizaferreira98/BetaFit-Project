using System.Security.Claims;
using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;
using BetaFit.UI.Models;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.UI.Controllers;

[Route("Cart")]
public class CartController : Controller
{
    private readonly IOrderService _orderService;
    private readonly HttpCartService _cart;
    private readonly HttpProfileService _profile;

    public CartController(IOrderService orderService, HttpCartService cart, HttpProfileService profile)
    { _orderService = orderService; _cart = cart; _profile = profile; }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var items = await _cart.GetAsync();
            if(HttpContext.Items["GuestCartWarning"] is string warning)TempData["Error"]=warning;
            ViewData["Title"] = "Carrinho";
            ViewData["Total"] = items.Sum(x => x.Subtotal);
            ViewData["ItemCount"] = items.Sum(x => x.Quantity);
            return View(items.Select(x => new CartItem { ProductId=x.ProductId, Name=x.Name, Price=x.Price, ImageUrl=x.ImageUrl, Size=x.Size, Color=x.Color, Quantity=x.Quantity }).ToList());
        }
        catch (HttpRequestException) { TempData["Error"] = "Não foi possível carregar o carrinho agora."; return View(new List<CartItem>()); }
    }

    [HttpPost("UpdateQuantity"), ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateQuantity(int productId, string? size, string? color, int quantity)
    { var result = await _cart.UpdateAsync(productId, size, color, quantity); if (!result.Ok) TempData["Error"] = result.Message; return RedirectToAction(nameof(Index)); }

    [HttpPost("Remove"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int productId, string? size, string? color)
    { if (!await _cart.RemoveAsync(productId, size, color)) TempData["Error"] = "Não foi possível remover o item."; return RedirectToAction(nameof(Index)); }

    [Authorize, HttpGet("Checkout")]
    public async Task<IActionResult> Checkout()
    {
        var items = await _cart.GetAsync();
        if (!items.Any()) return RedirectToAction(nameof(Index));
        var vm = new CheckoutViewModel { Items = ToViewItems(items) };
        try
        {
            var profile = await _profile.GetAsync();
            if (profile is not null)
            {
                ApplySavedAddress(vm, profile);
                vm.HasSavedAddress = HasCompleteAddress(vm);
                if (vm.HasSavedAddress)
                    vm.Cpf = string.Empty;
                ApplySavedCard(vm, profile, true);
            }
        }
        catch (HttpRequestException) { /* o formulário ainda permite preenchimento manual */ }
        return View(vm);
    }

    [Authorize, HttpPost("Checkout"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel vm)
    {
        try { vm.Items = ToViewItems(await _cart.GetAsync()); }
        catch (HttpRequestException) { TempData["Error"] = "Não foi possível carregar o carrinho."; return RedirectToAction(nameof(Index)); }
        if (!vm.Items.Any()) return RedirectToAction(nameof(Index));

        UserDto? profile = null;
        try { profile = await _profile.GetAsync(); }
        catch (HttpRequestException) { ModelState.AddModelError(string.Empty, "Não foi possível confirmar os dados do seu perfil agora."); return View(vm); }

        var hasSavedAddress = profile is not null && HasCompleteAddress(profile);
        ApplySavedCard(vm, profile);
        if (hasSavedAddress)
        {
            // O endereço nunca é confiado a campos ocultos: ele é recuperado novamente do perfil.
            ApplySavedAddress(vm, profile!);
            vm.HasSavedAddress = true;
            foreach (var field in AddressFields) ModelState.Remove(field);

            var typedCpf = DigitsOnly(vm.Cpf);
            var savedCpf = DigitsOnly(profile!.Cpf);
            if (!string.IsNullOrWhiteSpace(savedCpf) && !string.Equals(typedCpf, savedCpf, StringComparison.Ordinal))
                ModelState.AddModelError(nameof(vm.Cpf), "Informe o CPF cadastrado no seu perfil para confirmar esta compra.");
        }

        if (!ModelState.IsValid) return View(vm);

        var usesCard = vm.PaymentMethod is "Credito" or "Debito";
        if (usesCard)
        {
            if (vm.UseSavedCard && vm.HasSavedCard)
            {
                if(!vm.SavedCards.Any(x=>x.Id==vm.SelectedCardId && x.Type==vm.PaymentMethod && BetaFit.Application.Services.DemoWallet.ValidExpiry(x.Expiry)))
                {ModelState.AddModelError(string.Empty,"Selecione um cartão válido do tipo escolhido.");return View(vm);}
            }
            else
            {
                var number = DigitsOnly(vm.CardNumber);
                var code = DigitsOnly(vm.CardSecurityCode);
                if (string.IsNullOrWhiteSpace(vm.CardHolderName) || vm.CardHolderName.Length > 120 || number.Length is < 13 or > 19 || code.Length is < 3 or > 4 || string.IsNullOrWhiteSpace(vm.CardExpiry))
                { ModelState.AddModelError(string.Empty, "Cadastre um cartão válido para continuar com crédito ou débito."); return View(vm); }
                var cardResult = await _profile.SaveCardAsync(new PaymentCardDto { CardHolderName=vm.CardHolderName, CardNumber=number, Expiry=vm.CardExpiry, SecurityCode=code,Type=vm.PaymentMethod });
                if (!cardResult.Ok) { ModelState.AddModelError(string.Empty, cardResult.Message); return View(vm); }
                profile = await _profile.GetAsync(); vm.SelectedCardId=profile?.Cards.LastOrDefault()?.Id; ApplySavedCard(vm, profile);
            }
        }

        // Na primeira compra, o endereço e o CPF são gravados para que os próximos checkouts peçam somente o CPF.
        if (!hasSavedAddress || string.IsNullOrWhiteSpace(profile?.Cpf))
        {
            var saved = await _profile.SaveCheckoutAddressAsync(new CheckoutAddressDto
            {
                Cpf = vm.Cpf, Cep = vm.Cep, Street = vm.Street, Number = vm.Number, Complement = vm.Complement,
                Neighborhood = vm.Neighborhood, City = vm.City, State = vm.State
            });
            if (!saved.Ok) { ModelState.AddModelError(string.Empty, saved.Message); return View(vm); }
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();
        var dto = new CreateOrderDto
        {
            Items = vm.Items.Select(c => new CreateOrderItemDto { ProductId=c.ProductId, Quantity=c.Quantity, Size=c.Size, Color=c.Color }).ToList(),
            CouponCode=vm.CouponCode,PaymentMethod = vm.PaymentMethod, CustomerCpf = new string(vm.Cpf.Where(char.IsDigit).ToArray()),
            ShippingCep = new string(vm.Cep.Where(char.IsDigit).ToArray()), ShippingStreet = vm.Street.Trim(),
            ShippingNumber = vm.Number.Trim(), ShippingComplement = vm.Complement?.Trim(),
            ShippingNeighborhood = vm.Neighborhood.Trim(), ShippingCity = vm.City.Trim(), ShippingState = vm.State.Trim().ToUpperInvariant(),
            SavedCardId = usesCard ? vm.SelectedCardId : null, Installments = vm.PaymentMethod=="Credito" ? vm.Installments : 1
        };
        try
        {
            var order = await _orderService.CreateAsync(dto, userId);
            await _cart.ClearAsync();
            if (vm.PaymentMethod=="Boleto") return RedirectToAction(nameof(Boleto),new{id=order.Id});
            if (string.Equals(vm.PaymentMethod, "Pix", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction(nameof(Pix), new { id = order.Id });
            TempData["Success"] = "Pedido criado com sucesso. A forma de pagamento foi registrada.";
            return RedirectToAction("Details", "Orders", new { id = order.Id });
        }
        catch (HttpRequestException) { TempData["Error"] = "Não foi possível criar o pedido agora."; return RedirectToAction(nameof(Index)); }
        catch (InvalidOperationException ex) { ModelState.AddModelError(string.Empty, ex.Message); return View(vm); }
    }

    [Authorize, HttpGet("Pix/{id:int}")]
    public async Task<IActionResult> Pix(int id)
    {
        var order=await _orderService.GetByIdAsync(id); if(order is null)return NotFound();
        var userId=User.FindFirstValue(ClaimTypes.NameIdentifier); if(order.UserId!=userId)return Forbid();
        if(!order.PaymentMethod.Contains("Pix",StringComparison.OrdinalIgnoreCase))return RedirectToAction("Details","Orders",new{id});
        return View(order);
    }

    [Authorize, HttpPost("Pix/{id:int}/confirm"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmPix(int id)
    {
        var userId=User.FindFirstValue(ClaimTypes.NameIdentifier)??string.Empty;
        var result=await _orderService.ConfirmDemoPaymentAsync(id,userId);
        TempData[result.Ok?"Success":"Error"]=result.Ok?"Transação concluída com sucesso (demonstração).":result.Message;
        return RedirectToAction("Details","Orders",new{id});
    }

    [Authorize,HttpGet("Boleto/{id:int}")]
    public async Task<IActionResult> Boleto(int id)
    {
        var order=await _orderService.GetByIdAsync(id);if(order is null)return NotFound();
        if(order.UserId!=User.FindFirstValue(ClaimTypes.NameIdentifier))return Forbid();
        if(!order.PaymentMethod.Contains("Boleto",StringComparison.OrdinalIgnoreCase))return NotFound();
        return View(order);
    }
    [Authorize,HttpPost("Boleto/{id:int}/confirm"),ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmBoleto(int id)
    {
        var order=await _orderService.GetByIdAsync(id);if(order is null)return NotFound();
        if(order.UserId!=User.FindFirstValue(ClaimTypes.NameIdentifier))return Forbid();
        if(!order.PaymentMethod.Contains("Boleto",StringComparison.OrdinalIgnoreCase))return BadRequest();
        var result=await _orderService.ConfirmDemoPaymentAsync(id,order.UserId);
        TempData[result.Ok?"Success":"Error"]=result.Ok?"Pagamento do boleto simulado com sucesso.":result.Message;
        return RedirectToAction("Details","Orders",new{id});
    }

    private static List<CartItem> ToViewItems(IEnumerable<CartItemDto> items) => items.Select(x => new CartItem { ProductId=x.ProductId, Name=x.Name, Price=x.Price, ImageUrl=x.ImageUrl, Size=x.Size, Color=x.Color, Quantity=x.Quantity }).ToList();
    private static readonly string[] AddressFields = [nameof(CheckoutViewModel.Cep), nameof(CheckoutViewModel.Street), nameof(CheckoutViewModel.Number), nameof(CheckoutViewModel.Complement), nameof(CheckoutViewModel.Neighborhood), nameof(CheckoutViewModel.City), nameof(CheckoutViewModel.State)];
    private static string DigitsOnly(string? value) => new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
    private static void ApplySavedCard(CheckoutViewModel vm, UserDto? profile, bool selectByDefault = false)
    {
        vm.SavedCards=profile?.Cards??new();
        vm.HasSavedCard=vm.SavedCards.Any();
        vm.SavedCardLabel=vm.SavedCards.FirstOrDefault()?.Last4;
        if(selectByDefault && vm.HasSavedCard){vm.UseSavedCard=true;vm.SelectedCardId=vm.SavedCards.First().Id;}
    }

    private static void ApplySavedAddress(CheckoutViewModel vm, UserDto profile)
    {
        vm.Cep = profile.Cep ?? string.Empty; vm.Street = profile.Street ?? string.Empty; vm.Number = profile.Number ?? string.Empty;
        vm.Complement = profile.Complement; vm.Neighborhood = profile.Neighborhood ?? string.Empty;
        vm.City = profile.City ?? string.Empty; vm.State = profile.State ?? string.Empty;
    }
    private static bool HasCompleteAddress(CheckoutViewModel vm) => !string.IsNullOrWhiteSpace(vm.Cep) && !string.IsNullOrWhiteSpace(vm.Street) && !string.IsNullOrWhiteSpace(vm.Number) && !string.IsNullOrWhiteSpace(vm.Neighborhood) && !string.IsNullOrWhiteSpace(vm.City) && vm.State.Trim().Length == 2;
    private static bool HasCompleteAddress(UserDto profile) => !string.IsNullOrWhiteSpace(profile.Cep) && !string.IsNullOrWhiteSpace(profile.Street) && !string.IsNullOrWhiteSpace(profile.Number) && !string.IsNullOrWhiteSpace(profile.Neighborhood) && !string.IsNullOrWhiteSpace(profile.City) && (profile.State?.Trim().Length ?? 0) == 2;
}
