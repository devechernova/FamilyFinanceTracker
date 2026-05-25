using FamilyFinanceTracker.Models;
using FamilyFinanceTracker.Services;
using System.Text;
using System.Linq;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        FinanceManager manager = new FinanceManager();

        Console.Write("Benutzername eingeben: ");
        string name = Console.ReadLine()!;

        var user = manager.LoginUser(name);

        if (user == null)
        {
            Console.WriteLine("Benutzer wurde nicht gefunden!");
            return;
        }

        Console.WriteLine($"Willkommen, {user.Name}!");

        bool running = true;

        while (running)
        {
            Console.WriteLine("\n=== MENÜ ===");

            if (user.Role == Role.Parent)
            {
                Console.WriteLine("1. Einnahme hinzufügen");
                Console.WriteLine("2. Ausgabe hinzufügen");
                Console.WriteLine("3. Kontostand anzeigen");
                Console.WriteLine("4. Transaktionen anzeigen");
                Console.WriteLine("5. Statistik anzeigen");
                Console.WriteLine("6. Beenden");
            }
            else
            {
                Console.WriteLine("1. Ausgabe hinzufügen");
                Console.WriteLine("2. Meine Ausgaben anzeigen");
                Console.WriteLine("3. Beenden");
            }


            string choice = Console.ReadLine()!;

            if (user.Role == Role.Parent)
            {
                switch (choice)
                {
                    case "1":
                        AddTransaction(manager, user, TransactionType.Income);
                        break;

                    case "2":
                        AddTransaction(manager, user, TransactionType.Expense);
                        break;

                    case "3":
                        Console.WriteLine($"Aktueller Kontostand: {manager.GetBalance()} €");
                        break;

                    case "4":
                        ShowTransactions(manager);
                        break;

                    case "5":
                        ShowStatistics(manager);
                        break;

                    case "6":
                        running = false;
                        break;
                }
            }
            else
            {
                switch (choice)
                {
                    case "1":
                        AddTransaction(manager, user, TransactionType.Expense);
                        break;

                    case "2":
                        ShowTransactionsForUser(manager, user);
                        break;

                    case "3":
                        running = false;
                        break;
                }
            }
        }
    }

    static void AddTransaction(FinanceManager manager, User user, TransactionType type)
    {
        if (user.Role == Role.Child && type == TransactionType.Income)
        {
            Console.WriteLine("Kinder dürfen keine Einnahmen hinzufügen!");
            return;
        }
        Console.Write("Betrag eingeben: ");

        decimal amount;
        while (!decimal.TryParse(Console.ReadLine(), out amount))
        {
            Console.Write("Ungültiger Betrag, bitte erneut eingeben: ");
        }


        int categoryId = 0;

        if (type == TransactionType.Expense)
        {
            Console.WriteLine("Kategorie auswählen:");

            var categories = manager.GetCategories();

            foreach (var c in categories)
            {
                Console.WriteLine($"{c.Id}. {c.Name}");
            }

            while (!int.TryParse(Console.ReadLine(), out categoryId) ||
                   !categories.Any(c => c.Id == categoryId))
            {
                Console.Write("Ungültige Kategorie, bitte erneut wählen: ");
            }
        }
        else
        {
            // Einkommen — ohne Kategorie
            categoryId = 0;
            Console.WriteLine("Einnahme wird zum Konto hinzugefügt ✅");
        }


        manager.AddTransaction(new Transaction(0, amount, type, categoryId, user.Id));

        Console.WriteLine("Transaktion wurde hinzugefügt!");
    }

    static void ShowTransactions(FinanceManager manager)
    {
        var transactions = manager.GetAllTransactions();

        Console.WriteLine("\n=== TRANSAKTIONEN ===");

        foreach (var t in transactions)
        {
            string typeText = t.Type == TransactionType.Income ? "Einnahme" : "Ausgabe";

            Console.WriteLine($"{typeText}: {t.Amount} € | Kategorie: {manager.GetCategoryName(t.CategoryId)}");

        }
    }
    static void ShowTransactionsForUser(FinanceManager manager, User user)
    {
        var transactions = manager.GetAllTransactions()
                                  .Where(t => t.UserId == user.Id);

        Console.WriteLine("\n=== MEINE AUSGABEN ===");

        foreach (var t in transactions)
        {
            Console.WriteLine($"Ausgabe: {t.Amount} € | Kategorie: {manager.GetCategoryName(t.CategoryId)}");
        }
    }
    static void ShowStatistics(FinanceManager manager)
    {
        var stats = manager.GetExpensesByCategory();

        Console.WriteLine("\n=== AUSGABEN NACH KATEGORIEN ===");

        foreach (var entry in stats)
        {
            Console.WriteLine($"{entry.Key}: {entry.Value} €");
        }
    }
}

