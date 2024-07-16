using Erp.Model.BasicFiles;
using Erp.Model.Interfaces;
using Erp.Model.Thesis;
using Erp.ViewModel.Thesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.V_Proxeiro
{
    public class FlightLegsData 
    {
        public int FlightLegId { get; set; }
        public string Code { get; set; }
        public string Descr { get; set; }
        public AirportData AirportDataFrom { get; set; }
        public AirportData AirportDataTo { get; set; }
        public float FlightTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }


}
