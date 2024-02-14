using BookStore.Models;

namespace bookworm.Models
{
    public class Purchase
    {
    public int Id { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public DateTime PurchaseDate { get; set; }
    public decimal TotalPrice { get; set; }
    public ICollection<PurchaseItem> PurchaseItems { get; set; }
    }
}
