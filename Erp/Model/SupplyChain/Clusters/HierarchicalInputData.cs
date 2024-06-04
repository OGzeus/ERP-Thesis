using Erp.Model.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Erp.Model.SupplyChain.Clusters
{
    public class HierarchicalInputData : RecordBaseModel
    {
        // The method used to calculate the distance between clusters (e.g., Single, Complete, Average, Ward)
        private string _linkageMethod;
        public string LinkageMethod
        {
            get { return _linkageMethod; }
            set { _linkageMethod = value; OnPropertyChanged("LinkageMethod"); }
        }

        // The distance metric used for clustering (e.g., Euclidean, Manhattan)
        private string _distanceMetric;
        public string DistanceMetric
        {
            get { return _distanceMetric; }
            set { _distanceMetric = value; OnPropertyChanged("DistanceMetric"); }
        }

        // The maximum number of clusters to form
        private int _maxClusters;
        public int MaxClusters
        {
            get { return _maxClusters; }
            set { _maxClusters = value; OnPropertyChanged("MaxClusters"); }
        }

        // The threshold to apply when forming flat clusters
        private double _distanceThreshold;
        public double DistanceThreshold
        {
            get { return _distanceThreshold; }
            set { _distanceThreshold = value; OnPropertyChanged("DistanceThreshold"); }
        }

        // A boolean flag to enable verbose output
        private bool _verboseMode;
        public bool VerboseMode
        {
            get { return _verboseMode; }
            set { _verboseMode = value; OnPropertyChanged("VerboseMode"); }
        }

        // Option to specify if data should be standardized before clustering
        private bool _dataStandardization;
        public bool DataStandardization
        {
            get { return _dataStandardization; }
            set { _dataStandardization = value; OnPropertyChanged("DataStandardization"); }
        }

        // The seed for random number generation to ensure reproducibility
        private int _randomSeed;
        public int RandomSeed
        {
            get { return _randomSeed; }
            set { _randomSeed = value; OnPropertyChanged("RandomSeed"); }
        }
    }
}

