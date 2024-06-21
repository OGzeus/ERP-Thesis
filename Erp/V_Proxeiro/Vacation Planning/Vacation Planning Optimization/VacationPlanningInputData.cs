using Erp.Model.Enums;
using Erp.Model.Interfaces;
using Erp.Model.Thesis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.V_Proxeiro.Vacation_Planning
{
    public class VacationPlanningInputData : RecordBaseModel
    {
        #region General 
        public int VPId { get; set; }
        public string VPCode { get; set; }
        public string VPDescr { get; set; }
        public BasicEnums.EmployeeType EmployeeType { get; set; }
        public BasicEnums.VPLogicType VPLogicType { get; set; }
        public int MaxSatisfiedBids { get; set; }
        public int SeparValue { get; set; }
        public ReqScheduleInfoData Schedule { get; set; }
        public ObservableCollection<EmployeeData> Employees { get; set; }

        #endregion


        #region Optimization Data Input

        public DateTime[] Dates { get; set; }
        public string[] DatesStr { get; set; }


        public Dictionary<int, int> MaxLeaveBidsPerEmployee;
        public Dictionary<(int, int, int), int> ZBidsDict;
        public Dictionary<(int, int), int> RBidsDict;
        public int MaxLeaveBids { get; set; }
        public int MaxNonSpecific { get; set; }
        #endregion




    }


}
