using System; // Cần cho DateTime
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace baitapweb2.Models.DTO
{
    // Đổi tên từ AddBookRequestDTO thành AddBookDTO
    public class AddBookDTO
    {
        // Thuộc tính bắt buộc (Client gửi lên)

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public bool IsRead { get; set; } = false; // Mặc định

        public DateTime? DateRead { get; set; }
        public int? Rate { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        public string CoverUrl { get; set; }

        // Khóa Ngoại cần thiết (Client gửi ID lên)
        [Required]
        public int PublisherId { get; set; }

        // Danh sách ID Tác giả (Client gửi lên)
        public List<int> AuthorIds { get; set; }
    }
}