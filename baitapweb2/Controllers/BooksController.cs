using Microsoft.AspNetCore.Mvc;
using baitapweb2.Models.DTO;
using baitapweb2.Repositories;
using baitapweb2.Models.Domain; // Cần thiết để trả về Book Domain Model trong Add/Update

namespace baitapweb2.Controllers
{
    // Cấu hình Controller
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        // Khai báo Repository
        private readonly IBookRepository _bookRepository;

        // Dependency Injection: Nhận IBookRepository qua constructor
        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        // =========================================================================
        // GET ALL BOOKS (READ)
        // URL: GET /api/Books
        // =========================================================================
        [HttpGet]
        public IActionResult GetAll()
        {
            var books = _bookRepository.GetAllBooks();
            return Ok(books);
        }

        // =========================================================================
        // GET BOOK BY ID (READ)
        // URL: GET /api/Books/{id}
        // =========================================================================
        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            var bookDto = _bookRepository.GetBookById(id);

            if (bookDto == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy
            }

            return Ok(bookDto);
        }

        // =========================================================================
        // ADD NEW BOOK (CREATE)
        // URL: POST /api/Books
        // =========================================================================
        [HttpPost]
        public IActionResult AddBook([FromBody] AddBookDTO addBookRequest)
        {
            // Kiểm tra tính hợp lệ của Model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Gọi Repository để thêm sách. Trả về Domain Model (Book)
            Book addedBookDomain = _bookRepository.AddBook(addBookRequest);

            // Tùy chọn: Trả về 201 Created và đường dẫn đến tài nguyên mới
            return CreatedAtAction(nameof(GetBookById), new { id = addedBookDomain.BookId }, addedBookDomain);
        }

        // =========================================================================
        // UPDATE BOOK (UPDATE)
        // URL: PUT /api/Books/{id}
        // =========================================================================
        [HttpPut]
        [Route("{id:int}")]
        public IActionResult UpdateBookById([FromRoute] int id, [FromBody] AddBookDTO updateBookRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Gọi Repository để cập nhật sách. Trả về Domain Model (Book)
            Book? updatedBookDomain = _bookRepository.UpdateBookById(id, updateBookRequest);

            if (updatedBookDomain == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy sách
            }

            return Ok(updatedBookDomain);
        }

        // =========================================================================
        // DELETE BOOK (DELETE)
        // URL: DELETE /api/Books/{id}
        // =========================================================================
        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult DeleteBookById([FromRoute] int id)
        {
            // Gọi Repository để xóa sách
            Book? deletedBookDomain = _bookRepository.DeleteBookById(id);

            if (deletedBookDomain == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy
            }

            // Trả về sách đã xóa (hoặc NoContent)
            return Ok(deletedBookDomain);
            // Hoặc dùng: return NoContent(); để trả về 204
        }
    }
}