// File: Data/AppDbContext.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using baitapweb2.Models.Domain; // Chắc chắn có using này để truy cập Image

namespace baitapweb2.Data
{
    // Kế thừa từ IdentityDbContext<IdentityUser> để hỗ trợ các bảng Identity
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // =========================================================
        // KHAI BÁO CÁC DbSet CỦA ỨNG DỤNG
        // =========================================================
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Book_Author> Book_Authors { get; set; }

        // ************ BỔ SUNG: DbSet cho Image ************
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // BẮT BUỘC phải gọi base.OnModelCreating(modelBuilder);
            // để cấu hình các bảng Identity (Users, Roles, v.v.)
            base.OnModelCreating(modelBuilder);

            // Cấu hình quan hệ Many-to-Many cho Book_Author
            modelBuilder.Entity<Book_Author>()
        .HasKey(ba => new { ba.BookId, ba.AuthorId });
        }
    }
}