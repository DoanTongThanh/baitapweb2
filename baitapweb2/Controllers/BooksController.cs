using Microsoft.AspNetCore.Mvc;
using baitapweb2.Models.DTO;
using baitapweb2.Models.Domain;
using baitapweb2.Repositories; // Thêm namespace này!
using System.Linq;
using System.Collections.Generic;
using System;

namespace baitapweb2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        // THAY THẾ AppDbContext bằng IBookRepository
        private readonly IBookRepository _bookRepository;

        // Dependency Injection cho IBookRepository
        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        // =================================================================
        // 1. GET ALL BOOKS (DTO)
        // Logic truy vấn đã chuyển vào Repository
        // =================================================================
        [HttpGet("get-all-books")]
        public IActionResult GetAllBooks()
        {
            var allBooksWithDetails = _bookRepository.GetAllBooks();
            return Ok(allBooksWithDetails);
        }

        // =================================================================
        // 2. GET BOOK BY ID (DTO)
        // Logic truy vấn đã chuyển vào Repository
        // =================================================================
        [HttpGet("{id}")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            var bookWithDetails = _bookRepository.GetBookById(id);

            if (bookWithDetails == null)
            {
                return NotFound($"Book with Id = {id} not found.");
            }

            return Ok(bookWithDetails);
        }

        // =================================================================
        // 3. ADD BOOK (POST)
        // Logic truy vấn (ADD) đã chuyển vào Repository. 
        // Logic kiểm tra ID (VALIDATION) đã được đơn giản hóa.
        // =================================================================
        [HttpPost]
        public IActionResult AddBook([FromBody] AddBookDTO bookRequest)
        {
            // Logic kiểm tra ID nên nằm trong Repository hoặc Service Layer.
            // Để code Controller gọn gàng nhất, ta chỉ gọi phương thức Add.
            var bookDomain = _bookRepository.AddBook(bookRequest);

            // Kiểm tra lỗi có thể xảy ra trong Repository (ví dụ: ID không hợp lệ)
            if (bookDomain == null)
            {
                return BadRequest("Invalid Publisher or Author ID(s) found during creation.");
            }

            // Trả về 201 Created
            return CreatedAtAction(nameof(GetBookById), new { id = bookDomain.BookId }, bookDomain);
        }

        // =================================================================
        // 4. UPDATE BOOK BY ID (PUT)
        // Logic truy vấn (UPDATE) đã chuyển vào Repository
        // =================================================================
        [HttpPut("{id}")]
        public IActionResult UpdateBookById([FromRoute] int id, [FromBody] AddBookDTO bookRequest)
        {
            var updatedBook = _bookRepository.UpdateBookById(id, bookRequest);

            if (updatedBook == null)
            {
                // Nếu Repository trả về null, tức là không tìm thấy sách
                return NotFound($"Book with Id = {id} not found for updating.");
            }

            return Ok(updatedBook);
        }

        // =================================================================
        // 5. DELETE BOOK BY ID (DELETE)
        // Logic truy vấn (DELETE) đã chuyển vào Repository
        // =================================================================
        [HttpDelete("{id}")]
        public IActionResult DeleteBookById([FromRoute] int id)
        {
            var deletedBook = _bookRepository.DeleteBookById(id);

            if (deletedBook == null)
            {
                // Nếu Repository trả về null, tức là không tìm thấy sách
                return NotFound($"Book with Id = {id} not found for deletion.");
            }

            return Ok($"Book with Id = {id} successfully deleted.");
        }
    }
}