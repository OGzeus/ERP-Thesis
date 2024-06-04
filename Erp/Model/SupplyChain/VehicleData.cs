using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.SupplyChain
{
    public class VehicleData:RecordBaseModel
    {
        private int _Id;
        private string _Code;
        private string _Descr;
        private double _Capacity;
        private int _NumberOfVehicles;

        public int Id
        {
            get { return _Id; }
            set { _Id = value; OnPropertyChanged("Id"); }
        }
        public string Code
        {
            get { return _Code; }
            set { _Code = value; OnPropertyChanged("Code"); }
        }
        public string Descr
        {
            get { return _Descr; }
            set { _Descr = value; OnPropertyChanged("Descr"); }
        }
        public double Capacity
        {
            get { return _Capacity; }
            set { _Capacity = value; OnPropertyChanged("Capacity"); }
        }
        public int NumberOfVehicles
        {
            get { return _NumberOfVehicles; }
            set { _NumberOfVehicles = value; OnPropertyChanged("NumberOfVehicles"); }
        }
        private bool _Selected { get; set; }
        public bool Selected
        {
            get { return _Selected; }
            set { _Selected = value; OnPropertyChanged("Selected"); }
        }
    }
}
