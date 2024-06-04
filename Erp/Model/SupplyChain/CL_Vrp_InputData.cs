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

        private ObservableCollection<Cluster_Vehicles_Data > _CL_VehicleData1;
        public ObservableCollection<Cluster_Vehicles_Data> CL_VehicleData1
        {
            get { return _CL_VehicleData1; }
            set { _CL_VehicleData1 = value; OnPropertyChanged("CL_VehicleData1"); }
        }
        private ObservableCollection<Cluster_Vehicles_Data> _CL_VehicleData2;
        public ObservableCollection<Cluster_Vehicles_Data> CL_VehicleData2
        {
            get { return _CL_VehicleData2; }
            set { _CL_VehicleData2 = value; OnPropertyChanged("CL_VehicleData1"); }
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

        private VRP_InputData _VrpInputData_1;
        public VRP_InputData VrpInputData_1
        {
            get { return _VrpInputData_1; }
            set { _VrpInputData_1 = value; OnPropertyChanged("VrpInputData_1"); }
        }
        private VRP_InputData _VrpInputData_2;
        public VRP_InputData VrpInputData_2
        {
            get { return _VrpInputData_2; }
            set { _VrpInputData_2 = value; OnPropertyChanged("VrpInputData_2"); }
        }

        private CL_InputData _CLInputData_1;
        public CL_InputData CLInputData_1
        {
            get { return _CLInputData_1; }
            set { _CLInputData_1 = value; OnPropertyChanged("CLInputData_1"); }
        }


        private CL_InputData _CLInputData_2;
        public CL_InputData CLInputData_2
        {
            get { return _CLInputData_2; }
            set { _CLInputData_2 = value; OnPropertyChanged("CLInputData_2"); }
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
