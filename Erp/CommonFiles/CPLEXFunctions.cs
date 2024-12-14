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
//using ILOG.Concert;
//using ILOG.CPLEX;
using Microsoft.EntityFrameworkCore.Metadata;
using Syncfusion.Data.Extensions;
using System.IO;
using Erp.Model.Thesis.CrewScheduling;
using System.Windows;
using Syncfusion.Windows.Shared;

namespace Erp.CommonFiles
{
    public class CPLEXFunctions : RepositoryBase
    {
        //#region Vacation Planning
        //public VacationPlanningOutputData CalculateVacationPlanning_CPLEX(VacationPlanningInputData InputData)
        //{
        //    VacationPlanningOutputData OutputData = new VacationPlanningOutputData();

        //    try
        //    {
        //        #region Optimization Algorithm

        //        #region Indexes 

        //        int I = InputData.Employees.Count();
        //        int T = InputData.Dates.Count();
        //        int Jmax = InputData.N_Dict.Max(kvp => kvp.Value);
        //        int Rmax = InputData.RBids_Dict.Max(kvp => kvp.Value);
        //        int Zmax = InputData.ZBids_Dict.Max(kvp => kvp.Value);

        //        #endregion

        //        #region Parameters

        //        int Bmax = InputData.Bmax;
        //        int Se = InputData.Se;
        //        Dictionary<int, int> LL = InputData.LLi_Dict;
        //        Dictionary<int, int> E = InputData.MaxD_Dict;
        //        Dictionary<(int, int), int> DF = InputData.DateFrom_Dict;
        //        Dictionary<(int, int), int> DT = InputData.DateTo_Dict;
        //        Dictionary<(int, int), int> NDB = InputData.NDays_Dict;
        //        Dictionary<int, int> JB = InputData.N_Dict;
        //        Dictionary<(int, int), int> RB = InputData.RBids_Dict;
        //        Dictionary<(int, int, int), int> ZB = InputData.ZBids_Dict;

        //        #endregion

        //        #region Cplex Model Initialization

        //        Cplex model = new Cplex();
        //        Cplex FeasableModel = new Cplex();

        //        #endregion

        //        #region Decision Variables 
        //        INumVar[,,,] Y = new INumVar[I, Jmax, Rmax, Zmax];
        //        INumVar[,] X = new INumVar[I, T];
        //        INumVar[,,] R = new INumVar[I, Jmax, Rmax];

        //        // Create decision variables X
        //        for (int i = 0; i < I; i++)
        //            for (int t = 0; t < T; t++)
        //            {
        //                string varNameX = $"X{i + 1}_{t + 1}";

        //                X[i, t] = model.NumVar(0, 1, NumVarType.Bool, varNameX);
        //            }


        //        // Create decision variables Y
        //        for (int i = 0; i < I; i++)
        //            for (int j = 0; j < JB[i]; j++)
        //                for (int r = 0; r < RB[(i, j)]; r++)
        //                    for (int z = 0; z < ZB[(i, j, r)]; z++)
        //                    {
        //                        string varNameY = $"Y{i + 1}_{j + 1}_{r + 1}_{z + 1}";

        //                        Y[i, j, r, z] = model.NumVar(0, 1, NumVarType.Bool, varNameY);
        //                    }




        //        // Create decision variables R
        //        for (int i = 0; i < I; i++)
        //            for (int j = 0; j < JB[i]; j++)
        //                for (int r = 0; r < RB[(i, j)]; r++)
        //                {
        //                    string varNameR = $"R{i + 1}_{j + 1}_{r + 1}";

        //                    R[i, j, r] = model.NumVar(0, 0, NumVarType.Bool, varNameR);
        //                }
        //        #endregion

        //        #region Constrains

        //        // #2. Maximum Satisfied Bids Per Employee
        //        for (int i = 0; i < I; i++)
        //        {
        //            ILinearNumExpr sumLeaveBids = model.LinearNumExpr();

        //            for (int j = 0; j < JB[i]; j++)
        //            {
        //                for (int r = 0; r < RB[(i, j)]; r++)
        //                {
        //                    sumLeaveBids.AddTerm(1.0, R[i, j, r]);
        //                }
        //            }

        //            model.AddLe(sumLeaveBids, Bmax, "Con2_" + (i + 1));
        //        }


        //        // #3. Entitlements
        //        for (int i = 0; i < I; i++)
        //        {
        //            ILinearNumExpr SumNDB = model.LinearNumExpr();
        //            for (int j = 0; j < JB[i]; j++)
        //            {
        //                var NDays = NDB[(i, j)];

        //                for (int r = 0; r < RB[(i, j)]; r++)
        //                {
        //                    NDays = NDays - r;

        //                    SumNDB.AddTerm(NDays, R[i, j, r]); // Summing up the leave bid days for each employee
        //                }
        //            }
        //            model.AddLe(SumNDB, E[i], "Con3_" + (i + 1));
        //        }


        //        // #4. Limit Lines
        //        for (int t = 0; t < T; t++)
        //        {
        //            ILinearNumExpr expr = model.LinearNumExpr();

        //            int sumdays = 0;

        //            for (int i = 0; i < I; i++)
        //            {
        //                expr.AddTerm(1, X[i, t]);
        //                sumdays++;
        //            }

        //            if (sumdays > LL[t])
        //            {
        //                model.AddLe(expr, LL[t], "Con4_" + (t + 1));
        //            }
        //        }


        //        //#5. Separation or Overlapping
        //        #region  OverLapping

        //        for (int i = 0; i < I; i++)
        //            for (int j1 = 0; j1 < JB[i] - 1; j1++)
        //                for (int j2 = j1 + 1; j2 < JB[i]; j2++)
        //                    for (int r1 = 0; r1 < RB[(i, j1)]; r1++)
        //                        for (int r2 = 0; r2 < RB[(i, j2)]; r2++)
        //                            for (int z1 = 0; z1 < ZB[(i, j1, r1)]; z1++)
        //                                for (int z2 = 0; z2 < ZB[(i, j2, r2)]; z2++)
        //                                    if (SeparOrOverlap(i, j1, j2, z1, z2, r1, r2))
        //                                    {
        //                                        ILinearNumExpr expr = model.LinearNumExpr();
        //                                        expr.AddTerm(1, Y[i, j1, r1, z1]);
        //                                        expr.AddTerm(1, Y[i, j2, r2, z2]);
        //                                        model.AddLe(expr, 1, $"Con5_{i + 1}_{j1 + 1}_{z1 + 1}_{j2 + 1}_{z2 + 1}");
        //                                    }








        //        bool SeparOrOverlap(int i, int j1, int j2, int z1, int z2, int r1, int r2)
        //        {
        //            int Bid1_DateFrom = InputData.DateFrom_Dict[(i, j1)];
        //            int Bid2_DateFrom = InputData.DateFrom_Dict[(i, j2)];
        //            int Bid1_NBDays = InputData.NDays_Dict[(i, j1)];
        //            int Bid2_NBDays = InputData.NDays_Dict[(i, j2)];

        //            if (Bid2_DateFrom + z2 >= Bid1_DateFrom + Bid1_NBDays + Se + z1 - r1)
        //            {
        //                Console.WriteLine("Condition: false");
        //                return false;
        //            };

        //            if (Bid2_DateFrom + Bid2_NBDays + z2 - r1 <= Bid1_DateFrom - Se + z1)
        //            {
        //                Console.WriteLine("Condition: false");
        //                return false;
        //            };

        //            Console.WriteLine("Condition: True");
        //            return true;
        //        }

        //        #endregion
        //        //#6. Connection Between Y and X
        //        for (int i = 0; i < I; i++)
        //            for (int j = 0; j < JB[i]; j++)
        //            {
        //                int NumberOfDays = NDB[(i, j)];

        //                for (int r = 0; r < RB[(i, j)]; r++)
        //                {
        //                    NumberOfDays = NDB[(i, j)] - r;
        //                    for (int z = 0; z < ZB[(i, j, r)]; z++)
        //                    {
        //                        ILinearNumExpr expr = model.LinearNumExpr();
        //                        expr.AddTerm(NumberOfDays, Y[i, j, r, z]);

        //                        int start = DF[(i, j)] + z;
        //                        int end = start + NumberOfDays;

        //                        for (int t = start; t < end; t++)
        //                        {
        //                            expr.AddTerm(-1, X[i, t]); // Add X variables for each day of the bid
        //                        }
        //                        model.AddLe(expr, 0, $"Con6_{i + 1}_{j + 1}_{r + 1}_{z + 1}");
        //                    }
        //                }
        //            }

        //        //#7. Connection Between Y and R 
        //        for (int i = 0; i < I; i++)
        //            for (int j = 0; j < JB[i]; j++)
        //                for (int r = 0; r < RB[(i, j)]; r++)
        //                {
        //                    ILinearNumExpr sumYijrz = model.LinearNumExpr();
        //                    for (int z = 0; z < ZB[(i, j, r)]; z++)
        //                    {
        //                        sumYijrz.AddTerm(1.0, Y[i, j, r, z]);
        //                    }

        //                    model.AddEq(R[i, j, r], sumYijrz, $"Con7_{i + 1}_{j + 1}_{r + 1}");
        //                }

        //        #endregion

        //        #region Check Bids (Strict Seniority, Fair Priority, Bid by Bid)

        //        //Bids Assignment Logic e.x Strict Seniority,Fair Priority , Bid by Bid
        //        BasicEnums.VPLogicType logic = InputData.VPLogicType;

        //        //Total Number of Bids
        //        int N = JB.Values.Sum();

        //        int[] NextBid = new int[N];

        //        //Transfom N_Dict to Array
        //        int[] NrOfBids = JB.Values.ToArray();

        //        //Μετρητες για τα ολοκληρωμένα Bids και Employees
        //        int FinishedBids = 1;
        //        int FinishedEmpIds = 0;

        //        //Support Variables Initialize
        //        bool grant = false;
        //        int EmpId = 0;

        //        while (FinishedEmpIds <= I)
        //        {
        //            int j = NextBid[EmpId];

        //            #region Select  Rijr

        //            var r = 0;
        //            int Rvalue = new int();
        //            Rvalue = RB[(EmpId, j)];

        //            #endregion
        //            for (r = 0; r < Rvalue; r++)
        //            {
        //                #region Check Bid

        //                R[EmpId, j, r].LB = 1;
        //                R[EmpId, j, r].UB = 1;

        //                model.Solve();
        //                if (model.GetStatus() == Cplex.Status.Optimal)
        //                {
        //                    string message = $"Crew member {EmpId + 1} was awarded bid {j + 1}";
        //                    Console.WriteLine(message);
        //                    grant = true;

        //                    //Update Latest Feasable Model ,ObjectiveValue
        //                    OutputData.ObjValue++;
        //                    FeasableModel = model;

        //                    //We set Rvalue = 0 in order to break from the R Loop 
        //                    Rvalue = 0;
        //                }
        //                else
        //                {
        //                    grant = false;
        //                    R[EmpId, j, r].LB = 0;
        //                    R[EmpId, j, r].UB = 0;

        //                    string message = $"Crew member {EmpId + 1} was not awarded bid {j + 1}";
        //                    Console.WriteLine(message);
        //                }
        //                #endregion
        //            }

        //            NextBid[EmpId]++;
        //            if (NextBid[EmpId] == NrOfBids[EmpId])
        //            {
        //                FinishedEmpIds++;
        //            }
        //            if (FinishedBids == N)
        //            {
        //                break;
        //            }
        //            FinishedBids = FinishedBids + 1;
        //            if (FinishedBids <= N)
        //            {
        //                EmpId = GetNextId(EmpId, grant, I, NextBid, NrOfBids, FinishedEmpIds, logic);
        //            }
        //        }


        //        #endregion

        //        #endregion

        //        #region Support Arrays

        //        string[] Employees_Array = InputData.Employees.Select(d => d.Code).ToArray();
        //        string[] Dates_Array = InputData.DatesStr;

        //        #endregion

        //        #region Οutput Dictionaries Initialization
        //        OutputData.VPYijResultsDataGrid = new ObservableCollection<VPYijResultsData>();
        //        OutputData.VPYijrzResultsDataGrid = new ObservableCollection<VPYijResultsData>();
        //        OutputData.VPXitResultsDataGrid = new ObservableCollection<VPXijResultsData>();
        //        OutputData.VPLLiResultsDataGrid = new ObservableCollection<VPXiResultData>();
        //        OutputData.EmpLeaveStatusData = new ObservableCollection<EmployeeData>();
        //        #endregion

        //        #region Retrieve Optimization Results


        //        if (OutputData.ObjValue > 0.1)
        //        {

        //            FeasableModel.Solve();
        //            using (FeasableModel)
        //            {
        //                FeasableModel.Solve();

        //                #region Εxtract CPLEX Model,Solution

        //                #region Filepath

        //                string relativePath = Path.Combine("OptimizationResults", "CPLEX", "Thesis", "Vacation_Planning");
        //                Directory.CreateDirectory(relativePath);

        //                #endregion

        //                FeasableModel.ExportModel(Path.Combine(relativePath, "VP.lp"));
        //                FeasableModel.ExportModel(Path.Combine(relativePath, "VP.mps"));
        //                FeasableModel.WriteSolution(Path.Combine(relativePath, "VP.sol"));
        //                //OutputData.ObjValue = FeasableModel.ObjValue;

        //                #endregion


        //                #region Initialize Output Dictionaries,Lists 

        //                List<string> Emp_List = new List<string>();
        //                List<string> Dates_List = new List<string>();

        //                //Dictionary<(Employee Code, Date_String), Xit Value>
        //                Dictionary<(string, string), double> GrantedDays_Dict = new Dictionary<(string, string), double>();

        //                //Dictionary<(Bid Code,j ,r ,z), Xit Value>
        //                Dictionary<(string, int, int, int), double> GrantedBids_Dict = new Dictionary<(string, int, int, int), double>();

        //                #endregion

        //                #region Initialize GrantedDays_Dict Rows
        //                // Extract the optimal solution for the 'X' variables
        //                for (int i = 0; i < I; i++)
        //                {
        //                    for (int t = 0; t < T; t++)
        //                    {
        //                        string employee = Employees_Array[i];
        //                        string date = Dates_Array[t];

        //                        GrantedDays_Dict[(employee, date)] = 0;

        //                        // Add 'employee' and 'date' to the respective lists if they are not already there
        //                        if (!Emp_List.Contains(employee))
        //                            Emp_List.Add(employee);
        //                        if (!Dates_List.Contains(date))
        //                            Dates_List.Add(date);
        //                    }
        //                }
        //                #endregion

        //                #region Extract the optimal solution for the 'Y' variables .Then Fill GrantedBids_Dict
        //                for (int i = 0; i < I; i++)
        //                {
        //                    for (int j = 0; j < JB[i]; j++)
        //                    {
        //                        var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees_Array[i]);
        //                        var Rvalue = RB[(i, j)];

        //                        for (int r = 0; r < Rvalue; r++)
        //                        {
        //                            var Zvalue = ZB[(i, j, r)];

        //                            double rValue = FeasableModel.GetValue(R[i, j, r]);


        //                            for (int z = 0; z < Zvalue; z++)
        //                            {
        //                                string employee = Employees_Array[i];
        //                                int bidIndex = j;

        //                                double yValue = FeasableModel.GetValue(Y[i, j, r, z]);

        //                                // Store the optimal 'Y' value in the data structure
        //                                GrantedBids_Dict[(employee, bidIndex, r, z)] = yValue;
        //                            }
        //                        }

        //                    }
        //                }

        //                #endregion

        //                #region Insert optimal solution for the 'Y' Variables To VPYijResultsDataGrids, VPYirjzResultsDataGrids From GrantedBids_Dict
        //                Console.WriteLine("\nOptimal Solution for Y Variables:");
        //                for (int i = 0; i < I; i++)
        //                {

        //                    //Retrieve Employee
        //                    var EmployeeCode = Emp_List[i];
        //                    var Current_Employee = InputData.Employees.FirstOrDefault(emp => emp.Code == EmployeeCode);

        //                    int TotalGrantedDays = 0;

        //                    for (int j = 0; j < JB[i]; j++)
        //                    {
        //                        var Current_Bid = Current_Employee.LeaveBidDataGridStatic[j];
        //                        for (int r = 0; r < RB[(i, j)]; r++)
        //                        {
        //                            for (int z = 0; z < ZB[(i, j, r)]; z++) //allagh
        //                            {

        //                                #region Populate VPYijResultsDataGrid

        //                                //Retrieve YValue from Dictionary
        //                                double yValue = GrantedBids_Dict[(Current_Employee.Code, j, r, z)];
        //                                string Yij_String = $"Y{(i + 1)}{(j + 1)}";

        //                                var existingYijRecord = OutputData.VPYijResultsDataGrid.FirstOrDefault(record => record.Yij == Yij_String);

        //                                if (existingYijRecord != null)
        //                                {
        //                                    #region Update
        //                                    if (existingYijRecord.YijFlag == 0 && yValue == 1)
        //                                    {

        //                                        existingYijRecord.YijFlag = yValue;
        //                                        OutputData.VPYijResultsDataGrid.Remove(existingYijRecord);
        //                                        OutputData.VPYijResultsDataGrid.Add(existingYijRecord);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    #region Initialize Yij Record

        //                                    VPYijResultsData yijDataRecord = new VPYijResultsData();
        //                                    yijDataRecord.LeaveBidData = new LeaveBidsDataStatic();
        //                                    #endregion

        //                                    #region Refresh NumberOfDays ,NumberOfDaysMin ,NumberOfDaysMax  Based on BidType

        //                                    var NumberOfDays = Current_Bid.NumberOfDays;
        //                                    var NumberOfDaysMax = Current_Bid.NumberOfDaysMax;
        //                                    var NumberOfDaysMin = Current_Bid.NumberOfDaysMin;

        //                                    var BidType = Current_Bid.BidType;
        //                                    if (BidType == BasicEnums.BidType.Min_Max)
        //                                    {
        //                                        NumberOfDays = 0;
        //                                    }
        //                                    else if (BidType == BasicEnums.BidType.Non_Specific || BidType == BasicEnums.BidType.Specific)
        //                                    {
        //                                        NumberOfDaysMax = 0;
        //                                        NumberOfDaysMin = 0;
        //                                    }

        //                                    #endregion

        //                                    #region Insert Values To VPYijResultsDataGrid

        //                                    yijDataRecord.Yij = $"Y{(i + 1)}{(j + 1)}";
        //                                    yijDataRecord.Rijr = $"R{(i + 1)}{(j + 1)}{(r + 1)}";
        //                                    yijDataRecord.Yijrz = $"Y{(i + 1)}{(j + 1)}{(r + 1)}{(z + 1)}";
        //                                    yijDataRecord.YijFlag = yValue;
        //                                    yijDataRecord.Employee = Current_Employee;
        //                                    yijDataRecord.LeaveBidData = Current_Bid;
        //                                    yijDataRecord.DateFrom = Current_Bid.DateFrom;
        //                                    yijDataRecord.DateTo = Current_Bid.DateTo;
        //                                    yijDataRecord.DateFromStr = Current_Bid.DateFrom.ToString("dd/MM/yyyy");
        //                                    yijDataRecord.DateToStr = Current_Bid.DateTo.ToString("dd/MM/yyyy");

        //                                    yijDataRecord.NumberOfDays = NumberOfDays;
        //                                    yijDataRecord.NumberOfDaysMax = NumberOfDaysMax;
        //                                    yijDataRecord.NumberOfDaysMin = NumberOfDaysMin;

        //                                    //Add record to Datagrid
        //                                    OutputData.VPYijResultsDataGrid.Add(yijDataRecord);

        //                                    #endregion

        //                                }

        //                                #endregion

        //                                #endregion

        //                                #region Populate VPYijrzResultsDataGrid

        //                                #region Initialize Yirz Record

        //                                VPYijResultsData yijrzDataRecord = new VPYijResultsData();
        //                                yijrzDataRecord.LeaveBidData = new LeaveBidsDataStatic();
        //                                #endregion

        //                                #region Edit Dates, NumberOfDays based on r,z 
        //                                DateTime DateFrom = Current_Bid.DateFrom.AddDays(z);
        //                                var Yirjz_NumberOfDays = Current_Bid.NumberOfDaysMax - r;
        //                                DateTime DateTo = DateFrom.AddDays(Yirjz_NumberOfDays - 1);
        //                                #endregion

        //                                #region Insert Values To VPYijrzResultsDataGrid

        //                                yijrzDataRecord.Yij = $"Y{(i + 1)}{(j + 1)}";
        //                                yijrzDataRecord.Rijr = $"R{(i + 1)}{(j + 1)}{(r + 1)}";
        //                                yijrzDataRecord.Yijrz = $"Y{(i + 1)}{(j + 1)}{(r + 1)}{(z + 1)}";
        //                                yijrzDataRecord.YijFlag = yValue;
        //                                yijrzDataRecord.Employee = Current_Employee;
        //                                yijrzDataRecord.LeaveBidData = Current_Bid;
        //                                yijrzDataRecord.DateFrom = DateFrom;
        //                                yijrzDataRecord.DateTo = DateTo;
        //                                yijrzDataRecord.NumberOfDays = Yirjz_NumberOfDays;
        //                                yijrzDataRecord.DateFromStr = DateFrom.ToString("dd/MM/yyyy");
        //                                yijrzDataRecord.DateToStr = DateTo.ToString("dd/MM/yyyy");

        //                                //Add record to Datagrid
        //                                OutputData.VPYijrzResultsDataGrid.Add(yijrzDataRecord);
        //                                #endregion

        //                                #region Update TotalGrantedDays
        //                                if (yValue == 1)
        //                                {
        //                                    TotalGrantedDays = TotalGrantedDays + Yirjz_NumberOfDays;
        //                                }
        //                                #endregion

        //                                #endregion

        //                                #region Extract the optimal solution for the 'X' variables .Then Fill GrantedDays_Dict 
        //                                if (yValue == 1)
        //                                {
        //                                    for (DateTime date = DateFrom; date <= DateTo; date = date.AddDays(1))
        //                                    {
        //                                        var SelectedDate_Str = date.ToString("dd/MM/yyyy");

        //                                        GrantedDays_Dict[(Current_Employee.Code, SelectedDate_Str)] = 1;

        //                                    }
        //                                }
        //                                #endregion

        //                            }
        //                        }

        //                    }

        //                    var UpdatedEmp = InputData.Employees.FirstOrDefault(emp => emp.Code == Current_Employee.Code);
        //                    UpdatedEmp.LeaveStatus.ProjectedBalance = UpdatedEmp.LeaveStatus.CurrentBalance - TotalGrantedDays;
        //                    OutputData.EmpLeaveStatusData.Add(UpdatedEmp);


        //                }
        //                #endregion

        //                #region Insert optimal solution for the 'X' Variables To VPXitResultsDataGrid From GrantedDays_Dict
        //                foreach (var employee in Emp_List)
        //                {
        //                    foreach (var date in Dates_List)
        //                    {
        //                        double xValue = GrantedDays_Dict.ContainsKey((employee, date)) ? GrantedDays_Dict[(employee, date)] : 0.0;


        //                        #region Populate VP Xij
        //                        VPXijResultsData singleDataRecord = new VPXijResultsData();


        //                        singleDataRecord.Xit = $"X{(Array.IndexOf(Employees_Array, employee) + 1)}{(Array.IndexOf(Dates_Array, date) + 1)}";
        //                        singleDataRecord.XitFlag = xValue;
        //                        singleDataRecord.Date = date;


        //                        var SpecificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == employee);
        //                        singleDataRecord.Employee = SpecificEmployee;

        //                        OutputData.VPXitResultsDataGrid.Add(singleDataRecord);
        //                        #endregion
        //                    }

        //                }
        //                #endregion




        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }


        //        model.End();


        //        #endregion

        //        return OutputData;

        //    }
        //    catch (ILOG.Concert.Exception ex)
        //    {
        //        Console.WriteLine("A CPLEX error occurred: " + ex.Message);
        //        return OutputData;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Console.WriteLine("A System error occurred: " + ex.Message);
        //        return OutputData;
        //    }
        //}

        //public VacationPlanningOutputData CalculateVacationPlanning_CPLEX1(VacationPlanningInputData InputData)
        //{
        //    VacationPlanningOutputData OutputData = new VacationPlanningOutputData();

        //    try
        //    {
        //        #region Optimization Algorithm

        //        #region Indexes 

        //        int I = InputData.Employees.Count();
        //        int T = InputData.Dates.Count();
        //        int Jmax = InputData.N_Dict.Max(kvp => kvp.Value);
        //        int Rmax = InputData.RBids_Dict.Max(kvp => kvp.Value);
        //        int Zmax = InputData.ZBids_Dict.Max(kvp => kvp.Value);

        //        #endregion

        //        #region Parameters

        //        int Bmax = InputData.Bmax;
        //        int Se = InputData.Se;
        //        Dictionary<int, int> LL = InputData.LLi_Dict;
        //        Dictionary<int, int> E = InputData.MaxD_Dict;
        //        Dictionary<(int, int), int> DF = InputData.DateFrom_Dict;
        //        Dictionary<(int, int), int> DT = InputData.DateTo_Dict;
        //        Dictionary<(int, int), int> NDB = InputData.NDays_Dict;
        //        Dictionary<int, int> JB = InputData.N_Dict;
        //        Dictionary<(int, int), int> RB = InputData.RBids_Dict;
        //        Dictionary<(int, int, int), int> ZB = InputData.ZBids_Dict;

        //        #endregion

        //        #region Cplex Model Initialization

        //        Cplex model = new Cplex();
        //        Cplex FeasableModel = new Cplex();

        //        #endregion

        //        #region Decision Variables 
        //        INumVar[,,,] Y = new INumVar[I, Jmax, Rmax, Zmax];
        //        INumVar[,] X = new INumVar[I, T];
        //        INumVar[,,] R = new INumVar[I, Jmax, Rmax];

        //        // Create decision variables X
        //        for (int i = 0; i < I; i++)               
        //          for (int t = 0; t < T; t++)
        //          {
        //            string varNameX = $"X{i + 1}_{t + 1}";

        //            X[i, t] = model.NumVar(0,1, NumVarType.Bool,varNameX);
        //          }
               

        //        // Create decision variables Y
        //        for (int i = 0; i < I; i++)                
        //            for (int j = 0; j < JB[i]; j++)                   
        //              for (int r = 0; r < RB[(i, j)]; r++)                         
        //                for (int z = 0; z < ZB[(i, j, r)]; z++) 
        //                {
        //                    string varNameY = $"Y{i + 1}_{j + 1}_{r + 1}_{z + 1}";

        //                    Y[i, j, r, z] = model.NumVar(0, 1, NumVarType.Bool, varNameY);
        //                }
                        
                   
                

        //        // Create decision variables R
        //        for (int i = 0; i < I; i++)               
        //          for (int j = 0; j < JB[i]; j++)
        //            for (int r = 0; r < RB[(i, j)]; r++)
        //            {
        //                string varNameR = $"R{i + 1}_{j + 1}_{r + 1}";

        //                R[i, j, r] = model.NumVar(0, 0, NumVarType.Bool, varNameR);
        //            }
        //        #endregion

        //        #region Objective Function

        //        ILinearNumExpr objective = model.LinearNumExpr();

        //        for (int i = I - 1; i >= 0; i--)
        //            for (int j = JB[i] - 1; j >= 0; j--)
        //                for (int r = 0; r < RB[(i, j)]; r++)
        //                {
        //                    objective.AddTerm(1.0, R[i, j, r]);
        //                }

        //        model.AddMaximize(objective);

        //        #endregion

        //        #region Constrains

        //        // #2. Maximum Satisfied Bids Per Employee
        //        for (int i = 0; i < I; i++)
        //        {
        //            ILinearNumExpr sumLeaveBids = model.LinearNumExpr();

        //            for (int j = 0; j < JB[i]; j++)
        //            {
        //                for (int r = 0; r < RB[(i, j)]; r++)
        //                {
        //                    sumLeaveBids.AddTerm(1.0, R[i, j, r]);
        //                }
        //            }

        //            model.AddLe(sumLeaveBids, Bmax, "Con2_" + (i+1));
        //        }


        //        // #3. Entitlements
        //        for (int i = 0; i < I; i++)
        //        {
        //            ILinearNumExpr SumNDB = model.LinearNumExpr();
        //            for (int j = 0; j < JB[i]; j++)
        //            {
        //                var NDays = NDB[(i,j)];

        //                for (int r = 0; r < RB[(i, j)]; r++)
        //                {
        //                    NDays = NDays - r;

        //                    SumNDB.AddTerm(NDays, R[i, j, r]); // Summing up the leave bid days for each employee
        //                }
        //            }
        //            model.AddLe(SumNDB, E[i], "Con3_" + (i + 1));
        //        }


        //        // #4. Limit Lines
        //        for (int t = 0; t < T; t++)
        //        {
        //            ILinearNumExpr expr = model.LinearNumExpr();

        //            int sumdays = 0;

        //            for (int i = 0; i < I; i++)
        //            {
        //                expr.AddTerm(1, X[i, t]);
        //                sumdays++;
        //            }

        //            if (sumdays > LL[t])
        //            {
        //                model.AddLe(expr, LL[t], "Con4_" + (t + 1));
        //            }
        //        }


        //        //#5. Separation or Overlapping
        //        #region  OverLapping

        //        for (int i = 0; i < I; i++) 
        //          for (int j1 = 0; j1 < JB[i] - 1; j1++)                 
        //            for (int j2 = j1 + 1; j2 < JB[i]; j2++)                   
        //              for (int r1 = 0; r1 < RB[(i, j1)]; r1++)                          
        //                for (int r2 = 0; r2 < RB[(i, j2)]; r2++)                               
        //                  for (int z1 = 0; z1 < ZB[(i, j1, r1)]; z1++)                                
        //                    for (int z2 = 0; z2 < ZB[(i, j2, r2)]; z2++)                                      
        //                        if (SeparOrOverlap(i, j1, j2, z1, z2, r1, r2))
        //                        {
        //                            ILinearNumExpr expr = model.LinearNumExpr();
        //                            expr.AddTerm(1, Y[i, j1, r1, z1]);
        //                            expr.AddTerm(1, Y[i, j2, r2, z2]);
        //                            model.AddLe(expr,1,$"Con5_{i + 1}_{j1 + 1}_{z1 + 1}_{j2 + 1}_{z2 + 1}");
        //                        }
                                        
                                   
                                
                            
                        
                    
                

        //        bool SeparOrOverlap(int i, int j1, int j2, int z1, int z2, int r1, int r2)
        //        {
        //            int Bid1_DateFrom = InputData.DateFrom_Dict[(i, j1)];
        //            int Bid2_DateFrom = InputData.DateFrom_Dict[(i, j2)];
        //            int Bid1_NBDays = InputData.NDays_Dict[(i, j1)];
        //            int Bid2_NBDays = InputData.NDays_Dict[(i, j2)];

        //            if (Bid2_DateFrom + z2 >= Bid1_DateFrom + Bid1_NBDays + Se +z1 -r1 )
        //            {
        //                Console.WriteLine("Condition: false");
        //                return false;
        //            };

        //            if (Bid2_DateFrom + Bid2_NBDays + z2 -r1  <= Bid1_DateFrom - Se + z1)
        //            {
        //                Console.WriteLine("Condition: false");
        //                return false;
        //            };

        //            Console.WriteLine("Condition: True");
        //            return true;
        //        }

        //        #endregion
        //        //#6. Connection Between Y and X
        //        for (int i = 0; i < I; i++)               
        //          for (int j = 0; j < JB[i]; j++)
        //          {
        //            int NumberOfDays = NDB[(i, j)];

        //             for (int r = 0; r < RB[(i, j)]; r++)
        //             {
        //                NumberOfDays = NDB[(i, j)] - r;
        //                for (int z = 0; z < ZB[(i, j, r)]; z++)
        //                {
        //                    ILinearNumExpr expr = model.LinearNumExpr();
        //                    expr.AddTerm(NumberOfDays, Y[i, j, r, z]);

        //                    int start = DF[(i, j)] + z;
        //                    int end = start + NumberOfDays;

        //                    for (int t = start; t < end; t++)
        //                    {
        //                        expr.AddTerm(-1, X[i, t]); // Add X variables for each day of the bid
        //                    }
        //                    model.AddLe(expr, 0, $"Con6_{i + 1}_{j + 1}_{r + 1}_{z + 1}");
        //                }
        //             }
        //          }
                    
        //        //#7. Connection Between Y and R 
        //        for (int i = 0; i < I; i++)               
        //          for (int j = 0; j < JB[i]; j++)                    
        //            for (int r = 0; r < RB[(i, j)]; r++)
        //            {
        //                ILinearNumExpr sumYijrz = model.LinearNumExpr();
        //                for (int z = 0; z < ZB[(i, j, r)]; z++)
        //                {
        //                    sumYijrz.AddTerm(1.0, Y[i, j, r, z]);
        //                }

        //                model.AddEq(R[i, j, r], sumYijrz, $"Con7_{i + 1}_{j + 1}_{r + 1}");
        //            }
           
        //        #endregion

        //        #region Check Bids (Strict Seniority, Fair Priority, Bid by Bid)

        //        //Bids Assignment Logic e.x Strict Seniority,Fair Priority , Bid by Bid
        //        BasicEnums.VPLogicType logic = InputData.VPLogicType;

        //        //Total Number of Bids
        //        int N = JB.Values.Sum();

        //        int[] NextBid = new int[N];

        //        //Transfom N_Dict to Array
        //        int[] NrOfBids = JB.Values.ToArray();

        //        //Μετρητες για τα ολοκληρωμένα Bids και Employees
        //        int FinishedBids = 1;
        //        int FinishedEmpIds = 0;

        //        //Support Variables Initialize
        //        bool grant = false;
        //        int EmpId = 0;

        //        while (FinishedEmpIds <= I) 
        //        {
        //            int j = NextBid[EmpId];

        //            #region Select  Rijr

        //            var r = 0;
        //            int Rvalue = new int();
        //            Rvalue = RB[(EmpId, j)];

        //            #endregion
        //            for (r = 0; r < Rvalue; r++)
        //            {
        //                #region Check Bid

        //                R[EmpId, j, r].LB = 1;
        //                R[EmpId, j, r].UB = 1;

        //                model.Solve();
        //                if (model.GetStatus() == Cplex.Status.Optimal)
        //                {
        //                    string message = $"Crew member {EmpId + 1} was awarded bid {j + 1}";
        //                    Console.WriteLine(message);
        //                    grant = true;

        //                    //Update Latest Feasable Model ,ObjectiveValue
        //                    OutputData.ObjValue = model.ObjValue;
        //                    FeasableModel = model;

        //                    //We set Rvalue = 0 in order to break from the R Loop 
        //                    Rvalue = 0;
        //                }
        //                else
        //                {
        //                    grant = false;
        //                    R[EmpId, j, r].LB = 0;
        //                    R[EmpId, j, r].UB = 0;

        //                    string message = $"Crew member {EmpId + 1} was not awarded bid {j + 1}";
        //                    Console.WriteLine(message);
        //                }
        //                #endregion
        //            }

        //            NextBid[EmpId]++;
        //            if (NextBid[EmpId] == NrOfBids[EmpId])
        //            {
        //                FinishedEmpIds++;
        //            }
        //            if (FinishedBids == N)
        //            {
        //                break;
        //            }
        //            FinishedBids = FinishedBids + 1;
        //            if (FinishedBids <= N)
        //            {
        //                EmpId = GetNextId(EmpId, grant, I, NextBid, NrOfBids, FinishedEmpIds, logic);
        //            }
        //        }


        //        #endregion

        //        #endregion

        //        #region Support Arrays

        //        string[] Employees_Array = InputData.Employees.Select(d => d.Code).ToArray();
        //        string[] Dates_Array = InputData.DatesStr;

        //        #endregion

        //        #region Οutput Dictionaries Initialization
        //        OutputData.VPYijResultsDataGrid = new ObservableCollection<VPYijResultsData>();
        //        OutputData.VPYijrzResultsDataGrid = new ObservableCollection<VPYijResultsData>();
        //        OutputData.VPXitResultsDataGrid = new ObservableCollection<VPXijResultsData>();
        //        OutputData.VPLLiResultsDataGrid = new ObservableCollection<VPXiResultData>();
        //        OutputData.EmpLeaveStatusData = new ObservableCollection<EmployeeData>();
        //        #endregion

        //        #region Retrieve Optimization Results


        //        if (OutputData.ObjValue > 0.1)
        //        {

        //            FeasableModel.Solve();
        //            using (FeasableModel)
        //            {
        //                FeasableModel.Solve();

        //                #region Εxtract CPLEX Model,Solution

        //                #region Filepath

        //                string relativePath = Path.Combine("OptimizationResults", "CPLEX", "Thesis", "Vacation_Planning");
        //                Directory.CreateDirectory(relativePath);

        //                #endregion

        //                FeasableModel.ExportModel(Path.Combine(relativePath, "VP.lp"));
        //                FeasableModel.ExportModel(Path.Combine(relativePath, "VP.mps"));
        //                FeasableModel.WriteSolution(Path.Combine(relativePath, "VP.sol"));
        //                OutputData.ObjValue = FeasableModel.ObjValue;

        //                #endregion
 

        //                #region Initialize Output Dictionaries,Lists 

        //                List<string> Emp_List = new List<string>();
        //                List<string> Dates_List = new List<string>();

        //                //Dictionary<(Employee Code, Date_String), Xit Value>
        //                Dictionary<(string, string), double> GrantedDays_Dict = new Dictionary<(string, string), double>();

        //                //Dictionary<(Bid Code,j ,r ,z), Xit Value>
        //                Dictionary<(string, int, int, int), double> GrantedBids_Dict = new Dictionary<(string, int, int, int), double>();

        //                #endregion

        //                #region Initialize GrantedDays_Dict Rows
        //                // Extract the optimal solution for the 'X' variables
        //                for (int i = 0; i < I; i++)
        //                {
        //                    for (int t = 0; t < T; t++)
        //                    {
        //                        string employee = Employees_Array[i];
        //                        string date = Dates_Array[t];

        //                        GrantedDays_Dict[(employee, date)] = 0;

        //                        // Add 'employee' and 'date' to the respective lists if they are not already there
        //                        if (!Emp_List.Contains(employee))
        //                            Emp_List.Add(employee);
        //                        if (!Dates_List.Contains(date))
        //                            Dates_List.Add(date);
        //                    }
        //                }
        //                #endregion

        //                #region Extract the optimal solution for the 'Y' variables .Then Fill GrantedBids_Dict
        //                for (int i = 0; i < I; i++)
        //                {
        //                    for (int j = 0; j < JB[i]; j++)
        //                    {
        //                        var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees_Array[i]);
        //                        var Rvalue = RB[(i, j)];

        //                        for (int r = 0; r < Rvalue; r++)
        //                        {
        //                            var Zvalue = ZB[(i, j, r )];

        //                            double rValue = FeasableModel.GetValue(R[i,j,r]);


        //                            for (int z = 0; z < Zvalue; z++) 
        //                            {
        //                                string employee = Employees_Array[i];
        //                                int bidIndex = j;

        //                                double yValue = FeasableModel.GetValue(Y[i, j, r,z]);

        //                                // Store the optimal 'Y' value in the data structure
        //                                GrantedBids_Dict[(employee, bidIndex, r, z)] = yValue;
        //                            }
        //                        }

        //                    }
        //                }

        //                #endregion

        //                #region Insert optimal solution for the 'Y' Variables To VPYijResultsDataGrids, VPYirjzResultsDataGrids From GrantedBids_Dict
        //                Console.WriteLine("\nOptimal Solution for Y Variables:");
        //                for (int i = 0; i < I; i++)
        //                {

        //                    //Retrieve Employee
        //                    var EmployeeCode = Emp_List[i];
        //                    var Current_Employee = InputData.Employees.FirstOrDefault(emp => emp.Code == EmployeeCode);

        //                    int TotalGrantedDays = 0;

        //                    for (int j = 0; j < JB[i]; j++)
        //                    {
        //                        var Current_Bid = Current_Employee.LeaveBidDataGridStatic[j];
        //                        for (int r = 0; r < RB[(i, j)]; r++)
        //                        {
        //                            for (int z = 0; z < ZB[(i, j, r)]; z++) //allagh
        //                            {

        //                                #region Populate VPYijResultsDataGrid

        //                                //Retrieve YValue from Dictionary
        //                                double yValue = GrantedBids_Dict[(Current_Employee.Code, j, r, z)];
        //                                string Yij_String =  $"Y{(i + 1)}{(j + 1)}";

        //                                var existingYijRecord = OutputData.VPYijResultsDataGrid.FirstOrDefault(record => record.Yij == Yij_String);

        //                                if (existingYijRecord != null)
        //                                {
        //                                    #region Update
        //                                    if (existingYijRecord.YijFlag == 0 && yValue == 1)
        //                                    {

        //                                        existingYijRecord.YijFlag = yValue; 
        //                                        OutputData.VPYijResultsDataGrid.Remove(existingYijRecord); 
        //                                        OutputData.VPYijResultsDataGrid.Add(existingYijRecord);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    #region Initialize Yij Record

        //                                    VPYijResultsData yijDataRecord = new VPYijResultsData();
        //                                    yijDataRecord.LeaveBidData = new LeaveBidsDataStatic();
        //                                    #endregion

        //                                    #region Refresh NumberOfDays ,NumberOfDaysMin ,NumberOfDaysMax  Based on BidType

        //                                    var NumberOfDays = Current_Bid.NumberOfDays;
        //                                    var NumberOfDaysMax = Current_Bid.NumberOfDaysMax;
        //                                    var NumberOfDaysMin = Current_Bid.NumberOfDaysMin;

        //                                    var BidType = Current_Bid.BidType;
        //                                    if (BidType == BasicEnums.BidType.Min_Max)
        //                                    {
        //                                        NumberOfDays = 0;
        //                                    }
        //                                    else if (BidType == BasicEnums.BidType.Non_Specific || BidType == BasicEnums.BidType.Specific)
        //                                    {
        //                                        NumberOfDaysMax = 0;
        //                                        NumberOfDaysMin = 0;
        //                                    }

        //                                    #endregion

        //                                    #region Insert Values To VPYijResultsDataGrid

        //                                    yijDataRecord.Yij = $"Y{(i + 1)}{(j + 1)}";
        //                                    yijDataRecord.Rijr = $"R{(i + 1)}{(j + 1)}{(r + 1)}";
        //                                    yijDataRecord.Yijrz = $"Y{(i + 1)}{(j + 1)}{(r + 1)}{(z + 1)}";
        //                                    yijDataRecord.YijFlag = yValue;
        //                                    yijDataRecord.Employee = Current_Employee;
        //                                    yijDataRecord.LeaveBidData = Current_Bid;
        //                                    yijDataRecord.DateFrom = Current_Bid.DateFrom;
        //                                    yijDataRecord.DateTo = Current_Bid.DateTo;
        //                                    yijDataRecord.DateFromStr = Current_Bid.DateFrom.ToString("dd/MM/yyyy");
        //                                    yijDataRecord.DateToStr = Current_Bid.DateTo.ToString("dd/MM/yyyy");

        //                                    yijDataRecord.NumberOfDays = NumberOfDays;
        //                                    yijDataRecord.NumberOfDaysMax = NumberOfDaysMax;
        //                                    yijDataRecord.NumberOfDaysMin = NumberOfDaysMin;

        //                                    //Add record to Datagrid
        //                                    OutputData.VPYijResultsDataGrid.Add(yijDataRecord);

        //                                    #endregion

        //                                }

        //                                #endregion

        //                                #endregion

        //                                #region Populate VPYijrzResultsDataGrid

        //                                #region Initialize Yirz Record

        //                                VPYijResultsData yijrzDataRecord = new VPYijResultsData();
        //                                yijrzDataRecord.LeaveBidData = new LeaveBidsDataStatic();
        //                                #endregion

        //                                #region Edit Dates, NumberOfDays based on r,z 
        //                                DateTime DateFrom = Current_Bid.DateFrom.AddDays(z);
        //                                var Yirjz_NumberOfDays = Current_Bid.NumberOfDaysMax - r;
        //                                DateTime DateTo = DateFrom.AddDays(Yirjz_NumberOfDays - 1);
        //                                #endregion

        //                                #region Insert Values To VPYijrzResultsDataGrid

        //                                yijrzDataRecord.Yij = $"Y{(i + 1)}{(j + 1)}";
        //                                yijrzDataRecord.Rijr = $"R{(i + 1)}{(j + 1)}{(r + 1)}";
        //                                yijrzDataRecord.Yijrz = $"Y{(i + 1)}{(j + 1)}{(r + 1)}{(z + 1)}";
        //                                yijrzDataRecord.YijFlag = yValue;
        //                                yijrzDataRecord.Employee = Current_Employee;
        //                                yijrzDataRecord.LeaveBidData = Current_Bid;
        //                                yijrzDataRecord.DateFrom = DateFrom;
        //                                yijrzDataRecord.DateTo = DateTo;
        //                                yijrzDataRecord.NumberOfDays = Yirjz_NumberOfDays;
        //                                yijrzDataRecord.DateFromStr = DateFrom.ToString("dd/MM/yyyy");
        //                                yijrzDataRecord.DateToStr = DateTo.ToString("dd/MM/yyyy");

        //                                //Add record to Datagrid
        //                                OutputData.VPYijrzResultsDataGrid.Add(yijrzDataRecord);
        //                                #endregion

        //                                #region Update TotalGrantedDays
        //                                if (yValue == 1)
        //                                {
        //                                    TotalGrantedDays = TotalGrantedDays + Yirjz_NumberOfDays;
        //                                }
        //                                #endregion

        //                                #endregion

        //                                #region Extract the optimal solution for the 'X' variables .Then Fill GrantedDays_Dict 
        //                                if (yValue == 1)
        //                                {
        //                                    for (DateTime date = DateFrom; date <= DateTo; date = date.AddDays(1))
        //                                    {
        //                                        var SelectedDate_Str = date.ToString("dd/MM/yyyy");

        //                                        GrantedDays_Dict[(Current_Employee.Code, SelectedDate_Str)] = 1;

        //                                    }
        //                                }
        //                                #endregion

        //                            }
        //                        } 

        //                    }

        //                    var UpdatedEmp = InputData.Employees.FirstOrDefault(emp => emp.Code == Current_Employee.Code);
        //                    UpdatedEmp.LeaveStatus.ProjectedBalance = UpdatedEmp.LeaveStatus.CurrentBalance - TotalGrantedDays;
        //                    OutputData.EmpLeaveStatusData.Add(UpdatedEmp);


        //                }
        //                #endregion

        //                #region Insert optimal solution for the 'X' Variables To VPXitResultsDataGrid From GrantedDays_Dict
        //                foreach (var employee in Emp_List)
        //                {
        //                    foreach (var date in Dates_List)
        //                    {
        //                        double xValue = GrantedDays_Dict.ContainsKey((employee, date)) ? GrantedDays_Dict[(employee, date)] : 0.0;


        //                        #region Populate VP Xij
        //                        VPXijResultsData singleDataRecord = new VPXijResultsData();


        //                        singleDataRecord.Xit = $"X{(Array.IndexOf(Employees_Array, employee) + 1)}{(Array.IndexOf(Dates_Array, date) + 1)}";
        //                        singleDataRecord.XitFlag = xValue;
        //                        singleDataRecord.Date = date;


        //                        var SpecificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == employee);
        //                        singleDataRecord.Employee = SpecificEmployee;

        //                        OutputData.VPXitResultsDataGrid.Add(singleDataRecord);
        //                        #endregion
        //                    }

        //                }
        //                #endregion




        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }


        //        model.End();


        //        #endregion

        //        return OutputData;

        //    }
        //    catch (ILOG.Concert.Exception ex)
        //    {
        //        Console.WriteLine("A CPLEX error occurred: " + ex.Message);
        //        return OutputData;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Console.WriteLine("A System error occurred: " + ex.Message);
        //        return OutputData;
        //    }
        //}

        //public int GetNextId(int aId, bool accept, int N, int[] NextBid, int[] NrOfBids, 
        //    int FinishedEmpIds, BasicEnums.VPLogicType VPLogicType)
        //{
        //    try
        //    {
        //        int RId = 0;
        //        var logic = VPLogicType;
        //        if (FinishedEmpIds == N)
        //            return 0;

        //        if (logic == BasicEnums.VPLogicType.Strict_Seniority)
        //        {
        //            RId = aId;
        //        }
        //        else if (logic == BasicEnums.VPLogicType.Fair_Assignment)
        //        {
        //            if (accept == true)
        //            {
        //                RId = aId + 1;
        //            }
        //            else
        //            {
        //                RId = aId;
        //            }
        //        }
        //        else if (logic == BasicEnums.VPLogicType.Bid_By_Bid)
        //        {
        //            RId = aId + 1;
        //        }

        //        if (RId == N)
        //        {
        //            RId = 0;
        //        }
        //        while (NextBid[RId] == NrOfBids[RId])
        //        {
        //            RId++;
        //            if (RId == N)
        //            {
        //                RId = 0;
        //            }
        //        }

        //        return RId;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Console.WriteLine("An error occurred: " + ex.Message);
        //        return -1;
        //    }
        //}

        //public VPCGOutputData CalculateVPColumnGeneration_CPLEX(VPCGInputData InputData)
        //{

        //    VPCGOutputData Data = new VPCGOutputData();

        //    try
        //    {

        //        #region Indexes

        //        int I = InputData.I;
        //        int T = InputData.T;
        //        int J = InputData.Ri_Dict.Values.Max(list => list.Count);

        //        #endregion

        //        #region Parameters

        //        //Dictionary<(Employee,List<VacationPlans>), 0 or 1> 
        //        Dictionary<int, List<int>> Ri = InputData.Ri_Dict;

        //        //Dictionary<Employee, Residual Limit Line> 
        //        Dictionary<int, int> RLLt = InputData.RLLt_Dict;

        //        //Dictionary<(Employee,Vacation Plan), Residual Entiltement Cost> 
        //        Dictionary<(int, int), double> Cij = InputData.Cij_Dict;

        //        //Dictionary<(Employee,Vacation Plan Day), 0 or 1> 
        //        Dictionary<(int, int, int), int> Aijt = InputData.Aijt_Dict;

        //        #endregion

        //        #region Model Initialization 

        //        Cplex model = new Cplex();

        //        #endregion

        //        #region Decision Variables

        //        INumVar[,] W = new INumVar[I, J];

        //        for (int i = 0; i < I; i++)
        //        {
        //            List<int> VacationPlans_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

        //            foreach (int j in VacationPlans_List)
        //            {
        //                string varNameX = $"W{i + 1}_{j + 1}";

        //                W[i, j] = model.NumVar(0, 1, NumVarType.Bool, varNameX);
        //            }
        //        }

        //        #endregion

        //        #region Objective Function

        //        ILinearNumExpr objective = model.LinearNumExpr();

        //        for (int i = 0; i < I; i++)
        //        {
        //            List<int> VacationPlans_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

        //            foreach (int j in VacationPlans_List)
        //            {
        //                var Cij_Cost = Cij[(i, j)] * 1000;
        //                objective.AddTerm(Cij_Cost, W[i, j]);
        //            }
        //        }

        //        model.AddMinimize(objective);

        //        #endregion

        //        #region Constraints

        //        //#8. Con 8
        //        for (int i = 0; i < I; i++)
        //        {
        //            ILinearNumExpr expr = model.LinearNumExpr();

        //            List<int> VacationPlans_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();
        //            foreach (int j in VacationPlans_List)
        //            {
        //                expr.AddTerm(1, W[i, j]);

        //            }
        //            model.AddEq(expr, 1, "CON8_" + (i + 1));
        //        }

        //        //#9. Con 9
        //        for (int t = 0; t < T; t++)
        //        {
        //            ILinearNumExpr expr = model.LinearNumExpr();

        //            for (int i = 0; i < I; i++)
        //            {

        //                List<int> VacationPlans_List = Ri.TryGetValue(i, out var productList)
        //                                                      ? productList : new List<int>();
        //                foreach (int j in VacationPlans_List)
        //                {
        //                    expr.AddTerm(Aijt[(i, j, t)], W[i, j]);
        //                }
        //            }
        //            model.Ge(expr, 0, "CON9_" + (t + 1));
        //        }


        //        //#10. Con 10 
        //        for (int t = 0; t < T; t++)
        //        {
        //            ILinearNumExpr expr = model.LinearNumExpr();

        //            for (int i = 0; i < I; i++)
        //            {

        //                List<int> VacationPlans_List = Ri.TryGetValue(i, out var productList) 
        //                                                      ? productList : new List<int>();
        //                foreach (int j in VacationPlans_List)
        //                {
        //                    expr.AddTerm(Aijt[(i, j, t)], W[i, j]);
        //                }
        //            }
        //            model.AddLe(expr, RLLt[t], "CON10_" + (t + 1));
        //        }

        //        #endregion

        //        #region Get Results
        //        model.Solve();
        //        // Solve the model
        //        if (model.GetStatus() == Cplex.Status.Optimal)
        //        {
        //            #region FilePath 

        //            string relativePath = Path.Combine("OptimizationResults", "CPLEX", "Thesis", "VP_Column_Generation");
        //            Directory.CreateDirectory(relativePath);

        //            #endregion

        //            Data.ObjValue = model.ObjValue;

        //            #region Export Model To File   
                    
        //            model.ExportModel(Path.Combine(relativePath, "VP_CG.lp"));
        //            model.ExportModel(Path.Combine(relativePath, "VP_CG.mps"));
        //            model.WriteSolution(Path.Combine(relativePath, "VP_CG.sol"));

        //            #endregion


        //        }
        //        model.End();

        //        #endregion
        //    }
        //    catch (ILOG.Concert.Exception ex)
        //    {
        //        Console.WriteLine("Concert exception: " + ex.Message);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Console.WriteLine("System exception: " + ex.Message);
        //    }
        //    return Data;
        //}

        //#region Extra
        ////public VacationPlanningOutputData CalculateVacationPlanning_CPLEX2_New(VacationPlanningInputData InputData)
        ////{
        ////    // Create the CPLEX environment and model
        ////    Cplex model = new Cplex();
        ////    Cplex FeasableModel = new Cplex();

        ////    string relativePath = Path.Combine("OptimizationResults", "CPLEX", "Thesis", "Vacation_Planning");
        ////    Directory.CreateDirectory(relativePath);

        ////    #region Βοηθητικα Arrays

        ////    string[] Employees_Array = InputData.Employees.Select(d => d.Code).ToArray(); //Πινακας με τους Κωδικους Υπαλληλων
        ////    string[] Dates_Array = InputData.DatesStr; //Πινακας με τα Dates

        ////    #endregion

        ////    #region Optimization paramaters

        ////    int MaxSatisfiedBids = InputData.MaxSatisfiedBids;
        ////    int SeparValue = InputData.SeparValue;

        ////    int I = InputData.Employees.Count();
        ////    int T = InputData.Dates.Count();

        ////    //Μεγιστος Αριθμός Bids όλων των Employees , το χρειαζόμαστε για την δήλωση των Μεταβλητών.
        ////    int Nmax = InputData.N_Dict.Max(kvp => kvp.Value);

        ////    //Μεγιστο Zvalue , το χρειαζόμαστε για την δήλωση της Yijrz
        ////    int Zmax = InputData.ZBids_Dict.Max(kvp => kvp.Value);

        ////    //Μεγιστο Rvalue , το χρειαζόμαστε για την δήλωση της Rijr
        ////    int Rmax = InputData.RBids_Dict.Max(kvp => kvp.Value);

        ////    //Dictionary<Employee, Number of Bids> 
        ////    Dictionary<int, int> LLi_Dict = InputData.LLi_Dict;

        ////    //Dictionary<Employee, Number of Bids> 
        ////    Dictionary<int, int> N_dict = InputData.N_Dict;

        ////    //Dictionary<(Employee, Bid), Date From>
        ////    Dictionary<(int, int), int> DateFrom_Dict = InputData.DateFrom_Dict;

        ////    //Dictionary<(Employee, Bid), Date To>
        ////    Dictionary<(int, int), int> DateTo_Dict = InputData.DateTo_Dict;

        ////    //Dictionary<(Employee, Bid), Number Of Days> . If Bid.BidType = Min_Max then Number Of Days = Max Number Of Days
        ////    Dictionary<(int, int), int> NDays_Dict = InputData.NDays_Dict;

        ////    //RBidsDict = <(Employee Code, LeaveBidCode ),Rvalue>
        ////    Dictionary<(int, int), int> RBidsDict = InputData.RBids_Dict;

        ////    //ZBidsDict = <(Employee Code, LeaveBidCode ,Rvalue), Zvalue>
        ////    Dictionary<(int, int, int), int> ZbidsDict = InputData.ZBids_Dict;
        ////    #endregion

        ////    #region Οutput Dictionaries
        ////    VacationPlanningOutputData OutputData = new VacationPlanningOutputData();
        ////    OutputData.VPYijResultsDataGrid = new ObservableCollection<VPYijResultsData>();
        ////    OutputData.VPYijrzResultsDataGrid = new ObservableCollection<VPYijResultsData>();
        ////    OutputData.VPXitResultsDataGrid = new ObservableCollection<VPXijResultsData>();
        ////    OutputData.VPLLiResultsDataGrid = new ObservableCollection<VPXiResultData>();
        ////    OutputData.EmpLeaveStatusData = new ObservableCollection<EmployeeData>();
        ////    #endregion

        ////    try
        ////    {
        ////        #region Optimization

        ////        #region Optimization Algorithm

        ////        #region Decision Variables 
        ////        INumVar[,,,] Y = new INumVar[I, Nmax, Rmax, Zmax];
        ////        INumVar[,] X = new INumVar[I, T];
        ////        INumVar[,,] R = new INumVar[I, Nmax, Rmax];

        ////        // Create decision variables X
        ////        for (int i = 0; i < I; i++)
        ////        {
        ////            for (int t = 0; t < T; t++)
        ////            {
        ////                // Define the variable name
        ////                string varNameX = $"X{i + 1}_{t + 1}";

        ////                // Create the binary variable with a name
        ////                X[i, t] = model.NumVar(0, 1, NumVarType.Bool, varNameX);
        ////            }
        ////        }

        ////        // Create decision variables Y
        ////        for (int i = 0; i < I; i++)
        ////        {
        ////            for (int j = 0; j < N_dict[i]; j++)
        ////            {
        ////                for (int r = 0; r < RBidsDict[(i, j)]; r++) //allagh
        ////                {
        ////                    for (int z = 0; z < ZbidsDict[(i, j, r)]; z++) //allagh
        ////                    {
        ////                        // Define the variable name
        ////                        string varNameY = $"Y{i + 1}_{j + 1}_{r + 1}_{z + 1}";

        ////                        // Create the binary variable with a name
        ////                        Y[i, j, r, z] = model.NumVar(0, 1, NumVarType.Bool, varNameY);
        ////                    }
        ////                }
        ////            }
        ////        }

        ////        // Create decision variables R
        ////        for (int i = 0; i < I; i++)
        ////        {
        ////            for (int j = 0; j < N_dict[i]; j++)
        ////            {
        ////                for (int r = 0; r < RBidsDict[(i, j)]; r++)
        ////                {
        ////                    // Define the variable name
        ////                    string varNameR = $"R{i + 1}_{j + 1}_{r + 1}";

        ////                    // Create the binary variable with a name
        ////                    R[i, j, r] = model.NumVar(0, 0, NumVarType.Bool, varNameR);
        ////                }
        ////            }
        ////        }

        ////        var a = 1;

        ////        #endregion

        ////        #region Objective Function

        ////        ILinearNumExpr objective = model.LinearNumExpr();

        ////        for (int i = I - 1; i >= 0; i--)
        ////        {
        ////            for (int j = N_dict[i] - 1; j >= 0; j--)
        ////            {
        ////                for (int r = 0; r < RBidsDict[(i, j)]; r++) //allagh
        ////                {
        ////                    objective.AddTerm(1.0, R[i, j, r]);
        ////                }
        ////            }
        ////            //for (int t = 0; t < T; t++)
        ////            //{
        ////            //    objective.AddTerm(-0.00000001, X[i, t]);
        ////            //}
        ////        }

        ////        model.AddMaximize(objective);

        ////        #endregion

        ////        #region Constrains
        ////        // #1. Adding constraints for maximum number of satisfied bids 
        ////        for (int i = 0; i < I; i++)
        ////        {
        ////            ILinearNumExpr sumLeaveBids = model.LinearNumExpr();

        ////            for (int j = 0; j < N_dict[i]; j++)
        ////            {

        ////                for (int r = 0; r < RBidsDict[(i, j)]; r++)
        ////                {
        ////                    sumLeaveBids.AddTerm(1.0, R[i, j, r]);
        ////                }
        ////            }

        ////            // Adding the constraint for the current employee
        ////            model.AddLe(sumLeaveBids, MaxSatisfiedBids, "MaxSatisfiedBids_" + Employees_Array[i]);
        ////        }


        ////        // #2. Entitlements
        ////        for (int i = 0; i < I; i++)
        ////        {

        ////            ILinearNumExpr sumLeaveBidDays = model.LinearNumExpr();
        ////            for (int j = 0; j < N_dict[i]; j++)
        ////            {
        ////                var NumberOfDays = NDays_Dict[(i, j)];

        ////                for (int r = 0; r < RBidsDict[(i, j)]; r++)
        ////                {
        ////                    NumberOfDays = NumberOfDays - r;

        ////                    for (int z = 0; z < ZbidsDict[(i, j, r)]; z++)
        ////                    {
        ////                        sumLeaveBidDays.AddTerm(NumberOfDays, Y[i, j, r, z]); // Summing up the leave bid days for each employee
        ////                    }
        ////                }
        ////            }
        ////            var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees_Array[i]);

        ////            var MaxLeaveDays = specificEmployee.LeaveStatus.CurrentBalance;

        ////            // Adding the constraint for the current employee
        ////            model.AddLe(sumLeaveBidDays, MaxLeaveDays, "MaxLeaveDays_" + Employees_Array[i]);
        ////        }


        ////        // #3. Limit Lines
        ////        for (int t = 0; t < T; t++)
        ////        {
        ////            ILinearNumExpr expr = model.LinearNumExpr();

        ////            int sumdays = 0;

        ////            for (int i = 0; i < I; i++)
        ////            {
        ////                expr.AddTerm(1, X[i, t]);
        ////                sumdays++;
        ////            }

        ////            if (sumdays > LLi_Dict[t])
        ////            {
        ////                model.AddLe(expr, LLi_Dict[t], "LimitLine_" + Dates_Array[t]);
        ////            }
        ////        }


        ////        //#4. Overlapping
        ////        #region  OverLapping

        ////        for (int i = 0; i < I; i++)
        ////        {
        ////            for (int j1 = 0; j1 < N_dict[i] - 1; j1++)
        ////            {
        ////                for (int j2 = j1 + 1; j2 < N_dict[i]; j2++)
        ////                {
        ////                    for (int r1 = 0; r1 < RBidsDict[(i, j1)]; r1++)
        ////                    {
        ////                        for (int r2 = 0; r2 < RBidsDict[(i, j2)]; r2++)
        ////                        {
        ////                            for (int z1 = 0; z1 < ZbidsDict[(i, j1, r1)]; z1++)
        ////                            {
        ////                                for (int z2 = 0; z2 < ZbidsDict[(i, j2, r2)]; z2++)
        ////                                {
        ////                                    if (SeparOrOverlap(i, j1, j2, z1, z2, r1, r2))
        ////                                    {
        ////                                        ILinearNumExpr expr = model.LinearNumExpr();
        ////                                        expr.AddTerm(1, Y[i, j1, r1, z1]);
        ////                                        expr.AddTerm(1, Y[i, j2, r2, z2]);
        ////                                        model.AddLe(expr, 1, $"SO{i + 1}_{j1 + 1}_{z1 + 1}_{j2 + 1}_{z2 + 1}");
        ////                                    }
        ////                                }
        ////                            }
        ////                        }
        ////                    }
        ////                }
        ////            }
        ////        }

        ////        bool SeparOrOverlap(int i, int j1, int j2, int z1, int z2, int r1, int r2)
        ////        {


        ////            int Bid1_DateFrom = InputData.DateFrom_Dict[(i, j1)];
        ////            int Bid2_DateFrom = InputData.DateFrom_Dict[(i, j2)];

        ////            if (Bid2_DateFrom + z2 >= Bid1_DateFrom + InputData.NDays_Dict[(i, j1)] + SeparValue + z1 - r1 - 1)
        ////            {
        ////                Console.WriteLine("Condition: false");
        ////                return false;

        ////            };

        ////            if (Bid2_DateFrom + InputData.NDays_Dict[(i, j2)] + z2 - r1 - 1 <= Bid1_DateFrom - SeparValue + z1)
        ////            {
        ////                Console.WriteLine("Condition: false");
        ////                return false;

        ////            };

        ////            Console.WriteLine("Condition: True");
        ////            return true;
        ////        }

        ////        #endregion
        ////        //#5. Connection Between Y and X
        ////        for (int i = 0; i < I; i++)
        ////        {
        ////            for (int j = 0; j < N_dict[i]; j++)
        ////            {
        ////                int NumberOfDays = NDays_Dict[(i, j)];

        ////                for (int r = 0; r < RBidsDict[(i, j)]; r++)
        ////                {
        ////                    NumberOfDays = NumberOfDays - r;

        ////                    for (int z = 0; z < ZbidsDict[(i, j, r)]; z++)
        ////                    {
        ////                        ILinearNumExpr expr = model.LinearNumExpr();
        ////                        expr.AddTerm(NumberOfDays, Y[i, j, r, z]);

        ////                        int start = DateFrom_Dict[(i, j)] + z;
        ////                        int end = start + NumberOfDays;

        ////                        for (int t = start; t < end; t++)
        ////                        {
        ////                            expr.AddTerm(-1, X[i, t]); // Add X variables for each day of the bid
        ////                        }
        ////                        // Add the constraint
        ////                        model.AddLe(expr, 0, $"BidDaysConstraint_{i + 1}_{j + 1}_{r + 1}_{z + 1}");

        ////                    }
        ////                }
        ////            }
        ////        }


        ////        //#6. Connection Between Y and R -- Yijrz and Yijr
        ////        for (int i = 0; i < I; i++)
        ////        {
        ////            for (int j = 0; j < N_dict[i]; j++)
        ////            {

        ////                for (int r = 0; r < RBidsDict[(i, j)]; r++)
        ////                {
        ////                    ILinearNumExpr sumYijrz = model.LinearNumExpr();
        ////                    for (int z = 0; z < ZbidsDict[(i, j, r)]; z++)
        ////                    {
        ////                        sumYijrz.AddTerm(1.0, Y[i, j, r, z]);
        ////                    }
        ////                    // Adding the constraint for the current employee
        ////                    model.AddEq(R[i, j, r], sumYijrz, "Y_R_Connection" + Employees_Array[i]);
        ////                }
        ////            }
        ////        }


        ////        #endregion

        ////        #endregion

        ////        #region New Optimize settings

        ////        bool grant = false;
        ////        // Λογικη Ανάθεσης π.χ Strict Seniority
        ////        BasicEnums.VPLogicType logic = InputData.VPLogicType;

        ////        //Το αντιστοιχο FinishedIds στο κωδικα της Python
        ////        int FinishedEmpIds = 0;
        ////        int BidId = 0;

        ////        //Μετρητής για τα ολοκληρωμέναBids
        ////        int FinishedBids = 1;

        ////        var numRowsPerEmployee = InputData.Employees.Select(e => e.LeaveBidDataGridStatic.Count);
        ////        var numOfEmployes = InputData.Employees.Count; //Το αντιστοιχο N της python

        ////        int N = numRowsPerEmployee.Sum(); //Το N εδω ειναι o αριθμος των συνολικών Bids.
        ////        int[] NextBid = new int[N];
        ////        int[] NrOfBids = N_dict.Values.ToArray();
        ////        List<string> outputLines = new List<string>();


        ////        //model.Update();

        ////        while (FinishedEmpIds <= I)
        ////        {

        ////            int j = NextBid[BidId];

        ////            #region Select  Rijr
        ////            var r = 0;
        ////            int Rvalue = new int();

        ////            Rvalue = RBidsDict[(BidId, j)];

        ////            #endregion
        ////            for (r = 0; r < Rvalue; r++)
        ////            {
        ////                #region Check Bid

        ////                R[BidId, j, r].LB = 1;
        ////                R[BidId, j, r].UB = 1;

        ////                model.Solve();
        ////                bool solution = (model.GetStatus() == Cplex.Status.Optimal);
        ////                if (solution)
        ////                {
        ////                    double rValue = model.GetValue(R[BidId, j, r]);

        ////                    grant = true;
        ////                    string message = $"Crew member {BidId + 1} was awarded bid {j + 1}";
        ////                    Console.WriteLine(message);
        ////                    outputLines.Add(message);
        ////                    OutputData.ObjValue = model.ObjValue;
        ////                    Rvalue = 0;
        ////                }
        ////                else
        ////                {
        ////                    grant = false;
        ////                    R[BidId, j, r].LB = 0;
        ////                    R[BidId, j, r].UB = 0;

        ////                    string message = $"Crew member {BidId + 1} was not awarded bid {j + 1}";
        ////                    Console.WriteLine(message);
        ////                    outputLines.Add(message);
        ////                }

        ////                #endregion
        ////            }

        ////            NextBid[BidId]++;
        ////            if (NextBid[BidId] == NrOfBids[BidId])
        ////            {
        ////                FinishedEmpIds++;

        ////            }
        ////            if (model.GetStatus() == Cplex.Status.Optimal)
        ////            {
        ////                OutputData.ObjValue = model.ObjValue;


        ////                FeasableModel = model;

        ////            }
        ////            if (FinishedBids == N)
        ////            {
        ////                break;
        ////            }
        ////            FinishedBids = FinishedBids + 1;
        ////            if (FinishedBids <= N)
        ////            {
        ////                BidId = GetNextId(BidId, grant, numOfEmployes, NextBid, NrOfBids, FinishedEmpIds, logic);
        ////            }
        ////        }


        ////        #endregion

        ////        #endregion

        ////        #region Save,Show Results


        ////        if (OutputData.ObjValue > 0.1)
        ////        {
        ////            MessageBox.Show($"Optimization Completed for Vacation Planning with Code : {InputData.VPCode}", "", MessageBoxButton.OK, MessageBoxImage.Information);

        ////            FeasableModel.Solve();
        ////            using (FeasableModel)
        ////            {
        ////                FeasableModel.Solve();

        ////                #region Print Results

        ////                FeasableModel.ExportModel(Path.Combine(relativePath, "VP.lp"));
        ////                FeasableModel.ExportModel(Path.Combine(relativePath, "VP.mps"));
        ////                FeasableModel.WriteSolution(Path.Combine(relativePath, "VP.sol"));

        ////                #endregion

        ////                OutputData.ObjValue = FeasableModel.ObjValue;
        ////                //Data.ObjValue = Math.Round(Data.ObjValue);

        ////                #region Βοηθητικά Dictionaries για εξαγωγή της Βέλτιστης Λύσης

        ////                List<string> rows = new List<string>();
        ////                List<string> columns = new List<string>();
        ////                Dictionary<(string, string), double> GrantedBids_Dict = new Dictionary<(string, string), double>();
        ////                Dictionary<(string, int, int, int), double> y_plan = new Dictionary<(string, int, int, int), double>();

        ////                #endregion

        ////                #region Extract the optimal solution for the 'X' variables
        ////                // Extract the optimal solution for the 'X' variables
        ////                for (int i = 0; i < I; i++)
        ////                {
        ////                    for (int t = 0; t < T; t++)
        ////                    {
        ////                        string employee = Employees_Array[i];
        ////                        string date = Dates_Array[t];

        ////                        double xValue = FeasableModel.GetValue(X[i, t]);
        ////                        if (xValue == 1)
        ////                        {
        ////                            Console.WriteLine($"Employee: {employee}, Date: {date}, Value: {xValue}");
        ////                        }
        ////                        // Store the optimal 'X' value in the data structure
        ////                        GrantedBids_Dict[(employee, date)] = xValue;

        ////                        // Add 'employee' and 'date' to the respective lists if they are not already there
        ////                        if (!rows.Contains(employee))
        ////                            rows.Add(employee);
        ////                        if (!columns.Contains(date))
        ////                            columns.Add(date);
        ////                    }
        ////                }
        ////                #endregion
        ////                #region Insert optimal solution for the 'X' Variables to Model Class
        ////                Console.WriteLine("Optimal Solution for X Variables:");
        ////                foreach (var employee in rows)
        ////                {
        ////                    foreach (var date in columns)
        ////                    {
        ////                        double xValue = GrantedBids_Dict.ContainsKey((employee, date)) ? GrantedBids_Dict[(employee, date)] : 0.0;


        ////                        #region Populate VP Xij
        ////                        VPXijResultsData singleDataRecord = new VPXijResultsData();


        ////                        singleDataRecord.Xij = $"X{(Array.IndexOf(Employees_Array, employee) + 1)}{(Array.IndexOf(Dates_Array, date) + 1)}";
        ////                        singleDataRecord.XijFlag = xValue;
        ////                        singleDataRecord.Date = date;


        ////                        var SpecificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == employee);
        ////                        singleDataRecord.Employee = SpecificEmployee;

        ////                        OutputData.VPXitResultsDataGrid.Add(singleDataRecord);
        ////                        #endregion
        ////                    }

        ////                }
        ////                #endregion

        ////                #region Extract the optimal solution for the 'Y' variables
        ////                for (int i = 0; i < I; i++)
        ////                {
        ////                    for (int j = 0; j < N_dict[i]; j++)
        ////                    {
        ////                        var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees_Array[i]);
        ////                        var Rvalue = RBidsDict[(i, j)];

        ////                        for (int r = 0; r < Rvalue; r++)
        ////                        {
        ////                            var Zvalue = ZbidsDict[(i, j, r)];

        ////                            double rValue = FeasableModel.GetValue(R[i, j, r]);


        ////                            for (int z = 0; z < Zvalue; z++)
        ////                            {
        ////                                string employee = Employees_Array[i];
        ////                                int bidIndex = j;

        ////                                double yValue = FeasableModel.GetValue(Y[i, j, r, z]);

        ////                                // Store the optimal 'Y' value in the data structure
        ////                                y_plan[(employee, bidIndex, r, z)] = yValue;
        ////                            }
        ////                        }

        ////                    }
        ////                }

        ////                #endregion
        ////                #region Insert optimal solution for the 'Y' Variables to Model Class
        ////                Console.WriteLine("\nOptimal Solution for Y Variables:");
        ////                int EmpCounter = 0;
        ////                foreach (var employee in rows)
        ////                {
        ////                    var TotalNumberOfDays = 0;

        ////                    for (int j = 0; j < N_dict[EmpCounter]; j++)
        ////                    {
        ////                        #region Retrieve Data

        ////                        var EmployeeCode = employee;
        ////                        var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == EmployeeCode);
        ////                        var BidCode = specificEmployee.LeaveBidDataGridStatic[j].BidCode;

        ////                        #endregion

        ////                        for (int r = 0; r < RBidsDict[(EmpCounter, j)]; r++)
        ////                        {
        ////                            for (int z = 0; z < ZbidsDict[(EmpCounter, j, r)]; z++) //allagh
        ////                            {
        ////                                int bidIndex = j;
        ////                                double yValue = y_plan.ContainsKey((employee, bidIndex, r, z)) ? y_plan[(employee, bidIndex, r, z)] : 0.0;


        ////                                #region Populate VP Yij
        ////                                VPYijResultsData yijDataRecord = new VPYijResultsData();
        ////                                yijDataRecord.LeaveBidData = new LeaveBidsDataStatic();


        ////                                yijDataRecord.Yij = $"Y{(Array.IndexOf(Employees_Array, employee) + 1)}{(bidIndex + 1)}";
        ////                                yijDataRecord.Rijr = $"R{(Array.IndexOf(Employees_Array, employee) + 1)}{(bidIndex + 1)}{(r + 1)}";
        ////                                yijDataRecord.Yijrz = $"Y{(Array.IndexOf(Employees_Array, employee) + 1)}{(bidIndex + 1)}{(r + 1)}{(z + 1)}";

        ////                                yijDataRecord.YijFlag = yValue;

        ////                                var SpecificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == employee);
        ////                                yijDataRecord.Employee = SpecificEmployee;

        ////                                yijDataRecord.LeaveBidData = SpecificEmployee.LeaveBidDataGridStatic[j];

        ////                                #region Edit Dates

        ////                                var DateFrom = SpecificEmployee.LeaveBidDataGridStatic[j].DateFrom;
        ////                                var NumberOfDays = SpecificEmployee.LeaveBidDataGridStatic[j].NumberOfDaysMax - r;
        ////                                var NumberOfDaysMax = SpecificEmployee.LeaveBidDataGridStatic[j].NumberOfDaysMax;
        ////                                var NumberOfDaysMin = SpecificEmployee.LeaveBidDataGridStatic[j].NumberOfDaysMin;

        ////                                var BidType = SpecificEmployee.LeaveBidDataGridStatic[j].BidType;
        ////                                var DateTo = SpecificEmployee.LeaveBidDataGridStatic[j].DateTo;

        ////                                yijDataRecord.DateFrom = DateFrom;
        ////                                yijDataRecord.DateTo = DateTo;

        ////                                if (BidType == BasicEnums.BidType.Min_Max)
        ////                                {
        ////                                    NumberOfDays = 0;
        ////                                }
        ////                                else if (BidType == BasicEnums.BidType.Non_Specific)
        ////                                {
        ////                                    NumberOfDaysMax = 0;
        ////                                    NumberOfDaysMin = 0;

        ////                                }
        ////                                else if (BidType == BasicEnums.BidType.Specific)
        ////                                {
        ////                                    NumberOfDaysMax = 0;
        ////                                    NumberOfDaysMin = 0;
        ////                                }

        ////                                yijDataRecord.NumberOfDays = NumberOfDays;
        ////                                yijDataRecord.NumberOfDaysMax = NumberOfDaysMax;
        ////                                yijDataRecord.NumberOfDaysMin = NumberOfDaysMin;

        ////                                yijDataRecord.DateFromStr = DateFrom.ToString("dd/MM/yyyy");
        ////                                yijDataRecord.DateToStr = DateTo.ToString("dd/MM/yyyy");

        ////                                #endregion

        ////                                #region ADD RECORD 
        ////                                var existingRecord = OutputData.VPYijResultsDataGrid.FirstOrDefault(record => record.Yij == yijDataRecord.Yij);

        ////                                if (existingRecord != null)
        ////                                {
        ////                                    if (existingRecord.YijFlag == 1)
        ////                                    {

        ////                                    }
        ////                                    else if (existingRecord.YijFlag == 0 && yijDataRecord.YijFlag == 0)
        ////                                    {

        ////                                    }
        ////                                    else if (existingRecord.YijFlag == 0 && yijDataRecord.YijFlag == 1)
        ////                                    {
        ////                                        // Insert the new record and remove the existing record
        ////                                        OutputData.VPYijResultsDataGrid.Remove(existingRecord);
        ////                                        OutputData.VPYijResultsDataGrid.Add(yijDataRecord);
        ////                                    }
        ////                                }
        ////                                else
        ////                                {
        ////                                    OutputData.VPYijResultsDataGrid.Add(yijDataRecord);

        ////                                }

        ////                                #endregion



        ////                                #endregion

        ////                                #region Populate VP Yijz
        ////                                VPYijResultsData yijzDataRecord = new VPYijResultsData();
        ////                                yijzDataRecord.LeaveBidData = new LeaveBidsDataStatic();


        ////                                yijzDataRecord.Yij = $"Y{(Array.IndexOf(Employees_Array, employee) + 1)}{(bidIndex + 1)}";
        ////                                yijzDataRecord.Rijr = $"R{(Array.IndexOf(Employees_Array, employee) + 1)}{(bidIndex + 1)}{(r + 1)}";
        ////                                yijzDataRecord.Yijrz = $"Y{(Array.IndexOf(Employees_Array, employee) + 1)}{(bidIndex + 1)}{(r + 1)}{(z + 1)}";

        ////                                yijzDataRecord.YijFlag = yValue;

        ////                                yijzDataRecord.Employee = SpecificEmployee;

        ////                                yijzDataRecord.LeaveBidData = SpecificEmployee.LeaveBidDataGridStatic[j];

        ////                                #region Edit Dates


        ////                                DateFrom = SpecificEmployee.LeaveBidDataGridStatic[j].DateFrom.AddDays(z);
        ////                                NumberOfDays = SpecificEmployee.LeaveBidDataGridStatic[j].NumberOfDaysMax - r;
        ////                                DateTo = DateFrom.AddDays(NumberOfDays - 1);

        ////                                yijzDataRecord.DateFrom = DateFrom;
        ////                                yijzDataRecord.DateTo = DateTo;
        ////                                yijzDataRecord.NumberOfDays = NumberOfDays;
        ////                                yijzDataRecord.DateFromStr = DateFrom.ToString("dd/MM/yyyy");
        ////                                yijzDataRecord.DateToStr = DateTo.ToString("dd/MM/yyyy");

        ////                                #endregion
        ////                                OutputData.VPYijrzResultsDataGrid.Add(yijzDataRecord);



        ////                                #endregion
        ////                                if (yValue == 1)
        ////                                {
        ////                                    TotalNumberOfDays = TotalNumberOfDays + NumberOfDays;

        ////                                }
        ////                            }
        ////                        }

        ////                    }

        ////                    var UpdatedEmp = InputData.Employees.FirstOrDefault(emp => emp.Code == employee);
        ////                    UpdatedEmp.LeaveStatus.ProjectedBalance = UpdatedEmp.LeaveStatus.CurrentBalance - TotalNumberOfDays;
        ////                    OutputData.EmpLeaveStatusData.Add(UpdatedEmp);

        ////                    EmpCounter++;

        ////                }
        ////                #endregion

        ////            }
        ////        }
        ////        else
        ////        {
        ////            MessageBox.Show("Error during data processing", "", MessageBoxButton.OK, MessageBoxImage.Error);
        ////        }


        ////        model.End();


        ////        #endregion

        ////        return OutputData;


        ////    }
        ////    catch (ILOG.Concert.Exception ex)
        ////    {
        ////        Console.WriteLine("A CPLEX error occurred: " + ex.Message);
        ////        return OutputData;
        ////    }
        ////    catch (System.Exception ex)
        ////    {
        ////        Console.WriteLine("A System error occurred: " + ex.Message);
        ////        return OutputData;
        ////    }
        ////}

        //public bool SaveVpVijResultData(VacationPlanningOutputData Data, int ReplicationNumber, int VPID)
        //{
        //    try
        //    {
        //        using (var dbContext = new ErpDbContext(options))
        //        {

        //            foreach (var row in Data.VPYijrzResultsDataGrid)
        //            {
        //                VPYijzResultsDataEntity newRow = new VPYijzResultsDataEntity();

        //                var BidId = dbContext.LeaveBids.SingleOrDefault(b => b.BidCode == row.LeaveBidData.BidCode).BidId;
        //                newRow.BidId = BidId;
        //                newRow.EmpId = row.Employee.EmployeeId;
        //                newRow.VPID = VPID;
        //                newRow.Yij = row.Yij;
        //                newRow.Yijr = row.Rijr;
        //                newRow.Yijrz = row.Yijrz;

        //                newRow.DateFromStr = row.DateFromStr;
        //                newRow.DateToStr = row.DateToStr;
        //                newRow.NumberOfDays = row.NumberOfDays;
        //                newRow.NumberOfDaysMin = row.NumberOfDaysMin;
        //                newRow.NumberOfDaysMax = row.NumberOfDaysMax;
        //                newRow.ReplicationNumber = ReplicationNumber;

        //                bool Confirmed = new bool();

        //                if (row.YijFlag == 0)
        //                {
        //                    Confirmed = false;
        //                }
        //                else
        //                {
        //                    Confirmed = true;
        //                }
        //                newRow.Confirmed = Confirmed;


        //                dbContext.VPYijzResults.Add(newRow);

        //            }

        //            dbContext.SaveChanges();
        //            return true;
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        LogError(ex, "SaveVpVijResultData", "Notes");
        //        return false;
        //    }
        //}

        //public int CreatePythonTxt(VacationPlanningInputData InputData)
        //{

        //    string[] Employees = InputData.Employees.Select(d => d.Code).ToArray();

        //    int MaxLeaveBids = InputData.MaxLeaveBids;
        //    int MaxSatisfiedBids = InputData.Bmax;

        //    string[] Dates = InputData.DatesStr;
        //    int SeparValue = InputData.Se;
        //    int LimitLineFixed = InputData.Schedule.LimitLineFixed;
        //    int numOfEmployes = InputData.Employees.Count;

        //    int[] entitlementstxt = new int[numOfEmployes];
        //    int[] NumberOfBidstxt = new int[numOfEmployes];

        //    int[][] dateStarttxt = new int[numOfEmployes][];
        //    int[][] dateLengthtxt = new int[numOfEmployes][];

        //    Dictionary<string, int> MaxLeaveBidsPerEmployee = InputData.MaxLeaveBidsPerEmployee;
        //    int[] NrOfBids = MaxLeaveBidsPerEmployee.Values.ToArray();
        //    NumberOfBidstxt = NrOfBids;

        //    #region Python Insert Entiltements,NubmerOfBids
        //    var i = 0;
        //    foreach (var emp in InputData.Employees)
        //    {
        //        entitlementstxt[i] = emp.LeaveStatus.CurrentBalance;
        //        NumberOfBidstxt[i] = emp.LeaveBidDataGridStatic.Count; //ALAGH

        //        dateStarttxt[i] = new int[MaxLeaveBids];
        //        dateLengthtxt[i] = new int[MaxLeaveBids];
        //        for (int j = 0; j < MaxLeaveBidsPerEmployee[emp.Code]; j++)
        //        {

        //            var DateFrom = emp.LeaveBidDataGridStatic[j].DateFrom.ToString("dd/MM/yyyy");
        //            var DateFromIndex = Dates.IndexOf(DateFrom);

        //            dateStarttxt[i][j] = DateFromIndex + 1;
        //            dateLengthtxt[i][j] = emp.LeaveBidDataGridStatic[j].NumberOfDays;
        //        }


        //        i++;
        //    }



        //    #endregion



        //    return 1;
        //}

        //#endregion

        //#endregion

        //#region Crew Rostering
        //public CSOutputData CalculateCrewScheduling_SetPartition_CPLEX(CSInputData InputData)
        //{
        //    #region FilePath Initialization
        //    string relativePath = Path.Combine("OptimizationResults", "CPLEX", "Thesis", "CS_Set_Partition");
        //    Directory.CreateDirectory(relativePath);
        //    CSOutputData Data = new CSOutputData();

        //    #endregion

        //    // Save the files

        //    try
        //    {
        //        #region Indexes

        //        int I = InputData.I; 
        //        int F = InputData.F; 
        //        int RiMax = InputData.Ri.Values.Max(list => list.Count);

        //        #endregion

        //        #region Parameters

        //        var h = InputData.RoutesPenalty;
        //        var c = InputData.BoundsPenalty;
        //        Dictionary<int, int> Bf = InputData.RoutesCompl_Dict;
        //        Dictionary<(int, int), double> Cij_Hours = InputData.Cij_Hours;
        //        Dictionary<(int, int, int), int> Aijf = InputData.Aijf;
        //        Dictionary<int, List<int>> Ri = InputData.Ri;

        //        #endregion
                    
        //        //Model Initialization
        //        Cplex model = new Cplex();

        //        #region Decision Variables

        //        INumVar[,] X = new INumVar[I ,RiMax];
        //        INumVar[] Y = new INumVar[F];

        //        for (int i = 0; i < I; i++)
        //        {
        //            List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

        //            foreach (int j in RosterPerEmployee_List)
        //            {
        //                string varNameX = $"X{i + 1}_{j + 1}";

        //                X[i, j] = model.NumVar(0, 1, NumVarType.Bool, varNameX);
        //            }
        //        }
        //        for (int f = 0; f < F; f++)
        //        {
        //            string varNameY = $"Y{f + 1}";

        //            Y[f] = model.NumVar(0, 1, NumVarType.Bool, varNameY);

        //        }
        //        #endregion

        //        #region Objective Function

        //        ILinearNumExpr objective = model.LinearNumExpr();

        //        for (int i = 0; i < I; i++)
        //        {
        //            List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

        //            foreach (int j in RosterPerEmployee_List)
        //            {
        //                var Cij_Cost = Cij_Hours[(i, j)] * c;
        //                objective.AddTerm(Cij_Cost, X[i, j]);
        //            }
        //        }
        //        for (int f = 0; f < F; f++)
        //        {
        //            objective.AddTerm(h, Y[f]);
        //        }

        //        model.AddMinimize(objective);

        //        #endregion

        //        #region Constraints

        //        // #13. Con13
        //        for (int i = 0; i < I; i++)
        //        {
        //           ILinearNumExpr expr = model.LinearNumExpr();
        //           List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();
        //           foreach (int j in RosterPerEmployee_List)
        //           {
        //              expr.AddTerm(1, X[i,j]);

        //           }
        //           model.AddEq(expr, 1, "CON2_" + (i + 1));
        //        }

        //        // #14. Con14 
        //        for (int f = 0; f < F; f++)
        //        {
        //           ILinearNumExpr expr = model.LinearNumExpr();
        //           expr.AddTerm(1, Y[f]);
        //           for (int i = 0; i < I; i++)
        //           {
        //              List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

        //              foreach (int j in RosterPerEmployee_List)
        //              {
        //                expr.AddTerm(Aijf[(i, j, f)], X[i, j]);
        //              }
        //           }
        //           model.AddEq(expr, Bf[f], "CON3_" + (f + 1));
        //        }

        //        #endregion

        //        #region Get Results

        //        // Solve the model
        //        model.Solve();
        //        bool solution = (model.GetStatus() == Cplex.Status.Optimal);
        //        if (solution)
        //        {
        //            Data.ObjValue = model.ObjValue;

        //            // Export Model
        //            model.ExportModel(Path.Combine(relativePath, "CS_Set_Partition.lp"));
        //            model.ExportModel(Path.Combine(relativePath, "CS_Set_Partition.mps"));
        //            model.WriteSolution(Path.Combine(relativePath, "CS_Set_Partition.sol"));
        //        }

        //        model.End();

        //        #endregion
        //    }
        //    catch (ILOG.Concert.Exception ex)
        //    {
        //        Console.WriteLine("A CPLEX error occurred: " + ex.Message);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Console.WriteLine("A System error occurred: " + ex.Message);
        //    }

        //    return Data;
        //}

        //public CSOutputData CalculateCrewScheduling_SetCover_CPLEX(CSInputData InputData)
        //{
        //    #region Model,Data,FilePath Initialization

        //    Cplex model = new Cplex();
        //    string relativePath = Path.Combine("OptimizationResults", "CPLEX", "Thesis", "CS_Set_Cover");
        //    Directory.CreateDirectory(relativePath);
        //    CSOutputData Data = new CSOutputData();

        //    #endregion

        //    // Save the files

        //    try
        //    {
        //        // Initialize CPLEX environment and model

        //        #region Optimization paramaters

        //        #region Indexes
        //        int T = InputData.T; // Planning Horizon
        //        int I = InputData.I; // Number Of Employees Empty Schedules
        //        int F = InputData.F; // Number Of Routes
        //        #endregion

        //        #region Dictionaries
        //        Dictionary<int, List<int>> Ri = InputData.Ri;
        //        Dictionary<(int, int), double> Cij_Hours = InputData.Cij_Hours;
        //        Dictionary<(int, int, int), int> Aijf = InputData.Aijf;

        //        Dictionary<int, int> Bf = InputData.RoutesCompl_Dict;
        //        //We need RiMax for the declaration of Model Variables
        //        int RiMax = InputData.Ri.Values.Max(list => list.Count);

        //        #endregion

        //        #region Penalty Cost

        //        var h = InputData.RoutesPenalty;
        //        var c = InputData.BoundsPenalty;

        //        #endregion

        //        #endregion

        //        #region Decision Variables
        //        // Decision variables

        //        INumVar[,] X = new INumVar[I, RiMax];
        //        INumVar[] Y = new INumVar[F];

        //        for (int i = 0; i < I; i++)
        //        {
        //           List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) 
        //                                                    ? productList : new List<int>();

        //           foreach (int j in RosterPerEmployee_List)
        //           {
        //              string varNameX = $"X{i + 1}_{j + 1}";
                      
        //                X[i, j] = model.NumVar(0, 1, NumVarType.Bool, varNameX);
        //           }
        //        }
        //        for (int f = 0; f < F; f++)
        //        {
        //           string varNameY = $"Y{f + 1}";

        //           Y[f] = model.NumVar(0, 1, NumVarType.Bool, varNameY);

        //        }
        //        #endregion

        //        #region Objective Function

        //        ILinearNumExpr objective = model.LinearNumExpr();

        //        for (int f = 0; f < F; f++)
        //        {
        //            objective.AddTerm(h, Y[f]);
        //        }

        //        model.AddMinimize(objective);

        //        #endregion

        //        #region Constraints

        //        // #1. C1 -> C40 , X174-> X213 ROUTES 
        //        for (int i = 0; i < I; i++)
        //        {

        //            ILinearNumExpr expr = model.LinearNumExpr();
        //            List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) 
        //                                                     ? productList : new List<int>();
        //            foreach (int j in RosterPerEmployee_List)
        //            {
        //                expr.AddTerm(1, X[i, j]);

        //            }
        //            model.AddEq(expr, 1, "CON2_" + (i + 1));
        //        }

        //        // #2. C41 -> C213 , X1-> X173 ROUTES 
        //        for (int f = 0; f < F; f++)
        //        {
        //            ILinearNumExpr expr = model.LinearNumExpr();
        //            expr.AddTerm(1, Y[f]);
        //            for (int i = 0; i < I; i++)
        //            {
        //                List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) 
        //                                                         ? productList : new List<int>();

        //                foreach (int j in RosterPerEmployee_List)
        //                {
        //                    expr.AddTerm(Aijf[(i, j, f)], X[i, j]);
        //                }
        //            }
        //            model.AddGe(expr, Bf[f], "CON3_" + (f + 1));
        //        }

        //        #endregion

        //            #region Get Results

        //        // Solve the model
        //        model.Solve();
        //        bool solution = (model.GetStatus() == Cplex.Status.Optimal);
        //        if (solution)
        //        {
        //           Data.ObjValue = model.ObjValue;
        //           //Export Model
        //           model.ExportModel(Path.Combine(relativePath, "CS_SetCover.lp"));
        //           model.ExportModel(Path.Combine(relativePath, "CS_SetCover.mps"));
        //           model.WriteSolution(Path.Combine(relativePath, "CS_SetCover.sol"));
        //        }

        //        #endregion

        //        // Dispose of CPLEX objects
        //        model.End();
        //    }
        //    catch (ILOG.Concert.Exception ex)
        //    {
        //        Console.WriteLine("A CPLEX error occurred: " + ex.Message);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Console.WriteLine("A System error occurred: " + ex.Message);
        //    }

        //    return Data;
        //}


        //#endregion
    }
}
