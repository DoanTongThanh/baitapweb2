// File: Controllers/ImagesController.cs
using Microsoft.AspNetCore.Mvc;
using baitapweb2.Models.Domain;
using baitapweb2.Repositories;
using baitapweb2.Models.DTO; // Giả sử ImageUploadRequestDTO nằm ở đây
using System.IO;

namespace baitapweb2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;
        // Có thể thêm ILogger<ImagesController> ở đây nếu bạn muốn ghi log

        public ImagesController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        // =========================================================================
        // UPLOAD FILE (POST)
        // =========================================================================
        [HttpPost]
        [Route("Upload")]
        public IActionResult Upload([FromForm] ImageUploadRequestDTO request)
        {
            if (ValidateFileUpload(request))
            {
                var imageDomainModel = new Image
                {
                    File = request.File,
                    FileExtension = Path.GetExtension(request.File.FileName),
                    FileSizeInBytes = request.File.Length,
                    FileName = request.FileName,
                    FileDescription = request.FileDescription
                };

                _imageRepository.Upload(imageDomainModel);
                return Ok(imageDomainModel);
            }
            return BadRequest(ModelState);
        }

        // =========================================================================
        // GET ALL INFO IMAGES (GET)
        // =========================================================================
        [HttpGet]
        [Route("GetAllInfoImages")]
        public IActionResult GetAllInfoImages()
        {
            var allImages = _imageRepository.GetAllInfoImages();
            return Ok(allImages);
        }

        // =========================================================================
        // DOWNLOAD FILE (GET)
        // =========================================================================
        [HttpGet]
        [Route("Download/{id:int}")]
        public IActionResult DownloadImage([FromRoute] int id)
        {
            var result = _imageRepository.DownloadFile(id);

            var stream = result.Item1;
            var fileExtension = result.Item2;
            var fileName = result.Item3;

            if (stream.Length == 0)
            {
                return NotFound();
            }

            var contentType = GetContentType(fileExtension);

            return File(stream, contentType, fileName + fileExtension);
        }

        // =========================================================================
        // PRIVATE VALIDATION
        // =========================================================================
        private bool ValidateFileUpload(ImageUploadRequestDTO request)
        {
            var allowExtensions = new string[] { ".jpg", ".jpeg", ".png" };

            if (!allowExtensions.Contains(Path.GetExtension(request.File.FileName).ToLower()))
            {
                ModelState.AddModelError("file", "Unsupported file extension (Chỉ chấp nhận .jpg, .jpeg, .png)");
            }

            // Giới hạn 10MB
            if (request.File.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size too big, file phải < 10MB");
            }

            return ModelState.IsValid;
        }

        // Hàm hỗ trợ Content Type
        private string GetContentType(string fileExtension)
        {
            return fileExtension.ToLower() switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream",
            };
        }
    }
}