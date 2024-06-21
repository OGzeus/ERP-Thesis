using Erp.Model.Enums;
using Erp.Model.Interfaces;
using Erp.Model.Thesis.VacationPlanning;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.V_Proxeiro.Vacation_Planning
{
    public class VacationPlanningOutputData : RecordBaseModel
    {
        #region Optimization Data Output
        public double ObjValue { get; set; }
        public ObservableCollection<VPYijResultsData> VPYijResultsDataGrid { get; set; }
        public ObservableCollection<VPYijResultsData> VPRijzResultsDataGrid { get; set; }
        public ObservableCollection<VPXijResultsData> VPXijResultsDataGrid { get; set; }
        public ObservableCollection<VPXiResultData> VPXiResultsDataGrid { get; set; }

        #endregion

        #region Extra
        public BasicEnums.EmployeeType EmployeeType { get; set; }
        public BasicEnums.VPLogicType VPLogicType { get; set; }

        public string[] Dates { get; set; }
        public ObservableCollection<EmployeeData> EmpLeaveStatusData { get; set; }
        #endregion
    }

}
