using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;
using BetaFit.Domain.Entities;
using BetaFit.Domain.Enums;
using BetaFit.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace BetaFit.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly UserManager<IdentityUser> _userManager;    

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            UserManager<IdentityUser> userManager)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _userManager = userManager;
        }

        public async Task<List<OrderDto>> GetAllAsync()
        {
            var orders = await _orderRepository.GetAllAsync();

            var result = new List<OrderDto>();

            foreach (var order in orders)
            {
                var itemDtos = new List<OrderItemDto>();
                foreach (var item in order.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    itemDtos.Add(new OrderItemDto
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        UnitPrice = item.UnitPrice,OriginalPrice=item.OriginalPrice,
                        Quantity = item.Quantity,
                        Size = item.Size,
                        Color = item.Color,
                        Subtotal = item.UnitPrice * item.Quantity,
                        ImageUrl = item.ImageUrl ?? product?.Images?.OrderBy(i => i.SortOrder).Select(i => i.Url).FirstOrDefault() ?? product?.ImageUrl
                    });
                }

                result.Add(new OrderDto
                {
                    TrackingCode=order.TrackingCode,TrackingDescription=order.TrackingDescription,DeliveredAt=order.DeliveredAt,ExperienceRating=order.ExperienceRating,ExperienceComment=order.ExperienceComment,ReviewCoupon=order.ReviewCoupon,CouponCode=order.CouponCode,Discount=order.Discount,BoletoDigits=order.BoletoDigits, BoletoDueAt=order.BoletoDueAt, Installments=order.Installments, Id = order.Id,
                    UserId = order.UserId,
                    CustomerCpf = order.CustomerCpf, ShippingCep = order.ShippingCep, ShippingStreet = order.ShippingStreet, ShippingNumber = order.ShippingNumber, ShippingComplement = order.ShippingComplement, ShippingNeighborhood = order.ShippingNeighborhood, ShippingCity = order.ShippingCity, ShippingState = order.ShippingState,
                    UserName = await ObterNomeUsuarioAsync(order.UserId),
                    CreatedAt = order.CreatedAt,
                    Total = order.Total,
                    Status = order.Status.ToString(),
                    PaymentId = order.PaymentId,
                    PaymentMethod = order.PaymentMethod,
                    PaymentStatus = order.PaymentStatus,
                    Items = itemDtos
                });
            }

            return result;
        }

        public async Task<IEnumerable<OrderDto>> GetByUserIdAsync(string userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);
            var nomeUsuario = await ObterNomeUsuarioAsync(userId);

            var result = new List<OrderDto>();
            foreach (var order in orders)
            {
                var itemDtos = new List<OrderItemDto>();
                foreach (var item in order.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    itemDtos.Add(new OrderItemDto
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        UnitPrice = item.UnitPrice,OriginalPrice=item.OriginalPrice,
                        Quantity = item.Quantity,
                        Size = item.Size,
                        Color = item.Color,
                        Subtotal = item.UnitPrice * item.Quantity,
                        ImageUrl = item.ImageUrl ?? product?.Images?.OrderBy(i => i.SortOrder).Select(i => i.Url).FirstOrDefault() ?? product?.ImageUrl
                    });
                }

                result.Add(new OrderDto
                {
                    TrackingCode=order.TrackingCode,TrackingDescription=order.TrackingDescription,DeliveredAt=order.DeliveredAt,ExperienceRating=order.ExperienceRating,ExperienceComment=order.ExperienceComment,ReviewCoupon=order.ReviewCoupon,CouponCode=order.CouponCode,Discount=order.Discount,BoletoDigits=order.BoletoDigits, BoletoDueAt=order.BoletoDueAt, Installments=order.Installments, Id = order.Id,
                    UserId = order.UserId,
                    CustomerCpf = order.CustomerCpf, ShippingCep = order.ShippingCep, ShippingStreet = order.ShippingStreet, ShippingNumber = order.ShippingNumber, ShippingComplement = order.ShippingComplement, ShippingNeighborhood = order.ShippingNeighborhood, ShippingCity = order.ShippingCity, ShippingState = order.ShippingState,
                    UserName = nomeUsuario,
                    CreatedAt = order.CreatedAt,
                    Total = order.Total,
                    Status = order.Status.ToString(),
                    PaymentId = order.PaymentId,
                    PaymentMethod = order.PaymentMethod,
                    PaymentStatus = order.PaymentStatus,
                    Items = itemDtos
                });
            }

            return result;
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
                return null;

            if(order.PaymentMethod.Contains("Boleto",StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(order.BoletoDigits))
            {order.BoletoDigits=DemoBoleto.Create(order.Total);order.BoletoDueAt=order.CreatedAt.Date.AddDays(3);await _orderRepository.UpdateAsync(order);}

            var itemDtos = new List<OrderItemDto>();
            foreach (var item in order.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                itemDtos.Add(new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,OriginalPrice=item.OriginalPrice,
                    Quantity = item.Quantity,
                    Size = item.Size,
                    Color = item.Color,
                    Subtotal = item.UnitPrice * item.Quantity,
                    ImageUrl = item.ImageUrl ?? product?.Images?.OrderBy(i => i.SortOrder).Select(i => i.Url).FirstOrDefault() ?? product?.ImageUrl
                });
            }

            return new OrderDto
            {
                TrackingCode=order.TrackingCode,TrackingDescription=order.TrackingDescription,DeliveredAt=order.DeliveredAt,ExperienceRating=order.ExperienceRating,ExperienceComment=order.ExperienceComment,ReviewCoupon=order.ReviewCoupon,CouponCode=order.CouponCode,Discount=order.Discount,BoletoDigits=order.BoletoDigits, BoletoDueAt=order.BoletoDueAt, Installments=order.Installments, Id = order.Id,
                UserId = order.UserId,
                CustomerCpf = order.CustomerCpf, ShippingCep = order.ShippingCep, ShippingStreet = order.ShippingStreet, ShippingNumber = order.ShippingNumber, ShippingComplement = order.ShippingComplement, ShippingNeighborhood = order.ShippingNeighborhood, ShippingCity = order.ShippingCity, ShippingState = order.ShippingState,
                UserName = await ObterNomeUsuarioAsync(order.UserId),
                CreatedAt = order.CreatedAt,
                Total = order.Total,
                Status = order.Status.ToString(),
                PaymentId = order.PaymentId,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                Items = itemDtos
            };
        }

        public async Task<OrderDto> CreateAsync(
            CreateOrderDto dto,
            string userId)
        {
            SavedCardDto? selectedCard=null;
            if(dto.PaymentMethod is "Credito" or "Debito")
            {
                var user=await _userManager.FindByIdAsync(userId)??throw new InvalidOperationException("Usuário não encontrado.");
                selectedCard=DemoWallet.Read(await _userManager.GetClaimsAsync(user)).FirstOrDefault(c=>c.Id==dto.SavedCardId);
                if(selectedCard is null || selectedCard.Type!=dto.PaymentMethod || !DemoWallet.ValidExpiry(selectedCard.Expiry))throw new InvalidOperationException("Selecione um cartão válido cadastrado em sua conta para esta forma de pagamento.");
            }
            if(dto.Installments<1 || dto.Installments>12 || dto.PaymentMethod!="Credito" && dto.Installments!=1)throw new InvalidOperationException("Parcelamento inválido.");
            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.Now,
                Status = OrderStatus.Pendente,
                PaymentMethod = NormalizePaymentMethod(dto.PaymentMethod),
                PaymentStatus = "Aguardando",
                PaymentId = selectedCard is null ? null : $"{selectedCard.Brand} final {selectedCard.Last4}",
                Installments=dto.Installments,
                CustomerCpf = dto.CustomerCpf, ShippingCep = dto.ShippingCep, ShippingStreet = dto.ShippingStreet, ShippingNumber = dto.ShippingNumber, ShippingComplement = dto.ShippingComplement, ShippingNeighborhood = dto.ShippingNeighborhood, ShippingCity = dto.ShippingCity, ShippingState = dto.ShippingState,
                Items = new List<OrderItem>()
            };

            decimal total = 0;

            if (dto.Items is null || dto.Items.Count == 0) throw new InvalidOperationException("O pedido precisa ter pelo menos um item.");
            var requestedByProduct = dto.Items.GroupBy(x => x.ProductId).ToDictionary(g => g.Key, g => g.Sum(x => Math.Clamp(x.Quantity, 1, 99)));
            // O checkout é demonstrativo: exigir 11 dígitos evita dados vazios,
            // sem obrigar o usuário a informar um CPF real para uma apresentação.
            if (string.IsNullOrWhiteSpace(dto.CustomerCpf) || !IsValidDemoCpf(dto.CustomerCpf)) throw new InvalidOperationException("Informe um CPF de teste com 11 dígitos.");
            if (string.IsNullOrWhiteSpace(dto.ShippingCep) || string.IsNullOrWhiteSpace(dto.ShippingStreet) || string.IsNullOrWhiteSpace(dto.ShippingNumber) || string.IsNullOrWhiteSpace(dto.ShippingNeighborhood) || string.IsNullOrWhiteSpace(dto.ShippingCity) || string.IsNullOrWhiteSpace(dto.ShippingState)) throw new InvalidOperationException("Informe o endereço completo.");

            foreach (var itemDto in dto.Items)
            {
                var product = await _productRepository
                    .GetByIdAsync(itemDto.ProductId);

                if (product == null)
                {
                    throw new InvalidOperationException($"Produto {itemDto.ProductId} não encontrado.");
                }
                if(!product.IsActive)throw new InvalidOperationException($"Produto {product.Name} indisponível.");
                if (product.Stock < requestedByProduct[product.Id])
                    throw new InvalidOperationException($"Estoque insuficiente para o produto {product.Name}. Disponível: {product.Stock}.");

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.SalePrice??product.Price,OriginalPrice=product.Price,ImageUrl=product.ImageUrl,
                    Quantity = Math.Clamp(itemDto.Quantity, 1, 99),
                    Size = string.IsNullOrWhiteSpace(itemDto.Size) ? null : itemDto.Size.Trim()
                    ,Color = string.IsNullOrWhiteSpace(itemDto.Color) ? null : itemDto.Color.Trim()
                };

                var availableSizes = JsonSerializer.Deserialize<List<string>>(product.AvailableSizesJson) ?? new List<string>();
                if (availableSizes.Any() && string.IsNullOrWhiteSpace(orderItem.Size))
                    throw new InvalidOperationException($"Selecione um tamanho para o produto {product.Name}.");
                if (availableSizes.Any() && !availableSizes.Contains(orderItem.Size!, StringComparer.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"O tamanho {orderItem.Size} não está disponível para o produto {product.Name}.");
                var availableColors = JsonSerializer.Deserialize<List<string>>(product.AvailableColorsJson) ?? new List<string>();
                if (availableColors.Any() && string.IsNullOrWhiteSpace(orderItem.Color))
                    throw new InvalidOperationException($"Selecione uma cor para o produto {product.Name}.");
                if (availableColors.Any() && !availableColors.Contains(orderItem.Color!, StringComparer.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"A cor {orderItem.Color} não está disponível para o produto {product.Name}.");

                var variants=VariantInventory.Read(product);var variant=VariantInventory.Find(variants,orderItem.Size,orderItem.Color);
                var requested=dto.Items.Where(i=>i.ProductId==product.Id&&string.Equals(i.Size??"",orderItem.Size??"",StringComparison.OrdinalIgnoreCase)&&string.Equals(i.Color??"",orderItem.Color??"",StringComparison.OrdinalIgnoreCase)).Sum(i=>i.Quantity);
                if(variants.Count>0&&(variant==null||variant.Stock<requested))throw new InvalidOperationException("Estoque insuficiente para a variação selecionada de "+product.Name);
                order.Items.Add(orderItem);
                total += orderItem.UnitPrice * orderItem.Quantity;
            }

            if(!string.IsNullOrWhiteSpace(dto.CouponCode)){
                var owner=await _userManager.FindByIdAsync(userId)??throw new InvalidOperationException("Usuário inválido.");var coupon=(await _userManager.GetClaimsAsync(owner)).FirstOrDefault(c=>c.Type=="ReviewCoupon"&&c.Value==dto.CouponCode.Trim());
                if(coupon==null)throw new InvalidOperationException("Cupom inválido ou já utilizado.");order.CouponCode=coupon.Value;order.Discount=Math.Round(total*.05m,2);total-=order.Discount;await _userManager.RemoveClaimAsync(owner,coupon);
            }
            foreach(var item in order.Items){var p=await _productRepository.GetByIdAsync(item.ProductId);VariantInventory.Change(p!,item.Size,item.Color,-item.Quantity);}
            order.Total = total;
            if(dto.PaymentMethod=="Boleto") {order.BoletoDigits=DemoBoleto.Create(total);order.BoletoDueAt=DateTime.Today.AddDays(3); }

            await _orderRepository.AddAsync(order);

            return new OrderDto
            {
                TrackingCode=order.TrackingCode,TrackingDescription=order.TrackingDescription,DeliveredAt=order.DeliveredAt,ExperienceRating=order.ExperienceRating,ExperienceComment=order.ExperienceComment,ReviewCoupon=order.ReviewCoupon,CouponCode=order.CouponCode,Discount=order.Discount,BoletoDigits=order.BoletoDigits, BoletoDueAt=order.BoletoDueAt, Installments=order.Installments, Id = order.Id,
                UserId = order.UserId,
                CustomerCpf = order.CustomerCpf, ShippingCep = order.ShippingCep, ShippingStreet = order.ShippingStreet, ShippingNumber = order.ShippingNumber, ShippingComplement = order.ShippingComplement, ShippingNeighborhood = order.ShippingNeighborhood, ShippingCity = order.ShippingCity, ShippingState = order.ShippingState,
                UserName = await ObterNomeUsuarioAsync(order.UserId),
                CreatedAt = order.CreatedAt,
                Total = order.Total,
                Status = order.Status.ToString(),
                PaymentId = order.PaymentId,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                Items = order.Items.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,OriginalPrice=item.OriginalPrice,
                    Quantity = item.Quantity,
                    Size = item.Size,
                    Color = item.Color,
                    Subtotal = item.UnitPrice * item.Quantity
                }).ToList()
            };
        }

        public async Task<bool> UpdateStatusAsync(
            int id,
            string status)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
                return false;

            if (!Enum.TryParse<OrderStatus>(
                    status,
                    true,
                    out var newStatus))
            {
                return false;
            }

            if (!Enum.IsDefined(newStatus) || order.Status is OrderStatus.Cancelado or OrderStatus.Reembolso)
                return false;
            if(order.Status==newStatus)return true;
            if (newStatus == OrderStatus.Reembolso && order.Status != OrderStatus.Entregue) return false;
            if (order.Status == OrderStatus.Entregue && newStatus != OrderStatus.Reembolso) return false;
            static int Stage(OrderStatus value) => value switch
            {
                OrderStatus.Pendente => 0, OrderStatus.Confirmado => 1,
                OrderStatus.EmPreparacao => 2, OrderStatus.Pronto => 3,
                OrderStatus.Enviado => 4, OrderStatus.Entregue => 5, _ => 6
            };
            if (newStatus != OrderStatus.Cancelado && Stage(newStatus) < Stage(order.Status)) return false;

            if (newStatus == OrderStatus.Cancelado)
            {
                foreach (var item in order.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product is not null) VariantInventory.Change(product,item.Size,item.Color,item.Quantity);
                }
                order.Status = OrderStatus.Cancelado;
                order.PaymentStatus = "Cancelado";
                await _orderRepository.UpdateAsync(order);
                return true;
            }

            order.Status=newStatus;
            if(newStatus is OrderStatus.Confirmado or OrderStatus.EmPreparacao or OrderStatus.Pronto or OrderStatus.Enviado or OrderStatus.Entregue)order.PaymentStatus="Pago (demonstração)";
            if(newStatus==OrderStatus.Entregue){order.DeliveredAt=DateTime.Now;order.TrackingDescription="Entrega concluída pela loja.";}
            await _orderRepository.UpdateAsync(order);

            return true;
        }

        public async Task<(bool Ok, string Message)> CancelAsync(int id, string userId)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order is null || !string.Equals(order.UserId, userId, StringComparison.Ordinal))
                return (false, "Pedido não encontrado.");
            if (order.Status is not (OrderStatus.Pendente or OrderStatus.Confirmado))
                return (false, "Este pedido não pode mais ser cancelado.");

            foreach (var item in order.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product is not null) VariantInventory.Change(product,item.Size,item.Color,item.Quantity);
            }

            order.Status = OrderStatus.Cancelado;
            order.PaymentStatus = "Cancelado";
            await _orderRepository.UpdateAsync(order);
            return (true, "Pedido cancelado e estoque devolvido.");
        }

        public async Task<(bool Ok, string Message)> UpdateDeliveryAsync(int id, string userId, UpdateOrderDeliveryDto dto)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order is null || !string.Equals(order.UserId, userId, StringComparison.Ordinal)) return (false, "Pedido não encontrado.");
            if (order.Status is not (OrderStatus.Pendente or OrderStatus.Confirmado)) return (false, "O endereço só pode ser alterado antes da preparação do pedido.");
            order.ShippingCep = Digits(dto.ShippingCep);
            order.ShippingStreet = dto.ShippingStreet.Trim();
            order.ShippingNumber = dto.ShippingNumber.Trim();
            order.ShippingComplement = dto.ShippingComplement?.Trim();
            order.ShippingNeighborhood = dto.ShippingNeighborhood.Trim();
            order.ShippingCity = dto.ShippingCity.Trim();
            order.ShippingState = dto.ShippingState.Trim().ToUpperInvariant();
            await _orderRepository.UpdateAsync(order);
            return (true, "Endereço do pedido atualizado.");
        }

        public async Task<(bool Ok, string Message)> ConfirmDemoPaymentAsync(int id, string userId)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order is null || !string.Equals(order.UserId, userId, StringComparison.Ordinal)) return (false, "Pedido não encontrado.");
            if (order.Status is OrderStatus.Cancelado or OrderStatus.Reembolso) return (false, "Este pedido foi cancelado ou está em reembolso.");
            order.PaymentStatus = "Pago (demonstração)";
            if (order.Status == OrderStatus.Pendente) order.Status = OrderStatus.Confirmado;
            await _orderRepository.UpdateAsync(order);
            return (true, "Transação concluída com sucesso.");
        }

        private static string NormalizePaymentMethod(string? method) => method?.Trim().ToUpperInvariant() switch
        {
            "PIX" => "Pix demonstrativo",
            "CREDITO" => "Cartão de crédito demonstrativo",
            "DEBITO" => "Cartão de débito demonstrativo",
            "CARTAO" => "Cartão demonstrativo",
            "BOLETO" => "Boleto demonstrativo",
            _ => throw new InvalidOperationException("Escolha uma forma de pagamento válida.")
        };

        private static bool IsValidDemoCpf(string? value)
        {
            var cpf = new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
            return cpf.Length == 11;
        }

        private static string Digits(string? value) => new((value ?? string.Empty).Where(char.IsDigit).ToArray());

        private async Task<string> ObterNomeUsuarioAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.UserName ?? userId;
        }
    }
}
