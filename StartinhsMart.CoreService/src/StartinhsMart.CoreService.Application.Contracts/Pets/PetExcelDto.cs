using System;

namespace StartinhsMart.CoreService.Pets
{
    public class PetExcelDto
    {
        public string? Category { get; set; }
        public string? Name { get; set; }
        public string? Breed { get; set; }
        public float? Age { get; set; }
        public string? Gender { get; set; }
        public string? Color { get; set; }
        public float? Weight { get; set; }
        public string? HealthStatus { get; set; }
        public int? Vaccinations { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public bool IsBooth { get; set; }
        public bool IsStock { get; set; }
    }
}