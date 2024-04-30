using OnlineLibrary.DataLayer.Entiteties;

namespace OnlineLibrary.BusinessLayer.Service
{
    public interface IAuthorService
    {
		public List<Author> GetAllAuthors();
		public Author FindAuthor(int id);
		public void AddAuthor(Author author);
		public void RemoveAuthor(int id);
		public void UpdateAuthor(Author author);
		public List<Author> FilterBook(string? fullname, string? description);

	}
}
