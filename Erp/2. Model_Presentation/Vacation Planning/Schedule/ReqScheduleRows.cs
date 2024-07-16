using Erp.Model.Enums;
using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.V_Proxeiro.Schedule
{
    public class ReqScheduleRowsData : RecordBaseModel
    {
        public int ReqId { get; set; }
        public string ReqCode { get; set; }
        public BasicEnums.EmployeeType Position { get; set; }
        public DateTime Date { get; set; }
        public int LimitLine { get; set; }

    }
}
