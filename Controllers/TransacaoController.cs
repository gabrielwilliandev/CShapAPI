using API.DTOs;
using API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers 
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class TransacaoController : ControllerBase
    {
        private readonly ITransacaoService _transacaoService;
        private readonly IValidator<TransacaoCreateDto> _createValidator;
        private readonly IValidator<TransacaoUpdateDto> _updateValidator;

        public TransacaoController(ITransacaoService transacaoService, IValidator<TransacaoCreateDto> createValidator,
            IValidator<TransacaoUpdateDto> updateValidator)
        {
            _transacaoService = transacaoService;
            _createValidator = createValidator; 
            _updateValidator = updateValidator; 
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new Exception("Token inválido.");
            }
            return int.Parse(userIdClaim.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim)
        {
            var userId = GetUserId();

            // Passa as datas para o serviço
            var transacoes = await _transacaoService.GetAllAsync(userId, inicio, fim);

            return Ok(transacoes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            int userId = GetUserId();
            var transacao = await _transacaoService.GetByIdAsync(id, userId);

            if (transacao == null) return NotFound(new { Message = "Transação não encontrada." });

            return Ok(transacao);
        }
        [HttpPost]
        public async Task<IActionResult> Create(TransacaoCreateDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var userId = GetUserId();
            var transacao = await _transacaoService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = transacao.Id }, transacao);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TransacaoUpdateDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            var userId = GetUserId();
            var update = await _transacaoService.UpdateAsync(id, dto, userId);

            if (update == null) return NotFound(new { Message = "Transação não encontrada." });

            return Ok(update);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var deleted = await _transacaoService.DeleteAsync(id, userId);
            if (!deleted) return NotFound(new { Message = "Transação não encontrada." });
            return NoContent();
        }
        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllGlobal()
        {
            // Em um cenário real, aqui você verificaria: if (User.Role != "Admin") return Forbid();
            var transacoes = await _transacaoService.GetAllGlobalAsync();
            return Ok(transacoes);
        }
    }
    }
