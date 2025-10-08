using baitapweb2.Models.Domain;
using baitapweb2.Models.DTO;
using System.Collections.Generic;

namespace baitapweb2.Repositories
{
    // Chúng ta định nghĩa các hành động CRUD
    public interface IBookRepository
    {
        // READ
        List<BookDTO> GetAllBooks();
        BookDTO GetBookById(int id);

        // CREATE
        Book AddBook(AddBookDTO bookRequest);

        // UPDATE
        Book UpdateBookById(int id, AddBookDTO bookRequest);

        // DELETE
        Book? DeleteBookById(int id); // Sửa: Dùng Book? để cho phép trả về null nếu không tìm thấy sách
    }
}