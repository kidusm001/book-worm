using bookworm.Data;
using bookworm.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net;

namespace bookworm.Data
{
    public class Seed
    {
        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                if (!await roleManager.RoleExistsAsync(UserRoles.Author))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Author));

                //Users
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                string adminUserEmail = "teddysmithdeveloper@gmail.com";

                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new ApplicationUser()
                    {
                        UserName = "teddysmithdev",
                        Email = adminUserEmail,
                        EmailConfirmed = true,
                        FullName = "mike man",
                        

                    };
                    await userManager.CreateAsync(newAdminUser, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }

                string appUserEmail = "user@etickets.com";

                var appUser = await userManager.FindByEmailAsync(appUserEmail);
                if (appUser == null)
                {
                    var newAppUser = new ApplicationUser()
                    {
                        UserName = "app-user",
                        Email = appUserEmail,
                        EmailConfirmed = true,
                        FullName = "mike man",
                        

                    };
                    await userManager.CreateAsync(newAppUser, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAppUser, UserRoles.User);
                }
                string authorUserEmail = "author@etickets.com";

                var authorUser = await userManager.FindByEmailAsync(appUserEmail);
                if (authorUser == null)
                {
                    var newAuthorUser = new ApplicationUser()
                    {
                        UserName = "Authour-user",
                        Email = authorUserEmail,
                        EmailConfirmed = true,
                        FullName = "mike man",
                        

                    };
                    await userManager.CreateAsync(newAuthorUser, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAuthorUser, UserRoles.Author);
                }
            }
        }

        public static void SeedCategory(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<BookStoreContext>();

                // Check if categories already exist
                if (context.Categories.Any())
                {
                    // Categories already seeded
                    return;
                }

                // Seed categories
                context.Categories.AddRange(
                    new Category { Name = "Fiction" },
                    new Category { Name = "Non-fiction" },
                    new Category { Name = "Science Fiction" },
                    new Category { Name = "Fantasy" },
                    new Category { Name = "Romance" },
                    new Category { Name = "Historical Fiction" },
                    new Category { Name = "Biography" },
                    new Category { Name = "Autobiography" },
                    new Category { Name = "Self help" },
                    new Category { Name = "Thriller" },
                    new Category { Name = "Horror" },
                    new Category { Name = "Poetry" },
                    new Category { Name = "Young adult" },
                    new Category { Name = "Children" },
                    new Category { Name = "Science" },
                    new Category { Name = "Philosophy" },
                    new Category { Name = "History" },
                    new Category { Name = "Travel" },
                    new Category { Name = "Art and photography" }

                    // Add more categories as needed
                );

                // Save changes to the database
                context.SaveChanges();
            }
        }
    }
}
