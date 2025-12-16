using API.Models;

namespace API.DTOs
{
    public class TransacaoCreateDto
    {
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Type { get; set; }
        public string CategoryId { get; set; }
        
    }
}
