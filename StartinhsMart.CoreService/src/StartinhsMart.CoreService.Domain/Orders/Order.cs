using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Domain.Entities;
using JetBrains.Annotations;

namespace StartinhsMart.CoreService.Orders
{
    public class Order : FullAuditedAggregateRoot<Guid>, IMultiTenant, IHasConcurrencyStamp
    {
        public virtual Guid? TenantId { get; set; }

        public virtual Guid UserId { get; set; }

        [NotNull]
        public virtual string OrderNumber { get; set; } = null!;

        public virtual decimal TotalAmount { get; set; }

        public virtual OrderStatus Status { get; set; }

        public virtual PaymentMethod PaymentMethod { get; set; }

        public virtual PaymentStatus PaymentStatus { get; set; }

        [NotNull]
        public virtual string ShippingAddress { get; set; } = null!;

        [NotNull]
        public virtual string ShippingPhone { get; set; } = null!;

        [CanBeNull]
        public virtual string? CustomerName { get; set; }

        [CanBeNull]
        public virtual string? CustomerEmail { get; set; }

        [CanBeNull]
        public virtual string? Note { get; set; }

        public virtual DateTime? ShippedDate { get; set; }

        public virtual DateTime? DeliveredDate { get; set; }

        public virtual DateTime? CancelledDate { get; set; }

        [CanBeNull]
        public virtual string? CancellationReason { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

        public virtual ICollection<OrderItem> Items { get; set; }

        protected Order()
        {
            ConcurrencyStamp = Guid.NewGuid().ToString("N");
            Items = new List<OrderItem>();
        }

        public Order(
            Guid id,
            Guid userId,
            string orderNumber,
            decimal totalAmount,
            string shippingAddress,
            string shippingPhone,
            PaymentMethod paymentMethod = PaymentMethod.Cash,
            OrderStatus status = OrderStatus.Pending,
            PaymentStatus paymentStatus = PaymentStatus.Pending,
            string? customerName = null,
            string? customerEmail = null,
            string? note = null
        ) : base(id)
        {
            UserId = userId;
            OrderNumber = orderNumber;
            TotalAmount = totalAmount;
            ShippingAddress = shippingAddress;
            ShippingPhone = shippingPhone;
            PaymentMethod = paymentMethod;
            Status = status;
            PaymentStatus = paymentStatus;
            CustomerName = customerName;
            CustomerEmail = customerEmail;
            Note = note;
            ConcurrencyStamp = Guid.NewGuid().ToString("N");
            Items = new List<OrderItem>();
        }

        public void AddItem(OrderItem item)
        {
            Items.Add(item);
        }

        public void UpdateStatus(OrderStatus newStatus)
        {
            Status = newStatus;

            if (newStatus == OrderStatus.Shipped && !ShippedDate.HasValue)
            {
                ShippedDate = DateTime.UtcNow;
            }
            else if (newStatus == OrderStatus.Delivered && !DeliveredDate.HasValue)
            {
                DeliveredDate = DateTime.UtcNow;
            }
            else if (newStatus == OrderStatus.Cancelled && !CancelledDate.HasValue)
            {
                CancelledDate = DateTime.UtcNow;
            }
        }

        public void Cancel(string reason)
        {
            Status = OrderStatus.Cancelled;
            CancelledDate = DateTime.UtcNow;
            CancellationReason = reason;
        }

        public void UpdatePaymentStatus(PaymentStatus newPaymentStatus)
        {
            PaymentStatus = newPaymentStatus;
        }
    }
}
