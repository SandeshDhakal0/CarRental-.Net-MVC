using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HamroCarRental.Data;

namespace HamroCarRental.Controllers;

public class carCopiesController : Controller
{
    //getting database context in the controller
    private readonly ApplicationDbContext _context;

    //defining a constructor
    public carCopiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    //get carCopies
    public async Task<IActionResult> Index()
    {
        var allcarCopies = await _context.carCopies.ToListAsync();
        return View();
    }

    //feature 10: displaying list of carCopies older than 365 days and currently not on loan
    public async Task<IActionResult> OlderThan365Days()
    {
        //Getting Distinct Copy Numbers of Loaned cars
        var loanedCopy = (from loan in _context.Loans
            where loan.DateReturn == null
            select loan.CopyNumber).Distinct();

        //Getting Data of Copies that have not been loaned
        var notLoanedCopy = from copy in _context.carCopies
            join CarDetail in _context.CarDetails on copy.CarNumber equals CarDetail.CarNumber
            where !loanedCopy.Contains(copy.CopyNumber) && copy.IsLoan == false
            select new
            {
                copy.CopyNumber,
                CarDetail = CarDetail.CarModel,
                copy.DatePurchased
            };

        return View(await notLoanedCopy.ToListAsync());
    }

    // GET: CarCopy/Delete/id
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var CarCopyModel = await _context.carCopies
            .Include(d => d.CarDetail)
            .FirstOrDefaultAsync(m => m.CopyNumber == id);
        if (CarCopyModel == null) return NotFound();

        return View(CarCopyModel);
    }

    // POST: CarCopy/Delete/id
    [HttpPost]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var CarCopyModel = await _context.carCopies.FindAsync(id);
        _context.carCopies.Remove(CarCopyModel);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(OlderThan365Days));
    }


    //to remove all the older car copy from database at once
    public async Task<IActionResult> RemoveAll()
    {
        //Getting Distinct Copy Numbers of Loaned cars
        var loanedCopy = (from loan in _context.Loans
            where loan.DateReturn == null
            select loan.CopyNumber).Distinct();

        //Getting Data of Copies that have not been loaned
        var notLoanedCopy = from copy in _context.carCopies
            join CarDetail in _context.CarDetails on copy.CarNumber equals CarDetail.CarNumber
            where !loanedCopy.Contains(copy.CopyNumber) && copy.IsLoan == false
            select new
            {
                copy.CopyNumber,
                CarDetail = CarDetail.CarModel,
                copy.DatePurchased
            };

        foreach (var copy in notLoanedCopy.ToList())
            //Checking if the Copy is older than 365 days
            if (DateTime.Now.Subtract(copy.DatePurchased).Days > 365)
            {
                //Removing the Copy from the Database if the Copy is older than 365 days.
                var remove = (from removeCopy in _context.carCopies
                    where removeCopy.CopyNumber == copy.CopyNumber
                    select removeCopy).FirstOrDefault();
                _context.carCopies.Remove(remove);
            }

        //Save the changes made to the database.
        _context.SaveChanges();
        return RedirectToAction("OlderThan365Days");
    }
}