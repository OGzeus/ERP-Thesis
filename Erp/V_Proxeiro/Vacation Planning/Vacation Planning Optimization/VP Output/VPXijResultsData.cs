using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.V_Proxeiro.Vacation_Planning.VP_Output
{
    public class VPXijResultsData
    {
        #region General
        public EmployeeData Employee { get; set; }
        public string Xij { get; set; }
        public string Date { get; set; }
        public double XijFlag { get; set; } // To vlepoume
        public bool GrantedBidFlag { get; set; } //Na ginei  Granted sto Original

        #endregion

        #region Extra
        public string EmpCode { get; set; }
        public List<string> Dates { get; set; }

        #endregion

    }

}
