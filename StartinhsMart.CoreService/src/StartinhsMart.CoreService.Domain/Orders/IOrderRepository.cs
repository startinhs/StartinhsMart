using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace StartinhsMart.CoreService.Orders
{
    public interface IOrderRepository : IRepository<Order, Guid>
    {
        Task<List<Order>> GetListAsync(
            string? filterText = null,
            string? orderNumber = null,
            Guid? userId = null,
            OrderStatus? status = null,
            PaymentStatus? paymentStatus = null,
            DateTime? minCreationTime = null,
            DateTime? maxCreationTime = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? orderNumber = null,
            Guid? userId = null,
            OrderStatus? status = null,
            PaymentStatus? paymentStatus = null,
            DateTime? minCreationTime = null,
            DateTime? maxCreationTime = null,
            CancellationToken cancellationToken = default
        );

        Task<Order?> GetWithItemsAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<Order>> GetUserOrdersAsync(
            Guid userId,
            CancellationToken cancellationToken = default
        );
    }
}
