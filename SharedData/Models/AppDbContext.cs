using Microsoft.EntityFrameworkCore;

namespace SharedData.Models;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Episode> Episodes => Set<Episode>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Research> Researches => Set<Research>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>()
            .HasMany(a => a.Books)
            .WithOne(b => b.Author)
            .HasForeignKey(b => b.AuthorId);

        modelBuilder.Entity<Author>()
            .HasMany(a => a.Episodes)
            .WithOne(e => e.Author)
            .HasForeignKey(e => e.AuthorId);

        modelBuilder.Entity<Book>()
            .HasMany(b => b.Episodes)
            .WithOne(e => e.Book)
            .HasForeignKey(e => e.BookId)
            .OnDelete(DeleteBehavior.Restrict); // <--- Prevents multiple cascade paths        
    }
}
