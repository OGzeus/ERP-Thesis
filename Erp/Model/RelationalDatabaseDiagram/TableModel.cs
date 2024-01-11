using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.RelationalDatabaseDiagram
{
    public class TableModel
    {
        public string TableName { get; set; }
        public ObservableCollection<ColumnModel> Columns { get; set; }
        // Add these properties
        public double X { get; set; }
        public double Y { get; set; }

        public TableModel()
        {
            Columns = new ObservableCollection<ColumnModel>();
        }
    }

}