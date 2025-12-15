namespace PortfolioAnalyticsSystem.Models.DTOs;


// DTOs usados no endpoint /rebalancing.
// Separei esses modelos do domínio para representar exatamente
// o formato de resposta da API.

public class RebalancingResponseDto
{
    public bool NeedsRebalancing { get; set; }

    public List<CurrentAllocationDto> CurrentAllocation { get; set; } = new();

    public List<SuggestedTradeDto> SuggestedTrades { get; set; } = new();

    public decimal TotalTransactionCost { get; set; }

    public string ExpectedImprovement { get; set; } = string.Empty;
}

public class CurrentAllocationDto
{
    public string Symbol { get; set; } = string.Empty;

    public decimal CurrentWeight { get; set; }

    public decimal TargetWeight { get; set; }

    public decimal Deviation { get; set; }
}

public class SuggestedTradeDto
{
    public string Symbol { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal EstimatedValue { get; set; }

    public decimal TransactionCost { get; set; }

    public string Reason { get; set; } = string.Empty;
}
