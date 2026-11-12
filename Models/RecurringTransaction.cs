namespace BudgetTracker.Models;

public class RecurringTransaction
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    // Day of month to apply (1-28)
    public int DayOfMonth { get; set; }
    public DateTime LastApplied { get; set; } = DateTime.MinValue;
    public bool IsActive { get; set; } = true;
}
