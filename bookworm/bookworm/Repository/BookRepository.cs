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
            // Retrieve the book including its associated reviews
            return _context.Books
        .Include(book => book.Reviews) // Include the reviews navigation property
        .FirstOrDefault(book => book.Id == id);
        }

    }
}
