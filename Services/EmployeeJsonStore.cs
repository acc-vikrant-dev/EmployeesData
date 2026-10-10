using System.Text.Json;
using EmployeeConsoleSonarDemo.Models;

namespace EmployeeConsoleSonarDemo.Services;

public sealed class EmployeeJsonStore
{
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    public string DataFilePath { get; } =
        Path.Combine(AppContext.BaseDirectory, "Data", "employees.json");

    public EmployeeJsonStore()
    {
        var directory = Path.GetDirectoryName(DataFilePath)
            ?? throw new InvalidOperationException("Could not determine the data directory.");
        Directory.CreateDirectory(directory);

        if (!File.Exists(DataFilePath))
        {
            Save(CreateSeedEmployees());
        }
    }

    public List<Employee> Load()
    {
        if (!File.Exists(DataFilePath))
            return new List<Employee>();

        var json = File.ReadAllText(DataFilePath);
        return JsonSerializer.Deserialize<List<Employee>>(json, _options) ?? new List<Employee>();
    }

    public void Save(List<Employee> employees)
    {
        var directory = Path.GetDirectoryName(DataFilePath)
            ?? throw new InvalidOperationException("Could not determine the data directory.");
        Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(employees, _options);
        File.WriteAllText(DataFilePath, json);
    }

    private static List<Employee> CreateSeedEmployees() =>
    [
        new() { Id = 1, Name = "Aarav Sharma", Department = "Engineering", Email = "aarav.sharma@example.com", Salary = 85000m },
        new() { Id = 2, Name = "Mia Patel", Department = "Human Resources", Email = "mia.patel@example.com", Salary = 72000m },
        new() { Id = 3, Name = "Noah Wilson", Department = "Finance", Email = "noah.wilson@example.com", Salary = 79000m }
    ];
}
