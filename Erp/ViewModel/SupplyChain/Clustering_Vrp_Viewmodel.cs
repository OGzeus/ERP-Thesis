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
using Erp.Model.Data_Analytics.Forecast;
using Erp.Model.Data_Analytics;
using Erp.Model.Inventory;
using System.Windows.Data;
using Erp.Model.Manufacture.MRP;
using Erp.Model.SupplyChain;
using Erp.Model.Enums;
using Erp.Model.SupplyChain.Clusters;
using OxyPlot;
using OxyPlot.Series;
using System.Diagnostics.Metrics;
using System.Windows.Media;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Erp.Model.SupplyChain.VRP;
using static Microsoft.FSharp.Core.ByRefKinds;

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
        private KmeansOutputData _Kmeansoutputdata;
        public KmeansOutputData Kmeansoutputdata
        {
            get { return _Kmeansoutputdata; }
            set
            {
                _Kmeansoutputdata = value;
                INotifyPropertyChanged(nameof(Kmeansoutputdata));


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
        private PlotModel plotModel;
        public PlotModel PlotModel
        {
            get => plotModel;
            set
            {
                plotModel = value;
                INotifyPropertyChanged(nameof(PlotModel));
            }
        }

        #region Syncfusion 


        private ObservableCollection<SFMapDatapoint> _KmeansDatapoints;
        public ObservableCollection<SFMapDatapoint> KmeansDatapoints
        {
            get => _KmeansDatapoints;
            set
            {
                _KmeansDatapoints = value;
                INotifyPropertyChanged(nameof(KmeansDatapoints));
            }
        }
        #endregion
        #endregion


        public Clustering_Vrp_Viewmodel()
        {



            InputData = new CL_Vrp_InputData();
            InputData.CityData = new ObservableCollection<CityData>();
            InputData.CL_VehicleData1 = new ObservableCollection<Cluster_Vehicles_Data>();
            InputData.CL_VehicleData2 = new ObservableCollection<Cluster_Vehicles_Data>();
            InputData.CL_VehicleDataGrid = new ObservableCollection<Cluster_Vehicles_Data>();

            InputData.VrpInputData_1 = new VRP_InputData();
            InputData.VrpInputData_2 = new VRP_InputData();
            InputData.CLInputData_1 = new CL_InputData();
            InputData.CLInputData_2 = new CL_InputData();

            InputData.CLInputData_1.KmeansInputData = new KmeansInputData();
            InputData.CLInputData_1.KmeansInputData.MaxIterations = 1000000;
            InputData.CLInputData_1.KmeansInputData.NumberOfClusters = 5;

            InputData.CLInputData_1.HierInputData = new HierarchicalInputData();

            InputData.CLInputData_2.KmeansInputData = new KmeansInputData();
            InputData.CLInputData_2.KmeansInputData.MaxIterations = 1000000;
            InputData.CLInputData_2.KmeansInputData.NumberOfClusters = 5;

            InputData.CLInputData_2.HierInputData = new HierarchicalInputData();


            InputData.VrpInputData_1.SimAnnealing_InputData = new SimAnnealing_InputData();
            InputData.VrpInputData_1.SimAnnealing_InputData.InitialTemp = 1000;
            InputData.VrpInputData_1.SimAnnealing_InputData.StoppingTemp = 50;
            InputData.VrpInputData_1.SimAnnealing_InputData.CoolingRate = 50;
            InputData.VrpInputData_1.SimAnnealing_InputData.NumberOfDepots = 1;
            InputData.VrpInputData_1.SimAnnealing_InputData.MaxIterations = 3000;

            InputData.VrpInputData_2.SimAnnealing_InputData = new SimAnnealing_InputData();


            InputData.StackPanelEnabled = false;
            ClResultsdata = new CL_OutputData();
            ClResultsdata.Clusters1 = new ObservableCollection<ClusterDatapoint>();
            ClResultsdata.Clusters2 = new ObservableCollection<ClusterDatapoint>();
            ClResultsdata.DataPoints1 = new ObservableCollection<MainDatapoint>();
            ClResultsdata.DataPoints2 = new ObservableCollection<MainDatapoint>();
            ClResultsdata.Diagram1 = new DiagramsMRPData();
            ClResultsdata.Diagram2 = new DiagramsMRPData();

            GridResultsData = new MrpResultData();


            this.sfGridColumns = new Columns();
            this.SfGridColumnsRepair = new Columns();

            InsertDataCommand = new RelayCommand2(ExecuteInsertDataCommand);
            CalculateClustering1 = new RelayCommand2(ExecuteCalculateClustering1);
            CalculateClustering2 = new RelayCommand2(ExecuteCalculateClustering2);
            CalculateVRP1 = new RelayCommand2(ExecuteCalculateVRP1);
            CalculateVRP2 = new RelayCommand2(ExecuteCalculateVRP2);
            ShowCitiesGridCommand = new RelayCommand2(ExecuteShowCitiesGridCommand);
            ShowVehicles1GridCommand = new RelayCommand2(ExecuteShowVehiclesGridCommand1);
            ShowVehicles2GridCommand = new RelayCommand2(ExecuteShowVehiclesGridCommand2);

            KmeansDatapoints = new ObservableCollection<SFMapDatapoint>();
            KmeansDatapoints.Add(new SFMapDatapoint() { Name = "USA ", Latitude = "38.8833N", Longitude = "77.0167W" });
            KmeansDatapoints.Add(new SFMapDatapoint() { Name = "Brazil ", Latitude = "15.7833S", Longitude = "47.8667W" });
            KmeansDatapoints.Add(new SFMapDatapoint() { Name = "India ", Latitude = "21.0000N", Longitude = "78.0000E" });
            KmeansDatapoints.Add(new SFMapDatapoint() { Name = "China ", Latitude = "35.0000N", Longitude = "103.0000E" });
            KmeansDatapoints.Add(new SFMapDatapoint() { Name = "Indonesia ", Latitude = "6.1750S", Longitude = "106.8283E" });
            #region Diagrams


            ClResultsdata.Diagram1 = new DiagramsMRPData();
            ClResultsdata.Diagram1.Item = new ItemData();
            ClResultsdata.Diagram2 = new DiagramsMRPData();
            ClResultsdata.Diagram2.Workcenter = new WorkcenterData();

            ClResultsdata.Diagram2.Workcenter = new WorkcenterData();
            this.SfGridColumnsD = new Columns();



            ShowWorkcenterGridCommand = new RelayCommand2(ExecuteShowWorkcenterGridCommand);

            CreateDiagram1Command = new RelayCommand2(ExecuteCreateDiagram1Command);
            CreateDiagram2Command = new RelayCommand2(ExecuteCreateDiagram2Command);
            rowDataCommandD = new RelayCommand2(ChangeCanExecuteD);




            #endregion
            string filePath2 = @"C:\Users\npoly\OneDrive\Υπολογιστής\diagrams_syncfusion\Germany_Clustering.shp";
            InputData.HardCodedVehicles = new List<VehicleData>();
            #region HardCodedVehicle 

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
        public ICommand ShowVehicles1GridCommand { get; }
        public ICommand ShowVehicles2GridCommand { get; }

        public ICommand ShowCitiesGridCommand { get; }


        public void ExecuteShowVehiclesGridCommand1(object obj)
        {
            InputData.CL_VehicleDataGrid = new ObservableCollection<Cluster_Vehicles_Data>();

            InputData.CL_VehicleData1 = new ObservableCollection<Cluster_Vehicles_Data>();
            foreach (var item in ClResultsdata.Clusters1)
            {
                foreach(var Veh in InputData.HardCodedVehicles)
                {
                    var row = new Cluster_Vehicles_Data();
                    row.Cluster = item;
                    row.Vehicle = Veh;
                    InputData.CL_VehicleData1.Add(row);
                }
            };



            InputData.CL_VehicleDataGrid = InputData.CL_VehicleData1;

        }

        public void ExecuteShowVehiclesGridCommand2(object obj)
        {
            InputData.CL_VehicleDataGrid = new ObservableCollection<Cluster_Vehicles_Data>();
            InputData.CL_VehicleData2 = new ObservableCollection<Cluster_Vehicles_Data>();

            foreach (var item in ClResultsdata.Clusters2)
            {
                foreach (var Veh in InputData.HardCodedVehicles)
                {
                    var row = new Cluster_Vehicles_Data();
                    row.Cluster = item;
                    row.Vehicle = Veh;
                    InputData.CL_VehicleData1.Add(row);
                }
            };


            InputData.CL_VehicleDataGrid = InputData.CL_VehicleData2;


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
        #region InsertData For Optimisation
        public ICommand InsertDataCommand { get; }
        private void ExecuteInsertDataCommand(object obj)
        {
            #region Arxikopoihsh Dictionaries 
            //InputData.Workcenters = new ObservableCollection<WorkcenterData>();

            //InputData.T = new int();
            //InputData.P = new int();
            //InputData.Q = new int();
            //InputData.Pw = new Dictionary<string, List<string>>();
            //InputData.Qw = new Dictionary<string, List<string>>();
            //InputData.Dit = new Dictionary<(string, string), double>();
            //InputData.Ci = new Dictionary<string, List<string>>();
            //InputData.Rij = new Dictionary<(string, string), double>();
            //InputData.Miwt = new Dictionary<(string, string, string), double>();
            //InputData.Awt = new Dictionary<(string, string), double>();
            //InputData.Uiw = new Dictionary<(string, string), double>();
            //InputData.Hi = new Dictionary<string, double>();
            //InputData.Gi = new Dictionary<string, double>();
            //InputData.I0W = new Dictionary<string, string>();

            //InputData.Ii = new Dictionary<string, (double, double)>();
            //InputData.Imax_min = new Dictionary<string, (double, double)>();


            #endregion

        }
        #endregion

        public HashSet<string> GetDistinctBOMItems(MRPInputData inputData)
        {
            HashSet<string> distinctItems = new HashSet<string>();

            foreach (var item in inputData.EndItems)
            {
                foreach (var bomItem in item.Bom)
                {
                    distinctItems.Add(bomItem.BomItem.ItemCode);

                }

            }


            return (distinctItems);
        }
        #region Calculate Clustering
        public ICommand CalculateClustering1 { get; }

        private void ExecuteCalculateClustering1(object obj)
        {
            InputData.CLInputData_1.DataPoints = new Dictionary<string, (double , double )>();
            InputData.CLInputData_1.DataPoints_Int = new Dictionary<int, (double, double)>();
            InputData.CLInputData_1.City_Index = new Dictionary<string, int>();
            ClResultsdata.DataPoints1 = new ObservableCollection<MainDatapoint>();
            ClResultsdata.Clusters1 = new ObservableCollection<ClusterDatapoint>();

            #region Populate Datapoints Dictionaries

            int i = 1;
            foreach (var city in InputData.CityData)
            {
                InputData.CLInputData_1.DataPoints.Add(city.CityCode, (city.Latitude, city.Longitude));
                InputData.CLInputData_1.DataPoints_Int.Add(i, (city.Latitude, city.Longitude));
                InputData.CLInputData_1.City_Index.Add(city.CityCode,i);
                i = i + 1;
            }

            #endregion

            var Cl_Enum = InputData.CLInputData_1.CL_Enum;
            var Dict = InputData.CLInputData_1.DataPoints;
            if (Cl_Enum == BasicEnums.Clustering_Techniques.K_means)
            {
                Kmeansoutputdata = ML_AiFunctions.Calculate_Kmeans_Clustering(InputData.CLInputData_1.City_Index,InputData.CLInputData_1.KmeansInputData, Dict);
                PlotModel = CreatePlotModel(Kmeansoutputdata,InputData.CLInputData_1.KmeansInputData.NumberOfClusters);
                ML_AiFunctions.CreateShapefile(Kmeansoutputdata, @"C:\Users\npoly\OneDrive\Υπολογιστής\diagrams_syncfusion\Germany_Clustering2.shp");

                foreach(var cl in Kmeansoutputdata.Clusters)
                {
                    var clusterpoint = new ClusterDatapoint();

                    clusterpoint.ClusterCode = cl.CentroidCode;
                    clusterpoint.Longitude = cl.CentroidLongitude;
                    clusterpoint.Latitude = cl.CentroidLatitude;


                    var NumOfPoints = 0;
                    foreach (var point in cl.DataPoints)
                    {
                        point.Demand = InputData.CityData.Where(d => d.CityCode == point.Code).FirstOrDefault().Demand;
                        ClResultsdata.DataPoints1.Add(point);
                        NumOfPoints = NumOfPoints + 1;
                    }

                    clusterpoint.NumberOfPoints = NumOfPoints;
                    ClResultsdata.Clusters1.Add(clusterpoint);

                }
            }
            else if(Cl_Enum == BasicEnums.Clustering_Techniques.Hierarchical)
            {

            }
            else if (Cl_Enum == BasicEnums.Clustering_Techniques.Optimization)
            {

            }


            
        }

        private ObservableCollection<SFMapDatapoint> CreateSyncfusionPlotModel(KmeansOutputData outputData)
        {
            KmeansDatapoints = new ObservableCollection<SFMapDatapoint>();

            var i = 1;
            foreach (var cluster in outputData.Clusters)
            {


                foreach (var point in cluster.DataPoints)
                {
                    KmeansDatapoints.Add(new SFMapDatapoint() {Name = @"i", Latitude = point.Latitude.ToString(), Longitude = point.Latitude.ToString() });

                }

                //plotModel.Series.Add(scatterSeries);


                //centroidSeries.Points.Add(new ScatterPoint(cluster.CentroidLatitude, cluster.CentroidLongitude));
                //plotModel.Series.Add(centroidSeries);
            }
            return KmeansDatapoints;
            i++;
        }

        #region Oxyplots 
        private PlotModel CreatePlotModel(KmeansOutputData outputData, int NumberOfClusters)
        {
            var plotModel = new PlotModel { Title = "KMeans Clustering" };

            // Generate colors dynamically based on the number of clusters
            var colorConverter = new ColorConverter();
            var colors = colorConverter.GenerateColors(NumberOfClusters);

            foreach (var cluster in outputData.Clusters)
            {
                var clusterColor = OxyColor.FromRgb(colors[cluster.ClusterId % NumberOfClusters].R,
                                                     colors[cluster.ClusterId % NumberOfClusters].G,
                                                     colors[cluster.ClusterId % NumberOfClusters].B);

                var scatterSeries = new ScatterSeries
                {
                    MarkerType = MarkerType.Circle,
                    MarkerFill = clusterColor
                };

                foreach (var point in cluster.DataPoints)
                {
                    scatterSeries.Points.Add(new ScatterPoint(point.Longitude, point.Latitude));
                }

                plotModel.Series.Add(scatterSeries);

                var centroidSeries = new ScatterSeries
                {
                    MarkerType = MarkerType.Diamond,
                    MarkerFill = clusterColor, // Set centroid fill color same as cluster color
                    MarkerStroke = OxyColors.White,
                    MarkerStrokeThickness = 2
                };

                centroidSeries.Points.Add(new ScatterPoint(cluster.CentroidLongitude, cluster.CentroidLatitude));
                plotModel.Series.Add(centroidSeries);
            }

            return plotModel;
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
        public ICommand CalculateClustering2 { get; }

        private void ExecuteCalculateClustering2(object obj)
        {
            //OutputData = CommonFunctions.CalculateMRP2(InputData);
            SelectedTabIndex = 2;

        }
        #endregion

        #region Calculate VRP

        public ICommand CalculateVRP1 { get; }

        private void ExecuteCalculateVRP1(object obj)
        {
            var a = new ObservableCollection<ClusterDatapoint>();
            a.Add(ClResultsdata.Clusters1.FirstOrDefault());

            foreach (var Cluster in a)
            {

                #region VRP Dictionaries

                InputData.VrpInputData_1.Vehicles_IndexMap = new Dictionary<string, int>();
                InputData.VrpInputData_1.VehicleCapacity = new Dictionary<int, double>();

                InputData.VrpInputData_1.Customer_IndexMap = new Dictionary<string, int>();
                InputData.VrpInputData_1.Demand = new Dictionary<int, double>();
                InputData.VrpInputData_1.Coords = new Dictionary<int, (double, double)>();

                InputData.VrpInputData_1.Depot_IndexMap = new Dictionary<string, int>();
                InputData.VrpInputData_1.DepotCapacity = new Dictionary<int, double>();
                InputData.VrpInputData_1.DepotCoords = new Dictionary<int, (double, double)>();

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
                    ClResultsdata.DataPoints1.Where(cl => cl.ClusterCode == SelectedCluster)
                );
                int PointCounter = 1;
                foreach (var Point in ClResultsdata.DataPoints1)
                {
                    Customer_IndexMap.Add(Point.Code, PointCounter);
                    Demand.Add(PointCounter, Point.Demand);
                    Coords.Add(PointCounter, (Point.Latitude, Point.Longitude));

                    PointCounter++;
                }

                #region Fill Original Dictionaries

                InputData.VrpInputData_1.Vehicles_IndexMap = Vehicles_IndexMap;
                InputData.VrpInputData_1.VehicleCapacity = VehicleCapacity;

                InputData.VrpInputData_1.Customer_IndexMap = Customer_IndexMap;
                InputData.VrpInputData_1.Demand = Demand;
                InputData.VrpInputData_1.Coords = Coords;

                InputData.VrpInputData_1.Depot_IndexMap = Depot_IndexMap;
                InputData.VrpInputData_1.DepotCapacity = DepotCapacity;
                InputData.VrpInputData_1.DepotCoords = DepotCoords;


                #endregion

                var VRP_Enum = InputData.VrpInputData_1.VRP_Enum;

                if (VRP_Enum == BasicEnums.VRP_Techniques.Simulation_Annealing)
                {
                    VRPResultsData = ML_AiFunctions.Calculate_Simulation_Annealing(InputData.VrpInputData_1);

                    var aa = 1;
                    //PlotModel = CreatePlotModel(Kmeansoutputdata, InputData.CLInputData_1.KmeansInputData.NumberOfClusters);

                    //foreach (var cl in Kmeansoutputdata.Clusters)
                    //{
                    //    foreach (var point in cl.DataPoints)
                    //    {
                    //        ClResultsdata.DataPoints1.Add(point);

                    //    }
                    //}
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
        public ICommand CalculateVRP2 { get; }

        private void ExecuteCalculateVRP2(object obj)
        {
          

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

            if (F7key == "ItemCode")
            {
                ClResultsdata.Diagram1.Item = (SelectedItem2 as ItemData);
            }
            if (F7key == "Workcenter")
            {
                ClResultsdata.Diagram2.Workcenter = (SelectedItem2 as WorkcenterData);
            }

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

        #region Συνάρτηση Διαγράμμος

        #region Diagram 1
        public List<string> Stock { get; set; }
        public ICommand CreateDiagram1Command { get; }

        private void ExecuteCreateDiagram1Command(object obj)
        {
            //try
            //{
            //    var DiagramData_1 = new ObservableCollection<DataPerDayMRP>();
            //    var XData = new ObservableCollection<DecisionVariableX>();

            //    #region Κατασκευη linechart με 3 γραμμές
            //    if (!string.IsNullOrWhiteSpace(OutputData.Diagram1.Item.ItemCode))
            //    {
            //        DiagramData_1 = OutputData.Diagram1.DataPerDayMRP.Where(item => item.ItemCode == OutputData.Diagram1.Item.ItemCode).ToObservableCollection();
            //        XData = OutputData.XData.Where(item => item.ItemCode == OutputData.Diagram1.Item.ItemCode).ToObservableCollection();
            //    }
            //    else
            //    {
            //        MessageBox.Show("Error: ItemCode is null or whitespace.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            //    }


            //    OutputData.Diagram1.SeriesCollection = new SeriesCollection();

            //    // Add "Demand" series
            //    var demandSeries = new LineSeries
            //    {
            //        Title = "Demand",
            //        Values = new ChartValues<ObservableValue>(
            //            DiagramData_1.Select(d => new ObservableValue(d.Demand))),
            //    };
            //    OutputData.Diagram1.SeriesCollection.Add(demandSeries);

            //    // Add "Make" series
            //    var makeSeries = new ColumnSeries
            //    {
            //        Title = "Make",
            //        Values = new ChartValues<double>(DiagramData_1.Select(d => Convert.ToDouble(d.Make))),
            //    };
            //    OutputData.Diagram1.SeriesCollection.Add(makeSeries);

            //    //// Add "Make2" series
            //    //var make2Series = new StackedColumnSeries
            //    //{
            //    //    Title = "Make2",
            //    //    Values = new ChartValues<double>(XData.Select(d => Convert.ToDouble(d.Value))),
            //    //    StackMode = StackMode.Values
            //    //};
            //    //OutputData.Diagram1.SeriesCollection.Add(make2Series);

            //    // Add "Store" series
            //    var storeSeries = new ColumnSeries
            //    {
            //        Title = "Store",
            //        Values = new ChartValues<ObservableValue>(
            //            DiagramData_1.Select(d => new ObservableValue(d.Stock))),
            //    };
            //    OutputData.Diagram1.SeriesCollection.Add(storeSeries);

            //    // Add "BackLog" series
            //    var backlogSeries = new ColumnSeries
            //    {
            //        Title = "BackLog",
            //        Values = new ChartValues<ObservableValue>(
            //            DiagramData_1.Select(d => new ObservableValue(d.Backlog))),
            //    };
            //    OutputData.Diagram1.SeriesCollection.Add(backlogSeries);



            //    // Set the labels for the x-axis to the months in the data
            //    OutputData.Diagram1.Labels = DiagramData_1.Select(d => d.Date).ToArray();

            //    OutputData.Diagram1.YFormatter = value => value.ToString("N0");
            //    #endregion
            //}
            //catch
            //{
            //    Console.WriteLine("An error occurred");

            //}

        }
        #endregion

        #region Diagram 2
        public ICommand CreateDiagram2Command { get; }

        private void ExecuteCreateDiagram2Command(object obj)
        {
            //try
            //{
            //    var DiagramData_2 = new ObservableCollection<DataPerDayMRP>();
            //    var selectedworkcenter = OutputData.Diagram2.Workcenter.WorkCode;




            //    OutputData.Diagram2.SeriesCollection = new SeriesCollection();

            //    // Add "Demand" series

            //    var demandSeries = new LineSeries
            //    {
            //        Title = "Capacity",
            //        Values = new ChartValues<ObservableValue>(
            //InputData.Awt.Where(d => d.Key.Item1 == selectedworkcenter).Select(d => new ObservableValue(d.Value))),
            //    };

            //    //OutputData.Diagram2.SeriesCollection.Add(demandSeries);

            //    #region Teliko

            //    var WorkXData = OutputData.XData
            //        .Where(d => d.WorkCenter == selectedworkcenter && d.Value > 0);

            //    var WorkYData = OutputData.YData
            //        .Where(d => d.WorkCenter == selectedworkcenter && d.Value > 0);

            //    Dictionary<(string, string, string), (double, double)> Diagram2Dict = new Dictionary<(string, string, string), (double, double)>();

            //    foreach (var x in WorkXData)
            //    {
            //        var speficicYData = WorkYData.Where(d => d.ItemCodeTo == x.ItemCode && d.Date == x.Date);
            //        foreach (var y in speficicYData)
            //        {
            //            double UiwValue = InputData.Uiw[(x.ItemCode, selectedworkcenter)];
            //            double X_U = x.Value * UiwValue;

            //            double SijwValue = InputData.Sijw[(y.ItemCodeFrom, x.ItemCode, selectedworkcenter)];
            //            double Y_S = y.Value * SijwValue;

            //            var key = (y.ItemCodeFrom, x.ItemCode, x.Date);

            //            if (Diagram2Dict.ContainsKey(key))
            //            {
            //                var (existingX_U, existingY_S) = Diagram2Dict[key];
            //                Diagram2Dict[key] = (existingX_U + X_U, existingY_S + Y_S);
            //            }
            //            else
            //            {
            //                Diagram2Dict.Add(key, (X_U, Y_S));
            //            }
            //        }
            //    }
            //    foreach (var kvp in Diagram2Dict)
            //    {
            //        var key = kvp.Key;
            //        var values = kvp.Value;

            //        Console.WriteLine($"Key: {key.Item1}, {key.Item2}, {key.Item3} - Values: {values.Item1}, {values.Item2}");
            //    }
            //    #endregion

            //    var filteredList = OutputData.XData
            //        .Where(d => d.WorkCenter == selectedworkcenter && d.Value > 0)
            //        .GroupBy(d => d.Date)
            //        .Select(group => new
            //        {
            //            Date = group.Key,
            //            SumValue = group.Sum(item => item.Value)
            //        })
            //        .ToList(); // To materialize the query and convert it to a list

            //    var AverageUjw = InputData.Uiw.Sum(item => item.Value) / InputData.Uiw.Count();

            //    //var makeSeries = new ColumnSeries
            //    //{
            //    //    Title = "Make",
            //    //    Values = new ChartValues<double>(filteredList.Select(item => item.SumValue) ),
            //    //};
            //    //OutputData.Diagram2.SeriesCollection.Add(makeSeries);
            //    OutputData.Diagram2.Labels = InputData.Dates.ToArray();

            //    OutputData.Diagram2.YFormatter = value => value.ToString("N0");


            //    foreach (var date in InputData.Dates)
            //    {
            //        var stackedColumnSeries1 = new ColumnSeries
            //        {
            //            Values = new ChartValues<double>(),
            //            Title = "Production Time"

            //        };
            //        var stackedColumnSeries2 = new ColumnSeries
            //        {
            //            Values = new ChartValues<double>(),
            //            Title = "Setup Time"

            //        };

            //        var Dict = Diagram2Dict.Where(kv => kv.Key.Item3 == date);
            //        foreach (var kvp in Dict)
            //        {
            //            var Values = kvp.Value;
            //            stackedColumnSeries1.Title = kvp.Key.Item1 + "_X";
            //            stackedColumnSeries1.Values.Add(Values.Item1);

            //            stackedColumnSeries2.Title = kvp.Key.Item2 + "_Y";
            //            stackedColumnSeries2.Values.Add(Values.Item2);

            //        }

            //        OutputData.Diagram2.SeriesCollection.Add(stackedColumnSeries1);

            //    }

            //}
            //catch
            //{
            //    Console.WriteLine("An error occurred: ");
            //}

        }
        #endregion

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
