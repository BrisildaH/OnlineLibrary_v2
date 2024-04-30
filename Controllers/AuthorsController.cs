using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.DataLayer.DBContext;

namespace OnlineLibrary.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly OnlineLibraryDbContext _context;
        public AuthorsController(OnlineLibraryDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            var authors = _context.Authors/*.Include(p => p.)*/
                                      .OrderBy(p => p.FullName);
            return View(authors);
        }
        public IActionResult Details(int id)
        {
            var authors = _context.Authors/*.Include(p => p.Author)*/
                                      .Where(p => p.Id == id)
                                      .FirstOrDefault();
            return View(authors);
        }
    }
}