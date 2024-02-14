using BookStore.Models;

namespace bookworm.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
