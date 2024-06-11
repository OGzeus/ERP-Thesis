using Erp.CommonFiles;
using Erp.Helper;
using Erp.Model.BasicFiles;
using Erp.Model.Suppliers;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Syncfusion.Data.Extensions;
using Erp.Model.Manufacture;
using Erp.Model.Manufacture.MRP;
using Erp.Model.SupplyChain;
using Erp.Model.Enums;
using Erp.Model.SupplyChain.Clusters;

using System.Windows.Media;
using Erp.Model.SupplyChain.VRP;
using Erp.Model.Thesis.CrewScheduling;
using LiveCharts.Defaults;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Ink;

namespace Erp.ViewModel.SupplyChain
{
    public class Clustering_Vrp_Viewmodel : ViewModelBase
    {

        #region DataProperties

        private int _selectedTabIndex;

        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set
            {
                if (_selectedTabIndex != value)
                {
                    _selectedTabIndex = value;
                    INotifyPropertyChanged(nameof(SelectedTabIndex));
                }
            }
        }
        private ICollectionView collectionviewD;

        public ICollectionView CollectionViewD
        {
            get
            {
                return collectionviewD;
            }
            set
            {
                collectionviewD = value;
                INotifyPropertyChanged("CollectionViewD");
            }
        }
        private ICollectionView collectionViewRepair;

        public ICollectionView CollectionViewRepair
        {
            get
            {
                return collectionViewRepair;
            }
            set
            {
                collectionViewRepair = value;
                INotifyPropertyChanged("CollectionViewRepair");
            }
        }

        private CL_Vrp_InputData inputdata;
        public CL_Vrp_InputData InputData
        {
            get { return inputdata; }
            set
            {
                inputdata = value;
                INotifyPropertyChanged(nameof(InputData));


            }
        }


        private MrpResultData _GridResultsData;
        public MrpResultData GridResultsData
        {
            get { return _GridResultsData; }
            set
            {
                _GridResultsData = value;
                INotifyPropertyChanged(nameof(GridResultsData));


            }
        }

        private CL_OutputData _ClResultsdata;
        public CL_OutputData ClResultsdata
        {
            get { return _ClResultsdata; }
            set
            {
                _ClResultsdata = value;
                INotifyPropertyChanged(nameof(ClResultsdata));


            }
        }
        private VRPResultsData _VRPResultsData;
        public VRPResultsData VRPResultsData
        {
            get { return _VRPResultsData; }
            set
            {
                _VRPResultsData = value;
                INotifyPropertyChanged(nameof(VRPResultsData));


            }
        }

        private Columns sfGridColumns;
        public Columns SfGridColumns
        {
            get { return sfGridColumns; }
            set
            {
                this.sfGridColumns = value;
                INotifyPropertyChanged("SfGridColumns");
            }
        }
        private Columns sfGridColumnsd;
        public Columns SfGridColumnsD
        {
            get { return sfGridColumnsd; }
            set
            {
                this.sfGridColumnsd = value;
                INotifyPropertyChanged("SfGridColumnsD");
            }
        }
        private Columns sfGridColumnsRepair;
        public Columns SfGridColumnsRepair
        {
            get { return sfGridColumnsRepair; }
            set
            {
                this.sfGridColumnsRepair = value;
                INotifyPropertyChanged("SfGridColumnsRepair");
            }
        }


        #endregion
        #region Enums

        public BasicEnums.Clustering_Techniques[] Cl_Enums
        {
            get { return (BasicEnums.Clustering_Techniques[])Enum.GetValues(typeof(BasicEnums.Clustering_Techniques)); }
        }

        public BasicEnums.VRP_Techniques[] VRP_Enums
        {
            get { return (BasicEnums.VRP_Techniques[])Enum.GetValues(typeof(BasicEnums.VRP_Techniques)); }
        }

        #endregion
        #region Diagrams
        //private PlotModel plotModel;
        //public PlotModel PlotModel
        //{
        //    get => plotModel;
        //    set
        //    {
        //        plotModel = value;
        //        INotifyPropertyChanged(nameof(PlotModel));
        //    }
        //}

        #endregion


        public Clustering_Vrp_Viewmodel()
        {

            #region Αρχικοποιηση Data

            InputData = new CL_Vrp_InputData();
            InputData.CityData = new ObservableCollection<CityData>();
            InputData.CL_VehicleData = new ObservableCollection<Cluster_Vehicles_Data>();
            InputData.CL_VehicleDataGrid = new ObservableCollection<Cluster_Vehicles_Data>();

            InputData.VrpInputData = new VRP_InputData();
            InputData.CLInputData = new CL_InputData();

            InputData.CLInputData.KmeansInputData = new KmeansInputData();
            InputData.CLInputData.KmeansInputData.MaxIterations = 1000000;
            InputData.CLInputData.KmeansInputData.NumberOfClusters = 5;
            InputData.CLInputData.HierInputData = new HierarchicalInputData();


            InputData.VrpInputData.SimAnnealing_InputData = new SimAnnealing_InputData();
            InputData.VrpInputData.SimAnnealing_InputData.InitialTemp = 1000;
            InputData.VrpInputData.SimAnnealing_InputData.StoppingTemp = 50;
            InputData.VrpInputData.SimAnnealing_InputData.CoolingRate = 50;
            InputData.VrpInputData.SimAnnealing_InputData.NumberOfDepots = 1;
            InputData.VrpInputData.SimAnnealing_InputData.MaxIterations = 3000;



            InputData.StackPanelEnabled = false;
            ClResultsdata = new CL_OutputData();
            ClResultsdata.Clusters = new ObservableCollection<ClusterDatapoint>();
            ClResultsdata.DataPoints = new ObservableCollection<MainDatapoint>();
            ClResultsdata.Clustering_Diagram = new DiagramClusteringData();

            GridResultsData = new MrpResultData();


            this.sfGridColumns = new Columns();
            this.SfGridColumnsRepair = new Columns();

            #endregion

            #region Αρχικοποιηση Commands 

            CalculateClustering = new RelayCommand2(ExecuteCalculateClustering);
            CalculateVRP = new RelayCommand2(ExecuteCalculateVRP);
            ShowCitiesGridCommand = new RelayCommand2(ExecuteShowCitiesGridCommand);
            ShowVehiclesGridCommand = new RelayCommand2(ExecuteShowVehiclesGridCommand);
            #endregion

            #region Diagrams

            ClResultsdata.Clustering_Diagram = new DiagramClusteringData();

            this.SfGridColumnsD = new Columns();



            ShowWorkcenterGridCommand = new RelayCommand2(ExecuteShowWorkcenterGridCommand);


            rowDataCommandD = new RelayCommand2(ChangeCanExecuteD);




            #endregion

            #region HardCodedVehicle 
            InputData.HardCodedVehicles = new List<VehicleData>();

            var newVehicle = new VehicleData();
            newVehicle.Code = "V1";
            newVehicle.Descr = "Vehicle 1";
            newVehicle.Capacity = 50;
            newVehicle.NumberOfVehicles = 4;

            InputData.HardCodedVehicles.Add(newVehicle);
            newVehicle = new VehicleData();
            newVehicle.Code = "V2";
            newVehicle.Descr = "Vehicle 2";
            newVehicle.Capacity = 100;
            newVehicle.NumberOfVehicles = 2;

            InputData.HardCodedVehicles.Add(newVehicle);

            #endregion
        }


        #region Commands

        #region F7 
        public ICommand ShowVehiclesGridCommand { get; }

        public ICommand ShowCitiesGridCommand { get; }


        public void ExecuteShowVehiclesGridCommand(object obj)
        {
            InputData.CL_VehicleDataGrid = new ObservableCollection<Cluster_Vehicles_Data>();

            InputData.CL_VehicleData = new ObservableCollection<Cluster_Vehicles_Data>();
            foreach (var item in ClResultsdata.Clusters)
            {
                foreach(var Veh in InputData.HardCodedVehicles)
                {
                    var row = new Cluster_Vehicles_Data();
                    row.Cluster = item;
                    row.Vehicle = Veh;
                    InputData.CL_VehicleData.Add(row);
                }
            };



            InputData.CL_VehicleDataGrid = InputData.CL_VehicleData;

        }

        public void ExecuteShowCitiesGridCommand(object obj)
        {

            InputData.CityData = CommonFunctions.GetCityData(ShowDeleted);
            foreach (var item in InputData.CityData)
            {
                item.Demand = 100;
                item.Selected = true;
            }

        }
        #endregion

        #endregion


        #region Calculate Clustering
        public ICommand CalculateClustering { get; }

        private void ExecuteCalculateClustering(object obj)
        {
            InputData.CLInputData.DataPoints = new Dictionary<string, (double , double )>();
            InputData.CLInputData.DataPoints_Int = new Dictionary<int, (double, double)>();
            InputData.CLInputData.City_Index = new Dictionary<string, int>();
            ClResultsdata.DataPoints = new ObservableCollection<MainDatapoint>();
            ClResultsdata.Clusters = new ObservableCollection<ClusterDatapoint>();

            #region Populate Datapoints Dictionaries

            int i = 1;
            foreach (var city in InputData.CityData)
            {
                InputData.CLInputData.DataPoints.Add(city.CityCode, (city.Latitude, city.Longitude));
                InputData.CLInputData.DataPoints_Int.Add(i, (city.Latitude, city.Longitude));
                InputData.CLInputData.City_Index.Add(city.CityCode,i);
                i = i + 1;
            }

            #endregion

            var Cl_Enum = InputData.CLInputData.CL_Enum;
            var Dict = InputData.CLInputData.DataPoints;
            if (Cl_Enum == BasicEnums.Clustering_Techniques.K_means)
            {
               ClResultsdata.Kmeansoutputdata = ML_AiFunctions.Calculate_Kmeans_Clustering(InputData.CLInputData.City_Index,InputData.CLInputData.KmeansInputData, Dict);

                foreach(var cl in ClResultsdata.Kmeansoutputdata.Clusters)
                {
                    var clusterpoint = new ClusterDatapoint();

                    clusterpoint.ClusterCode = cl.CentroidCode;
                    clusterpoint.Longitude = cl.CentroidLongitude;
                    clusterpoint.Latitude = cl.CentroidLatitude;


                    var NumOfPoints = 0;
                    foreach (var point in cl.DataPoints)
                    {
                        point.Demand = InputData.CityData.Where(d => d.CityCode == point.Code).FirstOrDefault().Demand;
                        ClResultsdata.DataPoints.Add(point);
                        NumOfPoints = NumOfPoints + 1;
                    }

                    clusterpoint.NumberOfPoints = NumOfPoints;
                    ClResultsdata.Clusters.Add(clusterpoint);

                }
            }
            else if(Cl_Enum == BasicEnums.Clustering_Techniques.Hierarchical)
            {

            }
            else if (Cl_Enum == BasicEnums.Clustering_Techniques.Optimization)
            {

            }

            ExecuteCreateClusteringDiagramCommand(ClResultsdata);
            SelectedTabIndex = 1;
        }



        #region Clustering Diagrams
        private void ExecuteCreateClusteringDiagramCommand(object obj)
        {
            try
            {
                var ClustersData = new ObservableCollection<ClusterDatapoint>();
                var DataPointsData = new ObservableCollection<MainDatapoint>();

                var XData = new ObservableCollection<DecisionVariableX>();

                #region Κατασκευη linechart με 3 γραμμές

                var clList = ClResultsdata.Clusters.OrderBy(d => d.ClusterCode).ToList();
                ClustersData = clList.ToObservableCollection();

                DataPointsData = ClResultsdata.DataPoints; 

                ClResultsdata.Clustering_Diagram.SeriesCollection = new SeriesCollection();

                foreach (var Cluster in ClustersData)
                { 
                    var ClCode = Cluster.ClusterCode;

                    var DataPointSeries = new ScatterSeries();
                    DataPointSeries.Title = ClCode;
                    DataPointSeries.Values = new ChartValues<ObservablePoint>();
                    DataPointSeries.PointGeometry = DefaultGeometries.Circle;
                    foreach (var Point in DataPointsData)
                    {
                        if(Point.ClusterCode == ClCode)
                        {
                            var Obs_Point = new ObservablePoint();
                            Obs_Point.X = Point.Longitude;
                            Obs_Point.Y = Point.Latitude;
                            DataPointSeries.Values.Add(Obs_Point);
                        }
                    }

                    ClResultsdata.Clustering_Diagram.SeriesCollection.Add(DataPointSeries);

                }
                
                ClResultsdata.Clustering_Diagram.YFormatter = value => value.ToString("N0");

                #endregion
            }
            catch
            {
                Console.WriteLine("An error occurred");

            }

        }

        public class ColorConverter
        {
            public List<Color> GenerateColors(int numberOfColors)
            {
                List<Color> colors = new List<Color>();
                Random random = new Random();

                for (int i = 0; i < numberOfColors; i++)
                {
                    byte r = (byte)random.Next(256);
                    byte g = (byte)random.Next(256);
                    byte b = (byte)random.Next(256);
                    colors.Add(Color.FromRgb(r, g, b));
                }

                return colors;
            }
        }

        #endregion

        #endregion

        #region Calculate VRP

        public ICommand CalculateVRP { get; }

        private void ExecuteCalculateVRP(object obj)
        {
            var a = new ObservableCollection<ClusterDatapoint>();
            a.Add(ClResultsdata.Clusters.FirstOrDefault());

            foreach (var Cluster in a)
            {

                #region VRP Dictionaries

                InputData.VrpInputData.Vehicles_IndexMap = new Dictionary<string, int>();
                InputData.VrpInputData.VehicleCapacity = new Dictionary<int, double>();

                InputData.VrpInputData.Customer_IndexMap = new Dictionary<string, int>();
                InputData.VrpInputData.Demand = new Dictionary<int, double>();
                InputData.VrpInputData.Coords = new Dictionary<int, (double, double)>();

                InputData.VrpInputData.Depot_IndexMap = new Dictionary<string, int>();
                InputData.VrpInputData.DepotCapacity = new Dictionary<int, double>();
                InputData.VrpInputData.DepotCoords = new Dictionary<int, (double, double)>();

                var Vehicles_IndexMap = new Dictionary<string, int>();
                var VehicleCapacity = new Dictionary<int, double>();
                var NumberOfVehicles = new Dictionary<int, int>();

                var Customer_IndexMap = new Dictionary<string, int>();
                var Demand = new Dictionary<int, double>();
                var Coords = new Dictionary<int, (double, double)>();

                var Depot_IndexMap = new Dictionary<string, int>();
                var DepotCapacity = new Dictionary<int, double>();
                var DepotCoords = new Dictionary<int, (double, double)>();

                #endregion



                var SelectedCluster = Cluster.ClusterCode;


                Depot_IndexMap.Add(SelectedCluster, 1);
                DepotCapacity.Add(1, 20000);
                DepotCoords.Add(1, (Cluster.Latitude, Cluster.Longitude));



                var CL_Vehicles = new ObservableCollection<Cluster_Vehicles_Data>(
                    InputData.CL_VehicleDataGrid.Where(cl => cl.Cluster.ClusterCode == SelectedCluster)
                );

                int VehicleCounter = 1;
                foreach (var Veh in CL_Vehicles)
                {
                    for(int i  = 1; i <= Veh.Vehicle.NumberOfVehicles;i++)
                    {
                        string VehicleCode = Veh.Vehicle.Code + "_" + i;
                        Vehicles_IndexMap.Add(VehicleCode, VehicleCounter);
                        VehicleCapacity.Add(VehicleCounter, Veh.Vehicle.Capacity);
                        VehicleCounter = VehicleCounter + 1;

                    };

                }


                var CL_Points = new ObservableCollection<MainDatapoint>(
                    ClResultsdata.DataPoints.Where(cl => cl.ClusterCode == SelectedCluster)
                );
                int PointCounter = 1;
                foreach (var Point in ClResultsdata.DataPoints)
                {
                    Customer_IndexMap.Add(Point.Code, PointCounter);
                    Demand.Add(PointCounter, Point.Demand);
                    Coords.Add(PointCounter, (Point.Latitude, Point.Longitude));

                    PointCounter++;
                }

                #region Fill Original Dictionaries

                InputData.VrpInputData.Vehicles_IndexMap = Vehicles_IndexMap;
                InputData.VrpInputData.VehicleCapacity = VehicleCapacity;

                InputData.VrpInputData.Customer_IndexMap = Customer_IndexMap;
                InputData.VrpInputData.Demand = Demand;
                InputData.VrpInputData.Coords = Coords;

                InputData.VrpInputData.Depot_IndexMap = Depot_IndexMap;
                InputData.VrpInputData.DepotCapacity = DepotCapacity;
                InputData.VrpInputData.DepotCoords = DepotCoords;


                #endregion

                var VRP_Enum = InputData.VrpInputData.VRP_Enum;

                if (VRP_Enum == BasicEnums.VRP_Techniques.Simulation_Annealing)
                {
                    VRPResultsData = ML_AiFunctions.Calculate_Simulation_Annealing(InputData.VrpInputData);

                    var aa = 1;

                }
                else if (VRP_Enum == BasicEnums.VRP_Techniques.Tabu_Search)
                {

                }
                else if (VRP_Enum == BasicEnums.VRP_Techniques.Ant_Colony)
                {

                }
                else if (VRP_Enum == BasicEnums.VRP_Techniques.Optimization)
                {

                }


            }
        }


        #region LiveCharts

        #endregion

        #endregion

        #region Diagrams

        #region F7
        public ICommand ShowWorkcenterGridCommand { get; }
        private void ExecuteShowWorkcenterGridCommand(object obj)
        {


            //ClearColumnsD();
            //var F7input = F7Common.F7WorkcenterMRPDiagram2();
            //F7key = F7input.F7key;
            //var Data = InputData.Workcenters;
            //F7input.CollectionView = CollectionViewSource.GetDefaultView(Data);
            //CollectionViewD = F7input.CollectionView;
            //var a = F7input.SfGridColumns;
            //foreach (var item in a)
            //{
            //    this.SfGridColumnsD.Add(item);
            //}


        }
        public void ChangeCanExecuteD(object obj)
        {

            //if (F7key == "ItemCode")
            //{
            //    ClResultsdata.Diagram.Item = (SelectedItem2 as ItemData);
            //}
            //if (F7key == "Workcenter")
            //{
            //    ClResultsdata.Diagram.Workcenter = (SelectedItem2 as WorkcenterData);
            //}

        }


        private ICommand rowDataCommandD { get; set; }
        public ICommand RowDataCommandD
        {
            get
            {
                return rowDataCommandD;
            }
            set
            {
                rowDataCommandD = value;
            }
        }

        protected void ClearColumnsD()
        {

            var ColumnsCount = this.SfGridColumnsD.Count();
            if (ColumnsCount != 0)
            {
                for (int i = 0; i < ColumnsCount; i++)
                {
                    this.SfGridColumnsD.RemoveAt(0);
                }
            }
        }
        #endregion



        #endregion

        private ICommand rowDataCommand { get; set; }
        public ICommand RowDataCommand
        {
            get
            {
                return rowDataCommand;
            }
            set
            {
                rowDataCommand = value;
            }
        }


        protected void ClearColumns()
        {

            var ColumnsCount = this.SfGridColumns.Count();
            if (ColumnsCount != 0)
            {
                for (int i = 0; i < ColumnsCount; i++)
                {
                    this.sfGridColumns.RemoveAt(0);
                }
            }
        }


    }
}
