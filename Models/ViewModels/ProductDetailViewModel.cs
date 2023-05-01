using System.ComponentModel.DataAnnotations;

namespace HamroCarRental.Models.ViewModels;

public class ProductDetailViewModel
{
    public int CarNumber { get; set; }
    public string CarModel { get; set; }
    public string CarPictureURL { get; set; }
    public string CarCategory { get; set; }
    public decimal StandardCharge { get; set; }
    public string Description { get; set; }
    public List<CarCopy> AvailableCopies { get; set; }
    public int TotalCopies { get; set; }
    public DateTime DateReleased { get; set; }
}