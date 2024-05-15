using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Manufacture 
{
    public class WorkcenterData : RecordBaseModel
    {
        private int _WorkID;
        private string _WorkCode;
        private string _WorkDescr;
        private int _Capacity;

        public int WorkID
        {
            get { return _WorkID; }
            set { _WorkID = value; OnPropertyChanged("WorkID"); }
        }
        public string WorkCode
        {
            get { return _WorkCode; }
            set { _WorkCode = value; OnPropertyChanged("WorkCode"); }
        }

        public string WorkDescr
        {
            get { return _WorkDescr; }
            set { _WorkDescr = value; OnPropertyChanged("WorkDescr"); }
        }
        public int Capacity
        {
            get { return _Capacity; }
            set { _Capacity = value; OnPropertyChanged("Capacity"); }
        }
    }
}
