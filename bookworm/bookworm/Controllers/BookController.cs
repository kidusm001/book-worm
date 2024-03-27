using BookStore.Models;
using bookworm.Data;
using bookworm.Interfaces;
using bookworm.Models;
using bookworm.ViewModel;
using FuzzySharp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace bookworm.Controllers
{
    public class BookController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly BookStoreContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookRepository _bookRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IAuthorRepository _authorRepository;

        public BookController(BookStoreContext context, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager, IBookRepository bookRepository, IReviewRepository reviewRepository, IAuthorRepository authorRepository)
        {
            _context = context;
            this.webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _bookRepository = bookRepository;
            _reviewRepository = reviewRepository;
            _authorRepository = authorRepository;
        }

        // GET: BookController
        public async Task <IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                // Handle the case where the current user is not found
                return RedirectToAction("Login", "Account");
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

        public async Task<IActionResult> AuthorDashboard()
        {
            // Get the current user
            var currentUser = await _userManager.GetUserAsync(User);

            // Retrieve all books authored by the current user
            var books = _context.Books
                .Where(b => b.AuthorId == currentUser.Id)
                .ToList();

            return View(books);
        }
        public IActionResult AdminPanel()
        {
            
            return View();
        }

        public IActionResult MangeBook()
        {
            var books = _context.Books.ToList();
            return View(books);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                // Retrieve the currently logged-in user
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    // Handle the case where the user is not found
                    // Redirect to an error page or return an appropriate error message
                    return RedirectToAction("Error", "Home");
                }

                // Provide necessary data to the view for dropdowns, etc.
                var categories = await _context.Categories.ToListAsync();

                if (categories == null || !categories.Any())
                {
                    // Handle the case where categories are not found
                    // Redirect to an error page or return an appropriate error message
                    return RedirectToAction("Error", "Home");
                }

                // Set the AuthorId property to the current user's ID
                var model = new CreateBookViewModel { 
                    AuthorId = user.Id,
                    Categories = categories
                };

                

                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception
               

                // Redirect to an error page or return an appropriate error message
                return RedirectToAction("Error", "Home");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookViewModel model)
        {
            // Ensure only users with Author role can access this action method
            if (!User.IsInRole("author"))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                // Save cover image to wwwroot/images folder
                var coverImageFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.CoverImage.FileName);
                var coverImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", coverImageFileName);
                using (var stream = new FileStream(coverImagePath, FileMode.Create))
                {
                    await model.CoverImage.CopyToAsync(stream);
                }

                // Save eBook file to wwwroot/ebooks folder
                var ebookFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.EbookFile.FileName);
                var ebookPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ebooks", ebookFileName);
                using (var stream = new FileStream(ebookPath, FileMode.Create))
                {
                    await model.EbookFile.CopyToAsync(stream);
                }

                // Create new Book object
                var book = new Book
                {
                    Title = model.Title,
                    Description = model.Description,
                    Language = model.Language,
                    DatePublished = model.DatePublished,
                    Price = model.Price,
                    AuthorId = model.AuthorId,
                    CategoryId = model.CategoryId,
                    CoverImagePath = "/images/" + coverImageFileName, // Store relative path to image
                    FilePath = "/ebooks/" + ebookFileName, // Store relative path to eBook file
                    IsDiscounted = model.IsDiscounted,
                    DiscountAmount = model.DiscountAmount
                };

                // Add book to database
                _bookRepository.AddBook(book);
                await _bookRepository.SaveChangesAsync();

                return RedirectToAction("Author", "Book"); // Redirect to home page or another appropriate action
            }

            // If model state is not valid, redisplay the form with validation errors
            model.Categories = _bookRepository.GetAllCategories();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            var model = new EditBookViewModel
            {
                BookId = book.Id,
                Title = book.Title,
                Description = book.Description,
                Language = book.Language,
                DatePublished = book.DatePublished,
                Price = book.Price,
                AuthorId = book.AuthorId,
                CategoryId = book.CategoryId,
                IsDiscounted = book.IsDiscounted,
                DiscountAmount = book.DiscountAmount,
                Categories = _bookRepository.GetAllCategories()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditBookViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Map the view model data to a Book entity
                var bookToUpdate = await _bookRepository.GetBookByIdAsync(model.BookId);
                if (bookToUpdate == null)
                {
                    return NotFound();
                }

                bookToUpdate.Title = model.Title;
                bookToUpdate.Description = model.Description;
                bookToUpdate.Language = model.Language;
                bookToUpdate.DatePublished = model.DatePublished;
                bookToUpdate.Price = model.Price;
                bookToUpdate.AuthorId = model.AuthorId;
                bookToUpdate.CategoryId = model.CategoryId;
                bookToUpdate.IsDiscounted = model.IsDiscounted;
                bookToUpdate.DiscountAmount = model.DiscountAmount;

                // Update the cover image if provided
                if (model.CoverImage != null)
                {
                    // Save cover image to wwwroot/images folder
                    var coverImageFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.CoverImage.FileName);
                    var coverImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", coverImageFileName);
                    using (var stream = new FileStream(coverImagePath, FileMode.Create))
                    {
                        await model.CoverImage.CopyToAsync(stream);
                    }
                    bookToUpdate.CoverImagePath = "/images/" + coverImageFileName;
                }

                // Update the eBook file if provided
                if (model.EbookFile != null)
                {
                    // Save eBook file to wwwroot/ebooks folder
                    var ebookFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.EbookFile.FileName);
                    var ebookPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ebooks", ebookFileName);
                    using (var stream = new FileStream(ebookPath, FileMode.Create))
                    {
                        await model.EbookFile.CopyToAsync(stream);
                    }
                    bookToUpdate.FilePath = "/ebooks/" + ebookFileName;
                }

                // Update the book in the database
                bool updateResult = await _bookRepository.UpdateBookAsync(bookToUpdate);
                if (updateResult)
                {
                    // Update successful
                    return RedirectToAction("AuthorDashboard", "Book"); // Redirect to home page or another appropriate action
                }
                else
                {
                    // Update failed
                    ModelState.AddModelError(string.Empty, "Failed to update book.");
                }
            }

            // If model state is not valid or update failed, redisplay the form with validation errors
            model.Categories = _bookRepository.GetAllCategories();
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Retrieve the book from the database
            var book = await _bookRepository.GetBookByIdAsync(id);

            // Check if the book exists
            if (book == null)
            {
                return NotFound();
            }

            // Map the book to the DeleteBookViewModel
            var viewModel = new DeleteBookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Language = book.Language,
                DatePublished = book.DatePublished,
                Price = book.Price,
                AuthorName = book.Author?.FullName, // Assuming you have a FullName property in the ApplicationUser model
                CategoryName = book.Category?.Name // Assuming you have a Name property in the Category model
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Retrieve the book from the database
            var book = await _bookRepository.GetBookByIdAsync(id);

            // Check if the book exists
            if (book == null)
            {
                return NotFound();
            }

            try
            {
                // Delete the book from the database
                _bookRepository.DeleteBook(book);
                await _bookRepository.SaveChangesAsync();

                // Redirect to the index page or another appropriate action
                return RedirectToAction("AuthorDashboard", "Book");
            }
            catch (Exception ex)
            {
                // Log or handle any errors
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                // Handle the case where the current user is not found
                return RedirectToAction("Login", "Account");
            }
            
            var book = _bookRepository.GetBookByIdWithReviews(id);
            var reviews = _reviewRepository.GetReviewsForBook(id);
            var isInWishlist = false;
            isInWishlist = _context.WishLists.Any(w => w.UserId == currentUser.Id && w.BookId == book.Id) ;
            if (book == null)
            {
                return NotFound();
            }
            ViewBag.BookId = book.Id;
            var viewModel = new BookDetailsViewModel
            {
                Book = book,
                Reviews = reviews,
                BookId=book.Id,
                IsInWishList=isInWishlist
            };
            // Check if the user has purchased the book
            var hasPurchased = await _context.PurchaseItems
                .AnyAsync(pi => pi.Purchase.UserId == currentUser.Id && pi.BookId == book.Id);

            // Pass the book and purchase information to the view
            ViewBag.HasPurchased = hasPurchased;
            return View(viewModel);
        }

        public async Task<IActionResult> DetailsforAuthor(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var book = _bookRepository.GetBookByIdWithReviews(id);
            var reviews = _reviewRepository.GetReviewsForBook(id);
            var isInWishlist = false;
            isInWishlist = _context.WishLists.Any(w => w.UserId == currentUser.Id && w.BookId == book.Id);
            if (book == null)
            {
                return NotFound();
            }
            ViewBag.BookId = book.Id;
            var viewModel = new BookDetailsViewModel
            {
                Book = book,
                Reviews = reviews,
                BookId = book.Id,
                IsInWishList = isInWishlist
            };
            // Check if the user has purchased the book
            var hasPurchased = await _context.PurchaseItems
                .AnyAsync(pi => pi.Purchase.UserId == currentUser.Id && pi.BookId == book.Id);

            // Pass the book and purchase information to the view
            ViewBag.HasPurchased = hasPurchased;
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CreateForAdmin()
        {
            try
            {
                // Retrieve the currently logged-in user
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    // Handle the case where the user is not found
                    // Redirect to an error page or return an appropriate error message
                    return RedirectToAction("Error", "Home");
                }

                // Provide necessary data to the view for dropdowns, etc.
                var categories = await _context.Categories.ToListAsync();
                // Retrieve users with the "Author" role from the user manager
                var authors = await _userManager.GetUsersInRoleAsync("author");
                if (categories == null || !categories.Any())
                {
                    // Handle the case where categories are not found
                    // Redirect to an error page or return an appropriate error message
                    return RedirectToAction("Error", "Home");
                }

                // Set the AuthorId property to the current user's ID
                var model = new CreateBookVMForAdmin
                {
                    Authors = authors,
                    Categories = categories
                };



                return View(model);
            }
            catch (Exception ex)
            {
                // Log the exception


                // Redirect to an error page or return an appropriate error message
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateForAdmin(CreateBookVMForAdmin model)
        {
            // Ensure only users with Author role can access this action method
            if (!User.IsInRole("admin"))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                // Save cover image to wwwroot/images folder
                var coverImageFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.CoverImage.FileName);
                var coverImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", coverImageFileName);
                using (var stream = new FileStream(coverImagePath, FileMode.Create))
                {
                    await model.CoverImage.CopyToAsync(stream);
                }

                // Save eBook file to wwwroot/ebooks folder
                var ebookFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.EbookFile.FileName);
                var ebookPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ebooks", ebookFileName);
                using (var stream = new FileStream(ebookPath, FileMode.Create))
                {
                    await model.EbookFile.CopyToAsync(stream);
                }

                // Create new Book object
                var book = new Book
                {
                    Title = model.Title,
                    Description = model.Description,
                    Language = model.Language,
                    DatePublished = model.DatePublished,
                    Price = model.Price,
                    AuthorId = model.AuthorId,
                    CategoryId = model.CategoryId,
                    CoverImagePath = "/images/" + coverImageFileName, // Store relative path to image
                    FilePath = "/ebooks/" + ebookFileName, // Store relative path to eBook file
                    IsDiscounted = model.IsDiscounted,
                    DiscountAmount = model.DiscountAmount
                };

                // Add book to database
                _bookRepository.AddBook(book);
                await _bookRepository.SaveChangesAsync();

                return RedirectToAction("AdminPanel", "Book"); // Redirect to home page or another appropriate action
            }

            // If model state is not valid, redisplay the form with validation errors
            model.Categories = _bookRepository.GetAllCategories();
            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult AddBookWithAuthor()
        {
            var authors = _userManager.GetUsersInRoleAsync("author").Result;
            var authorList = authors.Select(a => new SelectListItem { Value = a.Id, Text = a.FullName }).ToList();
            var model = new AddBookWithAuthorViewModel { Authors = authorList };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddBook(CreateBookViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Save cover image to wwwroot/images folder
                var coverImageFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.CoverImage.FileName);
                var coverImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", coverImageFileName);
                using (var stream = new FileStream(coverImagePath, FileMode.Create))
                {
                    await model.CoverImage.CopyToAsync(stream);
                }

                // Save eBook file to wwwroot/ebooks folder
                var ebookFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.EbookFile.FileName);
                var ebookPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ebooks", ebookFileName);
                using (var stream = new FileStream(ebookPath, FileMode.Create))
                {
                    await model.EbookFile.CopyToAsync(stream);
                }

                // Get the selected author's ID
                var selectedAuthorId = model.AuthorId;

                // Create new Book object
                var book = new Book
                {
                    Title = model.Title,
                    Description = model.Description,
                    Language = model.Language,
                    DatePublished = model.DatePublished,
                    Price = model.Price,
                    AuthorId = selectedAuthorId,
                    CategoryId = model.CategoryId,
                    CoverImagePath = "/images/" + coverImageFileName, // Store relative path to image
                    FilePath = "/ebooks/" + ebookFileName, // Store relative path to eBook file
                    IsDiscounted = model.IsDiscounted,
                    DiscountAmount = model.DiscountAmount
                };

                // Add book to database
                _bookRepository.AddBook(book);
                await _bookRepository.SaveChangesAsync();

                return RedirectToAction("Index", "Home"); // Redirect to home page or another appropriate action
            }

            // If model state is not valid, redisplay the form with validation errors
            ViewData["Categories"] = _bookRepository.GetAllCategories();
            ViewData["Authors"] = _userManager.Users.Where(u => _userManager.IsInRoleAsync(u, "author").Result)
                                                      .Select(u => new SelectListItem { Value = u.Id, Text = u.FullName })
                                                      .ToList();
            return View(model);
        }

        //public async Task<IActionResult> AddToWishlist(int bookId)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    var book = await _context.Books.FindAsync(bookId);

        //    if (user != null && book != null)
        //    {
        //        var wishlistItem = new WishList
        //        {
        //            UserId = user.Id,
        //            BookId = bookId
        //        };

        //        _context.WishLists.Add(wishlistItem);
        //        await _context.SaveChangesAsync();
        //    }

        //    return RedirectToAction("Wishlist");
        //}

        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            var book = await _context.Books.FindAsync(bookId);

            if (user != null && book != null)
            {
                // Check if the book is already in the user's wishlist
                var isInWishlist = await _context.WishLists
                    .AnyAsync(w => w.UserId == user.Id && w.BookId == bookId);

                if (!isInWishlist)
                {
                    // Book is not in the wishlist, so add it
                    var wishlistItem = new WishList
                    {
                        UserId = user.Id,
                        BookId = bookId
                    };

                    _context.WishLists.Add(wishlistItem);
                    await _context.SaveChangesAsync();
                }

                // Return a success message
                return RedirectToAction("MyWishList", "Book");
            }

            // Return an error message
            return RedirectToAction("Details", "Book", new { id = bookId });
        }



        //public async Task<IActionResult> RemoveFromWishlist(int wishlistId)
        //{
        //    var wishlistItem = await _context.WishLists.FindAsync(wishlistId);

        //    if (wishlistItem != null)
        //    {
        //        _context.WishLists.Remove(wishlistItem);
        //        await _context.SaveChangesAsync();
        //    }

        //    return RedirectToAction("Wishlist");
        //}

        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            var wishlistItem = await _context.WishLists.FirstOrDefaultAsync(w => w.UserId == user.Id && w.BookId == bookId);

            if (wishlistItem != null)
            {
                _context.WishLists.Remove(wishlistItem);
                await _context.SaveChangesAsync();

                // Return a success message
                return RedirectToAction("MyWishList", "Book");
            }

            // Return an error message
            return RedirectToAction("Details", "Book", new { id = bookId });

        }

        [HttpGet]
        public async Task<IActionResult> MyWishList()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                // Handle the case where the current user is not found
                return RedirectToAction("Login", "Account");
            }

            var wishlistBooks = await _context.WishLists
                .Include(w => w.Book.Author) // Include the Author entity
                .Where(w => w.UserId == currentUser.Id)
                .Select(w => w.Book)
                .ToListAsync();

            return View(wishlistBooks);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            var categories = _bookRepository.GetAllCategories();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // If the search term is empty or null, return a view with no results
                return View("SearchResults", new SearchViewModel { Categories=categories});
            }

            // Fetch all books and authors from the database
            var books = await _context.Books.Include(b => b.Author).ToListAsync();
            

            // Perform the fuzzy search in-memory using LINQ to Objects
            var searchResults = books.Where(b =>
                Fuzz.PartialRatio(b.Title, searchTerm) > 70 ||
                Fuzz.PartialRatio(b.Author.FullName, searchTerm) > 70
            ).ToList();
            var model = new SearchViewModel
            {
                SearchTerm=searchTerm,
                books = searchResults,
                Categories = categories
            };
            return View("SearchResults", model);
        }


        //[HttpGet]
        //public async Task<IActionResult> SearchFilter(string searchTerm, List<int>? genreIds, int? minRating, decimal? minPrice, decimal? maxPrice)
        //{
        //    // Get all books from the database
        //    var query = _context.Books
        //        .Include(b => b.Author)
        //        .AsQueryable();

        //    // Filter by search term
        //    if (!string.IsNullOrWhiteSpace(searchTerm))
        //    {
        //        query = query.Where(b => Fuzz.PartialRatio(b.Title, searchTerm) > 70 || Fuzz.PartialRatio(b.Author.FullName, searchTerm) > 70);
        //    }

        //    // Filter by genres
        //    if (genreIds != null && genreIds.Any())
        //    {
        //        query = query.Where(b => genreIds.Contains(b.CategoryId));
        //    }

        //    // Filter by minimum rating
        //    if (minRating.HasValue)
        //    {
        //        query = query.Where(b => b.Reviews.Average(r => r.Rating) >= minRating.Value);
        //    }

        //    // Filter by minimum price
        //    if (minPrice.HasValue)
        //    {
        //        query = query.Where(b => b.Price >= minPrice.Value);
        //    }

        //    // Filter by maximum price
        //    if (maxPrice.HasValue)
        //    {
        //        query = query.Where(b => b.Price <= maxPrice.Value);
        //    }

        //    // Execute the query
        //    var searchResults = await query.ToListAsync();

        //    return View("SearchResults", searchResults);
        //}
        [HttpGet]
        public async Task<IActionResult> SearchFilter(SearchViewModel model)
        {
            // Get all books from the database
            var query = _context.Books
                .Include(b => b.Author)
                .AsQueryable();

            // Filter by search term
            if (!string.IsNullOrWhiteSpace(model.SearchTerm))
            {
                var searchTerm = model.SearchTerm.ToLower(); // Convert search term to lowercase for case-insensitive comparison
                query = query.Where(b => b.Title.ToLower().Contains(searchTerm) || b.Author.FullName.ToLower().Contains(searchTerm));

            }

            // Filter by genres
            if (model.GenreIds != null && model.GenreIds.Any())
            {
                query = query.Where(b => model.GenreIds.Contains(b.CategoryId));
            }




            // Filter by minimum price
            if (model.MinPrice.HasValue)
            {
                query = query.Where(b => b.Price >= model.MinPrice.Value);
            }

            // Filter by maximum price
            if (model.MaxPrice.HasValue)
            {
                query = query.Where(b => b.Price <= model.MaxPrice.Value);
            }

            // Execute the query
            var searchResults = await query.ToListAsync();
            model.SearchTerm= model.SearchTerm ?? string.Empty;
            var categories = _bookRepository.GetAllCategories(); // Fetch all categories
            model.Categories = categories;
            model.books = searchResults;

            return View("SearchResults", model);
        }

        public async Task<IActionResult> PutBookOnDiscount(int bookId, decimal discountPercentage)
        {
            // Retrieve the book from the database
            var book = await _context.Books.FindAsync(bookId);

            if (book == null)
            {
                return NotFound(); // Book not found
            }

            // Calculate the new price with the discount
            decimal discountAmount = (discountPercentage / 100) * book.Price;
            decimal discountedPrice = book.Price - discountAmount;

            // Update the book properties
            book.Price = discountedPrice;
            book.IsDiscounted = true;
            book.DiscountAmount = discountAmount;

            // Save changes to the database
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = bookId }); // Redirect to book details page
        }

        public async Task<IActionResult> PurchasedBooks()
        {
            // Get the current user
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login page if user is not logged in
            }

            // Query the database for purchase items associated with the current user
            var purchaseItems = _context.PurchaseItems
                                        .Include(pi => pi.Book)
                                        .Where(pi => pi.Purchase.UserId == currentUser.Id)
                                        .ToList();

            // Extract the list of purchased books from the purchase items
            var purchasedBooks = purchaseItems.Select(pi => pi.Book).ToList();

            return View(purchasedBooks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(ReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account"); // Redirect to login page if user is not logged in
                }

                // Check if the user has purchased the book
                //var hasPurchased = _context.PurchaseItems
                //                            .Any(pi => pi.Purchase.UserId == currentUser.Id && pi.BookId == model.BookId);

                //if (!hasPurchased)
                //{
                //    ModelState.AddModelError("", "You can only review books you have purchased.");
                //    return View(model);
                //}

                // Create a new review object
                var review = new Review
                {
                    UserId = currentUser.Id,
                    BookId = model.BookId,
                    Content = model.Content,
                    Rating = model.Rating,
                    DatePosted = DateTime.Now
                };

                // Add the review to the database
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Book", new { id = model.BookId }); // Redirect to the book details page
            }

            return RedirectToAction("Details", "Book", new { id = model.BookId });
        }

        public IActionResult UserBooks(string userId)
        {
            // Retrieve the user's books from the database
            var userBooks = _context.Books
                .Include(b => b.Author)
                .Where(b => b.AuthorId == userId)
                .ToList();



            // Pass the list of user's books to the view
            return View(userBooks);
        }

        public async Task<IActionResult> Read(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }


        public async Task<IActionResult> DownloadBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            // Read the ePub file from the file system
            var filePath = "wwwroot"+ Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", book.FilePath);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // Read the file content into a byte array
            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            // Return the file as a downloadable file
            return File(fileBytes, "application/epub+zip", book.Title + ".epub");
        }
        [HttpGet]
        public IActionResult ApplySpecialOffer(int id)
        {
            var book = _bookRepository.GetBookByIdWithReviews(id);
            
           
           


            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplySpecialOffer(int id, decimal discountAmount)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            book.IsDiscounted = true;
            book.DiscountAmount = discountAmount;

            // Calculate the discounted price
            if (book.DiscountAmount.HasValue)
            {
                book.Price -= (book.Price * book.DiscountAmount.Value) / 100; // Apply discount
            }

            _context.Update(book);
            await _context.SaveChangesAsync();

            return RedirectToAction("DetailsforAuthor", "Book", new { id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveSpecialOffer(int id, decimal discountAmount)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }
            if (book.IsDiscounted!=null && book.IsDiscounted==true)
            {
                if (book.DiscountAmount.HasValue)
                {
                    book.Price += (book.Price * book.DiscountAmount.Value) / 100; // Apply discount
                }
            }
            book.IsDiscounted = false;
            book.DiscountAmount = 0;

           

            _context.Update(book);
            await _context.SaveChangesAsync();

            return RedirectToAction("DetailsforAuthor", "Book", new { id });
        }


    }
}
