using System.ComponentModel.DataAnnotations;

namespace bookworm.ViewModel
{
    public class DeleteAccountViewModel
    {
        [Required]
        public string UserId { get; set; }

        
        public string? FullName { get; set; }

        
        public string? ProfilePicturePath { get; set; }
    }
}
