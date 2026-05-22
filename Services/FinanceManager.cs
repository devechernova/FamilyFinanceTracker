using System.Text.Json;
using System.Text.Json.Serialization;
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
    string json = File.ReadAllText("Data/users.json");

    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
    options.Converters.Add(new JsonStringEnumConverter());

    users = JsonSerializer.Deserialize<List<User>>(json, options)!;
}

private void LoadCategories()
    {
        string json = File.ReadAllText("Data/categories.json");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        categories = JsonSerializer.Deserialize<List<Category>>(json, options)!;
    }

private void LoadTransactions()
{
    if (!File.Exists("Data/transactions.json"))
    {
        transactions = new List<Transaction>();
        return;
    }

    string json = File.ReadAllText("Data/transactions.json");

    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    transactions = JsonSerializer.Deserialize<List<Transaction>>(json, options)
                   ?? new List<Transaction>();
}


private void SaveTransactions()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(transactions, options);

        File.WriteAllText("Data/transactions.json", json);
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


    public List<Category> GetCategories()
    {
        return categories;
    }

    public List<Transaction> GetAllTransactions()
    {
        return transactions;
    }
}

