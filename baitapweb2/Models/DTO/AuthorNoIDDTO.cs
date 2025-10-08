using System.ComponentModel.DataAnnotations;

namespace baitapweb2.Models.DTO
{
    // DTO để nhận thông tin Author (cho Add/Update)
    public class AuthorNoIDDTO
    {
        [Required]
        public string FullName { get; set; }
    }
}