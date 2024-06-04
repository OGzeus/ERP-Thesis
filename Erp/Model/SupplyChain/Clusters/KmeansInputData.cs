using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Erp.Model.SupplyChain.Clusters
{
    public class KmeansInputData : RecordBaseModel
    {
        // The number of clusters (K)
        private int _numberOfClusters;
        public int NumberOfClusters
        {
            get { return _numberOfClusters; }
            set { _numberOfClusters = value; OnPropertyChanged("NumberOfClusters"); }
        }

        // The maximum number of iterations for the algorithm
        private int _maxIterations;
        public int MaxIterations
        {
            get { return _maxIterations; }
            set { _maxIterations = value; OnPropertyChanged("MaxIterations"); }
        }

        // The tolerance for convergence
        private double _tolerance;
        public double Tolerance
        {
            get { return _tolerance; }
            set { _tolerance = value; OnPropertyChanged("Tolerance"); }
        }

        // The method used for initializing centroids (e.g., Random, K-means++)
        private string _initializationMethod;
        public string InitializationMethod
        {
            get { return _initializationMethod; }
            set { _initializationMethod = value; OnPropertyChanged("InitializationMethod"); }
        }

        // A boolean flag to enable verbose output
        private bool _verboseMode;
        public bool VerboseMode
        {
            get { return _verboseMode; }
            set { _verboseMode = value; OnPropertyChanged("VerboseMode"); }
        }

        // The seed for random number generation to ensure reproducibility
        private int _randomSeed;
        public int RandomSeed
        {
            get { return _randomSeed; }
            set { _randomSeed = value; OnPropertyChanged("RandomSeed"); }
        }

        // The distance metric used for clustering (e.g., Euclidean, Manhattan)
        private string _distanceMetric;
        public string DistanceMetric
        {
            get { return _distanceMetric; }
            set { _distanceMetric = value; OnPropertyChanged("DistanceMetric"); }
        }

        // The criterion for convergence (e.g., Change in Centroids, Inertia)
        private string _convergenceCriterion;
        public string ConvergenceCriterion
        {
            get { return _convergenceCriterion; }
            set { _convergenceCriterion = value; OnPropertyChanged("ConvergenceCriterion"); }
        }

        // Option to specify initial centroids for the clusters
        private string _initialCentroids;
        public string InitialCentroids
        {
            get { return _initialCentroids; }
            set { _initialCentroids = value; OnPropertyChanged("InitialCentroids"); }
        }

        // The number of times the algorithm runs to choose the best result
        private int _numberOfRuns;
        public int NumberOfRuns
        {
            get { return _numberOfRuns; }
            set { _numberOfRuns = value; OnPropertyChanged("NumberOfRuns"); }
        }

        // A boolean flag to indicate if data scaling (normalization/standardization) should be applied
        private bool _dataScaling;
        public bool DataScaling
        {
            get { return _dataScaling; }
            set { _dataScaling = value; OnPropertyChanged("DataScaling"); }
        }


    }
}
