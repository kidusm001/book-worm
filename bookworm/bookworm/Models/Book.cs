using bookworm.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        
        public string Description { get; set; }
        public string Language { get; set; }


        [Required,
        DataType(DataType.Date),
        Display(Name = "Date Published")]
        public DateTime DatePublished { get; set; }

        [Required,
        DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public string AuthorId { get; set; } // Reference to the author's user ID
        public ApplicationUser Author { get; set; } // Navigation property

        [Required]
        public string CoverImagePath { get; set; } // Store the path of the cover image
        [Required]
        public string FilePath { get; set; } // Store the path of the eBook file (PDF or ePUB)

        public bool? IsDiscounted { get; set; } // Indicates if the book is discounted
        public decimal? DiscountAmount { get; set; } // The discount amount (e.g., 10.0 for 10% discount)

        // Foreign key property
        public int CategoryId { get; set; }

        // Navigation property for the category of this book
        public Category Category { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
