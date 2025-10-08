// File: Models/DTO/ImageUploadRequestDTO.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // Cần thiết cho IFormFile

namespace baitapweb2.Models.DTO
{
    public class ImageUploadRequestDTO
    {
        [Required]
        public IFormFile File { get; set; } // File được upload

        [Required]
        public string FileName { get; set; } // Tên file do người dùng nhập

        public string? FileDescription { get; set; } // Mô tả file (tùy chọn)
    }
}