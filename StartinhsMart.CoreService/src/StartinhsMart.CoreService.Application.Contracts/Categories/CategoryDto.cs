using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace StartinhsMart.CoreService.Categories
{
    public class CategoryDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Name { get; set; } = null!;
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public Guid? ImageId { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public string ConcurrencyStamp { get; set; } = null!;
    }
}
