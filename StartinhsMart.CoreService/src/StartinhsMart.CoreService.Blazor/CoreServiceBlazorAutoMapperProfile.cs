using Volo.Abp.AutoMapper;
using StartinhsMart.CoreService.Pets;
using AutoMapper;

namespace StartinhsMart.CoreService.Blazor;

public class CoreServiceBlazorAutoMapperProfile : Profile
{
    public CoreServiceBlazorAutoMapperProfile()
    {
        //Define your AutoMapper configuration here for the Blazor project.

        CreateMap<PetDto, PetUpdateDto>();
    }
}