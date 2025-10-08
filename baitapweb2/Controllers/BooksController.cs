using Microsoft.AspNetCore.Mvc;
using baitapweb2.Models.DTO;
using baitapweb2.Repositories;
using baitapweb2.Models.Domain;
using baitapweb2.Filters;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization; // Cần thiết
using Microsoft.Extensions.Logging; // Cần thiết
using System.Text.Json; // Cần thiết cho Log

namespace baitapweb2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IPublisherRepository _publisherRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly ILogger<BooksController> _logger; // Bổ sung Logger

        // Bổ sung ILogger vào Constructor
        public BooksController(IBookRepository bookRepository, IPublisherRepository publisherRepository, IAuthorRepository authorRepository, ILogger<BooksController> logger)
        {
            _bookRepository = bookRepository;
            _publisherRepository = publisherRepository;
            _authorRepository = authorRepository;
            _logger = logger; // Gán Logger
        }

        // =========================================================================
        // GET ALL BOOKS (READ) - FILTER, SORT, VÀ PAGINATION
        // =========================================================================
        [HttpGet]
        [Authorize(Roles = "Read")] // BỔ SUNG AUTHORIZATION
        public IActionResult GetAll(
            [FromQuery] string? filterOn,
            [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy,
            [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
        )
        {
            // BỔ SUNG LOGGING
            _logger.LogInformation("GetAll Book Action method was invoked");
            _logger.LogWarning("This is a warning log");
            _logger.LogError("This is a error log");

            var books = _bookRepository.GetAllBooks(
                filterOn,
                filterQuery,
                sortBy,
                isAscending ?? true,
                pageNumber,
                pageSize
            );

            // Log kết quả cuối cùng
            _logger.LogInformation($"Finished GetAllBook request with data {JsonSerializer.Serialize(books)}");

            return Ok(books);
        }

        // =========================================================================
        // GET BOOK BY ID (READ)
        // =========================================================================
        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles = "Read")] // BỔ SUNG AUTHORIZATION
        public IActionResult GetBookById([FromRoute] int id)
        {
            var bookDto = _bookRepository.GetBookById(id);

            if (bookDto == null)
            {
                _logger.LogWarning($"Book with ID {id} not found.");
                return NotFound();
            }

            return Ok(bookDto);
        }

        // =========================================================================
        // ADD NEW BOOK (CREATE)
        // =========================================================================
        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Write")] // BỔ SUNG AUTHORIZATION (Chỉ có quyền Write mới được tạo)
        public IActionResult AddBook([FromBody] AddBookRequestDTO addBookRequest)
        {
            if (!ValidateAddBook(addBookRequest))
            {
                _logger.LogError("Validation failed for AddBookRequest.");
                return BadRequest(ModelState);
            }

            Book addedBookDomain = _bookRepository.AddBook(addBookRequest);

            _logger.LogInformation($"Book added successfully with ID: {addedBookDomain.BookId}");
            return CreatedAtAction(nameof(GetBookById), new { id = addedBookDomain.BookId }, addedBookDomain);
        }

        // =========================================================================
        // UPDATE BOOK (UPDATE)
        // =========================================================================
        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        [Authorize(Roles = "Write")] // BỔ SUNG AUTHORIZATION (Chỉ có quyền Write mới được cập nhật)
        public IActionResult UpdateBookById([FromRoute] int id, [FromBody] AddBookRequestDTO updateBookRequest)
        {
            if (!ValidateAddBook(updateBookRequest))
            {
                _logger.LogError($"Validation failed for UpdateBookRequest ID: {id}.");
                return BadRequest(ModelState);
            }

            Book? updatedBookDomain = _bookRepository.UpdateBookById(id, updateBookRequest);

            if (updatedBookDomain == null)
            {
                _logger.LogWarning($"Book with ID {id} not found for update.");
                return NotFound();
            }

            _logger.LogInformation($"Book updated successfully with ID: {id}");
            return Ok(updatedBookDomain);
        }

        // =========================================================================
        // DELETE BOOK (DELETE)
        // =========================================================================
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "Write")] // BỔ SUNG AUTHORIZATION (Chỉ có quyền Write mới được xóa)
        public IActionResult DeleteBookById([FromRoute] int id)
        {
            Book? deletedBookDomain = _bookRepository.DeleteBookById(id);

            if (deletedBookDomain == null)
            {
                _logger.LogWarning($"Book with ID {id} not found for deletion.");
                return NotFound();
            }

            _logger.LogInformation($"Book deleted successfully with ID: {id}");
            return Ok(deletedBookDomain);
        }

        // =========================================================================
        // PRIVATE METHODS (LOGIC VALIDATION)
        // =========================================================================
        #region Private Methods
        // ... (Giữ nguyên phần ValidateAddBook)
        private bool ValidateAddBook(AddBookRequestDTO addBookRequest)
        {
            // 1. Kiểm tra Book phải có ít nhất 1 Author
            if (addBookRequest.AuthorIds == null || addBookRequest.AuthorIds.Count == 0)
            {
                ModelState.AddModelError(nameof(addBookRequest.AuthorIds), "Mỗi cuốn sách phải có ít nhất 1 tác giả.");
            }
            // 2. Kiểm tra PublisherID phải tồn tại
            var publisher = _publisherRepository.GetAllPublishers().FirstOrDefault(p => p.Id == addBookRequest.PublisherId);
            if (publisher == null)
            {
                ModelState.AddModelError(nameof(addBookRequest.PublisherId), $"Publisher với ID {addBookRequest.PublisherId} không tồn tại.");
            }
            // 3. Kiểm tra tất cả các AuthorID phải tồn tại
            if (addBookRequest.AuthorIds != null && addBookRequest.AuthorIds.Any())
            {
                var validAuthorCount = _authorRepository.GetAllAuthors()
                    .Count(a => addBookRequest.AuthorIds.Contains(a.Id));

                if (validAuthorCount != addBookRequest.AuthorIds.Count)
                {
                    ModelState.AddModelError(nameof(addBookRequest.AuthorIds), "Một hoặc nhiều AuthorID không hợp lệ hoặc không tồn tại.");
                }
            }

            return ModelState.IsValid;
        }
        #endregion
    }
}