using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HamroCarRental.Data;
using HamroCarRental.Models;
using HamroCarRental.Models.Identity;

namespace HamroCarRental.Controllers;

[Authorize]
public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;


    public AdminController(ILogger<AdminController> logger, ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    // For dashboard 
    public IActionResult Index()
    {
        var MaximumUnits = 5;
        ViewBag.MemberCount = _context.Members.Count();
        ViewBag.carCount = _context.carCopies.Count();
        ViewBag.LoanCount = _context.Loans.Count();
       // ViewBag.ActorCount = _context.Actors.Count();

        /*Total car based on category  using LinQ and Lambda Expression*/
        var CarCategoryCounts = (from CarDetail in _context.CarDetails
                group CarDetail by CarDetail.CarCategory.CategoryName
                into CarCategoryGroup
                select new {Category = CarCategoryGroup.Key, Count = CarCategoryGroup.Count()})
            .OrderBy(x => x.Count).Reverse().Take(MaximumUnits);

        List<string> CarCategoryLabels = CarCategoryCounts.Select(x => x.Category).ToList();
        ViewBag.CarCategoryLabels = JsonSerializer.Serialize(CarCategoryLabels);

        var CarCategoryData = CarCategoryCounts.Select(x => x.Count).ToList();
        ViewBag.CarCategoryData = JsonSerializer.Serialize(CarCategoryData);


        var months = new[] {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};
        months = months.Take(DateTime.Now.Month).ToArray();
        var currentMonthName = months[DateTime.Now.Month - 1];
        var currentYear = DateTime.Now.Year;

        var loansByMonth = new List<int>();
        for (var i = 0; i < months.Length; i++)
        {
            var month = months[i];
            var loans = _context.Loans.Where(x => x.DateOut.Month == i + 1 && x.DateOut.Year == currentYear)
                .Sum(x => x.ReturnAmount);
            loansByMonth.Add((int) Math.Ceiling(loans));
        }

        ViewBag.LoanLabels = (string) JsonSerializer.Serialize(months);
        ViewBag.LoanData = (string) JsonSerializer.Serialize(loansByMonth);

        return View();
    }

    // for profile detail page
    public IActionResult ProfileDetail()
    {
        var user = _context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();


        ViewBag.UserName = user.UserName;
        ViewBag.Email = user.Email;
        ViewBag.FullName = user.FullName;
        ViewBag.UserRole = (from role in _context.Roles
            join userRole in _context.UserRoles on role.Id equals userRole.RoleId
            where userRole.UserId == user.Id
            select role.Name).FirstOrDefault();
        return View();
    }

    // to change password 

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProfileDetail(ChangePassword model)
    {
        var user = _context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();

        ViewBag.UserName = user.UserName;
        ViewBag.Email = user.Email;
        ViewBag.FullName = user.FullName;
        ViewBag.UserRole = (from role in _context.Roles
            join userRole in _context.UserRoles on role.Id equals userRole.RoleId
            where userRole.UserId == user.Id
            select role.Name).FirstOrDefault();

        if (!ModelState.IsValid) return View(model);


        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);


        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Invalid Old Password");
            return View(model);
        }

        ViewBag.IsSuccess = true;
        ModelState.Clear();
        return View(model);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}