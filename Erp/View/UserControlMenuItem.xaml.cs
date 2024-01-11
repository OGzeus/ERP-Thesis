using Erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Erp.View
{
    /// <summary>
    /// Interaction logic for UserControlMenuItem.xaml
    /// </summary>
    public partial class UserControlMenuItem : UserControl
    {
        MainView _context;
        public UserControlMenuItem(ItemMenu itemMenu, MainView context)
        {
            InitializeComponent();

            _context = context;

            ExpanderMenu.Visibility = itemMenu.SubItems == null ? Visibility.Collapsed : Visibility.Visible;
            ListViewItemMenu.Visibility = itemMenu.SubItems == null ? Visibility.Visible : Visibility.Collapsed;

            this.DataContext = itemMenu;

        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView)
            {
                if (listView.SelectedItem is SubItem selectedSubItem)
                {
                    //_context.SwitchScreen(selectedSubItem.Screen, selectedSubItem.Name);
                    _context.SwitchScreen2(selectedSubItem.Screen, selectedSubItem.Name);
                    _context.ClearSelectionExcept(this);

                }
            }
        }

        public void ClearSelection()
        {
            ListViewMenu.SelectedItem = null;
        }
    }
}
