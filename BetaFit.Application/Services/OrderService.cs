using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;
using BetaFit.Domain.Entities;
using BetaFit.Domain.Enums;
using BetaFit.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

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
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        Subtotal = item.UnitPrice * item.Quantity,
                        ImageUrl = product?.ImageUrl
                    });
                }

                result.Add(new OrderDto
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    UserName = await ObterNomeUsuarioAsync(order.UserId),
                    CreatedAt = order.CreatedAt,
                    Total = order.Total,
                    Status = order.Status.ToString(),
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
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        Subtotal = item.UnitPrice * item.Quantity,
                        ImageUrl = product?.ImageUrl
                    });
                }

                result.Add(new OrderDto
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    UserName = nomeUsuario,
                    CreatedAt = order.CreatedAt,
                    Total = order.Total,
                    Status = order.Status.ToString(),
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

            var itemDtos = new List<OrderItemDto>();
            foreach (var item in order.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                itemDtos.Add(new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    Subtotal = item.UnitPrice * item.Quantity,
                    ImageUrl = product?.ImageUrl
                });
            }

            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = await ObterNomeUsuarioAsync(order.UserId),
                CreatedAt = order.CreatedAt,
                Total = order.Total,
                Status = order.Status.ToString(),
                Items = itemDtos
            };
        }

        public async Task<OrderDto> CreateAsync(
            CreateOrderDto dto,
            string userId)
        {
            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.Now,
                Status = OrderStatus.Pendente,
                Items = new List<OrderItem>()
            };

            decimal total = 0;

            foreach (var itemDto in dto.Items)
            {
                var product = await _productRepository
                    .GetByIdAsync(itemDto.ProductId);

                if (product == null)
                {
                    throw new Exception(
                        $"Produto {itemDto.ProductId} não encontrado.");
                }

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = string.IsNullOrWhiteSpace(itemDto.Size) ? product.Name : $"{product.Name} · Tamanho {itemDto.Size}",
                    UnitPrice = product.Price,
                    Quantity = itemDto.Quantity
                };

                order.Items.Add(orderItem);

                total += product.Price * itemDto.Quantity;
            }

            order.Total = total;

            await _orderRepository.AddAsync(order);

            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = await ObterNomeUsuarioAsync(order.UserId),
                CreatedAt = order.CreatedAt,
                Total = order.Total,
                Status = order.Status.ToString(),

                Items = order.Items.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
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

            await _orderRepository.UpdateStatusAsync(id, newStatus);

            return true;
        }

        private async Task<string> ObterNomeUsuarioAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.UserName ?? userId;
        }
    }
}