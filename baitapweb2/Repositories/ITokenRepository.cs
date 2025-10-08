// File: ITokenRepository.cs
using Microsoft.AspNetCore.Identity;

namespace baitapweb2.Repositories
{
    public interface ITokenRepository
    {
        // Tạo JWT Token dựa trên thông tin người dùng
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}