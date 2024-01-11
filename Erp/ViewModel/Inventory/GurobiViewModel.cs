using CommunityToolkit.Mvvm.Input;
using ExcelDataReader;
using IronPython.Hosting;
using IronPython.Runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Reflection;




namespace Erp.ViewModel.Inventory
{
    public class GurobiViewModel : ViewModelBase
    {


    public GurobiViewModel()
    {
        //Na ftiaxtei function wste otan epilextei to veltiso mrp me to velisto INVENTORY POLICY .
        //Na apothikevete h zhthsh twn prwtwn ulwn  ston pinaka forecast_demand.Gia tis epomenes xronikes periodous
    }

    private DataTable _data;

    public DataTable Data
    {
        get
        {
            return _data;
        }
        set
        {
            _data = value;
            RaisePropertyChanged("Data");
        }
    }

    public ICommand LoadDataCommand => new RelayCommand(() =>
    {
        var engine = Python.CreateEngine();
        var scope = engine.CreateScope();

        // Install required modules
        var pip = engine.Execute("import pip", scope);
        var main = pip.GetPythonType("main");
        main.InvokeMember(null, BindingFlags.InvokeMethod, null, pip, new object[] { new[] { "install", "numpy" } });
        main.InvokeMember(null, BindingFlags.InvokeMethod, null, pip, new object[] { new[] { "install", "pyodbc" } });
        main.InvokeMember(null, BindingFlags.InvokeMethod, null, pip, new object[] { new[] { "install", "pandas" } });
        main.InvokeMember(null, BindingFlags.InvokeMethod, null, pip, new object[] { new[] { "install", "matplotlib" } });
        main.InvokeMember(null, BindingFlags.InvokeMethod, null, pip, new object[] { new[] { "install", "gurobipy" } });

        // Import required modules
        engine.Execute("import numpy as np", scope);
        engine.Execute("import pyodbc", scope);
        engine.Execute("import pandas as pd", scope);
        engine.Execute("import matplotlib.pyplot as plt", scope);
        engine.Execute("import gurobipy as gb", scope);
        engine.Execute("from gurobipy import *", scope);

        // Call required function
        engine.ExecuteFile(@"C:\Users\npoly\Source\Repos\PythonApplication1", scope);

        Console.ReadKey();

        var table = new DataTable();

        using (var stream = File.OpenRead("InvOptimisationExcel_2023-03-11_05-16-26.xlsx"))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                table.Load(reader);
            }
        }

        Data = table;
    });


    public event PropertyChangedEventHandler PropertyChanged;

    public void RaisePropertyChanged(string propertyname)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
}
