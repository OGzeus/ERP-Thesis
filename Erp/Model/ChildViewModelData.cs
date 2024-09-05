using Erp.ViewModel;
using Erp.ViewModel.BasicFiles;
using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model
{
    public class ChildViewModelData
    {
        public ViewModelBase ChildView { get; set; }

        public string Caption { get; set; }

        public IconChar Icon { get; set; }

        public ChildViewModelData(string Page)
        {

        

        }

    }
}
