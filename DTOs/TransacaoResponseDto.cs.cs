namespace API.DTOs
{
    public class TransacaoResponseDto
    {
        public string Id { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Type { get; set; }

        public string CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public string UserId { get; set; }
        public string? UserName { get; set; }
    }
}
