using BookStore.Models;
using bookworm.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace bookworm.Data
{
    public class BookStoreContext:IdentityDbContext<ApplicationUser>
    {
        public BookStoreContext(DbContextOptions<BookStoreContext> options)
    : base(options)
        {
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the relationship between Review and Book
            modelBuilder.Entity<Review>()
                        .HasOne(r => r.User)
                        .WithMany()
                        .HasForeignKey(r => r.UserId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                        .HasOne(r => r.Book)
                        .WithMany()
                        .HasForeignKey(r => r.BookId)
                        .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WishList>()
               .HasOne(w => w.Book)
               .WithMany() // Assuming one book can be in multiple wishlists
               .HasForeignKey(w => w.BookId)
               .OnDelete(DeleteBehavior.NoAction); // Specify ON DELETE NO ACTION
            modelBuilder.Entity<PurchaseItem>()
                .HasOne(pi => pi.Purchase)
                .WithMany(p => p.PurchaseItems) // Assuming one purchase can have multiple purchase items
                .HasForeignKey(pi => pi.PurchaseId)
                .OnDelete(DeleteBehavior.NoAction); // Specify ON DELETE NO ACTION

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId);

            // Other entity configurations...

            base.OnModelCreating(modelBuilder);
        }

    }
}
