using baitapweb2.Models.Domain;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
// Bỏ qua using Microsoft.EntityFrameworkCore; nếu nó gây lỗi biên dịch ở đây.

namespace baitapweb2.Models.Domain
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public int? Rate { get; set; }
        public string Genre { get; set; }
        public string CoverUrl { get; set; }
        public DateTime DateAdded { get; set; }

        // Khóa Ngoại (Foreign Key)
        public int PublisherId { get; set; }

        // Thuộc tính điều hướng CHÍNH
        public Publisher Publisher { get; set; }

        public ICollection<Book_Author> Book_Authors { get; set; }

        // CHỈ CÓ CÁC THUỘC TÍNH TRÊN.
        // ĐẢM BẢO KHÔNG CÓ DÒNG KHÁC TRỎ ĐẾN PUBLISHER.
    }
}