using bookworm.Models;

namespace bookworm.Interfaces
{
    public interface ICartService { 

         Task<Cart> GetOrCreateCartAsync(string userId);
        Task AddToCartAsync(int bookId, string userId);
        Task<Cart> GetCartAsync(string userId);
        Task RemoveFromCartAsync(int itemId, string userId);
    }
}
