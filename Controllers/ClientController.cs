using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.DataLayer.DBContext;
using OnlineLibrary.DataLayer.Entiteties;

namespace OnlineLibrary.Controllers
{
    public class ClientController : Controller
    {
        private readonly OnlineLibraryDbContext _context;
        public ClientController(OnlineLibraryDbContext context)
        {
            _context = context;
        }
		[Authorize(Roles = "Admin")]
		public IActionResult Index([FromQuery] string filterTerm)
        {
            var clients = _context.Clients
           .Include(p => p.ClientBooks)
                .ThenInclude(p => p.Book)
          .Where(p => (p.IsDeleted == false || p.IsDeleted == null))
          .OrderBy(p => p.FullName)
          .ToList();
            if (!string.IsNullOrEmpty(filterTerm))
            {
                clients = clients.Where(p => p.FullName.Contains(filterTerm) ||
                                             p.Email.Contains(filterTerm))
                    .ToList();
            }
            return View(clients);
        }

		[Authorize(Roles = "Admin")]
		public IActionResult Details(int id)
        {
            var books = _context.Clients.Where(p => p.ID == id)
                                      .FirstOrDefault();
            return View(books);
        }

		[Authorize(Roles = "Admin")]
		public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create([Bind("FullName,Email")] Client client)
        {
            var clientExists = _context.Clients.Where(p => p.ID == client.ID
                                                            && p.IsDeleted != true &&
                                                            p.IsActive != false)
                               .FirstOrDefault();

            if (ModelState.IsValid)
            {
                _context.Clients.Add(client);
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
            var client = _context.Clients
                .Where(p => p.ID == id)
                .FirstOrDefault();
            return View(client);
        }
        [HttpPost]
        public IActionResult Update([Bind("FullName, Email, ID")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Clients.Update(client);
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
            var client = _context.Clients
                .Where(p => p.ID == Id)
                .FirstOrDefault();
            //Soft Delete
            client.IsDeleted = true;
            //Hard Delete
            //_onlineLibraryDbContext.Clients.Remove(author);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

		[Authorize(Roles = "Admin")]
		[HttpPost, ActionName("Deactivate")]
        public IActionResult Deactivate(int Id)
        {
            var client = _context.Clients
                .Where(p => p.ID == Id)
                .FirstOrDefault();

            client.IsActive = false;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

		[Authorize(Roles = "Admin")]
		[HttpPost, ActionName("Activate")]
        public IActionResult Activate(int Id)
        {
            var client = _context.Clients
                .Where(p => p.ID == Id)
                .FirstOrDefault();

            client.IsActive = true;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }

}


