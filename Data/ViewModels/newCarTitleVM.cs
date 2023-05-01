using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HamroCarRental.Models;

public class newCarDetailVM
{
    public int CarNumber { get; set; }

    [Display(Name = "Select a category")]
    [ForeignKey("CategoryNumber")]
    public int CategoryNumber { get; set; }

    [Display(Name = "Select a studio")]
    [ForeignKey("StudioNumber")]
    public int StudioNumber { get; set; }

    [Display(Name = "Select a Brand")]
    [ForeignKey("BrandNumber")]
    public int BrandNumber { get; set; }

    [Display(Name = "car Picture")] public string CarPictureURL { get; set; }

    [Display(Name = "Car Title")] public string CarModel { get; set; }

    [Display(Name = "Release Date")] public DateTime DateReleased { get; set; }

    [Display(Name = "Standard Charge")] public decimal StandardCharge { get; set; }

    [Display(Name = "Penalty Charge")] public decimal PenaltyCharge { get; set; }

  //  [Display(Name = "Select actor(s)")] public ICollection<int> ActorNumbers { get; set; }

    public ICollection<CarCopy> carCopies { get; set; }
}