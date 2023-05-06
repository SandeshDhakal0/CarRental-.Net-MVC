using System.ComponentModel.DataAnnotations;

namespace HamroCarRental.Models.ViewModels;

public class Homecar
{
    public int CarNumber { get; set; }
    public string? CarModel { get; set; }
    public string? CarPictureURL { get; set; }
    public string? CarCategory { get; set; }
    public decimal StandardCharge { get; set; }

    [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
    public DateTime DateReleased { get; set; }
    public virtual CarDetail CarDetail { get; set; }

    public int AvailableQuantity { get; set; }

   // public string? CarType { get; set; }
}