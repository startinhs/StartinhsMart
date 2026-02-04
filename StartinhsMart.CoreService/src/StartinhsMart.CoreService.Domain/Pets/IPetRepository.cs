using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace StartinhsMart.CoreService.Pets
{
    public interface IPetRepository : IRepository<Pet, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            Guid? categoryId = null,
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
            CancellationToken cancellationToken = default);
        Task<List<Pet>> GetListAsync(
                    string? filterText = null,
                    Guid? categoryId = null,
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
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            Guid? categoryId = null,
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
            CancellationToken cancellationToken = default);
    }
}