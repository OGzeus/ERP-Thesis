using Erp.Model.BasicFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Data_Analytics
{
    public class DemandForecastData : INotifyPropertyChanged
    {
        public int ForId { get; set; }
        public string ForCode { get; set; }
        public ItemData Item { get; set; }
        public DateTime Date { get; set; }
        public string DateStr { get; set; }

        public decimal Demand { get; set; }

        // Define the PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;

        // Method to raise the PropertyChanged event
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
