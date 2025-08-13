using System;
using System.Collections.Generic;
using System.IO;

// Student class
public class Student
{
    public int Id { get; }
    public string FullName { get; }
    public int Score { get; }

    public Student(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70 && Score <= 79) return "B";
        if (Score >= 60 && Score <= 69) return "C";
        if (Score >= 50 && Score <= 59) return "D";
        return "F";
    }
}

// Custom exceptions
public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

// StudentResultProcessor class
public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var students = new List<Student>();
        using var reader = new StreamReader(inputFilePath);
        string? line;
        int lineNumber = 0;
        while ((line = reader.ReadLine()) != null)
        {
            lineNumber++;
            var parts = line.Split(',');
            if (parts.Length != 3)
                throw new MissingFieldException($"Line {lineNumber}: Missing fields.");
            if (!int.TryParse(parts[0].Trim(), out int id))
                throw new MissingFieldException($"Line {lineNumber}: Invalid ID format.");
            var fullName = parts[1].Trim();
            if (!int.TryParse(parts[2].Trim(), out int score))
                throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid score format.");
            students.Add(new Student(id, fullName, score));
        }
        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using var writer = new StreamWriter(outputFilePath);
        foreach (var student in students)
        {
            writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
        }
    }
}

// Main entry point
public class Program
{
    public static void Main()
    {
        var processor = new StudentResultProcessor();
        string inputFile = "students.txt";
        string outputFile = "report.txt";
        try
        {
            var students = processor.ReadStudentsFromFile(inputFile);
            processor.WriteReportToFile(students, outputFile);
            Console.WriteLine($"Report written to {outputFile}");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Input file not found.");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"InvalidScoreFormatException: {ex.Message}");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"MissingFieldException: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}
