using FamilyFinanceTracker.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

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

    // ✅EIN EINHEITLICHES VERFAHREN FÜR WEGE
    private string GetDataPath(string fileName)
    {
        return Path.GetFullPath(Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..",
            "Data",
            fileName
        ));
    }

    // ✅ USERS
    private void LoadUsers()
    {
        string path = GetDataPath("users.json");

        Console.WriteLine($"DEBUG USERS PATH: {path}");

        if (!File.Exists(path))
        {
            Console.WriteLine("DEBUG: users.json NOT FOUND ❌");
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

        Console.WriteLine($"DEBUG users count: {users.Count} ✅");
    }

    // ✅ CATEGORIES
    private void LoadCategories()
    {
        string path = GetDataPath("categories.json");

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

    // ✅ TRANSACTIONS
    private void LoadTransactions()
    {
        string path = GetDataPath("transactions.json");

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

            string path = GetDataPath("transactions.json");

            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fehler beim Speichern der Transaktionen!");
            Console.WriteLine(ex.Message);
        }
    }

    // ✅ LOGIN
    public User? LoginUser(string name)
    {
        Console.WriteLine($"DEBUG users count: {users.Count}");

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

    // ✅ ADD
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
