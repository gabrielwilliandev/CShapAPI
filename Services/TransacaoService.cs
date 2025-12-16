
using API.DTOs;
using API.Models;
using Google.Cloud.Firestore;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Query = Google.Cloud.Firestore.Query;

namespace API.Services
{
    public class TransacaoService : ITransacaoService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "Transactions";
        public TransacaoService(FirestoreDb firestoreDb) 
        {
            _firestoreDb = firestoreDb;
        }

        public async Task<TransacaoResponseDto> CreateAsync(TransacaoCreateDto dto, string userId)
        {

            var collection = _firestoreDb.Collection(CollectionName);

            var transacao = new Dictionary<string, object>
            {
                { "Description", dto.Description },
                { "Amount", (double)dto.Amount }, // Cast para double (Firestore number)
                { "Date", Timestamp.FromDateTime(dto.Date.ToUniversalTime()) },
                { "Type", dto.Type },
                { "CategoryId", dto.CategoryId },
                { "UserId", userId }
            };

            DocumentReference docRef = await collection.AddAsync(transacao);

            return await GetByIdAsync(docRef.Id, userId)
                   ?? throw new Exception("Erro inexperado ao criar transação.");

        }

        public async Task<bool> DeleteAsync(string id, string userId)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot transacaoRef = await docRef.GetSnapshotAsync();

            if(!transacaoRef.Exists || transacaoRef.GetValue<string>("UserId") != userId)
            {
                return false;
            }
            await docRef.DeleteAsync();
            return true;
        }

        public async Task<List<TransacaoResponseDto>> GetAllAsync(string userId, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            // 1. Referência da coleção
            Query query = _firestoreDb.Collection(CollectionName).WhereEqualTo("UserId", userId);

            // 2. Filtros de Data (Garantindo UTC para o Firestore)
            if (dataInicio.HasValue)
            {
                DateTime inicioUtc = DateTime.SpecifyKind(dataInicio.Value, DateTimeKind.Utc);
                query = query.WhereGreaterThanOrEqualTo("Date", Timestamp.FromDateTime(inicioUtc));
            }

            if (dataFim.HasValue)
            {
                DateTime fimUtc = DateTime.SpecifyKind(dataFim.Value, DateTimeKind.Utc);
                query = query.WhereLessThanOrEqualTo("Date", Timestamp.FromDateTime(fimUtc));
            }

            // 3. Ordenação (Requer índice composto se houver filtros de data)
            query = query.OrderByDescending("Date");

            // 4. Execução da busca
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            var transacoes = new List<TransacaoResponseDto>();

            foreach (DocumentSnapshot document in querySnapshot.Documents)
            {
                if (document.Exists)
                {
                    transacoes.Add(new TransacaoResponseDto
                    {
                        Id = document.Id,
                        Description = document.GetValue<string>("Description"),
                        // O Firestore armazena como double, convertemos para decimal aqui
                        Amount = document.ContainsField("Amount") ? Convert.ToDecimal(document.GetValue<double>("Amount")) : 0m,
                        Date = document.GetValue<Timestamp>("Date").ToDateTime(),
                        Type = document.GetValue<string>("Type"),
                        CategoryId = document.GetValue<string>("CategoryId"),
                        UserId = document.GetValue<string>("UserId")
                    });
                }
            }

            return transacoes;
        }

        public async Task<TransacaoResponseDto?> GetByIdAsync(string id, string userId)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists || snapshot.GetValue<string>("UserId") != userId)
            {
                return null;
            }

            return await MapToResponseDto(snapshot);
            
        }

        public async Task<TransacaoResponseDto?> UpdateAsync(string id, TransacaoUpdateDto dto, string userId)
        {
            DocumentReference docRef = _firestoreDb.Collection(CollectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists || snapshot.GetValue<string>("UserId") != userId)
            {
                return null;
            }
            var updates = new Dictionary<string, object>
            {
                { "Description", dto.Description },
                { "Amount", (double)dto.Amount }, // Cast para double (Firestore number)
                { "Date", Timestamp.FromDateTime(dto.Date.ToUniversalTime()) },
                { "Type", dto.Type },
                { "CategoryId", dto.CategoryId }
            };
            await docRef.UpdateAsync(updates);
            return await GetByIdAsync(id, userId);


        }

        public async Task<DashboardResponseDto> GetDashboardAsync(string userId, DateTime dataReferencia)
        {
            var inicioMes = new DateTime(dataReferencia.Year, dataReferencia.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var fimMes = inicioMes.AddMonths(1).AddTicks(-1);

            var query = _firestoreDb.Collection(CollectionName)
                .WhereEqualTo("UserId", userId)
                .WhereGreaterThanOrEqualTo("Date", Timestamp.FromDateTime(inicioMes))
                .WhereLessThanOrEqualTo("Date", Timestamp.FromDateTime(fimMes));

            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            // Simulação de agregação em memória (Firestore não tem Sum/GroupBy nativo rico como SQL)
            var transacoes = snapshot.Documents.Select(d => new {
                Amount = d.GetValue<decimal>("Amount"),
                Type = d.GetValue<string>("Type"),
                CategoryName = "Carregando..." // Idealmente salvo no documento da transação
            }).ToList();

            var totalReceitas = transacoes.Where(t => t.Type == "Entrada").Sum(t => t.Amount);
            var totalDespesas = transacoes.Where(t => t.Type == "Saída").Sum(t => t.Amount);

            return new DashboardResponseDto
            {
                TotalReceitas = totalReceitas,
                TotalDespesas = totalDespesas,
                Saldo = totalReceitas - totalDespesas
                // GastosPorCategoria exigiria buscar nomes das categorias ou desnormalizar
            };
        }
        public async Task<List<TransacaoResponseDto>> GetAllGlobalAsync()
        {
            // 1. Busca todas as transações ordenadas por data
            Query query = _firestoreDb.Collection(CollectionName).OrderByDescending("Date");
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            var transacoes = new List<TransacaoResponseDto>();

            // Caches simples para evitar buscar o mesmo Usuário ou Categoria várias vezes no loop
            var categoryCache = new Dictionary<string, string>();
            var userCache = new Dictionary<string, string>();

            foreach (DocumentSnapshot document in querySnapshot.Documents)
            {
                var categoryId = document.GetValue<string>("CategoryId");
                var userId = document.GetValue<string>("UserId");

                // 2. Buscar Nome da Categoria (se não estiver no cache)
                if (!string.IsNullOrEmpty(categoryId) && !categoryCache.ContainsKey(categoryId))
                {
                    var catSnap = await _firestoreDb.Collection("Categories").Document(categoryId).GetSnapshotAsync();
                    categoryCache[categoryId] = catSnap.Exists ? catSnap.GetValue<string>("Name") : "Sem Categoria";
                }

                // 3. Buscar Nome do Usuário (se não estiver no cache)
                if (!string.IsNullOrEmpty(userId) && !userCache.ContainsKey(userId))
                {
                    var userSnap = await _firestoreDb.Collection("Users").Document(userId).GetSnapshotAsync();
                    userCache[userId] = userSnap.Exists ? userSnap.GetValue<string>("Username") : "Usuário Desconhecido";
                }

                transacoes.Add(new TransacaoResponseDto
                {
                    Id = document.Id,
                    Description = document.GetValue<string>("Description"),
                    Amount = Convert.ToDecimal(document.GetValue<double>("Amount")),
                    Date = document.GetValue<Timestamp>("Date").ToDateTime(),
                    Type = document.GetValue<string>("Type"),
                    CategoryId = categoryId,
                    CategoryName = categoryCache.GetValueOrDefault(categoryId),
                    UserId = userId,
                    UserName = userCache.GetValueOrDefault(userId)
                });
            }

            return transacoes;
        }

        private async Task<TransacaoResponseDto> MapToResponseDto(DocumentSnapshot doc)
        {
            var categoryId = doc.GetValue<string>("CategoryId");
            // Nota: Em produção, o ideal é que o 'CategoryName' já esteja salvo na Transação 
            // para evitar múltiplas chamadas ao banco (Desnormalização).

            return new TransacaoResponseDto
            {
                Id = doc.Id,
                Description = doc.GetValue<string>("Description"),
                Amount = doc.GetValue<decimal>("Amount"),
                Date = doc.GetValue<Timestamp>("Date").ToDateTime(),
                Type = doc.GetValue<string>("Type"),
                CategoryId = categoryId,
                UserId = doc.GetValue<string>("UserId")
            };
        }
    }
}

