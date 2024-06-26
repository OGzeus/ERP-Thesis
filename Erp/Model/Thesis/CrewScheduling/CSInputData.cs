﻿using Erp.Model.BasicFiles;
using Erp.Model.Data_Analytics.Forecast;
using Erp.Model.Interfaces;
using Erp.Model.Inventory;
using Erp.Model.Manufacture.MRP;
using Erp.Model.Manufacture;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erp.Model.Enums;
using Deedle.Internal;

namespace Erp.Model.Thesis.CrewScheduling
{
    public class CSInputData : RecordBaseModel
    {
        private int id;
        private string code;
        private string descr;

        private DateTime _DateFrom;
        private DateTime _DateTo;

        private string _DateFrom_String;
        private string _DateTo_String;
        private ObservableCollection<FlightRoutesData> _FlightRoutesData;
        private ObservableCollection<EmployeeData> _Employees;
        private BasicEnums.EmployeeType _Position { get; set; }

        private int _RoutesPenalty;
        private int _BoundsPenalty;

        public int RoutesPenalty
        {
            get { return _RoutesPenalty; }
            set { _RoutesPenalty = value; OnPropertyChanged("RoutesPenalty"); }
        }

        public int BoundsPenalty
        {
            get { return _BoundsPenalty; }
            set { _BoundsPenalty = value; OnPropertyChanged("BoundsPenalty"); }
        }

        public int Id
        {
            get { return id; }
            set { id = value; OnPropertyChanged("Id"); }
        }

        public string Code
        {
            get { return code; }
            set { code = value; OnPropertyChanged("Code"); }
        }

        public string Descr
        {
            get { return descr; }
            set { descr = value; OnPropertyChanged("Descr"); }
        }

        public DateTime DateFrom
        {
            get { return _DateFrom; }
            set { _DateFrom = value; OnPropertyChanged("DateFrom"); }
        }
        public DateTime DateTo
        {
            get { return _DateTo; }
            set { _DateTo = value; OnPropertyChanged("DateTo"); }
        }

        public string DateFrom_Str
        {
            get { return _DateFrom_String; }
            set { _DateFrom_String = value; OnPropertyChanged("DateFrom_Str"); }
        }
        public string DateTo_Str
        {
            get { return _DateTo_String; }
            set { _DateTo_String = value; OnPropertyChanged("DateTo_Str"); }
        }
        public BasicEnums.EmployeeType Position
        {
            get { return _Position; }
            set { _Position = value; OnPropertyChanged("Position"); }
        }



        public ObservableCollection<FlightRoutesData> FlightRoutesData
        {
            get { return _FlightRoutesData; }
            set { _FlightRoutesData = value; OnPropertyChanged("FlightRoutesData"); }
        }
        public ObservableCollection<EmployeeData> Employees
        {
            get { return _Employees; }
            set { _Employees = value; OnPropertyChanged("Employees"); }
        }


        #region Crew Scheduling Input
        public int T { get; set; }  //Planning Horizon
        public int I { get; set; }  //Number Of Employees N
        public int F { get; set; }  //Number Of Routes R



        public Dictionary<int, DateTime> DatesIndexMap { get; set; } 
        public Dictionary<int, string> EmployeesIndexMap { get; set; } 
        public Dictionary<int, string> RoutesIndexMap { get; set; }
        public Dictionary<int, (DateTime, DateTime)> RoutesDates_Dict { get; set; } //Dictionary<Route, (StartDate, EndDate)>
   
        public Dictionary<int, (int, int)> RoutesDay_Dict { get; set; } //Dictionary<Route, (StartDay, EndDay)>
        public Dictionary<int, (int, int)> RoutesTime_Dict { get; set; } //Dictionary<Route, (StartTime, EndTime)> 
        public Dictionary<int, (double, double)> EmpBounds_Dict { get; set; } //Dictionary<Employee, (LowerBound, UpperBound)>

        public Dictionary<int, List<int>> Ri { get; set; } //Dictionary<Employee, List<Routes>> 

        public Dictionary<(int, int), double> Cij_Hours { get; set; } //Dictionary<(Emp, Route),Cost> 

        #endregion


  


    }
}
