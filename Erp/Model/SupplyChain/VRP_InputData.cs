using Erp.Model.Enums;
using Erp.Model.Interfaces;
using Erp.Model.SupplyChain.Clusters;
using Erp.Model.SupplyChain.VRP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Erp.Model.Enums.BasicEnums;

namespace Erp.Model.SupplyChain
{
    public class VRP_InputData : RecordBaseModel
    {
        #region VRP Technqiues
        private BasicEnums.VRP_Techniques _VRP_Enum;

        public BasicEnums.VRP_Techniques VRP_Enum
        {
            get { return _VRP_Enum; }
            set
            {
                _VRP_Enum = value;
                UpdateActivePanel();
                OnPropertyChanged("VRP_Enum");
            }
        }

        private SimAnnealing_InputData _SimAnnealing_InputDataInputData;
        public SimAnnealing_InputData SimAnnealing_InputData //allagh
        {
            get { return _SimAnnealing_InputDataInputData; }
            set { _SimAnnealing_InputDataInputData = value; OnPropertyChanged("SimAnnealing_InputData"); }
        }

        #endregion

        #region Dictionaries

        #region Vehicles Dictionaries
        public Dictionary<string, int> Vehicles_IndexMap { get; set; }
        public Dictionary<int, double> VehicleCapacity { get; set; }

        #endregion

        #region Customer Dictionaries

        // Dictionary Customers IndexMap
        public Dictionary<string, int> Customer_IndexMap { get; set; }

        // Dictionary for customer demands (key: customer index, value: demand)
        public Dictionary<int, double> Demand { get; set; }

        // Dictionary for customer coordinates (key: customer index, value: coordinate tuple)
        public Dictionary<int, (double, double)> Coords { get; set; }

        #endregion

        #region Warehouse Dictionaries

        // Dictionary to map warehouse names to their indices
        public Dictionary<string, int> Depot_IndexMap { get; set; }

        // Dictionary for warehouse capacities (key: warehouse index, value: capacity)
        public Dictionary<int, double> DepotCapacity { get; set; }

        // Dictionary for warehouse coordinates (key: warehouse index, value: coordinate tuple)
        public Dictionary<int, (double, double)> DepotCoords { get; set; }

        #endregion

        #endregion
        #region Panel Changes
        public enum VRP_ActivePanel
        {
            Simulation_Annealing,
            Optimization,
            Tabu_Search,
            Ant_Colony

        }

        private VRP_Techniques _activePanel_VRP;
        public VRP_Techniques CurrentActivePanel_VRP
        {
            get { return _activePanel_VRP; }
            private set
            {
                if (_activePanel_VRP != value)
                {
                    _activePanel_VRP = value;
                    OnPropertyChanged("CurrentActivePanel_VRP"); 
                }
            }
        }
        private void UpdateActivePanel()
        {
            if (VRP_Enum == BasicEnums.VRP_Techniques.Simulation_Annealing)
            {
                CurrentActivePanel_VRP = (VRP_Techniques)VRP_ActivePanel.Simulation_Annealing;
            }
            else if (VRP_Enum == BasicEnums.VRP_Techniques.Optimization)
            {
                CurrentActivePanel_VRP = (VRP_Techniques)VRP_ActivePanel.Optimization;

            }
            else if (VRP_Enum == BasicEnums.VRP_Techniques.Tabu_Search)
            {
                CurrentActivePanel_VRP = (VRP_Techniques)VRP_ActivePanel.Tabu_Search;
            }

        }
        #endregion
    }
}
