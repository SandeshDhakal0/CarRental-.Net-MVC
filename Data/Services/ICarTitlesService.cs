using eTickets.Data.Base;
using HamroCarRental.Data.ViewModels;
using HamroCarRental.Models;

namespace HamroCarRental.Data.Services;

public interface ICarDetailsService : IEntityBaseRepository<CarDetail>
{
    Task<CarDetail> GetCarDetailByIdAsync(int id);
    Task<NewCarDetailDropdownsVM> GetNewCarDetailDropdownsValues();
    Task AddNewCarDetailAsync(newCarDetailVM data);
    Task UpdateCarDetailAsync(newCarDetailVM data);
}