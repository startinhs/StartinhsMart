using Volo.Abp.AutoMapper;
using StartinhsMart.CoreService.Pets;
using StartinhsMart.CoreService.Categories;
using AutoMapper;

namespace StartinhsMart.CoreService.Blazor.Client;

public class CoreServiceBlazorAutoMapperProfile : Profile
{
    public CoreServiceBlazorAutoMapperProfile()
    {
        //Define your AutoMapper configuration here for the Blazor project.

        CreateMap<PetDto, PetUpdateDto>();
        CreateMap<CategoryDto, CategoryUpdateDto>();
    }
}