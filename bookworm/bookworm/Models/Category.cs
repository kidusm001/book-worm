using BookStore.Models;

namespace bookworm.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation property for books in this category
        public ICollection<Book> Books { get; set; }
    }
}
