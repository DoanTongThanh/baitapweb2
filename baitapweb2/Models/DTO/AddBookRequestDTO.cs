using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace baitapweb2.Models.DTO
{
    // Giữ tên AddBookDTO hoặc AddBookRequestDTO tùy theo tên file gốc của bạn.
    // Tôi giả định tên file là AddBookRequestDTO.cs như trong cấu trúc dự án của bạn.
    public class AddBookRequestDTO
    {
        // =========================================================
        // 1. Title: Bắt buộc và tối thiểu 10 ký tự
        // =========================================================
        [Required(ErrorMessage = "Title là bắt buộc.")]
        [MinLength(10, ErrorMessage = "Title phải có tối thiểu 10 ký tự.")]
        public string Title { get; set; }

        // Description không được rỗng
        [Required(ErrorMessage = "Description là bắt buộc.")]
        public string Description { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime? DateRead { get; set; }

        // =========================================================
        // 2. Rate: Phạm vi từ 0 đến 5
        // =========================================================
        [Range(0, 5, ErrorMessage = "Rate phải nằm trong khoảng từ 0 đến 5.")]
        public int? Rate { get; set; }

        // Giữ nguyên theo code bạn cung cấp
        [Required(ErrorMessage = "Genre là bắt buộc.")]
        public string Genre { get; set; }

        // Giữ nguyên theo code bạn cung cấp
        [Required(ErrorMessage = "CoverUrl là bắt buộc.")]
        public string CoverUrl { get; set; }

        // Khóa Ngoại cần thiết
        [Required(ErrorMessage = "PublisherId là bắt buộc.")]
        public int PublisherId { get; set; }

        // Danh sách ID Tác giả (Bắt buộc phải có ít nhất 1 tác giả)
        [Required(ErrorMessage = "Sách phải có ít nhất một tác giả.")]
        public List<int> AuthorIds { get; set; } = new List<int>();
    }
}