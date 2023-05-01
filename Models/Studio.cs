using System.ComponentModel.DataAnnotations;

namespace HamroCarRental.Models;

public class Studio
{
    [Key] public int StudioNumber { get; set; }

    [Display(Name = "Studio Name")] public string StudioName { get; set; }

    //relationship
    public ICollection<CarDetail> CarDetails { get; set; }
}