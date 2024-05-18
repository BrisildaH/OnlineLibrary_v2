﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.DataLayer.Entiteties;

namespace OnlineLibrary.DataLayer.DBContext
{
    public class OnlineLibraryDbContext : IdentityDbContext
    {
        public OnlineLibraryDbContext()
        {
        }

        public OnlineLibraryDbContext(DbContextOptions<OnlineLibraryDbContext> options) : base(options)
        {
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientBook> ClientBooks { get; set; }
        public DbSet<User> Users { get; set; }
    }

}
