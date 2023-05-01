using Microsoft.EntityFrameworkCore;
using HamroCarRental.Models;

namespace HamroCarRental.Data.Services;

public class CarCategoriesService : ICarCategoriesService
{
    private readonly ApplicationDbContext _context;

    public CarCategoriesService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CarCategory CarCategory)
    {
        await _context.CarCategories.AddAsync(CarCategory);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var result = await _context.CarCategories.FirstOrDefaultAsync(n => n.CategoryNumber == id);
        _context.CarCategories.Remove(result);
        await _context.SaveChangesAsync();
    }

    public async Task<CarCategory> GetCarCategoryAsync(int id)
    {
        var result = await _context.CarCategories.FirstOrDefaultAsync(n => n.CategoryNumber == id);
        return result;
    }

    public async Task<IEnumerable<CarCategory>> GetAllAsync()
    {
        var result = await _context.CarCategories.ToListAsync();
        return result;
    }

    public async Task<CarCategory> UpdateAsync(int id, CarCategory newCarCategory)
    {
        _context.Update(newCarCategory);
        await _context.SaveChangesAsync();
        return newCarCategory;
    }
}