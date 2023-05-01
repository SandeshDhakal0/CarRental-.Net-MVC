using System.ComponentModel.DataAnnotations;

namespace HamroCarRental.Models;

public class Brand
{
    [Key] public int BrandNumber { get; set; }

    [Display(Name = "Brand Picture")] public string BrandPictureURL { get; set; }

    [Display(Name = "Name")] public string BrandName { get; set; }


    //relationship
    public ICollection<CarDetail> CarDetails { get; set; }
}