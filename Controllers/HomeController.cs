using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Data;
using BudgetTracker.Models;

namespace BudgetTracker.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    // Auto-apply any due recurring transactions
    private async Task ApplyRecurringTransactions()
    {
        var today = DateTime.Now;
        var recurrings = await _db.RecurringTransactions
            .Where(r => r.IsActive)
            .ToListAsync();

        foreach (var r in recurrings)
        {
            // Apply if we haven't applied it this month yet and today >= DayOfMonth
            var alreadyAppliedThisMonth =
                r.LastApplied.Year == today.Year && r.LastApplied.Month == today.Month;

            if (!alreadyAppliedThisMonth && today.Day >= r.DayOfMonth)
            {
                _db.Transactions.Add(new Transaction
                {
                    Description = r.Description + " (recurring)",
                    Amount = r.Amount,
                    Category = r.Category,
                    Type = r.Type,
                    Date = new DateTime(today.Year, today.Month, r.DayOfMonth)
                });
                r.LastApplied = today;
            }
        }

        await _db.SaveChangesAsync();
    }

    public async Task<IActionResult> Index(string? category)
    {
        await ApplyRecurringTransactions();

        var all = await _db.Transactions
            .OrderByDescending(t => t.Date)
            .ToListAsync();

        var filtered = string.IsNullOrEmpty(category)
            ? all
            : all.Where(t => t.Category == category).ToList();

        // Monthly bar chart (last 6 months)
        var chart = new List<MonthlyData>();
        for (int i = 5; i >= 0; i--)
        {
            var m = DateTime.Now.AddMonths(-i);
            var monthTx = all.Where(t => t.Date.Year == m.Year && t.Date.Month == m.Month).ToList();
            chart.Add(new MonthlyData
            {
                Month = m.ToString("MMM"),
                Income = monthTx.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                Expenses = monthTx.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
            });
        }

        // Category breakdown (expenses only)
        var expensesByCategory = all
            .Where(t => t.Type == TransactionType.Expense)
            .GroupBy(t => t.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount) })
            .OrderByDescending(x => x.Total)
            .ToList();
        var totalExp = expensesByCategory.Sum(x => x.Total);
        var categoryBreakdown = expensesByCategory.Select(x => new CategoryData
        {
            Category = x.Category,
            Amount = x.Total,
            Percent = totalExp > 0 ? Math.Round((double)(x.Total / totalExp) * 100, 1) : 0
        }).ToList();

        // Budget limit
        var limitRow = await _db.BudgetLimits.FirstOrDefaultAsync();
        var thisMonth = DateTime.Now;
        var thisMonthExpenses = all
            .Where(t => t.Type == TransactionType.Expense
                     && t.Date.Year == thisMonth.Year
                     && t.Date.Month == thisMonth.Month)
            .Sum(t => t.Amount);

        // Recurring transactions
        var recurrings = await _db.RecurringTransactions
            .OrderBy(r => r.DayOfMonth)
            .ToListAsync();

        var vm = new BudgetViewModel
        {
            Transactions = all,
            FilteredTransactions = filtered,
            TotalIncome = all.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
            TotalExpenses = all.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount),
            ActiveCategory = category,
            AllCategories = all.Select(t => t.Category).Distinct().OrderBy(c => c).ToList(),
            MonthlyChart = chart,
            CategoryBreakdown = categoryBreakdown,
            MonthlyLimit = limitRow?.MonthlyLimit ?? 0,
            ThisMonthExpenses = thisMonthExpenses,
            RecurringTransactions = recurrings
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Add(Transaction transaction)
    {
        transaction.Date = DateTime.Now;
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await _db.Transactions.FindAsync(id);
        if (t != null) { _db.Transactions.Remove(t); await _db.SaveChangesAsync(); }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> SetBudgetLimit(decimal monthlyLimit)
    {
        var existing = await _db.BudgetLimits.FirstOrDefaultAsync();
        if (existing == null)
            _db.BudgetLimits.Add(new BudgetLimit { MonthlyLimit = monthlyLimit });
        else
            existing.MonthlyLimit = monthlyLimit;
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> AddRecurring(RecurringTransaction recurring)
    {
        _db.RecurringTransactions.Add(recurring);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRecurring(int id)
    {
        var r = await _db.RecurringTransactions.FindAsync(id);
        if (r != null) { _db.RecurringTransactions.Remove(r); await _db.SaveChangesAsync(); }
        return RedirectToAction("Index");
    }
}
