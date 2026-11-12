namespace BudgetTracker.Models;

public class BudgetViewModel
{
    public List<Transaction> Transactions { get; set; } = new();
    public List<Transaction> FilteredTransactions { get; set; } = new();
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Balance => TotalIncome - TotalExpenses;

    public string? ActiveCategory { get; set; }
    public List<string> AllCategories { get; set; } = new();

    // Monthly bar chart
    public List<MonthlyData> MonthlyChart { get; set; } = new();

    // Category donut chart
    public List<CategoryData> CategoryBreakdown { get; set; } = new();

    // Budget limit
    public decimal MonthlyLimit { get; set; }
    public decimal ThisMonthExpenses { get; set; }
    public decimal BudgetUsedPercent => MonthlyLimit > 0
        ? Math.Min(100, (ThisMonthExpenses / MonthlyLimit) * 100)
        : 0;
    public bool IsOverBudget => MonthlyLimit > 0 && ThisMonthExpenses >= MonthlyLimit;
    public bool IsNearBudget => MonthlyLimit > 0 && BudgetUsedPercent >= 80 && !IsOverBudget;

    // Recurring transactions
    public List<RecurringTransaction> RecurringTransactions { get; set; } = new();

    public Transaction NewTransaction { get; set; } = new();
}

public class MonthlyData
{
    public string Month { get; set; } = string.Empty;
    public decimal Income { get; set; }
    public decimal Expenses { get; set; }
}

public class CategoryData
{
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public double Percent { get; set; }
}
