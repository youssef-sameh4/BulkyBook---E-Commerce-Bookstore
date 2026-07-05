using Bulky.DataAccess.Data;

using Bulky.Models.Models;
using BulkyBook.Business.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Business.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetByIDAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            return category;
        }
        public async Task<Category> CreateCategoryAsync(Category category)
        {
            _context.Add(category); 
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if(category==null)
            {
                throw new KeyNotFoundException($"Category with id {id} not found.");
            }
           _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsCategoryNameUniqueAsync(string name, int? id = null)
        {
            if(id.HasValue)
            {
                return await _context.Categories.AnyAsync(c => c.Name.ToLower() ==name.ToLower() && c.Id != id.Value);
            }
            else
            {
                return await _context.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower());
            }
        }
    }
}
