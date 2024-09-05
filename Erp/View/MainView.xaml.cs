using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Erp.ViewModel;
using MaterialDesignThemes.Wpf;
using Erp.View.BasicFiles;
using Erp.Model;
using System.ComponentModel;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Data.Extensions;
using Erp.View.Thesis;
using Erp.Model.Thesis;

namespace Erp.View
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView  : Window, INotifyPropertyChanged
    {
        private MainViewModel _mainViewModel;

        public void INotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private List<SubItem> subItems;

        public List<SubItem> SubItems
        {
            get { return subItems; }
            set
            {
                subItems = value;
                INotifyPropertyChanged(nameof(SubItems));
            }
        }


        public MainView()
        {

            InitializeComponent();
            //this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.Height = SystemParameters.PrimaryScreenHeight * 0.70;
            this.Width = SystemParameters.PrimaryScreenWidth * 0.70;


            SubItems = new List<SubItem>();
            DataContext = new MainViewModel();


            #region Diplwmatikh Kozanidhs

            var menuFlights = new List<SubItem>();

            //menuFlights.Add(new SubItem("Languages", new LanguageView()));
            menuFlights.Add(new SubItem("Employees", new EmployeeView()));
            menuFlights.Add(new SubItem("Requirements Schedule", new RequirementsScheduleView()));
            menuFlights.Add(new SubItem("Vacation Planning", new VacationsPlanningView()));

            var VPThesis = new ItemMenu("Vacation Planning Menu", menuFlights, PackIconKind.Schedule);


            var menuFlights2 = new List<SubItem>();

            menuFlights2.Add(new SubItem("Countries", new CountryView()));
            menuFlights2.Add(new SubItem("Prefectures", new PrefectureView()));
            menuFlights2.Add(new SubItem("Cities", new CityView()));
            menuFlights2.Add(new SubItem("Airports", new AirportsView()));
            menuFlights2.Add(new SubItem("Flight Legs", new FlightLegsView()));
            menuFlights2.Add(new SubItem("Flight Routes", new FlightRoutesView()));
            menuFlights2.Add(new SubItem("Crew Rostering", new CrewSchedulingView()));

            var CSThesis = new ItemMenu("Crew Rostering Menu", menuFlights2, PackIconKind.Schedule);
            #endregion

            SubItems = SubItems.Concat(menuFlights).ToList();

            //Menu.Children.Add(new UserControlMenuItem(BasicFiles, this));
            //Menu.Children.Add(new UserControlMenuItem(Customers, this));
            ////Menu.Children.Add(new UserControlMenuItem(Schedules, this));
            ////Menu.Children.Add(new UserControlMenuItem(Vendors, this));
            //Menu.Children.Add(new UserControlMenuItem(Inventory, this));
            //Menu.Children.Add(new UserControlMenuItem(Production, this));
            //Menu.Children.Add(new UserControlMenuItem(SupChain, this));
            //Menu.Children.Add(new UserControlMenuItem(DataAnalysis, this));


            Menu.Children.Add(new UserControlMenuItem(VPThesis, this));
            Menu.Children.Add(new UserControlMenuItem(CSThesis, this));


        }

        public void ClearSelectionExcept(UserControlMenuItem selectedItem)
        {
            foreach (var menuItem in Menu.Children.OfType<UserControlMenuItem>())
            {
                if (menuItem != selectedItem)
                {
                    menuItem.ClearSelection();
                }
            }
        }

        private void TabControlExt_OnCloseButtonClick(object sender, CloseTabEventArgs e)
        {

            var selectedTab = e.TargetTabItem;

            MyTabControl.Items.Remove(selectedTab);

        }
        private void TabControlExt_OnCloseAllTabs(object sender, CloseTabEventArgs e)
        {
            for (int i = MyTabControl.Items.Count - 1; i >= 0; i--)
            {
                MyTabControl.Items.RemoveAt(i);
            }
        }

        private void TabControlExt_OnCloseOtherTabs(object sender, CloseTabEventArgs e)
        {
            var selectedTab = e.TargetTabItem;

            for (int i = MyTabControl.Items.Count - 1; i >= 0; i--)
            {
                var item = MyTabControl.Items[i];
                if (item != selectedTab)
                {
                    MyTabControl.Items.RemoveAt(i);
                }
            }
        }

        internal void SwitchScreen2(object sender, string HeaderText)
        {
            var screen = ((UserControl)sender);

            if (screen != null)
            {
                // Check if the screen is already in the tabs list
                var existingTab = MyTabControl.Items.OfType<TabItem>().FirstOrDefault(tab => tab.Content == screen);

                if (existingTab != null)
                {
                    // The screen is already in the tabs list, so select it
                    MyTabControl.SelectedItem = existingTab;

                }
                else
                {

                    var TabInput = new TabItemExt();

                    TabInput.Header = HeaderText;
                    TabInput.Header = new TextBlock
                    {
                        Text = HeaderText,
                        FontWeight = FontWeights.Bold, 
                        FontSize = 15,
                        FontFamily = new FontFamily("Segoe UI")// Set FontWeight to Bold
                    };

                    TabInput.FontSize = 14;
                    TabInput.FontFamily = new FontFamily("Segoe UI");

                    TabInput.Content = screen;

                    MyTabControl.SelectedItem = TabInput;

                    MyTabControl.Items.Add(TabInput);
                }


            }
          
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void pnlControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, 161, 2, 0);
        }

        private void pnlControlBar_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

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

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {

        }

    }
}
