using baitapweb2.Data;
using baitapweb2.Models.Domain;
using baitapweb2.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace baitapweb2.Repositories
{
    public class SQLBookRepository : IBookRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLBookRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // =========================================================================
        // READ ALL: GET ALL BOOKS (ĐÃ KHẮC PHỤC LỖI CS0535 VÀ MAPPING)
        // =========================================================================
        public List<BookDTO> GetAllBooks(
            string? filterOn = null,
            string? filterQuery = null,
            string? sortBy = null,
            bool isAscending = true,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var allBooks = _dbContext.Books
                .Include(b => b.Publisher)
                .Include(b => b.Book_Authors).ThenInclude(ba => ba.Author)
                .AsQueryable();

            // 1. FILTERING
            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                if (filterOn.Equals("title", StringComparison.OrdinalIgnoreCase))
                {
                    allBooks = allBooks.Where(x => x.Title.Contains(filterQuery));
                }
            }

            // 2. SORTING
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortBy.Equals("title", StringComparison.OrdinalIgnoreCase))
                {
                    allBooks = isAscending ? allBooks.OrderBy(x => x.Title) : allBooks.OrderByDescending(x => x.Title);
                }
            }

            // 3. PAGINATION
            var skipResults = (pageNumber - 1) * pageSize;

            // Ánh xạ Domain Model sang DTO chi tiết
            return allBooks.Skip(skipResults).Take(pageSize)
                .Select(book => new BookDTO()
                {
                    Id = book.BookId, // ĐÃ SỬA LỖI CS1061
                    Title = book.Title,
                    Description = book.Description,
                    IsRead = book.IsRead,
                    DateRead = book.IsRead && book.DateRead.HasValue ? book.DateRead.Value : (DateTime?)null,
                    Rate = book.IsRead && book.Rate.HasValue ? book.Rate.Value : (int?)null,
                    Genre = book.Genre,
                    CoverUrl = book.CoverUrl,
                    PublisherName = book.Publisher.Name,
                    AuthorNames = book.Book_Authors.Select(n => n.Author.FullName).ToList()
                }).ToList();
        }

        // =========================================================================
        // CREATE: ADD BOOK (ĐÃ KHẮC PHỤC LỖI CS1061 VÀ DTO)
        // =========================================================================
        public Book AddBook(AddBookRequestDTO addBookRequest) // ĐÃ SỬA DTO
        {
            var bookDomain = new Book
            {
                Title = addBookRequest.Title,
                Description = addBookRequest.Description,
                IsRead = addBookRequest.IsRead,
                DateRead = addBookRequest.IsRead ? addBookRequest.DateRead : null,
                Rate = addBookRequest.IsRead ? addBookRequest.Rate : null,
                Genre = addBookRequest.Genre,
                CoverUrl = addBookRequest.CoverUrl,
                PublisherId = addBookRequest.PublisherId
            };

            _dbContext.Books.Add(bookDomain);
            _dbContext.SaveChanges();

            // Thêm các mối quan hệ Book_Author
            if (addBookRequest.AuthorIds != null && addBookRequest.AuthorIds.Any())
            {
                foreach (var authorId in addBookRequest.AuthorIds)
                {
                    var bookAuthor = new Book_Author
                    {
                        BookId = bookDomain.BookId, // ĐÃ SỬA LỖI CS1061
                        AuthorId = authorId
                    };
                    _dbContext.Book_Authors.Add(bookAuthor);
                }
                _dbContext.SaveChanges();
            }

            return bookDomain;
        }

        // =========================================================================
        // READ BY ID (ĐÃ KHẮC PHỤC LỖI CS1061 VÀ DTO)
        // =========================================================================
        public BookDTO? GetBookById(int id)
        {
            var bookDomain = _dbContext.Books
                .Include(b => b.Publisher)
                .Include(b => b.Book_Authors).ThenInclude(ba => ba.Author)
                .FirstOrDefault(b => b.BookId == id); // ĐÃ SỬA LỖI CS1061

            if (bookDomain == null) return null;

            var bookDto = new BookDTO
            {
                Id = bookDomain.BookId, // ĐÃ SỬA LỖI CS1061
                Title = bookDomain.Title,
                Description = bookDomain.Description,
                IsRead = bookDomain.IsRead,
                DateRead = bookDomain.DateRead,
                Rate = bookDomain.Rate,
                Genre = bookDomain.Genre,
                CoverUrl = bookDomain.CoverUrl,
                PublisherName = bookDomain.Publisher.Name,
                AuthorNames = bookDomain.Book_Authors.Select(ba => ba.Author.FullName).ToList()
            };
            return bookDto;
        }

        // =========================================================================
        // UPDATE (ĐÃ KHẮC PHỤC LỖI CS1061 VÀ DTO)
        // =========================================================================
        public Book? UpdateBookById(int id, AddBookRequestDTO updateBookRequest) // ĐÃ SỬA DTO
        {
            var existingBook = _dbContext.Books
                .Include(b => b.Book_Authors)
                .FirstOrDefault(b => b.BookId == id); // ĐÃ SỬA LỖI CS1061

            if (existingBook == null) return null;

            // Cập nhật thuộc tính
            existingBook.Title = updateBookRequest.Title;
            existingBook.Description = updateBookRequest.Description;
            existingBook.IsRead = updateBookRequest.IsRead;
            existingBook.DateRead = updateBookRequest.IsRead ? updateBookRequest.DateRead : null;
            existingBook.Rate = updateBookRequest.IsRead ? updateBookRequest.Rate : null;
            existingBook.Genre = updateBookRequest.Genre;
            existingBook.CoverUrl = updateBookRequest.CoverUrl;
            existingBook.PublisherId = updateBookRequest.PublisherId;

            // Cập nhật tác giả (xóa cũ, thêm mới)
            var existingAuthors = existingBook.Book_Authors.ToList();
            _dbContext.Book_Authors.RemoveRange(existingAuthors);

            if (updateBookRequest.AuthorIds != null && updateBookRequest.AuthorIds.Any())
            {
                foreach (var authorId in updateBookRequest.AuthorIds)
                {
                    _dbContext.Book_Authors.Add(new Book_Author
                    {
                        BookId = existingBook.BookId, // ĐÃ SỬA LỖI CS1061
                        AuthorId = authorId
                    });
                }
            }

            _dbContext.SaveChanges();
            return existingBook;
        }

        // =========================================================================
        // DELETE (ĐÃ KHẮC PHỤC LỖI CS1061)
        // =========================================================================
        public Book? DeleteBookById(int id)
        {
            var bookDomain = _dbContext.Books.FirstOrDefault(b => b.BookId == id); // ĐÃ SỬA LỖI CS1061

            if (bookDomain == null) return null;

            // Xóa liên kết Book_Author trước (dùng id)
            var bookAuthors = _dbContext.Book_Authors.Where(ba => ba.BookId == id).ToList();
            _dbContext.Book_Authors.RemoveRange(bookAuthors);

            // Xóa sách
            _dbContext.Books.Remove(bookDomain);
            _dbContext.SaveChanges();

            return bookDomain;
        }
    }
}