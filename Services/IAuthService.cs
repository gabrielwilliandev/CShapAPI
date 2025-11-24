    using API.DTOs;

    namespace API.Services
    {
        public interface IAuthService
        {
            Task<UserResponseDto> Register(RegisterDto dto);
            Task<string> Login(LoginDto dto);
        }
    }
