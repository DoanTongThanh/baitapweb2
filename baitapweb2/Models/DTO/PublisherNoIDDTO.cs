using System.ComponentModel.DataAnnotations;

namespace baitapweb2.Models.DTO
{
    // DTO để nhận thông tin Publisher (cho Add/Update)
    public class PublisherNoIDDTO
    {
        [Required]
        public string Name { get; set; }
    }
}