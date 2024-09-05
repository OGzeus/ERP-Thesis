using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Erp.Model;
using Erp.Repositories;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using Erp.Model.BasicFiles;
using Erp.DataBase;
using Erp.Model.Enums;
using Gurobi;
using Deedle;
using System.Data;
using Erp.Model.Thesis;
using Erp.DataBase.Τhesis;
using Erp.Model.Thesis.VacationPlanning;
using Microsoft.EntityFrameworkCore.Internal;
using Erp.View.Thesis.CustomButtons;
using Erp.Model.Thesis.CrewScheduling;


namespace Erp.CommonFiles
{

    public class CommonFunctions : RepositoryBase
    {

        #region Thesis

        #region Vacation Planning File 

        #region Employee

        #region 1stTab General Info
        public ObservableCollection<EmployeeData> GetEmployeesByTypeData(BasicEnums.EmployeeType employeeType, bool ShowDeleted)
        {
            ObservableCollection<EmployeeData> DataList = new ObservableCollection<EmployeeData>();

            string FilterStr = "";

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = string.Concat(FilterStr, " AND E.IsDeleted = @ShowDeleted");
                }
                command.Parameters.AddWithValue("@Position", employeeType.ToString());
                FilterStr = string.Concat(FilterStr, " AND E.Position = @Position");

                command.CommandText = string.Format(@"SELECT E.EmployeeID, E.Code, E.Descr, E.LowerBound, E.UpperBound,  
E.Position,E.Seniority,E.IsDeleted
FROM Employees AS E
WHERE 1=1 {0}
ORDER BY E.Seniority", FilterStr);


                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        EmployeeData data = new EmployeeData();
                        data.EmpCrSettings = new EmployeeCR_Settings();
                        data.Certification = new CertificationData();
                        data.BaseAirport = new AirportData();
                        data.BaseAirport.City = new CityData();


                        data.EmployeeId = int.Parse(reader["EmployeeID"].ToString());
                        data.Code = reader["Code"].ToString();
                        data.Descr = reader["Descr"].ToString();
                        data.Position = (BasicEnums.EmployeeType)Enum.Parse(typeof(BasicEnums.EmployeeType), reader["Position"].ToString());
                        data.Seniority = int.Parse(reader["Seniority"].ToString());
                        data.EmpCrSettings.LowerBound = int.Parse(reader["LowerBound"].ToString());
                        data.EmpCrSettings.UpperBound = int.Parse(reader["UpperBound"].ToString());

                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());



                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }
        public ObservableCollection<EmployeeData> GetEmployeesByTypeData_CS(BasicEnums.EmployeeType employeeType, bool ShowDeleted)
        {
            ObservableCollection<EmployeeData> DataList = new ObservableCollection<EmployeeData>();

            string FilterStr = "";

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = string.Concat(FilterStr, " AND E.IsDeleted = @ShowDeleted");
                }
                command.Parameters.AddWithValue("@Position", employeeType.ToString());
                FilterStr = string.Concat(FilterStr, " AND E.Position = @Position");

                command.CommandText = string.Format(@"SELECT E.EmployeeID, E.Code, E.Descr, E.LowerBound, E.UpperBound,  
E.Position,E.Seniority,E.IsDeleted
,A.AirportID,A.AirportCode,A.AirportDescr
FROM Employees AS E
INNER JOIN Airports AS A ON E.BaseAirportId = A.AirportID
WHERE 1=1 {0}
ORDER BY E.Seniority", FilterStr);


                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        EmployeeData data = new EmployeeData();
                        data.EmpCrSettings = new EmployeeCR_Settings();
                        data.Certification = new CertificationData();
                        data.BaseAirport = new AirportData();
                        data.BaseAirport.City = new CityData();


                        data.EmployeeId = int.Parse(reader["EmployeeID"].ToString());
                        data.Code = reader["Code"].ToString();
                        data.Descr = reader["Descr"].ToString();
                        data.Position = (BasicEnums.EmployeeType)Enum.Parse(typeof(BasicEnums.EmployeeType), reader["Position"].ToString());
                        data.Seniority = int.Parse(reader["Seniority"].ToString());
                        data.EmpCrSettings.LowerBound = int.Parse(reader["LowerBound"].ToString());
                        data.EmpCrSettings.UpperBound = int.Parse(reader["UpperBound"].ToString());
                        data.BaseAirport.Code  = reader["AirportCode"].ToString();
                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());



                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }


public int SaveEmployeeData(EmployeeData flatData)
{
    try
    {
        using (var dbContext = new ErpDbContext(options))
        {
            // Separate query from execution
            int employeeId = flatData.EmployeeId;
            var existingQuery = dbContext.Employees.Where(c => c.EmployeeID == employeeId);
            var existing = existingQuery.SingleOrDefault();;
            if (existing != null)
            {
                // Update existing customer
                existing.Code = flatData.Code;
                existing.Descr = flatData.Descr;
                existing.FirstName = flatData.FirstName;
                existing.LastName = flatData.LastName;
                existing.Gender = flatData.Gender.ToString();
                existing.ContactNumber = flatData.ContactNumber;
                existing.Email = flatData.Email;
                existing.Address = flatData.Address;
                existing.Position = flatData.Position.ToString();
                existing.TotalFlightHours = flatData.TotalFlightHours;
                existing.Seniority = flatData.Seniority;
                existing.Language = flatData.Language; ;
                existing.DateOfBirth = DateTime.Now;
                existing.HireDate = DateTime.Now;
                existing.LowerBound = flatData.EmpCrSettings.LowerBound;
                existing.UpperBound = flatData.EmpCrSettings.UpperBound;
                existing.IsDeleted = flatData.IsDeleted;

                existing.BaseAirportId = flatData.BaseAirport.Id;


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
        LogError(ex, "SaveEmployeeData", "Notes");
        return -1;
    }
}

public int AddEmployeeData(EmployeeData flatData)
{
    try
    {
        using (var dbContext = new ErpDbContext(options))
        {
            // Separate query from execution
            var existingItemQuery = dbContext.Employees.Where(r => r.Code == flatData.Code);
            var existingItem = existingItemQuery.SingleOrDefault();
            // Execute the query and get the result

            if (existingItem == null)
            {
                var newItem = new EmployeeDataEntity();
                // Insert new item
                newItem.Code = flatData.Code;
                newItem.Descr = flatData.Descr;
                newItem.FirstName = " ";
                newItem.LastName = " ";
                newItem.Gender = BasicEnums.Gender.Male.ToString();
                newItem.ContactNumber = " ";
                newItem.Email = " ";
                newItem.Address = " ";
                newItem.Position = BasicEnums.EmployeeType.Captain.ToString();
                newItem.TotalFlightHours = 0;
                newItem.Seniority = 0;
                newItem.Language = 0;
                newItem.LowerBound = 70;
                newItem.UpperBound = 80;
                newItem.DateOfBirth = DateTime.Now;
                newItem.HireDate = DateTime.Now;
                newItem.IsDeleted = false;
                newItem.BaseAirportId = dbContext.Airports.FirstOrDefault().AirportID;
                newItem.CertificationID = dbContext.Certifications.FirstOrDefault().CertID;
                dbContext.Employees.Add(newItem);
                dbContext.SaveChanges();
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
        LogError(ex, "AddEmployeeData", "Notes");
        return 2;

    }
}

        public EmployeeData GetEmployeeChooserData(int Id, string Code)
        {
            EmployeeData FlatData = new EmployeeData();
            string FilterStr = "";
            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;

                    if (Id > 0)
                    {
                        command.Parameters.AddWithValue("@ID", Id);
                        FilterStr = String.Format(@" and Employees.EmployeeID =@ID");

                    }

                    else if (!string.IsNullOrWhiteSpace(Code))
                    {
                        command.Parameters.AddWithValue("@Code", Code);
                        FilterStr = String.Format(@" and Employees.Code =@Code");

                    }
                    command.CommandText = string.Format(@"SELECT Employees.EmployeeID, Employees.Code, Employees.Descr, 
Employees.FirstName,Employees.LastName, Employees.Gender, Employees.DateOfBirth, Employees.LowerBound, Employees.UpperBound, 
Employees.ContactNumber, Employees.Email,Employees.Address,Employees.Position,Employees.CertificationID,Employees.HireDate,Employees.TotalFlightHours,
Employees.Seniority,Employees.Language,Employees.BaseAirportId,Employees.IsDeleted,
C.CertID,C.Code as CCODE,C.Descr CDESCR,C.ValidityPeriod,C.ValidityTimeBucket,C.DateFrom,
C.DateTo,C.CertPosition,A.AirportID,A.AirportCode,A.AirportDescr,A.CityId
FROM Employees
INNER JOIN Certifications as C ON Employees.CertificationID = C.CertID
INNER JOIN Airports AS A ON Employees.BaseAirportId = A.AirportID
                                              Where 1=1 {0}", FilterStr);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            EmployeeData data = new EmployeeData();
                            data.Certification = new CertificationData();
                            data.BaseAirport = new AirportData();
                            data.BaseAirport.City = new CityData();
                            data.EmpCrSettings = new EmployeeCR_Settings();

                            data.EmployeeId = int.Parse(reader["EmployeeID"].ToString());
                            data.Code = reader["Code"].ToString();
                            data.Descr = reader["Descr"].ToString();
                            data.FirstName = reader["FirstName"].ToString();
                            data.LastName = reader["LastName"].ToString();
                            data.Gender = (BasicEnums.Gender)Enum.Parse(typeof(BasicEnums.Gender), reader["Gender"].ToString());
                            data.DateOfBirth = DateTime.Parse(reader["DateOfBirth"].ToString());
                            data.HireDate = DateTime.Parse(reader["HireDate"].ToString());
                            data.ContactNumber = reader["ContactNumber"].ToString();
                            data.Email = reader["Email"].ToString();
                            data.Address = reader["Address"].ToString();

                            data.Position = (BasicEnums.EmployeeType)Enum.Parse(typeof(BasicEnums.EmployeeType), reader["Position"].ToString());
                            data.TotalFlightHours = int.Parse(reader["TotalFlightHours"].ToString());
                            data.Seniority = int.Parse(reader["Seniority"].ToString());
                            data.Language = int.Parse(reader["Language"].ToString());

                            data.EmpCrSettings.LowerBound = int.Parse(reader["LowerBound"].ToString());
                            data.EmpCrSettings.UpperBound = int.Parse(reader["UpperBound"].ToString());

                            data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());


                            data.Certification.Id = int.Parse(reader["CertId"].ToString());
                            data.Certification.Code = reader["CCODE"].ToString();
                            data.Certification.Descr = reader["CDESCR"].ToString();
                            data.Certification.ValidityPeriod = int.Parse(reader["ValidityPeriod"].ToString());
                            data.Certification.CertPosition = (BasicEnums.CertPosition)Enum.Parse(typeof(BasicEnums.CertPosition), reader["CertPosition"].ToString());
                            data.Certification.ValidityTimeBucket = (BasicEnums.Timebucket)Enum.Parse(typeof(BasicEnums.Timebucket), reader["ValidityTimeBucket"].ToString());
                            data.Certification.DateFrom = DateTime.Parse(reader["DateFrom"].ToString());
                            data.Certification.DateTo = DateTime.Parse(reader["DateTo"].ToString());

                            data.BaseAirport.Id = int.Parse(reader["AirportID"].ToString());
                            data.BaseAirport.Code = reader["AirportCode"].ToString();
                            data.BaseAirport.Descr = reader["AirportDescr"].ToString();
                            data.BaseAirport.City.CityId = int.Parse(reader["CityId"].ToString());

                            FlatData = data;
                        }
                    }

                    connection.Close();
                }


                return FlatData;
            }
            catch (Exception ex)
            {
                LogError(ex, "GetEmployeeChooserData", "Notes");
                return null;
            }
        }
        public ObservableCollection<EmployeeData> GetEmployeeData(bool ShowDeleted)
        {
            ObservableCollection<EmployeeData> DataList = new ObservableCollection<EmployeeData>();

            string FilterStr = "";

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = String.Format(@" and Employees.IsDeleted =@ShowDeleted");
                }
                command.CommandText = string.Format(@"SELECT Employees.EmployeeID, Employees.Code, Employees.Descr, 
Employees.FirstName,Employees.LastName, Employees.Gender, Employees.DateOfBirth, Employees.LowerBound, Employees.UpperBound, 
Employees.ContactNumber, Employees.Email,Employees.Address,Employees.Position,Employees.CertificationID,Employees.HireDate,Employees.TotalFlightHours,
Employees.Seniority,Employees.Language,Employees.BaseAirportId,Employees.IsDeleted,
C.CertID,C.Code as CCODE,C.Descr CDESCR,C.ValidityPeriod,C.ValidityTimeBucket,C.DateFrom,
C.DateTo,C.CertPosition,A.AirportID,A.AirportCode,A.AirportDescr,A.CityId
FROM Employees
INNER JOIN Certifications as C ON Employees.CertificationID = C.CertID
INNER JOIN Airports AS A ON Employees.BaseAirportId = A.AirportID
                                              Where 1=1 {0}", FilterStr);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        EmployeeData data = new EmployeeData();
                        data.Certification = new CertificationData();
                        data.BaseAirport = new AirportData();
                        data.BaseAirport.City = new CityData();
                        data.EmpCrSettings = new EmployeeCR_Settings();


                        data.EmployeeId = int.Parse(reader["EmployeeID"].ToString());
                        data.Code = reader["Code"].ToString();
                        data.Descr = reader["Descr"].ToString();
                        data.FirstName = reader["FirstName"].ToString();
                        data.LastName = reader["LastName"].ToString();
                        data.Gender = (BasicEnums.Gender)Enum.Parse(typeof(BasicEnums.Gender), reader["Gender"].ToString());
                        data.DateOfBirth = DateTime.Parse(reader["DateOfBirth"].ToString());
                        data.HireDate = DateTime.Parse(reader["HireDate"].ToString());
                        data.ContactNumber = reader["ContactNumber"].ToString();
                        data.Email = reader["Email"].ToString();
                        data.Address = reader["Address"].ToString();

                        data.Position = (BasicEnums.EmployeeType)Enum.Parse(typeof(BasicEnums.EmployeeType), reader["Position"].ToString());
                        data.TotalFlightHours = int.Parse(reader["TotalFlightHours"].ToString());
                        data.Seniority = int.Parse(reader["Seniority"].ToString());
                        data.Language = int.Parse(reader["Language"].ToString());

                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());


                        data.EmpCrSettings.LowerBound = int.Parse(reader["LowerBound"].ToString());
                        data.EmpCrSettings.UpperBound = int.Parse(reader["UpperBound"].ToString());

                        data.Certification.Id = int.Parse(reader["CertId"].ToString());
                        data.Certification.Code = reader["CCODE"].ToString();
                        data.Certification.Descr = reader["CDESCR"].ToString();
                        data.Certification.ValidityPeriod = int.Parse(reader["ValidityPeriod"].ToString());
                        data.Certification.CertPosition = (BasicEnums.CertPosition)Enum.Parse(typeof(BasicEnums.CertPosition), reader["CertPosition"].ToString());
                        data.Certification.ValidityTimeBucket = (BasicEnums.Timebucket)Enum.Parse(typeof(BasicEnums.Timebucket), reader["ValidityTimeBucket"].ToString());
                        data.Certification.DateFrom = DateTime.Parse(reader["DateFrom"].ToString());
                        data.Certification.DateTo = DateTime.Parse(reader["DateTo"].ToString());

                        data.BaseAirport.Id = int.Parse(reader["AirportID"].ToString());
                        data.BaseAirport.Code = reader["AirportCode"].ToString();
                        data.BaseAirport.Descr = reader["AirportDescr"].ToString();
                        data.BaseAirport.City.CityId = int.Parse(reader["CityId"].ToString());




                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }

        #endregion

        #region 2ndTab LeaveBids

        public ObservableCollection<LeaveBidsDataStatic> GetLeaveBids(string EmployeeCode, string ScheduleCode)
        {
            ObservableCollection<LeaveBidsDataStatic> DataList = new ObservableCollection<LeaveBidsDataStatic>();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.Parameters.AddWithValue("@EmpCode", EmployeeCode);
                command.Parameters.AddWithValue("@ScheduleCode", ScheduleCode);


                command.CommandText = string.Format(@"SELECT L.BidId, L.BidCode, L.PriorityLevel, L.BidType, L.DateFrom, L.DateTo, L.DateFromStr, L.DateToStr,
    L.NumberOfDays, L.NumberOfDaysMin, L.NumberOfDaysMax, L.IsDeleted,
    E.EmployeeID, E.Code AS EMPCode,
    R.ReqCode
FROM LeaveBids AS L
INNER JOIN Employees AS E ON E.EmployeeID = L.EmpId
INNER JOIN ReqScheduleInfo AS R ON R.ID = L.SceduleId
WHERE E.Code = @EmpCode AND R.ReqCode = @ScheduleCode
ORDER BY L.PriorityLevel");


                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        LeaveBidsDataStatic data = new LeaveBidsDataStatic();
                        data.Employee = new EmployeeData();
                        data.Schedule = new ReqScheduleInfoData();

                        data.BidId = int.Parse(reader["BidId"].ToString());
                        data.BidCode = reader["BidCode"].ToString();
                        data.OldBidCode = data.BidCode;
                        data.PriorityLevel = int.Parse(reader["PriorityLevel"].ToString());

                        data.BidType = (BasicEnums.BidType)Enum.Parse(typeof(BasicEnums.BidType), reader["BidType"].ToString());

                        data.DateFrom = Convert.ToDateTime(reader["Datefrom"]);
                        data.DateTo = Convert.ToDateTime(reader["DateTo"]);


                        data.DateFromStr = data.DateFrom.ToString("dd/MM/yyyy");
                        data.DateToStr = data.DateTo.ToString("dd/MM/yyyy");


                        data.DateTo = Convert.ToDateTime(reader["DateTo"]);
                        data.NumberOfDays = int.Parse(reader["NumberOfDays"].ToString());
                        data.NumberOfDaysMin = int.Parse(reader["NumberOfDaysMin"].ToString());
                        data.NumberOfDaysMax = int.Parse(reader["NumberOfDaysMax"].ToString());

                        data.Employee.Code = reader["EMPCode"].ToString();
                        data.Schedule.ReqCode = reader["ReqCode"].ToString();
                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());
                        data.ExistingFlag = true;
                        data.NewBidFlag = false;
                        data.Bidflag = true;
                        DataList.Add(data);

                    }
                }

                connection.Close();
            }

            return DataList;
        }

        public bool SaveLeaveBidsData(ObservableCollection<LeaveBidsDataStatic> Data, string EmployeeCode, string ScheduleCode)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Retrieve the final item from the Rmaster table
                    var employee = dbContext.Employees.SingleOrDefault(r => r.Code == EmployeeCode);
                    var schedule = dbContext.ReqScheduleInfo.SingleOrDefault(r => r.ReqCode == ScheduleCode);

                    if (employee == null)
                    {
                        // Final item not found
                        return false;
                    }

                    int EmployeeId = employee.EmployeeID;
                    int scheduleId = schedule.ID;

                    int result = 0;
                    foreach (var row in Data)
                    {
                        string OldBidCode = row.OldBidCode;


                        if (row.ExistingFlag == false && row.Bidflag == true && row.NewBidFlag == true)
                        {
                            // Insert new bom
                            LeaveBidsDataEntity newBid = new LeaveBidsDataEntity
                            {
                                EmpId = EmployeeId,
                                SceduleId = scheduleId,
                                BidCode = row.BidCode,
                                PriorityLevel = row.PriorityLevel,
                                BidType = row.BidType.ToString(),
                                DateFrom = row.DateFrom,
                                DateTo = row.DateTo,
                                DateFromStr = row.DateFrom.ToString(),
                                DateToStr = row.DateTo.ToString(),
                                NumberOfDays = row.NumberOfDays,
                                NumberOfDaysMin = row.NumberOfDaysMin,
                                NumberOfDaysMax = row.NumberOfDaysMax

                            };

                            dbContext.LeaveBids.Add(newBid);
                            result += 1;

                        }
                        else if (row.ExistingFlag == true && row.Bidflag == true && row.Modify == true)
                        {
                                var existingBid = dbContext.LeaveBids.SingleOrDefault(b => b.EmpId == EmployeeId && b.BidId == row.BidId);
                                // Update existing bom
                                existingBid.BidCode = row.BidCode;
                                existingBid.PriorityLevel = row.PriorityLevel;
                                existingBid.BidType = row.BidType.ToString();
                                existingBid.DateFrom = row.DateFrom;
                                existingBid.DateTo = row.DateTo;
                                existingBid.DateFromStr = row.DateFrom.ToString();
                                existingBid.DateToStr = row.DateTo.ToString();
                                existingBid.NumberOfDays = row.NumberOfDays;
                                existingBid.NumberOfDaysMin = row.NumberOfDaysMin;
                                existingBid.NumberOfDaysMax = row.NumberOfDaysMax;
                        }


                    }

                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "SaveLeaveBidsData", "Notes");
                return false;
            }
        }

        //public bool SaveLeaveBidsRows(ObservableCollection<LeaveBidRowData> Data)
        //{
        //    try
        //    {
        //        using (var dbContext = new ErpDbContext(options))
        //        {
        //            // Retrieve the final item from the Rmaster table


        //            foreach (var row in Data)
        //            {

        //                // Insert new bom
        //                LeaveBidRowsDataEntity newRow = new LeaveBidRowsDataEntity();

        //                var BidId = dbContext.LeaveBids.SingleOrDefault(b => b.BidCode == row.LeaveBid.BidCode).BidId;
        //                newRow.BidId = BidId;
        //                newRow.EmpId = row.EmpId;
        //                newRow.ScheduleId = row.ScheduleId;
        //                newRow.Date = row.Date;
        //                newRow.DateStr = row.DateStr;


        //                dbContext.LeaveBidsRows.Add(newRow);

        //            }

        //            dbContext.SaveChanges();
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogError(ex, "SaveLeaveBidsRows", "Notes");
        //        return false;
        //    }
        //}

        public int DeleteLeaveBidData(LeaveBidsData Data)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    string BidCode = Data.BidCode;
                    var existingBid = dbContext.LeaveBids.SingleOrDefault(b => b.BidCode == Data.BidCode);

                    // Check if the bid exists
                    if (existingBid == null)
                    {
                        // Bid does not exist in the database
                        return 1;
                    }

                    dbContext.LeaveBids.Remove(existingBid);
                    dbContext.SaveChanges();
                    return 2; // Indicating successful deletion
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "DeleteLeaveBidData", "Notes");
                return 0; // Indicating failure due to an exception
            }
        }


        #endregion

        #region 2ndTab LeaveStatus
        public int SaveLeaveStatusData(EmployeeData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    int employeeId = flatData.EmployeeId;
                    var existingQuery = dbContext.LeaveStatus.Where(c => c.EmpId == employeeId);
                    var existing = existingQuery.SingleOrDefault();

                    if (existing != null)
                    {


                        // Update existing customer
                        existing.Total = flatData.LeaveStatus.Total;
                        existing.Used = flatData.LeaveStatus.Used;
                        existing.CurrentBalance = flatData.LeaveStatus.CurrentBalance;
                        existing.ProjectedBalance = flatData.LeaveStatus.ProjectedBalance;
                        existing.EmpId = employeeId;
                        dbContext.SaveChanges();

                        return 1;
                    }
                    else
                    {
                        var newItem = new LeaveStatusDataEntity();
                        // Insert new item
                        newItem.Total = flatData.LeaveStatus.Total;
                        newItem.Used = flatData.LeaveStatus.Used;
                        newItem.CurrentBalance = flatData.LeaveStatus.CurrentBalance;
                        newItem.ProjectedBalance = flatData.LeaveStatus.ProjectedBalance;
                        newItem.EmpId = employeeId;

                        dbContext.LeaveStatus.Add(newItem);
                        dbContext.SaveChanges();
                        return 1;

                    }

                }
            }
            catch (Exception ex)
            {
                LogError(ex, "SaveLeaveStatusData", "Notes");
                return -1;
            }
        }

        public LeaveStatusData GetLeaveStatusChooserData(int Id, string Code)
        {
            LeaveStatusData FlatData = new LeaveStatusData();
            string FilterStr = "";
            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;

                    if (Id > 0)
                    {
                        command.Parameters.AddWithValue("@ID", Id);
                        FilterStr = String.Format(@" and E.EmployeeID =@ID");

                    }

                    else if (!string.IsNullOrWhiteSpace(Code))
                    {
                        command.Parameters.AddWithValue("@Code", Code);
                        FilterStr = String.Format(@" and E.Code =@Code");

                    }
                    command.CommandText = string.Format(@"select L.LSId,L.Total,L.Used,L.CurrentBalance,L.ProjectedBalance,E.EmployeeID,E.Code
from LeaveStatus as L
INNER JOIN Employees as E ON E.EmployeeID =L.EmpId
                                              Where 1=1 {0}", FilterStr);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            LeaveStatusData data = new LeaveStatusData();



                            data.Total = int.Parse(reader["Total"].ToString());
                            data.Used = int.Parse(reader["Used"].ToString());
                            data.ProjectedBalance = int.Parse(reader["ProjectedBalance"].ToString());


                            FlatData = data;
                        }
                    }

                    connection.Close();
                }


                return FlatData;
            }
            catch (Exception ex)
            {
                LogError(ex, "GetLeaveStatusChooserData", "Notes");
                return null;
            }
        }
        #endregion

        #region Extra

        #region 3dTab Languages
        public ObservableCollection<EMPLanguageData> GetEMPLanguageData(string finalEmployeeCode, bool addLanguageFlag)
        {
            ObservableCollection<EMPLanguageData> data = new ObservableCollection<EMPLanguageData>();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.Parameters.AddWithValue("@finalEmployeeCode", finalEmployeeCode);

                command.CommandText = @"
            Select EmployeeID from Employees 
            Where Code = @finalEmployeeCode";

                int finalEmployeeId = (int)command.ExecuteScalar();
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@finalEmployeeId", finalEmployeeId);

                command.CommandText = @"
                select L.LId,L.LCode,L.LDescr
                From Language as L
                INNER JOIN EmpLanguages AS EL ON EL.LanguageId = L.LId
                WHERE EL.EMPId = @finalEmployeeId";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LanguageData languagedata = new LanguageData();
                        EMPLanguageData EMPLData = new EMPLanguageData();

                        EMPLData.FinalEmployeeId = finalEmployeeId;

                        languagedata.Id = int.Parse(reader["LId"].ToString());
                        languagedata.Code = reader["LCode"].ToString();
                        languagedata.Descr = reader["LDescr"].ToString();


                        EMPLData.Language = languagedata;


                        EMPLData.LanguageFlag = true;
                        EMPLData.NewLanguageFlag = false;
                        EMPLData.ExistingFlag = true;

                        data.Add(EMPLData);
                    }
                }

                if (addLanguageFlag)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@finalEmployeeId", finalEmployeeId);

                    command.CommandText = @"
select L.LId ,L.LCode,L.LDescr
From Language as L
WHERE NOT EXISTS (SELECT 1 FROM EmpLanguages WHERE LanguageId = L.LId AND EmpId = @finalEmployeeId) 
AND L.isDeleted = 0";


                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LanguageData languagedata = new LanguageData();
                            EMPLanguageData EMPLData = new EMPLanguageData();

                            EMPLData.FinalEmployeeId = finalEmployeeId;

                            languagedata.Id = int.Parse(reader["LId"].ToString());
                            languagedata.Code = reader["LCode"].ToString();
                            languagedata.Descr = reader["LDescr"].ToString();


                            EMPLData.Language = languagedata;

                            EMPLData.LanguageFlag = false;
                            EMPLData.NewLanguageFlag = true;
                            EMPLData.ExistingFlag = false;

                            data.Add(EMPLData);
                        }
                    }
                }

                connection.Close();
                return data;
            }
        }
        public bool SaveEMPLanguageData(ObservableCollection<EMPLanguageData> Data, string FinalEmployeeCode)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Retrieve the final item from the Rmaster table
                    var employee = dbContext.Employees.SingleOrDefault(r => r.Code == FinalEmployeeCode);

                    if (employee == null)
                    {
                        // Final item not found
                        return false;
                    }

                    int finalEmployeeId = employee.EmployeeID;

                    int result = 0;
                    foreach (var row in Data)
                    {
                        int EMPLanguageId = row.Language.Id;
                        var existingLanguage = dbContext.EmpLanguages.SingleOrDefault(b => b.EmpId == finalEmployeeId && b.LanguageId == EMPLanguageId);

                        if (existingLanguage == null && row.LanguageFlag == true && row.NewLanguageFlag == true)
                        {
                            // Insert new bom
                            EMPLanguagesDataEntity newLanguage = new EMPLanguagesDataEntity
                            {
                                EmpId = finalEmployeeId,
                                LanguageId = EMPLanguageId,
                            };

                            dbContext.EmpLanguages.Add(newLanguage);
                            result += 1;
                        }
                        else if (row.ExistingFlag == true && row.LanguageFlag == false)
                        {
                            dbContext.EmpLanguages.Remove(existingLanguage);

                        }
                        //else if (row.ExistingFlag == true && row.BomItemFlag == true)
                        //{
                        //    // Update existing bom
                        //    existingBom.Percentage = (float)row.BomPercentage;
                        //}
                    }

                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "SaveEMPLanguageData", "Notes");
                return false;
            }
        }

        #region Language

        public ObservableCollection<LanguageData> GetLanguageData(bool ShowDeleted)
        {
            ObservableCollection<LanguageData> DataList = new ObservableCollection<LanguageData>();

            string FilterStr = "";

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = String.Format(@" and IsDeleted =@ShowDeleted");
                }
                command.CommandText = string.Format(@"select LId,LCode,LDescr,IsDeleted from Language Where 1=1 {0}", FilterStr);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LanguageData data = new LanguageData();

                        data.Id = int.Parse(reader["LId"].ToString());
                        data.Code = reader["LCode"].ToString();
                        data.Descr = reader["LDescr"].ToString();
                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());

                        DataList.Add(data);
                    }
                }

                connection.Close();

            }

            return DataList;
        }


        public bool SaveLanguageData(ObservableCollection<LanguageData> Data)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {


                    bool hasChanges = false;
                    foreach (var row in Data)
                    {
                        var existingrow = dbContext.Language.SingleOrDefault(b => b.LId == row.Id);

                        if (existingrow == null)
                        {
                            LanguageDataEntity newrow = new LanguageDataEntity();
                            newrow.LCode = row.Code;
                            newrow.LDescr = row.Descr;
                            newrow.IsDeleted = false;
                            dbContext.Language.Add(newrow);
                            hasChanges = true;
                        }
                        else if (existingrow != null)
                        {

                            existingrow.LCode = row.Code;
                            existingrow.LDescr = row.Descr;
                            existingrow.IsDeleted = row.IsDeleted;

                            hasChanges = true;



                        }


                    }

                    if (hasChanges)
                    {
                        dbContext.SaveChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "SaveLanguageData", "Notes");
                return false;
            }
        }



        #endregion

        #endregion
        #endregion
        #endregion

        #region ReqSchedule
        public int SetMainSchedule(ReqScheduleInfoData FlatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    int ReqId = FlatData.ID;

                    var selectedQuery = dbContext.ReqScheduleInfo.Where(r => r.ID == ReqId);
                    var selectedSchedule = selectedQuery.SingleOrDefault();


                    var MainSchedulequery = dbContext.ReqScheduleInfo.Where(r => r.MainSchedule == true);
                    var MainSchedule = MainSchedulequery.SingleOrDefault();
                    // Execute the query and get the result


                    var result = System.Windows.MessageBox.Show($"The Schedule with Code {FlatData.ReqCode}  will be set as the Main Schedule  . Proceed?", "Confirmation", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        if (MainSchedule != null)
                        {

                            MainSchedule.MainSchedule = false;
                            selectedSchedule.MainSchedule = true;


                        }
                        else
                        {
                            selectedSchedule.MainSchedule = true;
                        }
                        dbContext.SaveChanges();
                        return 2;
                    }
                    else
                    {
                        return 1;
                    }


                }
            }
            catch (Exception ex)
            {
                LogError(ex, "SetMainSchedule", "Notes");
                return -1;

            }
        }
        public int SaveReqScheduleInfoData(ReqScheduleInfoData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    int ReqId = flatData.ID;
                    var existingItemQuery = dbContext.ReqScheduleInfo.Where(r => r.ID == ReqId);
                    var existingItem = existingItemQuery.SingleOrDefault();
                    // Execute the query and get the result

                    if (existingItem != null)
                    {

                        // Update existing item
                        existingItem.ID = flatData.ID;
                        existingItem.ReqCode = flatData.ReqCode;
                        existingItem.ReqDescr = flatData.ReqDescr;
                        existingItem.Notes = flatData.Notes;
                        existingItem.DateFrom = flatData.DateFrom;
                        existingItem.DateTo = flatData.DateTo;
                        existingItem.DateTo = flatData.DateTo;
                        existingItem.LimitLineFixed = flatData.LimitLineFixed;

                        existingItem.IsDeleted = flatData.IsDeleted;
                        existingItem.MainSchedule = flatData.MainSchedule;



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
                LogError(ex, "SaveReqScheduleInfoData", "Notes");
                return -1;

            }
        }
        public int AddReqScheduleInfoData(ReqScheduleInfoData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    var existingItemQuery = dbContext.ReqScheduleInfo.Where(r => r.ReqCode == flatData.ReqCode);
                    var existingItem = existingItemQuery.SingleOrDefault();
                    // Execute the query and get the result


                    if (existingItem == null)
                    {
                        var newItem = new ReqScheduleInfoDataEntity();
                        // Insert new ForeCast
                        newItem.ReqCode = flatData.ReqCode;
                        newItem.ReqDescr = flatData.ReqDescr;
                        newItem.Notes = flatData.Notes;
                        newItem.DateFrom = flatData.DateFrom;
                        newItem.DateTo = flatData.DateTo;
                        newItem.LimitLineFixed = flatData.LimitLineFixed;
                        newItem.IsDeleted = false;
                        newItem.MainSchedule = false;

                        dbContext.ReqScheduleInfo.Add(newItem);

                        dbContext.SaveChanges();

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
                LogError(ex, "AddReqScheduleInfoData", "Notes");
                return 2;

            }
        }
        public bool SaveReqScheduleRows(ReqScheduleInfoData FlatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    var ScheduleRows = FlatData.ReqScheduleRowsData;
                    var ScheduleInfo = dbContext.ReqScheduleInfo.SingleOrDefault(r => r.ID == FlatData.ID);

                    String ReqCode = ScheduleInfo.ReqCode.ToString();

                    bool hasChanges = false;
                    foreach (var row in ScheduleRows)
                    {
                        var DateStr = row.DateStr;

                        var existingRows = dbContext.ReqScheduleRows.Where(b => b.ReqCode == ReqCode && b.DateStr == row.DateStr
                        && b.Position == row.Position.ToString());

                        var existingrow = dbContext.ReqScheduleRows.FirstOrDefault(b => b.ReqCode == ReqCode && b.DateStr == row.DateStr
                        && b.Position == row.Position.ToString());

                        if (existingrow == null)
                        {
                            dbContext.ReqScheduleRows.Add(new ReqScheduleRowsDataEntity
                            {
                                ReqCode = ReqCode,
                                Position = row.Position.ToString(),
                                Date = row.Date,
                                DateStr = row.DateStr,
                                LimitLine = row.LimitLine


                            });
                            hasChanges = true;
                        }
                        else if (existingrow != null)
                        {

                            if (existingrow.LimitLine != row.LimitLine)
                            {
                                existingrow.LimitLine = row.LimitLine;
                                hasChanges = true;

                            }


                        }


                    }

                    if (hasChanges)
                    {
                        dbContext.SaveChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "SaveReqScheduleRows", "Notes");
                return false;
            }
        }


        public ObservableCollection<ReqScheduleRowsData> GetReqSchedulesRows(string ReqCode)
        {
            ObservableCollection<ReqScheduleRowsData> DataList = new ObservableCollection<ReqScheduleRowsData>();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.Parameters.AddWithValue("@Code", ReqCode);

                command.CommandText = string.Format(@"select REQID,REQCODE,POSITION,DATE,DATESTR,LIMITLINE
FROM ReqSchedulerows
Where ReqCode =@Code");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        ReqScheduleRowsData data = new ReqScheduleRowsData();

                        data.ReqCode = reader["REQCODE"].ToString();
                        data.Position = (BasicEnums.EmployeeType)Enum.Parse(typeof(BasicEnums.EmployeeType), reader["POSITION"].ToString());
                        data.LimitLine = int.Parse(reader["LIMITLINE"].ToString());
                        data.Date = Convert.ToDateTime(reader["DATE"]);

                        data.DateStr = reader["DATESTR"].ToString();


                        DataList.Add(data);

                    }
                }

                connection.Close();
            }

            return DataList;
        }
        public ObservableCollection<ReqScheduleRowsData> GetReqSchedulesRowsByEmpType(string ReqCode, BasicEnums.EmployeeType Position)
        {
            ObservableCollection<ReqScheduleRowsData> DataList = new ObservableCollection<ReqScheduleRowsData>();
            string FilterStr = "";
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;





                command.Parameters.AddWithValue("@ReqCode", ReqCode);
                command.Parameters.AddWithValue("@Position", Position.ToString());

                FilterStr += @" and REQCODE = @ReqCode";
                FilterStr += @" and POSITION = @Position";

                command.CommandText = string.Format(@"select REQID,REQCODE,POSITION,
DATE,DATESTR,LIMITLINE
FROM ReqSchedulerows
Where 1=1 {0}
Order by Date", FilterStr);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        ReqScheduleRowsData data = new ReqScheduleRowsData();

                        data.ReqCode = reader["REQCODE"].ToString();
                        data.Position = (BasicEnums.EmployeeType)Enum.Parse(typeof(BasicEnums.EmployeeType), reader["POSITION"].ToString());
                        data.LimitLine = int.Parse(reader["LIMITLINE"].ToString());
                        data.Date = Convert.ToDateTime(reader["DATE"]);

                        data.DateStr = reader["DATESTR"].ToString();


                        DataList.Add(data);

                    }
                }

                connection.Close();
            }

            return DataList;
        }
        public ObservableCollection<ReqScheduleInfoData> GetReqScheduleInfoData(bool ShowDeleted)
        {
            ObservableCollection<ReqScheduleInfoData> DataList = new ObservableCollection<ReqScheduleInfoData>();


            string FilterStr = "";


            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = String.Format(@"and IsDeleted = @ShowDeleted");

                }

                command.CommandText = string.Format(@"select ID,REQCODE,REQDESCR,NOTES,DATEFROM,DATETO,ISDELETED,MAINSCHEDULE,LimitLineFixed
FROM ReqScheduleInfo
Where  1=1 {0}", FilterStr);




                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ReqScheduleInfoData data = new ReqScheduleInfoData();

                        data.ID = int.Parse(reader["ID"].ToString());
                        data.ReqCode = reader["REQCODE"].ToString();
                        data.ReqDescr = reader["REQDESCR"].ToString();
                        data.LimitLineFixed = int.Parse(reader["LimitLineFixed"].ToString());
                        data.DateFrom = Convert.ToDateTime(reader["DATEFROM"]);
                        data.DateTo = Convert.ToDateTime(reader["DATETO"]);
                        data.DateFromStr = data.DateFrom.ToString("dd/MM/yyyy");
                        data.DateToStr = data.DateTo.ToString("dd/MM/yyyy");
                        data.IsDeleted = bool.Parse(reader["ISDELETED"].ToString());
                        data.MainSchedule = bool.Parse(reader["MAINSCHEDULE"].ToString());
                        data.Notes = reader["NOTES"].ToString();


                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }
        public ReqScheduleInfoData GetReqScheduleInfoChooserData(int id, string Code)
        {
            ReqScheduleInfoData data = new ReqScheduleInfoData();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (id > 0)
                {
                    command.Parameters.AddWithValue("@ID", id);

                    command.CommandText = string.Format(@"select ID,REQCODE,REQDESCR,NOTES,DATEFROM,DATETO,ISDELETED,MAINSCHEDULE,LimitLineFixed
                                                        FROM ReqScheduleInfo
                                                        Where ID=@ID");
                }
                else if (!string.IsNullOrWhiteSpace(Code))
                {
                    command.Parameters.AddWithValue("@Code", Code); // Corrected variable name

                    command.CommandText = string.Format(@"select ID,REQCODE,REQDESCR,NOTES,DATEFROM,DATETO,ISDELETED,MAINSCHEDULE,LimitLineFixed
                                                        FROM ReqScheduleInfo
                                                        Where REQCODE=@Code"); // Corrected parameter name
                }



                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        data.ID = int.Parse(reader["ID"].ToString());
                        data.ReqCode = reader["REQCODE"].ToString();
                        data.ReqDescr = reader["REQDESCR"].ToString();
                        data.DateFrom = Convert.ToDateTime(reader["DATEFROM"]);
                        data.DateTo = Convert.ToDateTime(reader["DATETO"]);
                        data.LimitLineFixed = int.Parse(reader["LimitLineFixed"].ToString());

                        data.IsDeleted = bool.Parse(reader["ISDELETED"].ToString());
                        data.MainSchedule = bool.Parse(reader["MAINSCHEDULE"].ToString());
                        data.Notes = reader["NOTES"].ToString();


                    }
                }

                connection.Close();
            }

            return data;
        }
        public ReqScheduleInfoData GetMainScheduleInfoData()
        {
            ReqScheduleInfoData data = new ReqScheduleInfoData();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;



                command.CommandText = string.Format(@"select ID,REQCODE,REQDESCR,NOTES,DATEFROM,DATETO,ISDELETED,MAINSCHEDULE,LimitLineFixed
                                                    FROM ReqScheduleInfo
                                                    Where MAINSCHEDULE=1"); // Corrected parameter name




                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        data.ID = int.Parse(reader["ID"].ToString());
                        data.ReqCode = reader["REQCODE"].ToString();
                        data.ReqDescr = reader["REQDESCR"].ToString();
                        data.DateFrom = Convert.ToDateTime(reader["DATEFROM"]);
                        data.DateTo = Convert.ToDateTime(reader["DATETO"]);
                        data.LimitLineFixed = int.Parse(reader["LimitLineFixed"].ToString());
                        data.IsDeleted = bool.Parse(reader["ISDELETED"].ToString());
                        data.MainSchedule = bool.Parse(reader["MAINSCHEDULE"].ToString());
                        data.Notes = reader["NOTES"].ToString();


                    }
                }

                connection.Close();
            }

            return data;
        }

        #endregion

        #region Vacation Planning

        #region CRUD Commands

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

                    // Execute the query and get the result


                    if (existing != null)
                    {


                        // Update existing customer
                        existing.VPCODE = flatData.VPCode;
                        existing.VPDESCR = flatData.VPDescr;
                        existing.MaxSatisfiedBids = flatData.Bmax;
                        existing.SeparValue = flatData.Se;

                        existing.EMPLOYEETYPE = flatData.EmployeeType.ToString();
                        existing.VPLOGICTYPE = flatData.VPLogicType.ToString();
                        existing.IsDeleted = flatData.IsDeleted;
                        existing.SCHEDULEID = flatData.Schedule.ID;


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
                    // Separate query from execution
                    var existingVPQuery = dbContext.VPInput.Where(r => r.VPCODE == flatData.VPCode);
                    var existingVP = existingVPQuery.SingleOrDefault();
                    // Execute the query and get the result


                    if (existingVP == null)
                    {
                        var newVP = new VPInputDataEntity();
                        // Insert new item
                        newVP.VPCODE = flatData.VPCode;
                        newVP.VPDESCR = flatData.VPDescr;
                        newVP.EMPLOYEETYPE = BasicEnums.EmployeeType.Captain.ToString();
                        newVP.VPLOGICTYPE = BasicEnums.VPLogicType.Strict_Seniority.ToString();
                        newVP.MaxSatisfiedBids = 2;
                        newVP.SeparValue = 2;

                        newVP.IsDeleted = false;

                        var MainSchedule = dbContext.ReqScheduleInfo.FirstOrDefault(r => r.MainSchedule == true);
                        newVP.SCHEDULEID = MainSchedule.ID;




                        dbContext.VPInput.Add(newVP);

                        dbContext.SaveChanges();
                        return 0;
                    }
                    else
                    {
                        // Else Print messages
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

        public ObservableCollection<VacationPlanningInputData> GetVPInputData(bool ShowDeleted)
        {
            ObservableCollection<VacationPlanningInputData> DataList = new ObservableCollection<VacationPlanningInputData>();

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

                command.CommandText = string.Format(@"SELECT V.VPID,V.VPCODE,V.VPDESCR,V.EMPLOYEETYPE,V.VPLOGICTYPE,V.SeparValue,
V.MaxSatisfiedBids,V.ISDELETED,
R.ID,R.ReqCode,R.ReqDescr,R.DateFrom,R.DateTo,R.LimitLineFixed
FROM VPInput as V
INNER JOIN ReqScheduleInfo AS R ON R.ID = V.SCHEDULEID
Where 1=1 {0}", FilterStr);


                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        VacationPlanningInputData data = new VacationPlanningInputData();
                        data.Schedule = new ReqScheduleInfoData();
                        data.VPId = int.Parse(reader["VPID"].ToString());
                        data.VPCode = reader["VPCODE"].ToString();
                        data.VPDescr = reader["VPDESCR"].ToString();
                        data.Bmax = int.Parse(reader["MaxSatisfiedBids"].ToString());
                        data.Se = int.Parse(reader["SeparValue"].ToString());

                        data.EmployeeType = (BasicEnums.EmployeeType)Enum.Parse(typeof(BasicEnums.EmployeeType), reader["EMPLOYEETYPE"].ToString());
                        data.VPLogicType = (BasicEnums.VPLogicType)Enum.Parse(typeof(BasicEnums.VPLogicType), reader["VPLogicType"].ToString());

                        data.IsDeleted = bool.Parse(reader["ISDELETED"].ToString());



                        data.Schedule.ID = int.Parse(reader["ID"].ToString());
                        data.Schedule.ReqCode = reader["ReqCode"].ToString();
                        data.Schedule.ReqDescr = reader["ReqDescr"].ToString();

                        data.Schedule.DateFrom = DateTime.Parse(reader["DateFrom"].ToString());
                        data.Schedule.DateTo = DateTime.Parse(reader["DateTo"].ToString());
                        data.Schedule.LimitLineFixed = int.Parse(reader["LimitLineFixed"].ToString());

                        data.Schedule.DateFromStr = data.Schedule.DateFrom.ToString("dd/MM/yyyy"); //THELEI ALAGH
                        data.Schedule.DateToStr = data.Schedule.DateTo.ToString("dd/MM/yyyy");



                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }

        public VacationPlanningInputData GetVPInputChooserData(int VPId, string VPCode, VacationPlanningInputData Data)
        {

            string FilterStr = "";
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (VPId > 0)
                {
                    command.Parameters.AddWithValue("@VPID", VPId);
                    FilterStr += " and V.VPID = @VPID";
                }
                else if (!string.IsNullOrWhiteSpace(VPCode))
                {
                    command.Parameters.AddWithValue("@VPCODE", VPCode);
                    FilterStr += " and V.VPCODE = @VPCODE";
                }

                command.CommandText = string.Format(@"SELECT V.VPID, V.VPCODE, V.VPDESCR, V.EMPLOYEETYPE, V.VPLOGICTYPE, V.MaxSatisfiedBids, V.ISDELETED,V.SeparValue,
                                      R.ID, R.ReqCode, R.ReqDescr, R.DateFrom, R.DateTo
                                      FROM VPInput AS V
                                      INNER JOIN ReqScheduleInfo AS R ON R.ID = V.SCHEDULEID
                                      WHERE 1=1 {0}", FilterStr);



                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        Data.Schedule = new ReqScheduleInfoData();
                        Data.VPId = int.Parse(reader["VPID"].ToString());
                        Data.VPCode = reader["VPCODE"].ToString();
                        Data.VPDescr = reader["VPDESCR"].ToString();
                        Data.EmployeeType = (BasicEnums.EmployeeType)Enum.Parse(typeof(BasicEnums.EmployeeType), reader["EMPLOYEETYPE"].ToString());
                        Data.VPLogicType = (BasicEnums.VPLogicType)Enum.Parse(typeof(BasicEnums.VPLogicType), reader["VPLogicType"].ToString());
                        Data.Bmax = int.Parse(reader["MaxSatisfiedBids"].ToString());
                        Data.Se = int.Parse(reader["SeparValue"].ToString());

                        Data.IsDeleted = bool.Parse(reader["ISDELETED"].ToString());



                        Data.Schedule.ID = int.Parse(reader["ID"].ToString());
                        Data.Schedule.ReqCode = reader["ReqCode"].ToString();
                        Data.Schedule.ReqDescr = reader["ReqDescr"].ToString();

                        Data.Schedule.DateFrom = DateTime.Parse(reader["DateFrom"].ToString());
                        Data.Schedule.DateTo = DateTime.Parse(reader["DateTo"].ToString());

                        Data.Schedule.DateFromStr = Data.Schedule.DateFrom.ToString(); //THELEI ALAGH
                        Data.Schedule.DateToStr = Data.Schedule.DateTo.ToString();
                    }
                }

                connection.Close();
            }

            return Data;
        }
        #endregion

        #region  Optimization

        //public VacationPlanningOutputData Calculate_VacationPlanning_Gurobi(VacationPlanningInputData InputData)
        //{
        //    GRBEnv env = new GRBEnv("vplogfile.log");
        //    GRBModel model = new GRBModel(env);
        //    GRBEnv finalenv = new GRBEnv("vplogfilefinal.log");
        //    VacationPlanningOutputData Data = new VacationPlanningOutputData();
        //    Data.VPYijResultsDataGrid = new ObservableCollection<VPYijResultsData>();
        //    Data.VPYijzResultsDataGrid = new ObservableCollection<VPYijResultsData>();
        //    Data.VPXijResultsDataGrid = new ObservableCollection<VPXijResultsData>();
        //    Data.VPXiResultsDataGrid = new ObservableCollection<VPXiResultData>();
        //    Data.EmpLeaveStatusData = new ObservableCollection<EmployeeData>();

        //    List<string> rows = new List<string>();
        //    List<string> columns = new List<string>();
        //    Dictionary<(string, string), double> make_plan = new Dictionary<(string, string), double>();
        //    double bigM = 10000;

        //    try
        //    {
        //        #region Optimization

        //        #region Optimization paramaters

        //        int MaxSatisfiedBids = InputData.MaxSatisfiedBids; //Max αριθμος ικανοποιημένων Bids ανα υπάλληλο
        //        int SeparValue = InputData.SeparValue; // Seperation Value

        //        string[] Employees = InputData.Employees.Select(d => d.Code).ToArray(); //Πινακας με τους Κωδικους Υπαλληλων
        //        string[] Dates = InputData.DatesStr; //Πινακας με τα Dates




        //        Dictionary<string, int> MaxLeaveBidsPerEmployee = InputData.MaxLeaveBidsPerEmployee;
        //        Dictionary<int, int> MaxLeaveBidsPerEmployee_Int = InputData.MaxLeaveBidsPerEmployee_Int;


        //        // Zvalue = Number Of Specific    .Για Specific Bids το Zvalue = 1 Παντα
        //        // Rvalue = Number of NonSpecific . Για Specific,NonSpecific Bids to Rvalue = 1 Παντα

        //        //ZBidsDict = <(Employee Code, LeaveBidCode ,Rvalue), Zvalue>
        //        Dictionary<(string, string, int), int> ZbidsDict_Str = InputData.ZBidsDict;

        //        //RBidsDict = <(Employee Code, LeaveBidCode ),Rvalue>
        //        Dictionary<(string, string), int> RBidsDict_Str = InputData.RBidsDict;

        //        //ZBidsDict = <(Employee Code, LeaveBidCode ,Rvalue), Zvalue>
        //        Dictionary<(int, int, int), int> ZbidsDict = InputData.ZBidsDict_Int;

        //        //RBidsDict = <(Employee Code, LeaveBidCode ),Rvalue>
        //        Dictionary<(int, int), int> RBidsDict = InputData.RBidsDict_Int;


        //        int MaxLeaveBids = InputData.MaxLeaveBids; //Μεγιστος αριθμός Bids υπαλλήλου απο ολούς τους υπαλλήλους
        //        int LimitLineFixed = InputData.Schedule.LimitLineFixed; // Σταθερό Limit Line σε όλες τις ημέρες
        //        int Zmax = ZbidsDict_Str.Max(kvp => kvp.Value); //Μεγιστο Zvalue , το χρειαζόμαστε για την δήλωση της Yijrz
        //        int MaxNonSpecific = RBidsDict_Str.Max(kvp => kvp.Value); //Μεγιστο Rvalue , το χρειαζόμαστε για την δήλωση της Yijrz



        //        #region from string to int 


        //        // Mapping from string keys to integer indices
        //        Dictionary<string, int> EmployeeIndexMap = new Dictionary<string, int>();
        //        Dictionary<string, int> LeaveBidCodeIndexMap = new Dictionary<string, int>();
        //        // Initialize integer index counters
        //        int EmpIndexCounter = 0;
        //        int LeaveBidsIndexCounter = 0;

        //        // Fill IndexMaps
        //        foreach (var emp in Employees)
        //        {
        //            EmployeeIndexMap[emp] = EmpIndexCounter++;
        //        }


        //        #endregion

        //        #endregion

        //        #region Optimization Algorithm

        //        #region Decision Variables 
        //        // Decision variables
        //        GRBVar[,,,] Y = new GRBVar[Employees.Length, MaxLeaveBids, MaxNonSpecific, Zmax];
        //        GRBVar[,] X = new GRBVar[Employees.Length, Dates.Length];

        //        GRBVar[,,] R = new GRBVar[Employees.Length, MaxLeaveBids, MaxNonSpecific];


        //        // Create decision variables X
        //        for (int i = 0; i < Employees.Length; i++)
        //        {
        //            for (int t = 0; t < Dates.Length; t++)
        //            {
        //                // Define the variable name
        //                string varNameX = $"X{i + 1}_{t + 1}";

        //                // Create the binary variable with a name
        //                X[i, t] = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, varNameX);
        //            }
        //        }

        //        // Create decision variables Y
        //        for (int i = 0; i < Employees.Length; i++)
        //        {
        //            for (int j = 0; j < MaxLeaveBidsPerEmployee[Employees[i]]; j++)
        //            {
        //                var Rvalue = RBidsDict[(i, j)];
        //                for (int r = 0; r < Rvalue; r++) //allagh
        //                {
        //                    var Zvalue = ZbidsDict[(i, j, r + 1)];

        //                    for (int z = 0; z < Zvalue; z++) //allagh
        //                    {
        //                        // Define the variable name
        //                        string varNameY = $"Y{i + 1}_{j + 1}_{r + 1}_{z + 1}";

        //                        // Create the binary variable with a name
        //                        Y[i, j, r, z] = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, varNameY);
        //                    }

        //                }
        //            }
        //        }

        //        // Create decision variables R
        //        for (int i = 0; i < Employees.Length; i++)
        //        {
        //            for (int j = 0; j < MaxLeaveBidsPerEmployee[Employees[i]]; j++)
        //            {

        //                var Rvalue = RBidsDict[(i, j)];
        //                for (int r = 0; r < Rvalue; r++) //allagh
        //                {

        //                    // Define the variable name
        //                    string varNameR = $"R{i + 1}_{j + 1}_{r + 1}";

        //                    // Create the binary variable with a name
        //                    R[i, j, r] = model.AddVar(0.0, 0.0, 0.0, GRB.BINARY, varNameR);

        //                }
        //            }
        //        }

        //        #endregion

        //        #region Objective Function

        //        GRBLinExpr objective = 0;
        //        GRBLinExpr SumX = 0;

        //        for (int i = Employees.Length - 1; i >= 0; i--)
        //        {
        //            for (int j = MaxLeaveBidsPerEmployee[Employees[i]] - 1; j >= 0; j--)
        //            {
        //                var Rvalue = RBidsDict[(i, j)];

        //                for (int r = 0; r < Rvalue; r++) //allagh
        //                {
        //                    objective.AddTerm(1, R[i, j, r]);

        //                }
        //            }
        //            for (int t = 0; t < Dates.Length; t++)
        //            {

        //                // Create the binary variable with a name
        //                SumX.AddTerm(1, X[i, t]);

        //            }
        //        }

        //        objective = objective - 0.00000001 * SumX;
        //        model.SetObjective(objective, GRB.MAXIMIZE);

        //        #endregion

        //        #region Constrains
        //        // #1. Adding constraints for maximum number of satisfied bids 
        //        for (int i = 0; i < Employees.Length; i++)
        //        {
        //            GRBLinExpr sumLeaveBids = 0;
        //            for (int j = 0; j < MaxLeaveBidsPerEmployee[Employees[i]]; j++)
        //            {

        //                var Rvalue = RBidsDict[(i, j)];

        //                for (int r = 0; r < Rvalue; r++)
        //                {
        //                    sumLeaveBids.AddTerm(1.0, R[i, j, r]);
        //                }


        //            }

        //            // Adding the constraint for the current employee
        //            model.AddConstr(sumLeaveBids <= MaxSatisfiedBids, "MaxSatisfiedBids_" + Employees[i]);
        //        }


        //        // #2. Entitlements
        //        for (int i = 0; i < Employees.Length; i++)
        //        {
        //            var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees[i]);
        //            GRBLinExpr sumLeaveBidDays = 0;
        //            for (int j = 0; j < MaxLeaveBidsPerEmployee[Employees[i]]; j++)
        //            {
        //                var NumberOfDays = specificEmployee.LeaveBidDataGridStatic[j].NumberOfDaysMax;
        //                var Rvalue = RBidsDict[(i, j)];

        //                for (int r = 0; r < Rvalue; r++)
        //                {
        //                    var Zvalue = ZbidsDict[(i, j, r + 1)];

        //                    NumberOfDays = NumberOfDays - r;
        //                    for (int z = 0; z < Zvalue; z++)
        //                    {
        //                        sumLeaveBidDays.AddTerm(NumberOfDays, Y[i, j, r, z]); // Summing up the leavebidsDays for each employee
        //                    }
        //                }


        //            }

        //            var MaxLeaveDays = specificEmployee.LeaveStatus.CurrentBalance;

        //            // Adding the constraint for the current employee
        //            model.AddConstr(sumLeaveBidDays <= MaxLeaveDays, "MaxLeaveDays_" + Employees[i]);


        //        }

        //        // #3. Limit Lines

        //        for (int t = 0; t < Dates.Length; t++)
        //        {
        //            GRBLinExpr expr = 0;
        //            int sumdays = 0;
        //            //Ξεχωριστό LimitLine για κάθε ημέρα 
        //            var LimitLine = InputData.Schedule.ReqScheduleRowsData.ElementAt(t).LimitLine;

        //            for (int i = 0; i < Employees.Length; i++)
        //            {

        //                expr.AddTerm(1, X[i, t]);
        //                sumdays = sumdays + 1;

        //            }
        //            if (sumdays > LimitLine)
        //            {
        //                model.AddConstr(expr <= LimitLine, "LimitLine_" + Dates[t]);
        //            }
        //        }

        //        //#4. Overlapping
        //        #region  OverLapping

        //        for (int i = 0; i < Employees.Length; i++)
        //        {
        //            for (int j1 = 0; j1 < MaxLeaveBidsPerEmployee[Employees[i]] - 1; j1++)
        //            {
        //                for (int j2 = j1 + 1; j2 < MaxLeaveBidsPerEmployee[Employees[i]]; j2++)
        //                {
        //                    var Rvalue = RBidsDict[(i, j1)];


        //                    var EmployeeCode = Employees[i];
        //                    var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees[i]);

        //                    #region Find z1,z2
        //                    int Z1value = new int();
        //                    int Z2value = new int();
        //                    int R1value = RBidsDict[(i, j1)];
        //                    int R2value = RBidsDict[(i, j2)];

        //                    #endregion

        //                    for (int r1 = 0; r1 < R1value; r1++)
        //                    {
        //                        Z1value = ZbidsDict[(i, j1, r1 + 1)];

        //                        for (int r2 = 0; r2 < R2value; r2++)
        //                        {
        //                            Z2value = ZbidsDict[(i, j2, r2 + 1)];

        //                            for (int z1 = 0; z1 < Z1value; z1++)
        //                            {
        //                                for (int z2 = 0; z2 < Z2value; z2++)
        //                                {


        //                                    if (SeparOrOverlap(i, j1, j2, z1, z2, r1, r2))
        //                                    {
        //                                        GRBLinExpr expr = Y[i, j1, r1, z1] + Y[i, j2, r2, z2];
        //                                        model.AddConstr(expr <= 1, $"SO{i + 1}_{j1 + 1}_{z1 + 1}_{j2 + 1}_{z2 + 1}");
        //                                    }
        //                                }
        //                            }
        //                        }


        //                    }

        //                }
        //            }
        //        }
        //        bool SeparOrOverlap(int i, int j1, int j2, int z1, int z2, int r1, int r2)
        //        {

        //            var emp = InputData.Employees.ElementAt(i);

        //            var SelectedBid1 = emp.LeaveBidDataGridStatic.ElementAt(j1);
        //            var SelectedBid2 = emp.LeaveBidDataGridStatic.ElementAt(j2);



        //            if (SelectedBid2.DateFrom.AddDays(z2) >= SelectedBid1.DateFrom.AddDays(SelectedBid1.NumberOfDaysMax + SeparValue + z1 - r1 - 1))
        //            {
        //                Console.WriteLine("Condition: false");
        //                return false;

        //            };


        //            if (SelectedBid2.DateFrom.AddDays(SelectedBid2.NumberOfDaysMax + z2 - r1 - 1) <= SelectedBid1.DateFrom.AddDays(-SeparValue + z1))
        //            {
        //                Console.WriteLine("Condition: false");
        //                return false;

        //            };
        //            Console.WriteLine("Condition: True");
        //            return true;
        //        }

        //        #endregion
        //        //#5.Connection Between Y and X
        //        for (int i = 0; i < Employees.Length; i++)
        //        {
        //            var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees[i]);
        //            var maxBids = MaxLeaveBidsPerEmployee[Employees[i]];

        //            for (int j = 0; j < maxBids; j++)
        //            {
        //                var bid = specificEmployee.LeaveBidDataGridStatic[j];
        //                var NumberOfDays = bid.NumberOfDaysMax;
        //                var Rvalue = RBidsDict[(i, j)];
        //                GRBLinExpr sumDays = 0;
        //                for (int r = 0; r < Rvalue; r++) //allagh
        //                {
        //                    var Zvalue = ZbidsDict[(i, j, r + 1)];
        //                    NumberOfDays = NumberOfDays - r;
        //                    for (int z = 0; z < Zvalue; z++) //allagh
        //                    {
        //                        var startDateIndex = Array.IndexOf(Dates, bid.DateFrom.ToString("dd/MM/yyyy"));

        //                        GRBLinExpr expr = 0;
        //                        expr = Y[i, j, r, z] * NumberOfDays;

        //                        int start = startDateIndex + z;
        //                        int end = start + NumberOfDays;

        //                        for (int t = start; t < end; t++)
        //                        {
        //                            expr = expr - X[i, t]; // Add X variables for each day of the bid
        //                        }
        //                        // Add the constraint
        //                        model.AddConstr(expr <= 0, $"BidDaysConstraint_{Employees[i]}_{j}_{r}_{z}");

        //                    }

        //                }


        //            }
        //        }

        //        //#6.Connection Betweer Y and R -- Yijrz and Yijr . 
        //        for (int i = 0; i < Employees.Length; i++)
        //        {
        //            for (int j = 0; j < MaxLeaveBidsPerEmployee[Employees[i]]; j++)
        //            {

        //                var Rvalue = RBidsDict[(i, j)];

        //                for (int r = 0; r < Rvalue; r++)
        //                {
        //                    var Zvalue = ZbidsDict[(i, j, r + 1)];
        //                    GRBLinExpr sumYijrz = 0;
        //                    for (int z = 0; z < Zvalue; z++)
        //                    {
        //                        sumYijrz.AddTerm(1.0, Y[i, j, r, z]);

        //                    }
        //                    // Adding the constraint for the current employee
        //                    model.AddConstr(R[i, j, r] == sumYijrz, "Y_R_Connection" + Employees[i]);
        //                }


        //            }


        //        }

        //        #endregion

        //        #endregion

        //        #region New Optimize settings
        //        bool grant = false;
        //        BasicEnums.VPLogicType logic = InputData.VPLogicType; // Λογικη Ανάθεσης π.χ Strict Seniority

        //        int FinishedEmpIds = 0; //Το αντιστοιχο FinishedIds στο κωδικα της Python
        //        int FinishedBidIds = 1; //Μετρητής για τα ολοκληρωμέναBids

        //        int id = 0;
        //        var numRowsPerEmployee = InputData.Employees.Select(e => e.LeaveBidDataGridStatic.Count);
        //        var numOfEmployes = InputData.Employees.Count; //Το αντιστοιχο N της python

        //        int N = numRowsPerEmployee.Sum(); //Το N εδω ειναι o αριθμος των συνολικών Bids.
        //        int[] NextBid = new int[N];
        //        int[] NrOfBids = MaxLeaveBidsPerEmployee.Values.ToArray();
        //        List<string> outputLines = new List<string>();


        //        model.Update();

        //        while (FinishedEmpIds <= numOfEmployes) // Kozani
        //        {


        //            int j = NextBid[id];

        //            #region Check Bid
        //            var z = 0;
        //            var r = 0;
        //            #region Find ZValue,RValue
        //            int Rvalue = new int();
        //            var EmployeeCode = Employees[id];
        //            var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees[id]);

        //            var BidCode = specificEmployee.LeaveBidDataGridStatic[j].BidCode;
        //            Rvalue = RBidsDict_Str.TryGetValue((Employees[id], BidCode), out int valueR) ? valueR : Rvalue;

        //            #endregion
        //            for (r = 0; r < Rvalue; r++)
        //            {
        //                #region Check Bid


        //                GRBVar Rijr = model.GetVarByName($"R{id + 1}_{j + 1}_{r + 1}");
        //                Rijr.LB = 1;
        //                Rijr.UB = 1;

        //                model.Update();
        //                model.Optimize();
        //                bool solution = (model.Status == GRB.Status.OPTIMAL);
        //                if (solution)
        //                {
        //                    double rValue = R[id, j, r].X;

        //                    grant = true;
        //                    string message = $"Crew member {id + 1} was awarded bid {j + 1}";
        //                    Console.WriteLine(message);
        //                    outputLines.Add(message);
        //                    Data.ObjValue = model.ObjVal;
        //                    Rvalue = 0;
        //                }
        //                else
        //                {
        //                    grant = false;
        //                    Rijr.LB = 0;
        //                    Rijr.UB = 0;

        //                    string message = $"Crew member {id + 1} was not awarded bid {j + 1}";
        //                    Console.WriteLine(message);
        //                    outputLines.Add(message);
        //                }

        //                #endregion
        //            }

        //            #endregion
        //            NextBid[id]++;
        //            if (NextBid[id] == NrOfBids[id])
        //            {
        //                FinishedEmpIds++;

        //            }
        //            if (model.Status == GRB.Status.OPTIMAL)
        //            {
        //                Data.ObjValue = model.ObjVal;
        //                model.Update();
        //                model.Write("out.mst");
        //                model.Write("out.sol");
        //                model.Write("VPFeasable.lp");
        //                model.Write("VPFeasableMPS.mps");


        //            }
        //            if (FinishedBidIds == N)
        //            {
        //                break;
        //            }
        //            FinishedBidIds = FinishedBidIds + 1;
        //            if (FinishedBidIds <= N)
        //            {
        //                id = GetNextId(id, grant, numOfEmployes, NextBid, NrOfBids, FinishedEmpIds, logic);
        //            }
        //        }
        //        #endregion

        //        #endregion

        //        #region Save,Show Results
        //        var Upgrade = new bool();
        //        var flag = new bool();
        //        var customMessageBox = new CustomMessageBox("Do you want to Save the Results or Search for a better Solution?");
        //        if (customMessageBox.ShowDialog() == true)
        //        {
        //            // User clicked Save Only or Save and Upgrade
        //            if (customMessageBox.DialogResult == true)
        //            {
        //                // User clicked Save Only or Save and Upgrade
        //                //var result = customMessageBox.Message.Contains("Save and Upgrade") ? "Save and Upgrade" : "Save Only";
        //                var result = "Save Only";
        //                Console.WriteLine($"User clicked {result}");

        //                if (result == "Save and Upgrade" || result == "Save Only")
        //                {
        //                    #region OutputResults
        //                    if (Data.ObjValue > 0)
        //                    {

        //                        // Read the model from the files
        //                        using (GRBModel modelFromFiles = new GRBModel(finalenv, "VPFeasableMPS.mps"))
        //                        {
        //                            modelFromFiles.Optimize();
        //                            modelFromFiles.Update();
        //                            Data.ObjValue = modelFromFiles.ObjVal;
        //                            //Data.ObjValue = Math.Round(Data.ObjValue);
        //                            #region Insert Xij
        //                            // Extract the optimal solution for the 'X' variables
        //                            for (int i = 0; i < Employees.Length; i++)
        //                            {
        //                                for (int t = 0; t < Dates.Length; t++)
        //                                {
        //                                    string employee = Employees[i];
        //                                    string date = Dates[t];
        //                                    GRBVar Xit = modelFromFiles.GetVarByName($"X{i + 1}_{t + 1}");

        //                                    double xValue = Xit.X;
        //                                    if (xValue == 1)
        //                                    {
        //                                        Console.WriteLine($"Employee: {employee}, Date: {date}, Value: {xValue}");
        //                                    }
        //                                    // Store the optimal 'X' value in the data structure
        //                                    make_plan[(employee, date)] = xValue;

        //                                    // Add 'employee' and 'date' to the respective lists if they are not already there
        //                                    if (!rows.Contains(employee))
        //                                        rows.Add(employee);
        //                                    if (!columns.Contains(date))
        //                                        columns.Add(date);
        //                                }
        //                            }
        //                            #endregion


        //                            #region Print the optimal solution for 'X' variables
        //                            Console.WriteLine("Optimal Solution for X Variables:");
        //                            foreach (var employee in rows)
        //                            {
        //                                foreach (var date in columns)
        //                                {
        //                                    double xValue = make_plan.ContainsKey((employee, date)) ? make_plan[(employee, date)] : 0.0;
        //                                    Console.WriteLine($"Employee: {employee}, Date: {date}, Value: {xValue} -> Employee: {employee}, Date: {date}, Value: {xValue} X{(Array.IndexOf(Employees, employee) + 1)}{(Array.IndexOf(Dates, date) + 1)}");


        //                                    #region Populate VP Xij
        //                                    VPXijResultsData singleDataRecord = new VPXijResultsData();


        //                                    singleDataRecord.Xij = $"X{(Array.IndexOf(Employees, employee) + 1)}{(Array.IndexOf(Dates, date) + 1)}";
        //                                    singleDataRecord.XijFlag = xValue;
        //                                    singleDataRecord.Date = date;




        //                                    var SpecificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == employee);
        //                                    singleDataRecord.Employee = SpecificEmployee;

        //                                    Data.VPXijResultsDataGrid.Add(singleDataRecord);
        //                                    #endregion
        //                                }

        //                            }
        //                            #endregion

        //                            #region Extract the optimal solution for the 'Y' variables
        //                            Dictionary<(string, int, int, int), double> y_plan = new Dictionary<(string, int, int, int), double>();
        //                            for (int i = 0; i < Employees.Length; i++)
        //                            {
        //                                for (int j = 0; j < MaxLeaveBidsPerEmployee[Employees[i]]; j++)
        //                                {
        //                                    var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees[i]);
        //                                    var Rvalue = RBidsDict[(i, j)];

        //                                    for (int r = 0; r < Rvalue; r++)
        //                                    {
        //                                        var Zvalue = ZbidsDict[(i, j, r + 1)];
        //                                        GRBVar Rijr = modelFromFiles.GetVarByName($"R{i + 1}_{j + 1}_{r + 1}");

        //                                        double rValue = Rijr.X;


        //                                        for (int z = 0; z < Zvalue; z++) //allagh
        //                                        {
        //                                            string employee = Employees[i];
        //                                            int bidIndex = j;
        //                                            GRBVar Yijrz = modelFromFiles.GetVarByName($"Y{i + 1}_{j + 1}_{r + 1}_{z + 1}");

        //                                            double yValue = Yijrz.X;

        //                                            // Store the optimal 'Y' value in the data structure
        //                                            y_plan[(employee, bidIndex, r, z)] = yValue;
        //                                        }
        //                                    }







        //                                }
        //                            }

        //                            #endregion
        //                            #region Print the optimal solution for 'Y' variables
        //                            Console.WriteLine("\nOptimal Solution for Y Variables:");
        //                            int counter = 0;
        //                            foreach (var employee in rows)
        //                            {
        //                                var TotalNumberOfDays = 0;

        //                                for (int j = 0; j < MaxLeaveBidsPerEmployee[employee]; j++)
        //                                {
        //                                    #region Find ZValue
        //                                    int Zvalue = new int();
        //                                    int Rvalue = new int();
        //                                    var EmployeeCode = employee;
        //                                    var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == EmployeeCode);
        //                                    var BidCode = specificEmployee.LeaveBidDataGridStatic[j].BidCode;
        //                                    Rvalue = RBidsDict_Str.TryGetValue((EmployeeCode, BidCode), out int valueR) ? valueR : Zvalue;
        //                                    #endregion

        //                                    for (int r = 0; r < Rvalue; r++)
        //                                    {
        //                                        Zvalue = ZbidsDict_Str.TryGetValue((EmployeeCode, BidCode, r + 1), out int value) ? value : Zvalue;
        //                                        for (int z = 0; z < Zvalue; z++) //allagh
        //                                        {
        //                                            int bidIndex = j;
        //                                            double yValue = y_plan.ContainsKey((employee, bidIndex, r, z)) ? y_plan[(employee, bidIndex, r, z)] : 0.0;

        //                                            Console.WriteLine($"Employee: {employee}, BidIndex: {bidIndex + 1}, Value: {yValue} -> Employee: {employee}, BidIndex: {bidIndex + 1}, Value: {yValue} Y{(Array.IndexOf(Employees, employee) + 1)}{(bidIndex + 1)}");

        //                                            #region Populate VP Yij
        //                                            VPYijResultsData yijDataRecord = new VPYijResultsData();
        //                                            yijDataRecord.LeaveBidData = new LeaveBidsDataStatic();


        //                                            yijDataRecord.Yij = $"Y{(Array.IndexOf(Employees, employee) + 1)}{(bidIndex + 1)}";
        //                                            yijDataRecord.Rijr = $"R{(Array.IndexOf(Employees, employee) + 1)}{(bidIndex + 1)}{(r + 1)}";
        //                                            yijDataRecord.Yijrz = $"Y{(Array.IndexOf(Employees, employee) + 1)}{(bidIndex + 1)}{(r + 1)}{(z + 1)}";

        //                                            yijDataRecord.YijFlag = yValue;

        //                                            var SpecificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == employee);
        //                                            yijDataRecord.Employee = SpecificEmployee;

        //                                            yijDataRecord.LeaveBidData = SpecificEmployee.LeaveBidDataGridStatic[j];

        //                                            #region Edit Dates

        //                                            var DateFrom = SpecificEmployee.LeaveBidDataGridStatic[j].DateFrom;
        //                                            var NumberOfDays = SpecificEmployee.LeaveBidDataGridStatic[j].NumberOfDaysMax - r;
        //                                            var NumberOfDaysMax = SpecificEmployee.LeaveBidDataGridStatic[j].NumberOfDaysMax;
        //                                            var NumberOfDaysMin = SpecificEmployee.LeaveBidDataGridStatic[j].NumberOfDaysMin;

        //                                            var BidType = SpecificEmployee.LeaveBidDataGridStatic[j].BidType;
        //                                            var DateTo = SpecificEmployee.LeaveBidDataGridStatic[j].DateTo;

        //                                            yijDataRecord.DateFrom = DateFrom;
        //                                            yijDataRecord.DateTo = DateTo;

        //                                            if (BidType == BasicEnums.BidType.Min_Max)
        //                                            {
        //                                                NumberOfDays = 0;
        //                                            }
        //                                            else if (BidType == BasicEnums.BidType.Non_Specific)
        //                                            {
        //                                                NumberOfDaysMax = 0;
        //                                                NumberOfDaysMin = 0;

        //                                            }
        //                                            else if (BidType == BasicEnums.BidType.Specific)
        //                                            {
        //                                                NumberOfDaysMax = 0;
        //                                                NumberOfDaysMin = 0;
        //                                            }

        //                                            yijDataRecord.NumberOfDays = NumberOfDays;
        //                                            yijDataRecord.NumberOfDaysMax = NumberOfDaysMax;
        //                                            yijDataRecord.NumberOfDaysMin = NumberOfDaysMin;

        //                                            yijDataRecord.DateFromStr = DateFrom.ToString("dd/MM/yyyy");
        //                                            yijDataRecord.DateToStr = DateTo.ToString("dd/MM/yyyy");

        //                                            #endregion

        //                                            #region ADD RECORD 
        //                                            var existingRecord = Data.VPYijResultsDataGrid.FirstOrDefault(record => record.Yij == yijDataRecord.Yij);

        //                                            if (existingRecord != null)
        //                                            {
        //                                                if (existingRecord.YijFlag == 1)
        //                                                {

        //                                                }
        //                                                else if (existingRecord.YijFlag == 0 && yijDataRecord.YijFlag == 0)
        //                                                {

        //                                                }
        //                                                else if (existingRecord.YijFlag == 0 && yijDataRecord.YijFlag == 1)
        //                                                {
        //                                                    // Insert the new record and remove the existing record
        //                                                    Data.VPYijResultsDataGrid.Remove(existingRecord);
        //                                                    Data.VPYijResultsDataGrid.Add(yijDataRecord);
        //                                                }
        //                                            }
        //                                            else
        //                                            {
        //                                                Data.VPYijResultsDataGrid.Add(yijDataRecord);

        //                                            }

        //                                            #endregion



        //                                            #endregion

        //                                            #region Populate VP Yijz
        //                                            VPYijResultsData yijzDataRecord = new VPYijResultsData();
        //                                            yijzDataRecord.LeaveBidData = new LeaveBidsDataStatic();


        //                                            yijzDataRecord.Yij = $"Y{(Array.IndexOf(Employees, employee) + 1)}{(bidIndex + 1)}";
        //                                            yijzDataRecord.Rijr = $"R{(Array.IndexOf(Employees, employee) + 1)}{(bidIndex + 1)}{(r + 1)}";
        //                                            yijzDataRecord.Yijrz = $"Y{(Array.IndexOf(Employees, employee) + 1)}{(bidIndex + 1)}{(r + 1)}{(z + 1)}";

        //                                            yijzDataRecord.YijFlag = yValue;

        //                                            yijzDataRecord.Employee = SpecificEmployee;

        //                                            yijzDataRecord.LeaveBidData = SpecificEmployee.LeaveBidDataGridStatic[j];

        //                                            #region Edit Dates


        //                                            DateFrom = SpecificEmployee.LeaveBidDataGridStatic[j].DateFrom.AddDays(z);
        //                                            NumberOfDays = SpecificEmployee.LeaveBidDataGridStatic[j].NumberOfDaysMax - r;
        //                                            DateTo = DateFrom.AddDays(NumberOfDays - 1);

        //                                            yijzDataRecord.DateFrom = DateFrom;
        //                                            yijzDataRecord.DateTo = DateTo;
        //                                            yijzDataRecord.NumberOfDays = NumberOfDays;
        //                                            yijzDataRecord.DateFromStr = DateFrom.ToString("dd/MM/yyyy");
        //                                            yijzDataRecord.DateToStr = DateTo.ToString("dd/MM/yyyy");

        //                                            #endregion
        //                                            Data.VPYijzResultsDataGrid.Add(yijzDataRecord);



        //                                            #endregion
        //                                            if (yValue == 1)
        //                                            {
        //                                                TotalNumberOfDays = TotalNumberOfDays + NumberOfDays;

        //                                            }
        //                                        }
        //                                    } //allagh

        //                                    counter++;
        //                                }

        //                                var UpdatedEmp = InputData.Employees.FirstOrDefault(emp => emp.Code == employee);
        //                                UpdatedEmp.LeaveStatus.ProjectedBalance = UpdatedEmp.LeaveStatus.CurrentBalance - TotalNumberOfDays;
        //                                Data.EmpLeaveStatusData.Add(UpdatedEmp);

        //                            }
        //                            #endregion

        //                            #region Create c#sol.txt for python
        //                            //string filePath = @"C:\Users\npoly\Source\Repos\Bids_CrewScheduling_Kozanidis\c#sol.txt";
        //                            //File.WriteAllText(filePath, string.Empty);

        //                            //using (StreamWriter writer = new StreamWriter(filePath, true)) // 'true' parameter appends to the existing file if it exists
        //                            //{
        //                            //    foreach (string line in outputLines)
        //                            //    {
        //                            //        writer.WriteLine(line);
        //                            //    }
        //                            //}

        //                            #endregion
        //                        }
        //                        #endregion
        //                    }
        //                    if (result == "Save and Upgrade")
        //                    {
        //                        // Handle Save and Upgrade scenario
        //                        Console.WriteLine("Saving and upgrading...");
        //                        //flag = SaveVpVijResultData(Data, 1, InputData.VPId);
        //                        //Console.WriteLine(flag);
        //                        Upgrade = true;
        //                    }
        //                    else
        //                    {
        //                        // Handle Save Only scenario
        //                        Console.WriteLine("Saving only...");
        //                        //flag = SaveVpVijResultData(Data, 1, InputData.VPId);
        //                        //Console.WriteLine(flag);
        //                        Upgrade = false;

        //                    }
        //                }
        //                else
        //                {
        //                    // User clicked Upgrade Only
        //                    Console.WriteLine("User clicked Upgrade Only");
        //                    // Handle Upgrade Only scenario
        //                    //flag = SaveVpVijResultData(Data, -1, InputData.VPId);
        //                    Console.WriteLine(flag);
        //                    Upgrade = true;

        //                }
        //            }
        //            while (Upgrade == true)
        //            {
        //                var CurrentObjectiveValue = Data.ObjValue;
        //                var NewInputData = InputData;


        //                //Data = CalculateVacationPlanningAdvanced2(NewInputData,Yijk);
        //            }
        //        }
        //        model.Dispose();
        //        env.Dispose();

        //        #endregion

        //        return Data;


        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("An error occurred: " + ex.Message);
        //        return Data;
        //    }
        //}
        public VacationPlanningOutputData Calculate_VacationPlanning_Gurobi_String(VacationPlanningInputData InputData)
        {
            GRBEnv env = new GRBEnv("vplogfile.log");
            GRBModel model = new GRBModel(env);
            GRBEnv finalenv = new GRBEnv("vplogfilefinal.log");
            string relativePath = Path.Combine("OptimizationResults", "Gurobi", "Thesis", "Vacation_Planning");
            Directory.CreateDirectory(relativePath);

            VacationPlanningOutputData Data = new VacationPlanningOutputData();
            Data.VPYijResultsDataGrid = new ObservableCollection<VPYijResultsData>();
            Data.VPYijrzResultsDataGrid = new ObservableCollection<VPYijResultsData>();
            Data.VPXitResultsDataGrid = new ObservableCollection<VPXijResultsData>();
            Data.VPLLiResultsDataGrid = new ObservableCollection<VPXiResultData>();

            Data.EmpLeaveStatusData = new ObservableCollection<EmployeeData>();

            List<string> rows = new List<string>();
            List<string> columns = new List<string>();
            Dictionary<(string, string), double> GrantedDays_Dict = new Dictionary<(string, string), double>();
            double bigM = 10000;

            try
            {
                #region Optimization

                #region Optimization paramaters

                int MaxSatisfiedBids = InputData.Bmax; //Max αριθμος ικανοποιημένων Bids ανα υπάλληλο
                int SeparValue = InputData.Se; // Seperation Value

                string[] Employees = InputData.Employees.Select(d => d.Code).ToArray(); //Πινακας με τους Κωδικους Υπαλληλων
                string[] Dates = InputData.DatesStr; //Πινακας με τα Dates




                Dictionary<string, int> MaxLeaveBidsPerEmployee = InputData.MaxLeaveBidsPerEmployee;

                // Zvalue = Number Of Specific    .Για Specific Bids το Zvalue = 1 Παντα
                // Rvalue = Number of NonSpecific . Για Specific,NonSpecific Bids to Rvalue = 1 Παντα

                //ZBidsDict = <(Employee Code, LeaveBidCode ,Rvalue), Zvalue>
                Dictionary<(string, string, int), int> ZbidsDict = InputData.ZBidsDict;

                //RBidsDict = <(Employee Code, LeaveBidCode ),Rvalue>
                Dictionary<(string, string), int> RBidsDict = InputData.RBidsDict;

                #region Print Console
                // Print MaxLeaveBidsPerEmployee dictionary
                Console.WriteLine("MaxLeaveBidsPerEmployee Dictionary:");
                Console.WriteLine("----------------------------------");
                foreach (var kvp in InputData.MaxLeaveBidsPerEmployee)
                {
                    Console.WriteLine($"Employee Code: {kvp.Key}, Max Leave Bids: {kvp.Value}");
                }
                Console.WriteLine();

                // Print ZbidsDict dictionary
                Console.WriteLine("ZbidsDict Dictionary:");
                Console.WriteLine("---------------------");
                Console.WriteLine("Format: <(Employee Code, LeaveBidCode, Rvalue), Zvalue>");
                foreach (var kvp in InputData.ZBidsDict)
                {
                    Console.WriteLine($"Employee Code: {kvp.Key.Item1}, LeaveBidCode: {kvp.Key.Item2}, Rvalue: {kvp.Key.Item3}, Zvalue: {kvp.Value}");
                }
                Console.WriteLine();

                // Print RBidsDict dictionary
                Console.WriteLine("RBidsDict Dictionary:");
                Console.WriteLine("---------------------");
                Console.WriteLine("Format: <(Employee Code, LeaveBidCode), Rvalue>");
                foreach (var kvp in InputData.RBidsDict)
                {
                    Console.WriteLine($"Employee Code: {kvp.Key.Item1}, LeaveBidCode: {kvp.Key.Item2}, Rvalue: {kvp.Value}");
                }


                #endregion


                int MaxLeaveBids = InputData.MaxLeaveBids; //Μεγιστος αριθμός Bids υπαλλήλου απο ολούς τους υπαλλήλους

                int LimitLineFixed = InputData.Schedule.LimitLineFixed; // Σταθερό Limit Line σε όλες τις ημέρες

                int Zmax = ZbidsDict.Max(kvp => kvp.Value); //Μεγιστο Zvalue , το χρειαζόμαστε για την δήλωση της Yijrz
                int MaxNonSpecific = RBidsDict.Max(kvp => kvp.Value); //Μεγιστο Rvalue , το χρειαζόμαστε για την δήλωση της Yijrz

                #endregion

                #region Optimization Algorithm

                #region Decision Variables 
                // Decision variables
                GRBVar[,,,] Y = new GRBVar[Employees.Length, MaxLeaveBids, MaxNonSpecific, Zmax];
                GRBVar[,] X = new GRBVar[Employees.Length, Dates.Length];

                GRBVar[,,] R = new GRBVar[Employees.Length, MaxLeaveBids, MaxNonSpecific];


                // Create decision variables X
                for (int i = 0; i < Employees.Length; i++)
                {
                    for (int t = 0; t < Dates.Length; t++)
                    {
                        // Define the variable name
                        string varNameX = $"X{i + 1}_{t + 1}";

                        // Create the binary variable with a name
                        X[i, t] = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, varNameX);
                    }
                }

                // Create decision variables Y
                for (int i = 0; i < Employees.Length; i++)
                {
                    for (int j = 0; j < MaxLeaveBidsPerEmployee[Employees[i]]; j++)
                    {
                        #region Find ZValue,RValue

                        int Rvalue = new int();
                        int Zvalue = new int();
                        var EmployeeCode = Employees[i];
                        var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees[i]);
                        var BidCode = specificEmployee.LeaveBidDataGridStatic[j].BidCode;
                        Rvalue = RBidsDict.TryGetValue((Employees[i], BidCode), out int valueR) ? valueR : Zvalue;
                        #endregion
                        for (int r = 0; r < Rvalue; r++) //allagh
                        {
                            Zvalue = ZbidsDict.TryGetValue((Employees[i], BidCode, r + 1), out int value) ? value : Zvalue;

                            for (int z = 0; z < Zvalue; z++) //allagh
                            {
                                // Define the variable name
                                string varNameY = $"Y{i + 1}_{j + 1}_{r + 1}_{z + 1}";

                                // Create the binary variable with a name
                                Y[i, j, r, z] = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, varNameY);
                            }

                        }
                    }
                }

                // Create decision variables R
                for (int i = 0; i < Employees.Length; i++)
                {
                    for (int j = 0; j < MaxLeaveBidsPerEmployee[Employees[i]]; j++)
                    {
                        #region Find ZValue,RValue

                        int Rvalue = new int();
                        int Zvalue = new int();
                        var EmployeeCode = Employees[i];
                        var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees[i]);
                        var BidCode = specificEmployee.LeaveBidDataGridStatic[j].BidCode;
                        Rvalue = RBidsDict.TryGetValue((Employees[i], BidCode), out int valueR) ? valueR : Zvalue;
                        #endregion
                        for (int r = 0; r < Rvalue; r++) //allagh
                        {

                            // Define the variable name
                            string varNameR = $"R{i + 1}_{j + 1}_{r + 1}";

                            // Create the binary variable with a name
                            R[i, j, r] = model.AddVar(0.0, 0.0, 0.0, GRB.BINARY, varNameR);

                        }
                    }
                }
                var a = 1;
                #endregion

                #region Objective Function

                GRBLinExpr objective = 0;
                GRBLinExpr SumX = 0;

                for (int i = Employees.Length - 1; i >= 0; i--)
                {
                    for (int j = MaxLeaveBidsPerEmployee[Employees[i]] - 1; j >= 0; j--)
                    {
                        #region Find ZValue,RValue
                        int Rvalue = new int();
                        int Zvalue = new int();
                        var EmployeeCode = Employees[i];
                        var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees[i]);
                        var BidCode = specificEmployee.LeaveBidDataGridStatic[j].BidCode;
                        Rvalue = RBidsDict.TryGetValue((Employees[i], BidCode), out int valueR) ? valueR : Zvalue;
                        #endregion
                        for (int r = 0; r < Rvalue; r++) //allagh
                        {
                            objective.AddTerm(1, R[i, j, r]);

                        }
                    }

                }

                model.SetObjective(objective, GRB.MAXIMIZE);

                #endregion

                #region Constrains
                // #1. Adding constraints for maximum number of satisfied bids 
                for (int i = 0; i < Employees.Length; i++)
                {
                    GRBLinExpr sumLeaveBids = 0;
                    for (int j = 0; j < MaxLeaveBidsPerEmployee[Employees[i]]; j++)
                    {

                        #region Find ZValue ,RValue
                        int Zvalue = new int();
                        int Rvalue = new int();
                        var EmployeeCode = Employees[i];
                        var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees[i]);
                        var BidCode = specificEmployee.LeaveBidDataGridStatic[j].BidCode;
                        Rvalue = RBidsDict.TryGetValue((Employees[i], BidCode), out int valueR) ? valueR : Zvalue;
                        #endregion
                        for (int r = 0; r < Rvalue; r++)
                        {
                            sumLeaveBids.AddTerm(1.0, R[i, j, r]);
                        }


                    }

                    // Adding the constraint for the current employee
                    model.AddConstr(sumLeaveBids <= MaxSatisfiedBids, "MaxSatisfiedBids_" + Employees[i]);
                }


                // #2. Entitlements
                for (int i = 0; i < Employees.Length; i++)
                {
                    var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees[i]);
                    GRBLinExpr sumLeaveBidDays = 0;
                    for (int j = 0; j < MaxLeaveBidsPerEmployee[Employees[i]]; j++)
                    {
                        var NumberOfDays = specificEmployee.LeaveBidDataGridStatic[j].NumberOfDaysMax;
                        #region Find ZValue ,RValue
                        int Zvalue = new int();
                        int Rvalue = new int();
                        var EmployeeCode = Employees[i];
                        var BidCode = specificEmployee.LeaveBidDataGridStatic[j].BidCode;
                        Rvalue = RBidsDict.TryGetValue((Employees[i], BidCode), out int valueR) ? valueR : Zvalue;
                        #endregion
                        for (int r = 0; r < Rvalue; r++)
                        {
                            Zvalue = ZbidsDict.TryGetValue((Employees[i], BidCode, r + 1), out int value) ? value : Zvalue;

                            NumberOfDays = NumberOfDays - r;
                            for (int z = 0; z < Zvalue; z++)
                            {
                                sumLeaveBidDays.AddTerm(NumberOfDays, Y[i, j, r, z]); // Summing up the leavebidsDays for each employee
                            }
                        }


                    }

                    var MaxLeaveDays = specificEmployee.LeaveStatus.CurrentBalance;

                    // Adding the constraint for the current employee
                    model.AddConstr(sumLeaveBidDays <= MaxLeaveDays, "MaxLeaveDays_" + Employees[i]);


                }

                // #3. Limit Lines

                for (int t = 0; t < Dates.Length; t++)
                {
                    GRBLinExpr expr = 0;
                    int sumdays = 0;
                    //Ξεχωριστό LimitLine για κάθε ημέρα 
                    var LimitLine = InputData.Schedule.ReqScheduleRowsData.ElementAt(t).LimitLine;

                    for (int i = 0; i < Employees.Length; i++)
                    {

                        expr.AddTerm(1, X[i, t]);
                        sumdays = sumdays + 1;

                    }
                    if (sumdays > LimitLine)
                    {
                        model.AddConstr(expr <= LimitLine, "LimitLine_" + Dates[t]);
                    }
                }

                //#5. Overlapping
                #region  OverLapping

                for (int i = 0; i < Employees.Length; i++)
                {
                    for (int j1 = 0; j1 < MaxLeaveBidsPerEmployee[Employees[i]] - 1; j1++)
                    {
                        for (int j2 = j1 + 1; j2 < MaxLeaveBidsPerEmployee[Employees[i]]; j2++)
                        {
                            var EmployeeCode = Employees[i];
                            var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees[i]);

                            #region Find z1,z2
                            int Z1value = new int();
                            int Z2value = new int();
                            int R1value = new int();
                            int R2value = new int();

                            var BidCode1 = specificEmployee.LeaveBidDataGridStatic[j1].BidCode;
                            R1value = RBidsDict.TryGetValue((Employees[i], BidCode1), out int valueR1) ? valueR1 : R1value;

                            var BidCode2 = specificEmployee.LeaveBidDataGridStatic[j2].BidCode;
                            R2value = RBidsDict.TryGetValue((Employees[i], BidCode2), out int valueR2) ? valueR2 : R2value;
                            #endregion

                            for (int r1 = 0; r1 < R1value; r1++)
                            {
                                Z1value = ZbidsDict.TryGetValue((Employees[i], BidCode1, r1 + 1), out int value1) ? value1 : Z1value;

                                for (int r2 = 0; r2 < R2value; r2++)
                                {
                                    Z2value = ZbidsDict.TryGetValue((Employees[i], BidCode2, r2 + 1), out int value2) ? value2 : Z2value;

                                    for (int z1 = 0; z1 < Z1value; z1++)
                                    {
                                        for (int z2 = 0; z2 < Z2value; z2++)
                                        {


                                            if (SeparOrOverlap(i, j1, j2, z1, z2, r1, r2))
                                            {
                                                GRBLinExpr expr = Y[i, j1, r1, z1] + Y[i, j2, r2, z2];
                                                model.AddConstr(expr <= 1, $"SO{i + 1}_{j1 + 1}_{z1 + 1}_{j2 + 1}_{z2 + 1}");
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
                //#6.Connection Between Y and X
                for (int i = 0; i < Employees.Length; i++)
                {
                    var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees[i]);
                    var maxBids = MaxLeaveBidsPerEmployee[Employees[i]];

                    for (int j = 0; j < maxBids; j++)
                    {
                        var bid = specificEmployee.LeaveBidDataGridStatic[j];
                        var NumberOfDays = bid.NumberOfDaysMax;

                        #region Find ZValue, RValue
                        int Zvalue = new int();
                        int Rvalue = new int();
                        var EmployeeCode = Employees[i];
                        var BidCode = specificEmployee.LeaveBidDataGridStatic[j].BidCode;
                        Rvalue = RBidsDict.TryGetValue((Employees[i], BidCode), out int valueR) ? valueR : Zvalue;
                        #endregion
                        GRBLinExpr sumDays = 0;
                        for (int r = 0; r < Rvalue; r++) //allagh
                        {
                            Zvalue = ZbidsDict.TryGetValue((Employees[i], BidCode, r + 1), out int value) ? value : Zvalue;
                            NumberOfDays = NumberOfDays - r;
                            for (int z = 0; z < Zvalue; z++) //allagh
                            {

                                var startDateIndex = Array.IndexOf(Dates, bid.DateFrom.ToString("dd/MM/yyyy"));

                                GRBLinExpr expr = 0;
                                expr = Y[i, j, r, z] * NumberOfDays;

                                int start = startDateIndex + z;
                                int end = start + NumberOfDays;

                                for (int t = start; t < end; t++)
                                {
                                    expr = expr - X[i, t]; // Add X variables for each day of the bid
                                }
                                // Add the constraint
                                model.AddConstr(expr <= 0, $"BidDaysConstraint_{Employees[i]}_{j}_{r}_{z}");


                            }

                        }


                    }
                }

                //#7.Connection Betweer Y and R -- Yijrz and Yijr . 
                for (int i = 0; i < Employees.Length; i++)
                {
                    for (int j = 0; j < MaxLeaveBidsPerEmployee[Employees[i]]; j++)
                    {
                        #region Find ZValue ,RValue
                        int Zvalue = new int();
                        int Rvalue = new int();
                        var EmployeeCode = Employees[i];
                        var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees[i]);
                        var BidCode = specificEmployee.LeaveBidDataGridStatic[j].BidCode;
                        Rvalue = RBidsDict.TryGetValue((Employees[i], BidCode), out int valueR) ? valueR : Zvalue;
                        #endregion
                        for (int r = 0; r < Rvalue; r++)
                        {
                            Zvalue = ZbidsDict.TryGetValue((Employees[i], BidCode, r + 1), out int value) ? value : Zvalue;
                            GRBLinExpr sumYijrz = 0;
                            for (int z = 0; z < Zvalue; z++)
                            {
                                sumYijrz.AddTerm(1.0, Y[i, j, r, z]);

                            }
                            // Adding the constraint for the current employee
                            model.AddConstr(R[i, j, r] == sumYijrz, "Y_R_Connection" + Employees[i]);
                        }


                    }


                }

                #endregion

                #endregion

                #region New Optimize settings
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


                model.Update();

                while (FinishedEmpIds <= numOfEmployes)
                {


                    int j = NextBid[id];

                    #region Check Bid
                    var z = 0;
                    var r = 0;
                    #region Find ZValue,RValue
                    int Rvalue = new int();
                    var EmployeeCode = Employees[id];
                    var specificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == Employees[id]);

                    var BidCode = specificEmployee.LeaveBidDataGridStatic[j].BidCode;
                    Rvalue = RBidsDict.TryGetValue((Employees[id], BidCode), out int valueR) ? valueR : Rvalue;
                    #endregion
                    for (r = 0; r < Rvalue; r++)
                    {
                        #region Check Bid


                        GRBVar Rijr = model.GetVarByName($"R{id + 1}_{j + 1}_{r + 1}");
                        Rijr.LB = 1;
                        Rijr.UB = 1;

                        model.Update();
                        model.Optimize();
                        bool solution = (model.Status == GRB.Status.OPTIMAL);
                        if (solution)
                        {
                            double rValue = R[id, j, r].X;

                            grant = true;
                            string message = $"Crew member {id + 1} was awarded bid {j + 1}";
                            Console.WriteLine(message);
                            outputLines.Add(message);
                            Data.ObjValue = model.ObjVal;
                            Rvalue = 0;
                        }
                        else
                        {
                            grant = false;
                            Rijr.LB = 0;
                            Rijr.UB = 0;

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
                    if (model.Status == GRB.Status.OPTIMAL)
                    {
                        Data.ObjValue = model.ObjVal;
                        model.Update();



                        // Save the files
                        model.Write(Path.Combine(relativePath, "VP.mst"));
                        model.Write(Path.Combine(relativePath, "VP.sol"));
                        model.Write(Path.Combine(relativePath, "VP.lp"));
                        model.Write(Path.Combine(relativePath, "VP.mps"));

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
                var customMessageBox = new CustomMessageBox("Do you want to Save the Results ");
                if (customMessageBox.ShowDialog() == true)
                {
                    // User clicked Save Only or Save and Upgrade
                    if (customMessageBox.DialogResult == true)
                    {
                        // User clicked Save Only or Save and Upgrade
                        //var result = customMessageBox.Message.Contains("Save and Upgrade") ? "Save and Upgrade" : "Save Only";
                        var result = "Save";
                        Console.WriteLine($"User clicked {result}");

                        if (result == "Save")
                        {
                            #region OutputResults
                            if (Data.ObjValue > 0)
                            {

                                // Read the model from the files
                                using (GRBModel modelFromFiles = new GRBModel(finalenv, Path.Combine(relativePath, "VP.mps")))
                                {
                                    modelFromFiles.Optimize();
                                    modelFromFiles.Update();
                                    Data.ObjValue = modelFromFiles.ObjVal;
                                    //Data.ObjValue = Math.Round(Data.ObjValue);
                                    #region Extract Xij
                                    // Extract the optimal solution for the 'X' variables
                                    for (int i = 0; i < Employees.Length; i++)
                                    {
                                        for (int t = 0; t < Dates.Length; t++)
                                        {
                                            string employee = Employees[i];
                                            string date = Dates[t];
                                            GRBVar Xit = modelFromFiles.GetVarByName($"X{i + 1}_{t + 1}");

                                            double xValue = 0;
                                            if (xValue == 1)
                                            {
                                                Console.WriteLine($"Employee: {employee}, Date: {date}, Value: {xValue}");
                                            }
                                            // Store the optimal 'X' value in the data structure
                                            GrantedDays_Dict[(employee, date)] = xValue;

                                            // Add 'employee' and 'date' to the respective lists if they are not already there
                                            if (!rows.Contains(employee))
                                                rows.Add(employee);
                                            if (!columns.Contains(date))
                                                columns.Add(date);
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

                                            #region Find ZValue
                                            int Zvalue = new int();
                                            int Rvalue = new int();
                                            var EmployeeCode = Employees[i];
                                            var BidCode = specificEmployee.LeaveBidDataGridStatic[j].BidCode;
                                            Rvalue = RBidsDict.TryGetValue((Employees[i], BidCode), out int valueR) ? valueR : Zvalue;
                                            #endregion
                                            for (int r = 0; r < Rvalue; r++)
                                            {
                                                Zvalue = ZbidsDict.TryGetValue((Employees[i], BidCode, r + 1), out int value) ? value : Zvalue;
                                                GRBVar Rijr = modelFromFiles.GetVarByName($"R{i + 1}_{j + 1}_{r + 1}");

                                                double rValue = Rijr.X;


                                                for (int z = 0; z < Zvalue; z++) //allagh
                                                {
                                                    string employee = Employees[i];
                                                    int bidIndex = j;
                                                    GRBVar Yijrz = modelFromFiles.GetVarByName($"Y{i + 1}_{j + 1}_{r + 1}_{z + 1}");

                                                    double yValue = Yijrz.X;

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
                                            Rvalue = RBidsDict.TryGetValue((EmployeeCode, BidCode), out int valueR) ? valueR : Zvalue;
                                            #endregion

                                            for (int r = 0; r < Rvalue; r++)
                                            {
                                                Zvalue = ZbidsDict.TryGetValue((EmployeeCode, BidCode, r + 1), out int value) ? value : Zvalue;
                                                for (int z = 0; z < Zvalue; z++) //allagh
                                                {
                                                    int bidIndex = j;
                                                    double yValue = y_plan.ContainsKey((employee, bidIndex, r, z)) ? y_plan[(employee, bidIndex, r, z)] : 0.0;

                                                    Console.WriteLine($"Employee: {employee}, BidIndex: {bidIndex + 1}, Value: {yValue} -> Employee: {employee}, BidIndex: {bidIndex + 1}, Value: {yValue} Y{(Array.IndexOf(Employees, employee) + 1)}{(bidIndex + 1)}");

                                                    #region Populate VP Yij
                                                    VPYijResultsData yijDataRecord = new VPYijResultsData();
                                                    yijDataRecord.LeaveBidData = new LeaveBidsDataStatic();


                                                    yijDataRecord.Yij = $"Y{(Array.IndexOf(Employees, employee) + 1)}{(bidIndex + 1)}";
                                                    yijDataRecord.Rijr = $"R{(Array.IndexOf(Employees, employee) + 1)}{(bidIndex + 1)}{(r + 1)}";
                                                    yijDataRecord.Yijrz = $"Y{(Array.IndexOf(Employees, employee) + 1)}{(bidIndex + 1)}{(r + 1)}{(z + 1)}";

                                                    yijDataRecord.YijFlag = yValue;

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
                                                    yijzDataRecord.Rijr = $"R{(Array.IndexOf(Employees, employee) + 1)}{(bidIndex + 1)}{(r + 1)}";
                                                    yijzDataRecord.Yijrz = $"Y{(Array.IndexOf(Employees, employee) + 1)}{(bidIndex + 1)}{(r + 1)}{(z + 1)}";

                                                    yijzDataRecord.YijFlag = yValue;

                                                    yijzDataRecord.Employee = SpecificEmployee;

                                                    yijzDataRecord.LeaveBidData = SpecificEmployee.LeaveBidDataGridStatic[j];

                                                    #region Change Dates Format , Fill GrantedDays_Dict


                                                    DateFrom = SpecificEmployee.LeaveBidDataGridStatic[j].DateFrom.AddDays(z);
                                                    NumberOfDays = SpecificEmployee.LeaveBidDataGridStatic[j].NumberOfDaysMax - r;
                                                    DateTo = DateFrom.AddDays(NumberOfDays - 1);

                                                    yijzDataRecord.DateFrom = DateFrom;
                                                    yijzDataRecord.DateTo = DateTo;
                                                    yijzDataRecord.NumberOfDays = NumberOfDays;
                                                    yijzDataRecord.DateFromStr = DateFrom.ToString("dd/MM/yyyy");
                                                    yijzDataRecord.DateToStr = DateTo.ToString("dd/MM/yyyy");

                                                    #region Insert Dates To List
                                                    if (yValue == 1)
                                                    {
                                                        for (DateTime date = DateFrom; date <= DateTo; date = date.AddDays(1))
                                                        {
                                                            var SelectedDate_Str = date.ToString("dd/MM/yyyy");

                                                            GrantedDays_Dict[(SpecificEmployee.Code, SelectedDate_Str)] = 1;

                                                        }
                                                    }
                                                    #endregion

                                                    #endregion
                                                    Data.VPYijrzResultsDataGrid.Add(yijzDataRecord);



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

                                    #region Insert the optimal solution for 'X' variables
                                    Console.WriteLine("Optimal Solution for X Variables:");
                                    foreach (var employee in rows)
                                    {
                                        foreach (var date in columns)
                                        {
                                            double xValue = GrantedDays_Dict.ContainsKey((employee, date)) ? GrantedDays_Dict[(employee, date)] : 0.0;
                                            Console.WriteLine($"Employee: {employee}, Date: {date}, Value: {xValue} -> Employee: {employee}, Date: {date}, Value: {xValue} X{(Array.IndexOf(Employees, employee) + 1)}{(Array.IndexOf(Dates, date) + 1)}");


                                            #region Populate VP Xij
                                            VPXijResultsData singleDataRecord = new VPXijResultsData();


                                            singleDataRecord.Xit = $"X{(Array.IndexOf(Employees, employee) + 1)}{(Array.IndexOf(Dates, date) + 1)}";
                                            singleDataRecord.XitFlag = xValue;
                                            singleDataRecord.Date = date;




                                            var SpecificEmployee = InputData.Employees.FirstOrDefault(emp => emp.Code == employee);
                                            singleDataRecord.Employee = SpecificEmployee;

                                            Data.VPXitResultsDataGrid.Add(singleDataRecord);
                                            #endregion
                                        }

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
                                Console.WriteLine("Saving and upgrading...");
                                //flag = SaveVpVijResultData(Data, 1, InputData.VPId);
                                //Console.WriteLine(flag);
                                Upgrade = true;
                            }
                            else
                            {
                                // Handle Save Only scenario
                                Console.WriteLine("Saving only...");
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
                model.Dispose();
                env.Dispose();

                #endregion

                return Data;


            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return Data;
            }
        }

        public bool SaveVpVijResultData(VacationPlanningOutputData Data, int ReplicationNumber, int VPID)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {

                    foreach (var row in Data.VPYijrzResultsDataGrid)
                    {
                        VPYijzResultsDataEntity newRow = new VPYijzResultsDataEntity();

                        var BidId = dbContext.LeaveBids.SingleOrDefault(b => b.BidCode == row.LeaveBidData.BidCode).BidId;
                        newRow.BidId = BidId;
                        newRow.EmpId = row.Employee.EmployeeId;
                        newRow.VPID = VPID;
                        newRow.Yij = row.Yij;
                        newRow.Yijr = row.Rijr;
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
            catch (Exception ex)
            {
                LogError(ex, "SaveVpVijResultData", "Notes");
                return false;
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
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return -1;
            }
        }

        public int CreatePythonTxt(VacationPlanningInputData InputData)
        {

            string[] Employees = InputData.Employees.Select(d => d.Code).ToArray();

            int MaxLeaveBids = InputData.MaxLeaveBids;
            int MaxSatisfiedBids = InputData.Bmax;
            string[] Dates = InputData.DatesStr;
            int SeparValue = InputData.Se;
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
            GRBEnv env = new GRBEnv("vpcglogfile.log");
            GRBModel model = new GRBModel(env);
            GRBEnv finalenv = new GRBEnv("vpcglogfile_final.log");
            VPCGOutputData Data = new VPCGOutputData();
            //Data.VPYijResultsDataGrid = new ObservableCollection<VPYijResultsData>();
            //Data.VPYijzResultsDataGrid = new ObservableCollection<VPYijResultsData>();


            List<string> rows = new List<string>();
            List<string> columns = new List<string>();

            try
            {
                #region Optimization

                #region Optimization paramaters

                string[] Dates = InputData.Dates; //Πινακας με τα Dates
                Dictionary<int, int> LeaveDays = InputData.Re_Dict;
                Dictionary<int, int> LLiDict = InputData.RLLt_Dict;

                #endregion


                #region Decision Variables
                // Decision variables

                GRBVar[] X = new GRBVar[LeaveDays.Count];

                for (int i = 0; i < LeaveDays.Count; i++)
                {
                    // Define the variable name
                    string varNameX = $"X{i + 1}";

                    // Create the binary variable with a name
                    X[i] = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, varNameX);
                }

                #endregion

                #region Objective Function

                GRBLinExpr objective = 0;

                for (int i = 0; i < LeaveDays.Count; i++)
                {
                    int Multiplier = LeaveDays[i + 1] * 1000;
                    objective.AddTerm(Multiplier, X[i]);
                }

                model.SetObjective(objective, GRB.MINIMIZE);

                #endregion

                #region Constrains

                // #1. MC Employees
                for (int i = 0; i < LeaveDays.Count; i++)
                {
                    GRBLinExpr expr = 0;
                    expr.AddTerm(1, X[i]);
                    model.AddConstr(expr, GRB.EQUAL, 1, "MC_" + (i + 1));
                }

                var a = 1;

                // #2. Days
                for (int t = 0; t < Dates.Length; t++)
                {
                    GRBLinExpr expr = 0;


                    model.AddConstr(expr >= 0, "Day_" + (t + 1));
                    //model.AddConstr(expr >= 0, "Day_" + Dates[t]);

                }
                var b = 1;

                // #3. Limit Lines
                for (int t = 0; t < Dates.Length; t++)
                {
                    GRBLinExpr expr = 0;



                    model.AddConstr(expr <= LLiDict[t + 1], "LimitLine_" + (t + 1));

                }
                #endregion

                #endregion


                model.Update();
                model.Optimize();
                bool solution = (model.Status == GRB.Status.OPTIMAL);
                if (solution)
                {
                    Data.ObjValue = model.ObjVal;
                    model.Update();
                    string relativePath = Path.Combine("OptimizationResults", "Gurobi", "Thesis", "VP_Column_Generation");
                    Directory.CreateDirectory(relativePath);

                    // Save the files
                    model.Write(Path.Combine(relativePath, "VP_CG.mst"));
                    model.Write(Path.Combine(relativePath, "VP_CG.sol"));
                    model.Write(Path.Combine(relativePath, "VP_CG.lp"));
                    model.Write(Path.Combine(relativePath, "VP_CG.mps"));


                }
                return Data;

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return Data;
            }

        }
        #endregion

        #endregion

        #region Extra

        #region Certification
        public int SaveCertificationData(CertificationData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    int Id = flatData.Id;
                    var existingQuery = dbContext.Certifications.Where(c => c.CertID == Id);
                    var existing = existingQuery.SingleOrDefault();
                    // Execute the query and get the result

                    if (existing != null)
                    {


                        // Update existing customer
                        existing.Code = flatData.Code;
                        existing.Descr = flatData.Descr;
                        existing.ValidityPeriod = flatData.ValidityPeriod;
                        existing.CertPosition = flatData.CertPosition.ToString();
                        existing.ValidityTimeBucket = flatData.ValidityTimeBucket.ToString();
                        existing.DateFrom = flatData.DateFrom;
                        existing.DateTo = flatData.DateTo;



                        existing.IsDeleted = flatData.IsDeleted;

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
                LogError(ex, "SaveCertificationData", "Notes");
                return -1;
            }
        }
        public int AddCertificationData(CertificationData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    var existingItemQuery = dbContext.Certifications.Where(r => r.Code == flatData.Code);
                    var existingItem = existingItemQuery.SingleOrDefault();
                    // Execute the query and get the result


                    if (existingItem == null)
                    {
                        var newItem = new CertificationsDataEntity();
                        // Insert new item
                        newItem.Code = flatData.Code;
                        newItem.Descr = flatData.Descr;
                        newItem.ValidityPeriod = 1;
                        newItem.CertPosition = BasicEnums.CertPosition.PNT.ToString();
                        newItem.ValidityTimeBucket = BasicEnums.Timebucket.Yearly.ToString();
                        newItem.DateFrom = DateTime.Now;
                        newItem.DateTo = DateTime.Now.AddYears(1);



                        newItem.IsDeleted = false;


                        dbContext.Certifications.Add(newItem);

                        dbContext.SaveChanges();
                        return 0;
                    }
                    else
                    {
                        // Else Print messages
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "AddCertificationData", "Notes");
                return 2;

            }
        }

        public CertificationData GetCertificationChooserData(int Id, string Code)
        {
            CertificationData FlatData = new CertificationData();
            string FilterStr = "";
            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;

                    if (Id > 0)
                    {
                        command.Parameters.AddWithValue("@ID", Id);
                        FilterStr = String.Format(@" and Certifications.CertID =@ID");

                    }

                    else if (!string.IsNullOrWhiteSpace(Code))
                    {
                        command.Parameters.AddWithValue("@Code", Code);
                        FilterStr = String.Format(@" and Certifications.Code =@Code");

                    }
                    command.CommandText = string.Format(@"select CertID,Code,Descr,ValidityPeriod,ValidityTimeBucket,DateFrom,
                                              DateTo,CertPosition,IsDeleted
                                              from Certifications
                                              Where 1=1 {0}", FilterStr);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            CertificationData data = new CertificationData();

                            data.Id = int.Parse(reader["CertId"].ToString());
                            data.Code = reader["Code"].ToString();
                            data.Descr = reader["Descr"].ToString();
                            data.ValidityPeriod = int.Parse(reader["ValidityPeriod"].ToString());

                            data.CertPosition = (BasicEnums.CertPosition)Enum.Parse(typeof(BasicEnums.CertPosition), reader["CertPosition"].ToString());
                            data.ValidityTimeBucket = (BasicEnums.Timebucket)Enum.Parse(typeof(BasicEnums.Timebucket), reader["ValidityTimeBucket"].ToString());

                            data.DateFrom = DateTime.Parse(reader["DateFrom"].ToString());
                            data.DateTo = DateTime.Parse(reader["DateTo"].ToString());


                            data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());



                        }
                    }

                    connection.Close();
                }

                return FlatData;
            }
            catch (Exception ex)
            {
                LogError(ex, "GetCertificationChooserData", "Notes");
                return null;
            }
        }
        public ObservableCollection<CertificationData> GetCertificationData(bool ShowDeleted)
        {
            ObservableCollection<CertificationData> DataList = new ObservableCollection<CertificationData>();

            string FilterStr = "";

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = String.Format(@" and Certifications.IsDeleted =@ShowDeleted");
                }
                command.CommandText = string.Format(@"select CertID,Code,Descr,ValidityPeriod,ValidityTimeBucket,DateFrom,
                                              DateTo,CertPosition,IsDeleted
                                              from Certifications
                                              Where 1=1 {0}", FilterStr);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CertificationData data = new CertificationData();




                        data.Id = int.Parse(reader["CertId"].ToString());
                        data.Code = reader["Code"].ToString();
                        data.Descr = reader["Descr"].ToString();
                        data.ValidityPeriod = int.Parse(reader["ValidityPeriod"].ToString());

                        data.CertPosition = (BasicEnums.CertPosition)Enum.Parse(typeof(BasicEnums.CertPosition), reader["CertPosition"].ToString());
                        data.ValidityTimeBucket = (BasicEnums.Timebucket)Enum.Parse(typeof(BasicEnums.Timebucket), reader["ValidityTimeBucket"].ToString());

                        data.DateFrom = DateTime.Parse(reader["DateFrom"].ToString());
                        data.DateTo = DateTime.Parse(reader["DateTo"].ToString());


                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());




                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }
        #endregion
        #endregion

        #endregion

        #region Crew Scheduling File 

        #region Country

        public ObservableCollection<CountryData> GetCountryData(bool ShowDeleted)
        {
            ObservableCollection<CountryData> DataList = new ObservableCollection<CountryData>();

            string FilterStr = "";

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = String.Format(@" and IsDeleted =@ShowDeleted");
                }
                command.CommandText = string.Format(@"select CountryId,CountryCode,CountryDescr,IsDeleted from Country Where 1=1 {0}", FilterStr);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CountryData data = new CountryData();

                        data.CountryId = int.Parse(reader["CountryId"].ToString());
                        data.CountryCode = reader["CountryCode"].ToString();
                        data.CountryDescr = reader["CountryDescr"].ToString();
                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());

                        DataList.Add(data);
                    }
                }

                connection.Close();

            }

            return DataList;
        }


        public bool SaveCountryData(ObservableCollection<CountryData> Data)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {


                    bool hasChanges = false;
                    foreach (var row in Data)
                    {
                        var existingrow = dbContext.Country.SingleOrDefault(b => b.CountryId == row.CountryId);

                        if (existingrow == null)
                        {
                            CountryDataEntity newrow = new CountryDataEntity();
                            newrow.CountryCode = row.CountryCode;
                            newrow.CountryDescr = row.CountryDescr;
                            newrow.IsDeleted = false;
                            dbContext.Country.Add(newrow);
                            hasChanges = true;
                        }
                        else if (existingrow != null)
                        {

                            existingrow.CountryCode = row.CountryCode;
                            existingrow.CountryDescr = row.CountryDescr;
                            existingrow.IsDeleted = row.IsDeleted;

                            hasChanges = true;



                        }


                    }

                    if (hasChanges)
                    {
                        dbContext.SaveChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "SaveDemandForecast", "Notes");
                return false;
            }
        }



        #endregion

        #region Prefecture
        public ObservableCollection<PrefectureData> GetPrefectureData()
        {
            ObservableCollection<PrefectureData> DataList = new ObservableCollection<PrefectureData>();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = string.Format(@"select Prefecture.PrefId,Prefecture.PrefCode,Prefecture.PrefDescr ,Country.CountryCode,Country.CountryId,Country.CountryDescr
                                                      from Prefecture 
                                                      Inner JOIN  Country on Prefecture.CountryId = Country.CountryId");
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PrefectureData data = new PrefectureData();
                        data.PrefId = int.Parse(reader["PrefId"].ToString());
                        data.PrefCode = reader["PrefCode"].ToString();
                        data.PrefDescr = reader["PrefDescr"].ToString();

                        data.CountryId = int.Parse(reader["CountryId"].ToString());
                        data.CountryCode = reader["CountryCode"].ToString();

                        DataList.Add(data);
                    }
                }

                connection.Close();

            }

            return DataList;
        }

        public bool SavePrefectureData(ObservableCollection<PrefectureData> Data)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {


                    bool hasChanges = false;
                    foreach (var row in Data)
                    {
                        var existingrow = dbContext.Prefecture.SingleOrDefault(b => b.PrefId == row.PrefId);

                        if (existingrow == null)
                        {
                            dbContext.Prefecture.Add(new PrefectureEntity
                            {
                                PrefCode = row.PrefCode,
                                PrefDescr = row.PrefDescr,
                                CountryId = dbContext.Country.SingleOrDefault(b => b.CountryCode == row.CountryCode).CountryId,
                                IsDeleted = false


                            });
                            hasChanges = true;
                        }
                        else if (existingrow != null)
                        {
                            existingrow.PrefCode = row.PrefCode;
                            existingrow.PrefDescr = row.PrefDescr;
                            existingrow.IsDeleted = row.IsDeleted;

                            int CountryId = dbContext.Country.SingleOrDefault(b => b.CountryCode == row.CountryCode).CountryId;

                            existingrow.CountryId = CountryId;
                            hasChanges = true;



                        }


                    }

                    if (hasChanges)
                    {
                        dbContext.SaveChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "SaveDemandForecast", "Notes");
                return false;
            }
        }
        #endregion

        #region City
        public ObservableCollection<CityData> GetCityData(bool ShowDeleted)
        {
            ObservableCollection<CityData> DataList = new ObservableCollection<CityData>();

            string FilterStr = "";

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = String.Format(@" and City.IsDeleted =@ShowDeleted");
                }
                command.CommandText = string.Format(@"select City.CityId,City.CityCode,City.CityDescr ,Prefecture.PrefId,Prefecture.PrefCode,Prefecture.PrefDescr ,Country.CountryId,Country.CountryCode ,Country.CountryDescr,City.Longitude,
City.Latitude,City.Population,City.IsDeleted
                                                    from City 
                                                    Inner Join Prefecture on CiTY.PrefId = Prefecture.PrefId
                                                    Inner JOIN  Country on Prefecture.CountryId = Country.CountryId Where 1=1 {0}", FilterStr);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CityData data = new CityData();
                        data.CityId = int.Parse(reader["CityId"].ToString());
                        data.PrefId = int.Parse(reader["PrefId"].ToString());
                        data.CountryId = int.Parse(reader["CountryId"].ToString());


                        data.CityCode = reader["CityCode"].ToString();
                        data.CityDescr = reader["CityDescr"].ToString();

                        data.PrefCode = reader["PrefCode"].ToString();
                        data.PrefDescr = reader["PrefDescr"].ToString();
                        data.CountryCode = reader["CountryCode"].ToString();
                        data.CountryDescr = reader["CountryDescr"].ToString();
                        data.Longitude = float.Parse(reader["Longitude"].ToString());
                        data.Latitude = float.Parse(reader["Latitude"].ToString());
                        data.Longitude = (float)Math.Round(data.Longitude, 4);
                        data.Latitude = (float)Math.Round(data.Latitude, 4);
                        data.Population = int.Parse(reader["Population"].ToString());
                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());


                        DataList.Add(data);
                    }
                }

                connection.Close();

            }

            return DataList;
        }

        public bool SaveCityData(ObservableCollection<CityData> Data)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Get list of CityIds from the database

                    foreach (var row in Data)
                    {
                        var existingrow = dbContext.City.SingleOrDefault(b => b.CityId == row.CityId);




                        if (existingrow == null)
                        {
                            // Insert new city
                            CityDataEntity newCity = new CityDataEntity
                            {
                                CityCode = row.CityCode,
                                CityDescr = row.CityDescr,
                                PrefId = row.PrefId,
                                Longitude = row.Longitude,
                                Latitude = row.Latitude,
                                Population = row.Population,
                                IsDeleted = false

                            };

                            dbContext.City.Add(newCity);
                        }
                        else if (existingrow != null)
                        {
                            // Update existing city
                            var existingCity = dbContext.City.Single(c => c.CityId == row.CityId);

                            existingCity.CityCode = row.CityCode;
                            existingCity.CityDescr = row.CityDescr;
                            existingCity.PrefId = row.PrefId;
                            existingCity.Longitude = row.Longitude;
                            existingCity.Latitude = row.Latitude;
                            existingCity.Population = row.Population;
                            existingCity.IsDeleted = row.IsDeleted;

                        }
                    }


                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "SaveCityData");
                return false;
            }
        }
        #endregion

        #region Airports
        public int SaveAirportsData(AirportData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    int Id = flatData.Id;
                    var existingQuery = dbContext.Airports.Where(c => c.AirportID == Id);
                    var existing = existingQuery.SingleOrDefault();
                    // Execute the query and get the result
                    var CityQuery = dbContext.City.Where(c => c.CityId == flatData.City.CityId);
                    var City = CityQuery.SingleOrDefault();

                    if (existing != null)
                    {


                        // Update existing customer
                        existing.AirportCode = flatData.Code;
                        existing.AirportDescr = flatData.Descr;

                        existing.CityId = City.CityId;

                        existing.IsDeleted = flatData.IsDeleted;

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
                LogError(ex, "SaveAirportsData", "Notes");
                return -1;
            }
        }

        public int AddAirportsData(AirportData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    var existingItemQuery = dbContext.Airports.Where(r => r.AirportCode == flatData.Code);
                    var existingItem = existingItemQuery.SingleOrDefault();
                    // Execute the query and get the result


                    if (existingItem == null)
                    {
                        var newItem = new AirportsDataEntity();
                        // Insert new item
                        newItem.AirportCode = flatData.Code;
                        newItem.AirportDescr = flatData.Descr;
                        newItem.CityId = dbContext.City.FirstOrDefault().CityId;
                        newItem.IsDeleted = false;

                        dbContext.Airports.Add(newItem);

                        dbContext.SaveChanges();
                        return 0;
                    }
                    else
                    {
                        // Else Print messages
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "AddAirportsData", "Notes");
                return 2;

            }
        }

        public AirportData GetAirportsChooserData(int Id, string Code)
        {
            AirportData FlatData = new AirportData();
            string FilterStr = "";
            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;

                    if (Id > 0)
                    {
                        command.Parameters.AddWithValue("@ID", Id);
                        FilterStr = String.Format(@" and A.AirportID =@ID");

                    }

                    else if (!string.IsNullOrWhiteSpace(Code))
                    {
                        command.Parameters.AddWithValue("@Code", Code);
                        FilterStr = String.Format(@" and A.AirportCode =@Code");

                    }
                    command.CommandText = string.Format(@"SELECT A.AirportID,A.AirportCode,A.AirportDescr,A.IsDeleted,
City.CityId,City.CityCode,City.CityDescr
FROM Airports AS A
INNER JOIN City ON City.CityId = A.CityId
                                              Where 1=1 {0}", FilterStr);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            AirportData data = new AirportData();
                            data.City = new CityData();


                            data.Id = int.Parse(reader["AirportID"].ToString());
                            data.Code = reader["AirportCode"].ToString();
                            data.Descr = reader["AirportDescr"].ToString();

                            data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());


                            data.City.CityId = int.Parse(reader["CityId"].ToString());
                            data.City.CityCode = reader["CityCode"].ToString();
                            data.City.CityDescr = reader["CityDescr"].ToString();
                        }
                    }

                    connection.Close();
                }

                return FlatData;
            }
            catch (Exception ex)
            {
                LogError(ex, "GetAirportsChooserData", "Notes");
                return null;
            }
        }
        public ObservableCollection<AirportData> GetAirportsData(bool ShowDeleted)
        {
            ObservableCollection<AirportData> DataList = new ObservableCollection<AirportData>();

            string FilterStr = "";

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = String.Format(@" and A.IsDeleted =@ShowDeleted");
                }
                command.CommandText = string.Format(@"SELECT A.AirportID,A.AirportCode,A.AirportDescr,A.IsDeleted,City.CityId,
City.CityCode,City.CityDescr,Country.CountryCode ,Country.CountryDescr,Prefecture.PrefCode,Prefecture.PrefDescr 
FROM Airports AS A
INNER JOIN City ON City.CityId = A.CityId
INNER JOIN Prefecture ON Prefecture.PrefId = City.PrefId 
Inner JOIN Country on Prefecture.CountryId = Country.CountryId
                                              Where 1=1 {0}", FilterStr);



                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AirportData data = new AirportData();
                        data.City = new CityData();


                        data.Id = int.Parse(reader["AirportID"].ToString());
                        data.Code = reader["AirportCode"].ToString();
                        data.Descr = reader["AirportDescr"].ToString();

                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());


                        data.City.CityId = int.Parse(reader["CityId"].ToString());
                        data.City.CityCode = reader["CityCode"].ToString();
                        data.City.CityDescr = reader["CityDescr"].ToString();

                        data.City.PrefCode = reader["PrefCode"].ToString();
                        data.City.PrefDescr = reader["PrefDescr"].ToString();
                        data.City.CountryCode = reader["CountryCode"].ToString();
                        data.City.CountryDescr = reader["CountryDescr"].ToString();



                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }
        #endregion

        #region FlightLegs
        public int SaveFlightLegsData(FlightLegsData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    int Id = flatData.FlightLegId;
                    var existingQuery = dbContext.FlightLegs.Where(c => c.FlightLegId == Id);
                    var existing = existingQuery.SingleOrDefault();
                    // Execute the query and get the result
                    var AirportFromQuery = dbContext.Airports.Where(c => c.AirportID == flatData.AirportDataFrom.Id);
                    var AirportToQuery = dbContext.Airports.Where(c => c.AirportID == flatData.AirportDataTo.Id);

                    var AirportFrom = AirportFromQuery.SingleOrDefault();
                    var AirportTo = AirportToQuery.SingleOrDefault();

                    if (existing != null)
                    {


                        // Update existing customer
                        existing.Code = flatData.Code;
                        existing.Descr = flatData.Descr;

                        existing.AirportFrom = AirportFrom.AirportID;
                        existing.AirportTo = AirportTo.AirportID;
                        existing.StartDate = flatData.StartDate;
                        existing.EndDate = flatData.EndDate;
                        existing.FlightTime = flatData.FlightTime;
                        existing.IsDeleted = flatData.IsDeleted;

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
                LogError(ex, "SaveFlightLegsData", "Notes");
                return -1;
            }
        }

        public int AddFlightLegsData(FlightLegsData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    var existingQuery = dbContext.FlightLegs.Where(r => r.Code == flatData.Code);
                    var existing = existingQuery.SingleOrDefault();
                    // Execute the query and get the result


                    if (existing == null)
                    {
                        var newItem = new FlightLegsDataEntity();
                        // Insert new item
                        newItem.Code = flatData.Code;
                        newItem.Descr = flatData.Descr;

                        newItem.AirportFrom = dbContext.Airports.FirstOrDefault().AirportID;
                        newItem.AirportTo = dbContext.Airports.Skip(1).FirstOrDefault().AirportID;
                        newItem.StartDate = DateTime.Now;
                        newItem.EndDate = DateTime.Now.AddDays(1);
                        newItem.FlightTime = (float)(newItem.EndDate - newItem.StartDate).Value.TotalHours;
                        newItem.IsDeleted = flatData.IsDeleted;
                        newItem.IsDeleted = false;

                        dbContext.FlightLegs.Add(newItem);

                        dbContext.SaveChanges();
                        return 0;
                    }
                    else
                    {
                        // Else Print messages
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "AddFlightLegsData", "Notes");
                return 2;

            }
        }

        public FlightLegsData GetFlightLegsChooserData(int Id, string Code)
        {
            FlightLegsData data = new FlightLegsData();
            string FilterStr = "";
            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;

                    if (Id > 0)
                    {
                        command.Parameters.AddWithValue("@ID", Id);
                        FilterStr = String.Format(@" and F.FlightLegId =@ID");

                    }

                    else if (!string.IsNullOrWhiteSpace(Code))
                    {
                        command.Parameters.AddWithValue("@Code", Code);
                        FilterStr = String.Format(@" and F.Code =@Code");

                    }
                    command.CommandText = string.Format(@"select F.FlightLegId,F.Code,F.Descr,F.StartDate,F.EndDate,F.FlightTime,F.IsDeleted,
AFrom.AirportID as AFId,AFrom.AirportCode as AFCode,AFrom.AirportDescr as AFDescr,
ATo.AirportID as ATId,ATo.AirportCode as ATCode,ATo.AirportDescr  as ATDescr
From FlightLegs as F
Inner Join Airports as AFrom on AFrom.AirportID = F.AirportFrom
Inner Join Airports as ATo on ATo.AirportID = F.AirportTo
                                              Where 1=1 {0}", FilterStr);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data.AirportDataFrom = new AirportData();
                            data.AirportDataTo = new AirportData();


                            data.FlightLegId = int.Parse(reader["FlightLegId"].ToString());
                            data.Code = reader["Code"].ToString();
                            data.Descr = reader["Descr"].ToString();
                            data.StartDate = DateTime.Parse(reader["StartDate"].ToString());
                            data.EndDate = DateTime.Parse(reader["EndDate"].ToString());

                            data.StartDate_String = data.StartDate.ToString("dd/MM/yyyy HH:mm");
                            data.EndDate_String = data.EndDate.ToString("dd/MM/yyyy HH:mm");

                            data.FlightTime = float.Parse(reader["FlightTime"].ToString());

                            data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());


                            data.AirportDataFrom.Id = int.Parse(reader["AFId"].ToString());
                            data.AirportDataFrom.Code = reader["AFCode"].ToString();
                            data.AirportDataFrom.Descr = reader["AFDescr"].ToString();

                            data.AirportDataTo.Id = int.Parse(reader["ATId"].ToString());
                            data.AirportDataTo.Code = reader["ATCode"].ToString();
                            data.AirportDataTo.Descr = reader["ATDescr"].ToString();
                        }
                    }

                    connection.Close();
                }

                return data;
            }
            catch (Exception ex)
            {
                LogError(ex, "GetFlightLegsChooserData", "Notes");
                return data;
            }
        }
        public ObservableCollection<FlightLegsData> GetFlightLegsData(bool ShowDeleted)
        {
            ObservableCollection<FlightLegsData> DataList = new ObservableCollection<FlightLegsData>();

            string FilterStr = "";

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = String.Format(@" and F.IsDeleted =@ShowDeleted");
                }
                command.CommandText = string.Format(@"select F.FlightLegId,F.Code,F.Descr,F.StartDate,F.EndDate,F.FlightTime,F.IsDeleted,
AFrom.AirportID as AFId,AFrom.AirportCode as AFCode,AFrom.AirportDescr as AFDescr,
ATo.AirportID as ATId,ATo.AirportCode as ATCode,ATo.AirportDescr  as ATDescr
From FlightLegs as F
Inner Join Airports as AFrom on AFrom.AirportID = F.AirportFrom
Inner Join Airports as ATo on ATo.AirportID = F.AirportTo
                                              Where 1=1 {0}", FilterStr);



                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        FlightLegsData data = new FlightLegsData();
                        data.AirportDataFrom = new AirportData();
                        data.AirportDataTo = new AirportData();


                        data.FlightLegId = int.Parse(reader["FlightLegId"].ToString());
                        data.Code = reader["Code"].ToString();
                        data.Descr = reader["Descr"].ToString();
                        data.StartDate = DateTime.Parse(reader["StartDate"].ToString());
                        data.EndDate = DateTime.Parse(reader["EndDate"].ToString());

                        data.StartDate_String = data.StartDate.ToString("dd/MM/yyyy HH:mm");
                        data.EndDate_String = data.EndDate.ToString("dd/MM/yyyy HH:mm");

                        data.FlightTime = float.Parse(reader["FlightTime"].ToString());

                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());


                        data.AirportDataFrom.Id = int.Parse(reader["AFId"].ToString());
                        data.AirportDataFrom.Code = reader["AFCode"].ToString();
                        data.AirportDataFrom.Descr = reader["AFDescr"].ToString();

                        data.AirportDataTo.Id = int.Parse(reader["ATId"].ToString());
                        data.AirportDataTo.Code = reader["ATCode"].ToString();
                        data.AirportDataTo.Descr = reader["ATDescr"].ToString();



                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }
        #endregion

        #region FlightRoutes
        public int SaveFlightRoutesData(FlightRoutesData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    int Id = flatData.FlightRouteId;
                    var existingQuery = dbContext.FlightRoutes.Where(c => c.FlightRouteId == Id);
                    var existing = existingQuery.SingleOrDefault();
                    // Execute the query and get the result
                    var AirportQuery = dbContext.Airports.Where(c => c.AirportID == flatData.Airport.Id);

                    var Airport = AirportQuery.SingleOrDefault();

                    if (existing != null)
                    {


                        // Update existing customer
                        existing.Code = flatData.Code;
                        existing.Descr = flatData.Descr;

                        existing.AirportId = Airport.AirportID;
                        existing.StartDate = flatData.StartDate;
                        existing.EndDate = flatData.EndDate;
                        existing.FlightTime = (float)Math.Round(flatData.FlightTime, 2);
                        existing.GroundTime = (float)Math.Round(flatData.GroundTime, 2);
                        existing.TotalTime = (float)Math.Round(flatData.TotalTime, 2);
                        existing.Complement_Captain = flatData.Complement_Captain;
                        existing.Complement_FO = flatData.Complement_FO;
                        existing.Complement_Cabin_Manager = flatData.Complement_Cabin_Manager;
                        existing.Complement_Flight_Attendant = flatData.Complement_Flight_Attendant;
                        existing.FlightTime = flatData.FlightTime;
                        existing.IsDeleted = flatData.IsDeleted;

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
                LogError(ex, "SaveFlightRoutesData", "Notes");
                return -1;
            }
        }

        public int AddFlightRoutesData(FlightRoutesData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    var existingQuery = dbContext.FlightRoutes.Where(r => r.Code == flatData.Code);
                    var existing = existingQuery.SingleOrDefault();
                    // Execute the query and get the result


                    if (existing == null)
                    {
                        var newItem = new FlightRoutesDataEntity();
                        newItem.Code = flatData.Code;
                        newItem.Descr = flatData.Descr;

                        newItem.AirportId = dbContext.Airports.FirstOrDefault().AirportID;
                        newItem.StartDate = DateTime.Now;
                        newItem.EndDate = DateTime.Now.AddDays(2);
                        newItem.TotalTime = (float)Math.Round((newItem.EndDate - newItem.StartDate).Value.TotalHours, 2);
                        var TotalTime = (float)newItem.TotalTime;
                        newItem.FlightTime = (float)Math.Round(TotalTime * 0.7, 2);
                        newItem.GroundTime = newItem.TotalTime - newItem.FlightTime;
                        newItem.Complement_Captain = 1;
                        newItem.Complement_FO = 1;
                        newItem.Complement_Cabin_Manager = 1;
                        newItem.Complement_Flight_Attendant = 3;
                        newItem.IsDeleted = false;


                        dbContext.FlightRoutes.Add(newItem);

                        dbContext.SaveChanges();
                        return 0;
                    }
                    else
                    {
                        // Else Print messages
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "AddFlightLegsData", "Notes");
                return 2;

            }
        }

        public FlightRoutesData GetFlightRoutesChooserData(int Id, string Code)
        {
            FlightRoutesData data = new FlightRoutesData();
            string FilterStr = "";
            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;

                    if (Id > 0)
                    {
                        command.Parameters.AddWithValue("@ID", Id);
                        FilterStr = String.Format(@" and F.FlightRouteId =@ID");

                    }

                    else if (!string.IsNullOrWhiteSpace(Code))
                    {
                        command.Parameters.AddWithValue("@Code", Code);
                        FilterStr = String.Format(@" and F.Code =@Code");

                    }
                    command.CommandText = string.Format(@"select F.FlightRouteId,F.Code,F.Descr,F.StartDate,F.EndDate,F.FlightTime,F.GroundTime,F.TotalTime,F.IsDeleted,
F.Complement_Captain AS CCA,F.Complement_FO AS CFO, F.Complement_Flight_Attendant AS CFA, F.Complement_Cabin_Manager AS CCM, 
A.AirportID ,A.AirportCode,A.AirportDescr
From FlightRoutes as F
Inner Join Airports as A on A.AirportID = F.AirportId
                                              Where 1=1 {0}", FilterStr);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data.Airport = new AirportData();


                            data.FlightRouteId = int.Parse(reader["FlightRouteId"].ToString());
                            data.Code = reader["Code"].ToString();
                            data.Descr = reader["Descr"].ToString();
                            data.StartDate = DateTime.Parse(reader["StartDate"].ToString());
                            data.EndDate = DateTime.Parse(reader["EndDate"].ToString());

                            data.StartDate_String = data.StartDate.ToString("dd/MM/yyyy HH:mm");
                            data.EndDate_String = data.EndDate.ToString("dd/MM/yyyy HH:mm");

                            data.TotalTime = float.Parse(reader["TotalTime"].ToString());
                            data.FlightTime = float.Parse(reader["FlightTime"].ToString());
                            data.GroundTime = float.Parse(reader["GroundTime"].ToString());

                            data.Complement_Captain = int.Parse(reader["CCA"].ToString());
                            data.Complement_FO = int.Parse(reader["CFO"].ToString());
                            data.Complement_Flight_Attendant = int.Parse(reader["CFA"].ToString());
                            data.Complement_Cabin_Manager = int.Parse(reader["CCM"].ToString());

                            data.Airport.Id = int.Parse(reader["AirportID"].ToString());
                            data.Airport.Code = reader["AirportCode"].ToString();
                            data.Airport.Descr = reader["AirportDescr"].ToString();

                            data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());


                        }
                    }

                    connection.Close();
                }

                return data;
            }
            catch (Exception ex)
            {
                LogError(ex, "GetFlightRoutesChooserData", "Notes");
                return data;
            }
        }
        public ObservableCollection<FlightRoutesData> GetFlightRoutesData(bool ShowDeleted)
        {
            ObservableCollection<FlightRoutesData> DataList = new ObservableCollection<FlightRoutesData>();

            string FilterStr = "";

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = String.Format(@" and F.IsDeleted =@ShowDeleted");
                }
                command.CommandText = string.Format(@"select F.FlightRouteId,F.Code,F.Descr,F.StartDate,F.EndDate,F.FlightTime,F.GroundTime,F.TotalTime,F.IsDeleted,
F.Complement_Captain AS CCA,F.Complement_FO AS CFO, F.Complement_Flight_Attendant AS CFA, F.Complement_Cabin_Manager AS CCM, 
A.AirportID ,A.AirportCode,A.AirportDescr
From FlightRoutes as F
Inner Join Airports as A on A.AirportID = F.AirportId
                                              Where 1=1 {0}", FilterStr);



                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        FlightRoutesData data = new FlightRoutesData();
                        data.Airport = new AirportData();


                        data.FlightRouteId = int.Parse(reader["FlightRouteId"].ToString());
                        data.Code = reader["Code"].ToString();
                        data.Descr = reader["Descr"].ToString();
                        data.StartDate = DateTime.Parse(reader["StartDate"].ToString());
                        data.EndDate = DateTime.Parse(reader["EndDate"].ToString());

                        data.StartDate_String = data.StartDate.ToString("dd/MM/yyyy HH:mm");
                        data.EndDate_String = data.EndDate.ToString("dd/MM/yyyy HH:mm");

                        data.TotalTime = float.Parse(reader["TotalTime"].ToString());
                        data.FlightTime = float.Parse(reader["FlightTime"].ToString());
                        data.GroundTime = float.Parse(reader["GroundTime"].ToString());

                        data.Complement_Captain = int.Parse(reader["CCA"].ToString());
                        data.Complement_FO = int.Parse(reader["CFO"].ToString());
                        data.Complement_Flight_Attendant = int.Parse(reader["CFA"].ToString());
                        data.Complement_Cabin_Manager = int.Parse(reader["CCM"].ToString());

                        data.Airport.Id = int.Parse(reader["AirportID"].ToString());
                        data.Airport.Code = reader["AirportCode"].ToString();
                        data.Airport.Descr = reader["AirportDescr"].ToString();

                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());



                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }
        #endregion

        #region Crew Scheduling

        #region CRUD Commands

        public int SaveCSInputData(CSInputData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    int Id = flatData.Id;
                    var existingQuery = dbContext.CSInput.Where(c => c.CSID == Id);
                    var existing = existingQuery.SingleOrDefault();

                    // Execute the query and get the result


                    if (existing != null)
                    {


                        // Update existing customer
                        existing.CSCODE = flatData.Code;
                        existing.CSDESCR = flatData.Descr;
                        existing.EMPLOYEETYPE = flatData.Position.ToString();
                        existing.DateFrom = flatData.DateFrom;
                        existing.DateTo = flatData.DateTo;
                        existing.RoutesPenalty = flatData.RoutesPenalty;
                        existing.BoundsPenalty = flatData.BoundsPenalty;
                        existing.IsDeleted = flatData.IsDeleted;

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
                LogError(ex, "SaveCSInputData", "Notes");
                return -1;
            }
        }
        public int AddCSInputData(CSInputData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    var existingQuery = dbContext.CSInput.Where(r => r.CSCODE == flatData.Code);
                    var existing = existingQuery.SingleOrDefault();
                    // Execute the query and get the result


                    if (existing == null)
                    {
                        var newCS = new CSInputDataEntity();
                        // Insert new item
                        newCS.CSCODE = flatData.Code;
                        newCS.CSDESCR = flatData.Descr;
                        newCS.EMPLOYEETYPE = flatData.Position.ToString();
                        newCS.DateFrom = flatData.DateFrom;
                        newCS.DateTo = flatData.DateTo;
                        newCS.RoutesPenalty = flatData.RoutesPenalty;
                        newCS.BoundsPenalty = flatData.BoundsPenalty;
                        newCS.IsDeleted = flatData.IsDeleted;




                        dbContext.CSInput.Add(newCS);

                        dbContext.SaveChanges();
                        return 0;
                    }
                    else
                    {
                        // Else Print messages
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "AddCSInputData", "Notes");
                return 2;

            }
        }

        public ObservableCollection<CSInputData> GetCSInputData(bool ShowDeleted)
        {
            ObservableCollection<CSInputData> DataList = new ObservableCollection<CSInputData>();

            string FilterStr = "";
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = String.Format(@"and IsDeleted = @ShowDeleted");

                }

                command.CommandText = string.Format(@"SELECT CSID,CSCODE,CSDESCR,EMPLOYEETYPE,DateFrom,DateTo,RoutesPenalty,
                                                     BoundsPenalty,IsDeleted 
                                                     FROM CSInput
                                                     Where 1=1 {0}", FilterStr);


                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CSInputData data = new CSInputData();
                        data.Id = int.Parse(reader["CSID"].ToString());
                        data.Code = reader["CSCODE"].ToString();
                        data.Descr = reader["CSDESCR"].ToString();
                        data.DateFrom = DateTime.Parse(reader["DateFrom"].ToString());
                        data.DateTo = DateTime.Parse(reader["DateTo"].ToString());
                        data.RoutesPenalty = int.Parse(reader["RoutesPenalty"].ToString());
                        data.BoundsPenalty = int.Parse(reader["BoundsPenalty"].ToString());

                        data.Position = (BasicEnums.EmployeeType)Enum.Parse(typeof(BasicEnums.EmployeeType), reader["EMPLOYEETYPE"].ToString());
                        data.CSType = BasicEnums.CSType.Set_Partition;

                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());

                        data.DateFrom_Str = data.DateFrom.ToString("dd/MM/yyyy HH:mm");
                        data.DateTo_Str = data.DateTo.ToString("dd/MM/yyyy HH:mm");




                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }

        public CSInputData GetCSInputChooserData(int CSId, string CSCode, CSInputData Data)
        {

            string FilterStr = "";
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (CSId > 0)
                {
                    command.Parameters.AddWithValue("@CSID", CSId);
                    FilterStr += " and CSID = @CSID";
                }
                else if (!string.IsNullOrWhiteSpace(CSCode))
                {
                    command.Parameters.AddWithValue("@CSCODE", CSCode);
                    FilterStr += " and CSCODE = @CSCODE";
                }

                command.CommandText = string.Format(@"SELECT CSID,CSCODE,CSDESCR,EMPLOYEETYPE,DateFrom,DateTo,RoutesPenalty,
                                                     BoundsPenalty,IsDeleted 
                                                     FROM CSInput
                                                     Where 1=1 {0}", FilterStr);



                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        Data.Id = int.Parse(reader["CSID"].ToString());
                        Data.Code = reader["CSCODE"].ToString();
                        Data.Descr = reader["CSDESCR"].ToString();
                        Data.DateFrom = DateTime.Parse(reader["DateFrom"].ToString());
                        Data.DateTo = DateTime.Parse(reader["DateTo"].ToString());

                        Data.DateFrom_Str = Data.DateFrom.ToString("dd/MM/yyyy HH:mm");
                        Data.DateTo_Str = Data.DateTo.ToString("dd/MM/yyyy HH:mm");
                        Data.RoutesPenalty = int.Parse(reader["RoutesPenalty"].ToString());
                        Data.BoundsPenalty = int.Parse(reader["BoundsPenalty"].ToString());

                        Data.Position = (BasicEnums.EmployeeType)Enum.Parse(typeof(BasicEnums.EmployeeType), reader["EMPLOYEETYPE"].ToString());
                        Data.CSType = BasicEnums.CSType.Set_Partition;

                        Data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());
                    }
                }

                connection.Close();
            }

            return Data;
        }

        public ObservableCollection<FlightRoutesData> GetCSFlightRoutesData(bool ShowDeleted, CSInputData InputData)
        {
            ObservableCollection<FlightRoutesData> DataList = new ObservableCollection<FlightRoutesData>();

            string FilterStr = "";

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = String.Format(@" and F.IsDeleted =@ShowDeleted");
                }

                command.Parameters.AddWithValue("@StartDate", InputData.DateFrom);
                command.Parameters.AddWithValue("@EndDate", InputData.DateTo);
                FilterStr += @" and F.StartDate <= @EndDate";
                FilterStr += @" and F.StartDate >= @StartDate";


                command.CommandText = string.Format(@"SELECT F.FlightRouteId,F.Code,F.Descr,F.StartDate,F.EndDate,F.FlightTime,F.GroundTime,F.TotalTime,
F.Complement_Captain,F.Complement_FO,F.Complement_Cabin_Manager,F.Complement_Flight_Attendant,
F.IsDeleted,A.AirportID,A.AirportCode,A.AirportDescr,C.CityCode,C.CityDescr,P.PrefDescr,Co.CountryDescr
FROM FLIGHTROUTES AS F
Inner JOIN Airports AS A on A.AirportID = F.AirportID
Inner Join City as C on C.CityId = A.CityId
Inner Join Prefecture as P on P.PrefId = C.PrefId
Inner JOIN Country as Co on Co.CountryId = P.CountryId
                                              Where 1=1 {0}", FilterStr);



                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        FlightRoutesData data = new FlightRoutesData();
                        data.Airport = new AirportData();
                        data.Airport.City = new CityData();

                        data.FlightRouteId = int.Parse(reader["FlightRouteId"].ToString());
                        data.Code = reader["Code"].ToString();
                        data.Descr = reader["Descr"].ToString();
                        data.StartDate = DateTime.Parse(reader["StartDate"].ToString());
                        data.EndDate = DateTime.Parse(reader["EndDate"].ToString());
                        data.StartDate_String = data.StartDate.ToString("dd/MM/yyyy HH:mm");
                        data.EndDate_String = data.EndDate.ToString("dd/MM/yyyy HH:mm");

                        data.Complement_Captain = int.Parse(reader["Complement_Captain"].ToString());
                        data.Complement_FO = int.Parse(reader["Complement_FO"].ToString());
                        data.Complement_Cabin_Manager = int.Parse(reader["Complement_Cabin_Manager"].ToString());
                        data.Complement_Flight_Attendant = int.Parse(reader["Complement_Flight_Attendant"].ToString());


                        data.TotalTime = float.Parse(reader["TotalTime"].ToString());
                        data.FlightTime = float.Parse(reader["FlightTime"].ToString());
                        data.GroundTime = float.Parse(reader["GroundTime"].ToString());
                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());

                        data.Airport.Id = int.Parse(reader["AirportID"].ToString());
                        data.Airport.Code = reader["AirportCode"].ToString();
                        data.Airport.Descr = reader["AirportDescr"].ToString();

                        data.Airport.City.CityCode = reader["CityCode"].ToString();
                        data.Airport.City.CityDescr = reader["CityDescr"].ToString();
                        data.Airport.City.PrefDescr = reader["PrefDescr"].ToString();
                        data.Airport.City.CountryDescr = reader["CountryDescr"].ToString();



                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }

        #endregion

        #region Optimisation

        public CSOutputData CalculateCrewScheduling_SetCover_GB(CSInputData InputData)
        {
            #region Model,Data,FilePath Initialization

            GRBEnv env = new GRBEnv("cslogfile.log");
            GRBModel model = new GRBModel(env);
            GRBEnv finalenv = new GRBEnv("cslogfile_final.log"); // !!!TO VLEPOUME BOREI NA FUGEI

            string relativePath = Path.Combine("OptimizationResults", "Gurobi", "Thesis", "CS_Set_Cover");
            Directory.CreateDirectory(relativePath);

            CSOutputData Data = new CSOutputData();

            #endregion

            try
            {
                #region Optimization

                #region Optimization paramaters

                #region Indexes
                int T = InputData.T; // Planning Horizon
                int I = InputData.I; // Number Of Employees Empty Schedules
                int F = InputData.F; // Number Of Routes
                #endregion

                #region Dictionaries
                Dictionary<int, List<int>> Ri = InputData.Ri;
                Dictionary<(int, int), double> Cij_Hours = InputData.Cij_Hours;
                Dictionary<(int, int, int), int> Aijf = InputData.Aijf;

                Dictionary<int, int> Bf = InputData.RoutesCompl_Dict;
                //We need RiMax for the declaration of Model Variables
                int RiMax = InputData.Ri.Values.Max(list => list.Count);

                #endregion

                #region Penalty Cost

                var h = InputData.RoutesPenalty;
                var c = InputData.BoundsPenalty;

                #endregion

                #endregion

                #region Decision Variables
                // Decision variables

                GRBVar[,] X = new GRBVar[I, RiMax];
                GRBVar[] Y = new GRBVar[F];

                for (int i = 0; i < I; i++)
                {
                    List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

                    foreach (int j in RosterPerEmployee_List)
                    {
                        // Define the variable name
                        string varNameX = $"X{i + 1}_{j + 1}";

                        // Create the binary variable with a name
                        X[i, j] = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, varNameX);
                    }

                }

                for (int f = 0; f < F; f++)
                {
                    string varNameY = $"Y{f + 1}";

                    // Create the binary variable with a name
                    Y[f] = model.AddVar(0.0, 1.0, 0.0, GRB.INTEGER, varNameY);

                }
                #endregion

                #region Objective Function

                GRBLinExpr objective = 0;

                for (int i = 0; i < I; i++)
                {
                    List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

                    foreach (int j in RosterPerEmployee_List)
                    {
                        var Cij_Cost = Cij_Hours[(i, j)] * c;
                        objective.AddTerm(Cij_Cost, X[i, j]);

                    }

                }

                for (int f = 0; f < F; f++)
                {
                    objective.AddTerm(h, Y[f]);
                }

                model.SetObjective(objective, GRB.MINIMIZE);

                #endregion

                #region Constrains

                for (int i = 0; i < I; i++)
                {
                    GRBLinExpr expr = 0;
                    List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

                    foreach (int j in RosterPerEmployee_List)
                    {
                        expr.AddTerm(1, X[i, j]);
                    }
                    model.AddConstr(expr, GRB.EQUAL, 1, "CON2_" + (i + 1));


                }
                for (int f = 0; f < F; f++)
                {
                    GRBLinExpr expr = 0;

                    expr.AddTerm(1, Y[f]);

                    for (int i = 0; i < I; i++)
                    {
                        List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

                        foreach (int j in RosterPerEmployee_List)
                        {
                            expr.AddTerm(Aijf[(i, j, f)], X[i, j]);

                        }
                    }

                    model.AddConstr(expr, GRB.GREATER_EQUAL, Bf[f], "CON3_" + (F + 1));

                }

                #endregion

                #endregion
                #region Retrieve Model,Solution

                model.Update();
                model.Optimize();
                bool solution = (model.Status == GRB.Status.OPTIMAL);
                if (solution)
                {
                    Data.ObjValue = model.ObjVal;
                    model.Update();

                    // Save the files
                    model.Write(Path.Combine(relativePath, "CS_SetCover.mst"));
                    model.Write(Path.Combine(relativePath, "CS_SetCover.sol"));
                    model.Write(Path.Combine(relativePath, "CS_SetCover.lp"));
                    model.Write(Path.Combine(relativePath, "CS_SetCover.mps"));


                }
                return Data;

                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return Data;
            }

        }
        public CSOutputData CalculateCrewScheduling_SetPartition_GB(CSInputData InputData)
        {
            #region Model,Data,FilePath Initialization

            GRBEnv env = new GRBEnv("cslogfile.log");
            GRBModel model = new GRBModel(env);
            GRBEnv finalenv = new GRBEnv("cslogfile_final.log"); // !!!TO VLEPOUME BOREI NA FUGEI

            string relativePath = Path.Combine("OptimizationResults", "Gurobi", "Thesis", "CS_Set_Partition");
            Directory.CreateDirectory(relativePath);

            CSOutputData Data = new CSOutputData();

            #endregion

            try
            {
                #region Optimization

                #region Optimization paramaters

                #region Indexes
                int T = InputData.T; // Planning Horizon
                int I = InputData.I; // Number Of Employees Empty Schedules
                int F = InputData.F; // Number Of Routes
                #endregion

                #region Dictionaries
                Dictionary<int, List<int>> Ri = InputData.Ri;
                Dictionary<(int, int), double> Cij_Hours = InputData.Cij_Hours;
                Dictionary<(int, int, int), int> Aijf = InputData.Aijf;

                Dictionary<int, int> Bf = InputData.RoutesCompl_Dict;
                //We need RiMax for the declaration of Model Variables
                int RiMax = InputData.Ri.Values.Max(list => list.Count);

                #endregion

                #region Penalty Cost

                var h = InputData.RoutesPenalty;
                var c = InputData.BoundsPenalty;

                #endregion

                #endregion

                #region Decision Variables
                // Decision variables

                GRBVar[,] X = new GRBVar[I, RiMax];
                GRBVar[] Y = new GRBVar[F];

                for (int i = 0; i < I; i++)
                {
                    List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

                    foreach (int j in RosterPerEmployee_List)
                    {
                        // Define the variable name
                        string varNameX = $"X{i + 1}_{j + 1}";

                        // Create the binary variable with a name
                        X[i, j] = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, varNameX);
                    }

                }

                for (int f = 0; f < F; f++)
                {
                    string varNameY = $"Y{f + 1}";

                    // Create the binary variable with a name
                    Y[f] = model.AddVar(0.0, 1.0, 0.0, GRB.INTEGER, varNameY);

                }
                #endregion

                #region Objective Function

                GRBLinExpr objective = 0;

                for (int i = 0; i < I; i++)
                {
                    List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

                    foreach (int j in RosterPerEmployee_List)
                    {
                        var Cij_Cost = Cij_Hours[(i, j)] * c;
                        objective.AddTerm(Cij_Cost, X[i, j]);

                    }

                }

                for (int f = 0; f < F; f++)
                {
                    objective.AddTerm(h, Y[f]);
                }

                model.SetObjective(objective, GRB.MINIMIZE);

                #endregion

                #region Constrains

                for (int i = 0; i < I; i++)
                {
                    GRBLinExpr expr = 0;
                    List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

                    foreach (int j in RosterPerEmployee_List)
                    {
                        expr.AddTerm(1, X[i, j]);
                    }
                    model.AddConstr(expr, GRB.EQUAL, 1, "CON2_" + (i + 1));


                }
                for (int f = 0; f < F; f++)
                {
                    GRBLinExpr expr = 0;

                    expr.AddTerm(1, Y[f]);

                    for (int i = 0; i < I; i++)
                    {
                        List<int> RosterPerEmployee_List = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

                        foreach (int j in RosterPerEmployee_List)
                        {
                            expr.AddTerm(Aijf[(i,j,f)], X[i, j]);

                        }
                    }

                    model.AddConstr(expr, GRB.EQUAL, Bf[f], "CON3_" + (F + 1));

                }

                #endregion

                #endregion
                #region Retrieve Model,Solution

                model.Update();
                model.Optimize();
                bool solution = (model.Status == GRB.Status.OPTIMAL);
                if (solution)
                {
                    Data.ObjValue = model.ObjVal;
                    model.Update();

                    // Save the files
                    model.Write(Path.Combine(relativePath, "CS_SetPartition.mst"));
                    model.Write(Path.Combine(relativePath, "CS_SetPartition.sol"));
                    model.Write(Path.Combine(relativePath, "CS_SetPartition.lp"));
                    model.Write(Path.Combine(relativePath, "CS_SetPartition.mps"));


                }
                return Data;

                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return Data;
            }

        }
        #endregion

        #endregion

        #endregion

        #endregion


        public UserModel GetByUserName(string userName)
        {
            UserModel user = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "select * from [User] where username=@username";
                command.Parameters.Add("@username", System.Data.SqlDbType.NVarChar).Value = userName;
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new UserModel()
                        {
                            Id = reader[0].ToString(),
                            UserName = reader[1].ToString(),
                            Password = string.Empty,
                            Name = reader[3].ToString(),
                            LastName = reader[4].ToString(),
                            Email = reader[5].ToString(),
                        };
                    }
                }



            }
            return user;
        }
    }
}
