// File: Models/Domain/Image.cs
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http; // Cần thiết cho IFormFile

namespace baitapweb2.Models.Domain
{
    public class Image
    {
        public int Id { get; set; }

        [NotMapped] // IFormFile không được ánh xạ tới cột trong database
        public IFormFile File { get; set; }

        public string FileName { get; set; }
        public string? FileDescription { get; set; }
        public string FileExtension { get; set; }
        public long FileSizeInBytes { get; set; }
        public string FilePath { get; set; } // Đường dẫn URL để truy cập file
    }
}