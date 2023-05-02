using System;
using System.ComponentModel.DataAnnotations;

namespace HamroCarRental.Models.ViewModels
{
    public class CheckoutViewModel
    {

        [Display(Name = "Date In")]
        public DateTime DateIn { get; set; }

        [Display(Name = "Date Out")]
        public DateTime DateOut { get; set; }

        [Display(Name = "Date Due")]
        public DateTime DateDue { get; set; }

        [Display(Name = "Date Return")]
        public DateTime DateReturn { get; set; }

        [Display(Name = "Return Amount")]
        public decimal ReturnAmount { get; set; }
    }
}
