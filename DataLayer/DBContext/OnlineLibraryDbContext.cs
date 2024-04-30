using Microsoft.EntityFrameworkCore;
using OnlineLibrary.DataLayer.Entiteties;

namespace OnlineLibrary.DataLayer.DBContext
{
    public class OnlineLibraryDbContext : DbContext
    {
        public OnlineLibraryDbContext()
        {
        }

        public OnlineLibraryDbContext(DbContextOptions<OnlineLibraryDbContext> options) : base(options)
        {
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
       
    }

}
