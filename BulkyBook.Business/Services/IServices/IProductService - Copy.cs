using Bulky.Models;
using Bulky.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Business.Services.IServices
{
    public interface IProductService
    {
        Task<Product?> GetByIDAsync(int id, bool includeCategory = false);
        Task<IEnumerable<Product>> GetAllProductsAsync(bool includeCategory=false);
        Task<Product> CreateProductAsync(Product product);
      
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
       

    }
}
