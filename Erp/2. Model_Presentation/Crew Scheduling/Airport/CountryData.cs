using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.V_Proxeiro
{
    public class CountryData : RecordBaseModel
    {
        public int CountryId { get; set; }
        public string CountryCode { get; set; }
        public string CountryDescr { get; set; }
    }

}
