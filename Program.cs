namespace FamilyFinanceTracker;

using FamilyFinanceTracker.Models;

class Program
{
    static void Main()
    {
        Transaction t = new Transaction(1, 50, TransactionType.Expense, 1, 1);
        Console.WriteLine($"Amount: {t.Amount}, Type: {t.Type}");
    }
}


