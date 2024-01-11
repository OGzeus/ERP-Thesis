using Erp.ViewModel;
using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Erp.Model.Interfaces
{
    public interface IToolbarRepository
    {

        ICommand RefreshCommand();

        ICommand SaveCommand();
        ICommand ShowChooserViewModelCommand();
        ICommand ShowSearchViewModelCommand();
    }
}
