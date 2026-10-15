using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookAuthors.Models;

namespace BookAuthors.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data for Authors
            modelBuilder.Entity<Author>().HasData(
                new Author { AuthorId = 1, Name = "Author 1" },
                new Author { AuthorId = 2, Name = "Author 2" },
                new Author { AuthorId = 3, Name = "Author 3" }
            );

            // Seed data for Books
            modelBuilder.Entity<Book>().HasData(
                new Book { BookId = 1, Title = "Book 1" },
                new Book { BookId = 2, Title = "Book 2" },
                new Book { BookId = 3, Title = "Book 3" }
            );

            // Seed data for many- many relationship between Authors and Books
            modelBuilder.Entity<Author>()
                .HasMany(a => a.Books)
                .WithMany(b => b.Authors)
                .UsingEntity(j => j.HasData(
                    new { AuthorsAuthorId = 1, BooksBookId = 1 },
                    new { AuthorsAuthorId = 1, BooksBookId = 2 },
                    new { AuthorsAuthorId = 2, BooksBookId = 2 },
                    new { AuthorsAuthorId = 2, BooksBookId = 3 },
                    new { AuthorsAuthorId = 3, BooksBookId = 1 }
                ));

        }
    }
}
