using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using StartinhsMart.CoreService.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace StartinhsMart.CoreService.Categories
{
    [Authorize(CoreServicePermissions.Categories.Default)]
    public class CategoriesAppService : CoreServiceAppService, ICategoriesAppService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly CategoryManager _categoryManager;

        public CategoriesAppService(
            ICategoryRepository categoryRepository,
            CategoryManager categoryManager)
        {
            _categoryRepository = categoryRepository;
            _categoryManager = categoryManager;
        }

        public virtual async Task<PagedResultDto<CategoryDto>> GetListAsync(GetCategoriesInput input)
        {
            var totalCount = await _categoryRepository.GetCountAsync(
                input.FilterText,
                input.Name,
                input.Slug,
                input.IsActive,
                input.ParentCategoryId
            );

            var items = await _categoryRepository.GetListAsync(
                input.FilterText,
                input.Name,
                input.Slug,
                input.IsActive,
                input.ParentCategoryId,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount
            );

            return new PagedResultDto<CategoryDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Category>, List<CategoryDto>>(items)
            };
        }

        public virtual async Task<CategoryDto> GetAsync(Guid id)
        {
            var category = await _categoryRepository.GetAsync(id);
            return ObjectMapper.Map<Category, CategoryDto>(category);
        }

        [Authorize(CoreServicePermissions.Categories.Create)]
        public virtual async Task<CategoryDto> CreateAsync(CategoryCreateDto input)
        {
            var category = await _categoryManager.CreateAsync(
                input.Name,
                input.IsActive,
                input.Slug,
                input.Description,
                input.ImageId,
                input.ParentCategoryId,
                input.SortOrder
            );

            return ObjectMapper.Map<Category, CategoryDto>(category);
        }

        [Authorize(CoreServicePermissions.Categories.Edit)]
        public virtual async Task<CategoryDto> UpdateAsync(Guid id, CategoryUpdateDto input)
        {
            var category = await _categoryManager.UpdateAsync(
                id,
                input.Name,
                input.IsActive,
                input.Slug,
                input.Description,
                input.ImageId,
                input.ParentCategoryId,
                input.SortOrder,
                input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Category, CategoryDto>(category);
        }

        [Authorize(CoreServicePermissions.Categories.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            // Check if category has children
            var children = await _categoryRepository.GetChildrenAsync(id);
            if (children.Any())
            {
                throw new Volo.Abp.BusinessException(CoreServiceDomainErrorCodes.CategoryNotFound)
                    .WithData("message", "Cannot delete category with children");
            }

            await _categoryRepository.DeleteAsync(id);
        }

        public virtual async Task<List<CategoryDto>> GetChildrenAsync(Guid parentCategoryId)
        {
            var children = await _categoryRepository.GetChildrenAsync(parentCategoryId);
            return ObjectMapper.Map<List<Category>, List<CategoryDto>>(children);
        }
    }
}
