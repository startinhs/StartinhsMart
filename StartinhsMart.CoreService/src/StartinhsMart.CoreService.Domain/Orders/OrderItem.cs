using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Domain.Entities;
using JetBrains.Annotations;

namespace StartinhsMart.CoreService.Orders
{
    public class OrderItem : FullAuditedEntity<Guid>, IHasConcurrencyStamp
    {
        public virtual Guid OrderId { get; set; }

        public virtual Guid PetId { get; set; }

        [NotNull]
        public virtual string PetName { get; set; } = null!;

        public virtual int Quantity { get; set; }

        public virtual decimal UnitPrice { get; set; }

        public virtual decimal TotalPrice { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

        // Navigation property
        public virtual Order? Order { get; set; }

        protected OrderItem()
        {
            ConcurrencyStamp = Guid.NewGuid().ToString("N");
        }

        public OrderItem(
            Guid id,
            Guid orderId,
            Guid petId,
            string petName,
            int quantity,
            decimal unitPrice
        ) : base(id)
        {
            OrderId = orderId;
            PetId = petId;
            PetName = petName;
            Quantity = quantity;
            UnitPrice = unitPrice;
            TotalPrice = quantity * unitPrice;
            ConcurrencyStamp = Guid.NewGuid().ToString("N");
        }

        public void UpdateQuantity(int newQuantity)
        {
            Quantity = newQuantity;
            TotalPrice = newQuantity * UnitPrice;
        }

        public void UpdatePrice(decimal newUnitPrice)
        {
            UnitPrice = newUnitPrice;
            TotalPrice = Quantity * newUnitPrice;
        }
    }
}
