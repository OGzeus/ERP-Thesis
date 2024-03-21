using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis
{
    public class LeaveBidRowData:RecordBaseModel
    {
        public int RowId { get; set; }
        public LeaveBidsDataStatic LeaveBid { get; set; }
        public int EmpId { get; set; }
        public int ScheduleId { get; set; }
        public DateTime Date { get; set; }
        public string DateStr { get; set; }
    }
}
