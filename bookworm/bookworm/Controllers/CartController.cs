using bookworm.Data;
using bookworm.Interfaces;
using bookworm.Models;
using bookworm.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

using System.Security.Claims;

namespace bookworm.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IBookRepository _bookRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BookStoreContext _context;
        private readonly StripeSetting _stripeSettings;
        public CartController(ICartService cartService, IBookRepository bookRepository, UserManager<ApplicationUser> userManager, BookStoreContext context, IOptions<StripeSetting> stripeSettings)
        {
            _cartService = cartService;
            _bookRepository = bookRepository;
            _userManager = userManager;
            _context = context;
            _stripeSettings = stripeSettings.Value;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int bookId)
        {
            // Get the currently logged-in user
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                // Handle the case where the current user is not found
                return RedirectToAction("Login");
            }

            // Add the book to the cart
            await _cartService.AddToCartAsync(bookId, currentUser.Id);

            // Redirect to the cart view or another appropriate action
            return RedirectToAction("MyCart");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int itemId)
        {
            // Get the currently logged-in user
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                // Handle the case where the current user is not found
                return RedirectToAction("Login");
            }

            // Remove the item from the cart
            await _cartService.RemoveFromCartAsync(itemId, currentUser.Id);

            // Redirect to the cart view or another appropriate action
            return RedirectToAction("MyCart");
        }

        [Authorize]
        public async Task<IActionResult> MyCart()
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);

            // Find the user's cart including the associated books
            var cart = await _context.Carts
                .Where(c => c.UserId == user.Id)
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync();

            return View(cart);
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);

            // Find the user's cart including the associated books
            var cart = await _context.Carts
                .Where(c => c.UserId == user.Id)
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync();

            // Calculate total price
            decimal totalPrice = cart.Items.Sum(item => item.Book.Price);

            // Create a new purchase entity
            var purchase = new Purchase
            {
                UserId = user.Id,
                PurchaseDate = DateTime.Now,
                TotalPrice = totalPrice,
                PurchaseItems = cart.Items.Select(ci => new PurchaseItem
                {
                    BookId = ci.BookId,
                    Price = ci.Book.Price
                }).ToList()
            };

            // Remove cart items
            _context.CartItems.RemoveRange(cart.Items);

            // Save the purchase and remove the cart items
            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Book");
        }

        public async Task<IActionResult> CreateCheckoutSession(string amount)
        {
            var currency = "usd";
            var successUrl = "https://localhost:7011/Cart/Success";
            var cancelUrl = "https://localhost:7011/Cart/Cancel";
            StripeConfiguration.ApiKey = "sk_test_51Ol7ZzJw3J6sb7xvNdT1LvtDCP28Ga9WOVnuGUEsMcsvOxXOQ299167AHzWoSvyhsHy8k8rp6GLQ4Cxo9tVa3pTw00yAi3NagR";

            // Get the current user
            var user = await _userManager.GetUserAsync(User);

            // Find the user's cart including the associated books
            var cart = await _context.Carts
                .Where(c => c.UserId == user.Id)
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync();

            // Calculate total price
            decimal totalPrice = cart.Items.Sum(item => item.Book.Price);

            var options = new SessionCreateOptions
            {
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = cart.Items.Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = currency,
                        UnitAmountDecimal = (long)item.Book.Price * 100, // Convert to cents
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Book.Title,
                            Description = item.Book.Description
                        }
                    },
                    Quantity = 1
                }).ToList(),
                Mode = "payment",
            };

            var service = new SessionService();
            var session = service.Create(options);
            
            return Redirect(session.Url);
        }

        public async Task<IActionResult> Success()
        {
            return RedirectToAction("Checkout");
        }

        public async Task<IActionResult> Cancel()
        {
            return RedirectToAction("MyCart");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
