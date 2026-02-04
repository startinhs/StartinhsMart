using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using StartinhsMart.CoreService.Orders;

namespace StartinhsMart.CoreService.Orders
{
    public class CreateOrderDto
    {
        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        [StringLength(500)]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string ShippingPhone { get; set; } = string.Empty;

        [StringLength(256)]
        public string? CustomerName { get; set; }

        [EmailAddress]
        [StringLength(256)]
        public string? CustomerEmail { get; set; }

        [StringLength(1000)]
        public string? Note { get; set; }

        [Required]
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    public class CreateOrderItemDto
    {
        [Required]
        public Guid PetId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
