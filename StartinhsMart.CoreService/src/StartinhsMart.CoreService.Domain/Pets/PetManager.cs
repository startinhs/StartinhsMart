using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace StartinhsMart.CoreService.Pets
{
    public class PetManager : DomainService
    {
        protected IPetRepository _petRepository;

        public PetManager(IPetRepository petRepository)
        {
            _petRepository = petRepository;
        }

        public virtual async Task<Pet> CreateAsync(
        bool isBooth, bool isStock, Guid? imageId = null, Guid? categoryId = null, string? name = null, string? breed = null, float? age = null, string? gender = null, string? color = null, float? weight = null, string? healthStatus = null, int? vaccinations = null, string? description = null, decimal? price = null, int? quantity = null)
        {

            var pet = new Pet(
             GuidGenerator.Create(),
             isBooth, isStock, imageId, categoryId, name, breed, age, gender, color, weight, healthStatus, vaccinations, description, price, quantity
             );

            return await _petRepository.InsertAsync(pet);
        }

        public virtual async Task<Pet> UpdateAsync(
            Guid id,
            bool isBooth, bool isStock, Guid? imageId = null, Guid? categoryId = null, string? name = null, string? breed = null, float? age = null, string? gender = null, string? color = null, float? weight = null, string? healthStatus = null, int? vaccinations = null, string? description = null, decimal? price = null, int? quantity = null, [CanBeNull] string? concurrencyStamp = null
        )
        {

            var pet = await _petRepository.GetAsync(id);

            pet.IsBooth = isBooth;
            pet.IsStock = isStock;
            pet.ImageId = imageId;
            pet.CategoryId = categoryId;
            pet.Name = name;
            pet.Breed = breed;
            pet.Age = age;
            pet.Gender = gender;
            pet.Color = color;
            pet.Weight = weight;
            pet.HealthStatus = healthStatus;
            pet.Vaccinations = vaccinations;
            pet.Description = description;
            pet.Price = price;
            pet.Quantity = quantity;

            pet.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _petRepository.UpdateAsync(pet);
        }

    }
}