using System.ComponentModel.DataAnnotations;

namespace bookworm.ViewModel
{
    public class EditAccountViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Please enter your full name.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your phone number.")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please upload a cover image.")]
        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { get; set; }

        public string? ProfilePictureUrl { get; set; } // To display the current profile picture in the view
    }
}
