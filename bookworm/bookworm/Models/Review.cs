using BookStore.Models;

namespace bookworm.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Content { get; set; }
        public decimal Rating { get; set; }
        public DateTime DatePosted { get; set; }
    }
}
