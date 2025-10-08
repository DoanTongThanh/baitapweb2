// File: SQLAuthRepository.cs
using baitapweb2.Models.DTO;
using Microsoft.AspNetCore.Identity;

namespace baitapweb2.Repositories
{
    public class SQLAuthRepository : IAuthRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenRepository _tokenRepository;

        public SQLAuthRepository(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            _userManager = userManager;
            _tokenRepository = tokenRepository;
        }

        public async Task<List<string>> Register(RegisterRequestDTO registerRequest)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequest.Username,
                Email = registerRequest.Username
            };

            var identityResult = await _userManager.CreateAsync(identityUser, registerRequest.Password);

            if (identityResult.Succeeded)
            {
                // Gán vai trò (role) 'User' cho người dùng mới
                if (registerRequest.Roles != null && registerRequest.Roles.Any())
                {
                    identityResult = await _userManager.AddToRolesAsync(identityUser, registerRequest.Roles);
                }

                // Nếu gán vai trò thành công hoặc không có vai trò nào được yêu cầu, trả về danh sách lỗi trống (thành công)
                if (identityResult.Succeeded)
                {
                    return new List<string>();
                }
            }

            // Nếu có lỗi, trả về danh sách lỗi
            return identityResult.Errors.Select(e => e.Description).ToList();
        }

        public async Task<string> Login(LoginRequestDTO loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.Username);

            if (user != null)
            {
                var checkPasswordResult = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

                if (checkPasswordResult)
                {
                    // Lấy vai trò của người dùng
                    var roles = await _userManager.GetRolesAsync(user);

                    // Tạo JWT Token
                    var jwtToken = _tokenRepository.CreateJWTToken(user, roles.ToList());
                    return jwtToken;
                }
            }

            // Trả về chuỗi rỗng nếu đăng nhập thất bại
            return string.Empty;
        }
    }
}