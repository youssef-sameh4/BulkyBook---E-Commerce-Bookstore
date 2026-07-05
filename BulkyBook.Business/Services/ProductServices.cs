using Bulky.DataAccess.Data;

using Bulky.Models;
using BulkyBook.Business.Services.IServices;
using Microsoft.EntityFrameworkCore;


namespace BulkyBook.Business.Services
{
    public class ProductServices : IProductService
    {
        private readonly AppDbContext  _context;

        public ProductServices(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Product>> GetAllProductsAsync(bool includeCategory=false)
        {

            if(includeCategory==true)
            {
                return await _context.Products.Include(p=>p.Category).ToListAsync();
            }
            return await _context.Products.ToListAsync();
        }
        public async Task<Product> CreateProductAsync(Product product)
        {
          await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }
        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);

            await _context.SaveChangesAsync();
        
        }
        public async Task DeleteProductAsync(int id)
        {
            var product=_context.Products.Find(id);
            if(product==null)
            {
                throw new KeyNotFoundException($"Product with id {id} not found.");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();


        }

     

        public async Task<Product?> GetByIDAsync(int id, bool includeCategory = false)
        {
            if (includeCategory)
            {
                return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            }
            else
            {

                return await _context.Products.FindAsync(id);
            }
            }




        }
}
