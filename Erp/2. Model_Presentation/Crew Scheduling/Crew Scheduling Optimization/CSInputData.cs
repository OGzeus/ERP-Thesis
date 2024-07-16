using Erp.Model.BasicFiles;
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
using Erp.V_Proxeiro;

namespace Erp.DataBasePresenation
{
    public class CSInputData : RecordBaseModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Descr { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public BasicEnums.EmployeeType Position { get; set; }
        public int RoutesPenalty { get; set; }
        public int BoundsPenalty { get; set; }
        public ObservableCollection<FlightRoutesData> FlightRoutesData { get; set; }
        public ObservableCollection<EmployeeData> Employees { get; set; }
    }

        #region Crew Scheduling Input
        #region Indexes
        public int T { get; set; }  //Planning Horizon
        public int I { get; set; }  //Number Of Employees 
        public int F { get; set; }  //Number Of Routes 

        #endregion

        #region Dictionaries 
        public Dictionary<int, DateTime> DatesIndexMap { get; set; }
        public Dictionary<int, string> EmployeesIndexMap { get; set; }

        //Dictionary<Employee, List<Rosters>> 
        public Dictionary<int, List<int>> Ri { get; set; }

        //Dictionary<(Employee, Roster),Hours Penalty> 
        public Dictionary<(int, int), double> Cij_Hours { get; set; }

        //Dictionary<(Employee, Roster,Route),0 or 1 > 
        public Dictionary<(int, int, int), int> Aijf { get; set; }

        #endregion

        #region  Routes Support Dictionaries
        public Dictionary<int, string> RoutesIndexMap { get; set; }
        public Dictionary<int, (DateTime, DateTime)> RoutesDates_Dict { get; set; } //Dictionary<Route, (StartDate, EndDate)>
        public Dictionary<int, (int, int)> RoutesDay_Dict { get; set; } //Dictionary<Route, (StartDay, EndDay)>
        public Dictionary<int, (int, int)> RoutesTime_Dict { get; set; } //Dictionary<Route, (StartTime, EndTime)> 
        public Dictionary<int, int> RoutesCompl_Dict { get; set; } //Dictionary<Route, Complement> 

        public Dictionary<int, (double, double)> EmpBounds_Dict { get; set; } //Dictionary<Employee, (LowerBound, UpperBound)>
        #endregion
        #endregion
  
}
