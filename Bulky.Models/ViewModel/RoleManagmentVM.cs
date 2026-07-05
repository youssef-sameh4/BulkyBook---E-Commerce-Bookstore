using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModel
{
    public class RoleManagmentVM
    {
        public ApplicationUser ApplicationUser { set; get; }
        public IEnumerable<SelectListItem> RoleList { set; get; }
    }
}
