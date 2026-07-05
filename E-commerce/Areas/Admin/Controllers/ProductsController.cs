
using Bulky.Models;
using Bulky.Models.Models;
using Bulky.Models.ViewModel;
using BulkyBook.Business.Services.IServices;
using E_commerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using System.Threading.Tasks;

namespace E_commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductsController(IProductService productService, ICategoryService categoryService, IWebHostEnvironment hostEnvironment)
        {
            _productService = productService;
            _categoryService = categoryService;
            _hostEnvironment = hostEnvironment;
        }


        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }
        [HttpGet]

        public async Task<IActionResult> Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                Categories = (await _categoryService.GetAllCategoriesAsync()).Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() })

            };
            if (id == null || id == 0)
            {
                return View(productVM);

            }
            else
            {
                productVM.Product = await _productService.GetByIDAsync(id.Value);
                
                return View(productVM);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductVM productVM, IFormFile? file)
        {
          

            if (ModelState.IsValid)
            {
                string wweRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine("images", "products");
                    string FinalPath = Path.Combine(wweRootPath, productPath);
                    if (!Directory.Exists(FinalPath))
                    {
                        Directory.CreateDirectory(FinalPath);
                    }
                    using (var fileStream = new FileStream(Path.Combine(FinalPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = Path.Combine(@"\", productPath, fileName).Replace("\\", "/");
                }
                if(productVM.Product.Id==0|| productVM.Product.Id==null)
                {
                    await _productService.CreateProductAsync(productVM.Product);
                }
                else
                {
                                       await _productService.UpdateProductAsync(productVM.Product);
                }

                    TempData["success"] = "Product created successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVM.Categories = (await _categoryService.GetAllCategoriesAsync())
    .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });

                return View(productVM);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var product = await _productService.GetByIDAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
           
            if (!ModelState.IsValid)
            {
                return View(product);
            }   
            await _productService.UpdateProductAsync(product);
            TempData["success"] = "Product updated successfully!";
            return RedirectToAction(nameof(Index));
        }
       

        #region API CALLS
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult>GetAll()
        {
            var products = await _productService.GetAllProductsAsync(true);
            return Json(new { data = products });
        }

        [HttpDelete]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if(id==null||id==0)
            {
                return Json(new { success = false, message = "Invail Id" });
            }


            var product = await _productService.GetByIDAsync(id.Value);
            if(product==null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\', '/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            await _productService.DeleteProductAsync(id.Value);
            
            return Json(new { success = true, message = "Product deleted successfully" });
        }
        #endregion
    }
}
