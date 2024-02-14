using bookworm.Models;
using bookworm.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace bookworm.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            return View(roleManager.Roles);
        }
        public IActionResult Create()
        {
            var model= new CreateRoleViewModel();
            model.roles = roleManager.Roles;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleViewModel model)
        {
            model.roles = roleManager.Roles;
            if (ModelState.IsValid)
            {
                IdentityResult result = await roleManager.
                    CreateAsync(new IdentityRole(model.name));
                if (result.Succeeded)
                {
                    model.name = "";
                    model.roles = roleManager.Roles;
                    return View(model);
                }
                   
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            // Find the role by id
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                // Handle the case where the role is not found, maybe return a not found view
                return NotFound();
            }

            var viewModel = new EditRoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
                Users = new List<string>()
            };

            // Get the users who belong to this role
            var usersInRole = userManager.Users.ToList(); // Materialize the collection
            foreach (var user in usersInRole)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    viewModel.Users.Add(user.UserName);
                }
            }


            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditRoleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Find the role by id
                var role = await roleManager.FindByIdAsync(viewModel.Id);
                if (role == null)
                {
                    // Handle the case where the role is not found, maybe return a not found view
                    return NotFound();
                }

                // Update the role name
                role.Name = viewModel.Name;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // If ModelState is not valid or if role update failed, return to the edit view with errors
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);

            var viewModel = new DeleteRoleViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name,
                UsersInRole = usersInRole.Select(u => u.UserName).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DeleteRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                return NotFound();
            }

            var result = await roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("Error");
            }
        }


    }
}
