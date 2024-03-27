using bookworm.Data;
using bookworm.Models;
using bookworm.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace bookworm.Controllers
{
    public class CategoryController : Controller
    {
        private readonly BookStoreContext _context;
        public CategoryController(BookStoreContext context) 
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Categories);
        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            var model = new CreateCategoryViewModel { categories = _context.Categories };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(CreateCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create a new Category object
                var category = new Category
                {
                    Name = model.name,
                    
                };

                // Add the category to the database
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                model.name = "";
                model.categories = _context.Categories;
                // Optionally, you can redirect the user to another page
                return View(model);
            }

            // If model state is not valid, return the view with errors
            return View(model);
        }

        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            var category= _context.Categories.FirstOrDefault(c => c.Id == id);
            var model = new CategoryViewModel
            {
                Id = category.Id, Name = category.Name,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the category from the database
                    var category = await _context.Categories.FindAsync(model.Id);
                    if (category == null)
                    {
                        return NotFound(); // Or handle the error appropriately
                    }

                    // Update the category properties
                    category.Name = model.Name;
                    

                    // Save the changes to the database
                    await _context.SaveChangesAsync();

                    // Redirect to the category list page or another appropriate action
                    return RedirectToAction("Index", "Category");
                }
                catch (Exception ex)
                {
                    // Log the error or handle it appropriately
                    ModelState.AddModelError("", "An error occurred while updating the category.");
                    return View(model); // Return to the edit view with an error message
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            var model = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(CategoryViewModel model)
        {
            try
            {
                // Retrieve the category from the database
                var category = await _context.Categories.FindAsync(model.Id);
                if (category == null)
                {
                    return NotFound(); // Or handle the error appropriately
                }

                // Remove the category from the database context
                _context.Categories.Remove(category);

                // Save the changes to the database
                await _context.SaveChangesAsync();

                // Redirect to the category list page or another appropriate action
                return RedirectToAction("Index", "Category");
            }
            catch (Exception ex)
            {
                // Log the error or handle it appropriately
                return StatusCode(500); // Return a server error response
            }
        }


    }
}
