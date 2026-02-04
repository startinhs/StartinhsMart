using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using StartinhsMart.CoreService.Orders;

namespace StartinhsMart.CoreService.Controllers.Orders
{
    [RemoteService(Name = "CoreService")]
    [Area("coreService")]
    [Route("api/core-service/orders")]
    public class OrderController : CoreServiceController
    {
        private readonly IOrdersAppService _ordersAppService;

        public OrderController(IOrdersAppService ordersAppService)
        {
            _ordersAppService = ordersAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<OrderDto>> GetListAsync(GetOrdersInput input)
        {
            return _ordersAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<OrderDto> GetAsync(Guid id)
        {
            return _ordersAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<OrderDto> CreateAsync(CreateOrderDto input)
        {
            return _ordersAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<OrderDto> UpdateAsync(Guid id, UpdateOrderDto input)
        {
            return _ordersAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _ordersAppService.DeleteAsync(id);
        }

        [HttpPut]
        [Route("{id}/status")]
        public virtual Task<OrderDto> UpdateStatusAsync(Guid id, [FromBody] OrderStatus newStatus)
        {
            return _ordersAppService.UpdateStatusAsync(id, newStatus);
        }

        [HttpPost]
        [Route("{id}/cancel")]
        public virtual Task<OrderDto> CancelOrderAsync(Guid id, [FromBody] string reason)
        {
            return _ordersAppService.CancelOrderAsync(id, reason);
        }

        [HttpGet]
        [Route("my-orders")]
        public virtual Task<PagedResultDto<OrderDto>> GetMyOrdersAsync(PagedAndSortedResultRequestDto input)
        {
            return _ordersAppService.GetMyOrdersAsync(input);
        }
    }
}
