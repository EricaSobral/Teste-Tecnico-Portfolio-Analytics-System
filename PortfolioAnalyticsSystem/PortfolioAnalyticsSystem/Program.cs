using PortfolioAnalyticsSystem.Data;
using PortfolioAnalyticsSystem.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrando o DataContext para que uma única instância seja utilizada
// na aplicação inteira. Assim, os dados do SeedData.json são carregados
// uma vez e mantidos em memória.
// Tive o auxílio de uma IA para tomar essa decisão, pois inicialmente
// não estava tão claro se o melhor seria usar Singleton ou Transient
builder.Services.AddSingleton<DataContext>();


builder.Services.AddScoped<PerformanceCalculator>();
builder.Services.AddScoped<RiskAnalyzer>();
builder.Services.AddScoped<RebalancingOptimizer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();