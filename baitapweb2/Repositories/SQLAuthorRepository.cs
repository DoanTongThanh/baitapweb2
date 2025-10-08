using baitapweb2.Data;
using baitapweb2.Models.Domain;
using baitapweb2.Models.DTO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore; // Cần dùng Include/FirstOrDefault, nên đảm bảo đã có

namespace baitapweb2.Repositories
{
    public class SQLAuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLAuthorRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // =========================================================================
        // CREATE: Thêm Author mới
        // =========================================================================
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

        // =========================================================================
        // DELETE: Xóa Author (Sửa lại để tuân thủ Bài tập 15: Phải báo lỗi nếu còn sách)
        // =========================================================================
        public Author? DeleteAuthorById(int id)
        {
            // 1. Tìm Author Domain
            var authorDomain = _dbContext.Authors.FirstOrDefault(n => n.AuthorId == id);

            if (authorDomain == null)
            {
                // Không tìm thấy Author, trả về null (Controller sẽ trả 404)
                return null;
            }

            // 2. KIỂM TRA NGHIỆP VỤ (Bài tập 15): Kiểm tra xem Author có sách liên quan không
            // Chúng ta kiểm tra sự tồn tại của bất kỳ bản ghi nào trong bảng Book_Author
            // có chứa AuthorId này.
            var hasRelatedBooks = _dbContext.Book_Authors.Any(ba => ba.AuthorId == id);

            if (hasRelatedBooks)
            {
                // Nếu có sách liên quan, KHÔNG ĐƯỢC XÓA và trả về null.
                // (Controller sẽ trả 404, bạn có thể cân nhắc trả về một mã lỗi khác cho 400 sau này)
                // Theo yêu cầu đơn giản, nếu không thể xóa thành công, ta trả về null.
                return null;
            }

            // 3. Thực hiện Xóa
            _dbContext.Authors.Remove(authorDomain);
            _dbContext.SaveChanges();

            // Trả về đối tượng đã xóa
            return authorDomain;
        }

        // =========================================================================
        // READ: Lấy tất cả Authors
        // =========================================================================
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

        // =========================================================================
        // READ: Lấy Author theo ID
        // =========================================================================
        public AuthorDTO GetAuthorById(int id)
        {
            var authorDomain = _dbContext.Authors.FirstOrDefault(n => n.AuthorId == id);

            if (authorDomain == null)
            {
                // Thay đổi: Trả về null thay vì DTO null nếu không tìm thấy.
                // (Controller sẽ xử lý NotFound)
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

        // =========================================================================
        // UPDATE: Cập nhật Author
        // =========================================================================
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