using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Domain.Entities;
using JetBrains.Annotations;

namespace StartinhsMart.CoreService.Categories
{
    public class Category : FullAuditedAggregateRoot<Guid>, IMultiTenant, IHasConcurrencyStamp
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; } = null!;

        [CanBeNull]
        public virtual string? Slug { get; set; }

        [CanBeNull]
        public virtual string? Description { get; set; }

        public virtual Guid? ImageId { get; set; }

        public virtual Guid? ParentCategoryId { get; set; }

        public virtual int SortOrder { get; set; }

        public virtual bool IsActive { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

        protected Category()
        {
            ConcurrencyStamp = Guid.NewGuid().ToString("N");
        }

        public Category(
            Guid id,
            string name,
            bool isActive = true,
            string? slug = null,
            string? description = null,
            Guid? imageId = null,
            Guid? parentCategoryId = null,
            int sortOrder = 0
        ) : base(id)
        {
            Name = name;
            IsActive = isActive;
            Slug = slug;
            Description = description;
            ImageId = imageId;
            ParentCategoryId = parentCategoryId;
            SortOrder = sortOrder;
            ConcurrencyStamp = Guid.NewGuid().ToString("N");
        }
    }
}
