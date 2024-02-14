using bookworm.Data;
using bookworm.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace bookworm.Controllers
{
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly BookStoreContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public IActionResult Index()
        {
            return View();
        }
    }
}
