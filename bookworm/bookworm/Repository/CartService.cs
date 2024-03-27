using bookworm.Data;
using bookworm.Interfaces;
using bookworm.Models;
using Microsoft.EntityFrameworkCore;

namespace bookworm.Repository
{
    public class CartService:ICartService
    {
        private readonly BookStoreContext _context;
        public CartService(BookStoreContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetOrCreateCartAsync(string userId)
        {
            var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, Items = new List<CartItem>() };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task AddToCartAsync(int bookId, string userId)
        {
            var cart = await GetOrCreateCartAsync(userId);

            if (cart.Items.Any(item => item.BookId == bookId))
            {
                // Book already exists in the cart
                return;
            }

            cart.Items.Add(new CartItem { BookId = bookId });
            await _context.SaveChangesAsync();
        }

        public async Task<Cart> GetCartAsync(string userId)
        {
            return await GetOrCreateCartAsync(userId);
        }

        public async Task RemoveFromCartAsync(int itemId, string userId)
        {
            // Find the user's cart
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                // Find the cart item by its ID
                var cartItem = cart.Items.FirstOrDefault(item => item.Id == itemId);

                if (cartItem != null)
                {
                    // Remove the cart item from the cart
                    cart.Items.Remove(cartItem);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Handle the case where the cart item is not found
                    throw new ApplicationException("Cart item not found.");
                }
            }
            else
            {
                // Handle the case where the cart is not found
                throw new ApplicationException("Cart not found.");
            }
        }

    }
}
