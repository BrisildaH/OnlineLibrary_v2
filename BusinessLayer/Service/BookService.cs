using OnlineLibrary.DataLayer.Entiteties;
using OnlineLibrary.DataLayer.Repositories;

namespace OnlineLibrary.BusinessLayer.Service
{
	public class BookService : IBookService
	{
		private readonly IBookRepository _bookRepository;
		public BookService(IBookRepository bookRepository)
		{
			_bookRepository = bookRepository;
		}
		public List<Book> GetAllBooks()
		{
			try
			{
				var books = _bookRepository.GetAllBooks();
				return books;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
				throw;
			}
		}
		public Book FindByID(int id)
		{
			try
			{
				var books = _bookRepository.FindByID(id);
				return books;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw ex;
			}
		}
		public void DeleteBook(int id)
		{
			try
			{
				var booktoDelete = _bookRepository.FindByID(id);
				if (booktoDelete == null)
				{
					Console.WriteLine("Book does not exist");
				}

				_bookRepository.DeleteBook(booktoDelete);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw ex;
			}
		}
		public void AddBook(Book book)
		{
			try
			{
				if (_bookRepository.BookExists(book))
				{
					Console.WriteLine("This book already exists. It cannot be added.");
					return;
				}

				if (!IsBookValid(book))
				{
					Console.WriteLine("The book data is not entered correctly");
					return;
				}

				_bookRepository.AddBook(book);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw;
			}
		}

		private bool IsBookValid(Book book)
		{
			if (string.IsNullOrWhiteSpace(book.Title))
			{
				Console.WriteLine("The book title is mandatory.");
				return false;
			}

			if (book.Author == null)
			{
				Console.WriteLine("The book author is mandatory.");
				return false;
			}

			if (string.IsNullOrWhiteSpace(book.ISBN) || (book.ISBN.Length != 10 && book.ISBN.Length != 13))
			{
				Console.WriteLine("The book ISBN is not valid.");
				return false;
			}
			return true;
		}
		public void UpdateBook(Book book)
		{
			try
			{
				if (!IsBookValid(book))
				{
					Console.WriteLine("Invalid book data. Update failed.");
					return;
				}

				if (!_bookRepository.BookExists(book))
				{
					Console.WriteLine("The book does not exist. Update failed.");
					return;
				}

				_bookRepository.UpdateBook(book);
			}
			catch (Exception ex)
			{
				Console.WriteLine("An error occurred while updating the book: " + ex.Message);
				throw ex;
			}
		}
		public List<Book> FilterBook(string? title, string? description, string? category)
		{
			var books = _bookRepository.GetAllBooks();

			if (!string.IsNullOrEmpty(title))
			{
				books = books.Where(p => p.Title.Contains(title))
				   .ToList();
			}

			if (!string.IsNullOrEmpty(description))
			{
				books = books.Where(p => p.Description.Contains(description))
				   .ToList();
			}

			if (!string.IsNullOrEmpty(category))
			{
				books = books.Where(p => p.Category.Contains(category))
				   .ToList();
			}

			return books;
		}
	}
}
