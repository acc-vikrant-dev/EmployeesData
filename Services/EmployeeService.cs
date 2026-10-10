using EmployeeConsoleSonarDemo.Models;

namespace EmployeeConsoleSonarDemo.Services;

public sealed class EmployeeService(EmployeeJsonStore store)
{
    public IReadOnlyList<Employee> GetAll() =>
        store.Load().OrderBy(employee => employee.Id).ToList();

    public Employee? GetById(int id) =>
        store.Load().FirstOrDefault(employee => employee.Id == id);

    public Employee Add(Employee employee)
    {
        Validate(employee);
        var employees = store.Load();
        employee.Id = employees.Count == 0 ? 1 : employees.Max(item => item.Id) + 1;
        employees.Add(employee);
        store.Save(employees);
        return employee;
    }

    public bool Update(Employee employee)
    {
        Validate(employee);
        var employees = store.Load();
        var index = employees.FindIndex(item => item.Id == employee.Id);
        if (index < 0)
            return false;

        employees[index] = employee;
        store.Save(employees);
        return true;
    }

    public bool Delete(int id)
    {
        var employees = store.Load();
        var employee = employees.FirstOrDefault(item => item.Id == id);
        if (employee is null)
            return false;

        employees.Remove(employee);
        store.Save(employees);
        return true;
    }

    private static void Validate(Employee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);

        if (string.IsNullOrWhiteSpace(employee.Name))
            throw new ArgumentException("Employee name is required.");
        if (string.IsNullOrWhiteSpace(employee.Department))
            throw new ArgumentException("Department is required.");
        if (string.IsNullOrWhiteSpace(employee.Email) ||
            !employee.Email.Contains('@', StringComparison.Ordinal) ||
            employee.Email.StartsWith('@') ||
            employee.Email.EndsWith('@'))
            throw new ArgumentException("A valid email address is required.");
        if (employee.Salary < 0)
            throw new ArgumentException("Salary cannot be negative.");
    }
}
