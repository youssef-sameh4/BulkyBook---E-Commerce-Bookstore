using Bulky.DataAccess.Data;
using Bulky.Models.ViewModel;
using E_commerce.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task <IActionResult> Index()
        {
            var orders =await _context.OrderHeaders.ToListAsync();
            var users = await _context.ApplicationUsers.CountAsync();
            var products = await _context.Products.CountAsync();
            DashboardVM dashboardVM = new DashboardVM
            {
                TotalOrders = orders.Count(),
                TotalUsers = users,
                TotalProducts=products,
                TotalRevenue=orders.Where(o=>o.OrderStatus==SD.StatusApproved||o.OrderStatus==SD.StatusShipped).Sum(o=>o.OrderTotal)

            };


            return View(dashboardVM);
        }
        [HttpGet]
        public async Task<IActionResult> GetChartData()
        {
            var orders = await _context.OrderHeaders.ToListAsync();
            var products = await _context.Products.Include(u => u.Category).ToListAsync();
            var categories = await _context.Categories.ToListAsync();


            //Revenue by month(last 6 months)
            var now = DateTime.UtcNow;
            var sixMonthsAgo = now.AddMonths(-5);
            var monthlyRevenue = Enumerable.Range(0, 6).Select(i =>
            {
                var month = sixMonthsAgo.AddMonths(i);
                var revenue = orders.Where(o => o.OrderDate.Year == month.Year && o.OrderDate.Month == month.Month
                && (o.OrderStatus == SD.StatusApproved || o.OrderStatus == SD.StatusShipped)).Sum(o => o.OrderTotal);

                return new { Label = month.ToString("MMM yyyy"), Revenue = revenue };
            }).ToList();

            // Orders by month (last 6 months)
            var monthlyOrders = Enumerable.Range(0, 6).Select(i =>
            {
                var month = sixMonthsAgo.AddMonths(i);
                var count = orders.Count(o => o.OrderDate.Year == month.Year && o.OrderDate.Month == month.Month);
                return new { Label = month.ToString("MMM yyyy"), Count = count };
            }).ToList();

            // Order status breakdown
            var statusBreakdown = orders
                .GroupBy(o => o.OrderStatus ?? "Unknown")
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            // Products per category
            var productsPerCategory = categories.Select(c => new
            {
                Category = c.Name,
                Count = products.Count(p => p.CategoryId == c.Id)
            }).ToList();



            return Json(new
            {
                monthlyRevenue,
                monthlyOrders,
                statusBreakdown,
                productsPerCategory
            });
        }
    }
}
