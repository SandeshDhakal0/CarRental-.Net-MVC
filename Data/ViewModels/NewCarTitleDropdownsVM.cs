using HamroCarRental.Models;

namespace HamroCarRental.Data.ViewModels;

public class NewCarDetailDropdownsVM
{
    public NewCarDetailDropdownsVM()
    {
        Studios = new List<Studio>();
        CarCategories = new List<CarCategory>();
        Brands = new List<Brand>();
      //  Actors = new List<Actor>();
    }

    public List<Studio> Studios { get; set; }
    public List<CarCategory> CarCategories { get; set; }
    public List<Brand> Brands { get; set; }
   // public List<Actor> Actors { get; set; }
}