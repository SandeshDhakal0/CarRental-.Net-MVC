using eTickets.Data.Base;
using RopeyDVDSystem.Data.ViewModels;
using RopeyDVDSystem.Models;

namespace RopeyDVDSystem.Data.Services;

public interface IDVDTitlesService : IEntityBaseRepository<DVDTitle>
{
    Task<DVDTitle> GetDVDTitleByIdAsync(int id);
    Task<NewDVDTitleDropdownsVM> GetNewDVDTitleDropdownsValues();
    Task AddNewDVDTitleAsync(newDVDTitleVM data);
    Task UpdateDVDTitleAsync(newDVDTitleVM data);
}