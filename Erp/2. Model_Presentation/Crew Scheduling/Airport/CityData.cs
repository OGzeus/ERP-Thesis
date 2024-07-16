using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.V_Proxeiro
{
    public class CityData : RecordBaseModel
    {
        public int CityId { get; set; }
        public int PrefId { get; set; }
        public int CountryId { get; set; }
        public string CityCode { get; set; }
        public string CityDescr { get; set; }
        public string CountryCode { get; set; }
        public string CountryDescr { get; set; }

        #region Extra
        public string PrefCode { get; set; }
        public string PrefDescr { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public int Population { get; set; }
        public int Demand { get; set; }
        public bool Selected { get; set; }
        #endregion
    }
}
