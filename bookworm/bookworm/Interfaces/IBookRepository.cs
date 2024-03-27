using BookStore.Models;
using bookworm.Models;

namespace bookworm.Interfaces
{
    public interface IBookRepository
    {
        IEnumerable<Category> GetAllCategories();
        void AddBook(Book book);
        Task<int> SaveChangesAsync();
        Task<Book> GetBookByIdAsync(int id);
        Task<bool> UpdateBookAsync(Book book);
        void DeleteBook(Book book);
        Book GetBookByIdWithReviews(int id);
        IEnumerable<Book> GetPopularBooks();
        IEnumerable<Book> GetNewBooks();
        IEnumerable<Book> GetSpecialOfferBooks();
        Book GetTopSellingBook();
    }
}
