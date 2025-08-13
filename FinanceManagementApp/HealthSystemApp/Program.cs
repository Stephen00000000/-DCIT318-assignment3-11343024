using System;
using System.Collections.Generic;
using System.Linq;

// Generic repository for entity management
public class Repository<T>
{
    private List<T> items = new();

    public void Add(T item) => items.Add(item);
    public List<T> GetAll() => new List<T>(items);
    public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);
    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item != null)
        {
            items.Remove(item);
            return true;
        }
        return false;
    }
}

// Patient class
public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }
}

// Prescription class
public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }
}

// HealthSystemApp class
public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new();
    private Repository<Prescription> _prescriptionRepo = new();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new();

    public void SeedData()
    {
        _patientRepo.Add(new Patient(1, "Alice Smith", 30, "Female"));
        _patientRepo.Add(new Patient(2, "Bob Johnson", 45, "Male"));
        _patientRepo.Add(new Patient(3, "Carol Lee", 28, "Female"));

        _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", DateTime.Now.AddDays(-10)));
        _prescriptionRepo.Add(new Prescription(2, 1, "Ibuprofen", DateTime.Now.AddDays(-5)));
        _prescriptionRepo.Add(new Prescription(3, 2, "Paracetamol", DateTime.Now.AddDays(-2)));
        _prescriptionRepo.Add(new Prescription(4, 3, "Cetirizine", DateTime.Now.AddDays(-1)));
        _prescriptionRepo.Add(new Prescription(5, 2, "Metformin", DateTime.Now));
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap.Clear();
        foreach (var prescription in _prescriptionRepo.GetAll())
        {
            if (!_prescriptionMap.ContainsKey(prescription.PatientId))
                _prescriptionMap[prescription.PatientId] = new List<Prescription>();
            _prescriptionMap[prescription.PatientId].Add(prescription);
        }
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("All Patients:");
        foreach (var patient in _patientRepo.GetAll())
        {
            Console.WriteLine($"ID: {patient.Id}, Name: {patient.Name}, Age: {patient.Age}, Gender: {patient.Gender}");
        }
    }

    public List<Prescription> GetPrescriptionsByPatientId(int patientId)
    {
        return _prescriptionMap.TryGetValue(patientId, out var prescriptions) ? prescriptions : new List<Prescription>();
    }

    public void PrintPrescriptionsForPatient(int id)
    {
        var prescriptions = GetPrescriptionsByPatientId(id);
        if (prescriptions.Count == 0)
        {
            Console.WriteLine($"No prescriptions found for patient ID {id}.");
            return;
        }
        Console.WriteLine($"Prescriptions for patient ID {id}:");
        foreach (var p in prescriptions)
        {
            Console.WriteLine($"Prescription ID: {p.Id}, Medication: {p.MedicationName}, Date Issued: {p.DateIssued:yyyy-MM-dd}");
        }
    }
}

// Main entry point
public class Program
{
    public static void Main()
    {
        var app = new HealthSystemApp();
        app.SeedData();
        app.BuildPrescriptionMap();
        app.PrintAllPatients();
        Console.WriteLine();
        // Select patient ID 2 for demonstration
        app.PrintPrescriptionsForPatient(2);
    }
}
