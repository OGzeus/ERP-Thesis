using Erp.Model.SupplyChain.Clusters;
using Erp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using System.Collections.ObjectModel;
using static Erp.CommonFiles.ML_AI_Functions;
using System.Windows.Forms;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using static Erp.Model.SupplyChain.Clusters.KmeansOutputData;
using Erp.Model.BasicFiles;
using Erp.Model.SupplyChain;
using System.Windows.Media;
using Erp.Model.SupplyChain.VRP;

namespace Erp.CommonFiles
{
    public class ML_AI_Functions : RepositoryBase
    {
        #region Clustering

        public class DataPoint
        {
            public float Latitude { get; set; }
            public float Longitude { get; set; }
        }
        public class ClusteredDataPoint
        {
            public float Latitude { get; set; }
            public float Longitude { get; set; }

            [ColumnName("PredictedLabel")]
            public uint Cluster { get; set; }
        }

        public void CreateShapefile(KmeansOutputData outputData, string filePath)
        {
            try
            {
                // Create a feature collection
                var featureCollection = new FeatureCollection();

                // Add cluster centers and data points as features
                foreach (var cluster in outputData.Clusters)
                {
                    // Add the cluster center
                    var clusterCenterGeometry = new Point(new Coordinate(cluster.CentroidLongitude, cluster.CentroidLatitude));
                    var clusterCenterAttributes = new AttributesTable
            {
                { "Type", "ClusterCtr" }, // Shortened to "ClusterCtr" to fit within 10 characters
                { "ClusId", cluster.ClusterId } // Shortened from "ClusterId"
            };
                    var clusterCenterFeature = new Feature(clusterCenterGeometry, clusterCenterAttributes);
                    featureCollection.Add(clusterCenterFeature);

                    // Add data points in this cluster
                    foreach (var point in cluster.DataPoints)
                    {
                        var pointGeometry = new Point(new Coordinate(point.Longitude, point.Latitude));
                        var pointAttributes = new AttributesTable
                {
                    { "Type", "DataPoint" },
                    { "ClusId", cluster.ClusterId } // Shortened from "ClusterId"
                };
                        var pointFeature = new Feature(pointGeometry, pointAttributes);
                        featureCollection.Add(pointFeature);
                    }
                }

                // Write the feature collection to Shapefile
                var shapeFileWriter = new ShapefileDataWriter(filePath, new GeometryFactory())
                {
                    Header = new DbaseFileHeader()
                };
                shapeFileWriter.Header.AddColumn("Type", 'C', 10, 0); // Shortened from "Cluster Center"
                shapeFileWriter.Header.AddColumn("ClusId", 'N', 10, 0); // Shortened from "ClusterId"
                shapeFileWriter.Write(featureCollection);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public void CreateShpFile(KmeansOutputData outputData, string filePath)
        {
            string filePath2 = @"C:\Users\npoly\OneDrive\Υπολογιστής\diagrams_syncfusion\output.shp";
            var featureCollection = new FeatureCollection();

            // Add features to the collection
            foreach (var cluster in outputData.Clusters)
            {
                foreach (var point in cluster.DataPoints)
                {
                    // Create a point geometry
                    var geometry = new Point(new Coordinate(point.Longitude, point.Latitude));

                    // Create attributes
                    // Create attributes table
                    var attributesTable = new AttributesTable();
                    attributesTable.Add("ClusterId", cluster.ClusterId);
                    attributesTable.Add("CentroidCode", cluster.CentroidCode);
                    attributesTable.Add("CentroidLatitude", cluster.CentroidLatitude);
                    attributesTable.Add("CentroidLongitude", cluster.CentroidLongitude);

                    // Create a feature
                    var feature = new Feature(geometry, attributesTable);


                    // Add the feature to the collection
                    featureCollection.Add(feature);
                }
            }

            // Write the feature collection to SHP file
            var shapeFileWriter = new ShapefileDataWriter(filePath2, new GeometryFactory());
            shapeFileWriter.Header = new DbaseFileHeader();
            shapeFileWriter.Header.AddColumn("ClusterId", 'C', 10, 0);
            shapeFileWriter.Header.AddColumn("Code", 'C', 50, 0);
            shapeFileWriter.Header.AddColumn("Latitude", 'N', 18, 8);
            shapeFileWriter.Header.AddColumn("Longitude", 'N', 18, 8);
            shapeFileWriter.Write(featureCollection);
        }
        public KmeansOutputData Calculate_Kmeans_Clustering(Dictionary<string, int> PointCode_Index,KmeansInputData input, Dictionary<string, (double, double)> DataPoints)
        {
            var outputData = new KmeansOutputData();

            try
            {
                var mlContext = new MLContext();
                var dataPointsList = DataPoints.Values.Select(point => new DataPoint { Latitude = (float)point.Item1, Longitude = (float)point.Item2 }).ToList();
                var data = mlContext.Data.LoadFromEnumerable(dataPointsList);

                var options = new KMeansTrainer.Options
                {
                    NumberOfClusters = input.NumberOfClusters,
                    MaximumNumberOfIterations = input.MaxIterations,
                    FeatureColumnName = "Features"
                };

                var pipeline = mlContext.Transforms.Concatenate("Features", nameof(DataPoint.Latitude), nameof(DataPoint.Longitude))
                                  .Append(mlContext.Clustering.Trainers.KMeans(options));

                var model = pipeline.Fit(data);
                var predictions = model.Transform(data);
                var clusteredData = mlContext.Data.CreateEnumerable<ClusteredDataPoint>(predictions, reuseRowObject: false).ToList();

                foreach (var point in clusteredData)
                {
                    var cluster = outputData.Clusters.FirstOrDefault(c => c.ClusterId == point.Cluster);
                    if (cluster == null)
                    {
                        cluster = new KmeansOutputData.ClusterData();


                        cluster.ClusterId = (int)point.Cluster;
                        cluster.CentroidCode = cluster.ClusterId.ToString(); 
                        cluster.CentroidLatitude = point.Latitude;
                        cluster.CentroidLongitude = point.Longitude;
                        cluster.DataPoints = new ObservableCollection<MainDatapoint>();
                      
                        outputData.Clusters.Add(cluster);
                    }

                    var NewPoint = new MainDatapoint();

                    NewPoint.ClusterCode = cluster.CentroidCode; 
                    NewPoint.Code = DataPoints.FirstOrDefault(dp => dp.Value.Item1 == point.Latitude && dp.Value.Item2 == point.Longitude).Key;
                    NewPoint.Longitude = point.Longitude;
                    NewPoint.Latitude = point.Latitude;
                    cluster.DataPoints.Add(NewPoint);
                }

                return outputData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        #endregion
        #region VRP 

        #region S_ANNEALING
        public VRPResultsData Calculate_Simulation_Annealing(VRP_InputData inputData)
        {
            // Extract data from input
            var vehiclesIndexMap = inputData.Vehicles_IndexMap;
            var vehicleCapacity = inputData.VehicleCapacity;

            var customerIndexMap = inputData.Customer_IndexMap;
            var demand = inputData.Demand;
            var coords = inputData.Coords;

            var depotIndexMap = inputData.Depot_IndexMap;
            var depotCapacity = inputData.DepotCapacity;
            var depotCoords = inputData.DepotCoords;
            var saSettings = inputData.SimAnnealing_InputData;

            // Extract Simulated Annealing settings
            double initialTemp = saSettings.InitialTemp;
            double coolingRate = saSettings.CoolingRate;
            double stoppingTemp = saSettings.StoppingTemp;
            int maxIterations = saSettings.MaxIterations;

            VRPResultsData results = new VRPResultsData();
            results.Routes = new ObservableCollection<RouteVRPData>();

            try
            {
                // Initialize the solution
                var currentSolution = InitializeSolution(demand, coords, vehicleCapacity);
                var bestSolution = currentSolution;
                double currentTemperature = initialTemp;

                int iteration = 0;
                while (currentTemperature > stoppingTemp && iteration < maxIterations)
                {
                    var neighborSolution = GenerateNeighborSolution(currentSolution, demand, vehicleCapacity);
                    double currentCost = CalculateCost(currentSolution, coords);
                    double neighborCost = CalculateCost(neighborSolution, coords);
                    double costDifference = neighborCost - currentCost;

                    if (costDifference < 0 || AcceptanceProbability(costDifference, currentTemperature) > new Random().NextDouble())
                    {
                        currentSolution = neighborSolution;
                        if (neighborCost < CalculateCost(bestSolution, coords))
                        {
                            bestSolution = neighborSolution;
                        }
                    }

                    currentTemperature *= coolingRate;
                    iteration++;
                }

                results.TotalCost = CalculateCost(bestSolution, coords);
                results.Iterations = iteration;

                // Populate routes data
                for (int vehicleIdx = 0; vehicleIdx < bestSolution.Count; vehicleIdx++)
                {
                    var route = bestSolution[vehicleIdx];
                    for (int i = 0; i < route.Count - 1; i++)
                    {
                        results.Routes.Add(new RouteVRPData
                        {
                            VehicleIndex = vehicleIdx + 1,
                            VehicleCode = vehiclesIndexMap.FirstOrDefault(x => x.Value == vehicleIdx + 1).Key,
                            CustomerFrom = customerIndexMap.FirstOrDefault(x => x.Value == route[i]).Key,
                            CustomerTo = customerIndexMap.FirstOrDefault(x => x.Value == route[i + 1]).Key,
                            Distance = CalculateDistance(coords[route[i]], coords[route[i + 1]])
                        });
                    }
                }

                // Return the results data object
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        private List<List<int>> InitializeSolution(Dictionary<int, double> demand, Dictionary<int, (double, double)> coords, Dictionary<int, double> vehicleCapacity)
        {
            try
            {
             // Create initial routes ensuring that the vehicle capacities are not exceeded
            List<List<int>> solution = new List<List<int>>();
            List<int> customers = demand.Keys.ToList();
            Random random = new Random();

            for (int vehicle = 1; vehicle <= vehicleCapacity.Count; vehicle++)
            {
                List<int> route = new List<int> { vehicle }; // Start route with vehicle index
                double capacityLeft = vehicleCapacity[vehicle];
                List<int> customersToRemove = new List<int>();

                foreach (var customer in customers)
                {
                    if (capacityLeft >= demand[customer])
                    {
                        route.Add(customer);
                        capacityLeft -= demand[customer];
                        customersToRemove.Add(customer);
                    }
                }

                route.Add(vehicle); // End route with vehicle index
                solution.Add(route);

                foreach (var customer in customersToRemove)
                {
                    customers.Remove(customer);
                }
            }

            return solution;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }

        }

        private List<List<int>> GenerateNeighborSolution(List<List<int>> currentSolution, Dictionary<int, double> demand, Dictionary<int, double> vehicleCapacity)
        {
            try
            {
                // Perform a swap between two random customers in different routes
                Random random = new Random();
                List<List<int>> neighborSolution = currentSolution.Select(route => route.ToList()).ToList();

                int route1Idx = random.Next(neighborSolution.Count);
                int route2Idx = random.Next(neighborSolution.Count);

                if (route1Idx == route2Idx || neighborSolution[route1Idx].Count <= 2 || neighborSolution[route2Idx].Count <= 2)
                {
                    return neighborSolution;
                }

                int cust1Idx = random.Next(1, neighborSolution[route1Idx].Count - 1);
                int cust2Idx = random.Next(1, neighborSolution[route2Idx].Count - 1);

                int cust1 = neighborSolution[route1Idx][cust1Idx];
                int cust2 = neighborSolution[route2Idx][cust2Idx];

                neighborSolution[route1Idx][cust1Idx] = cust2;
                neighborSolution[route2Idx][cust2Idx] = cust1;

                return neighborSolution;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }

        }

        private double CalculateCost(List<List<int>> solution, Dictionary<int, (double, double)> coords)
        {
            try
            {
                // Calculate total distance for all routes in the solution
                double totalCost = 0;
                foreach (var route in solution)
                {
                    for (int i = 0; i < route.Count - 1; i++)
                    {
                        totalCost += CalculateDistance(coords[route[i]], coords[route[i + 1]]);
                    }
                    // Return to depot
                    totalCost += CalculateDistance(coords[route.Last()], coords[route.First()]);
                }
                return totalCost;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return 0;
            }

        }

        private double CalculateDistance((double, double) point1, (double, double) point2)
        {
            return Math.Sqrt(Math.Pow(point2.Item1 - point1.Item1, 2) + Math.Pow(point2.Item2 - point1.Item2, 2));
        }

        private double AcceptanceProbability(double costDifference, double currentTemperature)
        {
            // Implement acceptance probability calculation
            return Math.Exp(-costDifference / currentTemperature);
        }

        #endregion

        #endregion
    }
}
