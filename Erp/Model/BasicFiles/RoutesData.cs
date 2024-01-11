using Erp.Model.Customers;
using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.BasicFiles
{
    public class RoutesData : RecordBaseModel
    {
        private int _RoutesId { get; set; }


        private CityData _CityFrom { get; set; }
        private CityData _CityTo { get; set; }

        private float _Distance { get; set; }
        private int _AvgSpeed { get; set; }

        public int RoutesId
        {
            get { return _RoutesId; }
            set { _RoutesId = value; INotifyPropertyChanged("RoutesId"); }
        }


        public CityData CityFrom
        {
            get { return _CityFrom; }
            set { _CityFrom = value; INotifyPropertyChanged("CityFrom"); }
        }
        public CityData CityTo
        {
            get { return _CityTo; }
            set { _CityTo = value; INotifyPropertyChanged("CityTo"); }
        }

        public float Distance
        {
            get { return _Distance; }
            set { _Distance = value; INotifyPropertyChanged("Distance"); }
        }

        public int AvgSpeed
        {
            get { return _AvgSpeed; }
            set { _AvgSpeed = value; INotifyPropertyChanged("AvgSpeed"); }
        }

    }
}
