using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace StartinhsMart.CoreService.Categories
{
    public interface ICategoriesAppService : IApplicationService
    {
        Task<PagedResultDto<CategoryDto>> GetListAsync(GetCategoriesInput input);
        Task<CategoryDto> GetAsync(Guid id);
        Task<CategoryDto> CreateAsync(CategoryCreateDto input);
        Task<CategoryDto> UpdateAsync(Guid id, CategoryUpdateDto input);
        Task DeleteAsync(Guid id);
        Task<List<CategoryDto>> GetChildrenAsync(Guid parentCategoryId);
    }
}
