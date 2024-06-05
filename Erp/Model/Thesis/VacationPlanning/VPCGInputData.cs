using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis.VacationPlanning
{
    public class VPCGInputData : RecordBaseModel
    {
        private string[] dates;
        public string[] Dates
        {
            get { return dates; }
            set { dates = value; OnPropertyChanged("Dates"); }
        }


        private ObservableCollection<VPXiResultData> _VPXiResultsDataGrid;

        public ObservableCollection<VPXiResultData> VPXiResultsDataGrid
        {
            get { return _VPXiResultsDataGrid; }
            set
            {
                _VPXiResultsDataGrid = value;
                OnPropertyChanged(nameof(VPXiResultsDataGrid));
            }
        }
        public Dictionary<int, int> LeaveDays { get; set; } //Remaining Leave Days Per Employee
        public Dictionary<int, int> LLiDict { get; set; } // Remaining Limit Line Per Day

    }
}
