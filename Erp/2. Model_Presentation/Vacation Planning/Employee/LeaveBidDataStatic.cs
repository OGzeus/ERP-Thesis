using Erp.Model.Enums;
using Erp.Model.Interfaces;
using Erp.Model.Thesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.V_Proxeiro.EmployeeScreen
{
    public class LeaveBidsDataStatic : RecordBaseModel
    {
        #region General
        public EmployeeData Employee { get; set; }
        public ReqScheduleInfoData Schedule { get; set; }

        public int BidId { get; set; }
        public string BidCode { get; set; }
        public int PriorityLevel { get; set; }
        public BasicEnums.BidType BidType { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int NumberOfDays { get; set; }
        public int NumberOfDaysMin { get; set; }
        public int NumberOfDaysMax { get; set; }

        #endregion

        #region CRUD Related Data
        public string OldBidCode { get; set; }
        public bool ExistingFlag { get; set; }
        public bool NewBidFlag { get; set; }
        public bool Bidflag { get; set; }
        public bool Modify { get; set; }

        #endregion


    }
}

