using System;
using Volo.Abp.Application.Dtos;
using StartinhsMart.CoreService.Orders;

namespace StartinhsMart.CoreService.Orders
{
    public class GetOrdersInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }
        public string? OrderNumber { get; set; }
        public Guid? UserId { get; set; }
        public OrderStatus? Status { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public DateTime? MinCreationTime { get; set; }
        public DateTime? MaxCreationTime { get; set; }
    }
}
