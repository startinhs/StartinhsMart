using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StartinhsMart.CoreService.Categories;
using StartinhsMart.CoreService.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace StartinhsMart.CoreService.Categories
{
    public class EfCoreCategoryRepository : EfCoreRepository<CoreServiceDbContext, Category, Guid>, ICategoryRepository
    {
        public EfCoreCategoryRepository(IDbContextProvider<CoreServiceDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<Category>> GetListAsync(
            string? filterText = null,
            string? name = null,
            string? slug = null,
            bool? isActive = null,
            Guid? parentCategoryId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, name, slug, isActive, parentCategoryId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? CategoryConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            string? slug = null,
            bool? isActive = null,
            Guid? parentCategoryId = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, name, slug, isActive, parentCategoryId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync()).AnyAsync(x => x.Id == id, GetCancellationToken(cancellationToken));
        }

        public async Task<List<Category>> GetChildrenAsync(Guid parentCategoryId, CancellationToken cancellationToken = default)
        {
            return await (await GetDbSetAsync())
                .Where(x => x.ParentCategoryId == parentCategoryId)
                .OrderBy(x => x.SortOrder)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Category> ApplyFilter(
            IQueryable<Category> query,
            string? filterText = null,
            string? name = null,
            string? slug = null,
            bool? isActive = null,
            Guid? parentCategoryId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), 
                    e => (e.Name != null && e.Name.Contains(filterText!)) || 
                         (e.Slug != null && e.Slug.Contains(filterText!)) ||
                         (e.Description != null && e.Description.Contains(filterText!)))
                .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name != null && e.Name.Contains(name!))
                .WhereIf(!string.IsNullOrWhiteSpace(slug), e => e.Slug != null && e.Slug.Contains(slug!))
                .WhereIf(isActive.HasValue, e => e.IsActive == isActive!.Value)
                .WhereIf(parentCategoryId.HasValue, e => e.ParentCategoryId == parentCategoryId!.Value);
        }
    }

    public static class CategoryConsts
    {
        public static string GetDefaultSorting(bool withEntityName)
        {
            return withEntityName
                ? "Category.SortOrder, Category.Name"
                : "SortOrder, Name";
        }
    }
}
