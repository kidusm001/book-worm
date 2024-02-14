using BookStore.Models;

namespace bookworm.Models
{
    public class PurchaseItem
    {
        public int Id { get; set; }
        public int PurchaseId { get; set; }
        public Purchase Purchase { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public decimal Price { get; set; }
    }
}
