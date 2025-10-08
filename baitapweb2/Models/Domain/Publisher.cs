using System.ComponentModel.DataAnnotations;


namespace baitapweb2.Models.Domain
{
    public class Publisher
    {
        public int PublisherId { get; set; }
        public string Name { get; set; } 
        // ... các thuộc tính khác
        public ICollection<Book> Books { get; set; }
    }
}