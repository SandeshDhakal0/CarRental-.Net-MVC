using HamroCarRental.Models;

namespace HamroCarRental.Data.Services;

public interface IBrandsService
{
    Task<IEnumerable<Brand>> GetAllAsync();
    Task<Brand> GetBrandAsync(int id);
    Task AddAsync(Brand Brand);
    Task<Brand> UpdateAsync(int id, Brand newBrand);
    Task DeleteAsync(int id);
}