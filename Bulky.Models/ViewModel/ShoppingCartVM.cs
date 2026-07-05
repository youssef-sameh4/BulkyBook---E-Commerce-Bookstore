using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModel
{
    public  class ShoppingCartVM
    {
       public IEnumerable<ShoppingCart> shoppingCartList { set; get; }
        public OrderHeader OrderHeader { get; set; } = new();
    }
}
