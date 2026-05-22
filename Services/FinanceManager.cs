using System.Text.Json.Serialization;
using FamilyFinanceTracker.Models;
using System.Text.Json;
using System.IO;

namespace FamilyFinanceTracker.Services;

public class FinanceManager
{
    private List<User> users = new List<User>();
    private List<Transaction> transactions = new List<Transaction>();
    private List<Transaction> transactions = new List<Transaction>();

    public FinanceManager()
    {
        LoadUsers();
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

    public List<Transaction> GetAllTransactions()
    {
        return transactions;
    }
}

