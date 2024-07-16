
using Erp.Model.Interfaces;
using System;
using System.Linq;

namespace Erp.Model.Thesis.CrewScheduling
{
    public class FlightRoutesData : RecordBaseModel
    {
        public int FlightRouteId { get; set; }
        public string Code { get; set; }
        public string Descr { get; set; }
        public AirportData Airport { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public float TotalTime { get; set; }
        public float FlightTime { get; set; }
        public float GroundTime { get; set; }
        public int Complement_Captain { get; set; }
        public int Complement_FO { get; set; }
        public int Complement_Cabin_Manager { get; set; }
        public int Complement_Flight_Attendant { get; set; }
    }
}
