using bookworm.Models;

namespace bookworm.Interfaces
{
    public interface IReviewRepository
    {
        IEnumerable<Review> GetReviewsForBook(int bookId);
    }
}
