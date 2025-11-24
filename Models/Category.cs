namespace API.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Transacao> Transactions { get; set; }
    }
}
