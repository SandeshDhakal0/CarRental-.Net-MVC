using HamroCarRental.Models;

namespace HamroCarRental.Data.Services;

public interface ICarCategoriesService
{
    Task<IEnumerable<CarCategory>> GetAllAsync();
    Task<CarCategory> GetCarCategoryAsync(int id);
    Task AddAsync(CarCategory CarCategory);
    Task<CarCategory> UpdateAsync(int id, CarCategory newCarCategory);
    Task DeleteAsync(int id);
}