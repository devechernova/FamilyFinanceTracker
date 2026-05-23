using FamilyFinanceTracker.Models;
using FamilyFinanceTracker.Services;
using System.Text;

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
            Console.WriteLine("1. Einnahme hinzufügen");
            Console.WriteLine("2. Ausgabe hinzufügen");
            Console.WriteLine("3. Kontostand anzeigen");
            Console.WriteLine("4. Transaktionen anzeigen");
            Console.WriteLine("5. Beenden");
            Console.Write("Option wählen: ");

            string choice = Console.ReadLine()!;

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
                    running = false;
                    Console.WriteLine("Programm wird beendet...");
                    break;

                default:
                    Console.WriteLine("Ungültige Eingabe, bitte erneut versuchen!");
                    break;
            }
        }
    }

    static void AddTransaction(FinanceManager manager, User user, TransactionType type)
    {
        Console.Write("Betrag eingeben: ");
        decimal amount = Convert.ToDecimal(Console.ReadLine());

        Console.WriteLine("Kategorie auswählen:");

        var categories = manager.GetCategories();

        foreach (var c in categories)
        {
            Console.WriteLine($"{c.Id}. {c.Name}");
        }

        int categoryId = Convert.ToInt32(Console.ReadLine());

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
}

