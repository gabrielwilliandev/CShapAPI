using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace API.Services
{
    public class TransacaoService : ITransacaoService
    {
        private readonly AppDbContext _context;
        public TransacaoService(AppDbContext context) 
        {
            _context = context;
        }

        public async Task<TransacaoResponseDto> CreateAsync(TransacaoCreateDto dto, int userId)
        {
            var transacao = new Transacao 
            {
                Description = dto.Description,
                Amount = dto.Amount,
                Date = dto.Date,
                Type = dto.Type,
                CategoryId = dto.CategoryId,
                UserId = userId
            };

            _context.Transactions.Add(transacao);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(transacao.Id, userId)
                   ?? throw new Exception("Erro inexperado ao criar transação.");
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var transacao = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (transacao != null)
            {
                _context.Transactions.Remove(transacao);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<TransacaoResponseDto>> GetAllAsync(int userId, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            var query = _context.Transactions
                                              .Include(t => t.Category)
                                              .Include(t => t.User)
                                              .Where(t => t.UserId == userId);

                                              if (dataInicio.HasValue)
                                                {
                                                    query = query.Where(t => t.Date >= dataInicio.Value);
                                                }

                                                // Se informou data de fim, filtra
                                                if (dataFim.HasValue)
                                                {
                                                    query = query.Where(t => t.Date <= dataFim.Value);
                                                }

                                                // Ordena por data (mais recente primeiro é comum em finanças)
                                                query = query.OrderByDescending(t => t.Date);

                                                return await query
                                                      .Select(t => new TransacaoResponseDto
                                              {
                                                  Id = t.Id,
                                                  Description = t.Description,
                                                  Amount = t.Amount,
                                                  Date = t.Date,
                                                  Type = t.Type,
                                                  CategoryId = t.CategoryId,
                                                  CategoryName = t.Category.Name,
                                                  UserId = t.UserId,
                                                  UserName = t.User.Username
                                              }).ToListAsync();
        }

        public async Task<TransacaoResponseDto?> GetByIdAsync(int id, int userId)
        {
            var transacao = await _context.Transactions
                                          .Include(t => t.Category)
                                          .Include(t => t.User)
                                          .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transacao == null) return null;

            return new TransacaoResponseDto
            {
                Id = transacao.Id,
                Description = transacao.Description,
                Amount = transacao.Amount,
                Date = transacao.Date,
                Type = transacao.Type,
                CategoryId = transacao.CategoryId,
                CategoryName = transacao.Category.Name,
                UserId = transacao.UserId,
                UserName = transacao.User.Username
            };
        }

        public async Task<TransacaoResponseDto?> UpdateAsync(int id, TransacaoUpdateDto dto, int userId)
        {
            // CORREÇÃO: Busca a transação e *obrigatoriamente* verifica se pertence ao userId
            var transacao = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transacao == null)
            {
                // Retorna null se não for encontrado OU se a transação não for do usuário logado
                return null;
            }

            transacao.Description = dto.Description;
            transacao.Amount = dto.Amount;
            transacao.Date = dto.Date;
            transacao.Type = dto.Type;
            transacao.CategoryId = dto.CategoryId;
            // Não altere transacao.UserId!

            // O contexto rastreia a mudança, não é preciso o _context.Transactions.Update(transacao);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(id, userId);
        }

        public async Task<DashboardResponseDto> GetDashboardAsync(int userId, DateTime dataReferencia)
        {
            var inicioMes = new DateTime(dataReferencia.Year, dataReferencia.Month, 1);

            var fimMes = inicioMes.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

            var transacoes = await _context.Transactions
                                            .Include(t => t.Category)
                                            .Where(t => t.UserId == userId
                                            && t.Date >= inicioMes
                                            && t.Date <= fimMes).ToListAsync();

            var totalReceitas = transacoes.Where(t => t.Type == "Entrada").Sum(t => t.Amount);
            var totalDespesas = transacoes.Where(t => t.Type == "Saída").Sum(t => t.Amount);
            var saldo = totalReceitas - totalDespesas;

            var categoriaStats = transacoes
                                 .Where(t => t.Type == "Saída")
                                 .GroupBy(t => t.Category.Name)
                                 .Select(g => new CategoryStatDto
                                 {
                                     CategoryName = g.Key,
                                     TotalAmount = g.Sum(t => t.Amount),
                                     Percentage = totalDespesas > 0 ? (double)(g.Sum(t => t.Amount) / totalDespesas) * 100 : 0
                                 })
                                 .OrderByDescending(x => x.TotalAmount)
                                 .ToList();
            return new DashboardResponseDto
            {
                TotalReceitas = totalReceitas,
                TotalDespesas = totalDespesas,
                Saldo = saldo,
                GastosPorCategoria = categoriaStats
            };
        }
            public async Task<List<TransacaoResponseDto>> GetAllGlobalAsync()
            {
            return await _context.Transactions
                        .Include(t => t.Category)
                        .Include(t => t.User) // Importante para saber de quem é
                        .OrderByDescending(t => t.Date)
                        .Select(t => new TransacaoResponseDto
                        {
                            Id = t.Id,
                            Description = t.Description,
                            Amount = t.Amount,
                            Date = t.Date,
                            Type = t.Type,
                            CategoryId = t.CategoryId,
                            CategoryName = t.Category.Name,
                            UserId = t.UserId,
                            UserName = t.User.Username // Mostra o nome do usuário
                        }).ToListAsync();
            }
        }
    }

