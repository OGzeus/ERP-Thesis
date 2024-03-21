using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis
{
    public class ReqScheduleRowsData : RecordBaseModel
    {
        public int ReqId { get; set; }
        public string ReqCode { get; set; }
        public DateTime Date { get; set; }

        public string DateStr { get; set; }

        public int LimitLine { get; set; }

        // Define the PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;

        // Method to raise the PropertyChanged event
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
