using Erp.Model.Enums;
using Erp.Model.Interfaces;
using Erp.Model.SupplyChain.TSP;
using Erp.Model.SupplyChain.VRP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Erp.Model.Enums.BasicEnums;

namespace Erp.Model.SupplyChain
{
    public class TSP_InputData:RecordBaseModel
    {
        #region TSP Techniques
        private BasicEnums.TSP_Techniques _TSP_Enum;

        public BasicEnums.TSP_Techniques TSP_Enum
        {
            get { return _TSP_Enum; }
            set
            {
                _TSP_Enum = value;
                UpdateActivePanel();
                OnPropertyChanged("TSP_Enum");
            }
        }

        private AntColony_TSP_InputData _AntColony_InputData;
        public AntColony_TSP_InputData AntColony_InputData 
        {
            get { return _AntColony_InputData; }
            set { _AntColony_InputData = value; OnPropertyChanged("AntColony_InputData"); }
        }
        private SAnnealing_TSP_InputData _SAnnealing_InputData;
        public SAnnealing_TSP_InputData SAnnealing_InputData
        {
            get { return _SAnnealing_InputData; }
            set { _SAnnealing_InputData = value; OnPropertyChanged("SAnnealing_InputData"); }
        }
        #endregion

        #region Dictionaries

        #region
        public Dictionary<string, int> City_IndexMap { get; set; }
        public Dictionary<int, (double, double)> Coords { get; set; }
        #endregion

        #endregion

        #region Panel Changes
        public enum TSP_ActivePanel
        {
            Simulation_Annealing,
            Ant_Colony_Optimization

        }

        private TSP_Techniques _activePanel_TSP;
        public TSP_Techniques CurrentActivePanel_TSP
        {
            get { return _activePanel_TSP; }
            private set
            {
                if (_activePanel_TSP != value)
                {
                    _activePanel_TSP = value;
                    OnPropertyChanged("CurrentActivePanel_TSP");
                }
            }
        }
        private void UpdateActivePanel()
        {
            if (TSP_Enum == BasicEnums.TSP_Techniques.Simulation_Annealing)
            {
                CurrentActivePanel_TSP = (TSP_Techniques)TSP_ActivePanel.Simulation_Annealing;
            }
            else if (TSP_Enum == BasicEnums.TSP_Techniques.Ant_Colony_Optimization)
            {
                CurrentActivePanel_TSP = (TSP_Techniques)TSP_ActivePanel.Ant_Colony_Optimization;

            }

        }
        #endregion
    }
}
