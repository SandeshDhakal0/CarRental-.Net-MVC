using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HamroCarRental.Data;
using HamroCarRental.Models;
using HamroCarRental.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HamroCarRental.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }


    // home page functionality 
    public IActionResult Index()
    {
        var searchBarValue = HttpContext.Session.GetString("searchBarValue");
        var sortingOrderCol = HttpContext.Session.GetString("sortingOrderCol");
        var stockAvailability = HttpContext.Session.GetString("stockAvailability");
        var ageRestricted = HttpContext.Session.GetString("AgeRestricted");

        var databasecarList = from car in _context.CarDetails select car.CarNumber;

        // checking the age restriction
        switch (ageRestricted)
        {
            case "yes":
                databasecarList = from car in databasecarList
                    join cardt in _context.CarDetails on car equals cardt.CarNumber
                    join dc in _context.CarCategories on cardt.CategoryNumber equals dc.CategoryNumber
                    where dc.AgeRestricted == "False"
                    select car;
                break;
        }


        ViewBag.sortingOrderCol = string.IsNullOrEmpty(sortingOrderCol) ? "na" : sortingOrderCol;
        ViewBag.stockAvailability = string.IsNullOrEmpty(stockAvailability) ? "all" : stockAvailability;
        ViewBag.AgeRestricted = string.IsNullOrEmpty(ageRestricted) ? "no" : ageRestricted;


        IEnumerable<Homecar> carDetails = from allcar in databasecarList
            join car in _context.CarDetails on allcar equals car.CarNumber
            join category in _context.CarCategories on car.CategoryNumber equals category.CategoryNumber
            select new Homecar
            {
                CarNumber = car.CarNumber,
                CarModel = car.CarModel,
                CarPictureURL = car.CarPictureURL,
                CarCategory = category.CategoryName,
                StandardCharge = car.StandardCharge,
                DateReleased = car.DateReleased,
                AvailableQuantity = _context.carCopies.Where(d => d.CarNumber == car.CarNumber).Count() == 0
                    ? -1
                    : (from CarCopy in _context.carCopies
                        where CarCopy.CarNumber == car.CarNumber
                        select CarCopy.IsLoan ? 0 : 1).Sum()
            };


        if (!string.IsNullOrEmpty(searchBarValue))
        {
            ViewBag.searchBarValue = searchBarValue;
            carDetails = carDetails.Where(d =>
               // d.CastMember.ToLower().Contains(searchBarValue.ToLower()) ||
                d.CarModel.ToLower().Contains(searchBarValue.ToLower()));
        }

        // cases for sorting
        switch (sortingOrderCol)
        {
            case "pa":
                carDetails = carDetails.OrderBy(d => d.StandardCharge);
                break;

            case "pd":
                carDetails = carDetails.OrderByDescending(d => d.StandardCharge);
                break;


            default:
                carDetails = carDetails.OrderBy(d => d.CarModel);
                break;
        }


        switch (stockAvailability)
        {
            case "available":
                carDetails = carDetails.Where(d => d.AvailableQuantity > 0);
                break;

            case "outOfStock":
                carDetails = carDetails.Where(d => d.AvailableQuantity == 0);
                break;
        }


        return View(carDetails);
    }

    public IActionResult ProductDetail(int id)
    {
        var carDetail = _context.CarDetails.FirstOrDefault(c => c.CarNumber == id);
        if (carDetail == null)
        {
            return NotFound();
        }

        var carCopies = _context.carCopies.Where(cc => cc.CarNumber == carDetail.CarNumber).ToList();
        var availableCopies = carCopies.Where(cc => !cc.IsLoan).ToList();

        var viewModel = new ProductDetailViewModel
        {
            CarModel = carDetail.CarModel,
            CarPictureURL = carDetail.CarPictureURL,
            CarCategory = _context.CarCategories.FirstOrDefault(c => c.CategoryNumber == carDetail.CategoryNumber)?.CategoryName,
            StandardCharge = carDetail.StandardCharge,
            Description = carDetail.Description,
            AvailableCopies = availableCopies,
            TotalCopies = carCopies.Count(),
            DateReleased = carDetail.DateReleased
        };

        return View(viewModel);
    }











    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult PostIndex()
    {
        HttpContext.Session.SetString("searchBarValue", Request.Form["searchBarValue"]);
        HttpContext.Session.SetString("sortingOrderCol", Request.Form["sortingOrderCol"]);
        HttpContext.Session.SetString("stockAvailability", Request.Form["stockAvailability"]);
        HttpContext.Session.SetString("AgeRestricted", Request.Form["AgeRestricted"]);


        return RedirectToAction("Index");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }


  


}
