namespace PortfolioAnalyticsSystem.Models;

public class Asset
{
    public string Symbol { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Sector { get; set; }
    public decimal CurrentPrice { get; set; }
}