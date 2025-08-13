using System;
using System.Collections.Generic;

// Core model using record
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// Interface for payment behavior
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// BankTransferProcessor implementation
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Bank Transfer] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

// MobileMoneyProcessor implementation
public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Mobile Money] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

// CryptoWalletProcessor implementation
public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Crypto Wallet] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

// Base Account class
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
    }
}

// Sealed SavingsAccount class
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. New balance: {Balance:C}");
        }
    }
}

// FinanceApp class
public class FinanceApp
{
    private List<Transaction> _transactions = new();

    public void Run()
    {
        var account = new SavingsAccount("ACC12345", 1000m);

        var t1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
        var t2 = new Transaction(2, DateTime.Now, 200m, "Utilities");
        var t3 = new Transaction(3, DateTime.Now, 120m, "Entertainment");

        ITransactionProcessor mobileMoney = new MobileMoneyProcessor();
        ITransactionProcessor bankTransfer = new BankTransferProcessor();
        ITransactionProcessor cryptoWallet = new CryptoWalletProcessor();

        mobileMoney.Process(t1);
        account.ApplyTransaction(t1);
        _transactions.Add(t1);

        bankTransfer.Process(t2);
        account.ApplyTransaction(t2);
        _transactions.Add(t2);

        cryptoWallet.Process(t3);
        account.ApplyTransaction(t3);
        _transactions.Add(t3);
    }
}

// Main entry point
public class Program
{
    public static void Main()
    {
        var app = new FinanceApp();
        app.Run();
    }
}
