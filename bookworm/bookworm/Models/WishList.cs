using BookStore.Models;

namespace bookworm.Models
{
    public class WishList
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
