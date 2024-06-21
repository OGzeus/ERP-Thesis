using Erp.Model.Thesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.V_Proxeiro.Vacation_Planning.VP_Output
{
    public class VPYijResultsData
    {
        #region General
        public EmployeeData Employee { get; set; }
        public LeaveBidsDataStatic LeaveBidData { get; set; }

        #region Borei kai na fugei Giati uparxei mesa sto LeaveBid h plhroforia
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int NumberOfDays { get; set; }
        public int NumberOfDaysMin { get; set; }
        public int NumberOfDaysMax { get; set; }
        #endregion

        public string Yij { get; set; }
        public string Rijr { get; set; }
        public string Yijrz { get; set; }
        public string Date { get; set; }
        public double YijFlag { get; set; }  

        #endregion


        #region Extra
        public string EmpCode { get; set; }
        public string LeaveBidCode { get; set; }
        public List<string> Dates { get; set; }
        public string DateFromStr { get; set; }
        public string DateToStr { get; set; }

        #endregion




    }

}
