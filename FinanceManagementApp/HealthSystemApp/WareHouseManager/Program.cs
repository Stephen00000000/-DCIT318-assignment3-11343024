using System;
using System.Collections.Generic;

// Marker interface for inventory items
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

// ElectronicItem class
public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }
}

// GroceryItem class
public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }
}

// Custom exceptions
public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

// Generic Inventory Repository
public class InventoryRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> _items = new();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out var item))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        return item;
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
    }

    public List<T> GetAllItems() => new List<T>(_items.Values);

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
            throw new InvalidQuantityException("Quantity cannot be negative.");
        var item = GetItemById(id);
        item.Quantity = newQuantity;
    }
}

// WareHouseManager class
public class WareHouseManager
{
    private InventoryRepository<ElectronicItem> _electronics = new();
    private InventoryRepository<GroceryItem> _groceries = new();

    public void SeedData()
    {
        _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
        _electronics.AddItem(new ElectronicItem(2, "Smartphone", 20, "Samsung", 12));
        _electronics.AddItem(new ElectronicItem(3, "Headphones", 15, "Sony", 18));

        _groceries.AddItem(new GroceryItem(1, "Milk", 30, DateTime.Now.AddDays(7)));
        _groceries.AddItem(new GroceryItem(2, "Bread", 25, DateTime.Now.AddDays(3)));
        _groceries.AddItem(new GroceryItem(3, "Eggs", 40, DateTime.Now.AddDays(10)));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        foreach (var item in repo.GetAllItems())
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}");
        }
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($"Stock increased for item ID {id}. New quantity: {item.Quantity}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Item ID {id} removed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public void TryOperations()
    {
        // Add duplicate item
        try
        {
            _groceries.AddItem(new GroceryItem(1, "Milk Duplicate", 10, DateTime.Now.AddDays(5)));
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"DuplicateItemException: {ex.Message}");
        }

        // Remove non-existent item
        try
        {
            _electronics.RemoveItem(99);
        }
        catch (ItemNotFoundException ex)
        {
            Console.WriteLine($"ItemNotFoundException: {ex.Message}");
        }

        // Update with invalid quantity
        try
        {
            _groceries.UpdateQuantity(2, -5);
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine($"InvalidQuantityException: {ex.Message}");
        }
    }

    public void Run()
    {
        SeedData();
        Console.WriteLine("Grocery Items:");
        PrintAllItems(_groceries);
        Console.WriteLine();
        Console.WriteLine("Electronic Items:");
        PrintAllItems(_electronics);
        Console.WriteLine();
        TryOperations();
    }
}

// Main entry point
public class Program
{
    public static void Main()
    {
        var manager = new WareHouseManager();
        manager.Run();
    }
}
