using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Marker interface for logging
public interface IInventoryEntity
{
    int Id { get; }
}

// Immutable InventoryItem record
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// Generic Inventory Logger
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new();
    private string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item) => _log.Add(item);
    public List<T> GetAll() => new List<T>(_log);

    public void SaveToFile()
    {
        try
        {
            var json = JsonSerializer.Serialize(_log);
            using var writer = new StreamWriter(_filePath);
            writer.Write(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath)) return;
            using var reader = new StreamReader(_filePath);
            var json = reader.ReadToEnd();
            var items = JsonSerializer.Deserialize<List<T>>(json);
            if (items != null)
                _log = items;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from file: {ex.Message}");
        }
    }
}

// Integration Layer – InventoryApp
public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger = new("inventory.json");

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Desk Chair", 25, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Monitor", 15, DateTime.Now));
        _logger.Add(new InventoryItem(4, "Keyboard", 30, DateTime.Now));
        _logger.Add(new InventoryItem(5, "Mouse", 50, DateTime.Now));
    }

    public void SaveData() => _logger.SaveToFile();
    public void LoadData() => _logger.LoadFromFile();

    public void PrintAllItems()
    {
        foreach (var item in _logger.GetAll())
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded:yyyy-MM-dd}");
        }
    }
}

// Main entry point
public class Program
{
    public static void Main()
    {
        var app = new InventoryApp();
        app.SeedSampleData();
        app.SaveData();
        // Simulate new session
        app = new InventoryApp();
        app.LoadData();
        app.PrintAllItems();
    }
}
