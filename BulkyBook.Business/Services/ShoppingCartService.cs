using Bulky.DataAccess.Data;
using Bulky.Models;
using BulkyBook.Business.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Business.Services
{
   public class ShoppingCartService : IShoppingCartService
    {
        private readonly AppDbContext _context;

        public ShoppingCartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ShoppingCart> AddToCartAsync(ShoppingCart shoppingCart)
        {
            var existingItem = await _context.ShoppingCarts.Include(p => p.Product).FirstOrDefaultAsync(s => s.ApplicationUserId == shoppingCart.ApplicationUserId
            && s.ProductId == shoppingCart.ProductId);
            if(existingItem!=null)
            {
                existingItem.Count += shoppingCart.Count;
              await _context.SaveChangesAsync();
                return existingItem;
            }
            else
            {
                _context.ShoppingCarts.Add(shoppingCart);
                await _context.SaveChangesAsync();

               
                return shoppingCart;
            }
        }

        public async Task ClearCartAsync(string userId)
        {
            var cartItem =await _context.ShoppingCarts.Include(p => p.Product).Where(s => s.ApplicationUserId == userId).ToListAsync();
            if (cartItem.Any())
            {
                _context.ShoppingCarts.RemoveRange(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ShoppingCart?> GetCartByIdAsync(int Id)
        {
            return await _context.ShoppingCarts.Include(p => p.Product).FirstOrDefaultAsync(s => s.Id == Id);
        }

        public async Task<int> GetCountCartByIdAsync(string UserId)
        {
            return await _context.ShoppingCarts.Where(u => u.ApplicationUserId == UserId).SumAsync(u => u.Count);
        }

        public async Task<IEnumerable<ShoppingCart>> GetUserCartByIdAsync(string UserId)
        {
            return await _context.ShoppingCarts.Include(u => u.Product).Where(s => s.ApplicationUserId == UserId).ToListAsync();
        }

        public async Task UpdateCartAsync(ShoppingCart shoppingCart)
        {
            if(shoppingCart.Count<=0)
            {
                _context.Remove(shoppingCart);
              
            }
            else
            {
                _context.ShoppingCarts.Update(shoppingCart);
            }
           await _context.SaveChangesAsync();
        }
    }
}
