using System.ComponentModel.DataAnnotations;

namespace HamroCarRental.Models;

public class CarCategory
{
    [Key] public int CategoryNumber { get; set; }

    [Display(Name = "Category Name")]
    [Required(ErrorMessage = "Category name must be entered")]
    public string CategoryName { get; set; }

    [Display(Name = "Category Description")]
    [Required(ErrorMessage = "Category description must be entered")]
    public string CategoryDescription { get; set; }

    [Display(Name = "Restricted Age")]
    [Required(ErrorMessage = "Age Restricted must be entered")]
    public string AgeRestricted { get; set; }

    //Relationship
    public ICollection<CarDetail> CarDetails { get; set; }
}