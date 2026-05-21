using System;

namespace FamilyFinanceTracker.Models;

public enum TransactionType
{
    Income,
    Expense
}

public class Transaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public int CategoryId { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }

    public Transaction(int id, decimal amount, TransactionType type, int categoryId, int userId)
    {
        Id = id;
        Amount = amount;
        Type = type;
        CategoryId = categoryId;
        UserId = userId;
        Date = DateTime.Now;
    }
}
