using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Business.Services.IServices
{
    public interface IOrderService
    {
        Task<OrderHeader> CreateOrderAsync(OrderHeader orderHeader);
        Task<OrderHeader?> GetOrderByIdAsync(int Id,bool includeUser=false,bool includeDetail=false);
        Task<IEnumerable<OrderHeader?>> GetAllOrderAsync (string ?userId=null ,string? status=null, bool includeUser = false, bool includeDetail = false);

        Task UpdateOrderAsync(OrderHeader orderHeader);
        Task UpdateOrderStatusAsync(int id, string orderStatus, string? carrier = null, string? trackingNumber = null);
        Task UpdateStripePaymentAsync(int orderId, string sessionId, string paymentIntentId);
        Task<string> CreateStripeCheckoutSessionAsync(OrderHeader orderHeader, IEnumerable<ShoppingCart> cartItems, string domain);
        Task<bool> VerifyStripePaymentAsync(OrderHeader orderHeader);
        Task<bool> CancelOrderWithRefundAsync(int orderId);
    }
}
