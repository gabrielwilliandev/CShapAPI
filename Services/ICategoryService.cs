using API.DTOs;
using API.Models;

namespace API.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryResponseDto>> GetAllAsync();
        Task<CategoryResponseDto?> GetByIdAsync(string id);
        Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto);
        Task<CategoryResponseDto?> UpdateAsync(string id, CategoryUpdateDto dto);
        Task<bool> DeleteAsync(string id);

    }
}
