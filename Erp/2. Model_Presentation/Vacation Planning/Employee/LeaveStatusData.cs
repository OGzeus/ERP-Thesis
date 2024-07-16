using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.V_Proxeiro.EmployeeScreen
{
    public class LeaveStatusData : RecordBaseModel
    {
        #region General
        public EmployeeData Employee { get; set; }
        public int Total { get; set; }
        public int Used { get; set; }
        public int CurrentBalance
        {
            get { return Total - Used; } // Computed value based on Total and Used
        }
        #endregion

        #region Extra
        public int ProjectedBalance { get; set; }

        #endregion
    }
}
