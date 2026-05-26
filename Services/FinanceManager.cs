using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.IO;
using FamilyFinanceTracker.Models;

namespace FamilyFinanceTracker.Services;

public class FinanceManager
{
    private List<User> users = new List<User>();
    private List<Category> categories = new List<Category>();
    private List<Transaction> transactions = new List<Transaction>();

    public FinanceManager()
    {
        LoadUsers();
        LoadCategories();
        LoadTransactions();
    }

    private void LoadUsers()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Data", "users.json");

        if (!File.Exists(path))
        {
            users = new List<User>();
            return;
        }

        string json = File.ReadAllText(path);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        options.Converters.Add(new JsonStringEnumConverter());

        users = JsonSerializer.Deserialize<List<User>>(json, options)
                ?? new List<User>();
    }

    private void LoadCategories()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Data", "categories.json");

        if (!File.Exists(path))
        {
            categories = new List<Category>();
            return;
        }

        string json = File.ReadAllText(path);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        categories = JsonSerializer.Deserialize<List<Category>>(json, options)
                     ?? new List<Category>();
    }

    private void LoadTransactions()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Data", "transactions.json");

        try
        {
            if (!File.Exists(path))
            {
                transactions = new List<Transaction>();
                return;
            }

            string json = File.ReadAllText(path);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            transactions = JsonSerializer.Deserialize<List<Transaction>>(json, options)
                           ?? new List<Transaction>();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fehler beim Laden der Transaktionen!");
            Console.WriteLine(ex.Message);

            transactions = new List<Transaction>();
        }
    }

    private void SaveTransactions()
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(transactions, options);

            string path = Path.Combine(AppContext.BaseDirectory, "Data", "transactions.json");

            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fehler beim Speichern der Transaktionen!");
            Console.WriteLine(ex.Message);
        }
    }

    public User? LoginUser(string name)
    {
        string input = name.Trim();

        foreach (var user in users)
        {
            if (string.Equals(user.Name, input, StringComparison.OrdinalIgnoreCase))
            {
                return user;
            }
        }

        return null;
    }
    public void AddTransaction(Transaction transaction)
    {
        int newId = transactions.Any()
            ? transactions.Max(t => t.Id) + 1
            : 1;

        transaction.Id = newId;

    transactions.Add(transaction);
        SaveTransactions();
    }

    public decimal GetBalance()
    {
        decimal balance = 0;

        foreach (var t in transactions)
        {
            if (t.Type == TransactionType.Income)
                balance += t.Amount;
            else
                balance -= t.Amount;
        }

        return balance;
    }

    public string GetCategoryName(int categoryId)
    {
        if (categoryId == 0)
            return "Keine Kategorie";

        var category = categories.FirstOrDefault(c => c.Id == categoryId);

        return category != null ? category.Name : "Unbekannt";
    }


    public List<Category> GetCategories()
    {
        return categories;
    }

    public List<Transaction> GetAllTransactions()
    {
        return transactions;
    }
    public Dictionary<string, decimal> GetExpensesByCategory()
    {
        var result = new Dictionary<string, decimal>();

        // nur Ausgaben
        var expenses = transactions.Where(t => t.Type == TransactionType.Expense);

        foreach (var t in expenses)
        {
            string categoryName = GetCategoryName(t.CategoryId);

            if (!result.ContainsKey(categoryName))
            {
                result[categoryName] = 0;
            }

            result[categoryName] += t.Amount;
        }

        return result;
    }
}

