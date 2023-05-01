using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eTickets.Data.Base;

namespace HamroCarRental.Models;

public class CarDetail : IEntityBase
{
    [Display(Name = "Category Number")]
    [ForeignKey("CategoryNumber")]
    public int CategoryNumber { get; set; }

    [Display(Name = "Studio Number")]
    [ForeignKey("StudioNumber")]
    public int StudioNumber { get; set; }

    [Display(Name = "Brand Number")]
    [ForeignKey("BrandNumber")]
    public int BrandNumber { get; set; }

    [Display(Name = "Car Picture")] public string CarPictureURL { get; set; }

    [Display(Name = "Car Model")] public string CarModel { get; set; }

    [Display(Name = "Release Date")] public DateTime DateReleased { get; set; }

    [Display(Name = "Standard Charge")] public decimal StandardCharge { get; set; }

    [Display(Name = "Penalty Charge")] public decimal PenaltyCharge { get; set; }

    //relationships
    public virtual CarCategory CarCategory { get; set; }
    public virtual Studio Studio { get; set; }
    public virtual Brand Brand { get; set; }

    //public ICollection<CastMember> CastMembers { get; set; }
    public ICollection<CarCopy> carCopies { get; set; }

    [Key] public int CarNumber { get; set; }
    public string Description { get; internal set; }
}