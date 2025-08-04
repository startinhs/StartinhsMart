using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using StartinhsMart.CoreService.Permissions;
using StartinhsMart.CoreService.Pets;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using StartinhsMart.CoreService.Shared;
using Volo.Abp.BlobStoring;

namespace StartinhsMart.CoreService.Pets
{

    [Authorize(CoreServicePermissions.Pets.Default)]
    public class PetsAppService : CoreServiceAppService, IPetsAppService
    {
        protected IDistributedCache<PetDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IPetRepository _petRepository;
        protected PetManager _petManager;
        protected IRepository<AppFileDescriptors.AppFileDescriptor, Guid> _appFileDescriptorRepository;
        protected IBlobContainer<PetFileContainer> _blobContainer;

        public PetsAppService(IPetRepository petRepository, PetManager petManager, IDistributedCache<PetDownloadTokenCacheItem, string> downloadTokenCache, IRepository<AppFileDescriptors.AppFileDescriptor, Guid> appFileDescriptorRepository, IBlobContainer<PetFileContainer> blobContainer)
        {
            _downloadTokenCache = downloadTokenCache;
            _petRepository = petRepository;
            _petManager = petManager;
            _appFileDescriptorRepository = appFileDescriptorRepository;
            _blobContainer = blobContainer;
        }

        public virtual async Task<PagedResultDto<PetDto>> GetListAsync(GetPetsInput input)
        {
            var totalCount = await _petRepository.GetCountAsync(input.FilterText, input.Category, input.Name, input.Breed, input.AgeMin, input.AgeMax, input.Gender, input.Color, input.WeightMin, input.WeightMax, input.HealthStatus, input.VaccinationsMin, input.VaccinationsMax, input.Description, input.PriceMin, input.PriceMax, input.QuantityMin, input.QuantityMax, input.IsBooth, input.IsStock);
            var items = await _petRepository.GetListAsync(input.FilterText, input.Category, input.Name, input.Breed, input.AgeMin, input.AgeMax, input.Gender, input.Color, input.WeightMin, input.WeightMax, input.HealthStatus, input.VaccinationsMin, input.VaccinationsMax, input.Description, input.PriceMin, input.PriceMax, input.QuantityMin, input.QuantityMax, input.IsBooth, input.IsStock, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<PetDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Pet>, List<PetDto>>(items)
            };
        }

        public virtual async Task<PetDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Pet, PetDto>(await _petRepository.GetAsync(id));
        }

        [Authorize(CoreServicePermissions.Pets.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _petRepository.DeleteAsync(id);
        }

        [Authorize(CoreServicePermissions.Pets.Create)]
        public virtual async Task<PetDto> CreateAsync(PetCreateDto input)
        {

            var pet = await _petManager.CreateAsync(
            input.IsBooth, input.IsStock, input.ImageId, input.Category, input.Name, input.Breed, input.Age, input.Gender, input.Color, input.Weight, input.HealthStatus, input.Vaccinations, input.Description, input.Price, input.Quantity
            );

            return ObjectMapper.Map<Pet, PetDto>(pet);
        }

        [Authorize(CoreServicePermissions.Pets.Edit)]
        public virtual async Task<PetDto> UpdateAsync(Guid id, PetUpdateDto input)
        {

            var pet = await _petManager.UpdateAsync(
            id,
            input.IsBooth, input.IsStock, input.ImageId, input.Category, input.Name, input.Breed, input.Age, input.Gender, input.Color, input.Weight, input.HealthStatus, input.Vaccinations, input.Description, input.Price, input.Quantity, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Pet, PetDto>(pet);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(PetExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _petRepository.GetListAsync(input.FilterText, input.Category, input.Name, input.Breed, input.AgeMin, input.AgeMax, input.Gender, input.Color, input.WeightMin, input.WeightMax, input.HealthStatus, input.VaccinationsMin, input.VaccinationsMax, input.Description, input.PriceMin, input.PriceMax, input.QuantityMin, input.QuantityMax, input.IsBooth, input.IsStock);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<Pet>, List<PetExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Pets.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(CoreServicePermissions.Pets.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> petIds)
        {
            await _petRepository.DeleteManyAsync(petIds);
        }

        [Authorize(CoreServicePermissions.Pets.Delete)]
        public virtual async Task DeleteAllAsync(GetPetsInput input)
        {
            await _petRepository.DeleteAllAsync(input.FilterText, input.Category, input.Name, input.Breed, input.AgeMin, input.AgeMax, input.Gender, input.Color, input.WeightMin, input.WeightMax, input.HealthStatus, input.VaccinationsMin, input.VaccinationsMax, input.Description, input.PriceMin, input.PriceMax, input.QuantityMin, input.QuantityMax, input.IsBooth, input.IsStock);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetFileAsync(GetFileInput input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var fileDescriptor = await _appFileDescriptorRepository.GetAsync(input.FileId);
            var stream = await _blobContainer.GetAsync(fileDescriptor.Id.ToString("N"));

            return new RemoteStreamContent(stream, fileDescriptor.Name, fileDescriptor.MimeType);
        }

        public virtual async Task<AppFileDescriptorDto> UploadFileAsync(IRemoteStreamContent input)
        {
            var id = GuidGenerator.Create();
            var fileDescriptor = await _appFileDescriptorRepository.InsertAsync(new AppFileDescriptors.AppFileDescriptor(id, input.FileName, input.ContentType));

            await _blobContainer.SaveAsync(fileDescriptor.Id.ToString("N"), input.GetStream());

            return ObjectMapper.Map<AppFileDescriptors.AppFileDescriptor, AppFileDescriptorDto>(fileDescriptor);
        }

        public virtual async Task<StartinhsMart.CoreService.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new PetDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new StartinhsMart.CoreService.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}