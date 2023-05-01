using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HamroCarRental.Models;

public class CarCopy
{
    [Key] public int CopyNumber { get; set; }

    [ForeignKey("CarNumber")] public int CarNumber { get; set; }

    public int AvailableQuantity { get; set; }

    public DateTime DatePurchased { get; set; }

    //Relationship
    public virtual CarDetail CarDetail { get; set; }

    public bool IsLoan { get; set; }
    public ICollection<Loan> Loans { get; set; }
}