using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.BasicFiles
{
    public class LotPolicyData : INotifyPropertyChanged
    {
        private ItemData _Item { get; set; }

        private int _LotPolicyId { get; set; }
        private string _Code { get; set; }

        private string _Descr { get; set; }

        private double _LeadTime { get; set; }

        private double _BatchSize { get; set; }

        private int _Period { get; set; }

        private bool _MainPolicy { get; set; }

        public ItemData Item
        {
            get { return _Item; }
            set { _Item = value; OnPropertyChanged("Item"); }
        }
        public int LotPolicyId
        {
            get { return _LotPolicyId; }
            set { _LotPolicyId = value; OnPropertyChanged("LotPolicyId"); }
        }
        public string Code
        {
            get { return _Code; }
            set { _Code = value; OnPropertyChanged("Code"); }
        }
        public string Descr
        {
            get { return _Descr; }
            set { _Descr = value; OnPropertyChanged("Descr"); }
        }
        public double LeadTime
        {
            get { return _LeadTime; }
            set { _LeadTime = value; OnPropertyChanged("LeadTime"); }
        }
        public double BatchSize
        {
            get { return _BatchSize; }
            set { _BatchSize = value; OnPropertyChanged("BatchSize"); }
        }
        public int Period
        {
            get { return _Period; }
            set { _Period = value; OnPropertyChanged("Period"); }
        }
        public bool MainPolicy
        {
            get { return _MainPolicy; }
            set { _MainPolicy = value; OnPropertyChanged("MainPolicy"); }

        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
