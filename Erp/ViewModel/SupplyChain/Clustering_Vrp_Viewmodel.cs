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
using Accord.MachineLearning;
using Erp.Model.SupplyChain.TSP;
using System.Windows.Data;
using System.Diagnostics.Metrics;
using System.IO;

namespace Erp.ViewModel.SupplyChain
{
    public class Clustering_Vrp_Viewmodel : ViewModelBase
    {

        #region DataProperties

        #region First Diagrams

        private string _XAxisTitle;
        public string XAxisTitle
        {
            get { return _XAxisTitle; }
            set
            {
                _XAxisTitle = value;
                INotifyPropertyChanged(nameof(XAxisTitle));
            }
        }

        private string _YAxisTitle;
        public string YAxisTitle
        {
            get { return _YAxisTitle; }
            set
            {
                _YAxisTitle = value;
                INotifyPropertyChanged(nameof(YAxisTitle));
            }
        }
        #endregion

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

        private TSP_OutputData _TSP_ResultsData;
        public TSP_OutputData TSP_ResultsData
        {
            get { return _TSP_ResultsData; }
            set
            {
                _TSP_ResultsData = value;
                INotifyPropertyChanged(nameof(TSP_ResultsData));


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
        private Columns _SfGridColumnsTSPD;
        public Columns SfGridColumnsTSPD
        {
            get { return _SfGridColumnsTSPD; }
            set
            {
                this._SfGridColumnsTSPD = value;
                INotifyPropertyChanged("SfGridColumnsTSPD");
            }
        }

        public int T { get; set; }
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

        public BasicEnums.TSP_Techniques[] TSP_Enums
        {
            get { return (BasicEnums.TSP_Techniques[])Enum.GetValues(typeof(BasicEnums.TSP_Techniques)); }
        }
        #endregion

        public Clustering_Vrp_Viewmodel()
        {
            XAxisTitle = "Number Of Clusters";
            #region Αρχικοποιηση Data

            InputData = new CL_Vrp_InputData();
            InputData.CityData = new ObservableCollection<CityData>();
            InputData.CL_VehicleDataGrid = new ObservableCollection<Cluster_Vehicles_Data>();

            #region Clustering

            InputData.CLInputData = new CL_InputData();

            InputData.CLInputData.KmeansInputData = new KmeansInputData();
            InputData.CLInputData.KmeansInputData.MaxIterations = 1000000;
            InputData.CLInputData.KmeansInputData.NumberOfClusters = 5;

            InputData.CLInputData.DBSCAN_InputData = new DBSCAN_InputData();
            InputData.CLInputData.DBSCAN_InputData.Epsilon = 50;
            InputData.CLInputData.DBSCAN_InputData.MinPoints = 3;


            InputData.CLInputData.HierInputData = new HierarchicalInputData();
            ClResultsdata = new CL_OutputData();
            ClResultsdata.Clusters = new ObservableCollection<ClusterDatapoint>();
            ClResultsdata.DataPoints = new ObservableCollection<MainDatapoint>();
            ClResultsdata.Clustering_Diagram = new DiagramClusteringData();

            #endregion
            #region TSP
            InputData.TSPInputData = new TSP_InputData();

            InputData.TSPInputData.SAnnealing_InputData = new SAnnealing_TSP_InputData();
            InputData.TSPInputData.SAnnealing_InputData.InitialTemp = 5000;
            InputData.TSPInputData.SAnnealing_InputData.CoolingRate = 0.9;
            InputData.TSPInputData.SAnnealing_InputData.StoppingTemp = 50;
            InputData.TSPInputData.SAnnealing_InputData.MaxIterations = 5000;


            InputData.TSPInputData.AntColony_InputData = new AntColony_TSP_InputData();
            InputData.TSPInputData.AntColony_InputData.Alpha = 1;
            InputData.TSPInputData.AntColony_InputData.Beta = 5;
            InputData.TSPInputData.AntColony_InputData.EvaporationRate = 0.1;
            InputData.TSPInputData.AntColony_InputData.InitialPheromoneLevel = 1;
            InputData.TSPInputData.AntColony_InputData.NumberOfAnts = 10;
            InputData.TSPInputData.AntColony_InputData.NumberOfIterations = 1000;

            TSP_ResultsData = new TSP_OutputData();
            TSP_ResultsData.TSP_DiagramData = new DiagramData();
            TSP_ResultsData.SelectedCluster = new ClusterDatapoint();
            TSP_ResultsData.AntColony_Outputdata = new AntColony_TSP_OutputData();
            TSP_ResultsData.CityTSPResults = new ObservableCollection<City_Tsp_OutputData>(); 

            this.SfGridColumnsTSPD = new Columns();


            #endregion
            #region Vrp 

            InputData.VrpInputData = new VRP_InputData();
            InputData.CL_VehicleData = new ObservableCollection<Cluster_Vehicles_Data>();

            InputData.VrpInputData.SimAnnealing_InputData = new SimAnnealing_InputData();
            InputData.VrpInputData.SimAnnealing_InputData.InitialTemp = 1000;
            InputData.VrpInputData.SimAnnealing_InputData.StoppingTemp = 50;
            InputData.VrpInputData.SimAnnealing_InputData.CoolingRate = 50;
            InputData.VrpInputData.SimAnnealing_InputData.NumberOfDepots = 1;
            InputData.VrpInputData.SimAnnealing_InputData.MaxIterations = 3000;

            #endregion


            InputData.StackPanelEnabled = false;

            GridResultsData = new MrpResultData();


            this.sfGridColumns = new Columns();
            this.SfGridColumnsRepair = new Columns();

            #endregion

            #region Αρχικοποιηση Commands 

            CalculateClustering = new RelayCommand2(ExecuteCalculateClustering);
            CalculateVRP = new RelayCommand2(ExecuteCalculateVRP);
            CalculateTSP = new RelayCommand2(ExecuteCalculateTSP);
            ShowCitiesGridCommand = new RelayCommand2(ExecuteShowCitiesGridCommand);
            ShowVehiclesGridCommand = new RelayCommand2(ExecuteShowVehiclesGridCommand);
            ShowSelectedClustersGridCommand = new RelayCommand2(ExecuteShowSelectedClustersGridCommand);
            CreateTSPDiagramCommand = new RelayCommand2(ExecuteCreateTSPDiagramCommand);
            CalculateFirstDiagrams = new RelayCommand2(ExecuteCalculateFirstDiagrams);

            #endregion

            #region Diagrams

            ClResultsdata.Clustering_Diagram = new DiagramClusteringData();

            this.SfGridColumnsD = new Columns();



            ShowWorkcenterGridCommand = new RelayCommand2(ExecuteShowWorkcenterGridCommand);


            rowDataCommand = new RelayCommand2(ChangeCanExecute);




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
            T = 1;
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
                foreach (var Veh in InputData.HardCodedVehicles)
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

        #region Clustering

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


            #region Print CSVS

            if (T == 0)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save the Dictionaries to CSV files?", "Save Results", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // Define the path to the directory
                    string desktopPath = "C:\\Users\\npoly\\Source\\Repos\\Optimization\\";
                    string directoryPath = Path.Combine(desktopPath, "ML_CSVFiles");

                    // Create the directory if it doesn't exist
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    WriteToCsv(Path.Combine(directoryPath, $"DataPoints.csv"), InputData.CLInputData.DataPoints);
                    WriteToCsv(Path.Combine(directoryPath, $"DataPoints_Int.csv"), InputData.CLInputData.DataPoints_Int);
                    WriteToCsv(Path.Combine(directoryPath, $"City_IndexMap.csv"), InputData.CLInputData.City_Index);
                    T = 1;
                }
                else
                {
                    T = 1;
                }
            }
            else
            {
                T = 1;
            }


            #endregion

            var Cl_Enum = InputData.CLInputData.CL_Enum;
            var Dict = InputData.CLInputData.DataPoints;
            if (Cl_Enum == BasicEnums.Clustering_Techniques.K_means)
            {
               ClResultsdata.Kmeansoutputdata = ML_AiFunctions.Calculate_Kmeans_Clustering(InputData.CLInputData.City_Index,InputData.CLInputData.KmeansInputData, Dict);
                
                ClResultsdata.DataPoints = new ObservableCollection<MainDatapoint>();
                ClResultsdata.Clusters = new ObservableCollection<ClusterDatapoint>();
                foreach (var cl in ClResultsdata.Kmeansoutputdata.Clusters)
                {


                    var clusterpoint = new ClusterDatapoint();

                    clusterpoint.ClusterCode = cl.CentroidCode;
                    clusterpoint.Longitude = Math.Round(cl.CentroidLongitude, 3);
                    clusterpoint.Latitude = Math.Round(cl.CentroidLatitude, 3);


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
            else if (Cl_Enum == BasicEnums.Clustering_Techniques.DBSCAN)
            {
                ClResultsdata.DBSCANoutputdata = ML_AiFunctions.CalculateDBSCANClustering(InputData.CLInputData, InputData.CLInputData.DBSCAN_InputData);

                ClResultsdata.DataPoints = new ObservableCollection<MainDatapoint>();
                ClResultsdata.Clusters = new ObservableCollection<ClusterDatapoint>();

                foreach (var cl in ClResultsdata.DBSCANoutputdata.Clusters)
                {
                    var clusterpoint = new ClusterDatapoint();

                    clusterpoint.ClusterCode = cl.CentroidCode;
                    clusterpoint.Longitude = Math.Round(cl.CentroidLongitude,3);
                    clusterpoint.Latitude = Math.Round(cl.CentroidLatitude, 3);
                    clusterpoint.DaviesBouldinIndex = Math.Round(cl.DaviesBouldinIndex, 3);

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
            ClResultsdata.Clusters.OrderByDescending(cluster => cluster.NumberOfPoints).Where(cluster => cluster.ClusterCode != "Noise").FirstOrDefault().IsSelected = true;
            ExecuteCreateClusteringDiagramCommand(ClResultsdata);


            SelectedTabIndex = 1;
        }

        #endregion

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

                    var DataPointSeries = new ScatterSeries
                    {
                        Title = ClCode,
                        Values = new ChartValues<ObservablePoint>(),
                        PointGeometry = DefaultGeometries.Circle,
                        Fill = ClCode == "Noise" ? System.Windows.Media.Brushes.Black : null,
                        Stroke = ClCode == "Noise" ? System.Windows.Media.Brushes.Black : null
                    };

                    foreach (var Point in DataPointsData)
                    {
                        if (Point.ClusterCode == ClCode)
                        {
                            var Obs_Point = new ObservablePoint
                            {
                                X = Math.Round(Point.Longitude,4),
                                Y = Math.Round(Point.Latitude, 4)
                            };
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
            public List<System.Windows.Media.Color> GenerateColors(int numberOfColors)
            {
                List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
                Random random = new Random();

                for (int i = 0; i < numberOfColors; i++)
                {
                    byte r = (byte)random.Next(256);
                    byte g = (byte)random.Next(256);
                    byte b = (byte)random.Next(256);
                    colors.Add(System.Windows.Media.Color.FromRgb(r, g, b));
                }

                return colors;
            }
        }


        #endregion

        #region Calculate First Diagrams
        public ICommand CalculateFirstDiagrams { get; }

        private void ExecuteCalculateFirstDiagrams(object obj)
        {
            var Cl_Enum = InputData.CLInputData.CL_Enum;

            if (Cl_Enum == BasicEnums.Clustering_Techniques.K_means)
            {
                List<double> clusterNumbers = new List<double> { 3, 4, 5, 6, 7, 8 };
                List<double> silhouetteScores = new List<double>();
                List<double> dbIndexes = new List<double>();

                foreach (var clusterNumber in clusterNumbers)
                {
                    InputData.CLInputData.KmeansInputData.NumberOfClusters = (int)clusterNumber;
                    ExecuteCalculateClustering(obj);

                    silhouetteScores.Add(ClResultsdata.Kmeansoutputdata.SilhouetteScore_Avg);
                    dbIndexes.Add(ClResultsdata.Kmeansoutputdata.DaviesBouldinIndex_Avg);
                }
                XAxisTitle = "Number Of Clusters";

                CreateSilhouetteDiagram(clusterNumbers, silhouetteScores, "K-means");
                CreateDaviesBouldinIndexDiagram(clusterNumbers, dbIndexes, "K-means");
            }
            else if (Cl_Enum == BasicEnums.Clustering_Techniques.DBSCAN)
            {
                List<double> epsilons = new List<double> { 40, 80, 100, 120, 140 };
                List<double> silhouetteScores = new List<double>();
                List<double> dbIndexes = new List<double>();

                foreach (var epsilon in epsilons)
                {
                    InputData.CLInputData.DBSCAN_InputData.Epsilon = epsilon;
                    ExecuteCalculateClustering(obj);

                    silhouetteScores.Add(ClResultsdata.DBSCANoutputdata.SilhouetteScore_Avg);
                    dbIndexes.Add(ClResultsdata.DBSCANoutputdata.DaviesBouldinIndex_Avg);
                }
                XAxisTitle = "eps";
                CreateSilhouetteDiagram(epsilons, silhouetteScores, "DBSCAN");
                CreateDaviesBouldinIndexDiagram(epsilons, dbIndexes, "DBSCAN");
            }
            SelectedTabIndex = 0;
        }

        #endregion

        #region Helper Methods for Creating Diagrams



        private void CreateSilhouetteDiagram(List<double> xValues, List<double> silhouetteScores, string algorithm)
        {
            var seriesCollection = new SeriesCollection
    {
        new LineSeries
        {
            Title = "Silhouette Scores",
            Values = new ChartValues<double>(silhouetteScores),
            PointGeometry = DefaultGeometries.Circle,
            StrokeThickness = 2,
            Fill = System.Windows.Media.Brushes.Transparent
        }
    };

            ClResultsdata.SilhouetteScores_Diagram.SeriesCollection = seriesCollection;
            ClResultsdata.SilhouetteScores_Diagram.Labels = xValues.Select(x => x.ToString()).ToArray();
            ClResultsdata.SilhouetteScores_Diagram.Formatter = value => value.ToString("N0");
        }

        private void CreateDaviesBouldinIndexDiagram(List<double> xValues, List<double> dbIndexes, string algorithm)
        {
            var seriesCollection = new SeriesCollection
    {
        new LineSeries
        {
            Title = "Davies-Bouldin Index",
            Values = new ChartValues<double>(dbIndexes),
            PointGeometry = DefaultGeometries.Circle,
            StrokeThickness = 2,
            Fill = System.Windows.Media.Brushes.Transparent
        }
    };

            ClResultsdata.DaviesBouldinIndex_Diagram.SeriesCollection = seriesCollection;
            ClResultsdata.DaviesBouldinIndex_Diagram.Labels = xValues.Select(x => x.ToString()).ToArray();
            ClResultsdata.DaviesBouldinIndex_Diagram.Formatter = value => value.ToString("N0");
        }
        #endregion


        #endregion
        #region TSP

        #region Calculate TSP
        public ICommand CalculateTSP { get; }
        private void ExecuteCalculateTSP(object obj)
        {
            #region Αρχικοποιηση

            TSP_ResultsData = new TSP_OutputData();
            TSP_ResultsData.TSP_DiagramData = new DiagramData();
            TSP_ResultsData.SelectedCluster = new ClusterDatapoint();
            TSP_ResultsData.AntColony_Outputdata = new AntColony_TSP_OutputData();
            TSP_ResultsData.CityTSPResults = new ObservableCollection<City_Tsp_OutputData>();


            InputData.TSPInputData.City_IndexMap = new Dictionary<string, int>();
            InputData.TSPInputData.Coords = new Dictionary<int, (double, double)>();

            var City_IndexMap = new Dictionary<string, int>();
            var Coords = new Dictionary<int, (double, double)>();


            #endregion

            var SelectedClusters = ClResultsdata.Clusters.Where(cluster => cluster.IsSelected).OrderByDescending(cluster => cluster.NumberOfPoints);
            var SelectedCluster = "";
             
            foreach (var Cluster in SelectedClusters)
            {
                #region Αρχικοποιηση Dictionaries

                City_IndexMap = new Dictionary<string, int>();
                Coords = new Dictionary<int, (double, double)>();
                #endregion
                 SelectedCluster = Cluster.ClusterCode;

                var CL_Points = new ObservableCollection<MainDatapoint>(
                    ClResultsdata.DataPoints.Where(cl => cl.ClusterCode == SelectedCluster)
                );

                int PointCounter = 0;

                foreach (var Point in CL_Points)
                {
                    City_IndexMap.Add(Point.Code, PointCounter);
                    Coords.Add(PointCounter, (Point.Latitude, Point.Longitude));

                    PointCounter++;
                }


                InputData.TSPInputData.City_IndexMap = City_IndexMap;
                InputData.TSPInputData.Coords = Coords;

                #region Print CSVS


                var TSP_Enum = InputData.TSPInputData.TSP_Enum;
                if (TSP_Enum == BasicEnums.TSP_Techniques.Simulation_Annealing)
                {
                    var SAnnealing_OutputData = ML_AiFunctions.Calculate_SAnnealing_TSP(InputData.TSPInputData);

                    TSP_ResultsData.SAnnealing_Outputdata = SAnnealing_OutputData;


                    int NumberVisited = 1;

                    var CurrentCluster = ClResultsdata.Clusters.Where(cluster => cluster.ClusterCode == SelectedCluster).FirstOrDefault();

                    foreach (var CityIndex in SAnnealing_OutputData.BestTour)
                    {
                        var Row = new City_Tsp_OutputData();
                        Row.City = new CityData();
                        Row.City.CityCode = City_IndexMap.FirstOrDefault(x => x.Value == CityIndex).Key;
                        Row.City = InputData.CityData.Where(x => x.CityCode == Row.City.CityCode).FirstOrDefault();
                        Row.Number_Visited = NumberVisited;
                        Row.BestTourLength = Math.Round(SAnnealing_OutputData.BestTourLength,3);
                        Row.Cluster = CurrentCluster;
                        NumberVisited++;
                        TSP_ResultsData.CityTSPResults.Add(Row);
                    }

                }
                else if (TSP_Enum == BasicEnums.TSP_Techniques.Ant_Colony_Optimization)
                {
                    var AntColony_OutputData = ML_AiFunctions.Calculate_AntColony_TSP(InputData.TSPInputData);

                    TSP_ResultsData.AntColony_Outputdata = AntColony_OutputData;


                    int NumberVisited = 1;

                    var CurrentCluster = ClResultsdata.Clusters.Where(cluster => cluster.ClusterCode == SelectedCluster).FirstOrDefault();

                    foreach (var CityIndex in AntColony_OutputData.BestTour)
                    {
                        var Row = new City_Tsp_OutputData();
                        Row.City = new CityData();
                        Row.City.CityCode = City_IndexMap.FirstOrDefault(x => x.Value == CityIndex).Key;
                        Row.City = InputData.CityData.Where(x => x.CityCode == Row.City.CityCode).FirstOrDefault();
                        Row.Number_Visited = NumberVisited;
                        Row.BestTourLength = Math.Round(AntColony_OutputData.BestTourLength,3);
                        Row.Cluster = CurrentCluster;
                        NumberVisited++;
                        TSP_ResultsData.CityTSPResults.Add(Row);
                    }

                }
                else if (TSP_Enum == BasicEnums.TSP_Techniques.Optimization)
                {
                    var Optimization_OutputData = ML_AiFunctions.Calculate_Optimization_TSP(InputData.TSPInputData);


                    int NumberVisited = 1;

                    var CurrentCluster = ClResultsdata.Clusters.Where(cluster => cluster.ClusterCode == SelectedCluster).FirstOrDefault();

                    foreach (var CityIndex in Optimization_OutputData.BestTour)
                    {
                        var Row = new City_Tsp_OutputData();
                        Row.City = new CityData();
                        Row.City.CityCode = City_IndexMap.FirstOrDefault(x => x.Value == CityIndex).Key;
                        Row.City = InputData.CityData.Where(x => x.CityCode == Row.City.CityCode).FirstOrDefault();
                        Row.Number_Visited = NumberVisited;
                        Row.BestTourLength = Optimization_OutputData.BestTourLength;
                        Row.Cluster = CurrentCluster;
                        NumberVisited++;
                        TSP_ResultsData.CityTSPResults.Add(Row);
                    }

                }

            }
            if (T == 0)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save the Dictionaries to CSV files?", "Save Results", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // Define the path to the directory
                    string desktopPath = "C:\\Users\\npoly\\Source\\Repos\\Optimization\\";
                    string directoryPath = Path.Combine(desktopPath, "ML_CSVFiles");

                    // Create the directory if it doesn't exist
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    WriteToCsv(Path.Combine(directoryPath, $"Coords.csv"), InputData.TSPInputData.Coords);
                    WriteToCsv(Path.Combine(directoryPath, $"City_IndexMap_TSP.csv"), InputData.TSPInputData.City_IndexMap);

                }
            }
            SelectedTabIndex = 3;
            TSP_ResultsData.TSP_DiagramData = new DiagramData();
            TSP_ResultsData.SelectedCluster = new ClusterDatapoint();
        }

        #endregion


        #endregion

        #region TSP F7 Diagrams

        #region F7
        public ICommand ShowSelectedClustersGridCommand { get; }

        private void ExecuteShowSelectedClustersGridCommand(object obj)
        {


            ClearColumns();

            var F7input = F7Common.F7SelectedClusters();
            F7key = F7input.F7key;

            var SelecteDClusters = ClResultsdata.Clusters.Where(cl => cl.IsSelected == true);

            F7input.CollectionView = CollectionViewSource.GetDefaultView(SelecteDClusters);

            CollectionView = F7input.CollectionView;
            var a = F7input.SfGridColumns;


            foreach (var item in a)
            {
                this.SfGridColumns.Add(item);
            }


        }

        #endregion

        #region TSP Diagrams

        public ICommand CreateTSPDiagramCommand { get; }

        private void ExecuteCreateTSPDiagramCommand(object obj)
        {
            try
            {
                var XData = new ObservableCollection<DecisionVariableX>();

                #region Κατασκευη linechart με 3 γραμμές

                var Data = TSP_ResultsData.CityTSPResults
                            .Where(city => city.Cluster.ClusterCode == TSP_ResultsData.SelectedCluster.ClusterCode)
                            .OrderBy(d => d.Number_Visited)
                            .ToList();

                TSP_ResultsData.TSP_DiagramData.SeriesCollection = new SeriesCollection();

                var ClCode = TSP_ResultsData.SelectedCluster.ClusterCode;

                // ScatterSeries for displaying the points
                var DataPointSeries = new ScatterSeries
                {
                    Title = ClCode,
                    Values = new ChartValues<ObservablePoint>(),
                    PointGeometry = DefaultGeometries.Circle,
                    DataLabels = true,
                    MinPointShapeDiameter = 35,
                    FontSize = 20,
                    LabelPoint = point =>
                    {
                        // Find the corresponding city by matching the coordinates
                        var matchingCity = Data.FirstOrDefault(city =>
                            Math.Round(city.City.Longitude, 4) == point.X &&
                            Math.Round(city.City.Latitude, 4) == point.Y);

                        // Return the Number_Visited as the label if a matching city is found
                        return matchingCity != null ? matchingCity.Number_Visited.ToString() : string.Empty;
                    },

                };

                // LineSeries for connecting the points
                var LineSeries = new LineSeries
                {
                    Title = ClCode + " Path",
                    Values = new ChartValues<ObservablePoint>(),
                    PointGeometry = null, // No points, just lines
                    Stroke = System.Windows.Media.Brushes.Black, // Set line color to black
                    StrokeThickness = 2
                };

                for (int i = 0; i < Data.Count; i++)
                {
                    var row = Data[i];

                    var Obs_Point = new ObservablePoint
                    {
                        X = Math.Round(row.City.Longitude, 4),
                        Y = Math.Round(row.City.Latitude, 4) // Corrected to Latitude
                    };

                    DataPointSeries.Values.Add(Obs_Point);
                    LineSeries.Values.Add(Obs_Point);
                }

                // To connect the last city back to the first city (completing the TSP cycle)
                if (Data.Count > 0)
                {
                    var firstRow = Data[0];
                    var firstPoint = new ObservablePoint
                    {
                        X = Math.Round(firstRow.City.Longitude, 4),
                        Y = Math.Round(firstRow.City.Latitude, 4) // Corrected to Latitude
                    };
                    LineSeries.Values.Add(firstPoint);
                }

                TSP_ResultsData.TSP_DiagramData.SeriesCollection.Add(LineSeries);
                TSP_ResultsData.TSP_DiagramData.SeriesCollection.Add(DataPointSeries);

                TSP_ResultsData.TSP_DiagramData.YFormatter = value => value.ToString("N0");

                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        #endregion

        public void ChangeCanExecute(object obj)
        {

            if (F7key == "SelectedCluster")
            {
                TSP_ResultsData.SelectedCluster = (SelectedItem as ClusterDatapoint);
            }


        }

        #endregion
        #endregion

        #region VRP

        #region Calculate VRP

        public ICommand CalculateVRP { get; }

        private void ExecuteCalculateVRP(object obj)
        {
            var clusters = new ObservableCollection<ClusterDatapoint>();
            clusters.Add(ClResultsdata.Clusters.FirstOrDefault());

            foreach (var Cluster in clusters)
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

        #region VRP Diagrams

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

        #region Write To CSV
        static Dictionary<TKey, TValue> SortDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
            return dict.OrderBy(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        static void WriteToCsv(string fileName, Dictionary<string, int> dictionary)
        {
            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLine("Key,Value");
                foreach (var kvp in dictionary)
                {
                    writer.WriteLine($"{kvp.Key},{kvp.Value}");
                }
            }
        }


        static void WriteToCsv(string fileName, Dictionary<string, (double, double)> dictionary)
        {
            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLine("Key,Value1,Value2");
                foreach (var kvp in dictionary)
                {
                    writer.WriteLine($"{kvp.Key},{kvp.Value.Item1},{kvp.Value.Item2}");
                }
            }
        }

        static void WriteToCsv(string fileName, Dictionary<int, (double, double)> dictionary)
        {
            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLine("Key,Value1,Value2");
                foreach (var kvp in dictionary)
                {
                    writer.WriteLine($"{kvp.Key},{kvp.Value.Item1},{kvp.Value.Item2}");
                }
            }
        }


        #endregion
    }
}
