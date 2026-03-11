namespace CrudEmpleados.Models;

public class Employee
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Puesto { get; set; } = string.Empty;
    public decimal Salario { get; set; }

    public override string ToString() =>
        $"| {Id,-5} | {Nombre,-15} | {Apellido,-15} | {Puesto,-20} | {Salario,12:C} |";

    public string ToCsv() =>
        $"{Id},{Nombre},{Apellido},{Puesto},{Salario}";

    public static Employee? FromCsv(string line)
    {
        var parts = line.Split(',');
        if (parts.Length < 5) return null;

        if (!int.TryParse(parts[0], out var id)) return null;
        if (!decimal.TryParse(parts[4], out var salario)) return null;

        return new Employee
        {
            Id = id,
            Nombre = parts[1],
            Apellido = parts[2],
            Puesto = parts[3],
            Salario = salario
        };
    }
}
