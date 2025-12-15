namespace PortfolioAnalyticsSystem.Models.DTOs;

public class RiskAnalysisResponseDto
{
    public string OverallRisk { get; set; }
    public decimal SharpeRatio { get; set; }
    public ConcentrationRiskDto ConcentrationRisk { get; set; }
    public List<SectorDiversificationDto> SectorDiversification { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

public class ConcentrationRiskDto
{
    public LargestPositionDto LargestPosition { get; set; }
    public decimal Top3Concentration { get; set; }
}

public class LargestPositionDto
{
    public string Symbol { get; set; }
    public decimal Percentage { get; set; }
}

public class SectorDiversificationDto
{
    public string Sector { get; set; }
    public decimal Percentage { get; set; }
    public string Risk { get; set; }
}

// DTO usado no endpoint de análise de risco.
// Separei esse modelo para deixar claro o formato da resposta da API
// e não misturar com o modelo de domínio.