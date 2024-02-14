using BookStore.Models;
using bookworm.Data;
using bookworm.Interfaces;
using bookworm.Models;
using bookworm.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bookworm.Controllers
{
    public class BookController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly BookStoreContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookRepository _bookRepository;

        public BookController(BookStoreContext context, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager, IBookRepository bookRepository)
        {
            _context = context;
            this.webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _bookRepository = bookRepository;
        }

        // GET: BookController
        public ActionResult Index()
        {
            return View();
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
            //if (!User.IsInRole("Author"))
            //{
            //    return Forbid();
            //}

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

                return RedirectToAction("Index", "Home"); // Redirect to home page or another appropriate action
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
                    return RedirectToAction("Index", "Home"); // Redirect to home page or another appropriate action
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
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Log or handle any errors
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult Details(int id)
        {
            var book = _bookRepository.GetBookByIdWithReviews(id);

            if (book == null)
            {
                return NotFound();
            }

            var viewModel = new BookDetailsViewModel
            {
                Book = book,
                Reviews = book.Reviews.ToList()
            };

            return View(viewModel);
        }




    }
}
