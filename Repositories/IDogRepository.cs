using DogGo.Models;

namespace DogGo.Repositories
{
    public interface IDogRepository
    {
        List<Dog> GetAllDogs();
        void AddDog(Dog dog);
        Dog GetDogById(int id);
        void UpdateDog(Dog dog);
        void DeleteDog(int dogId);
    }
}