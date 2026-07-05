using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModel
{
    public class DashboardVM
    {
        public int TotalOrders { set; get; }
        public int TotalUsers { set; get; }
        public int TotalProducts { set; get; } 
        public double TotalRevenue { get; set; }
    }
}
