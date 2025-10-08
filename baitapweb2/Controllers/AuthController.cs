// File: Controllers/AuthController.cs (Để hoàn thiện tính năng Auth)
using baitapweb2.Models.DTO;
using baitapweb2.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace baitapweb2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequestDto)
        {
            var errors = await _authRepository.Register(registerRequestDto);

            if (errors.Any())
            {
                return BadRequest(errors);
            }

            return Ok("User was registered successfully! Please login.");
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDto)
        {
            var jwtToken = await _authRepository.Login(loginRequestDto);

            if (string.IsNullOrEmpty(jwtToken))
            {
                return Unauthorized("Username or password incorrect.");
            }

            return Ok(jwtToken);
        }
    }
}