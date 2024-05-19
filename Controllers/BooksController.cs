using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.IdentityModel.Tokens;
using OnlineLibrary.DataLayer.DBContext;
using OnlineLibrary.DataLayer.Entiteties;
using System.Globalization;

namespace OnlineLibrary.Controllers
{

    public class BooksController : Controller
    {
        private readonly OnlineLibraryDbContext _context;
        public BooksController(OnlineLibraryDbContext context)
        {
            _context = context;
        }
        public IActionResult Index([FromQuery] string filterTerm)
        {
            var books = _context.Books
                 .Include(p => p.Author)
                 .Where(p => (p.IsDeleted == false || p.IsDeleted == null))
                 .OrderBy(p => p.Title)
                 .ToList();

            if (!string.IsNullOrEmpty(filterTerm))
            {
                books = books.Where(p => p.Title.Contains(filterTerm) ||
                                             p.Description.Contains(filterTerm))
                    .ToList();
            }
            return View(books);
        }
            public IActionResult Details(int id)
        {
            var books = _context.Books.Include(p => p.Author)
                                      .Where(p => p.Id == id)
                                      .FirstOrDefault();
            return View(books);
        }
		[Authorize(Roles = "Admin")]
		public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create([Bind("Title,Description, Category, ISBN, AuthorID")] Book book)
        {
            book.PhotoPath = "path ";
            var authorExists = _context.Authors.Where(p => p.Id == book.AuthorID
                                                            && p.IsDeleted != true &&
                                                            p.IsActive != false)
                               .FirstOrDefault();
            if (authorExists == null)
            {
                throw new Exception("Record does not exists");
            }
            if (ModelState.IsValid)
            {
                book.CreateDate = DateTime.Now;
                _context.Books.Add(book);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            else
            {
                return StatusCode(500, "Information is invalid");
            }

        }
		[Authorize(Roles = "Admin")]
		public IActionResult Update([FromRoute] int id)
        {
            var book = _context.Books
                .Where(p => p.Id == id)
                .FirstOrDefault();
            return View(book);
        }
        [HttpPost]
        public IActionResult Update([Bind("Title, Description, Category,Id, AuthorID, ISBN")] Book book)
        {
            if (ModelState.IsValid)
            {
                book.UpdateDate = DateTime.Now;
                _context.Books.Update(book);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return StatusCode(500, "Information is invalid");
            }

        }
		[Authorize(Roles = "Admin")]
		//Delete action pa view

		[HttpPost, ActionName("Delete")]
        public IActionResult Delete(int Id)
        {
            var book = _context.Books
                .Where(p => p.Id == Id)
                .FirstOrDefault();
            //Soft Delete
            book.IsDeleted = true;
            //Hard Delete
            //_onlineLibraryDbContext.Authors.Remove(author);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}
