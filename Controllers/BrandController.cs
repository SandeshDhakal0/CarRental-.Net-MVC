using Microsoft.AspNetCore.Mvc;
using HamroCarRental.Data.Services;
using HamroCarRental.Models;

namespace HamroCarRental.Controllers;

public class BrandsController : Controller
{
    //injecting service to the controller
    private readonly IBrandsService _service;

    //defining a constructor
    public BrandsController(IBrandsService service)
    {
        _service = service;
    }

    //get Brand
    public async Task<IActionResult> Index()
    {
        var allBrands = await _service.GetAllAsync();
        return View(allBrands);
    }

    //Get: Brands/Create
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create([Bind("BrandPictureURL,BrandName")] Brand Brand)
    {
        await _service.AddAsync(Brand);
        return RedirectToAction(nameof(Index));
    }

    //Get: Brands/details/Id
    public async Task<IActionResult> Details(int id)
    {
        var BrandDetails = await _service.GetBrandAsync(id);

        if (BrandDetails == null) return View("NotFound");
        return View(BrandDetails);
    }

    //Get: Brands/edit
    public async Task<IActionResult> Edit(int id)
    {
        var BrandDetails = await _service.GetBrandAsync(id);
        if (BrandDetails == null) return View("NotFound");
        return View(BrandDetails);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Brand Brand)
    {
        await _service.UpdateAsync(id, Brand);
        return RedirectToAction(nameof(Index));
    }

    //Get: Brands/delete
    public async Task<IActionResult> Delete(int id)
    {
        var BrandDetails = await _service.GetBrandAsync(id);
        if (BrandDetails == null) return View("NotFound");
        return View(BrandDetails);
    }

    [HttpPost]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var BrandDetails = await _service.GetBrandAsync(id);
        if (BrandDetails == null) return View("NotFound");

        await _service.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}