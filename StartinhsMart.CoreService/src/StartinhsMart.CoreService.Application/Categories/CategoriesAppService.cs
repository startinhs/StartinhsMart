using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using StartinhsMart.CoreService.Permissions;
using StartinhsMart.CoreService.Shared;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;

namespace StartinhsMart.CoreService.Categories
{
    [Authorize(CoreServicePermissions.Categories.Default)]
    public class CategoriesAppService : CoreServiceAppService, ICategoriesAppService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly CategoryManager _categoryManager;
        protected IDistributedCache<CategoryDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IRepository<AppFileDescriptors.AppFileDescriptor, Guid> _appFileDescriptorRepository;
        protected IBlobContainer<CategoryFileContainer> _blobContainer;

        public CategoriesAppService(
            ICategoryRepository categoryRepository,
            CategoryManager categoryManager,
            IDistributedCache<CategoryDownloadTokenCacheItem, string> downloadTokenCache,
            IRepository<AppFileDescriptors.AppFileDescriptor, Guid> appFileDescriptorRepository,
            IBlobContainer<CategoryFileContainer> blobContainer)
        {
            _categoryRepository = categoryRepository;
            _categoryManager = categoryManager;
            _downloadTokenCache = downloadTokenCache;
            _appFileDescriptorRepository = appFileDescriptorRepository;
            _blobContainer = blobContainer;
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

        public virtual async Task<IRemoteStreamContent> GetFileAsync(GetFileInput input)
        {
            var fileDescriptor = await _appFileDescriptorRepository.GetAsync(input.FileId);
            var stream = await _blobContainer.GetAsync(fileDescriptor.Id.ToString("N"));

            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();
                return new RemoteStreamContent(new MemoryStream(fileBytes), fileDescriptor.Name, fileDescriptor.MimeType);
            }
        }

        public virtual async Task<AppFileDescriptorDto> UploadFileAsync(IRemoteStreamContent input)
        {
            var id = GuidGenerator.Create();
            var fileDescriptor = await _appFileDescriptorRepository.InsertAsync(
                new AppFileDescriptors.AppFileDescriptor(id, input.FileName, input.ContentType));

            await _blobContainer.SaveAsync(fileDescriptor.Id.ToString("N"), input.GetStream());

            return ObjectMapper.Map<AppFileDescriptors.AppFileDescriptor, AppFileDescriptorDto>(fileDescriptor);
        }

        public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new CategoryDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}
