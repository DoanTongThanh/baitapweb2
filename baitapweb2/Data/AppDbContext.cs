// File: Data/AppDbContext.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Cần using này
using Microsoft.AspNetCore.Identity; // Cần using này
using baitapweb2.Models.Domain; // Đảm bảo bạn có using này để truy cập các Model

namespace baitapweb2.Data
{
    // Kế thừa từ IdentityDbContext<IdentityUser> để hỗ trợ các bảng Identity
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // =========================================================
        // KHAI BÁO LẠI CÁC DbSet BỊ THIẾU Ở ĐÂY
        // =========================================================
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Book_Author> Book_Authors { get; set; }

        // Bạn có thể có thêm phương thức OnModelCreating để cấu hình khóa ngoại, 
        // nhưng đây là phần cơ bản để sửa lỗi hiện tại.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // BẮT BUỘC phải gọi base.OnModelCreating(modelBuilder);
            // để cấu hình các bảng Identity (Users, Roles, v.v.)
            base.OnModelCreating(modelBuilder);

            // Nếu bạn có cấu hình quan hệ Many-to-Many cho Book_Author ở đây, hãy để nó lại.
            // Ví dụ:
            modelBuilder.Entity<Book_Author>()
                .HasKey(ba => new { ba.BookId, ba.AuthorId });
        }
    }
}