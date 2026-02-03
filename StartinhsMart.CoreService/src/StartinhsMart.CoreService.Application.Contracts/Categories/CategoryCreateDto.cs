using System;
using System.ComponentModel.DataAnnotations;

namespace StartinhsMart.CoreService.Categories
{
    public class CategoryCreateDto
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

        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}
