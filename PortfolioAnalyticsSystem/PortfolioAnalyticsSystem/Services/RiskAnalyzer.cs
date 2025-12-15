using PortfolioAnalyticsSystem.Data;
using PortfolioAnalyticsSystem.Models;
using PortfolioAnalyticsSystem.Models.DTOs;

namespace PortfolioAnalyticsSystem.Services;

public class RiskAnalyzer
{
    private readonly DataContext _dataContext;

    private const decimal DeviationThreshold = 2m;
    private const decimal MinTradeValue = 100m;
    private const decimal TransactionCostRate = 0.003m;

    public RiskAnalyzer(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public RiskAnalysisResponseDto Analyze(Portfolio portfolio)
    {
        // Usei LINQ pra deixar o cálculo mais simples.
        // Pedi ajuda de IA pra ver se esse caminho fazia sentido.
        decimal totalValue =
            portfolio.Positions
                .Select(p =>
                {
                    var asset = _dataContext.GetAssetBySymbol(p.Symbol);
                    if (asset == null || asset.CurrentPrice <= 0)
                        return 0m;

                    return p.Quantity * asset.CurrentPrice;
                })
                .Sum();

        // Se o total for 0, faço esse ajuste pra evitar erro na conta.
        if (totalValue <= 0)
            totalValue = 1m;

        // Concentração por posição (quanto cada ativo representa no portfólio).
        // Esse ajuste no target foi validado com o apoio de uma IA, pois o cálculo
        // de porcentagem estava gerando dúvidas na hora de garantir que tudo estivesse certo.
        var positions =
            portfolio.Positions
                .Select(p =>
                {
                    var asset = _dataContext.GetAssetBySymbol(p.Symbol);

                    var value =
                        (asset != null && asset.CurrentPrice > 0)
                            ? p.Quantity * asset.CurrentPrice
                            : 0m;

                    var percentage = (value / totalValue) * 100m;

                    return new
                    {
                        p.Symbol,
                        Value = value,
                        Percentage = percentage
                    };
                })
                .OrderByDescending(x => x.Percentage)
                .ToList();

        var largestPosition = positions.First();
        var top3Concentration = positions.Take(3).Sum(p => p.Percentage);

        // Agrupando por setor para medir diversificação.
        // Essa parte de calcular risco por setor foi verificada com IA também, 
        // porque as classificações de risco (alto, médio, baixo) não estavam tão claras.
        var sectorDiversification =
            positions
                .GroupBy(p =>
                {
                    var asset = _dataContext.GetAssetBySymbol(p.Symbol);
                    return asset?.Sector ?? "Unknown";
                })
                .Select(g =>
                {
                    var sectorPercentage = g.Sum(x => x.Percentage);

                    return new SectorDiversificationDto
                    {
                        Sector = g.Key,
                        Percentage = sectorPercentage,
                        Risk = sectorPercentage > 40 ? "High"
                             : sectorPercentage >= 25 ? "Medium"
                             : "Low"
                    };
                })
                .ToList();

        decimal sharpeRatio = 0m;
        var volatility = CalculateVolatility(portfolio);

        if (portfolio.TotalInvestment > 0 && volatility.HasValue && volatility.Value > 0)
        {
            var portfolioReturn = (totalValue - portfolio.TotalInvestment) / portfolio.TotalInvestment;
            sharpeRatio = (portfolioReturn - _dataContext.SelicRate) / volatility.Value;
        }

        var recommendations = new List<string>();

        if (largestPosition.Percentage > 25m)
        {
            recommendations.Add(
                $"Posição {largestPosition.Symbol} representa {largestPosition.Percentage:F1}% do portfólio (ideal < 20%)"
            );
        }

        foreach (var sector in sectorDiversification.Where(s => s.Risk == "High"))
        {
            recommendations.Add(
                $"Reduzir exposição ao setor {sector.Sector} ({sector.Percentage:F1}%)"
            );
        }

        string overallRisk =
            largestPosition.Percentage > 25m || sectorDiversification.Any(s => s.Percentage > 40m)
                ? "High"
                : largestPosition.Percentage >= 15m || sectorDiversification.Any(s => s.Percentage >= 25m)
                    ? "Medium"
                    : "Low";

        return new RiskAnalysisResponseDto
        {
            OverallRisk = overallRisk,
            SharpeRatio = sharpeRatio,

            ConcentrationRisk = new ConcentrationRiskDto
            {
                LargestPosition = new LargestPositionDto
                {
                    Symbol = largestPosition.Symbol,
                    Percentage = largestPosition.Percentage
                },
                Top3Concentration = top3Concentration
            },

            SectorDiversification = sectorDiversification,
            Recommendations = recommendations
        };
    }
    // Aqui eu calculo a volatilidade usando retornos diários dos ativos.
    // Usei uma lógica mais estatística (Zip + desvio padrão) e contei
    // com o apoio de IA pra garantir que o cálculo fazia sentido.
    private decimal? CalculateVolatility(Portfolio portfolio)
    {
        var volatilities =
            portfolio.Positions
                .Select(p =>
                {
                    if (!_dataContext.TryGetPriceHistory(p.Symbol, out var prices) || prices == null || prices.Count < 2)
                        return (decimal?)null;

                    var dailyReturns =
                        prices
                            .Zip(prices.Skip(1), (prev, curr) =>
                            {
                                if (prev <= 0 || curr <= 0) return (decimal?)null;
                                return (curr - prev) / prev;
                            })
                            .Where(r => r.HasValue)
                            .Select(r => r!.Value)
                            .ToList();

                    if (dailyReturns.Count == 0)
                        return (decimal?)null;

                    return StandardDeviation(dailyReturns); 
                })
                .Where(v => v.HasValue)
                .Select(v => v!.Value)
                .ToList();

        if (volatilities.Count == 0)
            return null;

        return volatilities.Average();
    }

    private decimal StandardDeviation(List<decimal> values)
    {
        var mean = values.Average();
        var variance = values.Select(v => (v - mean) * (v - mean)).Average();
        return (decimal)Math.Sqrt((double)variance);
    }
}
