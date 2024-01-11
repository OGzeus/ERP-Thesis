using Erp.Model.BasicFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Inventory.InvControl_TimeVaryingDemand
{
    public class TimeVaryingInvData
    {
        public ItemData Item { get; set; }
        public int Day { get; set; }

        public double Demand { get; set; }
        public double QuantityOrdered { get; set; }
        public double InventoryLevel { get; set; }
        public double IsOrdering { get; set; }
        public double SetupCost { get; set; }
        public double HoldingCost { get; set; }
        public double TotalCost { get; set; }
    }
}
