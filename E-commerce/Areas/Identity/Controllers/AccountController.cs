using Bulky.Models;
using Bulky.Models.ViewModel;
using BulkyBook.Business.Services.IServices;
using E_commerce.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Data;

namespace E_commerce.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IShoppingCartService shoppingCartService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            _shoppingCartService = shoppingCartService;
        }

        public IActionResult Login(string?returnUrl=null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVm loginVm, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(loginVm.Email, loginVm.Password, loginVm.RememberMe,
                    lockoutOnFailure: false);
                if (result.Succeeded)
                {

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {

                        return Redirect(returnUrl);
                    }
                    if(User.IsInRole(SD.Role_Admin)||User.IsInRole(SD.Role_Employee))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

                    }
                    return RedirectToAction("Index", "Home", new { area = "Customer" });
                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(loginVm);
        }
        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {

            var registervm = new RegisterVM();
            if (User.IsInRole(SD.Role_Admin))
            {
                registervm.RoleList = [

                new SelectListItem{Text=SD.Role_Admin,Value=SD.Role_Admin},
                new SelectListItem{Text=SD.Role_Customer,Value=SD.Role_Customer},
                new SelectListItem{Text=SD.Role_Employee,Value=SD.Role_Employee}

                ];

            }
            else
            {
                registervm.RoleList = new List<SelectListItem>();
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View(registervm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM, string? returnUrl = null)
        {
            if (!await roleManager.RoleExistsAsync(SD.Role_Customer))
            {
                await roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
                await roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                await roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));


            }
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    Name = registerVM.Name,
                    StreetAddress = registerVM.StreetAddress,
                    City = registerVM.City,
                    State = registerVM.State,
                    PostalCode = registerVM.PostalCode,
                    Email = registerVM.Email,
                    UserName = registerVM.Email,
                    PhoneNumber = registerVM.PhoneNumber


                };
                var result = await userManager.CreateAsync(user, registerVM.Password);


                if (result.Succeeded)
                {
                    if (User.IsInRole(SD.Role_Admin) && !string.IsNullOrEmpty(registerVM.Role))
                    {
                        await userManager.AddToRoleAsync(user, registerVM.Role);
                        TempData["success"] = $"User '{user.Name}' created successfully with role '{registerVM.Role}'.";
                        return RedirectToAction("Index", "User", new { area = "Admin" });
                    }
                    else
                    {
                        await userManager.AddToRoleAsync(user, SD.Role_Customer);

                        //user has been created
                        await signInManager.SignInAsync(user, isPersistent: false);
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return RedirectToAction("Index", "Home", new { area = "Customer" });
                    }

                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            return View(registerVM);
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            HttpContext.Session.SetInt32(SD.SessionCart, 0);
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
