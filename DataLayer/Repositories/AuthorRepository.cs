using OnlineLibrary.DataLayer.DBContext;
using OnlineLibrary.DataLayer.Entiteties;

namespace OnlineLibrary.DataLayer.Repositories
{
	public class AuthorRepository: IAuthorRepository
	{
		private readonly OnlineLibraryDbContext onlineLibraryDbContext;
		public AuthorRepository()
		{
			onlineLibraryDbContext = new OnlineLibraryDbContext();
		}

		public List<Author> GetAllAuthors()
		{
			var authors = onlineLibraryDbContext.Authors.ToList();
			return authors;
		}
		public Author FindAuthor(int id)
		{
			try
			{
				var authors = onlineLibraryDbContext.Authors.Where(p => p.Id == id)
					.FirstOrDefault();
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
				onlineLibraryDbContext.Add(author);
				onlineLibraryDbContext.SaveChanges();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw ex;
			}
		}
		public void RemoveAuthor(Author author)
		{
			try
			{
				onlineLibraryDbContext.Remove(author);
				onlineLibraryDbContext.SaveChanges();
				Console.WriteLine("Author removed");
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.Message.ToString());
				throw ex;
			}
		}
		public void UpdateAuthor(Author author)
		{
			try
			{
				var authorExists = onlineLibraryDbContext.Authors.Where(p => p.Id == author.Id)
				.FirstOrDefault();

				if (authorExists == null)
				{
					throw new Exception("Record does not exists");
				}

				authorExists.FullName = author.FullName;
				authorExists.Description = author.Description;
				authorExists.NumberOfBooks = author.NumberOfBooks;

				onlineLibraryDbContext.Authors.Update(authorExists);
				onlineLibraryDbContext.SaveChanges();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw ex;
			}
		}
		public bool AuthorExists(Author author)
		{
			return onlineLibraryDbContext.Authors.Any(a => a.Id == author.Id);
		}
	}

}
