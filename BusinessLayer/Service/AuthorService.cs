using OnlineLibrary.DataLayer.Entiteties;
using OnlineLibrary.DataLayer.Repositories;

namespace OnlineLibrary.BusinessLayer.Service
{
    public class AuthorService: IAuthorService
    {
		private readonly IAuthorRepository _authorRepository;
		public AuthorService(IAuthorRepository authorRepository)
		{
			_authorRepository = authorRepository;
		}

	public List<Author> GetAllAuthors()
		{
			try
			{
				var authors = _authorRepository.GetAllAuthors();
				return authors;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
				throw;
			}
		}
		public Author FindAuthor(int id)
		{
			try
			{
				var authors = _authorRepository.FindAuthor(id);
				return authors;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw ex;
			}
		}
		public void AddAuthor(Author author)
		{
			try
			{
				if (_authorRepository.AuthorExists(author))
				{
					Console.WriteLine("This author already exists. It cannot be added.");
					return;
				}

				if (!IsAuthorValid(author))
				{
					Console.WriteLine("The author data is not entered correctly");
					return;
				}

				_authorRepository.AddAuthor(author);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw ex;
			}
		}
		public void RemoveAuthor(int id)
		{
			try
			{
					var authortoDelete = _authorRepository.FindAuthor(id);
					if (authortoDelete == null)
					{
						Console.WriteLine("Author does not exist");
					}

				_authorRepository.RemoveAuthor(authortoDelete);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					throw ex;
				}
			}

		public void UpdateAuthor(Author author)
		{
			try
			{
				if (!IsAuthorValid(author))
				{
					Console.WriteLine("Invalid author data. Update failed.");
					return;
				}

				if (!_authorRepository.AuthorExists(author))
				{
					Console.WriteLine("The author does not exist. Update failed.");
					return;
				}

				_authorRepository.UpdateAuthor(author);
			}
			catch (Exception ex)
			{
				Console.WriteLine("An error occurred while updating the book: " + ex.Message);
				throw ex;
			}
		}
		private bool IsAuthorValid(Author author)
		{
			if (string.IsNullOrWhiteSpace(author.FullName))
			{
				Console.WriteLine("Full name is mandatory.");
				return false;
			}
			return true;
		}
		public List<Author> FilterBook(string? fullname, string? description)
		{
			var authors = _authorRepository.GetAllAuthors();

			if (!string.IsNullOrEmpty(fullname))
			{
				authors = authors.Where(p => p.FullName.Contains(fullname))
				                 .ToList();
			}

			if (!string.IsNullOrEmpty(description))
			{
				authors = authors.Where(p => p.Description.Contains(description))
				                 .ToList();
			}

			return authors;
		}
	}
}
