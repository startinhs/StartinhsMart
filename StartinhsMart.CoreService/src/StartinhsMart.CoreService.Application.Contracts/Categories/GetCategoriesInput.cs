using System;
using Volo.Abp.Application.Dtos;

namespace StartinhsMart.CoreService.Categories
{
    public class GetCategoriesInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public bool? IsActive { get; set; }
        public Guid? ParentCategoryId { get; set; }

        public GetCategoriesInput()
        {
            Sorting = "SortOrder, Name";
        }
    }
}
