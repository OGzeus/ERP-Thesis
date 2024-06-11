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
        private int _Id { get; set; }
        private string _Code { get; set; }
        private string _Descr { get; set; }

        private CityData _City { get; set; }

        private bool _Selected { get; set; }

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
        public CityData City
        {
            get { return _City; }
            set { _City = value; OnPropertyChanged("City"); }
        }
        public bool Selected
        {
            get { return _Selected; }
            set { _Selected = value; OnPropertyChanged("Selected"); }
        }
    }
}
