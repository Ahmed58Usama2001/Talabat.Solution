using AdminPanel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Talabat.Core.Entities.Identity;

namespace AdminPanel.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var usersQuery = _userManager.Users
                .Select(u => new AppUser
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    DisplayName = u.DisplayName,
                    Email = u.Email
                });

            var users = new List<UserViewModel>();

            foreach (var user in await usersQuery.ToListAsync())
            {
                var roles = await _userManager.GetRolesAsync(user);

                users.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    Roles = roles
                });
            }

            return View(users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user=await _userManager.FindByIdAsync(id);
            var allRoles=await _roleManager.Roles.ToListAsync();

            var viewModel = new UserRolesViewModel()
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = allRoles.Select(r => new RoleViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    IsSelected = _userManager.IsInRoleAsync(user, r.Name).Result
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            var userRole = await _userManager.GetRolesAsync(user);

            foreach (var role in model.Roles)
            {
                if(userRole.Any(r=>r==role.Name)&& !role.IsSelected)
                {
                    await _userManager.RemoveFromRoleAsync(user,role.Name);
                }

                if (!userRole.Any(r => r == role.Name) && role.IsSelected)
                {
                    await _userManager.AddToRoleAsync(user, role.Name);
                }
            }

            return RedirectToAction(nameof(Index));
        }

    }
}