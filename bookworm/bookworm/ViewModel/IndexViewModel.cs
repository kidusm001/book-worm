using BookStore.Models;
using bookworm.Models;

namespace bookworm.ViewModel
{
    public class IndexViewModel
    {
        public IEnumerable<Book>? PopularBooks;
        public IEnumerable<Book>? NewBooks;
        public IEnumerable<Book>? SpecialOfferBooks;
        public Book? TopSellingBook;
        public IEnumerable<Review>? TopSellingBookReviews;
        public IEnumerable<ApplicationUser>? FeaturedAuthors ;
    }
}
