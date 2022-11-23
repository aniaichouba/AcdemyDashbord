using AcdemyDashbord.Models;
using AcdemyDashbord.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace AcdemyDashbord.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public AccountController(UserManager<AppUser> userManager,
                                SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(AccountLoginViewModel model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return LocalRedirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Login invalid attempt");
            }
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(AccountRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (User.IsInRole("Admin") && signInManager.IsSignedIn(User))
                    {
                        return RedirectToAction(nameof(Users));
                    }
                    await signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        [AllowAnonymous]
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckingExistingEmail(AccountLoginViewModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null) return Json(true);
            else return Json($"This email {model.Email} is already exist.");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(AccountGetEditBody data)
        {
            if (!string.IsNullOrWhiteSpace(data.Id))
            {
                AppUser user = await userManager.FindByIdAsync(data.Id);
                if (user != null)
                {
                    AccountEditViewModel model = new AccountEditViewModel()
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    };
                    return View(model);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AccountEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;

                    var passwordHash = userManager.PasswordHasher.HashPassword(user, model.Password);
                    user.PasswordHash = passwordHash;


                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }
        public IActionResult AccessDenied(string ReturnUrl)
        {
            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                ViewBag.ReturnUrl = ReturnUrl;
            }
            else
            {
                ViewBag.ReturnUrl = "";
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Users()
        {
            var users = userManager.Users.Where(user => user.Email != User.Identity.Name);
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View("NotFound", $"The User ID must be exist and not empty in URL !.");
            }

            AppUser user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return View("NotFound", $"The User as ID {id} cannot be found");
            }

            var userRoles = await userManager.GetRolesAsync(user);
            var userClaims = await userManager.GetClaimsAsync(user);

            AccountEditUserViewModel model = new AccountEditUserViewModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = userRoles,
                Claims = userClaims.Select(c => c.Value).ToList()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(AccountEditUserViewModel model)
        {
            AppUser user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return View("NotFound", $"The User as ID {model.Id} cannot be found");
            }
            if (ModelState.IsValid)
            {
                user.LastName = model.LastName;
                user.FirstName = model.FirstName;
                user.Email = model.Email;
                user.UserName = model.Email;

                IdentityResult result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Users));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            model.Roles = await userManager.GetRolesAsync(user);
            model.Claims = (await userManager.GetClaimsAsync(user)).Select(c => c.Value).ToList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return View("NotFound", $"The User as ID {id} cannot be found");
            }

            IdentityResult result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return RedirectToAction(nameof(Users));
        }
    }
}
