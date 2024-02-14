namespace bookworm.ViewModel
{
    public class DeleteBookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public DateTime DatePublished { get; set; }
        public decimal Price { get; set; }
        public string AuthorName { get; set; }
        public string CoverImagePath { get; set; }
        public string FilePath { get; set; }
        public bool? IsDiscounted { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string CategoryName { get; set; }
    }
}
