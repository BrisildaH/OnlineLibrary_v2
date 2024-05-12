using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            var clients = _context.Clients
                                  .Include(p => p.ClientBooks)
                                   .ThenInclude(p=>p.Book)
                                  .Where(p => (p.IsDeleted == false || p.IsDeleted == null))
                                  .OrderBy(p => p.FullName)
                                  .ToList();
            return View(clients);

        }
        public IActionResult Details(int id)
        {
            var books = _context.Clients.Where(p => p.ID == id)
                                      .FirstOrDefault();
            return View(books);
        }


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

        public IActionResult FilterClient(string? fullname, string? email)
        {
            var filteredClients = _context.Clients.ToList();

            if (!string.IsNullOrEmpty(fullname))
            {
                var lowercaseFullname = fullname.ToLower();
                filteredClients = filteredClients.Where(p => p.FullName.ToLower().Contains(lowercaseFullname)).ToList();
            }

            if (!string.IsNullOrEmpty(email))
            {
                var lowercaseEmail = email.ToLower();
                filteredClients = filteredClients.Where(p => p.Email.ToLower().Contains(lowercaseEmail)).ToList();
            }

            return Ok(filteredClients);
        }
    }
}




