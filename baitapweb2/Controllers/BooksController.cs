using Microsoft.AspNetCore.Mvc;
using baitapweb2.Models.DTO;
using baitapweb2.Repositories;
using baitapweb2.Models.Domain;
using baitapweb2.Filters;
using System.Linq;
using System.Net; // Cần thiết nếu sử dụng HttpStatusCode

namespace baitapweb2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IPublisherRepository _publisherRepository;
        private readonly IAuthorRepository _authorRepository;

        public BooksController(IBookRepository bookRepository, IPublisherRepository publisherRepository, IAuthorRepository authorRepository)
        {
            _bookRepository = bookRepository;
            _publisherRepository = publisherRepository;
            _authorRepository = authorRepository;
        }

        // =========================================================================
        // GET ALL BOOKS (READ) - FILTER, SORT, VÀ PAGINATION
        // =========================================================================
        [HttpGet]
        public IActionResult GetAll(
            [FromQuery] string? filterOn,
            [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy,
            [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var books = _bookRepository.GetAllBooks(
                filterOn,
                filterQuery,
                sortBy,
                isAscending ?? true,
                pageNumber,
                pageSize
            );
            return Ok(books);
        }

        // =========================================================================
        // GET BOOK BY ID (READ)
        // =========================================================================
        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            var bookDto = _bookRepository.GetBookById(id);

            if (bookDto == null)
            {
                return NotFound();
            }

            return Ok(bookDto);
        }

        // =========================================================================
        // ADD NEW BOOK (CREATE)
        // =========================================================================
        [HttpPost]
        [ValidateModel]
        public IActionResult AddBook([FromBody] AddBookRequestDTO addBookRequest) // ĐÃ SỬA: AddBookRequestDTO
        {
            if (!ValidateAddBook(addBookRequest))
            {
                return BadRequest(ModelState);
            }

            Book addedBookDomain = _bookRepository.AddBook(addBookRequest);

            // ĐÃ SỬA LỖI CS1061: Dùng addedBookDomain.BookId
            return CreatedAtAction(nameof(GetBookById), new { id = addedBookDomain.BookId }, addedBookDomain);
        }

        // =========================================================================
        // UPDATE BOOK (UPDATE)
        // =========================================================================
        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        public IActionResult UpdateBookById([FromRoute] int id, [FromBody] AddBookRequestDTO updateBookRequest) // ĐÃ SỬA: AddBookRequestDTO
        {
            if (!ValidateAddBook(updateBookRequest))
            {
                return BadRequest(ModelState);
            }

            Book? updatedBookDomain = _bookRepository.UpdateBookById(id, updateBookRequest);

            if (updatedBookDomain == null)
            {
                return NotFound();
            }

            return Ok(updatedBookDomain);
        }

        // =========================================================================
        // DELETE BOOK (DELETE)
        // =========================================================================
        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult DeleteBookById([FromRoute] int id)
        {
            Book? deletedBookDomain = _bookRepository.DeleteBookById(id);

            if (deletedBookDomain == null)
            {
                return NotFound();
            }

            return Ok(deletedBookDomain);
        }

        // =========================================================================
        // PRIVATE METHODS (LOGIC VALIDATION)
        // =========================================================================
        #region Private Methods
        private bool ValidateAddBook(AddBookRequestDTO addBookRequest) // ĐÃ SỬA: AddBookRequestDTO
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