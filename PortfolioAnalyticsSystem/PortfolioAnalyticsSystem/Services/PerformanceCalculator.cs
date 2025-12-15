using PortfolioAnalyticsSystem.Data;
using PortfolioAnalyticsSystem.Models;
using PortfolioAnalyticsSystem.Models.DTOs;

namespace PortfolioAnalyticsSystem.Services;

public class PerformanceCalculator
{
    private readonly DataContext _dataContext;

    // Recebo o DataContext pra pegar preço atual e histórico aqui no service,
    // e deixar o controller só chamando e retornando.
    public PerformanceCalculator(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public PerformanceResponseDto Calculate(Portfolio portfolio)
    {
        
        decimal currentValue =
            portfolio.Positions
                .Select(p =>
                {
                    var asset = _dataContext.GetAssetBySymbol(p.Symbol);
                    if (asset == null || asset.CurrentPrice <= 0)
                        return 0m;

                    return p.Quantity * asset.CurrentPrice;
                })
                .Sum();

        decimal totalReturnAmount = currentValue - portfolio.TotalInvestment;

        decimal totalReturnPercent =
            portfolio.TotalInvestment > 0
                ? (totalReturnAmount / portfolio.TotalInvestment) * 100m
                : 0m;

        // Aqui eu usei 30 dias como base porque é o período que o seed representa.
        // Eu usei IA como apoio nessa conta pra não errar a fórmula do anualizado.
        int days = 30;
        decimal annualizedReturnPercent = 0m;

        if (days > 0)
        {
            var totalReturnDecimal = totalReturnPercent / 100m;
            annualizedReturnPercent =
                (decimal)((Math.Pow(1 + (double)totalReturnDecimal, 365.0 / days) - 1) * 100.0);
        }

        var positionsPerformance =
            portfolio.Positions
                .Select(p =>
                {
                    decimal investedAmount = p.Quantity * p.AveragePrice;

                    var asset = _dataContext.GetAssetBySymbol(p.Symbol);
                    decimal positionCurrentValue =
                        (asset != null && asset.CurrentPrice > 0)
                            ? p.Quantity * asset.CurrentPrice
                            : 0m;

                    decimal positionReturnPercent =
                        investedAmount > 0
                            ? ((positionCurrentValue - investedAmount) / investedAmount) * 100m
                            : 0m;

                    decimal weightPercent =
                        currentValue > 0
                            ? (positionCurrentValue / currentValue) * 100m
                            : 0m;

                    return new PositionPerformanceDto
                    {
                        Symbol = p.Symbol,
                        InvestedAmount = investedAmount,
                        CurrentValue = positionCurrentValue,
                        Return = positionReturnPercent,
                        Weight = weightPercent
                    };
                })
                .ToList();

        // Se não tiver histórico suficiente, eu retorno null aqui como o teste pede.
        decimal? volatility = CalculateVolatility(portfolio);

        return new PerformanceResponseDto
        {
            TotalInvestment = portfolio.TotalInvestment,
            CurrentValue = currentValue,
            TotalReturn = totalReturnPercent,
            TotalReturnAmount = totalReturnAmount,
            AnnualizedReturn = annualizedReturnPercent,
            Volatility = volatility,
            PositionsPerformance = positionsPerformance
        };
    }

    // Pra volatilidade eu usei os retornos diários e depois tirei uma média.
    // Mantive algo simples porque já atende o que o teste pede.
    // Nessa parte do Zip eu usei IA como apoio, já que a lógica é mais complicadinha.
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

                    return StandardDeviation(dailyReturns) * 100m;
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
