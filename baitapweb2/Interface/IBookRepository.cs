using baitapweb2.Models.DTO;
using baitapweb2.Models.Domain;
using System.Collections.Generic;

namespace baitapweb2.Repositories
{
    public interface IBookRepository
    {
        // =========================================================================
        // READ ALL: FILTER, SORT, VÀ PAGINATION
        // Chữ ký hàm phải có 6 tham số để khớp với BooksController
        // =========================================================================
        List<BookDTO> GetAllBooks(
            string? filterOn = null,
            string? filterQuery = null,
            string? sortBy = null,
            bool isAscending = true,
            int pageNumber = 1,
            int pageSize = 10
        );

        // =========================================================================
        // READ BY ID
        // Trả về DTO chi tiết (BookWithAuthorAndPublisherDTO)
        // =========================================================================
        BookDTO? GetBookById(int id);

        // =========================================================================
        // CREATE
        // Sử dụng AddBookRequestDTO
        // =========================================================================
        Book AddBook(AddBookRequestDTO addBookRequest);

        // =========================================================================
        // UPDATE
        // Sử dụng AddBookRequestDTO
        // =========================================================================
        Book? UpdateBookById(int id, AddBookRequestDTO updateBookRequest);

        // =========================================================================
        // DELETE
        // =========================================================================
        Book? DeleteBookById(int id);
    }
}