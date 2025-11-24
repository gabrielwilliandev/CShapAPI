using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DasboardController : ControllerBase
    {
        private readonly ITransacaoService _transacaoService;

        public DasboardController(ITransacaoService transacaoService)
        {
            _transacaoService = transacaoService;
        }

        private int GetUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null) throw new UnauthorizedAccessException("Token inválido.");
            return int.Parse(idClaim.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard([FromQuery] DateTime? data)
        {
            var userId = GetUserId();

            var dataReferencia = data ?? DateTime.Now;

            var dashboardData = await _transacaoService.GetDashboardAsync(userId, dataReferencia);

            return Ok(dashboardData);
        }
    }
}
