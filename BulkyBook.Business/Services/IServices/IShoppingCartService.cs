using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Business.Services.IServices
{
    public  interface IShoppingCartService
    {
        Task  <ShoppingCart?> GetCartByIdAsync(int Id);
        Task<IEnumerable< ShoppingCart>> GetUserCartByIdAsync(string UserId);
        Task<int> GetCountCartByIdAsync(string UserId);
        Task<ShoppingCart> AddToCartAsync(ShoppingCart shoppingCart);
        Task  UpdateCartAsync(ShoppingCart shoppingCart);
        Task ClearCartAsync(string userId);




    }
}
