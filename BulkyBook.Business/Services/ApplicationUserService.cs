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
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly AppDbContext _context;

        public ApplicationUserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllApplicationUsers()
        {
            return await _context.ApplicationUsers.ToListAsync();
        }

        public async Task<ApplicationUser> GetById(string userId)
        {
            return await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id==userId);
        }
    }
}
