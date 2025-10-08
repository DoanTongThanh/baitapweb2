using baitapweb2.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace baitapweb2.Data
{
    public class AppDbContext : DbContext
    {
        // Bắt buộc phải có constructor này!
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // --- 1. DBSET CẦN THIẾT ---
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book_Author> Book_Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- 2. CẤU HÌNH CHO BOOK_AUTHOR (N-N) ---

            // 2.1. ĐỊNH NGHĨA KHÓA CHÍNH KÉP
            modelBuilder.Entity<Book_Author>()
                .HasKey(ba => new { ba.BookId, ba.AuthorId });

            // 2.2. Mối quan hệ N-N (Phần Book)
            modelBuilder.Entity<Book_Author>()
                .HasOne(ba => ba.Book)
                .WithMany(b => b.Book_Authors)
                .HasForeignKey(ba => ba.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            // 2.3. Mối quan hệ N-N (Phần Author)
            modelBuilder.Entity<Book_Author>()
                .HasOne(ba => ba.Author)
                .WithMany(a => a.Book_Authors)
                .HasForeignKey(ba => ba.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);


            // --- 3. CẤU HÌNH CHO BOOK - PUBLISHER (1-N) ---

            // 3.1. Thiết lập mối quan hệ 1-N rõ ràng
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Publisher)
                .WithMany(p => p.Books)
                .HasForeignKey(b => b.PublisherId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


            // --- 4. ÁP DỤNG THAY ĐỔI CƠ SỞ (BASE) ---
            base.OnModelCreating(modelBuilder);
        }

        // ĐÃ XÓA định nghĩa lại lớp Publisher tại đây
    }
}