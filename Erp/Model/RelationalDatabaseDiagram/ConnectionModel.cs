using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.RelationalDatabaseDiagram
{
    public class ConnectionModel
    {
        public string SourceTableName { get; set; }
        public string TargetTableName { get; set; }
    }
}
