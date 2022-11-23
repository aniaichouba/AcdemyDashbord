using AcdemyDashbord.Models;
using AcdemyDashbord.Models.Types;
using AcdemyDashbord.Tools;
using AcdemyDashbord.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AcdemyDashbord.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public readonly UserManager<AppUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                        UserManager<AppUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(AdministrationCreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole()
                {
                    Name = model.RoleName
                };
                IdentityResult result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction(actionName: "Roles");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Roles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id != null)
            {
                var role = await roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    EditRoleViewModel model = new EditRoleViewModel()
                    {
                        Id = role.Id,
                        RoleName = role.Name,
                        Users = new List<string>()
                    };
                    foreach (AppUser user in userManager.Users)
                    {
                        if (await userManager.IsInRoleAsync(user, role.Name))
                        {
                            model.Users.Add(user.Email);
                        };
                    }
                    return View(model);
                }
                StatusResult result = new StatusResult()
                {
                    Message = $"The Role as ID {id} cannot be found"
                };
                return View("../Error/NotFound", result);
            }
            return RedirectToAction("Roles");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await roleManager.FindByIdAsync(model.Id);
                if (role == null)
                {
                    StatusResult status = new StatusResult()
                    {
                        Message = $"The Role as ID {model.Id} cannot be found"
                    };
                    return View("../Error/NotFound", status);
                }

                role.Name = model.RoleName;
                IdentityResult result = await this.roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View(model);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckingRoleName(EditRoleViewModel model)
        {
            var roleById = await roleManager.FindByIdAsync(model.Id);
            var roleByName = await roleManager.FindByNameAsync(model.RoleName);
            if (roleByName == null || roleById.Name == roleByName.Name) return Json(true);
            else return Json($"This role name ( {model.RoleName} ) is already exist.");
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersRole(string idRole)
        {
            if (string.IsNullOrEmpty(idRole))
            {
                return View("NotFound", $"The role must be exist and not empty in URL !.");
            }
            var role = await roleManager.FindByIdAsync(idRole);
            if (role == null)
            {
                return View("NotFound", $"The Role as ID {idRole} cannot be found");
            }

            List<EditUsersRoleViewModel> models = new List<EditUsersRoleViewModel>();
            foreach (AppUser user in userManager.Users)
            {
                EditUsersRoleViewModel model = new EditUsersRoleViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    IsSelected = false
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.IsSelected = true;
                }
                models.Add(model);
            }
            ViewBag.IdRole = idRole;
            ViewBag.RoleName = role.Name;
            return View(models);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersRole(List<EditUsersRoleViewModel> models, string idRole)
        {
            if (string.IsNullOrEmpty(idRole))
            {
                return View("NotFound", $"The role must be exist and not empty in URL !.");
            }
            var role = await roleManager.FindByIdAsync(idRole);
            if (role == null)
            {
                return View("NotFound", $"The Role as ID {idRole} cannot be found");
            }
            foreach (var model in models)
            {
                IdentityResult result = null;
                AppUser user = await userManager.FindByIdAsync(model.UserId);
                if (await userManager.IsInRoleAsync(user, role.Name) && !model.IsSelected)
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else if (!(await userManager.IsInRoleAsync(user, role.Name)) && model.IsSelected)
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                if (result != null && !result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return RedirectToAction(nameof(Edit), new { id = idRole });
        }
    }
}
