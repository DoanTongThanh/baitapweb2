using System.ComponentModel.DataAnnotations;

namespace baitapweb2.Models.DTO
{
    // DTO để trả về thông tin Publisher (bao gồm ID)
    public class PublisherDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}