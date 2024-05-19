﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.DataLayer.DBContext;
using OnlineLibrary.DataLayer.Entiteties;

namespace OnlineLibrary.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly OnlineLibraryDbContext _context;
        public AuthorsController(OnlineLibraryDbContext context)
        {
            _context = context;
        }
		public IActionResult Index([FromQuery] string filterTerm)
		{
			var authors = _context.Authors
				 .Include(p => p.Books)
				 .Where(p => (p.IsDeleted == false || p.IsDeleted == null))
				 .OrderBy(p => p.FullName)
				 .ToList();

			if (!string.IsNullOrEmpty(filterTerm))
			{
				authors = authors.Where(p => p.FullName.Contains(filterTerm) ||
											 p.Description.Contains(filterTerm))
					.ToList();
			}
			return View(authors);
		}
		public IActionResult Details(int id)
        {
            var authors = _context.Authors/*.Include(p => p.Author)*/
                                      .Where(p => p.Id == id)
                                      .FirstOrDefault();
            return View(authors);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create([Bind("FullName,Description")] Author author)
        {
            author.PhotoPath = "path ";

            if (ModelState.IsValid)
            {
                author.CreateDate = DateTime.Now;
                _context.Authors.Add(author);
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
            var author = _context.Authors
                .Where(p => p.Id == id)
                .FirstOrDefault();
            return View(author);
        }
        [HttpPost]
        public IActionResult Update([Bind("FullName, Description, Id")] Author author)
        {
            if (ModelState.IsValid)
            {
                author.UpdateDate = DateTime.Now;
                _context.Authors.Update(author);
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
            var author = _context.Authors
                .Where(p => p.Id == Id)
                .FirstOrDefault();
            //Soft Delete
            author.IsDeleted = true;
            //Hard Delete
            //_onlineLibraryDbContext.Authors.Remove(author);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

		[Authorize(Roles = "Admin")]

		[HttpPost, ActionName("Deactivate")]
        public IActionResult Deactivate(int Id)
        {
            var author = _context.Authors
                .Where(p => p.Id == Id)
                .FirstOrDefault();

            author.IsActive = false;
            author.UpdateDate = DateTime.Now;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
		[Authorize(Roles = "Admin")]

		[HttpPost, ActionName("Activate")]
        public IActionResult Activate(int Id)
        {
            var author = _context.Authors
                .Where(p => p.Id == Id)
                .FirstOrDefault();

            author.IsActive = true;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
//b
