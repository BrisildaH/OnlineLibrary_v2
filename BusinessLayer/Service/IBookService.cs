using OnlineLibrary.DataLayer.Entiteties;

namespace OnlineLibrary.BusinessLayer.Service
{
    public interface IBookService
    {
		public List<Book> GetAllBooks();
		public Book FindByID(int id);
		public void DeleteBook(int id);
		public void AddBook(Book book);
		public void UpdateBook(Book book);
		public List<Book> FilterBook(string? title, string? description, string? category);

	}
}
