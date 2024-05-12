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

        
        }
    }

