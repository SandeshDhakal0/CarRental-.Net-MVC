using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace RopeyDVDSystem.Models.Identity;

public class ApplicationUser : IdentityUser
{
    [Display(Name = "Full name")] public string? FullName { get; set; }
}