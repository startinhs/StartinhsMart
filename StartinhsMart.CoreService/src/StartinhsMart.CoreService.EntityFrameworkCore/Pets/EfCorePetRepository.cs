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

namespace StartinhsMart.CoreService.Pets
{
    public class EfCorePetRepository : EfCoreRepository<CoreServiceDbContext, Pet, Guid>, IPetRepository
    {
        public EfCorePetRepository(IDbContextProvider<CoreServiceDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? category = null,
            string? name = null,
            string? breed = null,
            float? ageMin = null,
            float? ageMax = null,
            string? gender = null,
            string? color = null,
            float? weightMin = null,
            float? weightMax = null,
            string? healthStatus = null,
            int? vaccinationsMin = null,
            int? vaccinationsMax = null,
            string? description = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            int? quantityMin = null,
            int? quantityMax = null,
            bool? isBooth = null,
            bool? isStock = null,
            CancellationToken cancellationToken = default)
        {

            var query = await GetQueryableAsync();

            query = ApplyFilter(query, filterText, category, name, breed, ageMin, ageMax, gender, color, weightMin, weightMax, healthStatus, vaccinationsMin, vaccinationsMax, description, priceMin, priceMax, quantityMin, quantityMax, isBooth, isStock);

            var ids = query.Select(x => x.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<Pet>> GetListAsync(
            string? filterText = null,
            string? category = null,
            string? name = null,
            string? breed = null,
            float? ageMin = null,
            float? ageMax = null,
            string? gender = null,
            string? color = null,
            float? weightMin = null,
            float? weightMax = null,
            string? healthStatus = null,
            int? vaccinationsMin = null,
            int? vaccinationsMax = null,
            string? description = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            int? quantityMin = null,
            int? quantityMax = null,
            bool? isBooth = null,
            bool? isStock = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, category, name, breed, ageMin, ageMax, gender, color, weightMin, weightMax, healthStatus, vaccinationsMin, vaccinationsMax, description, priceMin, priceMax, quantityMin, quantityMax, isBooth, isStock);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PetConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? category = null,
            string? name = null,
            string? breed = null,
            float? ageMin = null,
            float? ageMax = null,
            string? gender = null,
            string? color = null,
            float? weightMin = null,
            float? weightMax = null,
            string? healthStatus = null,
            int? vaccinationsMin = null,
            int? vaccinationsMax = null,
            string? description = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            int? quantityMin = null,
            int? quantityMax = null,
            bool? isBooth = null,
            bool? isStock = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, category, name, breed, ageMin, ageMax, gender, color, weightMin, weightMax, healthStatus, vaccinationsMin, vaccinationsMax, description, priceMin, priceMax, quantityMin, quantityMax, isBooth, isStock);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Pet> ApplyFilter(
            IQueryable<Pet> query,
            string? filterText = null,
            string? category = null,
            string? name = null,
            string? breed = null,
            float? ageMin = null,
            float? ageMax = null,
            string? gender = null,
            string? color = null,
            float? weightMin = null,
            float? weightMax = null,
            string? healthStatus = null,
            int? vaccinationsMin = null,
            int? vaccinationsMax = null,
            string? description = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            int? quantityMin = null,
            int? quantityMax = null,
            bool? isBooth = null,
            bool? isStock = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Category!.Contains(filterText!) || e.Name!.Contains(filterText!) || e.Breed!.Contains(filterText!) || e.Gender!.Contains(filterText!) || e.Color!.Contains(filterText!) || e.HealthStatus!.Contains(filterText!) || e.Description!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(category), e => e.Category.Contains(category))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                    .WhereIf(!string.IsNullOrWhiteSpace(breed), e => e.Breed.Contains(breed))
                    .WhereIf(ageMin.HasValue, e => e.Age >= ageMin!.Value)
                    .WhereIf(ageMax.HasValue, e => e.Age <= ageMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(gender), e => e.Gender.Contains(gender))
                    .WhereIf(!string.IsNullOrWhiteSpace(color), e => e.Color.Contains(color))
                    .WhereIf(weightMin.HasValue, e => e.Weight >= weightMin!.Value)
                    .WhereIf(weightMax.HasValue, e => e.Weight <= weightMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(healthStatus), e => e.HealthStatus.Contains(healthStatus))
                    .WhereIf(vaccinationsMin.HasValue, e => e.Vaccinations >= vaccinationsMin!.Value)
                    .WhereIf(vaccinationsMax.HasValue, e => e.Vaccinations <= vaccinationsMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description))
                    .WhereIf(priceMin.HasValue, e => e.Price >= priceMin!.Value)
                    .WhereIf(priceMax.HasValue, e => e.Price <= priceMax!.Value)
                    .WhereIf(quantityMin.HasValue, e => e.Quantity >= quantityMin!.Value)
                    .WhereIf(quantityMax.HasValue, e => e.Quantity <= quantityMax!.Value)
                    .WhereIf(isBooth.HasValue, e => e.IsBooth == isBooth)
                    .WhereIf(isStock.HasValue, e => e.IsStock == isStock);
        }
    }
}