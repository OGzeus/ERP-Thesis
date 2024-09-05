using Erp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Erp.ViewModel
{
    public class TabViewModel
    {
        public TabViewModel()
        {
        }

        public string Header { get; set; }
        public UserControl Content { get; set; }
    }
}
