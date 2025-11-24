namespace API.Models
{
    public class Transacao
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Type { get; set; }


        //Category
        public int CategoryId { get; set; }
        public Category? Category { get; set; }


        //User
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
