using Erp.Model.Interfaces;
using Erp.Model.Thesis.VacationPlanning;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.V_Proxeiro.Vacation_Planning.Vacation_Planning_Optimization
{
    public class VPCGInputData : RecordBaseModel
    {
        #region General
        public string[] Dates { get; set; }
        public ObservableCollection<VPXiResultData> VPXiResultsDataGrid { get; set; }

        #endregion

        #region Dictionaries

        //Remaining Leave Days Per Employee
        //Dictionary<Employee,Remaining_Leave_Days>
        public Dictionary<int, int> LeaveDays { get; set; }

        //Remaining Limit Line Per Date
        //Dictionary<Date,Remaining_Limit_Line>
        public Dictionary<int, int> LLiDict { get; set; }

        #endregion
    }


}
