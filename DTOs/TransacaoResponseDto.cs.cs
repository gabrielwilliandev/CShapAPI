namespace API.DTOs
{
    public class TransacaoResponseDto
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Type { get; set; }

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public int UserId { get; set; }
        public string? UserName { get; set; }
    }
}
