using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Erp.ViewModel.Inventory
{
    public class GurobiViewModel : ViewModelBase
    {


    public GurobiViewModel()
    {
        //Na ftiaxtei function wste otan epilextei to veltiso mrp me to velisto INVENTORY POLICY .
        //Na apothikevete h zhthsh twn prwtwn ulwn  ston pinaka forecast_demand.Gia tis epomenes xronikes periodous
    }

    private DataTable _data;

    public DataTable Data
    {
        get
        {
            return _data;
        }
        set
        {
            _data = value;
            RaisePropertyChanged("Data");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void RaisePropertyChanged(string propertyname)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
}
