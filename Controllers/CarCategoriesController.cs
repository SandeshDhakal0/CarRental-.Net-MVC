using Microsoft.AspNetCore.Mvc;
using HamroCarRental.Data.Services;
using HamroCarRental.Models;

namespace HamroCarRental.Controllers;

public class CarCategoriesController : Controller
{
    //injecting services into the controller
    private readonly ICarCategoriesService _service;

    //defining a constructor
    public CarCategoriesController(ICarCategoriesService service)
    {
        _service = service;
    }

    //get CarCategories
    public async Task<IActionResult> Index()
    {
        var allCarCategories = await _service.GetAllAsync();
        return View(allCarCategories);
    }

    //Get: Brands/details/id
    public async Task<IActionResult> Details(int id)
    {
        var categoryDetails = await _service.GetCarCategoryAsync(id);

        if (categoryDetails == null) return View("NotFound");
        return View(categoryDetails);
    }

    //Get: CarCategories/Create
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [Bind("CategoryName, CategoryDescription, AgeRestricted")] CarCategory CarCategory)
    {
        await _service.AddAsync(CarCategory);
        return RedirectToAction(nameof(Index));
    }

    //Get: CarCategories/edit/id
    public async Task<IActionResult> Edit(int id)
    {
        var categoryDetails = await _service.GetCarCategoryAsync(id);
        if (categoryDetails == null) return View("NotFound");
        return View(categoryDetails);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id,
        [Bind("CategoryNumber, CategoryName, CategoryDescription, AgeRestricted")] CarCategory CarCategory)
    {
        if (id == CarCategory.CategoryNumber)
        {
            await _service.UpdateAsync(id, CarCategory);
            return RedirectToAction(nameof(Index));
        }

        return View(CarCategory);
    }

    //Get: Actors/delete/id
    public async Task<IActionResult> Delete(int id)
    {
        var categoryDetails = await _service.GetCarCategoryAsync(id);
        if (categoryDetails == null) return View("NotFound");
        return View(categoryDetails);
    }

    [HttpPost]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var categoryDetails = await _service.GetCarCategoryAsync(id);
        if (categoryDetails == null) return View("NotFound");

        await _service.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}