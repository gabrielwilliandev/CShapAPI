using API.DTOs;
using API.Models;

namespace API.Services
{
    public interface IUserService
    {
        Task<List<UserResponseDto>> GetAllAsync();
        Task<UserResponseDto?> GetByIdAsync(string id);

        //Sobrecarga para o AuthController/Registro
        Task<User> CreateAsync(RegisterDto dto);
        //Sobrecarga para o UserController/Criação manual (que usa UserCreateDto
        Task<User> CreateAsync(UserCreateDto dto);
        Task<UserResponseDto?> UpdateAsync(string id, UserUpdateDto dto);
        Task<bool> DeleteAsync(string id);
        Task<User?> GetByEmailAsync(string email);
    }
}
