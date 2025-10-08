using System.ComponentModel.DataAnnotations;

namespace baitapweb2.Models.DTO
{
    // DTO để trả về thông tin Author (bao gồm ID)
    public class AuthorDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
    }
}