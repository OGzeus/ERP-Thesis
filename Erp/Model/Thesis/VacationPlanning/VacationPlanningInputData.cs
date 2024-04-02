using Erp.Model.BasicFiles;
using Erp.Model.Enums;
using Erp.Model.Interfaces;
using Erp.Model.Manufacture;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis.VacationPlanning
{
    public class VacationPlanningInputData : RecordBaseModel
    {
        private int _VPId;
        private int _MaxSatisfiedBids;
        private int _SeparValue;

        private string _VPCode;
        private string _VPDescr;
        private BasicEnums.EmployeeType _EmployeeType;
        private BasicEnums.VPLogicType _VPLogicType;
        private ReqScheduleInfoData _Schedule;
        private ObservableCollection<EmployeeData> _Employees;


        public ObservableCollection<EmployeeData> Employees
        {
            get { return _Employees; }
            set { _Employees = value; OnPropertyChanged("Employees"); }
        }
        public int VPId
        {
            get { return _VPId; }
            set { _VPId = value; OnPropertyChanged("VPId"); }
        }

        public int MaxSatisfiedBids
        {
            get { return _MaxSatisfiedBids; }
            set { _MaxSatisfiedBids = value; OnPropertyChanged("MaxSatisfiedBids"); }
        }

        public int SeparValue
        {
            get { return _SeparValue; }
            set { _SeparValue = value; OnPropertyChanged("SeparValue"); }
        }

        public string VPCode
        {
            get { return _VPCode; }
            set { _VPCode = value; OnPropertyChanged("VPCode"); }
        }

        public string VPDescr
        {
            get { return _VPDescr; }
            set { _VPDescr = value; OnPropertyChanged("VPDescr"); }
        }
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
        public ReqScheduleInfoData Schedule
        {
            get { return _Schedule; }
            set { _Schedule = value; OnPropertyChanged("Schedule"); }
        }

        #region Dictionaries/Input for Optimisation

        private int _MaxLeaveBids;
        private int _MaxNonSpecific;

        private string[] datesstr;
        private DateTime[] dates;

        public Dictionary<string, int> MaxLeaveBidsPerEmployee { get; set; } 
        public Dictionary<string, string> BidsPerEmployee { get; set; } 


        public List<(int, int)> OverlappingBids1 = new List<(int, int)>();
        public List<(int, int)> OverlappingBids2 = new List<(int, int)>();
        public Dictionary<(string, string,int), int> ZBidsDict { get; set; }
        //public Dictionary<(string, string), int> ZBidsDict { get; set; }
        public Dictionary<(string, string), int> RBidsDict { get; set; }

        public int MaxLeaveBids
        {
            get { return _MaxLeaveBids; }
            set { _MaxLeaveBids = value; OnPropertyChanged("MaxLeaveBids"); }
        }

        public int MaxNonSpecific
        {
            get { return _MaxNonSpecific; }
            set { _MaxNonSpecific = value; OnPropertyChanged("MaxNonSpecific"); }
        }
        public string[] DatesStr
        {
            get { return datesstr; }
            set { datesstr = value; OnPropertyChanged("DatesStr"); }
        }
        public DateTime[] Dates
        {
            get { return dates; }
            set { dates = value; OnPropertyChanged("Dates"); }
        }
        #endregion
    }
}
