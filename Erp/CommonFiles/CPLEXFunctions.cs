using Erp.Model.Enums;
using Erp.Model.Thesis.VacationPlanning;
using Erp.Model.Thesis;
using Erp.View.Thesis.CustomButtons;
using Gurobi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erp.DataBase.Τhesis;
using Erp.DataBase;
using Microsoft.Extensions.Options;
using Erp.Repositories;
using ILOG.Concert;
using ILOG.CPLEX;
using Microsoft.EntityFrameworkCore.Metadata;
using Syncfusion.Data.Extensions;
using System.IO;
using Erp.Model.Thesis.CrewScheduling;

namespace Erp.CommonFiles
{
    public class CPLEXFunctions : RepositoryBase
    {
        #region Vacation Planning
        public VacationPlanningOutputData CalculateVacationPlanning_CPLEX(VacationPlanningInputData InputData)
        {
            // Create the CPLEX environment and model
            Cplex model = new Cplex();
            Cplex FeasableModel = new Cplex();

            string relativePath = Path.Combine("OptimizationResults", "CPLEX", "Thesis", "Vacation_Planning");
            Directory.CreateDirectory(relativePath);

            //// Set the log file for the environment
            //model.SetOut(new System.IO.StreamWriter("vplogfile_CPLEX.log"));

            //// The model is the Cplex object itself, so no need to create a separate model object
            //// Set the final log file
            //model.SetOut(new System.IO.StreamWriter("vplogfilefinal_CPLEX.log"));

            // Set the final log file

            VacationPlanningOutputData Data = new VacationPlanningOutputData();
            Data.VPYijResultsDataGrid = new ObservableCollection<VPYijResultsData>();
            Data.VPYijzResultsDataGrid = new ObservableCollection<VPYijResultsData>();
            Data.VPXijResultsDataGrid = new ObservableCollection<VPXijResultsData>();
            Data.VPXiResultsDataGrid = new ObservableCollection<VPXiResultData>();
            Data.EmpLeaveStatusData = new ObservableCollection<EmployeeData>();

            List<string> rows = new List<string>();
            List<string> columns = new List<string>();
            Dictionary<(string, string), double> make_plan = new Dictionary<(string, string), double>();
            double bigM = 10000;

            try
            {
                #region Optimization

                #region Optimization paramaters

                int MaxSatisfiedBids = InputData.MaxSatisfiedBids; //Max αριθμος ικανοποιημένων Bids ανα υπάλληλο
                int SeparValue = InputData.SeparValue; // Seperation Value

                string[] Employees = InputData.Employees.Select(d => d.Code).ToArray(); //Πινακας με τους Κωδικους Υπαλληλων
                string[] Dates = InputData.DatesStr; //Πινακας με τα Dates




                Dictionary<string, int> MaxLeaveBidsPerEmployee = InputData.MaxLeaveBidsPerEmployee;
                Dictionary<int, int> MaxLeaveBidsPerEmployee_Int = InputData.MaxLeaveBidsPerEmployee_Int;


                // Zvalue = Number Of Specific    .Για Specific Bids το Zvalue = 1 Παντα
                // Rvalue = Number of NonSpecific . Για Specific,NonSpecific Bids to Rvalue = 1 Παντα

                //ZBidsDict = <(Employee Code, LeaveBidCode ,Rvalue), Zvalue>
                Dictionary<(string, string, int), int> ZbidsDict_Str = InputData.ZBidsDict;

                //RBidsDict = <(Employee Code, LeaveBidCode ),Rvalue>
                Dictionary<(string, string), int> RBidsDict_Str = InputData.RBidsDict;

                //ZBidsDict = <(Employee Code, LeaveBidCode ,Rvalue), Zvalue>
                Dictionary<(int, int, int), int> ZbidsDict = InputData.ZBidsDict_Int;

                //RBidsDict = <(Employee Code, LeaveBidCode ),Rvalue>
                Dictionary<(int, int), int> RBidsDict = InputData.RBidsDict_Int;


                int MaxLeaveBids = InputData.MaxLeaveBids; //Μεγιστος αριθμός Bids υπαλλήλου απο ολούς τους υπαλλήλους
                int LimitLineFixed = InputData.Schedule.LimitLineFixed; // Σταθερό Limit Line σε όλες τις ημέρες
                int Zmax = ZbidsDict_Str.Max(kvp => kvp.Value); //Μεγιστο Zvalue , το χρειαζόμαστε για την δήλωση της Yijrz
                int MaxNonSpecific = RBidsDict_Str.Max(kvp => kvp.Value); //Μεγιστο Rvalue , το χρειαζόμαστε για την δήλωση της Yijrz



                #region from string to int 


                // Mapping from string keys to integer indices
                Dictionary<string, int> EmployeeIndexMap = new Dictionary<string, int>();
                Dictionary<string, int> LeaveBidCodeIndexMap = new Dictionary<string, int>();
                // Initialize integer index counters
                int EmpIndexCounter = 0;
                int LeaveBidsIndexCounter = 0;

                // Fill IndexMaps
                foreach (var emp in Employees)
                {
                    EmployeeIndexMap[emp] = EmpIndexCounter++;
                }


                #endregion

                #endregion

                #region Optimization Algorithm

                #region Decision Variables 
                INumVar[,,,] Y = new INumVar[Employees.Length, MaxLeaveBids, MaxNonSpecific, Zmax];
                INumVar[,] X = new INumVar[Employees.Length, Dates.Length];
                INumVar[,,] R = new INumVar[Employees.Length, MaxLeaveBids, MaxNonSpecific];

                // Create decision variables X
                for (int i = 0; i < Employees.Length; i++)
                {
                    for (int t = 0; t < Dates.Length; t++)
                    {
                        // Define the variable name
                        string varNameX = $"X{i + 1}_{t + 1}";

                        // Create the binary variable with a name
                        X[i, t] = model.NumVar(0,1, NumVarType.Bool,varNameX);
                    }
                }

                // Create decision variables Y
                for (int i = 0; i < Employees.Length; i++)
                {
                    for (int j = 0; j < MaxLeaveBidsPerEmployee[Employees[i]]; j++)
                    {
                        var Rvalue = RBidsDict[(i, j)];
                        for (int r = 0; r < Rvalue; r++) //allagh
                        {
                            var Zvalue = ZbidsDict[(i, j, r + 1)];

                            for (int z = 0; z < Zvalue; z++) //allagh
                            {
                                // Define the variable name
                                string varNameY = $"Y{i + 1}_{j + 1}_{r + 1}_{z + 1}";

                                // Create the binary variable with a name
                                Y[i, j, r, z] = model.NumVar(0, 1, NumVarType.Bool, varNameY);
                            }
                        }
                    }
                }

                // Create decision variables R
                for (int i = 0; i < Employees.Length; i++)
                {
                    for (int j = 0; j < MaxLeaveBidsPerEmployee[Employees[i]]; j++)
                    {
                        var Rvalue = RBidsDict[(i, j)];
                        for (int r = 0; r < Rvalue; r++) //allagh
                        {
                            // Define the variable name
                            string varNameR = $"R{i + 1}_{j + 1}_{r + 1}";

                            // Create the binary variable with a name
                            R[i, j, r] = model.NumVar(0, 0, NumVarType.Bool, varNameR);
                        }
                    }
                }

                var a = 1;

                #endregion

                #region Objective Function

                ILinearNumExpr objective = model.LinearNumExpr();

                for (int i = Employees.Length - 1; i >= 0; i--)
                {
                    for (int j = MaxLeaveBidsPerEmployee[Employees[i]] - 1; j >= 0; j--)
                    {
                        var Rvalue = RBidsDict[(i, j)];

                        for (int r = 0; r < Rvalue; r++) //allagh
                        {
                            objective.AddTerm(1.0, R[i, j, r]);
                        }
                    }
                    for (int t = 0; t < Dates.Length; t++)
                    {
                        objective.AddTerm(-0.000001, X[i, t]);
                    }
                }

                model.AddMaximize(objective);

                #endregion

                #region Constrains
                // #1. Adding constraints for maximum number of satisfied bids 
                for (int i = 0; i < Employees.Length; i++)
                {
                    ILinearNumExpr sumLeaveBids = model.LinearNumExpr();

                    for (int j = 0; j < MaxLeaveBidsPerEmployee[Employees[i]]; j++)
                    {
                        var Rvalue = RBidsDict[(i, j)];

                        for (int r = 0; r < Rvalue; r++)
                        {
                            sumLeaveBids.AddTerm(1.0, R[i, j, r]);
                        }
                    }

                    // Adding the constraint for the current employee
                    model.AddLe(sumLeaveBids, MaxSatisfiedBids, "MaxSatisfiedBids_" + Employees[i]);
                }


                // #2. Entitlements
                for (int i = 0; i < Employees.Length; i++)
                {
                    var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees[i]);
                    ILinearNumExpr sumLeaveBidDays = model.LinearNumExpr();
                    for (int j = 0; j < MaxLeaveBidsPerEmployee[Employees[i]]; j++)
                    {
                        var NumberOfDays = specificEmployee.LeaveBidDataGridStatic[j].NumberOfDaysMax;
                        var Rvalue = RBidsDict[(i, j)];

                        for (int r = 0; r < Rvalue; r++)
                        {
                            var Zvalue = ZbidsDict[(i, j, r + 1)];
                            NumberOfDays = NumberOfDays - r;
                            for (int z = 0; z < Zvalue; z++)
                            {
                                sumLeaveBidDays.AddTerm(NumberOfDays, Y[i, j, r, z]); // Summing up the leave bid days for each employee
                            }
                        }
                    }

                    var MaxLeaveDays = specificEmployee.LeaveStatus.CurrentBalance;

                    // Adding the constraint for the current employee
                    model.AddLe(sumLeaveBidDays, MaxLeaveDays, "MaxLeaveDays_" + Employees[i]);
                }


                // #3. Limit Lines
                for (int t = 0; t < Dates.Length; t++)
                {
                    ILinearNumExpr expr = model.LinearNumExpr();
                    int sumdays = 0;
                    // Separate LimitLine for each day
                    var LimitLine = InputData.Schedule.ReqScheduleRowsData.ElementAt(t).LimitLine;

                    for (int i = 0; i < Employees.Length; i++)
                    {
                        expr.AddTerm(1, X[i, t]);
                        sumdays++;
                    }

                    if (sumdays > LimitLine)
                    {
                        model.AddLe(expr, LimitLine, "LimitLine_" + Dates[t]);
                    }
                }


                //#4. Overlapping
                #region  OverLapping

                for (int i = 0; i < Employees.Length; i++)
                {
                    for (int j1 = 0; j1 < MaxLeaveBidsPerEmployee[Employees[i]] - 1; j1++)
                    {
                        for (int j2 = j1 + 1; j2 < MaxLeaveBidsPerEmployee[Employees[i]]; j2++)
                        {
                            var Rvalue = RBidsDict[(i, j1)];

                            var EmployeeCode = Employees[i];
                            var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees[i]);

                            #region Find z1,z2
                            int Z1value = new int();
                            int Z2value = new int();
                            int R1value = RBidsDict[(i, j1)];
                            int R2value = RBidsDict[(i, j2)];

                            #endregion

                            for (int r1 = 0; r1 < R1value; r1++)
                            {
                                Z1value = ZbidsDict[(i, j1, r1 + 1)];

                                for (int r2 = 0; r2 < R2value; r2++)
                                {
                                    Z2value = ZbidsDict[(i, j2, r2 + 1)];

                                    for (int z1 = 0; z1 < Z1value; z1++)
                                    {
                                        for (int z2 = 0; z2 < Z2value; z2++)
                                        {
                                            if (SeparOrOverlap(i, j1, j2, z1, z2, r1, r2))
                                            {
                                                ILinearNumExpr expr = model.LinearNumExpr();
                                                expr.AddTerm(1, Y[i, j1, r1, z1]);
                                                expr.AddTerm(1, Y[i, j2, r2, z2]);
                                                model.AddLe(expr, 1, $"SO{i + 1}_{j1 + 1}_{z1 + 1}_{j2 + 1}_{z2 + 1}");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                bool SeparOrOverlap(int i, int j1, int j2, int z1, int z2, int r1, int r2)
                {

                    var emp = InputData.Employees.ElementAt(i);

                    var SelectedBid1 = emp.LeaveBidDataGridStatic.ElementAt(j1);
                    var SelectedBid2 = emp.LeaveBidDataGridStatic.ElementAt(j2);



                    if (SelectedBid2.DateFrom.AddDays(z2) >= SelectedBid1.DateFrom.AddDays(SelectedBid1.NumberOfDaysMax + SeparValue + z1 - r1 - 1))
                    {
                        Console.WriteLine("Condition: false");
                        return false;

                    };


                    if (SelectedBid2.DateFrom.AddDays(SelectedBid2.NumberOfDaysMax + z2 - r1 - 1) <= SelectedBid1.DateFrom.AddDays(-SeparValue + z1))
                    {
                        Console.WriteLine("Condition: false");
                        return false;

                    };
                    Console.WriteLine("Condition: True");
                    return true;
                }

                #endregion
                //#5. Connection Between Y and X
                for (int i = 0; i < Employees.Length; i++)
                {
                    var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees[i]);
                    var maxBids = MaxLeaveBidsPerEmployee[Employees[i]];

                    for (int j = 0; j < maxBids; j++)
                    {
                        var bid = specificEmployee.LeaveBidDataGridStatic[j];
                        var NumberOfDays = bid.NumberOfDaysMax;
                        var Rvalue = RBidsDict[(i, j)];
                        for (int r = 0; r < Rvalue; r++) 
                        {
                            var Zvalue = ZbidsDict[(i, j, r + 1)];
                            NumberOfDays = NumberOfDays - r;
                            for (int z = 0; z < Zvalue; z++) 
                            {
                                var startDateIndex = Array.IndexOf(Dates, bid.DateFrom.ToString("dd/MM/yyyy"));

                                ILinearNumExpr expr = model.LinearNumExpr();
                                expr.AddTerm(NumberOfDays, Y[i, j, r, z]);

                                int start = startDateIndex + z;
                                int end = start + NumberOfDays;

                                for (int t = start; t < end; t++)
                                {
                                    expr.AddTerm(-1, X[i, t]); // Add X variables for each day of the bid
                                }
                                // Add the constraint
                                model.AddLe(expr, 0, $"BidDaysConstraint_{Employees[i]}_{j}_{r}_{z}");

                            }
                        }
                    }
                }


                //#6. Connection Between Y and R -- Yijrz and Yijr
                for (int i = 0; i < Employees.Length; i++)
                {
                    for (int j = 0; j < MaxLeaveBidsPerEmployee[Employees[i]]; j++)
                    {
                        var Rvalue = RBidsDict[(i, j)];

                        for (int r = 0; r < Rvalue; r++)
                        {
                            var Zvalue = ZbidsDict[(i, j, r + 1)];
                            ILinearNumExpr sumYijrz = model.LinearNumExpr();
                            for (int z = 0; z < Zvalue; z++)
                            {
                                sumYijrz.AddTerm(1.0, Y[i, j, r, z]);
                            }
                            // Adding the constraint for the current employee
                            model.AddEq(R[i, j, r], sumYijrz, "Y_R_Connection" + Employees[i]);
                        }
                    }
                }


                #endregion

                #endregion

                #region New Optimize settings
                TextWriter TWoutput = File.CreateText("RunOutput.txt");

                bool grant = false;
                BasicEnums.VPLogicType logic = InputData.VPLogicType; // Λογικη Ανάθεσης π.χ Strict Seniority

                int FinishedEmpIds = 0; //Το αντιστοιχο FinishedIds στο κωδικα της Python
                int FinishedBidIds = 1; //Μετρητής για τα ολοκληρωμέναBids

                int id = 0;
                var numRowsPerEmployee = InputData.Employees.Select(e => e.LeaveBidDataGridStatic.Count);
                var numOfEmployes = InputData.Employees.Count; //Το αντιστοιχο N της python

                int N = numRowsPerEmployee.Sum(); //Το N εδω ειναι o αριθμος των συνολικών Bids.
                int[] NextBid = new int[N];
                int[] NrOfBids = MaxLeaveBidsPerEmployee.Values.ToArray();
                List<string> outputLines = new List<string>();


                //model.Update();

                while (FinishedEmpIds <= numOfEmployes) 
                {


                    int j = NextBid[id];

                    #region Check Bid
                    var z = 0;
                    var r = 0;
                    #region Find RValue
                    int Rvalue = new int();
                    var EmployeeCode = Employees[id];
                    var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees[id]);

                    var BidCode = specificEmployee.LeaveBidDataGridStatic[j].BidCode;
                    Rvalue = RBidsDict_Str.TryGetValue((Employees[id], BidCode), out int valueR) ? valueR : Rvalue;

                    #endregion
                    for (r = 0; r < Rvalue; r++)
                    {
                        #region Check Bid



                        R[id, j, r].LB = 1;
                        R[id, j, r].UB = 1;

                        model.SetOut(TWoutput);
                        model.Solve();
                        bool solution = (model.GetStatus() == Cplex.Status.Optimal);
                        if (solution)
                        {
                            double rValue = model.GetValue(R[id, j, r]);

                            grant = true;
                            string message = $"Crew member {id + 1} was awarded bid {j + 1}";
                            Console.WriteLine(message);
                            outputLines.Add(message);
                            Data.ObjValue = model.ObjValue;
                            Rvalue = 0;
                        }
                        else
                        {
                            grant = false;
                            R[id, j, r].LB = 0;
                            R[id, j, r].UB = 0;

                            string message = $"Crew member {id + 1} was not awarded bid {j + 1}";
                            Console.WriteLine(message);
                            outputLines.Add(message);
                        }

                        #endregion
                    }

                    #endregion
                    NextBid[id]++;
                    if (NextBid[id] == NrOfBids[id])
                    {
                        FinishedEmpIds++;

                    }
                    if (model.GetStatus() == Cplex.Status.Optimal)
                    {
                        Data.ObjValue = model.ObjValue;


                        FeasableModel = model;

                    }
                    if (FinishedBidIds == N)
                    {
                        break;
                    }
                    FinishedBidIds = FinishedBidIds + 1;
                    if (FinishedBidIds <= N)
                    {
                        id = GetNextId(id, grant, numOfEmployes, NextBid, NrOfBids, FinishedEmpIds, logic);
                    }
                }
                #endregion

                #endregion

                #region Save,Show Results
                var Upgrade = new bool();
                var flag = new bool();
                var customMessageBox = new CustomMessageBox("Do you want to Save the Results?");
                if (customMessageBox.ShowDialog() == true)
                {
                    // User clicked Save Only or Save and Upgrade
                    if (customMessageBox.DialogResult == true)
                    {
                        // User clicked Save Only or Save and Upgrade
                        //var result = customMessageBox.Message.Contains("Save and Upgrade") ? "Save and Upgrade" : "Save Only";
                        var result = "Save";
                        Console.WriteLine($"User clicked {result}");

                        if (result == "Save" )
                        {
                            #region OutputResults
                            if (Data.ObjValue > 0)
                            {

                                FeasableModel.Solve();
                                using (FeasableModel)
                                {
                                    FeasableModel.Solve();

                                    #region Print Results

                                    FeasableModel.ExportModel(Path.Combine(relativePath, "VP.lp"));
                                    FeasableModel.ExportModel(Path.Combine(relativePath, "VP.mps"));
                                    FeasableModel.WriteSolution(Path.Combine(relativePath, "VP.sol"));

                                    #endregion

                                    Data.ObjValue = FeasableModel.ObjValue;
                                    //Data.ObjValue = Math.Round(Data.ObjValue);
                                    #region Insert Xij
                                    // Extract the optimal solution for the 'X' variables
                                    for (int i = 0; i < Employees.Length; i++)
                                    {
                                        for (int t = 0; t < Dates.Length; t++)
                                        {
                                            string employee = Employees[i];
                                            string date = Dates[t];
                                            //GRBVar Xit = modelFromFiles.GetVarByName($"X{i + 1}_{t + 1}");

                                            double xValue = FeasableModel.GetValue(X[i, t]);
                                            if (xValue == 1)
                                            {
                                                Console.WriteLine($"Employee: {employee}, Date: {date}, Value: {xValue}");
                                            }
                                            // Store the optimal 'X' value in the data structure
                                            make_plan[(employee, date)] = xValue;

                                            // Add 'employee' and 'date' to the respective lists if they are not already there
                                            if (!rows.Contains(employee))
                                                rows.Add(employee);
                                            if (!columns.Contains(date))
                                                columns.Add(date);
                                        }
                                    }
                                    #endregion


                                    #region Print the optimal solution for 'X' variables
                                    Console.WriteLine("Optimal Solution for X Variables:");
                                    foreach (var employee in rows)
                                    {
                                        foreach (var date in columns)
                                        {
                                            double xValue = make_plan.ContainsKey((employee, date)) ? make_plan[(employee, date)] : 0.0;
                                            Console.WriteLine($"Employee: {employee}, Date: {date}, Value: {xValue} -> Employee: {employee}, Date: {date}, Value: {xValue} X{(Array.IndexOf(Employees, employee) + 1)}{(Array.IndexOf(Dates, date) + 1)}");


                                            #region Populate VP Xij
                                            VPXijResultsData singleDataRecord = new VPXijResultsData();


                                            singleDataRecord.Xij = $"X{(Array.IndexOf(Employees, employee) + 1)}{(Array.IndexOf(Dates, date) + 1)}";
                                            singleDataRecord.XijFlag = xValue;
                                            singleDataRecord.Date = date;




                                            var SpecificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == employee);
                                            singleDataRecord.Employee = SpecificEmployee;

                                            Data.VPXijResultsDataGrid.Add(singleDataRecord);
                                            #endregion
                                        }

                                    }
                                    #endregion

                                    #region Extract the optimal solution for the 'Y' variables
                                    Dictionary<(string, int, int, int), double> y_plan = new Dictionary<(string, int, int, int), double>();
                                    for (int i = 0; i < Employees.Length; i++)
                                    {
                                        for (int j = 0; j < MaxLeaveBidsPerEmployee[Employees[i]]; j++)
                                        {
                                            var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees[i]);
                                            var Rvalue = RBidsDict[(i, j)];

                                            for (int r = 0; r < Rvalue; r++)
                                            {
                                                var Zvalue = ZbidsDict[(i, j, r + 1)];

                                                double rValue = FeasableModel.GetValue(R[i,j,r]);


                                                for (int z = 0; z < Zvalue; z++) //allagh
                                                {
                                                    string employee = Employees[i];
                                                    int bidIndex = j;

                                                    double yValue = FeasableModel.GetValue(Y[i, j, r,z]);

                                                    // Store the optimal 'Y' value in the data structure
                                                    y_plan[(employee, bidIndex, r, z)] = yValue;
                                                }
                                            }







                                        }
                                    }

                                    #endregion
                                    #region Print the optimal solution for 'Y' variables
                                    Console.WriteLine("\nOptimal Solution for Y Variables:");
                                    int counter = 0;
                                    foreach (var employee in rows)
                                    {
                                        var TotalNumberOfDays = 0;

                                        for (int j = 0; j < MaxLeaveBidsPerEmployee[employee]; j++)
                                        {
                                            #region Find ZValue
                                            int Zvalue = new int();
                                            int Rvalue = new int();
                                            var EmployeeCode = employee;
                                            var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == EmployeeCode);
                                            var BidCode = specificEmployee.LeaveBidDataGridStatic[j].BidCode;
                                            Rvalue = RBidsDict_Str.TryGetValue((EmployeeCode, BidCode), out int valueR) ? valueR : Zvalue;
                                            #endregion

                                            for (int r = 0; r < Rvalue; r++)
                                            {
                                                Zvalue = ZbidsDict_Str.TryGetValue((EmployeeCode, BidCode, r + 1), out int value) ? value : Zvalue;
                                                for (int z = 0; z < Zvalue; z++) //allagh
                                                {
                                                    int bidIndex = j;
                                                    double yValue = y_plan.ContainsKey((employee, bidIndex, r, z)) ? y_plan[(employee, bidIndex, r, z)] : 0.0;

                                                    Console.WriteLine($"Employee: {employee}, BidIndex: {bidIndex + 1}, Value: {yValue} -> Employee: {employee}, BidIndex: {bidIndex + 1}, Value: {yValue} Y{(Array.IndexOf(Employees, employee) + 1)}{(bidIndex + 1)}");

                                                    #region Populate VP Yij
                                                    VPYijResultsData yijDataRecord = new VPYijResultsData();
                                                    yijDataRecord.LeaveBidData = new LeaveBidsDataStatic();


                                                    yijDataRecord.Yij = $"Y{(Array.IndexOf(Employees, employee) + 1)}{(bidIndex + 1)}";
                                                    yijDataRecord.Yijr = $"Y{(Array.IndexOf(Employees, employee) + 1)}{(bidIndex + 1)}{(r + 1)}";
                                                    yijDataRecord.Yijrz = $"Y{(Array.IndexOf(Employees, employee) + 1)}{(bidIndex + 1)}{(r + 1)}{(z + 1)}";

                                                    yijDataRecord.YijFlag = yValue;
                                                    yijDataRecord.ConfirmedBidFlag = yValue;

                                                    var SpecificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == employee);
                                                    yijDataRecord.Employee = SpecificEmployee;

                                                    yijDataRecord.LeaveBidData = SpecificEmployee.LeaveBidDataGridStatic[j];

                                                    #region Edit Dates

                                                    var DateFrom = SpecificEmployee.LeaveBidDataGridStatic[j].DateFrom;
                                                    var NumberOfDays = SpecificEmployee.LeaveBidDataGridStatic[j].NumberOfDaysMax - r;
                                                    var NumberOfDaysMax = SpecificEmployee.LeaveBidDataGridStatic[j].NumberOfDaysMax;
                                                    var NumberOfDaysMin = SpecificEmployee.LeaveBidDataGridStatic[j].NumberOfDaysMin;

                                                    var BidType = SpecificEmployee.LeaveBidDataGridStatic[j].BidType;
                                                    var DateTo = SpecificEmployee.LeaveBidDataGridStatic[j].DateTo;

                                                    yijDataRecord.DateFrom = DateFrom;
                                                    yijDataRecord.DateTo = DateTo;

                                                    if (BidType == BasicEnums.BidType.Min_Max)
                                                    {
                                                        NumberOfDays = 0;
                                                    }
                                                    else if (BidType == BasicEnums.BidType.Non_Specific)
                                                    {
                                                        NumberOfDaysMax = 0;
                                                        NumberOfDaysMin = 0;

                                                    }
                                                    else if (BidType == BasicEnums.BidType.Specific)
                                                    {
                                                        NumberOfDaysMax = 0;
                                                        NumberOfDaysMin = 0;
                                                    }

                                                    yijDataRecord.NumberOfDays = NumberOfDays;
                                                    yijDataRecord.NumberOfDaysMax = NumberOfDaysMax;
                                                    yijDataRecord.NumberOfDaysMin = NumberOfDaysMin;

                                                    yijDataRecord.DateFromStr = DateFrom.ToString("dd/MM/yyyy");
                                                    yijDataRecord.DateToStr = DateTo.ToString("dd/MM/yyyy");

                                                    #endregion

                                                    #region ADD RECORD 
                                                    var existingRecord = Data.VPYijResultsDataGrid.FirstOrDefault(record => record.Yij == yijDataRecord.Yij);

                                                    if (existingRecord != null)
                                                    {
                                                        if (existingRecord.YijFlag == 1)
                                                        {

                                                        }
                                                        else if (existingRecord.YijFlag == 0 && yijDataRecord.YijFlag == 0)
                                                        {

                                                        }
                                                        else if (existingRecord.YijFlag == 0 && yijDataRecord.YijFlag == 1)
                                                        {
                                                            // Insert the new record and remove the existing record
                                                            Data.VPYijResultsDataGrid.Remove(existingRecord);
                                                            Data.VPYijResultsDataGrid.Add(yijDataRecord);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Data.VPYijResultsDataGrid.Add(yijDataRecord);

                                                    }

                                                    #endregion



                                                    #endregion

                                                    #region Populate VP Yijz
                                                    VPYijResultsData yijzDataRecord = new VPYijResultsData();
                                                    yijzDataRecord.LeaveBidData = new LeaveBidsDataStatic();


                                                    yijzDataRecord.Yij = $"Y{(Array.IndexOf(Employees, employee) + 1)}{(bidIndex + 1)}";
                                                    yijzDataRecord.Yijr = $"Y{(Array.IndexOf(Employees, employee) + 1)}{(bidIndex + 1)}{(r + 1)}";
                                                    yijzDataRecord.Yijrz = $"Y{(Array.IndexOf(Employees, employee) + 1)}{(bidIndex + 1)}{(r + 1)}{(z + 1)}";

                                                    yijzDataRecord.YijFlag = yValue;
                                                    yijzDataRecord.ConfirmedBidFlag = yValue;

                                                    yijzDataRecord.Employee = SpecificEmployee;

                                                    yijzDataRecord.LeaveBidData = SpecificEmployee.LeaveBidDataGridStatic[j];

                                                    #region Edit Dates


                                                    DateFrom = SpecificEmployee.LeaveBidDataGridStatic[j].DateFrom.AddDays(z);
                                                    NumberOfDays = SpecificEmployee.LeaveBidDataGridStatic[j].NumberOfDaysMax - r;
                                                    DateTo = DateFrom.AddDays(NumberOfDays - 1);

                                                    yijzDataRecord.DateFrom = DateFrom;
                                                    yijzDataRecord.DateTo = DateTo;
                                                    yijzDataRecord.NumberOfDays = NumberOfDays;
                                                    yijzDataRecord.DateFromStr = DateFrom.ToString("dd/MM/yyyy");
                                                    yijzDataRecord.DateToStr = DateTo.ToString("dd/MM/yyyy");

                                                    #endregion
                                                    Data.VPYijzResultsDataGrid.Add(yijzDataRecord);



                                                    #endregion
                                                    if (yValue == 1)
                                                    {
                                                        TotalNumberOfDays = TotalNumberOfDays + NumberOfDays;

                                                    }
                                                }
                                            } //allagh

                                            counter++;
                                        }

                                        var UpdatedEmp = InputData.Employees.FirstOrDefault(emp => emp.Code == employee);
                                        UpdatedEmp.LeaveStatus.ProjectedBalance = UpdatedEmp.LeaveStatus.CurrentBalance - TotalNumberOfDays;
                                        Data.EmpLeaveStatusData.Add(UpdatedEmp);

                                    }
                                    #endregion

                                    #region Create c#sol.txt for python
                                    //string filePath = @"C:\Users\npoly\Source\Repos\Bids_CrewScheduling_Kozanidis\c#sol.txt";
                                    //File.WriteAllText(filePath, string.Empty);

                                    //using (StreamWriter writer = new StreamWriter(filePath, true)) // 'true' parameter appends to the existing file if it exists
                                    //{
                                    //    foreach (string line in outputLines)
                                    //    {
                                    //        writer.WriteLine(line);
                                    //    }
                                    //}

                                    #endregion
                                }
                                #endregion
                            }
                            if (result == "Dont Save")
                            {
                                // Handle Save and Upgrade scenario
                                Console.WriteLine("Dont Save...");
                                //flag = SaveVpVijResultData(Data, 1, InputData.VPId);
                                //Console.WriteLine(flag);
                                Upgrade = true;
                            }
                            else
                            {
                                // Handle Save Only scenario
                                Console.WriteLine("...");
                                //flag = SaveVpVijResultData(Data, 1, InputData.VPId);
                                //Console.WriteLine(flag);
                                Upgrade = false;

                            }
                        }
                        else
                        {
                            // User clicked Upgrade Only
                            Console.WriteLine("User clicked Upgrade Only");
                            // Handle Upgrade Only scenario
                            //flag = SaveVpVijResultData(Data, -1, InputData.VPId);
                            Console.WriteLine(flag);
                            Upgrade = true;

                        }
                    }
                    while (Upgrade == true)
                    {
                        var CurrentObjectiveValue = Data.ObjValue;
                        var NewInputData = InputData;


                        //Data = CalculateVacationPlanningAdvanced2(NewInputData,Yijk);
                    }
                }
                model.End(); 


                #endregion

                return Data;


            }
            catch (ILOG.Concert.Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return Data;
            }
        }

        public int GetNextId(int aId, bool accept, int N, int[] NextBid, int[] NrOfBids, int FinishedEmpIds, BasicEnums.VPLogicType VPLogicType)
        {
            try
            {
                int RId = 0;
                var logic = VPLogicType;
                if (FinishedEmpIds == N)
                    return 0;

                if (logic == BasicEnums.VPLogicType.Strict_Seniority)
                {
                    RId = aId;
                }
                else if (logic == BasicEnums.VPLogicType.Fair_Assignment)
                {
                    if (accept == true)
                    {
                        RId = aId + 1;
                    }
                    else
                    {
                        RId = aId;
                    }
                }
                else if (logic == BasicEnums.VPLogicType.Bid_By_Bid)
                {
                    RId = aId + 1;
                }

                if (RId == N)
                {
                    RId = 0;
                }

                while (NextBid[RId] == NrOfBids[RId])
                {
                    RId++;
                    if (RId == N)
                    {
                        RId = 0;
                    }
                }

                return RId;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return -1;
            }
        }

        public bool SaveVpVijResultData(VacationPlanningOutputData Data, int ReplicationNumber, int VPID)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {

                    foreach (var row in Data.VPYijzResultsDataGrid)
                    {
                        VPYijzResultsDataEntity newRow = new VPYijzResultsDataEntity();

                        var BidId = dbContext.LeaveBids.SingleOrDefault(b => b.BidCode == row.LeaveBidData.BidCode).BidId;
                        newRow.BidId = BidId;
                        newRow.EmpId = row.Employee.EmployeeId;
                        newRow.VPID = VPID;
                        newRow.Yij = row.Yij;
                        newRow.Yijr = row.Yijr;
                        newRow.Yijrz = row.Yijrz;

                        newRow.DateFromStr = row.DateFromStr;
                        newRow.DateToStr = row.DateToStr;
                        newRow.NumberOfDays = row.NumberOfDays;
                        newRow.NumberOfDaysMin = row.NumberOfDaysMin;
                        newRow.NumberOfDaysMax = row.NumberOfDaysMax;
                        newRow.ReplicationNumber = ReplicationNumber;

                        bool Confirmed = new bool();

                        if (row.YijFlag == 0)
                        {
                            Confirmed = false;
                        }
                        else
                        {
                            Confirmed = true;
                        }
                        newRow.Confirmed = Confirmed;


                        dbContext.VPYijzResults.Add(newRow);

                    }

                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                LogError(ex, "SaveVpVijResultData", "Notes");
                return false;
            }
        }

        public int CreatePythonTxt(VacationPlanningInputData InputData)
        {

            string[] Employees = InputData.Employees.Select(d => d.Code).ToArray();

            int MaxLeaveBids = InputData.MaxLeaveBids;
            int MaxSatisfiedBids = InputData.MaxSatisfiedBids;
            string[] Dates = InputData.DatesStr;
            int SeparValue = InputData.SeparValue;
            int LimitLineFixed = InputData.Schedule.LimitLineFixed;
            int numOfEmployes = InputData.Employees.Count;

            int[] generalxt = { LimitLineFixed, SeparValue, MaxSatisfiedBids };
            int[] entitlementstxt = new int[numOfEmployes];
            int[] NumberOfBidstxt = new int[numOfEmployes];

            int[][] dateStarttxt = new int[numOfEmployes][];
            int[][] dateLengthtxt = new int[numOfEmployes][];

            Dictionary<string, int> MaxLeaveBidsPerEmployee = InputData.MaxLeaveBidsPerEmployee;
            int[] NrOfBids = MaxLeaveBidsPerEmployee.Values.ToArray();
            NumberOfBidstxt = NrOfBids;

            #region Python Insert Entiltements,NubmerOfBids
            var i = 0;
            foreach (var emp in InputData.Employees)
            {
                entitlementstxt[i] = emp.LeaveStatus.CurrentBalance;
                NumberOfBidstxt[i] = emp.LeaveBidDataGridStatic.Count; //ALAGH

                dateStarttxt[i] = new int[MaxLeaveBids];
                dateLengthtxt[i] = new int[MaxLeaveBids];
                for (int j = 0; j < MaxLeaveBidsPerEmployee[emp.Code]; j++)
                {

                    var DateFrom = emp.LeaveBidDataGridStatic[j].DateFrom.ToString("dd/MM/yyyy");
                    var DateFromIndex = Dates.IndexOf(DateFrom);

                    dateStarttxt[i][j] = DateFromIndex + 1;
                    dateLengthtxt[i][j] = emp.LeaveBidDataGridStatic[j].NumberOfDays;
                }


                i++;
            }



            #endregion

            #region Create Notepad For Python 2nd part


            //// Specify the file path
            //string filePath = @"C:\Users\npoly\Source\Repos\Bids_CrewScheduling_Kozanidis\vms_data.txt";

            //// Write data to the text file and print to console for debugging
            //using (StreamWriter writer = new StreamWriter(filePath))
            //{
            //    // Write LimitLine, SeparValue, and MaxBids
            //    for (int a = 0; a < generalxt.Length; a++)
            //    {
            //        writer.Write($"{generalxt[a]} ");
            //        Console.Write($"{generalxt[a]} ");

            //    }
            //    writer.WriteLine("\n");
            //    Console.WriteLine("\n");

            //    // Write Entitlements
            //    for (int a = 0; a < entitlementstxt.Length; a++)
            //    {
            //        writer.Write($"{entitlementstxt[a]} ");
            //        Console.Write($"{entitlementstxt[a]} ");

            //    }
            //    writer.WriteLine("\n");
            //    Console.WriteLine("\n");

            //    // Write NumberOfBids
            //    for (int a = 0; a < NumberOfBidstxt.Length; a++)
            //    {
            //        writer.Write($"{NumberOfBidstxt[a]} ");
            //        Console.Write($"{NumberOfBidstxt[a]} ");

            //    }
            //    writer.WriteLine("\n");
            //    Console.WriteLine("\n");

            //    // Write DateStart
            //    int rowCount = dateStarttxt.Length;
            //    int currentRow = 0;
            //    foreach (int[] row in dateStarttxt)
            //    {
            //        currentRow++;
            //        foreach (int value in row)
            //        {
            //            if (value > 0)
            //            {
            //                writer.Write($"{value} ");
            //                Console.Write($"{value} ");
            //            }

            //        }
            //        if (currentRow < rowCount) // Check if it's not the last row
            //        {
            //            writer.WriteLine();
            //            Console.WriteLine();
            //        }
            //    }

            //    writer.WriteLine("\n");
            //    Console.WriteLine("\n");

            //    // Write DateLength
            //    foreach (int[] row in dateLengthtxt)
            //    {
            //        foreach (int value in row)
            //        {

            //            if (value > 0)
            //            {
            //                writer.Write($"{value} ");
            //                Console.Write($"{value} ");
            //            }
            //        }
            //        writer.WriteLine(" ");
            //        Console.WriteLine(" ");
            //    }
            //}

            #endregion

            return 1;
        }

        public VPCGOutputData CalculateVPColumnGeneration(VPCGInputData InputData)
        {
            VPCGOutputData Data = new VPCGOutputData();
            string relativePath = Path.Combine("OptimizationResults", "CPLEX", "Thesis", "VP_Column_Generation");
            Directory.CreateDirectory(relativePath);
            try
            {
                // Initialize CPLEX environment and model
                Cplex model = new Cplex();
                model.SetOut(new System.IO.StreamWriter("vpcglogfile_Cplex.log"));

                #region Optimization parameters

                string[] Dates = InputData.Dates; // Array with the dates
                Dictionary<int, int> LeaveDays = InputData.LeaveDays;
                Dictionary<int, int> LLiDict = InputData.LLiDict;

                #endregion

                #region Decision Variables
                // Decision variables

                INumVar[] X = new INumVar[LeaveDays.Count];

                for (int i = 0; i < LeaveDays.Count; i++)
                {
                    // Define the variable name
                    string varNameX = $"X{i + 1}";

                    // Create the binary variable with a name
                    X[i] = model.NumVar(0,1,NumVarType.Bool,varNameX);
                }

                #endregion

                #region Objective Function

                ILinearNumExpr objective = model.LinearNumExpr();

                for (int i = 0; i < LeaveDays.Count; i++)
                {
                    int Multiplier = LeaveDays[i + 1] * 1000;
                    objective.AddTerm(Multiplier, X[i]);
                }

                model.AddMinimize(objective);

                #endregion

                #region Constraints

                // #1. MC Employees
                for (int i = 0; i < LeaveDays.Count; i++)
                {
                    ILinearNumExpr expr = model.LinearNumExpr();
                    expr.AddTerm(1, X[i]);
                    model.AddEq(expr, 1, "MC_" + (i + 1));
                }

                // #2. Days
                for (int t = 0; t < Dates.Length; t++)
                {
                    ILinearNumExpr expr = model.LinearNumExpr();
                    model.AddGe(expr, 0, "Day_" + (t + 1));
                }

                // #3. Limit Lines
                for (int t = 0; t < Dates.Length; t++)
                {
                    ILinearNumExpr expr = model.LinearNumExpr();

                    model.AddLe(expr, LLiDict[t + 1], "LimitLine_" + (t + 1));
                }

                #endregion
                model.Solve();
                // Solve the model
                if (model.GetStatus() == Cplex.Status.Optimal)
                {
                    
                    Data.ObjValue = model.ObjValue;

                    // Export model to files
                    model.ExportModel(Path.Combine(relativePath, "VP_CG.lp"));
                    model.ExportModel(Path.Combine(relativePath, "VP_CG.mps"));
                    model.WriteSolution(Path.Combine(relativePath, "VP_CG.sol"));



                    // Saving the model state, equivalent to .mst in Gurobi
                }

                // Dispose of CPLEX objects
                model.End();
            }
            catch (ILOG.Concert.Exception ex)
            {
                Console.WriteLine("Concert exception: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("System exception: " + ex.Message);
            }

            return Data;
        }

        #endregion

        #region CrewScheduling
        public CSOutputData CalculateCrewScheduling_Cplex(CSInputData InputData)
        {
            CSOutputData Data = new CSOutputData();

            string relativePath = Path.Combine("OptimizationResults", "CPLEX", "Thesis", "Crew_Scheduling");
            Directory.CreateDirectory(relativePath);
            // Save the files


            try
            {
                // Initialize CPLEX environment and model
                Cplex model = new Cplex();
                model.SetOut(new System.IO.StreamWriter("cslogfile_Cplex.log"));

                #region Optimization parameters

                var T = InputData.T; // Planning Horizon
                var N = InputData.N; // Number Of Employees Empty Schedules
                var R = InputData.R; // Number Of Routes

                var DatesIndexMap = InputData.DatesIndexMap;
                var EmployeesIndexMap = InputData.EmployeesIndexMap;
                var RoutesIndexMap = InputData.RoutesIndexMap;

                var RoutesDates_Dict = InputData.RoutesDates_Dict;
                var RoutesDay_Dict = InputData.RoutesDay_Dict;
                var RoutesTime_Dict = InputData.RoutesTime_Dict;

                var EmpBounds_Dict = InputData.EmpBounds_Dict;

                #endregion

                #region Decision Variables
                // Decision variables

                INumVar[] X = new INumVar[R + N];

                for (int i = 0; i < R + N; i++)
                {
                    // Define the variable name
                    string varNameX = $"X{i + 1}";

                    // Create the binary variable with a name
                    X[i] = model.NumVar(0, 1, NumVarType.Bool, varNameX);
                }

                #endregion

                #region Objective Function

                ILinearNumExpr objective = model.LinearNumExpr();

                for (int i = 0; i < R; i++)
                {
                    int RoutesPenalty = InputData.RoutesPenalty;
                    objective.AddTerm(RoutesPenalty, X[i]);
                }

                model.AddMinimize(objective);

                #endregion

                #region Constraints

                // #1. C1 -> C40 , X174-> X213 ROUTES 
                for (int i = R; i < R + N; i++)
                {
                    ILinearNumExpr expr = model.LinearNumExpr();
                    expr.AddTerm(1, X[i]);
                    model.AddEq(expr, 1, "C_" + (i - R + 1));
                }

                // #2. C41 -> C213 , X1-> X173 ROUTES 
                for (int i = 0; i < R; i++)
                {
                    ILinearNumExpr expr = model.LinearNumExpr();
                    expr.AddTerm(1, X[i]);
                    model.AddEq(expr, 1, "C_" + (i + R + 1));
                }

                #endregion

                // Solve the model
                model.Solve();
                bool solution = (model.GetStatus() == Cplex.Status.Optimal);
                if (solution)
                {
                    Data.ObjValue = model.ObjValue;

                    // Export model to files
                    model.ExportModel("CSFeasable_Cplex.lp");
                    model.ExportModel("CSFeasableMPS_Cplex.mps");



                }

                // Dispose of CPLEX objects
                model.End();
            }
            catch (ILOG.Concert.Exception ex)
            {
                Console.WriteLine("A CPLEX error occurred: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("A System error occurred: " + ex.Message);
            }

            return Data;
        }

        #endregion
    }
}
