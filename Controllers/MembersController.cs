using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HamroCarRental.Data;
using HamroCarRental.Models.ViewModels;

namespace HamroCarRental.Controllers;

public class MembersController : Controller
{
    //getting database context to the controller
    private readonly ApplicationDbContext _context;

    //defining a constructor
    public MembersController(ApplicationDbContext context)
    {
        _context = context;
    }

    //get Members
    public async Task<IActionResult> Index()
    {
        //Using LINQ to get Member Details
        var allMembers = from members in _context.Members
            join membership in _context.MembershipCategories on members.MemberCategoryNumber equals membership
                .MembershipCategoryNumber
            select new
            {
                members.MemberNumber,
                members.MemberFirstName,
                members.MemberLastName,
                members.MemberAddress,
                MemberDOB = members.MemberDateOfBirth.ToString("dd MMM yyyy"),
                Membership = membership.MembershipCategoryName,
                TotalAcceptLoans = membership.MembershipCategoryTotalLoans,
                TotalCurrentLoans = (from loans in _context.Loans
                    where loans.DateReturn == DateTime.MinValue
                    where loans.MemberNumber == members.MemberNumber
                    select loans).Count()
            };
        return View(await allMembers.ToListAsync());
    }

    //feature 8: getting the list of members along with their loan details of last 31 days
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(string request)
    {
        string MemberNumber = Request.Form["SearchMemberNumber"];
        ViewBag.SearchMemberNumber = MemberNumber;

        var allMembers = from members in _context.Members
            join membership in _context.MembershipCategories on members.MemberCategoryNumber equals membership
                .MembershipCategoryNumber
            select new
            {
                members.MemberNumber,
                members.MemberFirstName,
                members.MemberLastName,
                members.MemberAddress,
                MemberDOB = members.MemberDateOfBirth.ToString("dd MMM yyyy"),
                Membership = membership.MembershipCategoryName,
                TotalAcceptLoans = membership.MembershipCategoryTotalLoans,
                TotalCurrentLoans = (from loans in _context.Loans
                    where loans.DateReturn == DateTime.MinValue
                    where loans.MemberNumber == members.MemberNumber
                    select loans).Count()
            };

        if (MemberNumber != "")
            allMembers = allMembers.Where(x =>
                x.MemberLastName == MemberNumber || x.MemberNumber.ToString() == MemberNumber);

        return View(await allMembers.ToListAsync());
    }

    //feature 3
    // GET: Members/Details/id
    public IActionResult Details(int id)
    {
        if (_context.Members.Where(x => x.MemberNumber == id).Count() == 0) return RedirectToAction("Index");

        var currentMember = _context.Members.Where(x => x.MemberNumber == id).FirstOrDefault();
        var filterRange = DateTime.Today - TimeSpan.FromDays(31);

        ViewBag.MemberFirstName = currentMember.MemberFirstName;
        ViewBag.MemberLastName = currentMember.MemberLastName;
        ViewBag.MemberAddress = currentMember.MemberAddress;
        ViewBag.Birthday = currentMember.MemberDateOfBirth.ToString("MMM d, yyyy");
        ViewBag.MemebershipType = _context.MembershipCategories
            .Where(x => x.MembershipCategoryNumber == currentMember.MemberCategoryNumber).FirstOrDefault()
            .MembershipCategoryName;
        if (_context.Loans.Where(x => x.MemberNumber == id).Count() > 0)
            ViewBag.LastLoan = _context.Loans.Where(x => x.MemberNumber == currentMember.MemberNumber)
                .OrderByDescending(x => x.DateOut).FirstOrDefault().DateOut.ToString("MMM d, yyyy");
        ViewBag.TotalLoans = _context.Loans.Where(x => x.MemberNumber == currentMember.MemberNumber).Count();

        IEnumerable<ReturnModel> loanRecord = from dt in _context.CarDetails
            join dtc in _context.CarCategories on dt.CategoryNumber equals dtc.CategoryNumber
            join dc in _context.carCopies on dt.CarNumber equals dc.CarNumber
            join l in _context.Loans on dc.CopyNumber equals l.CopyNumber
            join m in _context.Members on l.MemberNumber equals m.MemberNumber
            orderby l.DateOut descending
            where m.MemberNumber == currentMember.MemberNumber && l.DateOut >= filterRange
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

        return View(loanRecord);
    }

    //feature 12: displaying the list of inactive members
    public async Task<IActionResult> InactiveMembers()
    {
        //Get DateTime of 31 Days Before Today's DateTime
        var differenceDate = DateTime.Now.AddDays(-31);
        //Get Data of Loans Taken 31 Days from Current Date
        var membersLoan = (from loans in _context.Loans
            where loans.DateOut >= differenceDate
            select loans.MemberNumber).Distinct();
        //Get Data of car Copies which has not been loaned
        var membersNoLoan = from member in _context.Members
            join membership in _context.MembershipCategories on member.MemberCategoryNumber equals membership
                .MembershipCategoryNumber
            where !membersLoan.Contains(member.MemberNumber)
            select new
            {
                member.MemberNumber,
                MemberName = member.MemberFirstName + " " + member.MemberLastName,
                member.MemberAddress,
                MemberDOB = member.MemberDateOfBirth.ToString("dd MMM yyyy"),
                Membership = membership.MembershipCategoryName,
                LastLoan = (from loan in _context.Loans
                        join CarCopy in _context.carCopies on loan.CopyNumber equals CarCopy.CopyNumber
                        join CarDetail in _context.CarDetails on CarCopy.CarNumber equals CarDetail.CarNumber
                        where loan.MemberNumber == member.MemberNumber
                        orderby loan.DateOut descending
                        select new
                        {
                            loan.DateOut,
                            CarDetail = CarDetail.CarModel
                        }
                    ).FirstOrDefault()
            };

        return View(await membersNoLoan.ToListAsync());
    }
}