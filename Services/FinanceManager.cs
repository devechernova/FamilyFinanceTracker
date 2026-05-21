using System;

using FamilyFinanceTracker.Models;

namespace FamilyFinanceTracker.Services;

public class FinanceManager
{
    private List<User> users = new List<User>();
    private List<Transaction> transactions = new List<Transaction>();

    public FinanceManager()
    {
        // тестовые пользователи
        users.Add(new User(1, "Mama", Role.Parent));
        users.Add(new User(2, "Papa", Role.Parent));
        users.Add(new User(3, "Kind", Role.Child));
    }

    public User? LoginUser(string name)
    {
        foreach (var user in users)
        {
            if (user.Name.ToLower() == name.ToLower())
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

