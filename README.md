Budget Tracker

A full-stack personal finance web application built with ASP.NET Core MVC and SQLite. Track your income and expenses, set monthly budget limits, manage recurring transactions, and visualize spending patterns — all from a clean web interface.

Features

- Add & delete transactions — log income or expenses with a description, amount, and category
- Category filtering — filter your transaction history by category
- Monthly budget limit — set a spending cap with visual warnings when you're at 80% or over
- Recurring transactions — schedule automatic monthly entries (e.g. rent, subscriptions) that apply themselves on the configured day each month
- Monthly bar chart — visualize income vs. expenses across the last 6 months
- Category breakdown — see which spending categories are taking the biggest share
- Persistent storage — all data saved locally via SQLite (`budget.db`), no external database needed

Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 10) |
| Database | SQLite via Entity Framework Core |
| Frontend | Razor Views, HTML/CSS |
| ORM | Entity Framework Core 9 |

Getting Started

Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

Run locally

# Clone the repository
git clone https://github.com/yourusername/BudgetTracker.git

cd BudgetTracker

# Run the app

dotnet run

Then open your browser and go to `http://localhost:5000`.

The SQLite database (`budget.db`) is created automatically on first run — no setup needed.

Project Structure

```
BudgetTracker/
├── Controllers/
│   └── HomeController.cs       # All app logic and routes
├── Models/
│   ├── Transaction.cs          # Transaction model + enums
│   ├── RecurringTransaction.cs # Recurring entry model
│   ├── BudgetLimit.cs          # Monthly limit model
│   └── BudgetViewModel.cs      # View model with computed stats
├── Data/
│   └── AppDbContext.cs         # EF Core database context
├── Views/
│   └── Home/
│       └── Index.cshtml        # Main dashboard view
├── wwwroot/                    # Static assets (CSS, JS)
├── Program.cs                  # App setup and middleware
└── BudgetTracker.csproj
```
