using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace StartinhsMart.CoreService.Categories
{
    public class CategoryUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        [StringLength(256)]
        public string Name { get; set; } = null!;

        [StringLength(256)]
        public string? Slug { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        public Guid? ImageId { get; set; }

        public Guid? ParentCategoryId { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}
