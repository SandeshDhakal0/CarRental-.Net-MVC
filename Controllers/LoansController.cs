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

    public IActionResult Checkout(int copyNumber)
    {
        // Get the car copy by the copy number
        var carCopy = _context.carCopies
            .Include(cc => cc.CarNumber)
            .FirstOrDefault(cc => cc.CopyNumber == copyNumber);

        if (carCopy == null)
        {
            return NotFound();
        }

        // Create a new loan object
        var loan = new Loan
        {
            CopyNumber = carCopy.CopyNumber,
            CarCopy = carCopy,
            DateOut = DateTime.Now,
            DateDue = DateTime.Now.AddDays(7) // Due date is 7 days from now
        };

        return View(loan);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout([Bind("CopyNumber,MemberNumber,DateDue")] Loan loan)
    {
        if (ModelState.IsValid)
        {
            // Update the loan information
            loan.DateOut = DateTime.Now;

            // Get the car copy by the copy number
            var carCopy = _context.carCopies.FirstOrDefault(cc => cc.CopyNumber == loan.CopyNumber);

            if (carCopy == null)
            {
                return NotFound();
            }

            carCopy.AvailableQuantity = 0; // Mark the car copy as not available

            // Add the loan to the database
            _context.Add(loan);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(loan);
    }
}

    