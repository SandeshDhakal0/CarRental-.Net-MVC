using Microsoft.EntityFrameworkCore;
using HamroCarRental.Models;

namespace HamroCarRental.Data.Services;

public class BrandsService : IBrandsService
{
    private readonly ApplicationDbContext _context;

    public BrandsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Brand Brand)
    {
        await _context.Brands.AddAsync(Brand);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var result = await _context.Brands.FirstOrDefaultAsync(n => n.BrandNumber == id);
        _context.Brands.Remove(result);
        await _context.SaveChangesAsync();
    }

    public async Task<Brand> GetBrandAsync(int id)
    {
        var result = await _context.Brands.FirstOrDefaultAsync(n => n.BrandNumber == id);
        return result;
    }

    public async Task<IEnumerable<Brand>> GetAllAsync()
    {
        var result = await _context.Brands.ToListAsync();
        return result;
    }

    public async Task<Brand> UpdateAsync(int id, Brand newBrand)
    {
        _context.Update(newBrand);
        await _context.SaveChangesAsync();
        return newBrand;
    }
}