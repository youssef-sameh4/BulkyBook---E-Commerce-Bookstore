using Bulky.Models;
using BulkyBook.Business.Services.IServices;
using E_commerce.Utility;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class OrderController : Controller
    {
        public readonly IOrderService _orderService;
        [BindProperty]
        public OrderHeader OrderHeader { get; set; }
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> UpdateOrderDetails()
        {
            var orderFromDb = await _orderService.GetOrderByIdAsync(OrderHeader.Id);
            orderFromDb.Name = OrderHeader.Name;
            orderFromDb.PhoneNumber = OrderHeader.PhoneNumber;
            orderFromDb.PostalCode = OrderHeader.PostalCode;
            orderFromDb.City = OrderHeader.City;
            orderFromDb.StreetAddress = OrderHeader.StreetAddress;
            orderFromDb.State = OrderHeader.State;
            if (!string.IsNullOrEmpty(OrderHeader.Carrier) && orderFromDb.OrderStatus == SD.StatusShipped)
            {
                orderFromDb.Carrier = OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderHeader.TrackingNumber) && orderFromDb.OrderStatus == SD.StatusShipped)
            {
                orderFromDb.TrackingNumber = OrderHeader.TrackingNumber;
            }
            await _orderService.UpdateOrderAsync(orderFromDb);

            TempData["Success"] = "Order Details Updated Successfully.";

            return RedirectToAction(nameof(Details), new { orderId = orderFromDb.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> UpdateOrderStatus(string status)
        {
            var orderHeader = await _orderService.GetOrderByIdAsync(OrderHeader.Id);
            if (orderHeader == null)
            {
                TempData["error"] = "Order not found.";
                return RedirectToAction(nameof(Index));
            }

            string successMessage;

            switch (status)
            {
                case SD.StatusInProcess:
                    await _orderService.UpdateOrderStatusAsync(OrderHeader.Id, status);
                    successMessage = "Order processing started successfully.";
                    break;
                case SD.StatusCancelled:
                case SD.StatusRefunded:
                    try
                    {
                        bool refundIssued = await _orderService.CancelOrderWithRefundAsync(OrderHeader.Id);
                        if (refundIssued)
                        {
                            successMessage = "Order cancelled and refund issued successfully. Funds will be returned to customer within 5-10 business days.";
                        }
                        else
                        {
                            successMessage = "Order cancelled successfully. (No payment was processed)";
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        // Business rule violation (e.g., trying to cancel shipped order)
                        TempData["error"] = ex.Message;
                        return RedirectToAction(nameof(Details), new { orderId = OrderHeader.Id });
                    }
                    catch (Stripe.StripeException ex)
                    {
                        // Refund failed - order is still cancelled but admin needs to manually refund
                        TempData["error"] = $"Order cancelled but refund failed: {ex.Message}. Please process refund manually in Stripe Dashboard.";
                        return RedirectToAction(nameof(Details), new { orderId = OrderHeader.Id });
                    }

                    break;
                case SD.StatusShipped:

                    if (string.IsNullOrEmpty(OrderHeader.Carrier) || string.IsNullOrEmpty(OrderHeader.TrackingNumber))
                    {
                        TempData["error"] = "Please provide both carrier and tracking number.";
                        return RedirectToAction(nameof(Details), new { orderId = OrderHeader.Id });
                    }

                    await _orderService.UpdateOrderStatusAsync(
                        OrderHeader.Id, SD.StatusShipped, OrderHeader.Carrier, OrderHeader.TrackingNumber);
                    successMessage = "Order shipped successfully.";
                    break;

                default:
                    TempData["error"] = "Invalid status update.";
                    return RedirectToAction(nameof(Details), new { orderId = OrderHeader.Id });
            }

            TempData["Success"] = successMessage;

            return RedirectToAction(nameof(Details), new { orderId = OrderHeader.Id });

        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int orderId)
        {
            OrderHeader = await _orderService.GetOrderByIdAsync(orderId, true,true);
            return View(OrderHeader);

        }
        #region API CALLS
        [AllowAnonymous]
        public async Task<IActionResult>GetAll(string status)
        {
            string? userId = null;
            if (!User.IsInRole(SD.Role_Admin) && !User.IsInRole(SD.Role_Employee))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
            }
            var orders = await _orderService.GetAllOrderAsync(userId, status);
            return Json(new { data = orders });
        }
        
        #endregion
    }
}
