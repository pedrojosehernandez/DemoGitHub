using CrudEmpleados.Models;

namespace CrudEmpleados.Services;

public class CsvService
{
    private const string Header = "Id,Nombre,Apellido,Puesto,Salario";
    private readonly string _filePath;

    public CsvService(string filePath)
    {
        _filePath = filePath;
    }

    public List<Employee> LoadAll()
    {
        var employees = new List<Employee>();

        if (!File.Exists(_filePath))
            return employees;

        var lines = File.ReadAllLines(_filePath);

        foreach (var line in lines.Skip(1)) // saltar encabezado
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var employee = Employee.FromCsv(line);
            if (employee is not null)
                employees.Add(employee);
        }

        return employees;
    }

    public void SaveAll(List<Employee> employees)
    {
        var lines = new List<string> { Header };
        lines.AddRange(employees.Select(e => e.ToCsv()));
        File.WriteAllLines(_filePath, lines);
    }

    public int GetNextId(List<Employee> employees) =>
        employees.Count == 0 ? 1 : employees.Max(e => e.Id) + 1;
}
