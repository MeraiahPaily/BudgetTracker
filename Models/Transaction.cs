namespace BudgetTracker.Models;

public class Transaction
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public bool IsRecurring { get; set; } = false;
    public RecurringFrequency? Frequency { get; set; }
}

public enum TransactionType
{
    Income,
    Expense
}

public enum RecurringFrequency
{
    Monthly,
    Weekly,
    Yearly
}
