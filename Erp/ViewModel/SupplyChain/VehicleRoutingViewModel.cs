using Gurobi;
using LiveCharts.Defaults;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.DataVisualization.Charting;
using System.Windows.Forms.VisualStyles;
using System.ComponentModel;

namespace Erp.ViewModel.SupplyChain
{
    public class VehicleRoutingViewModel : ViewModelBase
    {
        private int n; // number of customers
        private int Q;  // vehicle capacity
        private List<int> N; // list of customers
        private List<int> V; // list of customers and depot
        private Dictionary<int, int> q; // demand for each customer
        private double[] loc_x;
        private double[] loc_y;
        public ChartValues<ObservablePoint> Customers { get; private set; }
        public ChartValues<ObservablePoint> Routes { get; private set; }

        public VehicleRoutingViewModel()
        {
            InitializeData();
            SolveVRP();
        }

        private void InitializeData()
        {
            Random rnd = new Random();
            n = 10;
            Q = 20;
            N = Enumerable.Range(1, n).ToList();
            V = new List<int> { 0 }.Concat(N).ToList();
            q = N.ToDictionary(i => i, i => rnd.Next(1, 10));
            loc_x = V.Select(_ => rnd.NextDouble() * 200).ToArray();
            loc_y = V.Select(_ => rnd.NextDouble() * 100).ToArray();
        }
        private void SolveVRP()
        {
            //try
            //{
                //    // Create new environment and model
                //    GRBEnv env = new GRBEnv(true);
                //    env.Set("LogFile", "mip1.log");
                //    env.Start();

                //    GRBModel model = new GRBModel(env);

                //    // Create decision variables and update model to integrate new variables
                //    GRBVar[,] x = new GRBVar[V.Count, V.Count];
                //    GRBVar[] u = new GRBVar[V.Count];

                //    for (int i = 0; i < V.Count; ++i)
                //    {
                //        u[i] = model.AddVar(0.0, double.MaxValue, 0.0, GRB.CONTINUOUS, "u" + i);
                //        for (int j = 0; j < V.Count; ++j)
                //        {
                //            if (i != j)
                //            {
                //                x[i, j] = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, "x" + i + "_" + j);
                //            }
                //        }
                //    }

                //    model.Update();

                //    // Set objective: minimize the distance from depot
                //    GRBLinExpr objective = new GRBLinExpr();
                //    for (int i = 0; i < V.Count; ++i)
                //    {
                //        for (int j = 0; j < V.Count; ++j)
                //        {
                //            if (i != j)
                //            {
                //                double cost = Math.Sqrt(Math.Pow(loc_x[i] - loc_x[j], 2) + Math.Pow(loc_y[i] - loc_y[j], 2));
                //                objective.AddTerm(cost, x[i, j]);
                //            }
                //        }
                //    }

                //    model.SetObjective(objective, GRB.MINIMIZE);

                //    // Add constraints
                //    for (int i = 1; i < V.Count; ++i)
                //    {
                //        // Ensure that each customer is visited exactly once
                //        GRBLinExpr outflowConstr = new GRBLinExpr();
                //        GRBLinExpr inflowConstr = new GRBLinExpr();

                //        for (int j = 0; j < V.Count; ++j)
                //        {
                //            if (i != j)
                //            {
                //                outflowConstr.AddTerm(1.0, x[i, j]);
                //                inflowConstr.AddTerm(1.0, x[j, i]);
                //            }
                //        }

                //        model.AddConstr(outflowConstr == 1, "visitOut" + i);
                //        model.AddConstr(inflowConstr == 1, "visitIn" + i);


                //        // Capacity constraint
                //        for (int j = 1; j < V.Count; ++j)
                //        {
                //            if (i != j)
                //            {
                //                model.AddConstr(u[i] - u[j] + Q * x[i, j] <= Q - q[j], "capacity" + i + "_" + j);
                //            }
                //        }
                //    }

                //    // Optimize the model
                //    model.Optimize();

                //    // Extract edges from the optimized model
                //    List<Tuple<int, int>> edges = new List<Tuple<int, int>>();
                //    for (int i = 0; i < V.Count; ++i)
                //    {
                //        for (int j = 0; j < V.Count; ++j)
                //        {
                //            if (i != j && x[i, j].X > 0.5)
                //            {
                //                edges.Add(new Tuple<int, int>(i, j));
                //            }
                //        }
                //    }

                //    // Process results
                //    Customers = new ChartValues<ObservablePoint>(V.Select(i => new ObservablePoint(loc_x[i], loc_y[i])));
                //    Routes = new ChartValues<ObservablePoint>(edges.Select(e => new ObservablePoint(loc_x[e.Item1], loc_y[e.Item2])));

                //    RaisePropertyChanged(nameof(Customers));
                //    RaisePropertyChanged(nameof(Routes));
                //    // TODO: Process results and update the view model properties accordingly...
                //}
                //catch (GRBException ex)
                //{
                //    Console.WriteLine("Error code: " + ex.ErrorCode + ". " + ex.Message);
                //}
            }
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }
}