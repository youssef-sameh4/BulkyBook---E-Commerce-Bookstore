using Bulky.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModel
{
    public  class ProductVM
    {
        public Product Product { get; set; }=new Product();
        public IEnumerable<SelectListItem> Categories { get; set; }=new List<SelectListItem>();

    }
}
