using Erp.Model.BasicFiles;
using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.SupplyChain
{
    public class CL_Vrp_OutputData : RecordBaseModel
    {
        private ObservableCollection<CityData> _CityData;
        public ObservableCollection<CityData> CityData
        {
            get { return _CityData; }
            set { _CityData = value; OnPropertyChanged("CityData"); }
        }

        private ObservableCollection<VehicleData> _VehicleData;
        public ObservableCollection<VehicleData> VehicleData
        {
            get { return _VehicleData; }
            set { _VehicleData = value; OnPropertyChanged("VehicleData"); }
        }

        #region CL_VRP

        private VRP_InputData _VrpInputData_1;
        public VRP_InputData VrpInputData_1
        {
            get { return _VrpInputData_1; }
            set { _VrpInputData_1 = value; OnPropertyChanged("VrpInputData_1"); }
        }

        private CL_InputData _CLInputData_1;
        public CL_InputData CLInputData_1
        {
            get { return _CLInputData_1; }
            set { _CLInputData_1 = value; OnPropertyChanged("CLInputData_1"); }
        }

        private VRP_InputData _VrpInputData_2;
        public VRP_InputData VrpInputData_2
        {
            get { return _VrpInputData_2; }
            set { _VrpInputData_2 = value; OnPropertyChanged("VrpInputData_2"); }
        }

        private CL_InputData _CLInputData_2;
        public CL_InputData CLInputData_2
        {
            get { return _CLInputData_2; }
            set { _CLInputData_2 = value; OnPropertyChanged("CLInputData_2"); }
        }

        #endregion
    }
}
