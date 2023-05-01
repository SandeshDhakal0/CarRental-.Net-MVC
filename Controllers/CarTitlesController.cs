using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HamroCarRental.Data;
using HamroCarRental.Data.Services;
using HamroCarRental.Models;


namespace HamroCarRental.Controllers;

public class CarDetailsController : Controller
{
    private readonly ApplicationDbContext _context;

    //getting both services anf database context in the controller
    private readonly ICarDetailsService _service;

    //defining a constructor
    public CarDetailsController(ICarDetailsService service, ApplicationDbContext context)
    {
        _service = service;
        _context = context;
    }

    //get CarDetails
    public async Task<IActionResult> Index()
    {
        var allCarDetails = await _service.GetAllAsync(n => n.Brand, n => n.Studio, n => n.CarCategory);
        return View(allCarDetails);
    }

    //GET: CarDetails/Details/id
    public async Task<IActionResult> Details(int id)
    {
        var CarDetailDetail = await _service.GetCarDetailByIdAsync(id);
        return View(CarDetailDetail);
    }

    //Get: CarDetails/create/id
    public async Task<IActionResult> Create()
    {
        var dDVTitleDropdownsData = await _service.GetNewCarDetailDropdownsValues();

        ViewBag.Studios = new SelectList(dDVTitleDropdownsData.Studios, "StudioNumber", "StudioName");
        ViewBag.Brands = new SelectList(dDVTitleDropdownsData.Brands, "BrandNumber", "BrandName");
        ViewBag.CarCategories = new SelectList(dDVTitleDropdownsData.CarCategories, "CategoryNumber", "CategoryName");
       // ViewBag.Actors = new SelectList(dDVTitleDropdownsData.Actors, "ActorNumber", "ActorFirstName");


        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(newCarDetailVM CarDetail)
    {
        await _service.AddNewCarDetailAsync(CarDetail);
        return RedirectToAction(nameof(Index));
    }

    //GET: CarDetails/Edit/id
    public async Task<IActionResult> Edit(int id)
    {
        var carDetails = await _service.GetCarDetailByIdAsync(id);
        if (carDetails == null) return View("NotFound");

        var response = new newCarDetailVM
        {
            CarNumber = carDetails.CarNumber,
            CarModel = carDetails.CarModel,
            DateReleased = carDetails.DateReleased,
            CategoryNumber = carDetails.CategoryNumber,
            StudioNumber = carDetails.StudioNumber,
            BrandNumber = carDetails.BrandNumber,
            CarPictureURL = carDetails.CarPictureURL,
            StandardCharge = carDetails.StandardCharge,
            PenaltyCharge = carDetails.PenaltyCharge
            //ActorNumbers = carDetails.CastMembers.Select(n => n.ActorNumber).ToList()
        };

        var carDropdownsData = await _service.GetNewCarDetailDropdownsValues();
        ViewBag.CarCategories = new SelectList(carDropdownsData.CarCategories, "CategoryNumber", "CategoryName");
        ViewBag.Brands = new SelectList(carDropdownsData.Brands, "BrandNumber", "BrandName");
        ViewBag.Studios = new SelectList(carDropdownsData.Studios, "StudioNumber", "StudioName");
     //   ViewBag.Actors = new SelectList(carDropdownsData.Actors, "ActorNumber", "ActorFirstName");

        return View(response);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, newCarDetailVM CarDetail)
    {
        //checking the id of the Car Title to be edited
        if (id != CarDetail.CarNumber) return View("NotFound");

        await _service.UpdateCarDetailAsync(CarDetail);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> CarDetails(int id)
    {
        var carDetail = await _service.GetCarDetailByIdAsync(id);

        if (carDetail == null)
        {
            return View("NotFound");
        }

        return View("CarDetails", carDetail);
    }

    //feature 4: listing the car details in increasing order of released date
    public async Task<IActionResult> CarDetailsIndex()
    {
        //Using LINQ to get data of all cars 
        var data = from CarDetail in _context.CarDetails
            join CarCategory in _context.CarCategories on CarDetail.CategoryNumber equals CarCategory.CategoryNumber
            join studio in _context.Studios on CarDetail.StudioNumber equals studio.StudioNumber
            orderby CarDetail.DateReleased
            select new
            {
                Title = CarDetail.CarModel,
                Picture = CarDetail.CarPictureURL,
                Studio = studio.StudioName,
                Brand = CarDetail.Brand.BrandName,
                //Cast = from casts in CarDetail.CastMembers
                //    join actor in _context.Actors on casts.ActorNumber equals actor.ActorNumber
                //    group actor by new {casts.CarNumber}
                //    into g
                //    select
                //        string.Join(", ",
                //            g.OrderBy(c => c.ActorSurname).Select(x => x.ActorFirstName + " " + x.ActorSurname)),
                Release = CarDetail.DateReleased.ToString("dd MMM yyyy")
            };
        //Ordering the data by Castmembers
       // data.OrderBy(c => c.Cast);
        return View(await data.ToListAsync());
    }



    //feature 13: listing the Car Titles whose copies haven't been loaned in 31 days
    public async Task<IActionResult> Unoccupiedcars()
    {
        //Get DateTime of 31 Days Before Today's DateTime
        var differenceDate = DateTime.Now.AddDays(-31);

        //Get all data of Loaned Copies in 31 Days
        var loanedCopyDetails = (from loan in _context.Loans
            where loan.DateOut >= differenceDate
            select loan.CopyNumber).Distinct();

        //Get all data of Copies that have not been loaned.
        var notLoanedCopyDetails = from copy in _context.carCopies
            join CarDetail in _context.CarDetails on copy.CarNumber equals CarDetail.CarNumber
            join category in _context.CarCategories on CarDetail.CategoryNumber equals category.CategoryNumber
            where !loanedCopyDetails.Contains(copy.CopyNumber)
            select new
            {
                copy.CopyNumber,
                Title = CarDetail.CarModel,
                Picture = CarDetail.CarPictureURL,
                ReleaseDate = CarDetail.DateReleased.ToString("dd MMM yyyy"),
                Category = category.CategoryName,
                LastLoan = (from loan in _context.Loans
                        join CarCopy in _context.carCopies on loan.CopyNumber equals CarCopy.CopyNumber
                        orderby loan.DateOut descending
                        select new
                        {
                            loan.DateOut
                        }
                    ).FirstOrDefault()
            };

        return View(await notLoanedCopyDetails.ToListAsync());
    }

}