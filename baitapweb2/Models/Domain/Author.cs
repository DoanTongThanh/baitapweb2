
using System.ComponentModel.DataAnnotations;


namespace baitapweb2.Models.Domain
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string FullName { get; set; } 
        // ... các thuộc tính khác
        public ICollection<Book_Author> Book_Authors { get; set; }
    }
}

