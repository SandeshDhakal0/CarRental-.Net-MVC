using System.ComponentModel.DataAnnotations;

namespace HamroCarRental.Models.ViewModels;

public class IssueModel
{
    public string? CarModel { get; set; }

    public string? CarCategory { get; set; }

    public int CopyNumber { get; set; }

    [Display(Name = "Release Date")] public DateTime DateReleased { get; set; }

    public string AgeRestricted { get; set; }

    public int LoanTypeNumber { get; set; }

    public int MemberNumber { get; set; }
}