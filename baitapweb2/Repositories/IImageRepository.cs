// File: Repositories/IImageRepository.cs
using baitapweb2.Models.Domain;

namespace baitapweb2.Repositories
{
    public interface IImageRepository
    {
        // Trả về Image Domain Model sau khi upload
        Image Upload(Image image);

        // Lấy thông tin tất cả hình ảnh từ database
        List<Image> GetAllInfoImages();

        // Download file: trả về (Byte[], Content Type, File Name)
        (byte[], string, string) DownloadFile(int id);
    }
}