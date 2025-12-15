using System.Text.Json;
using PortfolioAnalyticsSystem.Models;

namespace PortfolioAnalyticsSystem.Data
{
    // Criei a classe DataContext para carregar os dados do SeedData.json
    // quando a API é iniciada. Dessa forma, os dados ficam salvos em memória
    // e podem ser reutilizados sempre que necessário.
    //
    // Isso evita ler o arquivo JSON dentro do controller, já que 
    // a cada chamada de um endpoint, o arquivo seria lido novamente
    // o que diminuiria a performance,  dificultaria os testes
    // e deixaria o código  desorganizado.
    public class DataContext
    {
        public List<Portfolio> Portfolios { get; }
        public List<Asset> Assets { get; }
        public Dictionary<string, List<decimal>> PriceHistoryBySymbol { get; }
        public decimal SelicRate { get; }

        public DataContext()
        {
            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Data",
                "SeedData.json"
            );

            if (!File.Exists(filePath))
                throw new FileNotFoundException("SeedData.json não encontrado", filePath);

            var json = File.ReadAllText(filePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            // O teste exige endpoints no formato /api/portfolios/{id},
            // mas o seed não traz um campo "id" para os portfolios.
            // Para manter o contrato da API, decidi  gerar um Id sequencial em memória.
            var portfoliosJson = root.GetProperty("portfolios").GetRawText();
            Portfolios = JsonSerializer.Deserialize<List<Portfolio>>(portfoliosJson, options)
                         ?? new List<Portfolio>();

            for (int i = 0; i < Portfolios.Count; i++)
                Portfolios[i].Id = i + 1;

            var assetsJson = root.GetProperty("assets").GetRawText();
            Assets = JsonSerializer.Deserialize<List<Asset>>(assetsJson, options)
                     ?? new List<Asset>();

            // O priceHistory no seed possui data e preço, mas para as análises
            // pedidas no teste a data não é utilizada.
            // Por isso, simplifiquei a estrutura para uma lista de preços por ativo.
            PriceHistoryBySymbol = new Dictionary<string, List<decimal>>(
                StringComparer.OrdinalIgnoreCase
            );

            var priceHistoryObj = root.GetProperty("priceHistory");

            foreach (var property in priceHistoryObj.EnumerateObject())
            {
                var symbol = property.Name;

                // Aqui utilizei LINQ para extrair apenas os preços da estrutura original.
                // Essa parte foi validada com o auxílio de uma IA para garantir
                // clareza e evitar código desnecessariamente verboso.
                var prices =
                    property.Value
                        .EnumerateArray()
                        .Select(item => item.GetProperty("price").GetDecimal())
                        .ToList();

                PriceHistoryBySymbol[symbol] = prices;
            }

            // A Selic é carregada uma única vez, já que o valor não muda
            // durante a execução da aplicação.
            SelicRate = root
                .GetProperty("marketData")
                .GetProperty("selicRate")
                .GetDecimal();
        }

        public Portfolio? GetPortfolioById(int id)
            => Portfolios.FirstOrDefault(p => p.Id == id);

        public Asset? GetAssetBySymbol(string symbol)
            => Assets.FirstOrDefault(a =>
                string.Equals(a.Symbol, symbol, StringComparison.OrdinalIgnoreCase));

        public bool TryGetPriceHistory(string symbol, out List<decimal>? prices)
            => PriceHistoryBySymbol.TryGetValue(symbol, out prices);
    }
}
