using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.DataLayer.DBContext;

namespace OnlineLibrary.Controllers
{
    public class BooksController : Controller
    {
        private readonly OnlineLibraryDbContext _context;
        public BooksController(OnlineLibraryDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
        
            var books = _context.Books.Include(p=> p.Author)
                                      .OrderBy(p => p.Title);
            return View(books);
        }
        public IActionResult Details(int id)
        {
            var books = _context.Books.Include(p=> p.Author)
                                      .Where(p => p.Id == id)
                                      .FirstOrDefault();
            return View(books);
        }
    }
}