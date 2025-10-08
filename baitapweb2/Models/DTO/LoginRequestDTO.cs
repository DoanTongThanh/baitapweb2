// File: Models/DTO/LoginRequestDTO.cs
using System.ComponentModel.DataAnnotations;

namespace baitapweb2.Models.DTO
{
    public class LoginRequestDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)] // Có thể dùng EmailAddress nếu username là email
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}