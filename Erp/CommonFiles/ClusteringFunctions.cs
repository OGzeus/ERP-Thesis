using Erp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning;
using Erp.Model.SupplyChain.Clusters;
using Microsoft.ML.Data;
using System.Collections.ObjectModel;

namespace Erp.CommonFiles
{
    public class ClusteringFunctions : RepositoryBase
    {
        #region Kmeans Clustering 
        public class ClusteredDataPoint
        {
            public float Latitude { get; set; }
            public float Longitude { get; set; }

            [ColumnName("PredictedLabel")]
            public uint Cluster { get; set; }
        }
        public KmeansOutputData Calculate_Kmeans_Clustering(Dictionary<string, int> PointCode_Index, KmeansInputData input, Dictionary<string, (double, double)> DataPoints)
        {
            var outputData = new KmeansOutputData();

            try
            {
                var dataPointsList = DataPoints.Values.Select(point => new double[] { point.Item1, point.Item2 }).ToArray();

                var kmeans = new KMeans(input.NumberOfClusters)
                {
                    MaxIterations = input.MaxIterations
                };

                var clusters = kmeans.Learn(dataPointsList);
                var labels = clusters.Decide(dataPointsList);

                var reverseLookup = DataPoints.ToDictionary(dp => (float)dp.Value.Item1, dp => dp.Key);

                double totalInertia = 0;
                var silhouetteScores = new List<double>();

                // Initialize clusters
                var clustersDict = new Dictionary<int, KmeansOutputData.ClusterData>();

                foreach (var pointIndex in Enumerable.Range(0, dataPointsList.Length))
                {
                    var clusterId = labels[pointIndex];
                    if (!clustersDict.ContainsKey(clusterId))
                    {
                        clustersDict[clusterId] = new KmeansOutputData.ClusterData
                        {
                            ClusterId = clusterId,
                            CentroidCode = clusterId.ToString(),
                            DataPoints = new ObservableCollection<MainDatapoint>()
                        };
                    }

                    var point = dataPointsList[pointIndex];
                    var newPoint = new MainDatapoint
                    {
                        ClusterCode = clusterId.ToString(),
                        Code = reverseLookup[(float)point[0]],
                        Latitude = Math.Round(point[0], 4),
                        Longitude = Math.Round(point[1], 4)
                    };
                    clustersDict[clusterId].DataPoints.Add(newPoint);
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
                foreach (var pointIndex in Enumerable.Range(0, dataPointsList.Length))
                {
                    var clusterId = labels[pointIndex];
                    var cluster = clustersDict[clusterId];
                    var point = dataPointsList[pointIndex];

                    // Calculate the distance to the centroid for inertia
                    var distanceToCentroid = Math.Sqrt(Math.Pow(point[0] - cluster.CentroidLatitude, 2) + Math.Pow(point[1] - cluster.CentroidLongitude, 2));
                    totalInertia += distanceToCentroid;

                    // Compute silhouette score
                    silhouetteScores.Add(ComputeSilhouetteScore(new ClusteredDataPoint
                    {
                        Latitude = (float)point[0],
                        Longitude = (float)point[1],
                        Cluster = (uint)clusterId
                    }, dataPointsList, labels));

                    // Update withinClusterSumOfSquares
                }

                // Calculate betweenClusterSumOfSquares
                var overallCentroid = new double[]
                {
            dataPointsList.Average(dp => dp[0]),
            dataPointsList.Average(dp => dp[1])
                };

                double betweenClusterSumOfSquares = 0;
                foreach (var cluster in clustersDict.Values)
                {
                    var centroidDistance = Math.Sqrt(Math.Pow(cluster.CentroidLatitude - overallCentroid[0], 2) +
                                                     Math.Pow(cluster.CentroidLongitude - overallCentroid[1], 2));
                    betweenClusterSumOfSquares += cluster.DataPoints.Count * Math.Pow(centroidDistance, 2);
                }

                outputData.Inertia = totalInertia;
                outputData.SilhouetteScore_Avg = silhouetteScores.Count > 0 ? silhouetteScores.Average() : 0;

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
        private double ComputeSilhouetteScore(ClusteredDataPoint point, double[][] allPoints, int[] labels)
        {
            var sameClusterPoints = allPoints.Where((p, index) => labels[index] == point.Cluster).ToArray();
            var otherClusterPoints = allPoints.Where((p, index) => labels[index] != point.Cluster).ToArray();

            double a = sameClusterPoints.Average(p => Distance(point, p));
            double b = otherClusterPoints.ToList()
                                         .GroupBy(p => labels[Array.IndexOf(allPoints, p)])
                                         .Min(g => g.Average(p => Distance(point, p)));

            return (b - a) / Math.Max(a, b);
        }


        private double Distance(ClusteredDataPoint p1, double[] p2)
        {
            return Math.Sqrt(Math.Pow(p1.Latitude - p2[0], 2) + Math.Pow(p1.Longitude - p2[1], 2));
        }


        #endregion
    }
}
