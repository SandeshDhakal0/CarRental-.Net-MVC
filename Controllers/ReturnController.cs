using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using HamroCarRental.Data;
using HamroCarRental.Models.ViewModels;

namespace HamroCarRental.Controllers;

public class ReturnController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReturnController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<ReturnModel> GetAllLoanRecords()
    {
        // The car copy that are loan list
        IEnumerable<ReturnModel> loanRecord = from dt in _context.CarDetails
            join dc in _context.carCopies on dt.CarNumber equals dc.CarNumber
            join l in _context.Loans on dc.CopyNumber equals l.CopyNumber
            join m in _context.Members on l.MemberNumber equals m.MemberNumber
            where l.DateReturn == DateTime.MinValue
            orderby l.DateOut descending, dt.CarModel
            select new ReturnModel
            {
                CopyNumber = dc.CopyNumber,
                CarModel = dt.CarModel,
                DateOut = l.DateOut,
                DateDue = l.DateDue,
                MemberName = m.MemberFirstName + ' ' + m.MemberLastName,
                TotalLoan = (from la in _context.Loans
                    join dc in _context.carCopies on l.CopyNumber equals dc.CopyNumber
                    where la.DateOut == l.DateOut
                    select la.LoanNumber).Count(),
                LoanNumber = l.LoanNumber
            };


        return loanRecord;
    }

    public IActionResult Index()
    {
        var loanRecord = GetAllLoanRecords();

        ViewBag.LoanedCopyNumberList =
            JsonSerializer.Serialize(_context.carCopies.Select(x => x.CopyNumber).Distinct().ToList());

        return View(loanRecord);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(string request)
    {
        string CopyNumber = Request.Form["SearchCopyNumber"];
        ViewBag.SearchCopyNumber = CopyNumber;

        // Get a list of all car Copy that are on loan
        ViewBag.LoanedCopyNumberList = JsonSerializer.Serialize(_context.carCopies.Select(x => x.CopyNumber).ToList());

        if (CopyNumber != null &&
            int.TryParse(CopyNumber, out var copyNumber) &&
            _context.carCopies.Where(x => x.CopyNumber == copyNumber).Count() > 0)
        {
            var loanRecord = from dt in _context.CarDetails
                join dtc in _context.CarCategories on dt.CategoryNumber equals dtc.CategoryNumber
                join dc in _context.carCopies on dt.CarNumber equals dc.CarNumber
                join l in _context.Loans on dc.CopyNumber equals l.CopyNumber
                join m in _context.Members on l.MemberNumber equals m.MemberNumber
                orderby l.DateOut descending
                where dc.CopyNumber == copyNumber
                select new ReturnModel
                {
                    CopyNumber = dc.CopyNumber,
                    CarModel = dt.CarModel,
                    CarCategory = dtc.CategoryName,
                    DateOut = l.DateOut,
                    DateDue = l.DateDue,
                    DateReturn = l.DateReturn,
                    MemberName = m.MemberFirstName + ' ' + m.MemberLastName,
                    LoanNumber = l.LoanNumber,
                    Payment = l.ReturnAmount
                };

            if (loanRecord.Count() > 0)
            {
                var loanRecordFirst = loanRecord.First();
                if (loanRecordFirst.DateReturn != DateTime.MinValue) loanRecordFirst.OverDue = 1;
                ViewData["LoanRecord"] = loanRecordFirst;
            }

            return View();
        }

        if (CopyNumber == "")
        {
            // Get all the loan records
            var loanRecord = GetAllLoanRecords();
            return View(loanRecord);
        }

        return View();
    }


    public IActionResult Confirmation(int LoanID)
    {
        if (LoanID == 0 ||
            _context.Loans.Where(l => l.LoanNumber == LoanID).Count() == 0 ||
            _context.Loans.Where(l => l.LoanNumber == LoanID).First().DateReturn != DateTime.MinValue)
            return RedirectToAction("Index");

        // Get the loan record
        var currentLoan = (from dt in _context.CarDetails
            join dtc in _context.CarCategories on dt.CategoryNumber equals dtc.CategoryNumber
            join dc in _context.carCopies on dt.CarNumber equals dc.CarNumber
            join l in _context.Loans on dc.CopyNumber equals l.CopyNumber
            join m in _context.Members on l.MemberNumber equals m.MemberNumber
            where l.LoanNumber == LoanID
            select new ReturnModel
            {
                CopyNumber = dc.CopyNumber,
                CarModel = dt.CarModel,
                CarCategory = dtc.CategoryName,
                DateOut = l.DateOut,
                DateDue = l.DateDue,
                MemberName = m.MemberFirstName + ' ' + m.MemberLastName,
                LoanNumber = l.LoanNumber,
                StandardCharge = l.ReturnAmount,
                PenaltyCharge = dt.PenaltyCharge
            }).First();

        var today = DateTime.Today;
        var overDueDays = (today - currentLoan.DateDue).TotalDays;
        if (overDueDays < 0) overDueDays = 0;

        currentLoan.OverDue = (int) overDueDays;
        currentLoan.Payment = currentLoan.StandardCharge + currentLoan.PenaltyCharge * (decimal) overDueDays;
        currentLoan.PenaltyCharge = currentLoan.PenaltyCharge * (decimal) overDueDays;

        ViewData["Return"] = currentLoan;
        return View();
    }

    [HttpPost]
    public IActionResult Confirmation(ReturnModel returncar)
    {
        if (returncar.LoanNumber == 0 ||
            returncar.CopyNumber == 0 ||
            _context.Loans.Where(l => l.LoanNumber == returncar.LoanNumber).Count() == 0 ||
            _context.carCopies.Where(dc => dc.CopyNumber == returncar.CopyNumber).Count() == 0)
            return RedirectToAction("Index");


        var loan = _context.Loans.Find(returncar.LoanNumber);
        loan.DateReturn = DateTime.Today;
        loan.ReturnAmount = returncar.Payment;
        _context.SaveChanges();


        var copy = _context.carCopies.Find(returncar.CopyNumber);
        copy.IsLoan = false;

        _context.SaveChanges();

        return RedirectToAction("Index");
    }
}