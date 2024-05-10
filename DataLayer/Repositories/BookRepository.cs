using OnlineLibrary.DataLayer.DBContext;
using OnlineLibrary.DataLayer.Entiteties;

namespace OnlineLibrary.DataLayer.Repositories
{
	public class BookRepository : IBookRepository
	{

		private readonly OnlineLibraryDbContext onlineLibraryDbContext;
		public BookRepository()

		{
			onlineLibraryDbContext = new OnlineLibraryDbContext();

		}
		public List<Book> GetAllBooks()
		{
			var books = onlineLibraryDbContext.Books.ToList();
			return books;
		}
		// Get By ID
		public Book FindByID(int id)
		{
			try
			{
				var book = onlineLibraryDbContext.Books.Where(p => p.Id == id)
					.FirstOrDefault();
				return book;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw ex;
			}
		}
		//Add book
		public void AddBook(Book book)
		{
			try
			{
				onlineLibraryDbContext.Books.Add(book);
				onlineLibraryDbContext.SaveChanges();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw ex;
			}
		}
		//Update
		//Remove
		public void DeleteBook(Book book)
		{
			try
			{
				onlineLibraryDbContext.Books.Remove(book);
				onlineLibraryDbContext.SaveChanges();
				Console.WriteLine("Book removed");
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.Message.ToString());
				throw ex;
			}
		}
		public void UpdateBook(Book book)
		{
			try
			{
				var bookExists = onlineLibraryDbContext.Books.Where(p => p.Id == book.Id)
				.FirstOrDefault();

				if (bookExists == null)
				{
					throw new Exception("Record does not exists");
				}

				bookExists.Title = book.Title;
				bookExists.Description = book.Description;
				bookExists.Category = book.Category;

				onlineLibraryDbContext.Books.Update(bookExists);
				onlineLibraryDbContext.SaveChanges();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw ex;
			}
		}
		public bool BookExists(Book book)
		{
			return onlineLibraryDbContext.Books.Any(b => b.ISBN == book.ISBN);
		}
	}
}
	