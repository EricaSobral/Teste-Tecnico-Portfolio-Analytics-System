# 📊 Portfolio Analytics System

API desenvolvida em .NET 8 para análise de **performance**, **risco** e **rebalanceamento** de portfólios de investimento.

Os dados são carregados em memória a partir de um `SeedData.json`, conforme a especificação do teste técnico.

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

## 📁 Estrutura do projeto

Projeto/
├── Controllers/ # AnalyticsController
├── Services/ # Lógica dos cálculos financeiros
├── Models/ # Entidades e DTOs
├── Data/ # DataContext e SeedData.json
├── Tests/ # Testes unitários
├── Program.cs # Configuração da aplicação
└── README.md


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

## 🧠 Decisões técnicas importantes

### Seed em memória

O `DataContext` é responsável por:
- Carregar o `SeedData.json`
- Manter os dados em memória
- Disponibilizar acesso simples aos portfólios e ativos

Essa abordagem evita leituras repetidas do arquivo e mantém o código mais simples.

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

