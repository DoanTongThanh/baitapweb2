// File: Library-web/Controllers/BooksController.cs
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Library_web.Models.DTO;
using System.Net.Http.Json; // Cần dùng cho ReadFromJsonAsync và PostAsJsonAsync

namespace Library_web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BooksController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // =================================================================
        // 1. Index (GET) - Lấy và Hiển thị Danh sách Sách (Có Filter/Sort)
        // =================================================================
        public async Task<IActionResult> Index([FromQuery] string filterOn, string filterQuery, bool isAscending = true)
        {
            var response = new List<BookDTO>();
            try
            {
                var client = _httpClientFactory.CreateClient();
                // Sử dụng Filter/Sort từ API
                var apiUrl = $"https://localhost:7245/api/Books/get-all-books?filterOn={filterOn}&filterQuery={filterQuery}&isAscending={isAscending}";

                var httpResponseMessage = await client.GetAsync(apiUrl);
                httpResponseMessage.EnsureSuccessStatusCode();

                // Đọc dữ liệu JSON và ánh xạ sang List<BookDTO>
                response = await httpResponseMessage.Content.ReadFromJsonAsync<List<BookDTO>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
            // Truyền dữ liệu cho View
            return View(response);
        }

        // =================================================================
        // 2. listBookById (GET) - Lấy và Hiển thị Chi tiết Sách
        // =================================================================
        [HttpGet]
        public async Task<IActionResult> listBookById(int id)
        {
            var bookDTO = new BookDTO();
            try
            {
                var client = _httpClientFactory.CreateClient();
                // Gọi API để lấy chi tiết sách bằng ID
                var httpResponseMessage = await client.GetAsync($"https://localhost:7245/api/Books/get-book-by-id/{id}");
                httpResponseMessage.EnsureSuccessStatusCode();

                // Đọc dữ liệu JSON và ánh xạ sang BookDTO
                bookDTO = await httpResponseMessage.Content.ReadFromJsonAsync<BookDTO>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
            return View(bookDTO);
        }

        // =================================================================
        // 3. addBook (GET) - Hiển thị Form Thêm Sách
        // =================================================================
        [HttpGet]
        public IActionResult addBook()
        {
            // Trong ứng dụng thực tế: cần gọi API để lấy danh sách Publisher và Author để điền vào Select List
            return View();
        }

        // =================================================================
        // 4. addBook (POST) - Gửi dữ liệu Sách mới lên API
        // =================================================================
        [HttpPost]
        public async Task<IActionResult> addBook(AddBookDTO addBookDTO)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                // Gửi DTO dưới dạng JSON Content
                var httpResponseMessage = await client.PostAsJsonAsync("https://localhost:7245/api/Books/add-book", addBookDTO);

                httpResponseMessage.EnsureSuccessStatusCode();

                // Chuyển hướng về trang Index sau khi thành công
                return RedirectToAction("Index", "Books");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(); // Trả về lại View Form nếu có lỗi
            }
        }
    }
}