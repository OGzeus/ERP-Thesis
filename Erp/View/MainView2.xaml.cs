using Erp.Model;
using Erp.ViewModel;
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
using System.Windows.Shapes;

namespace Erp.View
{
    /// <summary>
    /// Interaction logic for MainView2.xaml
    /// </summary>
    public partial class MainView2 : Window
    {
        private ChildViewModelData dataForChild;
        private MainViewModel2 test;

        public MainView2()
        {
            InitializeComponent();
        }

        public MainView2(MainViewModel2 test)
        {
            InitializeComponent();
            DataContext = test;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else this.WindowState = WindowState.Normal;
        }
    }
}
