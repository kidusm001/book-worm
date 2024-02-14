using BookStore.Models;
using bookworm.Models;

namespace bookworm.ViewModel
{
    public class BookDetailsViewModel
    {
        public Book Book { get; set; }
        public List<Review> Reviews { get; set; }
    }

}
