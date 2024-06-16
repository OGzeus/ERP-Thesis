using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.SupplyChain.TSP
{
    public class AntColony_TSP_InputData:RecordBaseModel
    {


        private double _alpha;
        private double _beta;
        private double _evaporationRate;
        private double _initialPheromoneLevel;
        private int _numberOfAnts;
        private int _numberOfIterations;


        // ACO parameter: Importance of pheromone
        public double Alpha
        {
            get { return _alpha; }
            set { _alpha = value; OnPropertyChanged("Alpha"); }
        }

        // ACO parameter: Importance of heuristic information (inverse of distance)
        public double Beta
        {
            get { return _beta; }
            set { _beta = value; OnPropertyChanged("Beta"); }
        }

        // ACO parameter: Pheromone evaporation rate
        public double EvaporationRate
        {
            get { return _evaporationRate; }
            set { _evaporationRate = value; OnPropertyChanged("EvaporationRate"); }
        }

        // ACO parameter: Initial pheromone level on each path
        public double InitialPheromoneLevel
        {
            get { return _initialPheromoneLevel; }
            set { _initialPheromoneLevel = value; OnPropertyChanged("InitialPheromoneLevel"); }
        }

        // Number of ants in the colony
        public int NumberOfAnts
        {
            get { return _numberOfAnts; }
            set { _numberOfAnts = value; OnPropertyChanged("NumberOfAnts"); }
        }

        // Number of iterations to run the ACO algorithm
        public int NumberOfIterations
        {
            get { return _numberOfIterations; }
            set { _numberOfIterations = value; OnPropertyChanged("NumberOfIterations"); }
        }
    }
}
