using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using HamroCarRental.Data;
using HamroCarRental.Models;
using HamroCarRental.Models.ViewModels;

namespace HamroCarRental.Controllers;

public class IssueController : Controller
{
    private readonly ApplicationDbContext _context;

    public IssueController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<IssueModel> GetAllAvailableCopy()
    {
        IEnumerable<IssueModel> loanRecord = from dt in _context.CarDetails
            join dtc in _context.CarCategories on dt.CategoryNumber equals dtc.CategoryNumber
            join dc in _context.carCopies on dt.CarNumber equals dc.CarNumber
            where dc.IsLoan == false
            select new IssueModel
            {
                CopyNumber = dc.CopyNumber,
                CarModel = dt.CarModel,
                CarCategory = dtc.CategoryName,
                AgeRestricted = dtc.AgeRestricted,
                DateReleased = dt.DateReleased
            };
        return loanRecord;
    }

    public IActionResult Index()
    {
        var availableCopy = GetAllAvailableCopy();

        ViewBag.AvailableCopyNumberList = JsonSerializer.Serialize(_context.carCopies.Where(x => x.IsLoan == false)
            .Select(x => x.CopyNumber).Distinct().ToList());

        return View(availableCopy);
    }

    // for search functionality
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(string request)
    {
        string CopyNumber = Request.Form["SearchCopyNumber"];
        ViewBag.SearchCopyNumber = CopyNumber;

        ViewBag.AvailableCopyNumberList = JsonSerializer.Serialize(_context.carCopies.Where(x => x.IsLoan == false)
            .Select(x => x.CopyNumber).Distinct().ToList());

        if (CopyNumber != null &&
            int.TryParse(CopyNumber, out var copyNumber) &&
            _context.carCopies.Where(x => x.CopyNumber == copyNumber).Count() > 0)
        {
            var loanRecord = from dt in _context.CarDetails
                join dtc in _context.CarCategories on dt.CategoryNumber equals dtc.CategoryNumber
                join dc in _context.carCopies on dt.CarNumber equals dc.CarNumber
                where dc.IsLoan == false && dc.CopyNumber == copyNumber
                select new IssueModel
                {
                    CopyNumber = dc.CopyNumber,
                    CarModel = dt.CarModel,
                    CarCategory = dtc.CategoryName,
                    AgeRestricted = dtc.AgeRestricted,
                    DateReleased = dt.DateReleased
                };
            return View(loanRecord);
        }

        if (CopyNumber == "")
        {
            var loanRecord = GetAllAvailableCopy();
            return View(loanRecord);
        }

        return View();
    }

    public IActionResult Create(int id)
    {
        if (id == 0 || _context.carCopies.Where(l => l.CopyNumber == id && l.IsLoan == false).Count() == 0)
            return RedirectToAction("Index");


        var currentLoan = (from dt in _context.CarDetails
            join dtc in _context.CarCategories on dt.CategoryNumber equals dtc.CategoryNumber
            join dc in _context.carCopies on dt.CarNumber equals dc.CarNumber
            where dc.CopyNumber == id
            select new IssueModel
            {
                CopyNumber = dc.CopyNumber,
                CarModel = dt.CarModel,
                CarCategory = dtc.CategoryName,
                AgeRestricted = dtc.AgeRestricted
            }).First();

        ViewData["MemberNumberList"] = from m in _context.Members
            select new SelectViewModel
                {SelectValue = m.MemberNumber, SelectKey = m.MemberFirstName + " " + m.MemberLastName};
        ViewData["LoanTypeList"] = from lt in _context.LoanTypes
            select new SelectViewModel {SelectValue = lt.LoanTypeNumber, SelectKey = lt.LoanTypeName};
        ViewData["Availablecar"] = currentLoan;
        ViewBag.CopyNumber = currentLoan.CopyNumber;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("LoanTypeNumber, CopyNumber, MemberNumber")] IssueModel rent)
    {
        var test = rent;

        var currentLoan = (from dt in _context.CarDetails
            join dtc in _context.CarCategories on dt.CategoryNumber equals dtc.CategoryNumber
            join dc in _context.carCopies on dt.CarNumber equals dc.CarNumber
            where dc.CopyNumber == rent.CopyNumber
            select new IssueModel
            {
                CopyNumber = dc.CopyNumber,
                CarModel = dt.CarModel,
                CarCategory = dtc.CategoryName,
                AgeRestricted = dtc.AgeRestricted
            }).First();

        ViewData["MemberNumberList"] = from m in _context.Members
            select new SelectViewModel
                {SelectValue = m.MemberNumber, SelectKey = m.MemberFirstName + " " + m.MemberLastName};
        ViewData["LoanTypeList"] = from lt in _context.LoanTypes
            select new SelectViewModel {SelectValue = lt.LoanTypeNumber, SelectKey = lt.LoanTypeName};
        ViewData["Availablecar"] = currentLoan;

        ViewBag.LoanTypeNumber = rent.LoanTypeNumber;
        ViewBag.MemberNumber = rent.MemberNumber;
        ViewBag.CopyNumber = rent.CopyNumber;

        string operation;
        try
        {
            operation = Request.Form["GenerateButton"];
        }
        catch
        {
            operation = "";
        }


        var ageRestriction = (from dc in _context.carCopies
            join d in _context.CarDetails on dc.CarNumber equals d.CarNumber
            join dtc in _context.CarCategories on d.CategoryNumber equals dtc.CategoryNumber
            where dc.CopyNumber == rent.CopyNumber
            select dtc.AgeRestricted).First();

        if (ageRestriction == "True" &&
            (DateTime.Today - _context.Members.Where(m => m.MemberNumber == rent.MemberNumber).First()
                .MemberDateOfBirth).Days / 365 < 18)
        {
            TempData["Error"] = "Below age restriction";
            return View();
        }


        var maxRentLimit = (from m in _context.Members
            join mt in _context.MembershipCategories on m.MemberCategoryNumber equals mt.MembershipCategoryNumber
            where m.MemberNumber == rent.MemberNumber
            select mt.MembershipCategoryTotalLoans).FirstOrDefault();

        if (_context.Loans.Where(l => l.DateReturn == DateTime.MinValue && l.MemberNumber == rent.MemberNumber)
                .Count() >= maxRentLimit)
        {
            TempData["Error"] = "Maximum number reached";
            return View();
        }


        var loanDays = (from lt in _context.LoanTypes
            where lt.LoanTypeNumber == rent.LoanTypeNumber
            select lt.Duration).First();

        var returnModel = (from dt in _context.CarDetails
            join dtc in _context.CarCategories on dt.CategoryNumber equals dtc.CategoryNumber
            join dc in _context.carCopies on dt.CarNumber equals dc.CarNumber
            where dc.CopyNumber == rent.CopyNumber
            select new ReturnModel
            {
                CopyNumber = dc.CopyNumber,
                CarModel = dt.CarModel,
                CarCategory = dtc.CategoryName,
                DateOut = DateTime.Today,
                DateDue = DateTime.Today.AddDays(loanDays),
                MemberName = _context.Members.Where(m => m.MemberNumber == rent.MemberNumber).First().MemberFirstName +
                             " " + _context.Members.Where(m => m.MemberNumber == rent.MemberNumber).First()
                                 .MemberLastName,
                StandardCharge = dt.StandardCharge,
                Payment = dt.StandardCharge * loanDays
            }).First();
        ViewData["RentalInformation"] = returnModel;

        if (operation != "Generate")
        {
            // to add new loan record
            var loan = new Loan
            {
                CopyNumber = rent.CopyNumber,
                MemberNumber = rent.MemberNumber,
                LoanTypeNumber = rent.LoanTypeNumber,
                DateOut = DateTime.Today,
                DateDue = DateTime.Today.AddDays(loanDays),
                DateReturn = DateTime.MinValue,
                ReturnAmount = returnModel.Payment
            };
            _context.Loans.Add(loan);

            // Put the copy status to unavailable
            var Copy = _context.carCopies.Where(c => c.CopyNumber == rent.CopyNumber).First();
            Copy.IsLoan = true;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        return View();
    }
}