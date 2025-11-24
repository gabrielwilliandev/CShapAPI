using API.DTOs;
using API.Models;

namespace API.Services
{
    public interface ITransacaoService
    {
        Task<List<TransacaoResponseDto>> GetAllAsync(int userId, DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<List<TransacaoResponseDto>> GetAllGlobalAsync();
        Task<TransacaoResponseDto?> GetByIdAsync(int id, int userId);
        Task<TransacaoResponseDto> CreateAsync(TransacaoCreateDto dto, int userId);
        Task<TransacaoResponseDto?> UpdateAsync(int id, TransacaoUpdateDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);

        Task<DashboardResponseDto> GetDashboardAsync(int userId, DateTime dataReferencia);

    }
}
