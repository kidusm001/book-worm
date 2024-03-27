using System.ComponentModel.DataAnnotations;

namespace bookworm.ViewModel
{
    public class ReviewViewModel
    {
        [Required(ErrorMessage = "Please provide a rating.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Please provide a review.")]
        public string Content { get; set; }

        public int BookId { get; set; }

    }
}
