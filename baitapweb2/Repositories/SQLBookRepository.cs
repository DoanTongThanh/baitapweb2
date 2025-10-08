using baitapweb2.Data;
using baitapweb2.Models.Domain;
using baitapweb2.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;

namespace baitapweb2.Repositories
{
    public class SQLBookRepository : IBookRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLBookRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // READ: GetAllBooks - Logic đã được xác nhận và sửa lỗi
        public List<BookDTO> GetAllBooks()
        {
            var allBooksDomain = _dbContext.Books
                .Include(b => b.Publisher)
                .Include(b => b.Book_Authors).ThenInclude(ba => ba.Author)
                .ToList();

            var allBooksDTO = allBooksDomain.Select(book => new BookDTO()
            {
                Id = book.BookId,
                Title = book.Title,
                Description = book.Description,
                IsRead = book.IsRead,
                DateRead = book.DateRead,
                Rate = book.Rate,
                Genre = book.Genre,
                CoverUrl = book.CoverUrl,
                PublisherName = book.Publisher.Name,
                AuthorNames = book.Book_Authors.Select(n => n.Author.FullName).ToList()
            }).ToList();

            return allBooksDTO;
        }

        // READ: GetBookById - Logic đã được xác nhận và sửa lỗi
        public BookDTO GetBookById(int id)
        {
            var bookDomain = _dbContext.Books
                .Include(b => b.Publisher)
                .Include(b => b.Book_Authors).ThenInclude(ba => ba.Author)
                .FirstOrDefault(b => b.BookId == id);

            if (bookDomain == null)
            {
                return null;
            }

            var bookWithDTO = new BookDTO()
            {
                Id = bookDomain.BookId,
                Title = bookDomain.Title,
                Description = bookDomain.Description,
                IsRead = bookDomain.IsRead,
                DateRead = bookDomain.DateRead,
                Rate = bookDomain.Rate,
                Genre = bookDomain.Genre,
                CoverUrl = bookDomain.CoverUrl,
                PublisherName = bookDomain.Publisher.Name,
                AuthorNames = bookDomain.Book_Authors.Select(n => n.Author.FullName).ToList()
            };

            return bookWithDTO;
        }

        // CREATE: AddBook - Logic đã được xác nhận và sửa lỗi
        public Book AddBook(AddBookDTO bookRequest)
        {
            var bookDomainModel = new Book()
            {
                Title = bookRequest.Title,
                Description = bookRequest.Description,
                IsRead = bookRequest.IsRead,
                DateRead = bookRequest.DateRead,
                Rate = bookRequest.Rate,
                Genre = bookRequest.Genre,
                CoverUrl = bookRequest.CoverUrl,
                DateAdded = DateTime.Now,
                PublisherId = bookRequest.PublisherId
            };

            _dbContext.Books.Add(bookDomainModel);
            _dbContext.SaveChanges();

            foreach (var authorId in bookRequest.AuthorIds)
            {
                var bookAuthor = new Book_Author()
                {
                    BookId = bookDomainModel.BookId,
                    AuthorId = authorId
                };
                _dbContext.Book_Authors.Add(bookAuthor);
            }
            _dbContext.SaveChanges();

            return bookDomainModel;
        }

        // =========================================================================
        // UPDATE: UpdateBookById - Logic Mới
        // =========================================================================
        public Book UpdateBookById(int id, AddBookDTO bookRequest)
        {
            // 1. Tìm sách cần cập nhật
            var bookDomain = _dbContext.Books.FirstOrDefault(n => n.BookId == id);

            if (bookDomain == null)
            {
                return null;
            }

            // 2. Cập nhật thuộc tính Domain Model từ DTO
            bookDomain.Title = bookRequest.Title;
            bookDomain.Description = bookRequest.Description;
            bookDomain.IsRead = bookRequest.IsRead;
            bookDomain.DateRead = bookRequest.DateRead;
            bookDomain.Rate = bookRequest.Rate;
            bookDomain.Genre = bookRequest.Genre;
            bookDomain.CoverUrl = bookRequest.CoverUrl;
            // DateAdded KHÔNG nên được cập nhật
            bookDomain.PublisherId = bookRequest.PublisherId;

            _dbContext.SaveChanges();

            // 3. Xóa mối quan hệ Book_Author cũ
            var authorDomain = _dbContext.Book_Authors
                .Where(a => a.BookId == id)
                .ToList();

            if (authorDomain != null)
            {
                _dbContext.Book_Authors.RemoveRange(authorDomain);
                _dbContext.SaveChanges();
            }

            // 4. Thêm mối quan hệ Book_Author mới
            foreach (var authorId in bookRequest.AuthorIds)
            {
                var book_author = new Book_Author()
                {
                    BookId = id,
                    AuthorId = authorId
                };
                _dbContext.Book_Authors.Add(book_author);
            }
            _dbContext.SaveChanges();

            // 5. Trả về Domain Model đã cập nhật
            return bookDomain;
        }

        // =========================================================================
        // DELETE: DeleteBookById - Logic Mới
        // =========================================================================
        public Book? DeleteBookById(int id)
        {
            // 1. Tìm sách
            var bookDomain = _dbContext.Books.FirstOrDefault(n => n.BookId == id);

            if (bookDomain == null)
            {
                return null;
            }

            // 2. Xóa các mối quan hệ Book_Author liên quan
            var bookAuthorsDomain = _dbContext.Book_Authors.Where(a => a.BookId == id).ToList();
            _dbContext.Book_Authors.RemoveRange(bookAuthorsDomain);

            // 3. Xóa sách
            _dbContext.Books.Remove(bookDomain);

            _dbContext.SaveChanges();

            return bookDomain;
        }
    }
}