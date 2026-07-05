using Bulky.Models;
using Bulky.Models.ViewModel;
using BulkyBook.Business.Services.IServices;
using E_commerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IApplicationUserService _applicationUserService;

        public UserController(UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            IApplicationUserService applicationUserService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _applicationUserService = applicationUserService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> RoleManagment(string userId)
        {
            var user =await _applicationUserService.GetById(userId);
            if(user==null)
            {
                return Json(new { success = false, message = "User not found" });
            }
            RoleManagmentVM RoleVM =
                new RoleManagmentVM {
                    
                    ApplicationUser=user,
                     RoleList = _roleManager.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                     {
                         Text = u.Name,
                         Value = u.Name
                     })
                };

            RoleVM.ApplicationUser.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            return View(RoleVM);
        }
        [HttpPost]
        public async Task<IActionResult> RoleManagment(RoleManagmentVM roleManagmentVM)
        {
            var user = await _applicationUserService.GetById(roleManagmentVM.ApplicationUser.Id);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            string oldRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            if (!(roleManagmentVM.ApplicationUser.Role == oldRole))
            {
                //update Role
                await _userManager.RemoveFromRoleAsync(user, oldRole);
                await _userManager.AddToRoleAsync(user, roleManagmentVM.ApplicationUser.Role);
            }
            TempData["success"] = "Role has been updated";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ChangePassword(string userId)
        {
            var user = await _applicationUserService.GetById(userId);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            AdminChangePasswordVM adminChangePasswordVM = new AdminChangePasswordVM {
                UserEmail = user.Email,
                UserId = user.Id

            };

            return View(adminChangePasswordVM);
          
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(AdminChangePasswordVM adminChangePasswordVM)
        {
            if(!ModelState.IsValid)
            {
                return View(adminChangePasswordVM);
            }
            var user = await _applicationUserService.GetById(adminChangePasswordVM.UserId);

            if (user == null)
            {
                return NotFound();
            }
            var token =await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, adminChangePasswordVM.NewPassword);
            if (result.Succeeded)
            {
                TempData["success"] = $"Password for {user.Email} has been changed successfully.";
                return RedirectToAction(nameof(Index));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            adminChangePasswordVM.UserEmail = user.Email;
            return View(adminChangePasswordVM);
        }

        #region API CALLS
        public async Task< IActionResult> GetAll()
        {
            var users = await _applicationUserService.GetAllApplicationUsers();
            foreach(var user in users)
            {
                user.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            }
            return Json( new {data=users});
        }
        public async Task<IActionResult> LockUnlock([FromBody] string userId)
        {
            var user = await _applicationUserService.GetById(userId);
            if(user==null)
            {
                return Json(new { success = false, message = "User not found" });
            }
            if(await _userManager.IsLockedOutAsync(user))
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                return Json(new { success = true, message = "User unlocked successfully" });
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(20));
                return Json(new { success = true, message = "User locked successfully" });
            }
        }
        #endregion
    }
}
