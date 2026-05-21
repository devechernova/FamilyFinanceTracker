using FamilyFinanceTracker.Models;
using FamilyFinanceTracker.Services;

class Program
{
    static void Main()
    {
        FinanceManager manager = new FinanceManager();

        Console.Write("Enter username: ");
        string name = Console.ReadLine()!;

        var user = manager.LoginUser(name);

        if (user == null)
        {
            Console.WriteLine("User not found!");
            return;
        }

        Console.WriteLine($"Welcome {user.Name}");

        manager.AddTransaction(new Transaction(1, 100, TransactionType.Income, 1, user.Id));
        manager.AddTransaction(new Transaction(2, 40, TransactionType.Expense, 1, user.Id));

        Console.WriteLine($"Balance: {manager.GetBalance()}");
    }
}


