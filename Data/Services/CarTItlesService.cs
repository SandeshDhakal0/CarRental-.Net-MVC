using eTickets.Data.Base;
using Microsoft.EntityFrameworkCore;
using HamroCarRental.Data.ViewModels;
using HamroCarRental.Models;

namespace HamroCarRental.Data.Services;

public class CarDetailsService : EntityBaseRepository<CarDetail>, ICarDetailsService
{
    private readonly ApplicationDbContext _context;

    public CarDetailsService(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task AddNewCarDetailAsync(newCarDetailVM data)
    {
        var newcar = new CarDetail
        {
            CarModel = data.CarModel,
            CategoryNumber = data.CategoryNumber,
            StudioNumber = data.StudioNumber,
            BrandNumber = data.BrandNumber,
            CarPictureURL = data.CarPictureURL,
            DateReleased = data.DateReleased,
            StandardCharge = data.StandardCharge,
            PenaltyCharge = data.PenaltyCharge
        };
        await _context.CarDetails.AddAsync(newcar);
        await _context.SaveChangesAsync();

        /* ADD Movie Actors*/
        //foreach (var actorId in data.ActorNumbers)
        //{
        //    var newCastMember = new CastMember
        //    {
        //        CarNumber = newcar.CarNumber,
        //        ActorNumber = actorId
        //    };
        //    await _context.CastMembers.AddAsync(newCastMember);
        //}

        //await _context.SaveChangesAsync();
    }

    public async Task<CarDetail> GetCarDetailByIdAsync(int id)
    {
        var carDetails = await _context.CarDetails
            .Include(s => s.Studio)
            .Include(p => p.Brand)
            .Include(c => c.CarCategory)
            // .Include(cm => cm.CastMembers).ThenInclude(a => a.Actor)
            .FirstOrDefaultAsync(n => n.CarNumber == id);

        return carDetails;
    }

    public async Task<NewCarDetailDropdownsVM> GetNewCarDetailDropdownsValues()
    {
        var response = new NewCarDetailDropdownsVM
        {
            // Actors = await _context.Actors.OrderBy(n => n.ActorFirstName).ToListAsync(),
            CarCategories = await _context.CarCategories.OrderBy(n => n.CategoryName).ToListAsync(),
            Studios = await _context.Studios.OrderBy(n => n.StudioName).ToListAsync(),
            Brands = await _context.Brands.OrderBy(n => n.BrandName).ToListAsync()
        };


        return response;
    }

    public async Task UpdateCarDetailAsync(newCarDetailVM data)
    {
        var dbCarDetail = await _context.CarDetails.FirstOrDefaultAsync(n => n.CarNumber == data.CarNumber);

        if (dbCarDetail != null)
        {
            dbCarDetail.CarModel = data.CarModel;
            dbCarDetail.CategoryNumber = data.CategoryNumber;
            dbCarDetail.StudioNumber = data.StudioNumber;
            dbCarDetail.BrandNumber = data.BrandNumber;
            dbCarDetail.CarPictureURL = data.CarPictureURL;
            dbCarDetail.DateReleased = data.DateReleased;
            dbCarDetail.StandardCharge = data.StandardCharge;
            dbCarDetail.PenaltyCharge = data.PenaltyCharge;
            await _context.SaveChangesAsync();
        }

        //Remove existing actors
        // var existingActorsDb = _context.CastMembers.Where(n => n.CarNumber == data.CarNumber).ToList();
        //_context.CastMembers.RemoveRange(existingActorsDb);
        //await _context.SaveChangesAsync();


        //ADD Movie Actors
        //    foreach (var actorId in data.ActorNumbers)
        //    {
        //        var newCastMember = new CastMember
        //        {
        //            CarNumber = data.CarNumber,
        //            ActorNumber = actorId
        //        };
        //        await _context.CastMembers.AddAsync(newCastMember);
        //    }

        //    await _context.SaveChangesAsync();
        //}
    }
}