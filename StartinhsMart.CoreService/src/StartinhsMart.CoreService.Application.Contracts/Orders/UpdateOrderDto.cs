using System;
using System.ComponentModel.DataAnnotations;
using StartinhsMart.CoreService.Orders;

namespace StartinhsMart.CoreService.Orders
{
    public class UpdateOrderDto
    {
        [StringLength(500)]
        public string? ShippingAddress { get; set; }

        [StringLength(20)]
        public string? ShippingPhone { get; set; }

        [StringLength(256)]
        public string? CustomerName { get; set; }

        [EmailAddress]
        [StringLength(256)]
        public string? CustomerEmail { get; set; }

        [StringLength(1000)]
        public string? Note { get; set; }

        public OrderStatus? Status { get; set; }

        public PaymentStatus? PaymentStatus { get; set; }
    }
}
