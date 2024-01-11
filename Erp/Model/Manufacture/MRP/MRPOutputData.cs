using Erp.Model.Interfaces;
using Erp.Model.Manufacture.MPS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Manufacture.MRP
{
    public class MRPOutputData : RecordBaseModel
    {
        private double _ObjValue;

        public double ObjValue
        {
            get { return _ObjValue; }
            set
            {
                _ObjValue = value;
                OnPropertyChanged(nameof(ObjValue));
            }
        }

        private ObservableCollection<MPSOptResultsData> _MPSOptResultsData;

        public ObservableCollection<MPSOptResultsData> MPSOptResultsData
        {
            get { return _MPSOptResultsData; }
            set
            {
                _MPSOptResultsData = value;
                OnPropertyChanged(nameof(MPSOptResultsData));
            }
        }
        private ObservableCollection<MachRepairResultsData> _MachRepairResultsData;

        public ObservableCollection<MachRepairResultsData> MachRepairResultsData
        {
            get { return _MachRepairResultsData; }
            set
            {
                _MachRepairResultsData = value;
                OnPropertyChanged(nameof(MachRepairResultsData));
            }
        }

        private string[] dates;
        public string[] Dates
        {
            get { return dates; }
            set { dates = value; OnPropertyChanged("Dates"); }
        }

    }
}
