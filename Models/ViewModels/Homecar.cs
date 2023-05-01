using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HamroCarRental.Models;

namespace HamroCarRental.ViewModels
{
    public class LoanViewModel
    {
        public int LoanNumber { get; set; }

        [Display(Name = "Loan Type")]
        public int LoanTypeNumber { get; set; }

        [Display(Name = "Car Copy")]
        public int CopyNumber { get; set; }

        [Display(Name = "Member")]
        public int MemberNumber { get; set; }

        [Display(Name = "Date Out")]
        [DataType(DataType.Date)]
        public DateTime DateOut { get; set; }

        [Display(Name = "Date Due")]
        [DataType(DataType.Date)]
        public DateTime DateDue { get; set; }

        [Display(Name = "Date Return")]
        [DataType(DataType.Date)]
        public DateTime DateReturn { get; set; }

        [Display(Name = "Return Amount")]
        public decimal ReturnAmount { get; set; }

        public List<LoanType> LoanTypes { get; set; }
        public List<CarCopy> CarCopies { get; set; }
        public List<Member> Members { get; set; }
    }
}
