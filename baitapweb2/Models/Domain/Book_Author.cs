// Trong Book_Author.cs:

using baitapweb2.Models.Domain; // Đảm bảo bạn có namespace này nếu Author nằm ở đó.

public class Book_Author
{
    // Khóa ngoại
    public int BookId { get; set; }
    public int AuthorId { get; set; }

    // Navigation Properties (Kiểu dữ liệu phải là số ít)
    public Book Book { get; set; }

    // LỖI CS0246 NẰM Ở ĐÂY NẾU BẠN GÕ LÀ 'public Authors Author { get; set; }'
    // PHẢI LÀ TÊN LỚP ENTITY (SỐ ÍT)
    public Author Author { get; set; }
}