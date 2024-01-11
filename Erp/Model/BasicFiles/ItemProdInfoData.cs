using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Erp.Model.BasicFiles
{
    public class ItemProdInfoData : INotifyPropertyChanged
    {


        private int _CostInfoId { get; set; }
        private ItemData _Item { get; set; }


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
