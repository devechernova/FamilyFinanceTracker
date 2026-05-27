using Spectre.Console;
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

        User? user = null;

        while (user == null)
        {
            Console.Write("Benutzername eingeben: ");
            string name = Console.ReadLine();

            user = manager.LoginUser(name);

            if (user == null)
            {
                Console.WriteLine("Benutzer wurde nicht gefunden, bitte erneut eingeben! ❌");
            }
        }

                Console.WriteLine($"Willkommen, {user.Name}!");

        bool running = true;

        while (running)
        {
            string choice;

            if (user.Role == Role.Parent)
            {
                choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Menü auswählen:[/]")
                        .PageSize(10)
                        .AddChoices(new[]
                        {
                    "Einnahme hinzufügen",
                    "Ausgabe hinzufügen",
                    "Kontostand anzeigen",
                    "Transaktionen anzeigen",
                    "Statistik anzeigen",
                    "Beenden"
                        }));
            }
            else
            {
                choice = AnsiConsole.Prompt(
     new SelectionPrompt<string>()
         .Title("[green]Menü auswählen:[/]")
         .PageSize(10)
         .AddChoices(new[]
         {
            "Ausgabe hinzufügen",
            "Meine Ausgaben anzeigen",
            "Beenden"
         }));
            }

            if (user.Role == Role.Parent)
            {
                switch (choice)
                {
                    case "Einnahme hinzufügen":
                        AddTransaction(manager, user, TransactionType.Income);
                        break;

                    case "Ausgabe hinzufügen":
                        AddTransaction(manager, user, TransactionType.Expense);
                        break;

                    case "Kontostand anzeigen":
                        AnsiConsole.MarkupLine($"[yellow]Kontostand: {manager.GetBalance()} €[/]");
                        break;

                    case "Transaktionen anzeigen":
                        ShowTransactions(manager);
                        break;

                    case "Statistik anzeigen":
                        ShowStatistics(manager);
                        break;

                    case "Beenden":
                        running = false;
                        break;
                }
            }
            else
            {
                switch (choice)
                {
                    case "Ausgabe hinzufügen":
                        AddTransaction(manager, user, TransactionType.Expense);
                        break;

                    case "Meine Ausgaben anzeigen":
                        ShowTransactionsForUser(manager, user);
                        break;

                    case "Beenden":
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

        AnsiConsole.MarkupLine("\n[bold underline]Transaktionen:[/]\n");
        
        var table = new Table();

        table.AddColumn("[yellow]Typ[/]");
        table.AddColumn("[yellow]Betrag[/]");
        table.AddColumn("[yellow]Kategorie[/]");

        foreach (var t in transactions)
        {
            string typeText = t.Type == TransactionType.Income ? "Einnahme" : "Ausgabe";
            string category = manager.GetCategoryName(t.CategoryId);

            table.AddRow(
                typeText,
                $"{t.Amount} €",
                category
            );
        }

        AnsiConsole.Write(table);
    }
    static void ShowTransactionsForUser(FinanceManager manager, User user)
    {
        var transactions = manager.GetAllTransactions()
                                  .Where(t => t.UserId == user.Id);

        AnsiConsole.MarkupLine("\n[bold underline]Meine Ausgaben:[/]\n");

        var table = new Table();

        table.AddColumn("[yellow]Betrag[/]");
        table.AddColumn("[yellow]Kategorie[/]");

        foreach (var t in transactions)
        {
            table.AddRow(
                $"{t.Amount} €",
                manager.GetCategoryName(t.CategoryId)
            );
        }

        AnsiConsole.Write(table);
    }
    static void ShowStatistics(FinanceManager manager)
    {
        var stats = manager.GetExpensesByCategory();

        decimal total = stats.Values.Sum();

        Console.WriteLine("\n===============================================");
        Console.WriteLine("      AUSGABEN NACH KATEGORIEN");
        Console.WriteLine("===============================================");
        var top = stats.OrderByDescending(x => x.Value).FirstOrDefault();

        if (top.Key != null)
        {

            Console.WriteLine($"\nTop Kategorie: {top.Key} ({top.Value} €) 🔥");
            Console.WriteLine("------------------------------------------------\n");

        }
        int maxLength = stats.Keys.Max(k => k.Length);
        int maxBarLength = stats

    .Select(e => Math.Max(1, (int)((e.Value / total) * 100 * 2)))
    .Max();

        Console.WriteLine($"{"Kategorie".PadRight(maxLength + 2)}{"Verteilung".PadRight(maxBarLength + 2)} Prozent");
        Console.WriteLine(new string('-', maxLength + maxBarLength + 15));

        foreach (var entry in stats.OrderByDescending(e => e.Value))
        {
            decimal percent = total > 0 ? (entry.Value / total) * 100 : 0;


            int barLength = Math.Max(1, (int)(percent * 2));
            string bar = new string('█', barLength);


            Console.WriteLine(
               $"{entry.Key.PadRight(maxLength + 2)}" +
               $"{bar.PadRight(maxBarLength)}  " +
               $"{percent,6:F1}%\n"
             );

        }
    }
}

