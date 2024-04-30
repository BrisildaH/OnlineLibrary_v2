using OnlineLibrary.DataLayer.Entiteties;

namespace OnlineLibrary.DataLayer.Repositories
{
	public interface IBookRepository
	{
		public void AddBook(Book book);
		public void DeleteBook(Book book);
		public List<Book> GetAllBooks();
		public Book FindByID(int id);
		public void UpdateBook(Book book);
		public bool BookExists(Book book);
	}
}
