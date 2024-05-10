using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.DataLayer.DBContext;
using OnlineLibrary.DataLayer.Entiteties;
using OnlineLibrary.Models;

namespace OnlineLibrary.Controllers
{
    public class RentBookController: Controller
    {
        private readonly OnlineLibraryDbContext _context;
        public RentBookController(OnlineLibraryDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            var rentbooks = _context.ClientBooks
                .Include(p => p.Client)
                .Include(p=> p.Book).ThenInclude(p=> p.Author)
                 .Where(p => (p.IsDelete != false))
                                      .OrderBy(p => p.Book.Title);
            return View(rentbooks);
        
    }
        public IActionResult Create()
        {
            var rentbookModel = new RentBookModel();
            rentbookModel.Clients = _context.Clients.Where(p => p.IsActive != false &&
            p.IsDeleted != true)
                                                    .ToList();
            rentbookModel.Books = _context.Books.Where(p => p.IsDeleted != true)
                                                .ToList();
            return View(rentbookModel);
        }

        [HttpPost, ActionName("Create")]
        public IActionResult Create([Bind("ClientID, BookID")] ClientBook clientBook)
        {
            if (ModelState.IsValid)
            {
                _context.ClientBooks.Add(clientBook);
                _context.SaveChanges();
                return RedirectToAction("Index","Books");
            }
            else
            {
                return StatusCode(500, "Information is invalid");
            }
        }
        
    }
}
