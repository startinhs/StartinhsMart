using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace StartinhsMart.CoreService.Pets
{
    public abstract class PetsAppServiceTests<TStartupModule> : CoreServiceApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IPetsAppService _petsAppService;
        private readonly IRepository<Pet, Guid> _petRepository;

        public PetsAppServiceTests()
        {
            _petsAppService = GetRequiredService<IPetsAppService>();
            _petRepository = GetRequiredService<IRepository<Pet, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _petsAppService.GetListAsync(new GetPetsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("68359b11-96cd-485f-b6eb-3513622706c4")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("c308bd9a-ea04-43b2-bfce-f7405d9b88b9")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _petsAppService.GetAsync(Guid.Parse("68359b11-96cd-485f-b6eb-3513622706c4"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("68359b11-96cd-485f-b6eb-3513622706c4"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new PetCreateDto
            {
                ImageId = Guid.Parse("1153444338"),
                Category = "6fd3c1b0c36a45a192e4dbd9d01aa481565f3a59ba8445aeabec9b4ba7d95a1bc797cb8703f",
                Name = "4e19e73f8e83411ea5702065",
                Breed = "ed6c2de107f54b029055a",
                Age = 1809062331,
                Gender = "ef34a00ef94e4b3e8b9ea3e6b2b34028ee0271a5ed214a259b735e68651051fb85c8a6fc798b4186919eef24c",
                Color = "ddaac243531b40d7b5752655bc82819dce0745c0",
                Weight = 1123578799,
                HealthStatus = "43118ac2a4c64ac9ab5f7825e69a774f69877ff9e31644c285f",
                Vaccinations = 606468900,
                Description = "6dfca202af1e42119ed20bd31e99883f6ab6cc68929a4",
                Price = 1041052449,
                Quantity = 1329420725,
                IsBooth = true,
                IsStock = true
            };

            // Act
            var serviceResult = await _petsAppService.CreateAsync(input);

            // Assert
            var result = await _petRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.ImageId.ShouldBe(Guid.Parse("1153444338"));
            result.Category.ShouldBe("6fd3c1b0c36a45a192e4dbd9d01aa481565f3a59ba8445aeabec9b4ba7d95a1bc797cb8703f");
            result.Name.ShouldBe("4e19e73f8e83411ea5702065");
            result.Breed.ShouldBe("ed6c2de107f54b029055a");
            result.Age.ShouldBe(1809062331);
            result.Gender.ShouldBe("ef34a00ef94e4b3e8b9ea3e6b2b34028ee0271a5ed214a259b735e68651051fb85c8a6fc798b4186919eef24c");
            result.Color.ShouldBe("ddaac243531b40d7b5752655bc82819dce0745c0");
            result.Weight.ShouldBe(1123578799);
            result.HealthStatus.ShouldBe("43118ac2a4c64ac9ab5f7825e69a774f69877ff9e31644c285f");
            result.Vaccinations.ShouldBe(606468900);
            result.Description.ShouldBe("6dfca202af1e42119ed20bd31e99883f6ab6cc68929a4");
            result.Price.ShouldBe(1041052449);
            result.Quantity.ShouldBe(1329420725);
            result.IsBooth.ShouldBe(true);
            result.IsStock.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new PetUpdateDto()
            {
                ImageId = Guid.Parse("851720999"),
                Category = "6669709b14bc4b8db9",
                Name = "7b8ae4ee85ed4878900c1b593cca1ab7128bf6d6cfc54c9eb03005d5b",
                Breed = "87014a3c6e9d4c65a3c9dbe7a830f7501e1a3ab9b1b549a7829521fafd3592aadd714fac18e943f9b6d",
                Age = 1383100018,
                Gender = "36c7dec27f69461dbd385bf9830667757a9fda9489f8420e9cd428a5440cc2a51d409185f885428f9d887558214c",
                Color = "078c5099271145ff80813082decdfee6ad6bbdae040643639c0abbfec53e11867fe2a8a7e77d45cc96c",
                Weight = 1018966733,
                HealthStatus = "2696cf72ccdf4d3597b60562f60ac866a9b0ac4930d94a8f82b8",
                Vaccinations = 830584174,
                Description = "d54137a80d4944bdbc3241bdc8a93d8ea6016acdf7164cc79058622386567e6cad03bc5f2cfd",
                Price = 793982643,
                Quantity = 100245281,
                IsBooth = true,
                IsStock = true
            };

            // Act
            var serviceResult = await _petsAppService.UpdateAsync(Guid.Parse("68359b11-96cd-485f-b6eb-3513622706c4"), input);

            // Assert
            var result = await _petRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.ImageId.ShouldBe(Guid.Parse("851720999"));
            result.Category.ShouldBe("6669709b14bc4b8db9");
            result.Name.ShouldBe("7b8ae4ee85ed4878900c1b593cca1ab7128bf6d6cfc54c9eb03005d5b");
            result.Breed.ShouldBe("87014a3c6e9d4c65a3c9dbe7a830f7501e1a3ab9b1b549a7829521fafd3592aadd714fac18e943f9b6d");
            result.Age.ShouldBe(1383100018);
            result.Gender.ShouldBe("36c7dec27f69461dbd385bf9830667757a9fda9489f8420e9cd428a5440cc2a51d409185f885428f9d887558214c");
            result.Color.ShouldBe("078c5099271145ff80813082decdfee6ad6bbdae040643639c0abbfec53e11867fe2a8a7e77d45cc96c");
            result.Weight.ShouldBe(1018966733);
            result.HealthStatus.ShouldBe("2696cf72ccdf4d3597b60562f60ac866a9b0ac4930d94a8f82b8");
            result.Vaccinations.ShouldBe(830584174);
            result.Description.ShouldBe("d54137a80d4944bdbc3241bdc8a93d8ea6016acdf7164cc79058622386567e6cad03bc5f2cfd");
            result.Price.ShouldBe(793982643);
            result.Quantity.ShouldBe(100245281);
            result.IsBooth.ShouldBe(true);
            result.IsStock.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _petsAppService.DeleteAsync(Guid.Parse("68359b11-96cd-485f-b6eb-3513622706c4"));

            // Assert
            var result = await _petRepository.FindAsync(c => c.Id == Guid.Parse("68359b11-96cd-485f-b6eb-3513622706c4"));

            result.ShouldBeNull();
        }
    }
}