using Erp.Model.BasicFiles;
using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis
{
    public class AirportData : RecordBaseModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Descr { get; set; }

        public CityData City { get; set; }
    }
}
