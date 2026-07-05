using Bulky.Models;
using Bulky.Models.ViewModel;
using BulkyBook.Business.Services.IServices;
using E_commerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace E_commerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IApplicationUserService _applicationUserService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;

        public CartController(IApplicationUserService applicationUserService, IShoppingCartService shoppingCartService, IOrderService orderService = null)
        {
            _applicationUserService = applicationUserService;
            _shoppingCartService = shoppingCartService;
            _orderService = orderService;
        }


        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (String.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var carItems = await _shoppingCartService.GetUserCartByIdAsync(userId);
            var user = await _applicationUserService.GetById(userId);
            ShoppingCartVM shoppingCartVM = new()
            {
                shoppingCartList = carItems,
                OrderHeader=new()
            };
            shoppingCartVM.OrderHeader.ApplicationUser = user;
            shoppingCartVM.OrderHeader.Name = user.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = user.PhoneNumber;
            shoppingCartVM.OrderHeader.StreetAddress = user.StreetAddress;
            shoppingCartVM.OrderHeader.City = user.City;
            shoppingCartVM.OrderHeader.State = user.State;
            shoppingCartVM.OrderHeader.PostalCode = user.PostalCode;

            foreach (var cart  in shoppingCartVM.shoppingCartList)
            {

                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(shoppingCartVM);
        }
        [HttpPost]
        public async Task<IActionResult> Index(ShoppingCartVM shoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var cartItems = await _shoppingCartService.GetUserCartByIdAsync(userId);

            shoppingCartVM.shoppingCartList = cartItems;

            
      

            shoppingCartVM.OrderHeader.OrderDate = DateTime.UtcNow;
            shoppingCartVM.OrderHeader.ApplicationUserId = userId;

            foreach (var cart in shoppingCartVM.shoppingCartList)
            {
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            shoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;

            shoppingCartVM.OrderHeader.OrderDetails =
                shoppingCartVM.shoppingCartList.Select(cart => new OrderDetail
                {
                    ProductId = cart.ProductId,
                    Price = cart.Price,
                    Count = cart.Count,
                }).ToList();

            await _orderService.CreateOrderAsync(shoppingCartVM.OrderHeader);

            try
            {
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";

                var sessionUrl = await _orderService.CreateStripeCheckoutSessionAsync(shoppingCartVM.OrderHeader, shoppingCartVM.shoppingCartList, domain);

                await _shoppingCartService.ClearCartAsync(userId);
                HttpContext.Session.SetInt32(SD.SessionCart, 0);
                Response.Headers.Append("Location", sessionUrl);
                return new StatusCodeResult(303);

            }
            catch (StripeException ex)
            {
                TempData["error"] = "Payment processing failed. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }
      
        public async Task<IActionResult> Plus(int cartId)
        {
            var cart =await _shoppingCartService.GetCartByIdAsync(cartId);
            
            if(cart!=null)
            {
                cart.Count+=1;
               await _shoppingCartService.UpdateCartAsync(cart);
                await UpdateCartSessionAsync();
            }
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await _shoppingCartService.GetCartByIdAsync(cartId);
            if (cart != null)
            {
                cart.Count-=1;
                await _shoppingCartService.UpdateCartAsync(cart);
                await UpdateCartSessionAsync();
            }
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await _shoppingCartService.GetCartByIdAsync(cartId);
            if (cart != null)
            {
                cart.Count=0;
                await _shoppingCartService.UpdateCartAsync(cart);
            }
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> UpdateCart(int cartId, int count)
        {
            var cart = await _shoppingCartService.GetCartByIdAsync(cartId);
            if (cart == null)
            {
                return NotFound();
            }


            if (count <= 1)
            {
                cart.Count = 0;
            }
            else
            {
                if (count >= 1000)
                {
                    cart.Count = 1000;
                }
                else
                {
                    cart.Count = count;
                }
            }
            await _shoppingCartService.UpdateCartAsync(cart);
            await UpdateCartSessionAsync();
            return Ok(new { success = true });
        }

        private async Task UpdateCartSessionAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                var count = await _shoppingCartService.GetCountCartByIdAsync(userId);
                HttpContext.Session.SetInt32(SD.SessionCart, count);
            }
        }
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var orderHeader = await _orderService.GetOrderByIdAsync(id, includeUser: true);
            if (orderHeader == null)
            {
                return NotFound();
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            if (orderHeader.ApplicationUserId != userId)
            {
                return RedirectToAction("AccessDenied", "Account", new { area = "Identity" });
            }

            try
            {
                var result = await _orderService.VerifyStripePaymentAsync(orderHeader);

                if (result)
                {
                    TempData["success"] = "Payment completed successfully! Your order has been confirmed.";
                }
                else
                {
                    TempData["error"] = "Payment status is pending. Please contact support if you completed the payment.";
                }
            }
            catch (StripeException ex)
            {
                TempData["error"] = "Unable to verify payment status. Please contact support with your order number.";
            }

            var user = await _applicationUserService.GetById(userId);
            //await _emailService.SendOrderConfirmationEmailAsync(user.Email,
            //    orderHeader.Id, (decimal)orderHeader.OrderTotal);


            return View(id);
        }
    }
}
