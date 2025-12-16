using System.Text.Json.Serialization;

namespace PortfolioAnalyticsSystem.Models
{
    public class Portfolio
    {
        // Esse Id é gerado em memória no DataContext,
        // já que o seed não traz esse campo.
        public int Id { get; set; } 
        public string Name { get; set; }
        public string UserId { get; set; }
        public decimal TotalInvestment { get; set; }

        public List<PortfolioPosition> Positions { get; set; } = new();
    }

    // Classe auxiliar para representar Positions[] do JSON
    public class PortfolioPosition
    {
        // No JSON esse campo vem como "assetSymbol"
        [JsonPropertyName("assetSymbol")]
        public string Symbol { get; set; }

        public decimal Quantity { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TargetAllocation { get; set; }
    }
}
