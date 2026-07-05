using Bulky.Models;
using Bulky.Models.Models;
using BulkyBook.Business.Services.IServices;
using E_commerce.Models;
using E_commerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace E_commerce.Areas.Admin.Controllers.Controllers
{
    [Area("Customer")]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        public HomeController(ILogger<HomeController> logger, IProductService productService, IShoppingCartService shoppingCartService)
        {
            _logger = logger;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync(includeCategory: true);

            return View(products);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int productId)
        {
            var product=await _productService.GetByIDAsync(productId, includeCategory: true);
            if(product==null)
            {
                return NotFound();
            }
            ShoppingCart shopping = new ShoppingCart()
            { 
            Product= product,
            ProductId =  productId,
            Count=1
            };

            return View(shopping);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart cart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
           
            if (String.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            cart.ApplicationUserId = userId;
           await _shoppingCartService.AddToCartAsync(cart);
            var count =await _shoppingCartService.GetCountCartByIdAsync(userId);
            HttpContext.Session.SetInt32(SD.SessionCart, count);
            TempData["success"] = "Item added to cart";

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
