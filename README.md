# 📊 Portfolio Analytics System

API desenvolvida em .NET 8 para análise de **performance**, **risco** e **rebalanceamento** de portfólios de investimento.

Este projeto foi implementado como **entrega de um teste técnico**, seguindo exatamente o escopo proposto.  
Os dados são carregados em memória a partir de um arquivo `SeedData.json`, sem uso de banco de dados persistente.

---

## 🎯 Objetivo

Implementar **3 endpoints analíticos** que processam dados financeiros de portfólios pré-carregados, **sem uso de banco de dados persistente**.

---

## 🚀 Tecnologias utilizadas

- .NET 8 (Web API)
- C#
- Swagger / OpenAPI
- xUnit (testes unitários)

---

## ▶️ Como executar o projeto

### Rodar a API
```bash
dotnet run
```
Acesse:

https://localhost:{porta}/swagger

### Rodar os testes
```bash
dotnet test
```
---

## 📁 Estrutura do projeto
```bash
Projeto/
├── Controllers/ # AnalyticsController
├── Services/ # Lógica dos cálculos financeiros
├── Models/ # Entidades e DTOs
├── Data/ # DataContext e SeedData.json
├── Tests/ # Testes unitários
├── Program.cs # Configuração da aplicação
└── README.md
```

---

## 📊 Dados

- Os dados são carregados automaticamente do arquivo `SeedData.json` no startup da aplicação
- Os dados permanecem em memória durante toda a execução
- Não há banco de dados
- Não há persistência
- Os IDs dos portfólios são gerados em memória de forma sequencial (1, 2, 3…)

---

## 🔌 Endpoints disponíveis

### 1️⃣ Performance Analysis

GET /api/portfolios/{id}/performance


Retorna métricas de performance do portfólio, incluindo:
- Valor investido
- Valor atual
- Retorno total (valor e %)
- Retorno anualizado
- Volatilidade
- Performance individual por posição

---

### 2️⃣ Risk Analysis

GET /api/portfolios/{id}/risk-analysis

Analisa:
- Sharpe Ratio
- Concentração de risco (maior posição e top 3)
- Diversificação por setor
- Recomendações de risco

---

### 3️⃣ Rebalancing
GET /api/portfolios/{id}/rebalancing


Sugere ajustes de rebalanceamento com base no target allocation, considerando:
- Desvio maior que 2%
- Custo de transação
- Benefício esperado

---

## 🧠 Decisões Técnicas Importantes

### Dados carregados em memória (DataContext)

O `DataContext` foi criado para centralizar a leitura do `SeedData.json` no momento em que a aplicação é iniciada.  
A ideia foi carregar os dados **uma única vez** e mantê-los em memória durante toda a execução da API.

Essa abordagem evita a leitura repetida do arquivo a cada requisição, mantém o acesso aos dados organizado e deixa o código mais simples, o que faz sentido para o escopo do teste técnico.

Como o seed não possui o campo `id` nos portfólios, mas o teste exige endpoints no formato `/api/portfolios/{id}`, os IDs são gerados em memória de forma sequencial apenas para manter o contrato da API.

---

### Uso de Singleton para o DataContext

O `DataContext` foi registrado como **Singleton** para garantir que os dados do seed sejam carregados apenas uma vez.  
Usei apoio de IA para confirmar essa escolha, já que eu tinha dúvida se esse seria o ciclo de vida mais adequado nesse cenário.

Como os dados são apenas lidos e não sofrem alteração durante a execução, essa decisão se mostrou segura e suficiente para o contexto do projeto.

---

### Organização da lógica nos Services

Toda a lógica de cálculo ficou concentrada nos Services:

- `PerformanceCalculator`
- `RiskAnalyzer`
- `RebalancingOptimizer`

Os controllers ficaram responsáveis apenas por receber o id do portfólio, validar se ele existe e retornar o resultado.

Essa separação deixou o fluxo mais fácil de acompanhar e evitou que regras financeiras ficassem misturadas com código de controller.

Durante o desenvolvimento, considerei criar helpers para alguns cálculos financeiros e matemáticos. No entanto, como o projeto é pequeno e os cálculos não se repetem de forma significativa, optei por mantê-los diretamente nos services.  
Essa decisão foi tomada para evitar abstrações desnecessárias, já que eu não tinha total clareza de que esses helpers realmente trariam ganho de reutilização nesse cenário.

---

### Cálculos financeiros e uso de LINQ

Os cálculos de performance, risco, volatilidade e rebalanceamento foram implementados de forma direta, priorizando clareza e legibilidade.

Em partes mais delicadas, como:

- cálculo de volatilidade
- uso de `Zip` para trabalhar com retornos diários
- ajustes de porcentagem e target allocation  

utilizei LINQ para simplificar o código. Nessas situações, contei com apoio de IA para validar a abordagem e garantir que os cálculos estivessem corretos, já que são pontos mais fáceis de errar.

---

### Rebalanceamento intencionalmente simples

A lógica de rebalanceamento foi implementada de forma propositalmente simples, seguindo apenas as regras descritas no enunciado.

Não foram utilizados algoritmos avançados ou estratégias complexas, pois o objetivo do teste é demonstrar entendimento do problema e aderência ao escopo, e não criar uma solução de nível produtivo.

---

## 🧪 Testes unitários

Foram implementados **5 testes unitários**, cobrindo os requisitos obrigatórios:
- Cálculo de retorno total
- Cálculo de volatilidade
- Sharpe Ratio em diferentes cenários
- Identificação de concentração de risco
- Sugestão de rebalanceamento

Os testes utilizam os **3 portfólios reais** do `SeedData.json`:
- Portfólio Conservador (`user-001`)
- Portfólio Crescimento (`user-002`)
- Portfólio Dividendos (`user-003`)

---
### ⚠️ Fora do escopo

- Banco de dados persistente

- Entity Framework

- Autenticação / autorização

- Atualização dinâmica de preços

- Algoritmos financeiros avançados

- Testes de integração
---
### 📌 Observações finais

- O foco deste projeto foi:

- Cumprir exatamente o escopo do teste

- Manter o código simples, legível e funcional

- Evitar complexidade desnecessária

- Priorizar clareza e aderência ao enunciado

