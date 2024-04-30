using OnlineLibrary.DataLayer.Entiteties;

namespace OnlineLibrary.DataLayer.Repositories
{
    public interface IAuthorRepository
    {
		public List<Author> GetAllAuthors();
		public Author FindAuthor(int id);
		public void AddAuthor(Author author);
		public void RemoveAuthor(Author author);
		public void UpdateAuthor(Author author);
		public bool AuthorExists(Author author);
	}
}
