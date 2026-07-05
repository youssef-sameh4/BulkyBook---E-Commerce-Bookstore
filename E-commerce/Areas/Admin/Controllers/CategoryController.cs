using Bulky.DataAccess.Data;
using BulkyBook.Business.Services;
using BulkyBook.Business.Services.IServices;
using Bulky.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using E_commerce.Utility;


namespace E_commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =(SD.Role_Admin))]
    public class CategoryController : Controller
    {

       
        private readonly ICategoryService _categoryService;

        public CategoryController( ICategoryService categoryService )
        {
           
            _categoryService = categoryService;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }
        [HttpGet]
  
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!string.IsNullOrEmpty(category.Name) && await _categoryService.IsCategoryNameUniqueAsync(category.Name))
            {
                ModelState.AddModelError("", "Category name already exists!.");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            await _categoryService.CreateCategoryAsync(category);
            TempData["success"]= "Category created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id==null|| id ==0)
            {
                return NotFound();
            }
            var category= await _categoryService.GetByIDAsync(id.Value);
            if(category==null)
            {
                               return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        { 
            if(!string.IsNullOrEmpty(category.Name)&&await _categoryService.IsCategoryNameUniqueAsync(category.Name,category.Id))
            {
                ModelState.AddModelError("", "Category name already exists!.");
            }
            if(!ModelState.IsValid)
            {
                return View(category);
            }
            await _categoryService.UpdateCategoryAsync(category);
            TempData["success"]= "Category updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int ?id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var category = await _categoryService.GetByIDAsync(id.Value);
            if(category==null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost,ActionName("Delete")]
        public async Task<IActionResult> Delete(int  id)
        {

            var category = await _categoryService.GetByIDAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            await _categoryService.DeleteCategoryAsync(id);
            TempData["success"] = "Category deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
