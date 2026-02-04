using System;
using StartinhsMart.CoreService.Shared;
using Volo.Abp.AutoMapper;
using StartinhsMart.CoreService.Pets;
using StartinhsMart.CoreService.Categories;
using StartinhsMart.CoreService.Orders;
using AutoMapper;

namespace StartinhsMart.CoreService;

public class CoreServiceApplicationAutoMapperProfile : Profile
{
    public CoreServiceApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<Pet, PetDto>();
        CreateMap<Pet, PetExcelDto>();
        CreateMap<PetDto, PetUpdateDto>();
        CreateMap<AppFileDescriptors.AppFileDescriptor, AppFileDescriptorDto>();

        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryDto, CategoryUpdateDto>();

        CreateMap<Order, OrderDto>();
        CreateMap<OrderItem, OrderItemDto>();
    }
}