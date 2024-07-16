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

        private ObservableCollection<VPYijResultsData> _VPYijrzResultsDataGrid;

        public ObservableCollection<VPYijResultsData> VPYijrzResultsDataGrid
        {
            get { return _VPYijrzResultsDataGrid; }
            set
            {
                _VPYijrzResultsDataGrid = value;
                OnPropertyChanged(nameof(VPYijrzResultsDataGrid));
            }
        }
        private ObservableCollection<VPXijResultsData> _VPXitResultsDataGrid;

        public ObservableCollection<VPXijResultsData> VPXitResultsDataGrid
        {
            get { return _VPXitResultsDataGrid; }
            set
            {
                _VPXitResultsDataGrid = value;
                OnPropertyChanged(nameof(VPXitResultsDataGrid));
            }
        }
        private ObservableCollection<VPXiResultData> _VPLLiResultsDataGrid;

        public ObservableCollection<VPXiResultData> VPLLiResultsDataGrid
        {
            get { return _VPLLiResultsDataGrid; }
            set
            {
                _VPLLiResultsDataGrid = value;
                OnPropertyChanged(nameof(VPLLiResultsDataGrid));
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
