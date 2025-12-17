using API.DTOs;
using API.Models;
using API.Services;
using BCrypt.Net;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwt;

    public AuthService(IUserService userService, IJwtService jwt)
    {
        _userService = userService;
        _jwt = jwt;
    }

    public async Task<string> Login(LoginDto dto)
    {
        var user = await _userService.GetByEmailAsync(dto.Email);

        if (user == null)
            throw new Exception("Email inválido.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Senha inválida.");

        return _jwt.GenerateToken(user);
    }

    public async Task<UserResponseDto> Register(RegisterDto dto)
    {
        var existing = await _userService.GetByEmailAsync(dto.Email);

        if (existing != null)
            throw new Exception("Email já cadastrado.");

        var createdUser = await _userService.CreateAsync(dto);

        return new UserResponseDto
        {
            Id = createdUser.Id,
            Username = createdUser.Username!,
            Email = createdUser.Email!
        };
    }
}
