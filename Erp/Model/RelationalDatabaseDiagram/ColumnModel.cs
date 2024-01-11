using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.RelationalDatabaseDiagram
{
    public class ColumnModel
    {
        public string ColumnName { get; set; }
        public bool IsForeignKey { get; set; }
        public string ReferenceTableName { get; set; }
    }
}
