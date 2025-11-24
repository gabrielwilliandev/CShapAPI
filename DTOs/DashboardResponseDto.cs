namespace API.DTOs
{
    public class DashboardResponseDto
    {
        public decimal TotalReceitas { get; set; }
        public decimal TotalDespesas { get; set; }
        public decimal Saldo { get; set; }

        public List<CategoryStatDto> GastosPorCategoria { get; set; } = new();
    }

    public class CategoryStatDto
    {
        public string CategoryName { get; set; }
        public decimal TotalAmount { get; set; }
        public double Percentage { get; set; }
    }
}
