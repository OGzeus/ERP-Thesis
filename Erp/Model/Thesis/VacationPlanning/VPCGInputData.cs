using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis.VacationPlanning
{
    public class VPCGInputData : RecordBaseModel
    {
        private string[] dates;
        public string[] Dates
        {
            get { return dates; }
            set { dates = value; OnPropertyChanged("Dates"); }
        }


        private ObservableCollection<VPXiResultData> _VPXiResultsDataGrid;

        public ObservableCollection<VPXiResultData> VPXiResultsDataGrid
        {
            get { return _VPXiResultsDataGrid; }
            set
            {
                _VPXiResultsDataGrid = value;
                OnPropertyChanged(nameof(VPXiResultsDataGrid));
            }
        }

        #region VP Col_Gen Input

        #region Indexes 
        public int T { get; set; }  //Planning Horizon
        public int I { get; set; }  //Number Of Employees 


        #endregion

        #region Dictionaries
        //Dictionary<Employee, List<VacationPlans>> 
        public Dictionary<int, int> Re_Dict { get; set; } //Remaining Leave Days Per Employee
     
        //Dictionary<Employee, Remaining Limit Line> 
        public Dictionary<int, int> RLLt_Dict { get; set; } // Remaining Limit Line Per Day

        //Dictionary<Employee, List<VacationPlans>> 
        public Dictionary<int, List<int>> Ri_Dict { get; set; } // Vacations Plans per Emplyoee

        //Dictionary<(Employee, Vacation Plan),Re cost> 
        public Dictionary<(int, int), double> Cij_Dict { get; set; } // Residual Entiltement Cost 

        //Dictionary<(Employee, Vacation Plan ,Day),0 or 1 > 
        public Dictionary<(int, int, int), int> Aijt_Dict { get; set; } //Days per VP plan per Employee
        #endregion
        #endregion

    }
}
