using BookStore.Models;
using bookworm.Models;

namespace bookworm.ViewModel
{
    public class SearchViewModel
    {
        public string SearchTerm { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public List<Book>? books { get; set; }
        public List<int>? GenreIds { get; set; }
        
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
