using Erp.Model.BasicFiles;
using Erp.Model.Enums;
using Erp.Model.Interfaces;
using Erp.Model.SupplyChain.VRP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Erp.Model.Enums.BasicEnums;

namespace Erp.Model.SupplyChain
{
    public class CL_Vrp_InputData : RecordBaseModel
    {
        #region Grids

        private ObservableCollection<CityData> _CityData;
        public ObservableCollection<CityData> CityData
        {
            get { return _CityData; }
            set { _CityData = value; OnPropertyChanged("CityData"); }
        }

        private ObservableCollection<Cluster_Vehicles_Data > _CL_VehicleData;
        public ObservableCollection<Cluster_Vehicles_Data> CL_VehicleData
        {
            get { return _CL_VehicleData; }
            set { _CL_VehicleData = value; OnPropertyChanged("CL_VehicleData1"); }
        }


        private ObservableCollection<Cluster_Vehicles_Data> _CL_VehicleDataGrid;
        public ObservableCollection<Cluster_Vehicles_Data> CL_VehicleDataGrid
        {
            get { return _CL_VehicleDataGrid; }
            set { _CL_VehicleDataGrid = value; OnPropertyChanged("CL_VehicleDataGrid"); }
        }
        #endregion

        #region CL_VRP

        private bool _CVRP_Flag;
        public bool CVRP_Flag
        {
            get { return _CVRP_Flag; }
            set { _CVRP_Flag = value; OnPropertyChanged("CVRP_Flag"); }
        }
        private bool _CVRPTW_Flag;
        public bool CVRPTW_Flag
        {
            get { return _CVRPTW_Flag; }
            set { _CVRPTW_Flag = value; OnPropertyChanged("CVRPTW_Flag"); }
        }

        private VRP_InputData _VrpInputData;
        public VRP_InputData VrpInputData
        {
            get { return _VrpInputData; }
            set { _VrpInputData = value; OnPropertyChanged("VrpInputData"); }
        }


        private CL_InputData _CLInputData;
        public CL_InputData CLInputData
        {
            get { return _CLInputData; }
            set { _CLInputData = value; OnPropertyChanged("CLInputData"); }
        }


        #endregion

        private List<VehicleData> _HardCodedVehicles;
        public List<VehicleData> HardCodedVehicles
        {
            get { return _HardCodedVehicles; }
            set { _HardCodedVehicles = value; OnPropertyChanged("HardCodedVehicles"); }
        }

        #region  Bool visibility 
        private bool _StackPanelEnabled;

        public bool StackPanelEnabled
        {
            get { return _StackPanelEnabled; }
            set
            {
                if (_StackPanelEnabled != value)
                {
                    _StackPanelEnabled = value;
                    OnPropertyChanged("StackPanelEnabled");
                }
            }
        }

        #endregion


    }
}
