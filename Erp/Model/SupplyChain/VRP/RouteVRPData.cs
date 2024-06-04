using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.SupplyChain.VRP
{
    public class RouteVRPData : RecordBaseModel
    {
        private int _VehicleIndex { get; set; }

        private string _VehicleCode { get; set; }

        private string _CustomerFrom{ get; set; }
        private string _CustomerTo { get; set; }
        private double _Distance { get; set; }

        public int VehicleIndex
        {
            get { return _VehicleIndex; }
            set { _VehicleIndex = value; OnPropertyChanged("VehicleIndex"); }
        }
        public string VehicleCode
        {
            get { return _VehicleCode; }
            set { _VehicleCode = value; OnPropertyChanged("VehicleCode"); }
        }

        public string CustomerFrom
        {
            get { return _CustomerFrom; }
            set { _CustomerFrom = value; OnPropertyChanged("CustomerFrom"); }
        }

        public string CustomerTo
        {
            get { return _CustomerTo; }
            set { _CustomerTo = value; OnPropertyChanged("CustomerTo"); }
        }
        public double Distance
        {
            get { return _Distance; }
            set { _Distance = value; OnPropertyChanged("Distance"); }
        }
    }
}
