using Erp.Model.BasicFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Manufacture
{
    public class MrpResultData
    {

            public int Id { get; set; }
            public string RowDescr { get; set; }
            public int ItemId { get; set; }

            public ItemData Item { get; set; }
            public List<float> Quantities { get; set; }

    }
}
