using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using StartinhsMart.CoreService.Pets;
using StartinhsMart.CoreService.EntityFrameworkCore;
using Xunit;

namespace StartinhsMart.CoreService.EntityFrameworkCore.Domains.Pets
{
    public class PetRepositoryTests : CoreServiceEntityFrameworkCoreTestBase
    {
        private readonly IPetRepository _petRepository;

        public PetRepositoryTests()
        {
            _petRepository = GetRequiredService<IPetRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _petRepository.GetListAsync(
                    category: "2c5c4ec7dcc04b99b795bb06cb67637e5b79a5065cd941cdb9ccc1573713e13",
                    name: "4b0e93b63b09474588749637fc2edde4009dc45deb384fb7871a",
                    breed: "e175df21277542ae9bebcc359cbe015f4f670c5b05d34581b3f1dc0756ad2b4267c8a8ea450f4ff38",
                    gender: "dcd8c6cd99134b0699861bf388c0c370b49c7bb809a9495aaca195c9650f56dbc8953b9a",
                    color: "126eff44147f42f58ed2b16e49989cd0b6c52bf0e93b4529a934f08331ad7cb",
                    healthStatus: "105ea81f524345d1a1962ceae920f4433ed793314dc44c119337",
                    description: "7352a7360fd34c548560dd3c0da468922ef3aae12b6d4f40",
                    isBooth: true,
                    isStock: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("68359b11-96cd-485f-b6eb-3513622706c4"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _petRepository.GetCountAsync(
                    category: "71d6c152db9d48b19f32a01dd1b09b0aa1663a",
                    name: "999045e29",
                    breed: "3ea09f317e854f32ac123a5173ca46dc7d3d7272e4e042819e352f43a7bb590b56dcf15608e342059dcd74414994",
                    gender: "cff14483e56",
                    color: "12dd846df33147a7b03a1447ca83bcb08a8e89b4133c4f9f9855aef",
                    healthStatus: "a0e583b832654582b5",
                    description: "df0409f813ad49ae817a38a0b5a0aafdd80a7",
                    isBooth: true,
                    isStock: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}