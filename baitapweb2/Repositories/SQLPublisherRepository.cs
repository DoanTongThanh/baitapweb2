using baitapweb2.Data;
using baitapweb2.Models.Domain;
using baitapweb2.Models.DTO;
using System.Collections.Generic;
using System.Linq;

namespace baitapweb2.Repositories
{
    public class SQLPublisherRepository : IPublisherRepository
    {
        private readonly AppDbContext _dbContext;

        public SQLPublisherRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // =========================================================================
        // CREATE
        // =========================================================================
        public Publisher AddPublisher(PublisherNoIDDTO addPublisherRequest)
        {
            // Ánh xạ DTO sang Domain Model
            var publisherDomain = new Publisher()
            {
                Name = addPublisherRequest.Name
            };

            _dbContext.Publishers.Add(publisherDomain);
            _dbContext.SaveChanges();

            return publisherDomain;
        }

        // =========================================================================
        // DELETE (Sửa lại để tuân thủ Bài tập 7: Phải báo lỗi nếu còn sách)
        // =========================================================================
        public Publisher? DeletePublisherById(int id)
        {
            // 1. Tìm Publisher Domain
            var publisherDomain = _dbContext.Publishers
                .FirstOrDefault(n => n.PublisherId == id);

            if (publisherDomain == null)
            {
                // Không tìm thấy Publisher, trả về null (Controller sẽ trả 404)
                return null;
            }

            // 2. KIỂM TRA NGHIỆP VỤ (Bài tập 7): Kiểm tra xem có sách nào sử dụng Publisher này không
            var hasBooks = _dbContext.Books.Any(b => b.PublisherId == id);

            if (hasBooks)
            {
                // Nếu có sách liên quan, KHÔNG ĐƯỢC XÓA và trả về null.
                // Điều này cho phép Controller biết rằng hành động xóa không thành công.
                return null;
            }

            // 3. Thực hiện Xóa
            _dbContext.Publishers.Remove(publisherDomain);
            _dbContext.SaveChanges();

            return publisherDomain;
        }

        // =========================================================================
        // READ ALL
        // =========================================================================
        public List<PublisherDTO> GetAllPublishers()
        {
            // Ánh xạ Domain Model sang DTO
            var allPublishersDTO = _dbContext.Publishers.Select(publisher => new PublisherDTO()
            {
                Id = publisher.PublisherId,
                Name = publisher.Name
            }).ToList();

            return allPublishersDTO;
        }

        // =========================================================================
        // READ BY ID
        // =========================================================================
        public PublisherDTO GetPublisherById(int id)
        {
            var publisherDomain = _dbContext.Publishers.FirstOrDefault(n => n.PublisherId == id);

            if (publisherDomain == null)
            {
                return null;
            }

            // Ánh xạ Domain Model sang DTO
            var publisherDTO = new PublisherDTO()
            {
                Id = publisherDomain.PublisherId,
                Name = publisherDomain.Name
            };

            return publisherDTO;
        }

        // =========================================================================
        // UPDATE
        // =========================================================================
        public Publisher UpdatePublisherById(int id, PublisherNoIDDTO publisherNoIdDto)
        {
            var publisherDomain = _dbContext.Publishers.FirstOrDefault(n => n.PublisherId == id);

            if (publisherDomain == null)
            {
                return null;
            }

            // Cập nhật thuộc tính
            publisherDomain.Name = publisherNoIdDto.Name;

            _dbContext.SaveChanges();

            return publisherDomain;
        }
    }
}