// File: IAuthRepository.cs
using baitapweb2.Models.DTO;
using Microsoft.AspNetCore.Identity;

namespace baitapweb2.Repositories
{
    public interface IAuthRepository
    {
        // Trả về một List<string> (các lỗi) nếu đăng ký thất bại
        Task<List<string>> Register(RegisterRequestDTO registerRequest);

        // Trả về JWT Token nếu đăng nhập thành công
        Task<string> Login(LoginRequestDTO loginRequest);
    }
}