using Accord.MachineLearning;
using Erp.Model.Thesis.CrewScheduling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Erp.CommonFiles
{
    public class Nikos_Polychronopoulos_ML_Functions
    {


        #region Clustering

        #region Classes
        public class MainDatapoint
        {
            public string ClusterCode;
            public string Code;
            public double Longitude;
            public double Latitude;
            public double Demand;
            public double SilhouetteScore;

        }
        public class ClusterDatapoint
        {
            public string ClusterCode { get; set; }

            public double Longitude { get; set; }

            public double Latitude { get; set; }

            public int NumberOfPoints { get; set; }

            public double DaviesBouldinIndex { get; set; }

        }

        public class CL_InputData
        {
            public Dictionary<string, (double Latitude, double Longitude)> DataPoints { get; set; }
            public Dictionary<int, (double Latitude, double Longitude)> DataPoints_Int { get; set; }
            public DBSCAN_InputData DBSCAN_InputData { get; set; }
            public Kmeans_InputData Kmeans_InputData { get; set; }

        }
        public class CL_OutputData 
        {

            public ObservableCollection<ClusterDatapoint> Clusters { get; set; }

            public ObservableCollection<MainDatapoint> DataPoints { get; set; }


            public DBSCAN_OutputData DBSCANoutputdata { get; set; }
            public KmeansOutputData KmeansNoutputdata { get; set; }


        }

        public class Kmeans_InputData
        {
            public int NumberOfClusters { get; set; }
            public int MaxIterations { get; set; }


        }
        public class DBSCAN_InputData
        {
            public double Epsilon { get; set; }
            public int MinPoints { get; set; }


        }
        public class DBSCAN_OutputData
        {
            public class ClusterData
            {
                public int ClusterId { get; set; }
                public string CentroidCode { get; set; }
                public double CentroidLatitude { get; set; }
                public double CentroidLongitude { get; set; }
                public double DaviesBouldinIndex { get; set; }


                public ObservableCollection<MainDatapoint> DataPoints { get; set; }

            }
            public List<ClusterData> Clusters { get; set; }
            public double SilhouetteScore_Avg { get; set; }
            public double DaviesBouldinIndex_Avg { get; set; }
            public double NoiseRatio { get; set; }
            // New variables to store data needed for plotting


            public DBSCAN_OutputData()
            {
                Clusters = new List<ClusterData>();

            }

        }

        public class KmeansOutputData
        {
            public class ClusterData
            {
                public int ClusterId { get; set; }
                public string CentroidCode { get; set; }
                public double CentroidLatitude { get; set; }
                public double CentroidLongitude { get; set; }
                public ObservableCollection<MainDatapoint> DataPoints { get; set; }

            }

            public List<ClusterData> Clusters { get; set; }
            public double SilhouetteScore_Avg { get; set; }
            public double Inertia { get; set; }
            public double DaviesBouldinIndex_Avg { get; set; } // Add this field



            public KmeansOutputData()
            {
                Clusters = new List<ClusterData>();
            }
        }


        #endregion

        #region DBSCAN

        // Αλγόριθμος ομαδοποίησης DBSCAN
        public DBSCAN_OutputData CalculateDBSCANClustering(CL_InputData inputData, DBSCAN_InputData dbscanInput)
        {
            // Μετατροπή των σημείων δεδομένων σε 2D πίνακα
            double[][] data = inputData.DataPoints_Int.Values
                .Select(v => new double[] { v.Latitude, v.Longitude })
                .ToArray();

            int[] labels = new int[data.Length];

            // Αρχικοποίηση του πίνακα ετικετών σε -1 (δηλώνοντας ότι όλα τα σημεία είναι αρχικά μη ταξινομημένα)
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i] = -1;
            }

            int clusterId = 0;

            // Για κάθε σημείο δεδομένων
            for (int i = 0; i < data.Length; i++)
            {
                if (labels[i] != -1) continue; // Εάν το σημείο έχει ήδη ταξινομηθεί, προχωράμε στο επόμενο

                var neighbors = GetNeighbors(i, data, dbscanInput.Epsilon); // Βρίσκουμε τους γείτονες του σημείου

                if (neighbors.Count < dbscanInput.MinPoints)
                {
                    labels[i] = -1; // Αν οι γείτονες είναι λιγότεροι από το ελάχιστο, το σημείο θεωρείται θόρυβος
                }
                else
                {
                    // Επέκταση της ομάδας αν πληρούνται οι συνθήκες
                    ExpandCluster(data, labels, i, neighbors, clusterId, dbscanInput.Epsilon, dbscanInput.MinPoints);
                    clusterId++;
                }
            }

            // Εξαγωγή των ομάδων και των σημείων θορύβου
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

            // Δημιουργία της δομής εξόδου
            var output = new DBSCAN_OutputData();

            int clusterIndex = 1;
            foreach (var cluster in clusters)
            {
                if (cluster.Count < dbscanInput.MinPoints)
                {
                    noisePoints.AddRange(cluster.Select(p => (p.Code, p.Latitude, p.Longitude)));
                    continue;
                }

                // Υπολογισμός του κεντροειδούς
                double centroidLatitude = cluster.Average(p => p.Latitude);
                double centroidLongitude = cluster.Average(p => p.Longitude);

                // Δημιουργία των δεδομένων της ομάδας
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

            // Προσθήκη της ομάδας θορύβου
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
                        Latitude = Math.Round(p.Latitude, 4),
                        Longitude = Math.Round(p.Longitude, 4),
                        ClusterCode = "Noise"
                    }).ToList())
            };

            output.Clusters.Add(noiseClusterData);

            // Υπολογισμός των μετρικών ποιότητας της ομαδοποίησης
            var silhouetteAvg = CalculateSilhouetteScore(output);
            var dbIndexAvg = CalculateDaviesBouldinIndex(output);
            var noiseRatio = CalculateNoiseRatio(noisePoints.Count, data.Length);

            output.SilhouetteScore_Avg = Math.Round(silhouetteAvg, 3);
            output.DaviesBouldinIndex_Avg = Math.Round(dbIndexAvg, 3);
            output.NoiseRatio = Math.Round(noiseRatio, 3);

            return output;
        }

        // Βοηθητική συνάρτηση για την εύρεση γειτόνων εντός της απόστασης epsilon
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

        // Επέκταση της ομάδας
        private static void ExpandCluster(double[][] data, int[] labels, int pointIndex, List<int> neighbors, int clusterId, double epsilon, int minPoints)
        {
            labels[pointIndex] = clusterId;

            Queue<int> pointsToProcess = new Queue<int>(neighbors);

            while (pointsToProcess.Count > 0)
            {
                int currentPointIndex = pointsToProcess.Dequeue();

                if (labels[currentPointIndex] == -1)
                {
                    labels[currentPointIndex] = clusterId; // Μετατροπή του θορύβου σε σημείο ορίου
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
        #region Υπολογισμός Silhouette, Davies-Bouldin Index, Noise Ratio

        // Συνάρτηση για τον υπολογισμό του δείκτη silhouette
        private double CalculateSilhouetteScore(DBSCAN_OutputData output)
        {
            var silhouetteScores = new List<double>();

            foreach (var cluster in output.Clusters)
            {
                if (cluster.ClusterId == -1) continue; // Παράλειψη του θορύβου

                foreach (var point in cluster.DataPoints)
                {
                    double a = AverageDistanceToCluster(point, cluster.DataPoints);
                    double b = double.MaxValue;

                    foreach (var otherCluster in output.Clusters)
                    {
                        if (otherCluster.ClusterId == cluster.ClusterId || otherCluster.ClusterId == -1) continue;

                        double avgDist = AverageDistanceToCluster(point, otherCluster.DataPoints);
                        if (avgDist < b)
                        {
                            b = avgDist;
                        }
                    }

                    silhouetteScores.Add((b - a) / Math.Max(a, b));
                }
            }

            return silhouetteScores.Average();
        }

        // Συνάρτηση για τον υπολογισμό του δείκτη Davies-Bouldin
        private double CalculateDaviesBouldinIndex(DBSCAN_OutputData output)
        {
            var dbIndexList = new List<double>();

            foreach (var cluster in output.Clusters)
            {
                if (cluster.ClusterId == -1) continue; // Παράλειψη του θορύβου

                double maxRatio = double.MinValue;

                foreach (var otherCluster in output.Clusters)
                {
                    if (otherCluster.ClusterId == cluster.ClusterId || otherCluster.ClusterId == -1) continue;

                    double si = AverageClusterScatter(cluster.DataPoints);
                    double sj = AverageClusterScatter(otherCluster.DataPoints);
                    double dij = CalculateDistance(
                        (cluster.CentroidLatitude, cluster.CentroidLongitude),
                        (otherCluster.CentroidLatitude, otherCluster.CentroidLongitude));

                    double ratio = (si + sj) / dij;
                    if (ratio > maxRatio)
                    {
                        maxRatio = ratio;
                    }
                }

                dbIndexList.Add(maxRatio);
            }

            return dbIndexList.Average();
        }

        // Υπολογισμός της διάχυσης εντός ομάδας
        private double AverageClusterScatter(ObservableCollection<MainDatapoint> clusterPoints)
        {
            var centroid = CalculateCentroid(clusterPoints);
            double scatterSum = 0.0;

            foreach (var point in clusterPoints)
            {
                scatterSum += CalculateDistance((point.Latitude, point.Longitude), centroid);
            }

            return clusterPoints.Count > 0 ? scatterSum / clusterPoints.Count : 0;
        }

        private double CalculateNoiseRatio(int noisePointsCount, int totalPointsCount)
        {
            return (double)noisePointsCount / totalPointsCount;
        }

        // Υπολογισμός της μέσης απόστασης προς την ομάδα
        private double AverageDistanceToCluster(MainDatapoint point, ObservableCollection<MainDatapoint> clusterPoints)
        {
            double sum = 0.0;

            foreach (var otherPoint in clusterPoints)
            {
                sum += CalculateDistance((point.Latitude, point.Longitude), (otherPoint.Latitude, otherPoint.Longitude));
            }

            return clusterPoints.Count > 0 ? sum / clusterPoints.Count : 0;
        }

        // Υπολογισμός του κεντροειδούς μιας ομάδας
        private (double Latitude, double Longitude) CalculateCentroid(ObservableCollection<MainDatapoint> clusterPoints)
        {
            double latitudeSum = 0.0;
            double longitudeSum = 0.0;

            foreach (var point in clusterPoints)
            {
                latitudeSum += point.Latitude;
                longitudeSum += point.Longitude;
            }

            return (latitudeSum / clusterPoints.Count, longitudeSum / clusterPoints.Count);
        }

        #endregion
        #endregion

        #region Calculate Distance
        // Συνάρτηση για τον υπολογισμό της απόστασης
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

        // Συνάρτηση για τη μετατροπή μοιρών σε ακτίνια
        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
        #endregion

        #endregion

        #region TSP

        #region Classes
        public class TSP_InputData
        {
            public Dictionary<string, int> City_IndexMap { get; set; }
            public Dictionary<int, (double, double)> Coords { get; set; }

            public AntColony_TSP_InputData AntColony_InputData { get; set; }
            public SAnnealing_TSP_InputData SAnnealing_InputData { get; set; }

        }

        public class TSP_OutputData
        {
            public AntColony_TSP_OutputData AntColony_Outputdata { get; set; }
            public SAnnealing_TSP_OutputData SAnnealing_Outputdata { get; set; }

        }

        public class SAnnealing_TSP_InputData 
        {

            public double InitialTemp { get; set; }
            public double CoolingRate { get; set; }
            public double StoppingTemp { get; set; }
            public int MaxIterations { get; set; }

        }
        public class SAnnealing_TSP_OutputData
        {
            public List<int> BestTour { get; set; }
            public double BestTourLength { get; set; }

            public SAnnealing_TSP_OutputData()
            {
                BestTour = new List<int>();
            }


        }
        public class AntColony_TSP_InputData
        {

            public double Alpha { get; set; }

            // ACO parameter: Importance of heuristic information (inverse of distance)
            public double Beta { get; set; }


            // ACO parameter: Pheromone evaporation rate
            public double EvaporationRate { get; set; }


            // ACO parameter: Initial pheromone level on each path
            public double InitialPheromoneLevel { get; set; }

            // Number of ants in the colony
            public int NumberOfAnts { get; set; }

            // Number of iterations to run the ACO algorithm
            public int NumberOfIterations { get; set; }


        }
        public class AntColony_TSP_OutputData 
        {
            public List<int> BestTour { get; set; }
            public double BestTourLength { get; set; }
            public double[,] PheromoneLevels { get; set; }

            public AntColony_TSP_OutputData()
            {
                BestTour = new List<int>();
            }


        }
        #endregion

        #region Ant Colony Optimization

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
                // Initialize pheromone levels
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

                // Main loop for iterations
                for (int iteration = 0; iteration < NumberOfIterations; iteration++)
                {
                    List<int>[] tours = new List<int>[NumberOfAnts];
                    double[] tourLengths = new double[NumberOfAnts];

                    // Construct solutions for each ant
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

                    // Update pheromones
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

        // Construct a solution for one ant
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

                // Calculate probabilities for next city
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

                // Select next city based on probabilities
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

        // Update pheromone levels
        private void UpdatePheromones(double[,] pheromones, List<int>[] tours, double[] tourLengths, double evaporationRate)
        {
            int n = pheromones.GetLength(0);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    pheromones[i, j] *= (1 - evaporationRate); // Evaporate pheromones
                }
            }

            // Add new pheromones based on tours
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

        // Calculate the total length of a tour
        public static double CalculateTourLength(List<int> tour, double[,] distanceMatrix)
        {
            double length = 0.0;
            for (int i = 0; i < tour.Count - 1; i++)
            {
                length += distanceMatrix[tour[i], tour[i + 1]];
            }
            length += distanceMatrix[tour[tour.Count - 1], tour[0]];
            return length;
        }

        #endregion

        #region SAnnealing
        public SAnnealing_TSP_OutputData Calculate_SAnnealing_TSP(TSP_InputData InputData)
        {
            var OutputData = new SAnnealing_TSP_OutputData();

            try
            {
                #region CityData

                var City_IndexMap = InputData.City_IndexMap;
                var Coords = InputData.Coords;

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

                #region SAnnealing Settings
                double T0 = InputData.SAnnealing_InputData.InitialTemp;      // Initial Temperature
                double alpha = InputData.SAnnealing_InputData.CoolingRate;   // Cooling Rate
                double Ts = InputData.SAnnealing_InputData.StoppingTemp;     // Stopping Temperature
                double Nmax = InputData.SAnnealing_InputData.MaxIterations;  // Maximum Iterations
                #endregion

                // Initialize variables for the algorithm
                double currentTemp = T0;
                int iteration = 0;

                // Initial solution and its cost
                List<int> currentSolution = GenerateInitialSolution(n);
                double currentCost = CalculateTourLength(currentSolution, distanceMatrix);

                // Best solution found
                List<int> bestSolution = new List<int>(currentSolution);
                double bestCost = currentCost;

                // Main loop of the Simulated Annealing algorithm
                while (currentTemp > Ts && iteration < Nmax)
                {
                    // Generate a neighboring solution
                    List<int> neighborSolution = GenerateNeighbor(currentSolution);
                    double neighborCost = CalculateTourLength(neighborSolution, distanceMatrix);

                    // Determine if we should accept the neighboring solution
                    if (AcceptSolution(currentCost, neighborCost, currentTemp))
                    {
                        currentSolution = neighborSolution;
                        currentCost = neighborCost;

                        // Update the best solution found
                        if (currentCost < bestCost)
                        {
                            bestSolution = new List<int>(currentSolution);
                            bestCost = currentCost;
                        }
                    }

                    // Cool down the temperature
                    currentTemp *= alpha;
                    iteration++;
                }

                // Set the output data
                OutputData.BestTour = bestSolution;
                OutputData.BestTourLength = bestCost;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return OutputData;
        }

        private static List<int> GenerateInitialSolution(int cityCount)
        {
            List<int> solution = Enumerable.Range(0, cityCount).ToList();
            Random rng = new Random();
            int n = solution.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = solution[k];
                solution[k] = solution[n];
                solution[n] = value;
            }
            return solution;
        }

        // Helper method to generate a neighboring solution by swapping two cities
        private static List<int> GenerateNeighbor(List<int> currentSolution)
        {
            List<int> neighbor = new List<int>(currentSolution);
            Random rng = new Random();
            int index1 = rng.Next(neighbor.Count);
            int index2 = (index1 + 1 + rng.Next(neighbor.Count - 1)) % neighbor.Count;
            int temp = neighbor[index1];
            neighbor[index1] = neighbor[index2];
            neighbor[index2] = temp;
            return neighbor;
        }

        // Helper method to determine if a new solution should be accepted
        private static bool AcceptSolution(double currentCost, double neighborCost, double temperature)
        {
            if (neighborCost < currentCost)
            {
                return true;
            }
            else
            {
                double acceptanceProbability = Math.Exp((currentCost - neighborCost) / temperature);
                Random rng = new Random();
                return rng.NextDouble() < acceptanceProbability;
            }
        }


        #endregion


        #endregion

        #region CSV Functions

        private Dictionary<TKey, TValue> ReadFromCsv<TKey, TValue>(string fileName)
        {
            var dictionary = new Dictionary<TKey, TValue>();
            using (var reader = new StreamReader(fileName))
            {
                var header = reader.ReadLine(); // Skip header
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    if (typeof(TKey) == typeof(string) && typeof(TValue) == typeof(List<string>))
                    {
                        var key = (TKey)(object)values[0];
                        var value = (TValue)(object)values[1].Split(';').ToList();
                        dictionary[key] = value;
                    }
                    else if (typeof(TKey) == typeof((string, string)) && typeof(TValue) == typeof(double))
                    {
                        var key = (TKey)(object)(values[0], values[1]);
                        var value = (TValue)(object)double.Parse(values[2]);
                        dictionary[key] = value;
                    }
                    else if (typeof(TKey) == typeof((string, string, string)) && typeof(TValue) == typeof(double))
                    {
                        var key = (TKey)(object)(values[0], values[1], values[2]);
                        var value = (TValue)(object)double.Parse(values[3]);
                        dictionary[key] = value;
                    }
                    else if (typeof(TKey) == typeof(string) && typeof(TValue) == typeof(double))
                    {
                        var key = (TKey)(object)values[0];
                        var value = (TValue)(object)double.Parse(values[1]);
                        dictionary[key] = value;
                    }
                    else if (typeof(TKey) == typeof(string) && typeof(TValue) == typeof(int))
                    {
                        var key = (TKey)(object)values[0];
                        var value = (TValue)(object)double.Parse(values[1]);
                        dictionary[key] = value;
                    }
                    else if (typeof(TKey) == typeof(string) && typeof(TValue) == typeof(string))
                    {
                        var key = (TKey)(object)values[0];
                        var value = (TValue)(object)values[1];
                        dictionary[key] = value;
                    }
                    else if (typeof(TKey) == typeof(string) && typeof(TValue) == typeof((double, double)))
                    {
                        var key = (TKey)(object)values[0];
                        var value = (TValue)(object)(double.Parse(values[1]), double.Parse(values[2]));
                        dictionary[key] = value;
                    }
                    else if (typeof(TKey) == typeof(int) && typeof(TValue) == typeof((double, double)))
                    {
                        var key = (TKey)(object)values[0];
                        var value = (TValue)(object)(double.Parse(values[1]), double.Parse(values[2]));
                        dictionary[key] = value;
                    }
                }
            }
            return dictionary;
        }

        #endregion
        public Nikos_Polychronopoulos_ML_Functions()
        {
            #region Αρχικοποίηση Κλασεων/ Εισαγωγή διαφόρων Settings

            #region Clustering Classes

            var CLInputData = new CL_InputData();

            CLInputData.DBSCAN_InputData = new DBSCAN_InputData();
            CLInputData.DBSCAN_InputData.Epsilon = 50;
            CLInputData.DBSCAN_InputData.MinPoints = 3;

            var ClResultsdata = new CL_OutputData();
            ClResultsdata.Clusters = new ObservableCollection<ClusterDatapoint>();
            ClResultsdata.DataPoints = new ObservableCollection<MainDatapoint>();

            #endregion

            #region TSP Classes
            var TSPInputData = new TSP_InputData();
            TSPInputData.SAnnealing_InputData = new SAnnealing_TSP_InputData();
            TSPInputData.SAnnealing_InputData.InitialTemp = 5000;
            TSPInputData.SAnnealing_InputData.CoolingRate = 0.9;
            TSPInputData.SAnnealing_InputData.StoppingTemp = 50;
            TSPInputData.SAnnealing_InputData.MaxIterations = 5000;

            TSPInputData.AntColony_InputData = new AntColony_TSP_InputData();
            TSPInputData.AntColony_InputData.Alpha = 1;
            TSPInputData.AntColony_InputData.Beta = 5;
            TSPInputData.AntColony_InputData.EvaporationRate = 0.1;
            TSPInputData.AntColony_InputData.InitialPheromoneLevel = 1;
            TSPInputData.AntColony_InputData.NumberOfAnts = 10;
            TSPInputData.AntColony_InputData.NumberOfIterations = 1000;

            var TSP_ResultsData = new TSP_OutputData();
            TSP_ResultsData.AntColony_Outputdata = new AntColony_TSP_OutputData();
            #endregion

            #endregion

            #region Αρχικοποίηση Dictionaries

            string desktopPath = "C:\\Users\\npoly\\Source\\Repos\\Optimization\\";
            string directoryPath = Path.Combine(desktopPath, "ML_CSVFiles");

            CLInputData.DataPoints_Int = new Dictionary<int, (double , double )>();
            CLInputData.DataPoints = new Dictionary<string, (double , double )>();
            TSPInputData.City_IndexMap = new Dictionary<string, int>();
            TSPInputData.Coords = new Dictionary<int, (double, double)>();

            CLInputData.DataPoints = ReadFromCsv<string, (double, double)>(Path.Combine(directoryPath, $"DataPoints.csv"));
            CLInputData.DataPoints_Int = ReadFromCsv<int, (double, double)>(Path.Combine(directoryPath, $"DataPoints_Int.csv"));
            TSPInputData.City_IndexMap = ReadFromCsv<string,int>(Path.Combine(directoryPath, $"City_IndexMap.csv"));
            TSPInputData.Coords = ReadFromCsv<int, (double, double)>(Path.Combine(directoryPath, $"Coords.csv"));

            #endregion

            #region Επιλογή Αλγορίθμου
            var Choices = new List<string>();
            Choices.Add("DBSCAN");
            Choices.Add("S_Annealing");
            Choices.Add("Ant_Colony");

            var SelectedChoise = "DBSCAN";
            if (SelectedChoise == "DBSCAN")
            {


                ClResultsdata.DBSCANoutputdata = CalculateDBSCANClustering(CLInputData, CLInputData.DBSCAN_InputData);
            }
            else if (SelectedChoise == "S_Annealing")
            {


                TSP_ResultsData.SAnnealing_Outputdata = Calculate_SAnnealing_TSP(TSPInputData);
            }
            else if (SelectedChoise == "Ant_Colony")
            {
                TSP_ResultsData.AntColony_Outputdata = Calculate_AntColony_TSP(TSPInputData);

            }


            #endregion

        }
    }
}
