using System.Globalization;
using System.Text;
using EmployeeConsoleSonarDemo.Models;
using EmployeeConsoleSonarDemo.Services;

Console.OutputEncoding = Encoding.UTF8;

var store = new EmployeeJsonStore();
var service = new EmployeeService(store);

Console.WriteLine("=== Employee Management Console (SonarQube Demo) ===");
Console.WriteLine($"Data file: {store.DataFilePath}");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1. List employees");
    Console.WriteLine("2. View employee by ID");
    Console.WriteLine("3. Add employee");
    Console.WriteLine("4. Update employee");
    Console.WriteLine("5. Delete employee");
    Console.WriteLine("0. Exit");
    Console.Write("Choose an option: ");

    var choice = Console.ReadLine()?.Trim();
    try
    {
        switch (choice)
        {
            case "1":
                PrintEmployees(service.GetAll());
                break;
            case "2":
                ViewEmployee(service);
                break;
            case "3":
                AddEmployee(service);
                break;
            case "4":
                UpdateEmployee(service);
                break;
            case "5":
                DeleteEmployee(service);
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Invalid option. Choose 0 through 5.");
                break;
        }
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Validation error: {ex.Message}");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Operation failed: {ex.Message}");
    }
    catch (IOException ex)
    {
        Console.WriteLine($"File error: {ex.Message}");
    }
}

static void PrintEmployees(IReadOnlyList<Employee> employees)
{
    if (employees.Count == 0)
    {
        Console.WriteLine("No employees found.");
        return;
    }

    Console.WriteLine();
    Console.WriteLine($"{"ID",-5} {"Name",-24} {"Department",-18} {"Email",-32} {"Salary",12}");
    Console.WriteLine(new string('-', 95));
    foreach (var employee in employees)
    {
        Console.WriteLine($"{employee.Id,-5} {employee.Name,-24} {employee.Department,-18} {employee.Email,-32} {employee.Salary,12:C}");
    }
}

static void ViewEmployee(EmployeeService service)
{
    var id = ReadInt("Employee ID: ");
    var employee = service.GetById(id);
    Console.WriteLine(employee is null
        ? "Employee not found."
        : $"{employee.Id}: {employee.Name} | {employee.Department} | {employee.Email} | {employee.Salary.ToString("C", CultureInfo.CurrentCulture)}");
}

static void AddEmployee(EmployeeService service)
{
    var employee = new Employee
    {
        Name = ReadRequired("Name: "),
        Department = ReadRequired("Department: "),
        Email = ReadRequired("Email: "),
        Salary = ReadDecimal("Salary: ")
    };

    var created = service.Add(employee);
    Console.WriteLine($"Employee created with ID {created.Id}.");
}

static void UpdateEmployee(EmployeeService service)
{
    var id = ReadInt("Employee ID to update: ");
    var existing = service.GetById(id);
    if (existing is null)
    {
        Console.WriteLine("Employee not found.");
        return;
    }

    Console.WriteLine("Press Enter to keep the current value.");
    var name = ReadOptional($"Name [{existing.Name}]: ");
    var department = ReadOptional($"Department [{existing.Department}]: ");
    var email = ReadOptional($"Email [{existing.Email}]: ");
    var salaryText = ReadOptional($"Salary [{existing.Salary}]: ");

    existing.Name = string.IsNullOrWhiteSpace(name) ? existing.Name : name;
    existing.Department = string.IsNullOrWhiteSpace(department) ? existing.Department : department;
    existing.Email = string.IsNullOrWhiteSpace(email) ? existing.Email : email;
    if (!string.IsNullOrWhiteSpace(salaryText))
    {
        if (!decimal.TryParse(salaryText, NumberStyles.Number, CultureInfo.InvariantCulture, out var salary))
            throw new ArgumentException("Salary must be a valid number.");
        existing.Salary = salary;
    }

    service.Update(existing);
    Console.WriteLine("Employee updated.");
}

static void DeleteEmployee(EmployeeService service)
{
    var id = ReadInt("Employee ID to delete: ");
    Console.Write(service.Delete(id) ? "Employee deleted." : "Employee not found.");
    Console.WriteLine();
}

static string ReadRequired(string prompt)
{
    Console.Write(prompt);
    return Console.ReadLine() ?? string.Empty;
}

static string ReadOptional(string prompt)
{
    Console.Write(prompt);
    return Console.ReadLine() ?? string.Empty;
}

static int ReadInt(string prompt)
{
    Console.Write(prompt);
    if (!int.TryParse(Console.ReadLine(), out var value) || value <= 0)
        throw new ArgumentException("Enter a positive whole number.");
    return value;
}

static decimal ReadDecimal(string prompt)
{
    Console.Write(prompt);
    if (!decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value) || value < 0)
        throw new ArgumentException("Enter a non-negative salary, using a dot for decimals.");
    return value;
}
