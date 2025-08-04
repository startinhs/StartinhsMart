using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using StartinhsMart.CoreService.Pets;

namespace StartinhsMart.CoreService.Pets
{
    public class PetsDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IPetRepository _petRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public PetsDataSeedContributor(IPetRepository petRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _petRepository = petRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _petRepository.InsertAsync(new Pet
            (
                id: Guid.Parse("68359b11-96cd-485f-b6eb-3513622706c4"),
                imageId: Guid.Parse("259865124"),
                category: "2c5c4ec7dcc04b99b795bb06cb67637e5b79a5065cd941cdb9ccc1573713e13",
                name: "4b0e93b63b09474588749637fc2edde4009dc45deb384fb7871a",
                breed: "e175df21277542ae9bebcc359cbe015f4f670c5b05d34581b3f1dc0756ad2b4267c8a8ea450f4ff38",
                age: 1336990707,
                gender: "dcd8c6cd99134b0699861bf388c0c370b49c7bb809a9495aaca195c9650f56dbc8953b9a",
                color: "126eff44147f42f58ed2b16e49989cd0b6c52bf0e93b4529a934f08331ad7cb",
                weight: 851277523,
                healthStatus: "105ea81f524345d1a1962ceae920f4433ed793314dc44c119337",
                vaccinations: 1308467454,
                description: "7352a7360fd34c548560dd3c0da468922ef3aae12b6d4f40",
                price: 1128064317,
                quantity: 1575825037,
                isBooth: true,
                isStock: true
            ));

            await _petRepository.InsertAsync(new Pet
            (
                id: Guid.Parse("c308bd9a-ea04-43b2-bfce-f7405d9b88b9"),
                imageId: Guid.Parse("1676038185"),
                category: "71d6c152db9d48b19f32a01dd1b09b0aa1663a",
                name: "999045e29",
                breed: "3ea09f317e854f32ac123a5173ca46dc7d3d7272e4e042819e352f43a7bb590b56dcf15608e342059dcd74414994",
                age: 492338173,
                gender: "cff14483e56",
                color: "12dd846df33147a7b03a1447ca83bcb08a8e89b4133c4f9f9855aef",
                weight: 1736309984,
                healthStatus: "a0e583b832654582b5",
                vaccinations: 2043004534,
                description: "df0409f813ad49ae817a38a0b5a0aafdd80a7",
                price: 2035370261,
                quantity: 1267434264,
                isBooth: true,
                isStock: true
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}