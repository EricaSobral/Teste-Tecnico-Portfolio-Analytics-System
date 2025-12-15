using PortfolioAnalyticsSystem.Data;
using PortfolioAnalyticsSystem.Models;
using PortfolioAnalyticsSystem.Models.DTOs;

namespace PortfolioAnalyticsSystem.Services;

public class RebalancingOptimizer
{
    private readonly DataContext _dataContext;

    private const decimal DeviationThreshold = 2m;
    private const decimal MinTradeValue = 100m;
    private const decimal TransactionCostRate = 0.003m;

    public RebalancingOptimizer(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public RebalancingResponseDto Optimize(Portfolio portfolio)
    {
        // Usei LINQ aqui pra deixar a conta mais direta.
        // Confirmei essa forma com o apoio de uma IA, porque eu estava em dúvida
        // se esse seria o jeito mais simples de fazer.
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

        // Se o total vier zerado, ajusto pra não dar erro na conta.
        if (totalValue <= 0)
            totalValue = 1m;

        // Aqui eu monto a alocação atual comparando o peso real com o target.
        // Tive que pensar com cuidado nessa parte porque o target pode vir
        // como fração ou como porcentagem, então confirmei esse ajuste
        // com o apoio de uma IA.
        var allocation =
            portfolio.Positions
                .Select(p =>
                {
                    var asset = _dataContext.GetAssetBySymbol(p.Symbol);

                    decimal currentPrice =
                        (asset != null && asset.CurrentPrice > 0) ? asset.CurrentPrice : 0m;

                    decimal positionValue =
                        currentPrice > 0 ? p.Quantity * currentPrice : 0m;

                    decimal currentWeight = (positionValue / totalValue) * 100m;
                    decimal targetWeight = p.TargetAllocation;

                    if (targetWeight > 0 && targetWeight <= 1)
                        targetWeight *= 100m;

                    decimal deviation = currentWeight - targetWeight;

                    return new
                    {
                        p.Symbol,
                        CurrentPrice = currentPrice,
                        PositionValue = positionValue,
                        CurrentWeight = currentWeight,
                        TargetWeight = targetWeight,
                        Deviation = deviation
                    };
                })
                .ToList();

        var currentAllocation =
            allocation
                .Select(a => new CurrentAllocationDto
                {
                    Symbol = a.Symbol,
                    CurrentWeight = a.CurrentWeight,
                    TargetWeight = a.TargetWeight,
                    Deviation = a.Deviation
                })
                .ToList();

        // Aqui eu filtro só o que realmente precisa de rebalanceamento
        // e ordeno pelos maiores desvios primeiro.
        // Confirmei esse critério com o apoio de uma IA
        // pra garantir que fazia sentido pro escopo do teste.
        var needsTrade =
            allocation
                .Where(a => Math.Abs(a.Deviation) > DeviationThreshold)
                .OrderByDescending(a => Math.Abs(a.Deviation))
                .ToList();

        var suggestedTrades = new List<SuggestedTradeDto>();

        // A lógica de trade é propositalmente simples:
        // tento aproximar o valor atual do target sem fazer otimização avançada.
        // Conferi essa abordagem com uma IA porque fiquei na dúvida
        // se precisava de algo mais complexo aqui.
        foreach (var item in needsTrade)
        {
            if (item.CurrentPrice <= 0)
                continue;

            decimal targetValue = totalValue * (item.TargetWeight / 100m);
            decimal differenceValue = targetValue - item.PositionValue;

            if (differenceValue == 0)
                continue;

            string action = differenceValue > 0 ? "BUY" : "SELL";

            decimal estimatedValue = Math.Abs(differenceValue);

            if (estimatedValue < MinTradeValue)
                continue;

            int quantity = (int)Math.Floor(estimatedValue / item.CurrentPrice);

            if (quantity <= 0)
                continue;

            estimatedValue = quantity * item.CurrentPrice;

            // Aqui calculei o custo da transação em cima do valor estimado.
            // Conferi essa conta com o apoio de uma IA pra não errar
            // em detalhe de porcentagem.
            decimal transactionCost = estimatedValue * TransactionCostRate;

            suggestedTrades.Add(new SuggestedTradeDto
            {
                Symbol = item.Symbol,
                Action = action,
                Quantity = quantity,
                EstimatedValue = estimatedValue,
                TransactionCost = transactionCost,
                Reason = action == "SELL"
                    ? $"Reduzir de {item.CurrentWeight:F1}% para {item.TargetWeight:F1}%"
                    : $"Aumentar de {item.CurrentWeight:F1}% para {item.TargetWeight:F1}%"
            });
        }

        decimal totalTransactionCost = suggestedTrades.Sum(t => t.TransactionCost);

        return new RebalancingResponseDto
        {
            NeedsRebalancing = suggestedTrades.Any(),
            CurrentAllocation = currentAllocation,
            SuggestedTrades = suggestedTrades,
            TotalTransactionCost = totalTransactionCost,
            ExpectedImprovement = suggestedTrades.Any()
                ? "Redução de risco de concentração (estimativa)"
                : "Portfólio já está próximo do target"
        };
    }
}