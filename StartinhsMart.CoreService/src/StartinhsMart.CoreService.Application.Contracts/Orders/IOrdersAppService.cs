using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace StartinhsMart.CoreService.Orders
{
    public interface IOrdersAppService : IApplicationService
    {
        Task<PagedResultDto<OrderDto>> GetListAsync(GetOrdersInput input);

        Task<OrderDto> GetAsync(Guid id);

        Task<OrderDto> CreateAsync(CreateOrderDto input);

        Task<OrderDto> UpdateAsync(Guid id, UpdateOrderDto input);

        Task DeleteAsync(Guid id);

        Task<OrderDto> UpdateStatusAsync(Guid id, OrderStatus newStatus);

        Task<OrderDto> CancelOrderAsync(Guid id, string reason);

        Task<PagedResultDto<OrderDto>> GetMyOrdersAsync(PagedAndSortedResultRequestDto input);
    }
}
