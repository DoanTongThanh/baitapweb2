// File: Models/DTO/RegisterRequestDTO.cs
using System.ComponentModel.DataAnnotations;

namespace baitapweb2.Models.DTO
{
    public class RegisterRequestDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)] // Có thể dùng EmailAddress nếu username là email
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        // Vai trò người dùng (ví dụ: ["User", "Admin"])
        public List<string> Roles { get; set; } = new List<string>();
    }
}