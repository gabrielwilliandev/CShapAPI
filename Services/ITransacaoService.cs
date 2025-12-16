using API.DTOs;
using API.Models;

namespace API.Services
{
    public interface ITransacaoService
    {
        Task<List<TransacaoResponseDto>> GetAllAsync(string userId, DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<List<TransacaoResponseDto>> GetAllGlobalAsync();
        Task<TransacaoResponseDto?> GetByIdAsync(string id, string userId);
        Task<TransacaoResponseDto> CreateAsync(TransacaoCreateDto dto, string userId);
        Task<TransacaoResponseDto?> UpdateAsync(string id, TransacaoUpdateDto dto, string userId);
        Task<bool> DeleteAsync(string id, string userId);

        Task<DashboardResponseDto> GetDashboardAsync(string userId, DateTime dataReferencia);

    }
}
