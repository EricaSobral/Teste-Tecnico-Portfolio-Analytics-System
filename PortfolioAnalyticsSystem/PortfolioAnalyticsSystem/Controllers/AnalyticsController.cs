using Microsoft.AspNetCore.Mvc;
using PortfolioAnalyticsSystem.Data;
using PortfolioAnalyticsSystem.Services;
using PortfolioAnalyticsSystem.Models.DTOs;

namespace PortfolioAnalyticsSystem.Controllers;

[ApiController]
[Route("api")]
public class AnalyticsController : ControllerBase
{
    private readonly DataContext _dataContext;
    private readonly PerformanceCalculator _performanceCalculator;
    private readonly RiskAnalyzer _riskAnalyzer;
    private readonly RebalancingOptimizer _rebalancingOptimizer;


    public AnalyticsController(
        DataContext dataContext,
        PerformanceCalculator performanceCalculator,
        RiskAnalyzer riskAnalyzer,
        RebalancingOptimizer rebalancingOptimizer)
    {
        _dataContext = dataContext;
        _performanceCalculator = performanceCalculator;
        _riskAnalyzer = riskAnalyzer;
        _rebalancingOptimizer = rebalancingOptimizer;
    }

    [HttpGet("portfolios/{id:int}/performance")]
    public IActionResult GetPerformance(int id)
    {
        var portfolio = _dataContext.GetPortfolioById(id);

        if (portfolio is null)
            return NotFound();

        var result = _performanceCalculator.Calculate(portfolio);

        return Ok(result);
    }

    [HttpGet("portfolios/{id:int}/risk-analysis")]
    public IActionResult GetRiskAnalysis(int id)
    {
        var portfolio = _dataContext.GetPortfolioById(id);

        if (portfolio is null)
            return NotFound();

        var result = _riskAnalyzer.Analyze(portfolio);

        return Ok(result);
    }

    [HttpGet("portfolios/{id:int}/rebalancing")]
    public IActionResult GetRebalancing(int id)
    {
        var portfolio = _dataContext.GetPortfolioById(id);

        if (portfolio == null)
            return NotFound();

        var result = _rebalancingOptimizer.Optimize(portfolio);

        return Ok(result);
    }

}


// Esse controller centraliza os endpoints analíticos do portfólio.
// Ele apenas recebe o id, valida a existência do portfólio
// e envia os cálculos para os services, mantendo o controller simples.
