using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HamroCarRental.Data;
using HamroCarRental.Models.ViewModels;
using HamroCarRental.Models;
using Microsoft.AspNetCore.Authorization;

namespace HamroCarRental.Controllers;

public class LoansController : Controller
{
    //getting the database context to the controller
    private readonly ApplicationDbContext _context;

    //defining the constructor
    public LoansController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var allLoans = await _context.Loans.Include(n => n.Member).ToListAsync();
        return View(allLoans);
    }


    //[HttpPost]
    //public IActionResult Checkout(CheckoutViewModel model)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        // Create a new loan object and populate it with the data from the view model
    //        Loan loan = new Loan
    //        {
    //            //MemberNumber = model.MemberNumber,
    //            //CopyNumber = model.CopyNumber,
    //            DateOut = model.DateOut,
    //            DateDue = model.DateDue,
    //            DateReturn = model.DateReturn,
    //            ReturnAmount = model.ReturnAmount
    //        };

    //        return View(model);
    //    }
    }
    