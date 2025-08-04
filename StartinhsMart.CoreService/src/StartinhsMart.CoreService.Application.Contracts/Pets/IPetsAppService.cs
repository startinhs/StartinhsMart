using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using StartinhsMart.CoreService.Shared;

namespace StartinhsMart.CoreService.Pets
{
    public interface IPetsAppService : IApplicationService
    {
        Task<IRemoteStreamContent> GetFileAsync(GetFileInput input);

        Task<AppFileDescriptorDto> UploadFileAsync(IRemoteStreamContent input);

        Task<PagedResultDto<PetDto>> GetListAsync(GetPetsInput input);

        Task<PetDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<PetDto> CreateAsync(PetCreateDto input);

        Task<PetDto> UpdateAsync(Guid id, PetUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(PetExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> petIds);

        Task DeleteAllAsync(GetPetsInput input);
        Task<StartinhsMart.CoreService.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}