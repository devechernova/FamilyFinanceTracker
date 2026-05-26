# FamilyFinanceTracker

A small C# console application for managing a family budget. Parents and
children share a household balance: parents record income and expenses and
see the full picture, while children can only log their own expenses.

The application UI is in **German**. This README is in English so the project
is approachable to a wider audience — every German label is translated below.

## Features

- **Role-based access** — `Parent` users can record income, expenses, view
  the running balance, list all transactions, and see category statistics.
  `Child` users can only add their own expenses and review them.
- **Persistent storage** — users, categories, and transactions live in
  human-readable JSON files under `Data/`. No database required.
- **Category statistics** — expenses are aggregated by category, sorted by
  amount, and shown as an ASCII bar chart with percentages.
- **UTF-8 console output** — German umlauts and emoji render correctly on
  modern terminals.

## Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or newer.
- A terminal that supports UTF-8 (any modern Linux/macOS terminal, or
  Windows Terminal / PowerShell 7+).

Verify with:

```bash
dotnet --version   # should report 9.0.x
```

## Running

The same commands work on Linux, macOS, and Windows.

```bash
cd FamilyFinanceTracker
dotnet run
```

On first launch you will be prompted for a username (`Benutzername
eingeben:`). The bundled `Data/users.json` defines three default accounts:

| Name   | Role   | Permissions                                |
|--------|--------|--------------------------------------------|
| Mama   | Parent | Full menu (income, expenses, stats, etc.)  |
| Papa   | Parent | Full menu                                  |
| Kind   | Child  | Add own expenses, view own expenses        |

## Menu

### Parent menu

| Key | German label               | Action                                   |
|----:|----------------------------|------------------------------------------|
| 1   | Einnahme hinzufügen        | Add income                               |
| 2   | Ausgabe hinzufügen         | Add expense (pick a category)            |
| 3   | Kontostand anzeigen        | Show current balance                     |
| 4   | Transaktionen anzeigen     | List all transactions                    |
| 5   | Statistik anzeigen         | Show expenses-by-category bar chart      |
| 6   | Beenden                    | Exit                                     |

### Child menu

| Key | German label               | Action                                   |
|----:|----------------------------|------------------------------------------|
| 1   | Ausgabe hinzufügen         | Add an expense                           |
| 2   | Meine Ausgaben anzeigen    | List own expenses                        |
| 3   | Beenden                    | Exit                                     |

The screen clears between menu iterations; after each action the program
waits for a key press (`Weiter mit beliebiger Taste...`) so you can read
the output before the menu redraws.

## Project layout

```
FamilyFinanceTracker/
├── Program.cs                   Entry point, menu, user-facing flow
├── FamilyFinanceTracker.csproj  .NET 9 project file
├── Models/
│   ├── User.cs                  User + Role enum (Parent / Child)
│   ├── Transaction.cs           Transaction + TransactionType enum
│   └── Category.cs              Category definition
├── Services/
│   └── FinanceManager.cs        Loads/saves JSON, business logic
└── Data/
    ├── users.json               Seed users
    ├── categories.json          Expense categories
    └── transactions.json        Persisted transactions (auto-updated)
```

The `Data/*.json` files are copied to the build output on each build
(`<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>`), so the
running binary reads from `bin/Debug/net9.0/Data/`. Edit the source files
in `Data/` and rebuild to seed new users or categories.

## Data files

`users.json` — list of accounts:

```json
[
  { "Id": 1, "Name": "Mama", "Role": "Parent" },
  { "Id": 2, "Name": "Papa", "Role": "Parent" },
  { "Id": 3, "Name": "Kind", "Role": "Child" }
]
```

`categories.json` — expense buckets (e.g. `Lebensmittel`, `Miete`,
`Transport (ÖPNV)`). New entries are picked up on next launch.

`transactions.json` — appended automatically when a transaction is added;
IDs are assigned sequentially.

## Notes

- Income is recorded without a category; expenses always require one.
- Balance is computed on the fly as `Σ income − Σ expense` across all
  users in the household.
- The login lookup is case-insensitive, but the name must match exactly
  otherwise (no fuzzy match).
