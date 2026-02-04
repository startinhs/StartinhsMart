using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace StartinhsMart.CoreService.Pets
{
    public class PetCreateDto
    {
        public Guid? ImageId { get; set; }
        public Guid? CategoryId { get; set; }
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