using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace StartinhsMart.CoreService.Categories
{
    public interface ICategoryRepository : IRepository<Category, Guid>
    {
        Task<List<Category>> GetListAsync(
            string? filterText = null,
            string? name = null,
            string? slug = null,
            bool? isActive = null,
            Guid? parentCategoryId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            string? slug = null,
            bool? isActive = null,
            Guid? parentCategoryId = null,
            CancellationToken cancellationToken = default
        );

        Task<bool> AnyAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<Category>> GetChildrenAsync(
            Guid parentCategoryId,
            CancellationToken cancellationToken = default
        );
    }
}
