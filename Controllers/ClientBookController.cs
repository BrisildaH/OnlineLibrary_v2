using Microsoft.AspNetCore.Mvc;
using OnlineLibrary.DataLayer.DBContext;
using OnlineLibrary.DataLayer.Entiteties;

namespace OnlineLibrary.Controllers
{
    public class ClientBookController : Controller
    {
        private readonly OnlineLibraryDbContext _context;
        public ClientBookController(OnlineLibraryDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult Create([Bind("AuthorID,BookID")] Book book)
        //{
          
        //    var clientbook = _context.Authors.Where(p => p.Id == book.AuthorID
        //                                                    && p.IsDeleted != true &&
        //                                                    p.IsActive != false)
        //                       .FirstOrDefault();
        //    if (authorExists == null)
        //    {
        //        throw new Exception("Record does not exists");
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        book.CreateDate = DateTime.Now;
        //        _context.Books.Add(book);
        //        _context.SaveChanges();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    else
        //    {
        //        return StatusCode(500, "Information is invalid");
        //    }

        }
    }

