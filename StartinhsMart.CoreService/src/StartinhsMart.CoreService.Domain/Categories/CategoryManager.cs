using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace StartinhsMart.CoreService.Categories
{
    public class CategoryManager : DomainService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryManager(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Category> CreateAsync(
            string name,
            bool isActive = true,
            string? slug = null,
            string? description = null,
            Guid? imageId = null,
            Guid? parentCategoryId = null,
            int sortOrder = 0)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            // Validate parent category exists if provided
            if (parentCategoryId.HasValue)
            {
                var parentExists = await _categoryRepository.AnyAsync(parentCategoryId.Value);
                if (!parentExists)
                {
                    throw new BusinessException(CoreServiceDomainErrorCodes.CategoryParentNotFound)
                        .WithData("parentCategoryId", parentCategoryId.Value);
                }
            }

            var category = new Category(
                GuidGenerator.Create(),
                name,
                isActive,
                slug,
                description,
                imageId,
                parentCategoryId,
                sortOrder
            );

            return await _categoryRepository.InsertAsync(category);
        }

        public async Task<Category> UpdateAsync(
            Guid id,
            string name,
            bool isActive,
            string? slug = null,
            string? description = null,
            Guid? imageId = null,
            Guid? parentCategoryId = null,
            int sortOrder = 0,
            string? concurrencyStamp = null)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var category = await _categoryRepository.GetAsync(id);

            // Prevent circular reference
            if (parentCategoryId.HasValue && parentCategoryId.Value == id)
            {
                throw new BusinessException(CoreServiceDomainErrorCodes.CategoryCircularReference)
                    .WithData("categoryId", id);
            }

            // Validate parent category exists if provided
            if (parentCategoryId.HasValue && parentCategoryId.Value != category.ParentCategoryId)
            {
                var parentExists = await _categoryRepository.AnyAsync(parentCategoryId.Value);
                if (!parentExists)
                {
                    throw new BusinessException(CoreServiceDomainErrorCodes.CategoryParentNotFound)
                        .WithData("parentCategoryId", parentCategoryId.Value);
                }
            }

            category.Name = name;
            category.IsActive = isActive;
            category.Slug = slug;
            category.Description = description;
            category.ImageId = imageId;
            category.ParentCategoryId = parentCategoryId;
            category.SortOrder = sortOrder;

            if (!string.IsNullOrWhiteSpace(concurrencyStamp))
            {
                category.ConcurrencyStamp = concurrencyStamp;
            }

            return await _categoryRepository.UpdateAsync(category);
        }
    }
}
