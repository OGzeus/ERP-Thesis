using Erp.Model.BasicFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Inventory.InvControl_TimeVaryingDemand
{
    public class TimeVaryingInvResultsData
    {
        public int Id { get; set; }
        public string RowDescr { get; set; }
        public int ItemId { get; set; }

        public ItemData Item { get; set; }
        public List<string> Values { get; set; }
    }
}
