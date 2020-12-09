using Hairdresser.Data;
using Hairdresser.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Hairdresser.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            var userRoles = _context.UserRoles.ToList();
            var roles = _context.Roles.ToList();

            List<UserRole> userRolesList = new List<UserRole>();

            foreach(IdentityUser user in users)
            {
                var roleId = "";
                var role = "";

                if(userRoles.Where(x => x.UserId == user.Id).Any())
                {
                    roleId = userRoles.Where(x => x.UserId == user.Id).First().RoleId;
                    role = roles.Where(x => x.Id == roleId).FirstOrDefault().Name;
                }

                userRolesList.Add(
                    new UserRole
                    {
                        User = user,
                        Role = role ?? null
                    });
            }

            if (User.IsInRole("Administrator"))
            {
                return View(userRolesList);
            }
            else
            {
                return Redirect("~/");
            }
        }
    }
}
