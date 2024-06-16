using Erp.Model.SupplyChain.Clusters;
using Erp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using System.Collections.ObjectModel;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Erp.Model.SupplyChain;
using Erp.Model.SupplyChain.VRP;
using Erp.Model.SupplyChain.TSP;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines;
using Accord.Math;
using Accord.Statistics;

namespace Erp.CommonFiles
{
    public class ML_AI_Functions : RepositoryBase
    {
        #region Clustering

        #region KmeansClustering
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


        public KmeansOutputData Calculate_Kmeans_Clustering(Dictionary<string, int> PointCode_Index, KmeansInputData input, Dictionary<string, (double, double)> DataPoints)
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

                var reverseLookup = DataPoints.ToDictionary(dp => (float)dp.Value.Item1, dp => dp.Key);

                double totalInertia = 0;
                var silhouetteScores = new List<double>();

                // Calculate CH Index variables
                double betweenClusterSumOfSquares = 0;
                double withinClusterSumOfSquares = 0;

                // Initialize clusters
                var clustersDict = new Dictionary<uint, KmeansOutputData.ClusterData>();

                foreach (var point in clusteredData)
                {
                    if (!clustersDict.ContainsKey(point.Cluster))
                    {
                        clustersDict[point.Cluster] = new KmeansOutputData.ClusterData
                        {
                            ClusterId = (int)point.Cluster,
                            CentroidCode = point.Cluster.ToString(),
                            DataPoints = new ObservableCollection<MainDatapoint>()
                        };
                    }

                    var newPoint = new MainDatapoint
                    {
                        ClusterCode = point.Cluster.ToString(),
                        Code = reverseLookup[(float)point.Latitude],
                        Latitude = Math.Round(point.Latitude, 4),
                        Longitude = Math.Round(point.Longitude, 4)
                    };
                    clustersDict[point.Cluster].DataPoints.Add(newPoint);
                }

                // Calculate centroids
                foreach (var cluster in clustersDict.Values)
                {
                    var centroidLatitude = cluster.DataPoints.Average(dp => dp.Latitude);
                    var centroidLongitude = cluster.DataPoints.Average(dp => dp.Longitude);

                    cluster.CentroidLatitude = Math.Round(centroidLatitude, 4);
                    cluster.CentroidLongitude = Math.Round(centroidLongitude, 4);
                }

                // Calculate metrics
                foreach (var point in clusteredData)
                {
                    var cluster = clustersDict[point.Cluster];

                    // Calculate the distance to the centroid for inertia
                    var distanceToCentroid = Math.Sqrt(Math.Pow(point.Latitude - cluster.CentroidLatitude, 2) + Math.Pow(point.Longitude - cluster.CentroidLongitude, 2));
                    totalInertia += distanceToCentroid;

                    // Compute silhouette score
                    silhouetteScores.Add(ComputeSilhouetteScore(point, clusteredData));

                    // Update withinClusterSumOfSquares
                    withinClusterSumOfSquares += Math.Pow(distanceToCentroid, 2);
                }

                // Calculate betweenClusterSumOfSquares
                var overallCentroid = new DataPoint
                {
                    Latitude = (float)dataPointsList.Average(dp => dp.Latitude),
                    Longitude = (float)dataPointsList.Average(dp => dp.Longitude)
                };

                foreach (var cluster in clustersDict.Values)
                {
                    var centroidDistance = Math.Sqrt(Math.Pow(cluster.CentroidLatitude - overallCentroid.Latitude, 2) +
                                                     Math.Pow(cluster.CentroidLongitude - overallCentroid.Longitude, 2));
                    betweenClusterSumOfSquares += cluster.DataPoints.Count * Math.Pow(centroidDistance, 2);
                }

                outputData.Inertia = totalInertia;
                outputData.SilhouetteScore = silhouetteScores.Count > 0 ? silhouetteScores.Average() : 0;

                outputData.Clusters = clustersDict.Values.OrderBy(d => d.ClusterId).ToList();
                return outputData;
            }
            catch (Exception ex)
            {
                // Consider using a logging framework for better error handling in production code
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        // Placeholder function for silhouette score computation
        private double ComputeSilhouetteScore(ClusteredDataPoint point, List<ClusteredDataPoint> allPoints)
        {
            var sameClusterPoints = allPoints.Where(p => p.Cluster == point.Cluster).ToList();
            var otherClusterPoints = allPoints.Where(p => p.Cluster != point.Cluster).ToList();

            double a = sameClusterPoints.Average(p => Distance(point, p));
            double b = otherClusterPoints.GroupBy(p => p.Cluster)
                                         .Min(g => g.Average(p => Distance(point, p)));

            return (b - a) / Math.Max(a, b);
        }

        private double Distance(ClusteredDataPoint p1, ClusteredDataPoint p2)
        {
            return Math.Sqrt(Math.Pow(p1.Latitude - p2.Latitude, 2) + Math.Pow(p1.Longitude - p2.Longitude, 2));
        }



        #endregion

        #region DBSCAN

        // DBSCAN clustering algorithm
        public DBSCAN_OutputData CalculateDBSCANClustering(CL_InputData inputData, DBSCAN_InputData dbscanInput)
        {
            // Convert data points to a 2D array
            double[][] data = inputData.DataPoints_Int.Values
                .Select(v => new double[] { v.Latitude, v.Longitude })
                .ToArray();

            int[] labels = new int[data.Length];

            // Initialize labels array to -1 (indicating all points are initially unclassified)
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i] = -1;
            }

            int clusterId = 0;

            for (int i = 0; i < data.Length; i++)
            {
                if (labels[i] != -1) continue;

                var neighbors = GetNeighbors(i, data, dbscanInput.Epsilon);

                if (neighbors.Count < dbscanInput.MinPoints)
                {
                    labels[i] = -1; // Mark as noise
                }
                else
                {
                    ExpandCluster(data, labels, i, neighbors, clusterId, dbscanInput.Epsilon, dbscanInput.MinPoints);
                    clusterId++;
                }
            }

            // Extract clusters and noise points
            var clusters = new List<List<(string Code, double Latitude, double Longitude)>>();
            var noisePoints = new List<(string Code, double Latitude, double Longitude)>();

            var labeledPoints = inputData.DataPoints.Zip(labels, (point, label) => new { point, label })
                                                     .GroupBy(x => x.label);

            foreach (var group in labeledPoints)
            {
                if (group.Key == -1)
                {
                    noisePoints.AddRange(group.Select(x => (x.point.Key, x.point.Value.Latitude, x.point.Value.Longitude)));
                }
                else
                {
                    clusters.Add(group.Select(x => (x.point.Key, x.point.Value.Latitude, x.point.Value.Longitude)).ToList());
                }
            }

            // Create output structure
            var output = new DBSCAN_OutputData();

            int clusterIndex = 1;
            foreach (var cluster in clusters)
            {
                // Calculate centroid
                double centroidLatitude = cluster.Average(p => p.Latitude);
                double centroidLongitude = cluster.Average(p => p.Longitude);

                // Create cluster data
                var clusterData = new DBSCAN_OutputData.ClusterData
                {
                    ClusterId = clusterIndex,
                    CentroidCode = $"Cluster_{clusterIndex}",
                    CentroidLatitude = centroidLatitude,
                    CentroidLongitude = centroidLongitude,
                    DataPoints = new ObservableCollection<MainDatapoint>(
                        cluster.Select(p => new MainDatapoint
                        {
                            Code = p.Code,
                            Latitude = Math.Round(p.Latitude, 4),
                            Longitude = Math.Round(p.Longitude, 4),
                            ClusterCode = $"Cluster_{clusterIndex}"
                        }).ToList())
                };
                clusterIndex++;
                output.Clusters.Add(clusterData);
            }

            // Add noise cluster
            var noiseClusterData = new DBSCAN_OutputData.ClusterData
            {
                ClusterId = clusterIndex,
                CentroidCode = "Noise",
                CentroidLatitude = 0,
                CentroidLongitude = 0,
                DataPoints = new ObservableCollection<MainDatapoint>(
                    noisePoints.Select(p => new MainDatapoint
                    {
                        Code = p.Code,
                        Latitude = Math.Round(p.Latitude,4),
                        Longitude = Math.Round(p.Longitude, 4),
                        ClusterCode = "Noise"
                    }).ToList())
            };

            output.Clusters.Add(noiseClusterData);

            // Calculate clustering quality metrics
            var SilhouetteScore = CalculateSilhouetteScore(data, labels);
            var DaviesBouldinIndex = CalculateDaviesBouldinIndex(data, labels);
            var NoiseRatio = CalculateNoiseRatio(noisePoints.Count, data.Length);

            output.SilhouetteScore = Math.Round(SilhouetteScore,3);
            output.DaviesBouldinIndex = Math.Round(DaviesBouldinIndex, 3);
            output.NoiseRatio = Math.Round(NoiseRatio, 3);

            return output;
        }

        // Utility function to calculate Euclidean distance between two points
        private static double CalculateDistance((double Latitude, double Longitude) point1, (double Latitude, double Longitude) point2)
        {
            double earthRadiusKm = 6371.0;

            double dLat = DegreesToRadians(point2.Latitude - point1.Latitude);
            double dLon = DegreesToRadians(point2.Longitude - point1.Longitude);

            double lat1 = DegreesToRadians(point1.Latitude);
            double lat2 = DegreesToRadians(point2.Latitude);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return earthRadiusKm * c;
        }
        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        // Utility function to find neighbors within epsilon distance
        private static List<int> GetNeighbors(int pointIndex, double[][] data, double epsilon)
        {
            List<int> neighbors = new List<int>();
            for (int i = 0; i < data.Length; i++)
            {
                if (CalculateDistance((data[pointIndex][0], data[pointIndex][1]), (data[i][0], data[i][1])) <= epsilon)
                {
                    neighbors.Add(i);
                }
            }
            return neighbors;
        }

        private static void ExpandCluster(double[][] data, int[] labels, int pointIndex, List<int> neighbors, int clusterId, double epsilon, int minPoints)
        {
            labels[pointIndex] = clusterId;

            Queue<int> pointsToProcess = new Queue<int>(neighbors);

            while (pointsToProcess.Count > 0)
            {
                int currentPointIndex = pointsToProcess.Dequeue();

                if (labels[currentPointIndex] == -1)
                {
                    labels[currentPointIndex] = clusterId; // Change noise to border point
                }

                if (labels[currentPointIndex] != -1) continue;

                labels[currentPointIndex] = clusterId;

                var currentNeighbors = GetNeighbors(currentPointIndex, data, epsilon);

                if (currentNeighbors.Count >= minPoints)
                {
                    foreach (var neighborIndex in currentNeighbors)
                    {
                        if (!pointsToProcess.Contains(neighborIndex))
                        {
                            pointsToProcess.Enqueue(neighborIndex);
                        }
                    }
                }
            }
        }
        #region Calculate Silhouette,Davies-Bouldin Index,Noise Ratio
        // Function to calculate the silhouette score
        private double CalculateSilhouetteScore(double[][] data, int[] labels)
        {
            double silhouetteSum = 0.0;
            int[] uniqueLabels = labels.Distinct().Where(l => l != -1).ToArray(); // Exclude noise

            for (int i = 0; i < data.Length; i++)
            {
                if (labels[i] == -1) continue; // Skip noise points

                double a = AverageDistanceToCluster(data, labels, i, labels[i]);
                double b = double.MaxValue;

                foreach (int label in uniqueLabels)
                {
                    if (label == labels[i]) continue;
                    double avgDist = AverageDistanceToCluster(data, labels, i, label);
                    if (avgDist < b)
                    {
                        b = avgDist;
                    }
                }

                silhouetteSum += (b - a) / Math.Max(a, b);
            }

            return silhouetteSum / data.Length;
        }

        private double AverageDistanceToCluster(double[][] data, int[] labels, int pointIndex, int clusterLabel)
        {
            double sum = 0.0;
            int count = 0;

            for (int i = 0; i < data.Length; i++)
            {
                if (labels[i] == clusterLabel)
                {
                    sum += CalculateDistance((data[pointIndex][0], data[pointIndex][1]), (data[i][0], data[i][1]));
                    count++;
                }
            }

            return count > 0 ? sum / count : 0;
        }

        // Function to calculate the Davies-Bouldin index
        private double CalculateDaviesBouldinIndex(double[][] data, int[] labels)
        {
            var clusters = labels.Distinct().Where(l => l != -1).ToArray(); // Exclude noise
            double dbIndex = 0.0;
            int clusterCount = clusters.Length;

            for (int i = 0; i < clusterCount; i++)
            {
                double maxRatio = double.MinValue;

                for (int j = 0; j < clusterCount; j++)
                {
                    if (i == j) continue;

                    double si = AverageClusterScatter(data, labels, clusters[i]);
                    double sj = AverageClusterScatter(data, labels, clusters[j]);
                    double dij = CalculateDistance(CalculateCentroid(data, labels, clusters[i]), CalculateCentroid(data, labels, clusters[j]));

                    double ratio = (si + sj) / dij;
                    if (ratio > maxRatio)
                    {
                        maxRatio = ratio;
                    }
                }

                dbIndex += maxRatio;
            }

            return dbIndex / clusterCount;
        }

        private double AverageClusterScatter(double[][] data, int[] labels, int clusterLabel)
        {
            var clusterPoints = data.Where((_, index) => labels[index] == clusterLabel).ToArray();
            var centroid = CalculateCentroid(data, labels, clusterLabel);
            double scatterSum = 0.0;

            foreach (var point in clusterPoints)
            {
                scatterSum += CalculateDistance((point[0], point[1]), centroid);
            }

            return scatterSum / clusterPoints.Length;
        }

        private (double Latitude, double Longitude) CalculateCentroid(double[][] data, int[] labels, int clusterLabel)
        {
            var clusterPoints = data.Where((_, index) => labels[index] == clusterLabel).ToArray();
            double latitudeSum = 0.0;
            double longitudeSum = 0.0;

            foreach (var point in clusterPoints)
            {
                latitudeSum += point[0];
                longitudeSum += point[1];
            }

            return (latitudeSum / clusterPoints.Length, longitudeSum / clusterPoints.Length);
        }

        // Function to calculate the noise ratio
        private double CalculateNoiseRatio(int noisePointsCount, int totalPointsCount)
        {
            return (double)noisePointsCount / totalPointsCount;
        }

        #endregion
        #endregion

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
                            Distance = CalculateVPDistance(coords[route[i]], coords[route[i + 1]])
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

        private double CalculateVPDistance((double, double) point1, (double, double) point2)
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

        #region TSP
        public AntColony_TSP_OutputData Calculate_AntColony_TSP(TSP_InputData InputData)
        {
            var OutputData = new AntColony_TSP_OutputData();

            #region CityData

            var City_IndexMap = new Dictionary<string, int>();
            var Coords = new Dictionary<int, (double, double)>();

            City_IndexMap = InputData.City_IndexMap;
            Coords = InputData.Coords;

            // Calculate the distance matrix
            int n = Coords.Count;
            double[,] distanceMatrix = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i != j)
                    {
                        distanceMatrix[i, j] = CalculateDistance(Coords[i], Coords[j]);
                    }
                    else
                    {
                        distanceMatrix[i, j] = 0; // Distance to itself
                    }
                }
            }

            #endregion

            #region AntColony Settings
            double Alpha = InputData.AntColony_InputData.Alpha;
            double Beta = InputData.AntColony_InputData.Beta;
            double EvaporationRate = InputData.AntColony_InputData.EvaporationRate;
            double InitialPheromoneLevel = InputData.AntColony_InputData.InitialPheromoneLevel;
            int NumberOfAnts = InputData.AntColony_InputData.NumberOfAnts;
            int NumberOfIterations = InputData.AntColony_InputData.NumberOfIterations;
            #endregion

            try
            {
                // Implement Ant Colony Optimization logic here
                double[,] pheromones = new double[n, n];
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        pheromones[i, j] = InitialPheromoneLevel;
                    }
                }

                List<int> bestTour = new List<int>();
                double bestTourLength = double.MaxValue;

                for (int iteration = 0; iteration < NumberOfIterations; iteration++)
                {
                    List<int>[] tours = new List<int>[NumberOfAnts];
                    double[] tourLengths = new double[NumberOfAnts];

                    for (int k = 0; k < NumberOfAnts; k++)
                    {
                        tours[k] = ConstructSolution(distanceMatrix, pheromones, Alpha, Beta);
                        tourLengths[k] = CalculateTourLength(tours[k], distanceMatrix);

                        if (tourLengths[k] < bestTourLength)
                        {
                            bestTourLength = tourLengths[k];
                            bestTour = new List<int>(tours[k]);
                        }
                    }

                    UpdatePheromones(pheromones, tours, tourLengths, EvaporationRate);

                }

                OutputData.BestTour = bestTour;
                OutputData.BestTourLength = bestTourLength;
                OutputData.PheromoneLevels = pheromones;

                return OutputData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return OutputData;
            }
        }

        private List<int> ConstructSolution(double[,] distanceMatrix, double[,] pheromones, double alpha, double beta)
        {
            int n = distanceMatrix.GetLength(0);
            List<int> tour = new List<int>();
            bool[] visited = new bool[n];
            Random rand = new Random();

            int currentCity = rand.Next(n);
            tour.Add(currentCity);
            visited[currentCity] = true;

            for (int step = 1; step < n; step++)
            {
                double[] probabilities = new double[n];
                double sumProbabilities = 0.0;

                for (int j = 0; j < n; j++)
                {
                    if (!visited[j])
                    {
                        probabilities[j] = Math.Pow(pheromones[currentCity, j], alpha) * Math.Pow(1.0 / distanceMatrix[currentCity, j], beta);
                        sumProbabilities += probabilities[j];
                    }
                }

                double randomValue = rand.NextDouble() * sumProbabilities;
                double cumulativeProbability = 0.0;

                for (int j = 0; j < n; j++)
                {
                    if (!visited[j])
                    {
                        cumulativeProbability += probabilities[j];
                        if (cumulativeProbability >= randomValue)
                        {
                            currentCity = j;
                            break;
                        }
                    }
                }

                tour.Add(currentCity);
                visited[currentCity] = true;
            }

            return tour;
        }

        private double CalculateTourLength(List<int> tour, double[,] distanceMatrix)
        {
            double length = 0.0;
            for (int i = 0; i < tour.Count - 1; i++)
            {
                length += distanceMatrix[tour[i], tour[i + 1]];
            }
            length += distanceMatrix[tour[tour.Count - 1], tour[0]]; // Return to start
            return length;
        }

        private void UpdatePheromones(double[,] pheromones, List<int>[] tours, double[] tourLengths, double evaporationRate)
        {
            int n = pheromones.GetLength(0);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    pheromones[i, j] *= (1 - evaporationRate);
                }
            }

            for (int k = 0; k < tours.Length; k++)
            {
                double deltaPheromone = 1.0 / tourLengths[k];
                for (int i = 0; i < tours[k].Count - 1; i++)
                {
                    pheromones[tours[k][i], tours[k][i + 1]] += deltaPheromone;
                    pheromones[tours[k][i + 1], tours[k][i]] += deltaPheromone;
                }
                pheromones[tours[k][tours[k].Count - 1], tours[k][0]] += deltaPheromone;
                pheromones[tours[k][0], tours[k][tours[k].Count - 1]] += deltaPheromone;
            }
        }

        #endregion
    }
}
