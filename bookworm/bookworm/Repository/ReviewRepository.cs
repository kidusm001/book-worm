using BookStore.Models;
using bookworm.Data;
using bookworm.Interfaces;
using bookworm.Models;
using Microsoft.EntityFrameworkCore;

namespace bookworm.Repository
{
    public class ReviewRepository:IReviewRepository
    {
        private readonly BookStoreContext _context;

        public ReviewRepository(BookStoreContext context)
        {
            _context = context;
        }

        public IEnumerable<Review> GetReviewsForBook(int bookId)
        {
            var reviews = _context.Reviews
                .Include(r => r.User) // Include the user navigation property
                .Where(r => r.BookId == bookId)
                .ToList();

            return reviews;
        }

    }
}
