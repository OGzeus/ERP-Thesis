using Erp.Model.Enums;
using Erp.Model.Interfaces;
using Erp.Model.Manufacture.MPS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis.VacationPlanning
{
    public class VacationPlanningOutputData : RecordBaseModel
    {
        private BasicEnums.EmployeeType _EmployeeType;
        private BasicEnums.VPLogicType _VPLogicType;
        public BasicEnums.EmployeeType EmployeeType
        {
            get { return _EmployeeType; }
            set { _EmployeeType = value; OnPropertyChanged("EmployeeType"); }
        }
        public BasicEnums.VPLogicType VPLogicType
        {
            get { return _VPLogicType; }
            set { _VPLogicType = value; OnPropertyChanged("VPLogicType"); }
        }

        private double _ObjValue;

        public double ObjValue
        {
            get { return _ObjValue; }
            set
            {
                _ObjValue = value;
                OnPropertyChanged(nameof(ObjValue));
            }
        }

        private ObservableCollection<VPYijResultsData> _VPYijResultsDataGrid;

        public ObservableCollection<VPYijResultsData> VPYijResultsDataGrid
        {
            get { return _VPYijResultsDataGrid; }
            set
            {
                _VPYijResultsDataGrid = value;
                OnPropertyChanged(nameof(VPYijResultsDataGrid));
            }
        }

        private ObservableCollection<VPYijResultsData> _VPYijzResultsDataGrid;

        public ObservableCollection<VPYijResultsData> VPYijzResultsDataGrid 
        {
            get { return _VPYijzResultsDataGrid; }
            set
            {
                _VPYijzResultsDataGrid = value;
                OnPropertyChanged(nameof(VPYijzResultsDataGrid));
            }
        }
        private ObservableCollection<VPXijResultsData> _VPXijResultsDataGrid;

        public ObservableCollection<VPXijResultsData> VPXijResultsDataGrid
        {
            get { return _VPXijResultsDataGrid; }
            set
            {
                _VPXijResultsDataGrid = value;
                OnPropertyChanged(nameof(VPXijResultsDataGrid));
            }
        }

        private string[] dates;
        public string[] Dates
        {
            get { return dates; }
            set { dates = value; OnPropertyChanged("Dates"); }
        }

        private ObservableCollection<EmployeeData> _EmpLeaveStatusData;

        public ObservableCollection<EmployeeData> EmpLeaveStatusData
        {
            get { return _EmpLeaveStatusData; }
            set
            {
                _EmpLeaveStatusData = value;
                OnPropertyChanged(nameof(EmpLeaveStatusData));
            }
        }


    }
}
