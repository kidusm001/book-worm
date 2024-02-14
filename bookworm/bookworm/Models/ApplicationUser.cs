using BookStore.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace bookworm.Models
{
    public class ApplicationUser:IdentityUser

    {
        [Required]
        public string FullName { get; set; }
        public string? ProfilePicturePath { get; set; }
        // Collections for users with the "User" role
        public ICollection<WishList>? Wishlist { get; set; }
        public ICollection<Purchase>? Purchases { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public Cart? Cart { get; set; }

        // Collections for users with the "Author" role
        public ICollection<Book>? AuthoredBooks { get; set; }
    }
}
