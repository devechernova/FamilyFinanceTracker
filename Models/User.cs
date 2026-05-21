using System;

namespace FamilyFinanceTracker.Models;

public enum Role
{
    Parent,
    Child
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public Role Role { get; set; }

    public User(int id, string name, Role role)
    {
        Id = id;
        Name = name;
        Role = role;
    }
}