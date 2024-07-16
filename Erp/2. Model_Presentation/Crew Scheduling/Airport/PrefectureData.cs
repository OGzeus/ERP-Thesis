using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Erp.Model.Enums.BasicEnums;

namespace Erp.V_Proxeiro
{
    public class PrefectureData : RecordBaseModel
    {
        public int PrefId { get; set; }
        public int CountryId { get; set; }
        public string PrefCode { get; set; }
        public string PrefDescr { get; set; }
        public string CountryCode { get; set; }
        public CountryData Country { get; set; }
    }
}
