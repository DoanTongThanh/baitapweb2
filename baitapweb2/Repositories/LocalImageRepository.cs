// File: Repositories/LocalImageRepository.cs
using baitapweb2.Data;
using baitapweb2.Models.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http; // Cần thiết cho IHttpContextAccessor

namespace baitapweb2.Repositories
{
    public class LocalImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _dbContext;

        // Constructor nhận các service cần thiết
        public LocalImageRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        // Service Upload
        public Image Upload(Image image)
        {
            // Kết hợp đường dẫn đến thư mục Images đã tạo thủ công
            var localFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", $"{image.FileName}{image.FileExtension}");

            // Ghi file vật lý vào Server
            using (var stream = new FileStream(localFilePath, FileMode.Create))
            {
                image.File.CopyTo(stream);
            }

            // Tạo URL để lưu vào DB và trả về cho Client
            var urlFilePath = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";

            image.FilePath = urlFilePath;

            // Lưu thông tin Image vào database
            _dbContext.Images.Add(image);
            _dbContext.SaveChanges();

            return image;
        }

        // Lấy tất cả thông tin Image
        public List<Image> GetAllInfoImages()
        {
            return _dbContext.Images.ToList();
        }

        // Service Download file
        public (byte[], string, string) DownloadFile(int id)
        {
            var fileById = _dbContext.Images.Where(x => x.Id == id).FirstOrDefault();

            if (fileById == null)
            {
                return (Array.Empty<byte>(), string.Empty, string.Empty);
            }

            // Path để đọc file từ Server
            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", $"{fileById.FileName}{fileById.FileExtension}");

            // Đọc file thành mảng byte
            var stream = File.ReadAllBytes(path);

            return (stream, fileById.FileExtension, fileById.FileName);
        }
    }
}