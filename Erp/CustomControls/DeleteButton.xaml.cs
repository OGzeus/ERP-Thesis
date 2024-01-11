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

namespace Erp.CustomControls
{
    /// <summary>
    /// Interaction logic for DeleteButton.xaml
    /// </summary>
    public partial class DeleteButton : UserControl
    {
        public DeleteButton()
        {
            InitializeComponent();
        }
        public event RoutedEventHandler OnClick1;
        public event RoutedEventHandler OnClick2;

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle the Click event for OnClick1
            ButtonDel.Background = Brushes.Red;
            OnClick1?.Invoke(sender, e);
        }



    }
}
