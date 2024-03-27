using bookworm.Models;
using bookworm.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace bookworm.Controllers
{
    public class AccountController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment webHostEnvironment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUser = await userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                // Handle the case where the current user is not found
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpGet]
        public IActionResult ManageUser()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Copy data from RegisterViewModel to IdentityUser
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                };
                //if (model.ProfilePicture != null)
                //{
                //    // Generate a unique file name to avoid conflicts
                //    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfilePicture.FileName;

                //    // Construct the file path
                //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile_pictures", uniqueFileName);

                //    // Copy the uploaded file to the file path
                //    using (var stream = new FileStream(filePath, FileMode.Create))
                //    {
                //        await model.ProfilePicture.CopyToAsync(stream);
                //    }

                //    // Set the ProfilePicturePath property to the relative URL of the saved image
                //    user.ProfilePicturePath = "~/profile_pictures/" + uniqueFileName;
                //}


                // Store user data in AspNetUsers database table
                var result = await userManager.CreateAsync(user, model.Password);

                // If user is successfully created, sign-in the user using
                // SignInManager and redirect to index action of HomeController
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "user");
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "book");
                }

                // If there are any errors, add them to the ModelState object
                // which will be displayed by the validation summary tag helper
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LogInViewModel model = new LogInViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LogInViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email,
                    model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "book");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);


        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                                    new { ReturnUrl = returnUrl });

            var properties =
                signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult>
    ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            LogInViewModel loginViewModel = new LogInViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins =
                        (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState
                    .AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return View("Login", loginViewModel);
            }

            // Get the login information about the user from the external login provider
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState
                    .AddModelError(string.Empty, "Error loading external login information.");

                return View("Login", loginViewModel);
            }

            // If the user already has a login (i.e if there is a record in AspNetUserLogins
            // table) then sign-in the user with this external login provider
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            // If there is no record in AspNetUserLogins table, the user may not have
            // a local account
            else
            {
                // Get the email claim value
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                

                if (email != null)
                {
                    // Create a new user without password if we do not have a user already
                    var user = await userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            FullName = info.Principal.FindFirstValue(ClaimTypes.Name),
                            ProfilePicturePath= info.Principal.FindFirstValue("picture")
                        };
                        
                        await userManager.CreateAsync(user);
                        await userManager.AddToRoleAsync(user, "user");
                    }

                    // Add a login (i.e insert a row for the user in AspNetUserLogins table)
                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                // If we cannot find the user email we cannot continue
                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on Pragim@PragimTech.com";

                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            // Get the current user
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }

            // Create an EditAccountViewModel and populate it with user data
            var viewModel = new EditAccountViewModel
            {
                Id = currentUser.Id,
                FullName = currentUser.FullName,
                Email = currentUser.Email,
                PhoneNumber = currentUser.PhoneNumber,
                ProfilePictureUrl = currentUser.ProfilePicturePath // Assuming you have a ProfilePictureUrl property in your ApplicationUser model
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditforAdmin(string id)
        {
            // Get the current user
            var currentUser = await userManager.FindByIdAsync(id);
            if (currentUser == null)
            {
                return NotFound();
            }

            // Create an EditAccountViewModel and populate it with user data
            var viewModel = new EditAccountViewModel
            {
                Id = currentUser.Id,
                FullName = currentUser.FullName,
                Email = currentUser.Email,
                PhoneNumber = currentUser.PhoneNumber,
                ProfilePictureUrl = currentUser.ProfilePicturePath // Assuming you have a ProfilePictureUrl property in your ApplicationUser model
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditAccountViewModel model)
        {
            // Get the current user
            var user = await userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                // Update user properties with the values from the form
                user.FullName = model.FullName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;

                // Delete existing profile picture if any
                if (!string.IsNullOrEmpty(user.ProfilePicturePath))
                {
                    var existingImagePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfilePicturePath.TrimStart('/'));
                    if (System.IO.File.Exists(existingImagePath))
                    {
                        System.IO.File.Delete(existingImagePath);
                    }
                }

                // Save profile picture if provided
                if (model.ProfilePicture != null)
                {
                    // Generate a unique file name for the profile picture
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfilePicture.FileName;

                    // Set the path to save the profile picture
                    var profilePicturePath = Path.Combine(_webHostEnvironment.WebRootPath, "profile_pictures", uniqueFileName);

                    // Copy the profile picture to the wwwroot/profile_pictures folder
                    using (var stream = new FileStream(profilePicturePath, FileMode.Create))
                    {
                        await model.ProfilePicture.CopyToAsync(stream);
                    }

                    // Update the user's ProfilePicturePath property with the saved path
                    user.ProfilePicturePath = "/profile_pictures/" + uniqueFileName;
                }

                // Update the user in the database
                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Redirect to the appropriate action
                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    // Handle errors if the user update fails
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            // If model state is not valid, redisplay the form with validation errors
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditforAdmin(EditAccountViewModel model)
        {
            // Get the current user
            var user = await userManager.FindByIdAsync(model.Id);

            if (ModelState.IsValid)
            {
                // Update user properties with the values from the form
                user.FullName = model.FullName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;

                // Delete existing profile picture if any
                if (!string.IsNullOrEmpty(user.ProfilePicturePath))
                {
                    var existingImagePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfilePicturePath.TrimStart('/'));
                    if (System.IO.File.Exists(existingImagePath))
                    {
                        System.IO.File.Delete(existingImagePath);
                    }
                }

                // Save profile picture if provided
                if (model.ProfilePicture != null)
                {
                    // Generate a unique file name for the profile picture
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfilePicture.FileName;

                    // Set the path to save the profile picture
                    var profilePicturePath = Path.Combine(_webHostEnvironment.WebRootPath, "profile_pictures", uniqueFileName);

                    // Copy the profile picture to the wwwroot/profile_pictures folder
                    using (var stream = new FileStream(profilePicturePath, FileMode.Create))
                    {
                        await model.ProfilePicture.CopyToAsync(stream);
                    }

                    // Update the user's ProfilePicturePath property with the saved path
                    user.ProfilePicturePath = "/profile_pictures/" + uniqueFileName;
                }

                // Update the user in the database
                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Redirect to the appropriate action
                    return RedirectToAction("AdminPanel", "Book");
                }
                else
                {
                    // Handle errors if the user update fails
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            // If model state is not valid, redisplay the form with validation errors
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete()
        {
            // Get the current user
            var currentUser = await userManager.GetUserAsync(User);

            // Ensure the user is authenticated
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated
            }

            // Create a DeleteAccountViewModel to pass necessary information to the view
            var model = new DeleteAccountViewModel
            {
                UserId = currentUser.Id,
                FullName = currentUser.FullName, // Assuming you have a property for the user's full name
                ProfilePicturePath = currentUser.ProfilePicturePath // Assuming you have a property for the profile picture path
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DeleteAccountViewModel model)
        {
            // Ensure the model state is valid
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get the user to be deleted
            var user = await userManager.FindByIdAsync(model.UserId);

            // Check if the user exists
            if (user == null)
            {
                return NotFound();
            }

            // Delete the profile picture if it exists
            if (!string.IsNullOrEmpty(model.ProfilePicturePath))
            {
                var profilePicturePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", model.ProfilePicturePath.TrimStart('~', '/'));
                if (System.IO.File.Exists(profilePicturePath))
                {
                    System.IO.File.Delete(profilePicturePath);
                }
            }

            // Delete the user
            var result = await userManager.DeleteAsync(user);

            // Check if the deletion was successful
            if (result.Succeeded)
            {
                // Redirect to the home page or another appropriate action
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // If the deletion was not successful, display an error message
                ModelState.AddModelError(string.Empty, "Error deleting user.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteForAdmin(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Ensure that the current user is authorized to delete the account
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser.Id != id && !User.IsInRole("admin"))
            {
                return Forbid();
            }
            var user = await userManager.FindByIdAsync(id);
            var viewModel = new DeleteAccountViewModel
            {
                UserId = user.Id,
                FullName = user.FullName
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteForAdminComfirm(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Ensure that the current user is authorized to delete the account
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser.Id != userId && !User.IsInRole("admin"))
            {
                return Forbid();
            }

            // Retrieve the profile picture path from the user's information
            var profilePicturePath = user.ProfilePicturePath;

            // Delete the profile picture file from the server if it exists
            if (!string.IsNullOrEmpty(profilePicturePath))
            {
                var profilePictureFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", profilePicturePath.TrimStart('/'));
                if (System.IO.File.Exists(profilePictureFullPath))
                {
                    System.IO.File.Delete(profilePictureFullPath);
                }
            }

            // Delete the user account
            var result = await  userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                // Sign out the user and redirect to the home page or another appropriate page
                await signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }

            // If deletion fails, display error messages
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            // If deletion fails, redisplay the delete confirmation view
            var viewModel = new DeleteAccountViewModel
            {
                UserId = userId
            };
            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AuthorLogin(string returnUrl)
        {
            LogInViewModel model = new LogInViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AuthorLogin(LogInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email,
                    model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(model.Email);
                    if (await userManager.IsInRoleAsync(user, "author"))
                    {
                       

                        if (!string.IsNullOrEmpty(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("AuthorDashboard", "Book");
                        }
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);


        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AdminLogin(string returnUrl)
        {
            LogInViewModel model = new LogInViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AdminLogin(LogInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email,
                    model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(model.Email);
                    if (await userManager.IsInRoleAsync(user, "admin"))
                    {


                        if (!string.IsNullOrEmpty(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("AdminPanel", "Book");
                        }
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);


        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AuthorRegister()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AuthorRegister(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FullName = model.FullName };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Assign the "Author" role to the user
                    await userManager.AddToRoleAsync(user, "author");

                    // Sign in the user after registration
                    await signInManager.SignInAsync(user, isPersistent: false);

                    // Redirect to author-specific dashboard or profile page
                    return RedirectToAction("AuthorDashboard", "Book");
                }
                // Handle registration errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // If model state is not valid, redisplay the registration form with errors
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        
        [ValidateAntiForgeryToken]
        public IActionResult CreateAdminUser()
        {
            return View();
        }



        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdminUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Assign the "Admin" role to the new user
                    await userManager.AddToRoleAsync(user, "admin");

                    // Redirect to a success page or admin panel
                    return RedirectToAction("AdminPanel", "Book");
                }
                // Handle registration errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // If model state is not valid, redisplay the registration form with errors
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AdminRegister()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminRegister(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FullName = model.FullName };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Assign the "Author" role to the user
                    await userManager.AddToRoleAsync(user, "admin");

                    // Sign in the user after registration
                    await signInManager.SignInAsync(user, isPersistent: false);

                    // Redirect to author-specific dashboard or profile page
                    return RedirectToAction("AdminPanel", "Book");
                }
                // Handle registration errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // If model state is not valid, redisplay the registration form with errors
            return View(model);
        }
        public async Task<IActionResult> ManageUsers()
        {
            // Retrieve all users from the database
            var users = await userManager.Users.ToListAsync();

            return View(users); // Pass the list of users to the view
        }

    }
}
