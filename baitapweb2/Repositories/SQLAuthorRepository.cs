using baitapweb2.Data;
using baitapweb2.Models.Domain;
using baitapweb2.Models.DTO;
using System.Collections.Generic;
using System.Linq;

namespace baitapweb2.Repositories
{
    public class SQLAuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLAuthorRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // CREATE: Thêm Author mới
        public Author AddAuthor(AuthorNoIDDTO addAuthorRequest)
        {
            // Ánh xạ DTO sang Domain Model
            var authorDomain = new Author()
            {
                FullName = addAuthorRequest.FullName
            };

            _dbContext.Authors.Add(authorDomain);
            _dbContext.SaveChanges();

            return authorDomain;
        }

        // DELETE: Xóa Author
        public Author? DeleteAuthorById(int id)
        {
            var authorDomain = _dbContext.Authors.FirstOrDefault(n => n.AuthorId == id);

            if (authorDomain == null)
            {
                return null;
            }

            // Xóa Author
            _dbContext.Authors.Remove(authorDomain);

            // Xóa các mối quan hệ Book_Author liên quan
            var bookAuthorsDomain = _dbContext.Book_Authors.Where(a => a.AuthorId == id).ToList();
            if (bookAuthorsDomain.Any())
            {
                _dbContext.Book_Authors.RemoveRange(bookAuthorsDomain);
            }

            _dbContext.SaveChanges();

            return authorDomain;
        }

        // READ: Lấy tất cả Authors
        public List<AuthorDTO> GetAllAuthors()
        {
            // Ánh xạ Domain Model sang DTO
            var allAuthorsDTO = _dbContext.Authors.Select(author => new AuthorDTO()
            {
                Id = author.AuthorId,
                FullName = author.FullName
            }).ToList();

            return allAuthorsDTO;
        }

        // READ: Lấy Author theo ID
        public AuthorDTO GetAuthorById(int id)
        {
            var authorDomain = _dbContext.Authors.FirstOrDefault(n => n.AuthorId == id);

            if (authorDomain == null)
            {
                return null;
            }

            // Ánh xạ Domain Model sang DTO
            var authorDTO = new AuthorDTO()
            {
                Id = authorDomain.AuthorId,
                FullName = authorDomain.FullName
            };

            return authorDTO;
        }

        // UPDATE: Cập nhật Author
        public Author UpdateAuthorById(int id, AuthorNoIDDTO authorNoIdDto)
        {
            var authorDomain = _dbContext.Authors.FirstOrDefault(n => n.AuthorId == id);

            if (authorDomain == null)
            {
                return null;
            }

            // Cập nhật thuộc tính
            authorDomain.FullName = authorNoIdDto.FullName;

            _dbContext.SaveChanges();

            return authorDomain;
        }
    }
}