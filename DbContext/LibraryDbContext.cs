using Microsoft.EntityFrameworkCore;
using LibraryApi.Models;

namespace LibraryApi.Data 
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<Shelf> Shelves => Set<Shelf>();
        public DbSet<Bookshelf> Bookshelves => Set<Bookshelf>();
    }
}
