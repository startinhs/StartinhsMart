using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using StartinhsMart.CoreService.EntityFrameworkCore;

namespace StartinhsMart.CoreService.Orders
{
    public class EfCoreOrderRepository : EfCoreRepository<CoreServiceDbContext, Order, Guid>, IOrderRepository
    {
        public EfCoreOrderRepository(IDbContextProvider<CoreServiceDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<Order>> GetListAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(
                (await GetQueryableAsync()).Include(o => o.Items),
                filterText,
                orderNumber,
                userId,
                status,
                paymentStatus,
                minCreationTime,
                maxCreationTime
            );

            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? OrderConsts.GetDefaultSorting(false) : sorting);

            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string? filterText = null,
            string? orderNumber = null,
            Guid? userId = null,
            OrderStatus? status = null,
            PaymentStatus? paymentStatus = null,
            DateTime? minCreationTime = null,
            DateTime? maxCreationTime = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(
                await GetDbSetAsync(),
                filterText,
                orderNumber,
                userId,
                status,
                paymentStatus,
                minCreationTime,
                maxCreationTime
            );

            return await query.LongCountAsync(cancellationToken);
        }

        public async Task<Order?> GetWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var query = (await GetQueryableAsync())
                .Include(o => o.Items)
                .Where(o => o.Id == id);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Order>> GetUserOrdersAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var query = (await GetQueryableAsync())
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreationTime);

            return await query.ToListAsync(cancellationToken);
        }

        protected virtual IQueryable<Order> ApplyFilter(
            IQueryable<Order> query,
            string? filterText = null,
            string? orderNumber = null,
            Guid? userId = null,
            OrderStatus? status = null,
            PaymentStatus? paymentStatus = null,
            DateTime? minCreationTime = null,
            DateTime? maxCreationTime = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), 
                    e => e.OrderNumber.Contains(filterText!) || 
                         (e.CustomerName != null && e.CustomerName.Contains(filterText!)) ||
                         (e.CustomerEmail != null && e.CustomerEmail.Contains(filterText!)))
                .WhereIf(!string.IsNullOrWhiteSpace(orderNumber), e => e.OrderNumber.Contains(orderNumber!))
                .WhereIf(userId.HasValue, e => e.UserId == userId!.Value)
                .WhereIf(status.HasValue, e => e.Status == status!.Value)
                .WhereIf(paymentStatus.HasValue, e => e.PaymentStatus == paymentStatus!.Value)
                .WhereIf(minCreationTime.HasValue, e => e.CreationTime >= minCreationTime!.Value)
                .WhereIf(maxCreationTime.HasValue, e => e.CreationTime <= maxCreationTime!.Value);
        }
    }
}
