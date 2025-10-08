using baitapweb2.Data;
using baitapweb2.Models.Domain;
using baitapweb2.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace baitapweb2.Repositories
{
    // Triển khai IBookRepository
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _dbContext;

        public BookRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // ======================= CREATE =======================
        public Book AddBook(AddBookDTO bookRequest)
        {
            // 1. Kiểm tra Publisher đã được loại bỏ ở Controller (có thể đưa về sau)

            // 2. Map DTO sang Domain Model
            var bookDomain = new Book
            {
                Title = bookRequest.Title,
                Description = bookRequest.Description,
                IsRead = bookRequest.IsRead,
                DateRead = bookRequest.IsRead ? bookRequest.DateRead : null,
                Rate = bookRequest.IsRead ? bookRequest.Rate : null,
                Genre = bookRequest.Genre,
                CoverUrl = bookRequest.CoverUrl,
                DateAdded = DateTime.Now,
                PublisherId = bookRequest.PublisherId,
                Book_Authors = new List<Book_Author>()
            };

            // 3. Xử lý mối quan hệ N-N (Book_Author)
            foreach (var authorId in bookRequest.AuthorIds)
            {
                // Logic kiểm tra Author (nên nằm ở Controller/Service)
                bookDomain.Book_Authors.Add(new Book_Author()
                {
                    AuthorId = authorId,
                    Book = bookDomain
                });
            }

            // 4. Lưu vào Database
            _dbContext.Books.Add(bookDomain);
            _dbContext.SaveChanges();

            // Trả về đối tượng Domain đã được tạo
            return bookDomain;
        }

        // ======================= DELETE =======================
        public Book DeleteBookById(int id)
        {
            var bookToDelete = _dbContext.Books.FirstOrDefault(b => b.BookId == id);

            if (bookToDelete == null)
            {
                return null;
            }

            // Xóa các bản ghi liên quan trong bảng Book_Author
            var bookAuthorsToDelete = _dbContext.Book_Authors.Where(ba => ba.BookId == id).ToList();
            _dbContext.Book_Authors.RemoveRange(bookAuthorsToDelete);

            // Xóa cuốn sách
            _dbContext.Books.Remove(bookToDelete);
            _dbContext.SaveChanges();

            return bookToDelete;
        }

        // ======================= READ ALL =======================
        public List<BookDTO> GetAllBooks()
        {
            // Logic truy vấn và Map DTO
            var allBooksWithDetails = _dbContext.Books
                .Include(book => book.Publisher)
                .Include(book => book.Book_Authors).ThenInclude(ba => ba.Author)

                .Select(book => new BookDTO()
                {
                    Id = book.BookId,
                    Title = book.Title,
                    Description = book.Description,
                    IsRead = book.IsRead,
                    DateRead = book.IsRead ? book.DateRead : null,
                    Rate = book.IsRead ? book.Rate : null,
                    Genre = book.Genre,
                    CoverUrl = book.CoverUrl,
                    DateAdded = book.DateAdded,

                    PublisherName = book.Publisher.Name,
                    AuthorNames = book.Book_Authors
                                       .Select(ba => ba.Author.FullName)
                                       .ToList()
                })
                .ToList();

            return allBooksWithDetails;
        }

        // ======================= READ BY ID =======================
        public BookDTO GetBookById(int id)
        {
            // Logic truy vấn và Map DTO
            var bookWithDetails = _dbContext.Books
                .Where(book => book.BookId == id)
                .Include(book => book.Publisher)
                .Include(book => book.Book_Authors).ThenInclude(ba => ba.Author)

                .Select(book => new BookDTO()
                {
                    Id = book.BookId,
                    Title = book.Title,
                    Description = book.Description,
                    IsRead = book.IsRead,
                    DateRead = book.IsRead ? book.DateRead : null,
                    Rate = book.IsRead ? book.Rate : null,
                    Genre = book.Genre,
                    CoverUrl = book.CoverUrl,
                    DateAdded = book.DateAdded,

                    PublisherName = book.Publisher.Name,
                    AuthorNames = book.Book_Authors
                                       .Select(ba => ba.Author.FullName)
                                       .ToList()
                })
                .FirstOrDefault();

            return bookWithDetails;
        }

        // ======================= UPDATE =======================
        public Book UpdateBookById(int id, AddBookDTO bookRequest)
        {
            var existingBook = _dbContext.Books
                                 .Include(b => b.Book_Authors)
                                 .FirstOrDefault(b => b.BookId == id);

            if (existingBook == null)
            {
                return null;
            }

            // Cập nhật các thuộc tính cơ bản
            existingBook.Title = bookRequest.Title;
            existingBook.Description = bookRequest.Description;
            existingBook.IsRead = bookRequest.IsRead;
            existingBook.DateRead = bookRequest.IsRead ? bookRequest.DateRead : null;
            existingBook.Rate = bookRequest.IsRead ? bookRequest.Rate : null;
            existingBook.Genre = bookRequest.Genre;
            existingBook.CoverUrl = bookRequest.CoverUrl;
            existingBook.PublisherId = bookRequest.PublisherId; // Cập nhật Publisher

            // Xử lý mối quan hệ N-N: Xóa cũ, thêm mới
            _dbContext.Book_Authors.RemoveRange(existingBook.Book_Authors);

            var newBookAuthors = new List<Book_Author>();
            foreach (var authorId in bookRequest.AuthorIds)
            {
                // Logic kiểm tra Author (nên nằm ở Controller/Service)
                newBookAuthors.Add(new Book_Author()
                {
                    BookId = existingBook.BookId,
                    AuthorId = authorId
                });
            }

            existingBook.Book_Authors = newBookAuthors;

            _dbContext.SaveChanges();

            return existingBook;
        }
    }
}