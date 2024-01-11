using Erp.Model.BasicFiles;
using System;
using System.ComponentModel;

namespace Erp.Model.BasicFiles
{
    public class FactoryData : INotifyPropertyChanged
    {
        private int _FactoryID;
        private int _CityID;
        private int _ProductionCapacity;
        private string _Code;
        private string _Descr;
        private CityData _City;

        public int FactoryID
        {
            get { return _FactoryID; }
            set { _FactoryID = value; OnPropertyChanged("FactoryID"); }
        }

        public int CityID
        {
            get { return _CityID; }
            set { _CityID = value; OnPropertyChanged("CityID"); }
        }

        public int ProductionCapacity
        {
            get { return _ProductionCapacity; }
            set { _ProductionCapacity = value; OnPropertyChanged("ProductionCapacity"); }
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

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}