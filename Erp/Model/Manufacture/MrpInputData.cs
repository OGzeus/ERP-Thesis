using DevExpress.XtraEditors.Filtering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Manufacture
{
    public class MrpInputData : INotifyPropertyChanged
    {
        private string enditemcode;
        private string itemcode;
        private float currentstock;
        private DateTime delivereddate;
        private float deliveredquantity;
        private string lotpolicy;
        private int safetyleadtime;
        private float percentage;

        public string EndItemCode
        {
            get { return enditemcode; }
            set { enditemcode = value; OnPropertyChanged("EndItemCode"); }
        }
        public string ItemCode
        {
            get { return itemcode; }
            set { itemcode = value; OnPropertyChanged("ItemCode"); }
        }

        [Display(GroupName = "Πργρ/σμένες Παραλαβές")]

        public float CurrentStock
        {
            get { return currentstock; }
            set { currentstock = value; OnPropertyChanged("CurrentStock"); }
        }

        [Display(GroupName = "Πργρ/σμένες Παραλαβές")]

        public DateTime DeliveredDate
        {
            get { return delivereddate; }
            set { delivereddate = value; OnPropertyChanged("DeliveredDate"); }
        }

        public float DeliveredQuantity
        {
            get { return deliveredquantity; }
            set { deliveredquantity = value; OnPropertyChanged("DeliveredQuantity"); }
        }

        public string LotPolicy
        {
            get { return lotpolicy; }
            set { lotpolicy = value; OnPropertyChanged("LotPolicy"); }
        }


        public int SafetyLeadTime
        {
            get { return safetyleadtime; }
            set { safetyleadtime = value; OnPropertyChanged("SafetyLeadTime"); }
        }

        public float Percentage
        {
            get { return percentage; }
            set { percentage = value; OnPropertyChanged("Percentage"); }
        }
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}