using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp;

namespace StartinhsMart.CoreService.Orders
{
    public class OrderManager : DomainService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderManager(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Order> CreateAsync(
            Guid userId,
            string shippingAddress,
            string shippingPhone,
            PaymentMethod paymentMethod,
            string? customerName = null,
            string? customerEmail = null,
            string? note = null
        )
        {
            // Generate unique order number
            var orderNumber = await GenerateOrderNumberAsync();

            var order = new Order(
                GuidGenerator.Create(),
                userId,
                orderNumber,
                0, // Will be calculated when items are added
                shippingAddress,
                shippingPhone,
                paymentMethod,
                OrderStatus.Pending,
                PaymentStatus.Pending,
                customerName,
                customerEmail,
                note
            );

            return order;
        }

        public async Task<string> GenerateOrderNumberAsync()
        {
            // Format: ORD-YYYYMMDD-XXXXX
            var date = DateTime.UtcNow;
            var dateStr = date.ToString("yyyyMMdd");
            var random = new Random();
            var randomNumber = random.Next(10000, 99999);
            
            var orderNumber = $"ORD-{dateStr}-{randomNumber}";

            // Check if order number already exists (very rare)
            var existingOrder = await _orderRepository.FindAsync(o => o.OrderNumber == orderNumber);
            if (existingOrder != null)
            {
                // Recursively generate a new one
                return await GenerateOrderNumberAsync();
            }

            return orderNumber;
        }

        public void CalculateOrderTotal(Order order)
        {
            decimal total = 0;
            foreach (var item in order.Items)
            {
                total += item.TotalPrice;
            }
            order.TotalAmount = total;
        }

        public void ValidateStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            // Define valid status transitions
            var validTransitions = new System.Collections.Generic.Dictionary<OrderStatus, OrderStatus[]>
            {
                { OrderStatus.Pending, new[] { OrderStatus.Confirmed, OrderStatus.Cancelled } },
                { OrderStatus.Confirmed, new[] { OrderStatus.Processing, OrderStatus.Cancelled } },
                { OrderStatus.Processing, new[] { OrderStatus.Shipped, OrderStatus.Cancelled } },
                { OrderStatus.Shipped, new[] { OrderStatus.Delivered, OrderStatus.Cancelled } },
                { OrderStatus.Delivered, new[] { OrderStatus.Refunded } },
                { OrderStatus.Cancelled, System.Array.Empty<OrderStatus>() },
                { OrderStatus.Refunded, System.Array.Empty<OrderStatus>() }
            };

            if (!validTransitions.ContainsKey(currentStatus))
            {
                throw new BusinessException(CoreServiceDomainErrorCodes.InvalidOrderStatus)
                    .WithData("CurrentStatus", currentStatus);
            }

            if (!System.Array.Exists(validTransitions[currentStatus], s => s == newStatus))
            {
                throw new BusinessException(CoreServiceDomainErrorCodes.InvalidOrderStatusTransition)
                    .WithData("CurrentStatus", currentStatus)
                    .WithData("NewStatus", newStatus);
            }
        }
    }
}
