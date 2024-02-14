using bookworm.Models;
using System.ComponentModel.DataAnnotations;

namespace bookworm.ViewModel
{
    public class CreateBookViewModel
    {
        [Required(ErrorMessage = "The Title field is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "The Description field is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The Language field is required.")]
        public string Language { get; set; }

        [Required(ErrorMessage = "The Date Published field is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date Published")]
        public DateTime DatePublished { get; set; }

        [Required(ErrorMessage = "The Price field is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "The Price must be greater than 0.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please select an author.")]
        public string AuthorId { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please upload a cover image.")]
        [Display(Name = "Cover Image")]
        public IFormFile CoverImage { get; set; }

        [Required(ErrorMessage = "Please upload an eBook file.")]
        [Display(Name = "eBook File")]
        public IFormFile EbookFile { get; set; }

        [Display(Name = "Is Discounted")]
        public bool? IsDiscounted { get; set; }

        [Display(Name = "Discount Amount")]
        [Range(0, 100, ErrorMessage = "Discount amount must be between 0 and 100.")]
        public decimal? DiscountAmount { get; set; }

        public IEnumerable<Category>? Categories { get; set; }
    }
}
