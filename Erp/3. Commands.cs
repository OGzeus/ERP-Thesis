using Erp.Model.Enums;
using Erp.Model.Thesis.VacationPlanning;
using Erp.Repositories;
using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Linq;
using Syncfusion.UI.Xaml.Grid;
using System.Windows.Data;
using System.ComponentModel;
using Erp.Model.Thesis;
using Erp.DataBase;
using Erp.DataBase.Τhesis;

namespace Erp.V_Proxeiro
{
    public class Commands : RepositoryBase
    {
        #region CRUD Commands

        #region Get Commands

public VacationPlanningInputData GetVPInputChooserData(VacationPlanningInputData Data)
{
    string FilterStr = "";
    using (var connection = GetConnection())
    using (var command = new SqlCommand())
    {
        connection.Open();
        command.Connection = connection;
        if (Data.VPId > 0)
        {
            command.Parameters.AddWithValue("@VPID", Data.VPId);
            FilterStr += " and V.VPID = @VPID";
        }
        else if (!string.IsNullOrWhiteSpace(Data.VPCode))
        {
            command.Parameters.AddWithValue("@VPCODE", Data.VPCode);
            FilterStr += " and V.VPCODE = @VPCODE";
        }
        command.CommandText = string.Format(@"SELECT V.VPID, V.VPCODE, V.VPDESCR,
                                                V.EMPLOYEETYPE, V.VPLOGICTYPE, 
                                                V.MaxSatisfiedBids, V.ISDELETED,
                                                V.SeparValue,
                                                R.ID, R.ReqCode,
                                                    R.ReqDescr, R.DateFrom, R.DateTo
                                             FROM VPInput AS V
                                             INNER JOIN ReqScheduleInfo AS R ON R.ID = V.SCHEDULEID
                                             WHERE 1=1 {0}", FilterStr);
        using (var reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                #region Populate Data

                #region Populate Vacation Planning Data

                Data.VPId = int.Parse(reader["VPID"].ToString());
                Data.VPCode = reader["VPCODE"].ToString();
                Data.VPDescr = reader["VPDESCR"].ToString();
                Data.EmployeeType = (BasicEnums.EmployeeType)Enum.Parse(typeof(BasicEnums.EmployeeType),
                                                                    reader["EMPLOYEETYPE"].ToString());
                Data.VPLogicType = (BasicEnums.VPLogicType)Enum.Parse(typeof(BasicEnums.VPLogicType),
                                                                   reader["VPLogicType"].ToString());
                Data.Bmax = int.Parse(reader["MaxSatisfiedBids"].ToString());
                Data.Se = int.Parse(reader["SeparValue"].ToString());
                Data.IsDeleted = bool.Parse(reader["ISDELETED"].ToString());

                #endregion
                #region Populate Schedule Data

                Data.Schedule = new ReqScheduleInfoData();

                Data.Schedule.ID = int.Parse(reader["ID"].ToString());
                Data.Schedule.ReqCode = reader["ReqCode"].ToString();
                Data.Schedule.ReqDescr = reader["ReqDescr"].ToString();
                Data.Schedule.DateFrom = DateTime.Parse(reader["DateFrom"].ToString());
                Data.Schedule.DateTo = DateTime.Parse(reader["DateTo"].ToString());
                Data.Schedule.DateFromStr = Data.Schedule.DateFrom.ToString(); //THELEI ALAGH
                Data.Schedule.DateToStr = Data.Schedule.DateTo.ToString();

                #endregion

                #endregion
            }
        }
        connection.Close();
    }
    return Data;
}
public ObservableCollection<VacationPlanningInputData> GetVPInputData(bool ShowDeleted)
{
    ObservableCollection<VacationPlanningInputData> DataGrid= new ObservableCollection<VacationPlanningInputData>();
    string FilterStr = "";

    using (var connection = GetConnection())
    using (var command = new SqlCommand())
    {
        connection.Open();
        command.Connection = connection;

        if (ShowDeleted == false)
        {
            command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
            FilterStr = String.Format(@"and V.IsDeleted = @ShowDeleted");
        }
        string SQLQuery = @"SELECT V.VPID,V.VPCODE,V.VPDESCR,V.EMPLOYEETYPE,V.VPLOGICTYPE,V.SeparValue,
                                V.MaxSatisfiedBids,V.ISDELETED,
                                R.ID,R.ReqCode,R.ReqDescr,R.DateFrom,R.DateTo,R.LimitLineFixed
                                FROM VPInput as V
                                INNER JOIN ReqScheduleInfo AS R ON R.ID = V.SCHEDULEID
                                Where 1=1 {0}";

        command.CommandText = string.Format(SQLQuery, FilterStr);

        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                #region Initialize  Row
                VacationPlanningInputData data = new VacationPlanningInputData();
                data.Schedule = new ReqScheduleInfoData();

                #endregion

                #region Populate Data  Row

                #region Populate VP
                data.VPId = int.Parse(reader["VPID"].ToString());
                data.VPCode = reader["VPCODE"].ToString();
                data.VPDescr = reader["VPDESCR"].ToString();
                data.Bmax = int.Parse(reader["MaxSatisfiedBids"].ToString());
                data.Se = int.Parse(reader["SeparValue"].ToString());
                data.EmployeeType = (BasicEnums.EmployeeType)Enum.Parse(typeof(BasicEnums.EmployeeType), 
                                     reader["EMPLOYEETYPE"].ToString());
                data.VPLogicType = (BasicEnums.VPLogicType)Enum.Parse(typeof(BasicEnums.VPLogicType),
                                    reader["VPLogicType"].ToString());
                data.IsDeleted = bool.Parse(reader["ISDELETED"].ToString());

                #endregion

                #region Populate Schedule

                data.Schedule.ID = int.Parse(reader["ID"].ToString());
                data.Schedule.ReqCode = reader["ReqCode"].ToString();
                data.Schedule.ReqDescr = reader["ReqDescr"].ToString();
                data.Schedule.DateFrom = DateTime.Parse(reader["DateFrom"].ToString());
                data.Schedule.DateTo = DateTime.Parse(reader["DateTo"].ToString());
                data.Schedule.LimitLineFixed = int.Parse(reader["LimitLineFixed"].ToString());
                data.Schedule.DateFromStr = data.Schedule.DateFrom.ToString("dd/MM/yyyy");
                data.Schedule.DateToStr = data.Schedule.DateTo.ToString("dd/MM/yyyy");

                #endregion

                #endregion

                //Αdd Row to ObservableCollection
                DataGrid.Add(data);
            }
        }

        connection.Close();
    }
    return DataGrid;
}
      
        #endregion

        #region Save,Add Commands
        public int SaveVPInputData(VacationPlanningInputData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    int vpId = flatData.VPId;
                    var existingQuery = dbContext.VPInput.Where(c => c.VPID == vpId);
                    var existing = existingQuery.SingleOrDefault();

                    if (existing != null)
                    {
                        #region Update VP Entity from Model Data

                        existing.VPCODE = flatData.VPCode;
                        existing.VPDESCR = flatData.VPDescr;
                        existing.MaxSatisfiedBids = flatData.Bmax;
                        existing.SeparValue = flatData.Se;
                        existing.EMPLOYEETYPE = flatData.EmployeeType.ToString();
                        existing.VPLOGICTYPE = flatData.VPLogicType.ToString();
                        existing.IsDeleted = flatData.IsDeleted;
                        #endregion

                        #region Update Schedule Foreign Key

                        existing.SCHEDULEID = flatData.Schedule.ID;
                        #endregion

                        dbContext.SaveChanges();
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "SaveVPInputData", "Notes");
                return -1;
            }
        }
        public int AddVPInputData(VacationPlanningInputData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    var existingVPQuery = dbContext.VPInput.Where(r => r.VPCODE == flatData.VPCode);
                    var existingVP = existingVPQuery.SingleOrDefault();

                    if (existingVP == null)
                    {
                        #region Initialize Entities

                        var newVP = new VPInputDataEntity();
                        var MainSchedule = dbContext.ReqScheduleInfo.FirstOrDefault(r => r.MainSchedule == true);
                        #endregion

                        #region Insert VP Model Data to Entity

                        newVP.VPCODE = flatData.VPCode;
                        newVP.VPDESCR = flatData.VPDescr;
                        newVP.EMPLOYEETYPE = BasicEnums.EmployeeType.Captain.ToString();
                        newVP.VPLOGICTYPE = BasicEnums.VPLogicType.Strict_Seniority.ToString();
                        newVP.MaxSatisfiedBids = 2;
                        newVP.SeparValue = 2;
                        newVP.IsDeleted = false;

                        //Schedule Foreign Key
                        newVP.SCHEDULEID = MainSchedule.ID;

                        #endregion



                        #region Save Changes

                        dbContext.VPInput.Add(newVP);
                        dbContext.SaveChanges();
                        #endregion

                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "AddVPInputData", "Notes");
                return 2;
            }
        }

        #endregion

        #endregion

        #region Data_Grid Commands
        public class F7Data 
        {
            public string F7key { get; set; }
            public Columns SfGridColumns { get; set; }
            public ICollectionView CollectionView { get; set; }

        }
        public F7Data F7VacationPlanning(bool ShowDeleted)
        {
            #region Initialize Data

            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            #endregion

            #region Get Data

            var Data = GetVPInputData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);
           
            #endregion

            #region Set Columns of Syncfusion.DataGrid

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "VPCode", HeaderText = "VP Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "VPDescr", HeaderText = "VP Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "EmployeeType", HeaderText = "Employee Type" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "VPLogicType", HeaderText = "VP Logic Type" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "MaxSatisfiedBids", HeaderText = "Max Satisfied Bids" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "SeparValue", HeaderText = "Separation Value" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Schedule.ReqCode", HeaderText = "Schedule Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Schedule.ReqDescr", HeaderText = "Schedule Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Schedule.DateFromStr", HeaderText = "Date From" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Schedule.DateToStr", HeaderText = "Date To" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });
            #endregion

            #region Return Data to ViewModel

            GridData.F7key = "VPCode";
            return GridData;
            #endregion
        }
        #endregion
    }
}
