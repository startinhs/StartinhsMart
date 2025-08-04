using Volo.Abp.Application.Dtos;
using System;

namespace StartinhsMart.CoreService.Pets
{
    public class GetPetsInput : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Category { get; set; }
        public string? Name { get; set; }
        public string? Breed { get; set; }
        public float? AgeMin { get; set; }
        public float? AgeMax { get; set; }
        public string? Gender { get; set; }
        public string? Color { get; set; }
        public float? WeightMin { get; set; }
        public float? WeightMax { get; set; }
        public string? HealthStatus { get; set; }
        public int? VaccinationsMin { get; set; }
        public int? VaccinationsMax { get; set; }
        public string? Description { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public int? QuantityMin { get; set; }
        public int? QuantityMax { get; set; }
        public bool? IsBooth { get; set; }
        public bool? IsStock { get; set; }

        public GetPetsInput()
        {

        }
    }
}