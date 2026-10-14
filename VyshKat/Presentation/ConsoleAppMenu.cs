namespace VyshKat.Presentation;

using VyshKat.Application;
using VyshKat.Domain.Abstractions;
using VyshKat.Domain.Gear;
using VyshKat.Domain.Vehicles;

public class ConsoleAppMenu
{
    private readonly FleetService _fleetService;
    private readonly FleetReportService _reportService;

    public ConsoleAppMenu(FleetService fleetService, FleetReportService reportService)
    {
        _fleetService = fleetService ?? throw new ArgumentNullException(nameof(fleetService));
        _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
    }

    public void Run()
    {
        var exit = false;

        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("=== VyshKat Fleet Management System ===");
            Console.WriteLine("1. Accept a device into the fleet");
            Console.WriteLine("2. Show all fleet items (name and inventory number)");
            Console.WriteLine("3. Report: item count by type");
            Console.WriteLine("4. Report: total daily energy consumption");
            Console.WriteLine("5. Report: vehicles suitable for novices (rating >= 6)");
            Console.WriteLine("0. Exit");
            Console.Write("\nSelect an option: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1": AcceptDevice(); break;
                case "2": ShowAllItems(); break;
                case "3": ShowCountReport(); break;
                case "4": ShowEnergyReport(); break;
                case "5": ShowNoviceReport(); break;
                case "0": exit = true; break;
                default:
                    Pause("Invalid input.");
                    break;
            }
        }
    }

    // ---------- Accept a device ----------

    private void AcceptDevice()
    {
        Console.Clear();
        Console.WriteLine("Select the device type:");
        Console.WriteLine("1. Scooter");
        Console.WriteLine("2. Bicycle");
        Console.WriteLine("3. Electric bicycle");
        Console.WriteLine("4. Helmet");
        Console.WriteLine("5. Docking station");
        Console.WriteLine("6. Charging cabinet");
        Console.Write("\nType: ");

        var typeChoice = Console.ReadLine();

        var invNumber = ReadNonEmpty("Inventory number: ");
        if (invNumber is null) return;

        var name = ReadNonEmpty("Name: ");
        if (name is null) return;

        IInventoryItem? item = typeChoice switch
        {
            "1" => BuildScooter(invNumber, name),
            "2" => BuildBicycle(invNumber, name),
            "3" => BuildElectricBicycle(invNumber, name),
            "4" => new Helmet(invNumber, name),
            "5" => new DockingStation(invNumber, name),
            "6" => BuildChargingCabinet(invNumber, name),
            _   => null
        };

        if (item is null)
        {
            Pause("Unknown device type.");
            return;
        }

        var accepted = _fleetService.TryAccept(item);
        Pause(accepted
            ? "Device accepted into the fleet."
            : "Device failed the inspection or the inventory number is already taken.");
    }

    private Scooter? BuildScooter(string invNumber, string name)
    {
        var rating = ReadRating();
        if (rating is null) return null;

        return new Scooter(invNumber, name, rating.Value);
    }

    private Bicycle? BuildBicycle(string invNumber, string name)
    {
        var rating = ReadRating();
        if (rating is null) return null;

        return new Bicycle(invNumber, name, rating.Value);
    }

    private ElectricBicycle? BuildElectricBicycle(string invNumber, string name)
    {
        var rating = ReadRating();
        if (rating is null) return null;

        var energy = ReadEnergy();
        if (energy is null) return null;

        return new ElectricBicycle(invNumber, name, rating.Value, energy.Value);
    }

    private ChargingCabinet? BuildChargingCabinet(string invNumber, string name)
    {
        var energy = ReadEnergy();
        if (energy is null) return null;

        return new ChargingCabinet(invNumber, name, energy.Value);
    }

    // ---------- Reports ----------

    private void ShowAllItems()
    {
        Console.Clear();
        Console.WriteLine("=== All fleet items ===");

        var items = _fleetService.GetAllItems().ToList();
        Console.WriteLine($"Total items: {items.Count}");
        Console.WriteLine();

        foreach (var item in items)
        {
            Console.WriteLine($"[{item.GetType().Name}] {item.Name} — inv. no. {item.InventoryNumber}");
        }

        Pause();
    }

    private void ShowCountReport()
    {
        Console.Clear();
        Console.WriteLine("=== Item count by type ===");

        var counts = _reportService.GetItemsCountByType();
        foreach (var pair in counts)
        {
            Console.WriteLine($"{pair.Key}: {pair.Value}");
        }

        Pause();
    }

    private void ShowEnergyReport()
    {
        Console.Clear();
        Console.WriteLine("=== Daily energy consumption ===");

        var consumers = _reportService.GetEnergyConsumers().ToList();
        if (consumers.Count == 0)
        {
            Pause("No energy-consuming devices in the fleet.");
            return;
        }

        foreach (var consumer in consumers)
        {
            var label = consumer is IInventoryItem inv
                ? $"{inv.Name} (inv. no. {inv.InventoryNumber})"
                : consumer.GetType().Name;

            Console.WriteLine($"{label}: {consumer.DailyEnergyConsumptionKwh} kWh/day");
        }

        var total = _reportService.CalculateTotalDailyEnergyConsumption();
        Console.WriteLine();
        Console.WriteLine($"TOTAL: {total} kWh/day");

        Pause();
    }

    private void ShowNoviceReport()
    {
        Console.Clear();
        Console.WriteLine("=== Vehicles suitable for novices (rating >= 6) ===");

        var items = _reportService.GetNoviceFriendlyItems().ToList();
        if (items.Count == 0)
        {
            Pause("No suitable vehicles found.");
            return;
        }

        foreach (var item in items)
        {
            var label = item is IInventoryItem inv
                ? $"{inv.Name} (inv. no. {inv.InventoryNumber})"
                : item.GetType().Name;

            Console.WriteLine($"{label}, rating: {item.NoviceRating}");
        }

        Pause();
    }

    // ---------- Input helpers ----------

    private static string? ReadNonEmpty(string prompt)
    {
        Console.Write(prompt);
        var value = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(value))
        {
            Pause("Value cannot be empty.");
            return null;
        }
        return value;
    }

    private static int? ReadRating()
    {
        Console.Write("Novice rating (1-10): ");
        if (!int.TryParse(Console.ReadLine(), out var rating) || rating is < 1 or > 10)
        {
            Pause("Rating must be an integer between 1 and 10.");
            return null;
        }
        return rating;
    }

    private static decimal? ReadEnergy()
    {
        Console.Write("Daily energy consumption, kWh: ");
        if (!decimal.TryParse(Console.ReadLine(), out var energy) || energy < 0)
        {
            Pause("Energy consumption must be a non-negative number.");
            return null;
        }
        return energy;
    }

    private static void Pause(string? message = null)
    {
        if (message is not null)
        {
            Console.WriteLine();
            Console.WriteLine(message);
        }
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }
}