using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class VrpVisViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Customer> Customers { get; set; }
    public ObservableCollection<Warehouse> Warehouses { get; set; }
    public Factory Factory { get; set; }
    public ObservableCollection<ArcData> Arcs { get; set; }

    public VrpVisViewModel()
    {
        // Load data
        LoadData();
    }

    private void LoadData()
    {
        // Load customer, warehouse, factory, and arc data here
        Customers = new ObservableCollection<Customer>
        {
            new Customer { X = 22.94, Y = 39.37, Name = "Company A", OrderSize = 10, ProductType = "Cement Type 1" },
            new Customer { X = 21.77, Y = 39.56, Name = "Company B", OrderSize = 8, ProductType = "Cement Type 2" },
            new Customer { X = 22.11, Y = 37.04, Name = "Company C", OrderSize = 6, ProductType = "Cement Type 1" },
            new Customer { X = 22.42, Y = 37.08, Name = "Company D", OrderSize = 12, ProductType = "Cement Type 2" },
        };

        Warehouses = new ObservableCollection<Warehouse>
        {
            new Warehouse { X = 22.42, Y = 39.64, Name = "Warehouse 1" },
            new Warehouse { X = 22.37, Y = 37.52, Name = "Warehouse 2" },
        };

        Factory = new Factory { X = 23.73, Y = 37.98, Name = "Factory" };

        Arcs = new ObservableCollection<ArcData>
        {
            new ArcData { Start = "Factory", End = "Warehouse 1", Distance = 355, AvgSpeed = 80, OilConsumptionBigTruck = 0.35, OilConsumptionSmallTruck = 0.25 },
            new ArcData { Start = "Factory", End = "Warehouse 2", Distance = 169, AvgSpeed = 80, OilConsumptionBigTruck = 0.35, OilConsumptionSmallTruck = 0.25 },
            new ArcData { Start = "Warehouse 1", End = "Company A", Distance = 58, AvgSpeed = 60, OilConsumptionBigTruck = 0.35, OilConsumptionSmallTruck = 0.25 },
            new ArcData { Start = "Warehouse 1", End = "Company B", Distance = 68, AvgSpeed = 60, OilConsumptionBigTruck = 0.35, OilConsumptionSmallTruck = 0.25 },
            new ArcData { Start = "Warehouse 2", End = "Company C", Distance = 125, AvgSpeed = 60, OilConsumptionBigTruck = 0.35, OilConsumptionSmallTruck = 0.25 },
            new ArcData { Start = "Warehouse 2", End = "Company D", Distance = 54, AvgSpeed = 60, OilConsumptionBigTruck = 0.35, OilConsumptionSmallTruck = 0.25 },
        };

        OnPropertyChanged(nameof(Customers));
        OnPropertyChanged(nameof(Warehouses));
        OnPropertyChanged(nameof(Factory));
        OnPropertyChanged(nameof(Arcs));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class Location
{
    public double X { get; set; }
    public double Y { get; set; }
    public string Name { get; set; }
}

public class Customer : Location
{
    public int OrderSize { get; set; }
    public string ProductType { get; set; }
}

public class Warehouse : Location
{
}

public class Factory : Location
{
}

public class ArcData
{
    public string Start { get; set; }
    public string End { get; set; }
    public double Distance { get; set; }
    public double AvgSpeed { get; set; }
    public double OilConsumptionBigTruck { get; set; }
    public double OilConsumptionSmallTruck { get; set; }
}