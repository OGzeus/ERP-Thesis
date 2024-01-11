using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Data_Analytics
{
    public class SalesDashBoardData
    {
        public class PieChartData
        {
            public string Item { get; set; }
            public double Profit { get; set; }
        }

        public class BarChartData
        {
            public string City { get; set; }
            public int Orders { get; set; }
            public double Profit { get; set; }
        }
    }
}
