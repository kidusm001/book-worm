using BookStore.Models;
using bookworm.Data;
using bookworm.Interfaces;
using bookworm.Models;
using Microsoft.EntityFrameworkCore;

namespace bookworm.Repository
{
    public class BookRepository:IBookRepository
    {
        private readonly BookStoreContext _context;

        public BookRepository(BookStoreContext context)
        {
            _context = context;
        }
        public IEnumerable<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        public void AddBook(Book book)
        {
            _context.Books.Add(book);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task<Book> GetBookByIdAsync(int id)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
        public async Task<bool> UpdateBookAsync(Book book)
        {
            try
            {
                _context.Books.Update(book);
                int result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return false;
            }
        }
        public void DeleteBook(Book book)
        {
            _context.Books.Remove(book);
        }

        public Book GetBookByIdWithReviews(int id)
        {
            // Retrieve the book including its associated reviews and users who posted them
            return _context.Books
                .Include(b => b.Author) // Include the author navigation property
                .Include(b => b.Category) // Include the Category navigation property
                .Include(b => b.Reviews) // Include the reviews navigation property
                    .ThenInclude(r => r.User) // Include the user who posted the review
                .FirstOrDefault(b => b.Id == id);
        }

        public IEnumerable<Book> GetPopularBooks()
        {
            var popularBooks = _context.Books
                .Select(book => new
                {
                    Book = book,
                    PurchasesCount = _context.PurchaseItems
                        .Count(pi => pi.BookId == book.Id),
                    ReviewsCount = book.Reviews.Count()
                })
                .OrderByDescending(item => item.PurchasesCount)
                .ThenByDescending(item => item.ReviewsCount)
                .Select(item => item.Book)
                .Take(10)
                .ToList();

            return popularBooks;
        }

        public IEnumerable<Book> GetNewBooks()
        {
            var newBooks = _context.Books
                .OrderByDescending(book => book.DatePublished)
                .Take(10)
                .ToList();

            return newBooks;
        }

        public IEnumerable<Book> GetSpecialOfferBooks()
        {
            var specialOfferBooks = _context.Books
                .Where(book => book.IsDiscounted == true)
                .Take(10)
                .ToList();

            return specialOfferBooks;
        }
        public Book GetTopSellingBook()
        {
            var topSellingBook = _context.Books
                .Select(book => new
                {
                    Book = book,
                    PurchasesCount = _context.PurchaseItems
                        .Count(pi => pi.BookId == book.Id),
                    ReviewsCount = book.Reviews.Count()
                })
                .OrderByDescending(item => item.PurchasesCount)
                .ThenByDescending(item => item.ReviewsCount)
                .Select(item => item.Book)
                .FirstOrDefault();

            return topSellingBook;
        }



    }
}
