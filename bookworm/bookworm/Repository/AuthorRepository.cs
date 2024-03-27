using bookworm.Data;
using bookworm.Interfaces;
using bookworm.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace bookworm.Repository
{
    public class AuthorRepository: IAuthorRepository
    {
        private readonly BookStoreContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthorRepository(BookStoreContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IEnumerable<ApplicationUser> GetFeaturedAuthors()
        {
            var authorRole = _context.Roles.SingleOrDefault(r => r.Name == "author");
            var authorRoleId = authorRole?.Id;
            var authorIds = _context.UserRoles
                    .Where(ur => ur.RoleId == authorRoleId) 
                    .Select(ur => ur.UserId)
                    .ToList();

            var featuredAuthors = _context.Users
    .Include(u => u.AuthoredBooks)
        .ThenInclude(b => b.PurchaseItems) // Include PurchaseItems for each AuthoredBook
    .Where(u => authorIds.Contains(u.Id) && u.AuthoredBooks != null)
    .ToList()
    .OrderByDescending(u => u.AuthoredBooks.Sum(b => b.PurchaseItems?.Count ?? 0))
    .Take(10)
    .ToList();


            return featuredAuthors;
        }

    }

}

