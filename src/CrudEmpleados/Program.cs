using CrudEmpleados.Models;
using CrudEmpleados.Services;

var csvPath = Path.Combine(AppContext.BaseDirectory, "empleados.csv");
var csvService = new CsvService(csvPath);
var running = true;

while (running)
{
    Console.Clear();
    Console.WriteLine("╔══════════════════════════════════════╗");
    Console.WriteLine("║   CRUD DE EMPLEADOS - CSV            ║");
    Console.WriteLine("╠══════════════════════════════════════╣");
    Console.WriteLine("║  1. Listar empleados                 ║");
    Console.WriteLine("║  2. Buscar empleado por Id           ║");
    Console.WriteLine("║  3. Crear empleado                   ║");
    Console.WriteLine("║  4. Actualizar empleado              ║");
    Console.WriteLine("║  5. Eliminar empleado                ║");
    Console.WriteLine("║  0. Salir                            ║");
    Console.WriteLine("╚══════════════════════════════════════╝");
    Console.Write("\nSeleccione una opción: ");

    var option = Console.ReadLine();

    switch (option)
    {
        case "1":
            ListarEmpleados();
            break;
        case "2":
            BuscarEmpleado();
            break;
        case "3":
            CrearEmpleado();
            break;
        case "4":
            ActualizarEmpleado();
            break;
        case "5":
            EliminarEmpleado();
            break;
        case "0":
            running = false;
            Console.WriteLine("\n¡Hasta luego!");
            break;
        default:
            Console.WriteLine("\nOpción no válida.");
            Pausar();
            break;
    }
}

void ListarEmpleados()
{
    var employees = csvService.LoadAll();
    Console.Clear();
    Console.WriteLine("\n=== LISTADO DE EMPLEADOS ===");

    if (employees.Count == 0)
    {
        Console.WriteLine("No hay empleados registrados.");
    }
    else
    {
        PrintHeader();
        foreach (var emp in employees)
            Console.WriteLine(emp);
        PrintFooter();
    }

    Pausar();
}

void BuscarEmpleado()
{
    Console.Clear();
    Console.WriteLine("\n=== BUSCAR EMPLEADO ===");
    Console.Write("Ingrese el Id: ");

    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("Id no válido.");
        Pausar();
        return;
    }

    var employees = csvService.LoadAll();
    var emp = employees.FirstOrDefault(e => e.Id == id);

    if (emp is null)
    {
        Console.WriteLine($"No se encontró el empleado con Id {id}.");
    }
    else
    {
        PrintHeader();
        Console.WriteLine(emp);
        PrintFooter();
    }

    Pausar();
}

void CrearEmpleado()
{
    Console.Clear();
    Console.WriteLine("\n=== CREAR EMPLEADO ===");

    var employees = csvService.LoadAll();

    var nombre = LeerTexto("Nombre");
    var apellido = LeerTexto("Apellido");
    var puesto = LeerTexto("Puesto");
    var salario = LeerDecimal("Salario");

    var employee = new Employee
    {
        Id = csvService.GetNextId(employees),
        Nombre = nombre,
        Apellido = apellido,
        Puesto = puesto,
        Salario = salario
    };

    employees.Add(employee);
    csvService.SaveAll(employees);

    Console.WriteLine($"\nEmpleado creado con Id {employee.Id}.");
    Pausar();
}

void ActualizarEmpleado()
{
    Console.Clear();
    Console.WriteLine("\n=== ACTUALIZAR EMPLEADO ===");
    Console.Write("Ingrese el Id del empleado a actualizar: ");

    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("Id no válido.");
        Pausar();
        return;
    }

    var employees = csvService.LoadAll();
    var emp = employees.FirstOrDefault(e => e.Id == id);

    if (emp is null)
    {
        Console.WriteLine($"No se encontró el empleado con Id {id}.");
        Pausar();
        return;
    }

    Console.WriteLine($"Datos actuales: {emp}");
    Console.WriteLine("(Deje vacío para mantener el valor actual)\n");

    var nombre = LeerTextoOpcional("Nombre", emp.Nombre);
    var apellido = LeerTextoOpcional("Apellido", emp.Apellido);
    var puesto = LeerTextoOpcional("Puesto", emp.Puesto);
    var salario = LeerDecimalOpcional("Salario", emp.Salario);

    emp.Nombre = nombre;
    emp.Apellido = apellido;
    emp.Puesto = puesto;
    emp.Salario = salario;

    csvService.SaveAll(employees);
    Console.WriteLine("\nEmpleado actualizado correctamente.");
    Pausar();
}

void EliminarEmpleado()
{
    Console.Clear();
    Console.WriteLine("\n=== ELIMINAR EMPLEADO ===");
    Console.Write("Ingrese el Id del empleado a eliminar: ");

    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("Id no válido.");
        Pausar();
        return;
    }

    var employees = csvService.LoadAll();
    var emp = employees.FirstOrDefault(e => e.Id == id);

    if (emp is null)
    {
        Console.WriteLine($"No se encontró el empleado con Id {id}.");
        Pausar();
        return;
    }

    Console.WriteLine($"¿Eliminar a {emp.Nombre} {emp.Apellido}? (s/n): ");
    var confirm = Console.ReadLine()?.Trim().ToLower();

    if (confirm == "s")
    {
        employees.Remove(emp);
        csvService.SaveAll(employees);
        Console.WriteLine("Empleado eliminado.");
    }
    else
    {
        Console.WriteLine("Operación cancelada.");
    }

    Pausar();
}

// --- Utilidades ---

string LeerTexto(string campo)
{
    string? valor;
    do
    {
        Console.Write($"{campo}: ");
        valor = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(valor))
            Console.WriteLine($"  {campo} es obligatorio.");
    } while (string.IsNullOrWhiteSpace(valor));

    return valor;
}

string LeerTextoOpcional(string campo, string actual)
{
    Console.Write($"{campo} [{actual}]: ");
    var valor = Console.ReadLine()?.Trim();
    return string.IsNullOrWhiteSpace(valor) ? actual : valor;
}

decimal LeerDecimal(string campo)
{
    decimal valor;
    do
    {
        Console.Write($"{campo}: ");
        if (decimal.TryParse(Console.ReadLine(), out valor) && valor >= 0)
            break;
        Console.WriteLine($"  {campo} debe ser un número válido >= 0.");
    } while (true);

    return valor;
}

decimal LeerDecimalOpcional(string campo, decimal actual)
{
    Console.Write($"{campo} [{actual}]: ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) return actual;
    return decimal.TryParse(input, out var valor) && valor >= 0 ? valor : actual;
}

void PrintHeader()
{
    Console.WriteLine(new string('-', 79));
    Console.WriteLine($"| {"Id",-5} | {"Nombre",-15} | {"Apellido",-15} | {"Puesto",-20} | {"Salario",12} |");
    Console.WriteLine(new string('-', 79));
}

void PrintFooter()
{
    Console.WriteLine(new string('-', 79));
}

void Pausar()
{
    Console.WriteLine("\nPresione cualquier tecla para continuar...");
    Console.ReadKey(true);
}
