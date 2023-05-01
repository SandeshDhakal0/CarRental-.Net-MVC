using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HamroCarRental.Data;
using HamroCarRental.Models.ViewModels;
using HamroCarRental.Models;

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
        var allLoans = await _context.Loans.Include(n => n.Member).Include(n => n.LoanType).ToListAsync();
        return View(allLoans);
    }

    [HttpPost]
    public async Task<IActionResult> Create(LoanViewModel model)
    {
        if (ModelState.IsValid)
        {
            var loan = new Loan
            {
                LoanTypeNumber = model.LoanTypeNumber,
                CopyNumber = model.CopyNumber,
                MemberNumber = model.MemberNumber,
                DateOut = model.DateOut,
                DateDue = model.DateDue,
                ReturnAmount = model.ReturnAmount
            };

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return RedirectToAction("Checkout");
        }

        // If the model state is not valid, redisplay the form with validation errors
        model.LoanTypes = await _context.LoanTypes.ToListAsync();
        model.CarCopies = await _context.carCopies.ToListAsync();
        model.Members = await _context.Members.ToListAsync();
        return View(model);
    }



}