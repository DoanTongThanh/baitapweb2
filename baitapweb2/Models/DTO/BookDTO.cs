using System; // <--- CẦN THÊM USING NÀY CHO DateTime
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Không bắt buộc ở đây, nhưng tốt cho cú pháp

namespace baitapweb2.Models.DTO
{
    // Đã đổi tên thành BookDTO
    public class BookDTO
    {
        // Sử dụng Id thay vì BookId để chuẩn hóa API
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public int? Rate { get; set; }
        public string Genre { get; set; }
        public string CoverUrl { get; set; }
        public DateTime DateAdded { get; set; }

        // Thay thế Khóa Ngoại (PublisherId) bằng Tên Nhà xuất bản
        public string PublisherName { get; set; }

        // Thay thế collection Book_Authors bằng danh sách tên Tác giả
        public List<string> AuthorNames { get; set; }
    }
}