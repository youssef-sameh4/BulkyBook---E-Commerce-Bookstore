using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Business.Services.IServices
{
    public  interface IApplicationUserService
    {
        public Task<ApplicationUser> GetById(string userId);
        public Task<IEnumerable<ApplicationUser>> GetAllApplicationUsers();

    }
}
