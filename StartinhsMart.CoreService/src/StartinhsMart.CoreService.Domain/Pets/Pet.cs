using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities;
using Volo.Abp;

namespace StartinhsMart.CoreService.Pets
{
    public class Pet : AuditedEntity<Guid>, IMultiTenant, IHasConcurrencyStamp
    {
        public virtual Guid? TenantId { get; set; }

        public virtual Guid? ImageId { get; set; }

        public virtual Guid? CategoryId { get; set; }

        [CanBeNull]
        public virtual string? Name { get; set; }

        [CanBeNull]
        public virtual string? Breed { get; set; }

        public virtual float? Age { get; set; }

        [CanBeNull]
        public virtual string? Gender { get; set; }

        [CanBeNull]
        public virtual string? Color { get; set; }

        public virtual float? Weight { get; set; }

        [CanBeNull]
        public virtual string? HealthStatus { get; set; }

        public virtual int? Vaccinations { get; set; }

        [CanBeNull]
        public virtual string? Description { get; set; }

        public virtual decimal? Price { get; set; }

        public virtual int? Quantity { get; set; }

        public virtual bool IsBooth { get; set; }

        public virtual bool IsStock { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

        protected Pet()
        {
            ConcurrencyStamp = Guid.NewGuid().ToString("N");
        }

        public Pet(Guid id, bool isBooth, bool isStock, Guid? imageId = null, Guid? categoryId = null, string? name = null, string? breed = null, float? age = null, string? gender = null, string? color = null, float? weight = null, string? healthStatus = null, int? vaccinations = null, string? description = null, decimal? price = null, int? quantity = null)
        {
            ConcurrencyStamp = Guid.NewGuid().ToString("N");
            Id = id;
            IsBooth = isBooth;
            IsStock = isStock;
            ImageId = imageId;
            CategoryId = categoryId;
            Name = name;
            Breed = breed;
            Age = age;
            Gender = gender;
            Color = color;
            Weight = weight;
            HealthStatus = healthStatus;
            Vaccinations = vaccinations;
            Description = description;
            Price = price;
            Quantity = quantity;
        }

    }
}