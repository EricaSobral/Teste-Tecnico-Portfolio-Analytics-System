namespace PortfolioAnalyticsSystem.Models.DTOs;

public class PerformanceResponseDto
{
    public decimal TotalInvestment { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal TotalReturn { get; set; }
    public decimal TotalReturnAmount { get; set; }
    public decimal AnnualizedReturn { get; set; }
    public decimal? Volatility { get; set; } // pode ser null se não houver histórico
    public List<PositionPerformanceDto> PositionsPerformance { get; set; } = new();
}

public class PositionPerformanceDto
{
    public string Symbol { get; set; }
    public decimal InvestedAmount { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal Return { get; set; }
    public decimal Weight { get; set; }
}

// DTO usado no endpoint de performance.
// Separei esse modelo para representar apenas os dados
// que a API precisa devolver nesse cálculo.