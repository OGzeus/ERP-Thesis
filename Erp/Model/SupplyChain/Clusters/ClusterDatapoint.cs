using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.SupplyChain.Clusters
{
    public class ClusterDatapoint : RecordBaseModel
    {
        private string _ClusterCode;

        private double longitude;
        private double latitude;

        private int _NumberOfPoints;
        public string ClusterCode
        {
            get { return _ClusterCode; }
            set { _ClusterCode = value; OnPropertyChanged("ClusterCode"); }
        }

        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; OnPropertyChanged("Longitude"); }
        }
        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; OnPropertyChanged("Latitude"); }
        }
        public int NumberOfPoints
        {
            get { return _NumberOfPoints; }
            set { _NumberOfPoints = value; OnPropertyChanged("NumberOfPoints"); }
        }

        private bool _IsSelected{ get; set; }

        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; INotifyPropertyChanged("IsSelected"); }
        }

        private double _DaviesBouldinIndex { get; set; }
        public double DaviesBouldinIndex
        {
            get { return _DaviesBouldinIndex; }
            set { _DaviesBouldinIndex = value; INotifyPropertyChanged("DaviesBouldinIndex"); }
        }
    }
}
