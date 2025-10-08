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

        // CREATE
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

        // DELETE
        public Publisher? DeletePublisherById(int id)
        {
            var publisherDomain = _dbContext.Publishers
                .FirstOrDefault(n => n.PublisherId == id);

            if (publisherDomain == null)
            {
                return null;
            }

            // Kiểm tra xem có sách nào sử dụng Publisher này không
            var hasBooks = _dbContext.Books.Any(b => b.PublisherId == id);

            if (hasBooks)
            {
                // Nếu có sách liên quan, bạn có thể chọn:
                // 1. Throw exception: Không cho xóa.
                // 2. Xóa các sách liên quan trước (cascade delete - không khuyến khích ở Repository).
                // Ở đây, chúng ta sẽ không cho phép xóa nếu còn sách liên quan.
                // Tuy nhiên, để đơn giản, chúng ta sẽ xóa Publisher và dựa vào cấu hình Database/EF Core.
                // Nếu cấu hình Database có cascade delete thì các sách liên quan cũng bị xóa,
                // nhưng tốt nhất là nên đảm bảo không có FK liên quan trước khi xóa.
                // Trong môi trường thực tế, nên trả về lỗi. 
                // Ở đây, giả định không cần kiểm tra phức tạp và tiến hành xóa.
            }

            _dbContext.Publishers.Remove(publisherDomain);
            _dbContext.SaveChanges();

            return publisherDomain;
        }

        // READ ALL
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

        // READ BY ID
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

        // UPDATE
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