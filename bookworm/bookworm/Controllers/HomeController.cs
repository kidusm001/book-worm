using bookworm.Data;
using bookworm.Interfaces;
using bookworm.Models;
using bookworm.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace bookworm.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly BookStoreContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookRepository _bookRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IAuthorRepository _authorRepository;
        public HomeController(ILogger<HomeController> logger, BookStoreContext context, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager, IBookRepository bookRepository, IReviewRepository reviewRepository, IAuthorRepository authorRepository)
        {
            _logger = logger;
            _context = context;
            this.webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _bookRepository = bookRepository;
            _reviewRepository = reviewRepository;
            _authorRepository = authorRepository;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null)
            {
                // Handle the case where the current user is not found
                return RedirectToAction("Index", "Book");
            }
            // Get popular books (e.g., top rated or most reviewed)
            var popularBooks = _bookRepository.GetPopularBooks();

            // Get new books (e.g., recently published)
            var newBooks = _bookRepository.GetNewBooks();

            // Get special offer books (e.g., discounted books)
            var specialOfferBooks = _bookRepository.GetSpecialOfferBooks();

            // Get top selling book and its reviews
            var topSellingBook = _bookRepository.GetTopSellingBook();
            var topSellingBookReviews = _reviewRepository.GetReviewsForBook(topSellingBook.Id);

            // Get featured authors based on book sales
            var featuredAuthors = _authorRepository.GetFeaturedAuthors();

            var viewModel = new IndexViewModel
            {
                PopularBooks = popularBooks,
                NewBooks = newBooks,
                SpecialOfferBooks = specialOfferBooks,
                TopSellingBook = topSellingBook,
                TopSellingBookReviews = topSellingBookReviews,
                FeaturedAuthors = featuredAuthors
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
