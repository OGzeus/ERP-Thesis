﻿using OfficeOpenXml;
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
using Syncfusion.Windows.Shared;
using Erp.Model.Suppliers;
using Erp.Model.Inventory;
using Erp.ViewModel.Inventory;
using Erp.Model.Manufacture;
using Erp.Model.Data_Analytics;
using Erp.Model.Customers;
using Erp.DataBase;
using Erp.Model.Enums;
using Gurobi;
using Erp.Model.Inventory.InvControl_ConstantDemand;
using Erp.Model.Inventory.InvControl_TimeVaryingDemand;
using Deedle;
using System.Data;
using System.Web.UI.WebControls;
using Erp.Model.Data_Analytics.Forecast;
using Erp.Model.Manufacture.MPS;
using static Erp.Model.Enums.BasicEnums;
using Erp.Model.Manufacture.MRP;
using Erp.Model.Thesis;
using Erp.DataBase.Τhesis;
using Erp.Model.Thesis.VacationPlanning;
using Microsoft.EntityFrameworkCore.Internal;
using Erp.View.Thesis.CustomButtons;
using Newtonsoft.Json.Linq;
using Erp.Model.SupplyChain;
using Erp.Model.Thesis.CrewScheduling;
using OxyPlot;
using NetTopologySuite.Mathematics;
using System.Collections;
using static OfficeOpenXml.ExcelErrorValue;

namespace Erp.CommonFiles
{

    public class CommonFunctions : RepositoryBase
    {

        #region Thesis

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
                    var existing= existingQuery.SingleOrDefault();
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
                        newItem.FlightTime =(float)(newItem.EndDate - newItem.StartDate).Value.TotalHours;
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
                        existing.GroundTime = (float)Math.Round(flatData.GroundTime,2);
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
                    var selectedSchedule= selectedQuery.SingleOrDefault();


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

                        var existingRows = dbContext.ReqScheduleRows.Where(b => b.ReqCode == ReqCode  && b.DateStr == row.DateStr
                        && b.Position == row.Position.ToString());

                        var existingrow = dbContext.ReqScheduleRows.FirstOrDefault(b => b.ReqCode == ReqCode && b.DateStr == row.DateStr 
                        && b.Position == row.Position.ToString());

                        if (existingrow == null)
                        {
                            dbContext.ReqScheduleRows.Add(new ReqScheduleRowsDataEntity
                            {
                                ReqCode = ReqCode,
                                Position= row.Position.ToString(),
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
        public ObservableCollection<ReqScheduleRowsData> GetReqSchedulesRowsByEmpType(string ReqCode,BasicEnums.EmployeeType Position)
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
                    var existing= existingQuery.SingleOrDefault();
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

        public int SaveEmployeeData(EmployeeData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    int employeeId = flatData.EmployeeId;
                    var existingQuery = dbContext.Employees.Where(c => c.EmployeeID == employeeId);
                    var existing = existingQuery.SingleOrDefault();
                    // Execute the query and get the result
                    //var AirportQuery = dbContext.Airports.Where(c => c.AirportCode == flatData.BaseAirport.Code);
                    //var Airport = AirportQuery.SingleOrDefault();

                    //var CertificationQuery = dbContext.Certifications.Where(c => c.Code == flatData.Certification.Code);
                    //var Certification = AirportQuery.SingleOrDefault();

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

                        existing.BaseAirportId = flatData.BaseAirport.Id;
                        existing.CertificationID = flatData.Certification.Id;

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

                        newItem.BaseAirportId = dbContext.Airports.FirstOrDefault().AirportID;
                        newItem.CertificationID = dbContext.Certifications.FirstOrDefault().CertID;

                        newItem.IsDeleted = false;


                        dbContext.Employees.Add(newItem);

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

        #region 2ndTab Languages
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

        #endregion

        #region 3dTab LeaveBids

        public ObservableCollection<LeaveBidsDataStatic> GetLeaveBids(string EmployeeCode,string ScheduleCode)
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

                        data.BidCode = reader["BidCode"].ToString();

                        data.PriorityLevel = int.Parse(reader["PriorityLevel"].ToString());

                        data.BidType= (BasicEnums.BidType)Enum.Parse(typeof(BasicEnums.BidType), reader["BidType"].ToString());

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
                        else if (row.ExistingFlag == true && row.Bidflag == false)
                        {
                            var existingBid = dbContext.LeaveBids.SingleOrDefault(b => b.EmpId == EmployeeId && b.BidCode == row.BidCode);
                            dbContext.LeaveBids.Remove(existingBid);

                        }
                        else if (row.ExistingFlag == true && row.Bidflag == true && row.Modify == true)
                        {
                            if(row.OldBidCode != row.BidCode)
                            {
                                var existingBid = dbContext.LeaveBids.SingleOrDefault(b => b.EmpId == EmployeeId && b.BidCode == row.OldBidCode);
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
                            else
                            {
                                var existingBid = dbContext.LeaveBids.SingleOrDefault(b => b.EmpId == EmployeeId && b.BidCode == row.BidCode);
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

        #region 3dTab LeaveStatus
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
                        existing.MaxSatisfiedBids = flatData.MaxSatisfiedBids;
                        existing.SeparValue = flatData.SeparValue;

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
                        data.MaxSatisfiedBids = int.Parse(reader["MaxSatisfiedBids"].ToString());
                        data.SeparValue = int.Parse(reader["SeparValue"].ToString());

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
                        Data.MaxSatisfiedBids = int.Parse(reader["MaxSatisfiedBids"].ToString());
                        Data.SeparValue = int.Parse(reader["SeparValue"].ToString());

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
            Data.VPYijzResultsDataGrid = new ObservableCollection<VPYijResultsData>();
            Data.VPXijResultsDataGrid = new ObservableCollection<VPXijResultsData>();
            Data.VPXiResultsDataGrid = new ObservableCollection<VPXiResultData>();
            Data.EmpLeaveStatusData = new ObservableCollection<EmployeeData>();

            List<string> rows = new List<string>();
            List<string> columns = new List<string>();
            Dictionary<(string, string), double> GrantedDays_Dict = new Dictionary<(string, string), double>();
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

                    foreach (var row in Data.VPYijzResultsDataGrid)
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

        public int GetNextId(int aId, bool accept, int N, int[] NextBid, int[] NrOfBids,int FinishedEmpIds, BasicEnums.VPLogicType VPLogicType)
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
            foreach(var emp in InputData.Employees)
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
                Dictionary<int, int> LeaveDays = InputData.LeaveDays;
                Dictionary<int, int> LLiDict = InputData.LLiDict;

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
                    int Multiplier = LeaveDays[i+1] * 1000;
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


                    model.AddConstr(expr >= 0, "Day_" + (t+1));
                    //model.AddConstr(expr >= 0, "Day_" + Dates[t]);

                }
                var b = 1;

                // #3. Limit Lines
                for (int t = 0; t < Dates.Length; t++)
                {
                    GRBLinExpr expr = 0;



                   model.AddConstr(expr <= LLiDict[t+1], "LimitLine_" + (t + 1));
                    
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
                    string relativePath = Path.Combine("OptimizationResults", "Gurobi", "Thesis","VP_Column_Generation");
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
        public CSOutputData CalculateCrewScheduling_Init_GB(CSInputData InputData)
        {
            GRBEnv env = new GRBEnv("cslogfile.log");
            string relativePath = Path.Combine("OptimizationResults", "Gurobi", "Thesis", "Crew_Scheduling");
            Directory.CreateDirectory(relativePath);
            GRBModel model = new GRBModel(env);
            GRBEnv finalenv = new GRBEnv("cslogfile_final.log");
            CSOutputData Data = new CSOutputData();
            //Data.VPYijResultsDataGrid = new ObservableCollection<VPYijResultsData>();
            //Data.VPYijzResultsDataGrid = new ObservableCollection<VPYijResultsData>();



            try
            {
                #region Optimization

                #region Optimization paramaters

                var T = InputData.T; // Planning Horizon
                var I = InputData.I; // Number Of Employees Empty Schedules
                var F = InputData.F; // Number Of Routes

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

                GRBVar[] X = new GRBVar[F+I];

                for (int i = 0; i < F+I; i++)
                {
                    // Define the variable name
                    string varNameX = $"X{i + 1}";

                    // Create the binary variable with a name
                    X[i] = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, varNameX);
                }

                #endregion

                #region Objective Function

                GRBLinExpr objective = 0;

                for (int i = 0; i < F; i++)
                {
                    int RoutesPenalty = InputData.RoutesPenalty;
                    objective.AddTerm(RoutesPenalty, X[i]);
                }

                //var NumberOfSchedules = 0;
                //for (int i = N; i < NumberOfSchedules; i++)
                //{
                //    int PenaltyPerDay = InputData.BoundsPenalty;
                //    int TotalPenantly = PenaltyPerDay;
                //    objective.AddTerm(TotalPenantly, X[i]);
                //}

                model.SetObjective(objective, GRB.MINIMIZE);

                #endregion

                #region Constrains

                // #1. C1 -> C40 , X174-> X213 ROUTES 
                for (int i = F; i < F+I; i++)
                {
                    GRBLinExpr expr = 0;
                    expr.AddTerm(1, X[i]);
                    model.AddConstr(expr, GRB.EQUAL, 1, "C_" + (i-F + 1));
                }

                // #2. C41 -> C213 , X1-> X173 ROUTES 
                for (int i = 0; i < F; i++)
                {
                    GRBLinExpr expr = 0;
                    expr.AddTerm(1, X[i]);
                    model.AddConstr(expr, GRB.EQUAL, 1, "C_" + (i + F + 1));
                }

                #endregion

                model.Update();
                model.Optimize();
                bool solution = (model.Status == GRB.Status.OPTIMAL);
                if (solution)
                {
                    Data.ObjValue = model.ObjVal;
                    model.Update();



                    // Save the files
                    model.Write(Path.Combine(relativePath, "CS.mst"));
                    model.Write(Path.Combine(relativePath, "CS.sol"));
                    model.Write(Path.Combine(relativePath, "CS.lp"));
                    model.Write(Path.Combine(relativePath, "CS.mps"));


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
        public CSOutputData CalculateCrewScheduling_SetCover_GB(CSInputData InputData)
        {
            GRBEnv env = new GRBEnv("cslogfile.log");
            string relativePath = Path.Combine("OptimizationResults", "Gurobi", "Thesis", "Crew_Scheduling","Set_Cover");
            Directory.CreateDirectory(relativePath);
            GRBModel model = new GRBModel(env);
            GRBEnv finalenv = new GRBEnv("cslogfile_final.log");
            CSOutputData Data = new CSOutputData();
            //Data.VPYijResultsDataGrid = new ObservableCollection<VPYijResultsData>();
            //Data.VPYijzResultsDataGrid = new ObservableCollection<VPYijResultsData>();



            try
            {
                #region Optimization

                #region Optimization paramaters

                var T = InputData.T; // Planning Horizon
                var I = InputData.I; // Number Of Employees Empty Schedules
                var F = InputData.F; // Number Of Routes

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

                GRBVar[] X = new GRBVar[F + I];

                for (int i = 0; i < F + I; i++)
                {
                    // Define the variable name
                    string varNameX = $"X{i + 1}";

                    // Create the binary variable with a name
                    X[i] = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, varNameX);
                }

                #endregion

                #region Objective Function

                GRBLinExpr objective = 0;

                for (int i = 0; i < F; i++)
                {
                    int RoutesPenalty = InputData.RoutesPenalty;
                    objective.AddTerm(RoutesPenalty, X[i]);
                }

                //var NumberOfSchedules = 0;
                //for (int i = N; i < NumberOfSchedules; i++)
                //{
                //    int PenaltyPerDay = InputData.BoundsPenalty;
                //    int TotalPenantly = PenaltyPerDay;
                //    objective.AddTerm(TotalPenantly, X[i]);
                //}

                model.SetObjective(objective, GRB.MINIMIZE);

                #endregion

                #region Constrains

                // #1. C1 -> C40 , X174-> X213 ROUTES 
                for (int i = F; i < F + I; i++)
                {
                    GRBLinExpr expr = 0;
                    expr.AddTerm(1, X[i]);
                    model.AddConstr(expr, GRB.EQUAL, 1, "C_" + (i - F + 1));
                }

                // #2. C41 -> C213 , X1-> X173 ROUTES 
                for (int i = 0; i < F; i++)
                {
                    GRBLinExpr expr = 0;
                    expr.AddTerm(1, X[i]);
                    model.AddConstr(expr, GRB.EQUAL, 1, "C_" + (i + F + 1));
                }

                #endregion

                model.Update();
                model.Optimize();
                bool solution = (model.Status == GRB.Status.OPTIMAL);
                if (solution)
                {
                    Data.ObjValue = model.ObjVal;
                    model.Update();



                    // Save the files
                    model.Write(Path.Combine(relativePath, "CS.mst"));
                    model.Write(Path.Combine(relativePath, "CS.sol"));
                    model.Write(Path.Combine(relativePath, "CS.lp"));
                    model.Write(Path.Combine(relativePath, "CS.mps"));


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
            GRBEnv env = new GRBEnv("cslogfile.log");
            string relativePath = Path.Combine("OptimizationResults", "Gurobi", "Thesis", "Crew_Scheduling","Set_Partition");
            Directory.CreateDirectory(relativePath);
            GRBModel model = new GRBModel(env);
            GRBEnv finalenv = new GRBEnv("cslogfile_final.log");
            CSOutputData Data = new CSOutputData();
            //Data.VPYijResultsDataGrid = new ObservableCollection<VPYijResultsData>();
            //Data.VPYijzResultsDataGrid = new ObservableCollection<VPYijResultsData>();



            try
            {
                #region Optimization

                #region Optimization paramaters

                var T = InputData.T; // Planning Horizon
                var I = InputData.I; // Number Of Employees Empty Schedules
                var F = InputData.F; // Number Of Routes
                var Ri = InputData.Ri;
                int RiMax = InputData.Ri.Values.Max(list => list.Count);

                var DatesIndexMap = InputData.DatesIndexMap;
                var EmployeesIndexMap = InputData.EmployeesIndexMap;
                var RoutesIndexMap = InputData.RoutesIndexMap;

                var RoutesDates_Dict = InputData.RoutesDates_Dict;
                var RoutesDay_Dict = InputData.RoutesDay_Dict;
                var RoutesTime_Dict = InputData.RoutesTime_Dict;

                var EmpBounds_Dict = InputData.EmpBounds_Dict;

                var h = InputData.RoutesPenalty;
                var c = InputData.BoundsPenalty;
                #endregion

                #region Decision Variables
                // Decision variables

                GRBVar[,] X = new GRBVar[I , RiMax];
                GRBVar[] Y = new GRBVar[F];

                for (int i = 0; i <  I; i++)
                {
                    List<int> routesInI = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

                    foreach (int j in routesInI)
                    {
                        // Define the variable name
                        string varNameX = $"X{i + 1}_{j + 1}";

                        // Create the binary variable with a name
                        X[i,j] = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, varNameX);
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
                    List<int> routesInI = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

                    foreach (int j in routesInI)
                    {
                        var Cij = InputData.Cij_Hours[(i,j)] *c;
                        objective.AddTerm(Cij, X[i,j]);

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
                    List<int> routesInI = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

                    foreach (int j in routesInI)
                    {
                        expr.AddTerm(1, X[i, j]);
                    }
                    model.AddConstr(expr, GRB.EQUAL, 1, "CON2_" + (i+1));


                }
                for (int f = 0; f < F; f++)
                {
                    GRBLinExpr expr = 0;
                    var bf = 1;
                    expr.AddTerm(1, Y[f]);

                    for (int i = 0; i < I; i++)
                    {
                        List<int> routesInI = Ri.TryGetValue(i, out var productList) ? productList : new List<int>();

                        foreach (int j in routesInI)
                        {
                            expr.AddTerm(1, X[i, j]);

                        }
                    }

                    model.AddConstr(expr, GRB.EQUAL, bf, "CON3_" + ( F + 1));

                }

                #endregion

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

        #region Crud Commands
        #endregion

        #endregion


        #region ERP Factory Program

        #region BasicFiles

        #region Factory
        public ObservableCollection<FactoryData> GetFactoryChooserData()
        {
            ObservableCollection<FactoryData> DataList = new ObservableCollection<FactoryData>();


            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = string.Format(@"select Factory.Code,Factory.Descr,City.CityCode,City.CityDescr from Factory
Inner Join City on City.CityId = Factory.FactoryID");
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        FactoryData data = new FactoryData();
                        data.City = new CityData();
                        data.Code = reader["Code"].ToString();
                        data.Descr = reader["Descr"].ToString();
                        data.City.CityCode = reader["CityCode"].ToString();
                        data.City.CityDescr = reader["CityDescr"].ToString();

                        DataList.Add(data);
                    }
                }

                connection.Close();

            }

            return DataList;
        }

        #endregion

        #region Items
        public ItemData GetItemChooserData(int Id, string Code)
        {
            ItemData Data = new ItemData();
            Data.LotPolicy = new LotPolicyData();
            Data.LotPolicy.Item = new ItemData();
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
                        command.Parameters.AddWithValue("@ItemId", Id);
                        FilterStr = String.Format(@" and R.ItemId =@ItemId");
                    }

                    else if (!string.IsNullOrWhiteSpace(Code))
                    {
                        command.Parameters.AddWithValue("@ItemCode", Code);
                        FilterStr = String.Format(@" and R.ItemCode =@ItemCode");

                    }

                    command.CommandText = string.Format(@"SELECT
    R.ItemId,
    R.ItemCode,
    R.ItemDescr,
    R.MesUnit,
    R.ItemType,
    R.Assembly,
    R.AssemblyNumber,
    R.InputOrderFlag,
    R.OutputOrderFlag,
    R.CanBeProduced,
    R.Profit,
    R.SalesPrice,
    R.ManufacturingCost,
    R.HoldingCost,
    R.ShortageCost,
    R.LeadTime,
    L.LotPolicyId,
    L.LotPolicyCode,
    L.LotPolicyDescr,
    L.LeadTime AS LotPolicyLeadTime,
    L.BatchSize,
    L.Period,
    L.MainPolicy
FROM 
    Rmaster AS R
INNER JOIN LotPolicy AS L ON L.LotPolicyId = R.LotPolicyId
WHERE 
    R.ItemCode = @ItemCode
    AND L.MainPolicy = 1 {0}", FilterStr); 

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Data.ItemId = int.Parse(reader["ItemId"].ToString());
                            Data.ItemCode = reader["ItemCode"].ToString();
                            Data.ItemDescr = reader["ItemDescr"].ToString();
                            Data.MesUnit = reader["MesUnit"].ToString();
                            Data.InputOrderFlag = bool.Parse(reader["InputOrderFlag"].ToString());
                            Data.OutputOrderFlag = bool.Parse(reader["OutputOrderFlag"].ToString());
                            Data.CanBeProduced = bool.Parse(reader["CanBeProduced"].ToString());

                            Data.Assembly = (BasicEnums.Assembly)Enum.Parse(typeof(BasicEnums.Assembly), reader["Assembly"].ToString());
                            Data.AssemblyNumber = int.Parse(reader["AssemblyNumber"].ToString());

                            Data.ItemType = (BasicEnums.ItemType)Enum.Parse(typeof(BasicEnums.ItemType), reader["ItemType"].ToString());



                            Data.Profit = float.Parse(reader["Profit"].ToString());
                            Data.SalesPrice = float.Parse(reader["SalesPrice"].ToString());
                            Data.ManufacturingCost = float.Parse(reader["ManufacturingCost"].ToString());
                            Data.HoldingCost = float.Parse(reader["HoldingCost"].ToString());
                            Data.ShortageCost = float.Parse(reader["ShortageCost"].ToString());
                            Data.LeadTime = float.Parse(reader["LeadTime"].ToString());

                            Data.LotPolicy.LotPolicyId = int.Parse(reader["LotPolicyId"].ToString());
                            Data.LotPolicy.Code = reader["LotPolicyCode"].ToString();
                            Data.LotPolicy.Descr = reader["LotPolicyDescr"].ToString();
                            Data.LotPolicy.LeadTime = double.Parse(reader["LotPolicyLeadTime"].ToString());
                            Data.LotPolicy.BatchSize = double.Parse(reader["BatchSize"].ToString());
                            Data.LotPolicy.Period = int.Parse(reader["Period"].ToString());
                            Data.LotPolicy.MainPolicy = bool.Parse(reader["MainPolicy"].ToString());

                            Data.LotPolicy.Item.ItemId = int.Parse(reader["ItemId"].ToString());
                            Data.LotPolicy.Item.ItemCode = reader["ItemCode"].ToString();



                        }
                    }


                    connection.Close();

                }

                return Data;
            }
            catch (Exception ex)
            {
                return Data = null;
            }
        }
        public ObservableCollection<ItemData> GetItemData(bool ShowDeleted)
        {
            ObservableCollection<ItemData> DataList = new ObservableCollection<ItemData>();

             string FilterStr = "";
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = String.Format(@"and R.IsDeleted = @ShowDeleted");

                }

                command.CommandText = string.Format(@"SELECT
    R.ItemId,
    R.ItemCode,
    R.ItemDescr,
    R.MesUnit,
    R.ItemType,
    R.Assembly,
    R.AssemblyNumber,
    R.InputOrderFlag,
    R.OutputOrderFlag,
    R.CanBeProduced,
    R.Profit,
    R.SalesPrice,
    R.ManufacturingCost,
    R.HoldingCost,
    R.ShortageCost,
    R.LeadTime, 
    R.IsDeleted,
    L.LotPolicyId,
    L.LotPolicyCode,
    L.LotPolicyDescr,
    L.LeadTime AS LotPolicyLeadTime,
    L.BatchSize,
    L.Period,
    L.MainPolicy
FROM 
    Rmaster AS R
INNER JOIN LotPolicy AS L ON L.LotPolicyId = R.LotPolicyId
WHERE 
     L.MainPolicy = 1 {0}", FilterStr);


                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ItemData data = new ItemData();
                        data.LotPolicy = new LotPolicyData();
                        data.LotPolicy.Item = new ItemData();

                        data.ItemId = int.Parse(reader["ItemId"].ToString());

                        data.ItemCode = reader["ItemCode"].ToString();
                        data.ItemDescr = reader["ItemDescr"].ToString();
                        data.MesUnit = reader["MesUnit"].ToString();
                        data.Assembly = (BasicEnums.Assembly)Enum.Parse(typeof(BasicEnums.Assembly), reader["Assembly"].ToString());
                        data.AssemblyNumber = int.Parse(reader["AssemblyNumber"].ToString());

                        data.ItemType = (BasicEnums.ItemType)Enum.Parse(typeof(BasicEnums.ItemType), reader["ItemType"].ToString());
                        data.InputOrderFlag = bool.Parse(reader["InputOrderFlag"].ToString());
                        data.OutputOrderFlag = bool.Parse(reader["OutputOrderFlag"].ToString());
                        data.CanBeProduced = bool.Parse(reader["CanBeProduced"].ToString());
                        data.Profit = float.Parse(reader["Profit"].ToString());
                        data.SalesPrice = float.Parse(reader["SalesPrice"].ToString());
                        data.ManufacturingCost = float.Parse(reader["ManufacturingCost"].ToString());
                        data.HoldingCost = float.Parse(reader["HoldingCost"].ToString());
                        data.ShortageCost = float.Parse(reader["ShortageCost"].ToString());
                        data.LeadTime = float.Parse(reader["LeadTime"].ToString());
                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());

                        data.LotPolicy.LotPolicyId = int.Parse(reader["LotPolicyId"].ToString());
                        data.LotPolicy.Code = reader["LotPolicyCode"].ToString();
                        data.LotPolicy.Descr = reader["LotPolicyDescr"].ToString();
                        data.LotPolicy.LeadTime = double.Parse(reader["LotPolicyLeadTime"].ToString());
                        data.LotPolicy.BatchSize = double.Parse(reader["BatchSize"].ToString());
                        data.LotPolicy.Period = int.Parse(reader["Period"].ToString());
                        data.LotPolicy.MainPolicy = bool.Parse(reader["MainPolicy"].ToString());

                        DataList.Add(data);
                    }
                }

                connection.Close();

            }

            return DataList;
        }
        public ObservableCollection<ItemData> GetItemMPSInputChooserData()
        {
            ObservableCollection<ItemData> DataList = new ObservableCollection<ItemData>();


            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = string.Format(@"SELECT
    R.ItemId,
    R.ItemCode,
    R.ItemDescr,
    R.Assembly,
    R.ItemType,
	R.Profit,
    R.SalesPrice,
    R.ManufacturingCost,
    R.HoldingCost,
    R.ShortageCost,
    R.LeadTime,
	R.MaxInventory,
	R.StoreTarget

FROM 
    Rmaster AS R
INNER JOIN LotPolicy AS L ON L.LotPolicyId = R.LotPolicyId
WHERE 
     R.OutputOrderFlag = 1;
");
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ItemData data = new ItemData();
                        data.LotPolicy = new LotPolicyData();
                        data.LotPolicy.Item = new ItemData();

                        data.ItemId = int.Parse(reader["ItemId"].ToString());
                        data.ItemCode = reader["ItemCode"].ToString();
                        data.ItemDescr = reader["ItemDescr"].ToString();
                        data.Assembly = (BasicEnums.Assembly)Enum.Parse(typeof(BasicEnums.Assembly), reader["Assembly"].ToString());
                        data.ItemType = (BasicEnums.ItemType)Enum.Parse(typeof(BasicEnums.ItemType), reader["ItemType"].ToString());
                        data.HoldingCost = float.Parse(reader["HoldingCost"].ToString());
                        data.MaxInventory = int.Parse(reader["MaxInventory"].ToString());
                        data.StoreTarget = int.Parse(reader["StoreTarget"].ToString());

                        data.Profit = float.Parse(reader["Profit"].ToString());
                        data.SalesPrice = float.Parse(reader["SalesPrice"].ToString());
                        data.ManufacturingCost = float.Parse(reader["ManufacturingCost"].ToString());


                        data.ShortageCost = float.Parse(reader["ShortageCost"].ToString());
                        data.LeadTime = float.Parse(reader["LeadTime"].ToString());


                        DataList.Add(data);
                    }
                }

                connection.Close();

            }

            return DataList;
        }

        public int SaveItemData(ItemData flatData)
        {
            int ItemId = flatData.ItemId;
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    var existingItemQuery = dbContext.Rmaster.Where(r => r.ItemId == ItemId);
                    var existingItem = existingItemQuery.SingleOrDefault();
                    // Execute the query and get the result

                    if (existingItem != null)
                    {
                        // Insert new item

                        existingItem.ItemCode = flatData.ItemCode;
                        existingItem.ItemDescr = flatData.ItemDescr;
                        existingItem.MesUnit = flatData.MesUnit;
                        existingItem.ItemType = flatData.ItemType.ToString();
                        existingItem.Assembly = flatData.Assembly.ToString();

                        if(flatData.Assembly == BasicEnums.Assembly.Finished)
                        {
                            existingItem.AssemblyNumber = 2;

                        }
                        else if(flatData.Assembly == BasicEnums.Assembly.SemiFinished)
                        {
                            existingItem.AssemblyNumber = 1;

                        }
                        else if (flatData.Assembly == BasicEnums.Assembly.RawMaterial)
                        {
                            existingItem.AssemblyNumber = 0;

                        }

                        existingItem.CanBeProduced = flatData.CanBeProduced;
                        existingItem.InputOrderFlag = flatData.InputOrderFlag;
                        existingItem.OutputOrderFlag = flatData.OutputOrderFlag;
                        existingItem.Profit = flatData.Profit;
                        existingItem.SalesPrice = flatData.SalesPrice;
                        existingItem.ManufacturingCost = flatData.ManufacturingCost;
                        existingItem.HoldingCost = (float)Math.Round(flatData.HoldingCost, 3);
                        existingItem.ShortageCost = flatData.ShortageCost;
                        existingItem.LeadTime = flatData.LeadTime;
                        existingItem.IsDeleted = flatData.IsDeleted;

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
                LogError(ex, "SaveItemData", "Notes");
                return -1;

            }
        }
        public int AddItemData(ItemData FlatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    var existingItemQuery = dbContext.Rmaster.Where(r => r.ItemCode == FlatData.ItemCode);
                    var existingItem = existingItemQuery.SingleOrDefault();
                    // Execute the query and get the result


                    if (existingItem == null)
                    {
                        var newItem = new ItemDataEntity();
                        // Insert new item
                        newItem.ItemCode = FlatData.ItemCode;
                        newItem.ItemDescr = FlatData.ItemDescr;
                        newItem.MesUnit = " ";
                        newItem.ItemType = BasicEnums.ItemType.NoType.ToString();
                        newItem.Assembly = BasicEnums.Assembly.Finished.ToString();
                        newItem.AssemblyNumber = 2;
                        newItem.CanBeProduced = false;
                        newItem.InputOrderFlag = false;
                        newItem.OutputOrderFlag = false;
                        newItem.Profit = 0;
                        newItem.SalesPrice = 0;
                        newItem.ManufacturingCost = 0;
                        newItem.HoldingCost = 0;
                        newItem.ShortageCost = 0;
                        newItem.LeadTime = 0;
                        newItem.IsDeleted = false;
                        int maxLotPolicyId = dbContext.LotPolicy.Max(lp => (int?)lp.LotPolicyId) ?? 0;

                        newItem.LotPolicyId = maxLotPolicyId;


                        dbContext.Rmaster.Add(newItem);

                        dbContext.SaveChanges();


                        #region Add Default Lot Policy

                        var newLot = new LotPolicyDataEntity();

                        var itemId = dbContext.Rmaster
    .Where(r => r.ItemCode == FlatData.ItemCode)
    .Select(r => r.ItemId)
    .SingleOrDefault();

                        // Insert new item

                        newLot.LotPolicyId = maxLotPolicyId + 1;

                        newLot.ItemId = itemId;
                        newLot.LotPolicyCode = "Lot";
                        newLot.LotPolicyDescr = "Lot-For-Lot";
                        newLot.LeadTime = 0;
                        newLot.BatchSize = 0;
                        newLot.Period = 0;
                        newLot.MainPolicy = true;
                        newLot.IsDeleted = false;


                        dbContext.LotPolicy.Add(newLot);

                        dbContext.SaveChanges();
                        #endregion
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
                LogError(ex, "AddItemData", "Notes");
                return 2;

            }
        }

        public ObservableCollection<LotPolicyData> GetLotPolicyChooserData(bool ShowDeleted)
        {
            ObservableCollection<LotPolicyData> DataList = new ObservableCollection<LotPolicyData>();


            string FilterStr = "";
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = String.Format(@"and L.IsDeleted = @ShowDeleted");

                }
                command.CommandText = string.Format(@"SELECT
    R.ItemId,
    R.ItemCode,
    R.IsDeleted,
    L.LotPolicyId,
    L.LotPolicyCode,
    L.LotPolicyDescr,
    L.LeadTime AS LotPolicyLeadTime,
    L.BatchSize,
    L.Period,
    L.MainPolicy,
    L.IsDeleted
FROM 
    LotPolicy AS L
Inner JOIN Rmaster AS R ON L.ItemId = R.ItemId  Where 1=1 {0}
",FilterStr);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LotPolicyData Data = new LotPolicyData();
                        Data.Item = new ItemData();

                        Data.LotPolicyId = int.Parse(reader["LotPolicyId"].ToString());
                        Data.Code = reader["LotPolicyCode"].ToString();
                        Data.Descr = reader["LotPolicyDescr"].ToString();
                        Data.LeadTime = double.Parse(reader["LotPolicyLeadTime"].ToString());
                        Data.BatchSize = double.Parse(reader["BatchSize"].ToString());
                        Data.Period = int.Parse(reader["Period"].ToString());
                        Data.MainPolicy = bool.Parse(reader["MainPolicy"].ToString());

                        Data.Item.ItemId = int.Parse(reader["ItemId"].ToString());
                        Data.Item.ItemCode = reader["ItemCode"].ToString();

                        DataList.Add(Data);
                    }
                }

                connection.Close();

            }

            return DataList;
        }
        public ObservableCollection<LotPolicyData> GetLotPolicySpecificData(ItemData Item)
        {
            ObservableCollection<LotPolicyData> DataList = new ObservableCollection<LotPolicyData>();


            using (var connection = GetConnection())


            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.Parameters.AddWithValue("@ItemId", Item.ItemId);


                command.CommandText = string.Format(@"SELECT
    R.ItemId,
    R.ItemCode,
    L.LotPolicyId,
    L.LotPolicyCode,
    L.LotPolicyDescr,
    L.LeadTime AS LotPolicyLeadTime,
    L.BatchSize,
    L.Period,
    L.MainPolicy
FROM 
    LotPolicy AS L
Inner JOIN Rmaster AS R ON L.ItemId = R.ItemId 
WHERE 
    R.ItemId = @ItemId
");
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LotPolicyData Data = new LotPolicyData();
                        Data.Item = new ItemData();

                        Data.LotPolicyId = int.Parse(reader["LotPolicyId"].ToString());
                        Data.Code = reader["LotPolicyCode"].ToString();
                        Data.Descr = reader["LotPolicyDescr"].ToString();
                        Data.LeadTime = double.Parse(reader["LotPolicyLeadTime"].ToString());
                        Data.BatchSize = double.Parse(reader["BatchSize"].ToString());
                        Data.Period = int.Parse(reader["Period"].ToString());
                        Data.MainPolicy = bool.Parse(reader["MainPolicy"].ToString());

                        Data.Item.ItemId = int.Parse(reader["ItemId"].ToString());
                        Data.Item.ItemCode = reader["ItemCode"].ToString();

                        DataList.Add(Data);
                    }
                }

                connection.Close();

            }

            return DataList;
        }

        public ObservableCollection<ItemData> GetItemsWithSalesData()
        {
            ObservableCollection<ItemData> DataList = new ObservableCollection<ItemData>();


            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = string.Format(@"SELECT ItemId, ItemCode, ItemDescr, MesUnit, ItemType, Assembly, CanBeProduced, InputOrderFlag, OutputOrderFlag
FROM Rmaster
WHERE HasSales = 1;");
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ItemData data = new ItemData();
                        data.ItemId = int.Parse(reader["ItemId"].ToString());

                        data.ItemCode = reader["ItemCode"].ToString();
                        data.ItemDescr = reader["ItemDescr"].ToString();
                        data.MesUnit = reader["MesUnit"].ToString();
                        data.Assembly = (BasicEnums.Assembly)Enum.Parse(typeof(BasicEnums.Assembly), reader["Assembly"].ToString());
                        data.ItemType = (BasicEnums.ItemType)Enum.Parse(typeof(BasicEnums.ItemType), reader["ItemType"].ToString());
                        data.InputOrderFlag = bool.Parse(reader["InputOrderFlag"].ToString());
                        data.OutputOrderFlag = bool.Parse(reader["OutputOrderFlag"].ToString());
                        data.CanBeProduced = bool.Parse(reader["CanBeProduced"].ToString());

                        DataList.Add(data);
                    }
                }

                connection.Close();

            }

            return DataList;
        }

        #region Bill Of Materials
        public ObservableCollection<BomData> GetBomData(string finalItemCode, bool addItemFlag)
        {
            ObservableCollection<BomData> data = new ObservableCollection<BomData>();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.Parameters.AddWithValue("@FinalItemCode", finalItemCode);

                command.CommandText = @"
            Select ItemId from Rmaster 
            Where ItemCode = @FinalItemCode";

                int finalItemId = (int)command.ExecuteScalar();
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@FinalItemId", finalItemId);

                command.CommandText = @"
            select Bom.ItemId, Bom.ComponentId, Rmaster.ItemCode, Rmaster.ItemDescr, Rmaster.ItemType,
            Rmaster.Assembly, Bom.Percentage, Rmaster.MesUnit
            From Rmaster
            Inner Join Bom on Bom.ComponentId = Rmaster.ItemId
            Where Bom.ItemId = @FinalItemId";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ItemData itemData = new ItemData();
                        BomData bomData = new BomData();

                        bomData.FinalItemId = int.Parse(reader["ItemId"].ToString());

                        itemData.ItemId = int.Parse(reader["ComponentId"].ToString());
                        itemData.ItemCode = reader["ItemCode"].ToString();
                        itemData.ItemDescr = reader["ItemDescr"].ToString();
                        itemData.Assembly = (BasicEnums.Assembly)Enum.Parse(typeof(BasicEnums.Assembly), reader["Assembly"].ToString());
                        itemData.ItemType = (BasicEnums.ItemType)Enum.Parse(typeof(BasicEnums.ItemType), reader["ItemType"].ToString());
                        itemData.MesUnit = reader["MesUnit"].ToString();

                        bomData.BomItem = itemData;

                        bomData.BomPercentage = float.Parse(reader["Percentage"].ToString());
                        bomData.BomItemFlag = true;
                        bomData.NewItemFlag = false;
                        bomData.ExistingFlag = true;

                        data.Add(bomData);
                    }
                }

                if (addItemFlag)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@FinalItemId", finalItemId);
                  
                    command.CommandText = @"
SELECT Rmaster.ItemId AS CompItemId, Rmaster.ItemCode, Rmaster.ItemDescr, Rmaster.ItemType,
Rmaster.Assembly, Rmaster.MesUnit
FROM Rmaster
WHERE NOT EXISTS (SELECT 1 FROM Bom WHERE ComponentId = Rmaster.ItemId AND ItemId = @FinalItemId) 
AND Rmaster.Assembly !='Finished' AND Rmaster.isDeleted = 0";


                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ItemData itemData = new ItemData();
                            BomData bomData = new BomData();

                            bomData.FinalItemId = finalItemId;

                            itemData.ItemId = int.Parse(reader["CompItemId"].ToString());
                            itemData.ItemCode = reader["ItemCode"].ToString();
                            itemData.ItemDescr = reader["ItemDescr"].ToString();
                            itemData.Assembly = (BasicEnums.Assembly)Enum.Parse(typeof(BasicEnums.Assembly), reader["Assembly"].ToString());
                            itemData.ItemType = (BasicEnums.ItemType)Enum.Parse(typeof(BasicEnums.ItemType), reader["ItemType"].ToString());
                            itemData.MesUnit = reader["MesUnit"].ToString();

                            bomData.BomItem = itemData;

                            bomData.BomPercentage = 0;
                            bomData.BomItemFlag = false;
                            bomData.NewItemFlag = true;
                            bomData.ExistingFlag = false;

                            data.Add(bomData);
                        }
                    }
                }

                connection.Close();
                return data;
            }
        }
        public bool SaveBomData(ObservableCollection<BomData> Data, string FinalItemCode)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Retrieve the final item from the Rmaster table
                    var finalItem = dbContext.Rmaster.SingleOrDefault(r => r.ItemCode == FinalItemCode);

                    if (finalItem == null)
                    {
                        // Final item not found
                        return false;
                    }

                    int finalItemId = finalItem.ItemId;

                    int result = 0;
                    foreach (var row in Data)
                    {
                        int bomItemId = row.BomItem.ItemId;
                        var existingBom = dbContext.Bom.SingleOrDefault(b => b.ItemId == finalItemId && b.ComponentId == bomItemId);

                        if (existingBom == null && row.BomItemFlag == true && row.NewItemFlag == true)
                        {
                            // Insert new bom
                            BomDataEntity newBom = new BomDataEntity
                            {
                                ItemId = finalItemId,
                                ComponentId = bomItemId,
                                Percentage = (float)row.BomPercentage
                            };

                            dbContext.Bom.Add(newBom);
                            result += 1;
                        }
                        else if (row.ExistingFlag == true && row.BomItemFlag == false)
                        {
                            dbContext.Bom.Remove(existingBom);

                        }
                        else if (row.ExistingFlag == true && row.BomItemFlag == true)
                        {
                            // Update existing bom
                            existingBom.Percentage = (float)row.BomPercentage;
                        }
                    }

                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "SaveBomData", "Notes");
                return false;
            }
        }
        #endregion

        #region Production Process Flow
        public ObservableCollection<ItemProcessData> GetPPFData(string finalCode, bool addFlag)
        {
            ObservableCollection<ItemProcessData> data = new ObservableCollection<ItemProcessData>();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.Parameters.AddWithValue("@FinalCode", finalCode);

                command.CommandText = @"
            Select ItemId from Rmaster 
            Where ItemCode = @FinalCode";

                int finalId = (int)command.ExecuteScalar();
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@FinalId", finalId);

                command.CommandText = @"
            select PPF.ItemId, PPF.MachineId, Machines.MachCode, Machines.MachDescr,Machines.MachineType,PPF.ProductionTime,PPF.MachineOrder
            From Machines
            Inner Join ProductionProcessFlow as PPF on PPF.MachineId = Machines.MachID
            Where PPF.ItemId = @FinalId 
Order By PPF.MachineOrder ";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MachineData machData = new MachineData();
                        ItemProcessData Data = new ItemProcessData();

                        Data.Finalid = int.Parse(reader["ItemId"].ToString());

                        machData.MachID = int.Parse(reader["MachineId"].ToString());
                        machData.MachCode = reader["MachCode"].ToString();
                        machData.MachDescr = reader["MachDescr"].ToString();
                        machData.MachineType = (BasicEnums.MachType)Enum.Parse(typeof(BasicEnums.MachType), reader["MachineType"].ToString());


                        Data.Machine = machData;

                        Data.ProductionTime = float.Parse(reader["ProductionTime"].ToString());
                        Data.MachineOrder = int.Parse(reader["MachineOrder"].ToString());

                        Data.ClassicFlag = true;
                        Data.NewProcessFlag = false;
                        Data.ExistingFlag = true;

                        data.Add(Data);
                    }
                }

                if (addFlag)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@FinalId", finalId);

                    command.CommandText = @"
SELECT 
    Machines.MachID, 
    Machines.MachCode, 
    Machines.MachDescr,
    Machines.MachineType, 
    Machines.ProductionRate
FROM 
    Machines
WHERE 
    NOT EXISTS (
        SELECT 1 
        FROM ProductionProcessFlow as PPF 
        WHERE ItemId = @FinalId and MachineId= Machines.MachID
    ) 
    AND Machines.PrimaryModel = 1 And Machines.isDeleted = 0";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MachineData machData = new MachineData();
                            ItemProcessData Data = new ItemProcessData();

                            Data.Finalid = finalId;

                            machData.MachID = int.Parse(reader["MachID"].ToString());
                            machData.MachCode = reader["MachCode"].ToString();
                            machData.MachDescr = reader["MachDescr"].ToString();
                            machData.MachineType = (BasicEnums.MachType)Enum.Parse(typeof(BasicEnums.MachType), reader["MachineType"].ToString());


                            Data.Machine = machData;

                            Data.ProductionTime = 0;
                            Data.MachineOrder = 0;

                            Data.ClassicFlag = false;

                            Data.ExistingFlag = false;
                            Data.NewProcessFlag = true;

                            data.Add(Data);
                        }
                    }
                }

                connection.Close();
                return data;
            }
        }
        public bool SavePPFData(ObservableCollection<ItemProcessData> Data, string FinalItemCode)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Retrieve the final item from the Rmaster table
                    var finalItem = dbContext.Rmaster.SingleOrDefault(r => r.ItemCode == FinalItemCode);

                    if (finalItem == null)
                    {
                        return false;
                    }

                    int finalItemId = finalItem.ItemId;

                    int result = 0;
                    foreach (var row in Data)
                    {
                        int MachineId = row.Machine.MachID;
                        var existingProcess = dbContext.ProductionProcessFlow.SingleOrDefault(b => b.Item.ItemId == finalItemId && b.Machine.MachID == MachineId);


                        if (existingProcess == null && row.ClassicFlag == true && row.NewProcessFlag == true)
                        {
                            // Insert new bom
                            ItemProcessDataEntity newProcess = new ItemProcessDataEntity
                            {
                                ItemId = finalItemId,
                                MachineId = MachineId,
                                MachineOrder = row.MachineOrder,
                                ProductionTime = row.ProductionTime
                            };

                            dbContext.ProductionProcessFlow.Add(newProcess);
                            result += 1;
                        }
                        else if (row.ExistingFlag == true && row.ClassicFlag == false)
                        {
                            dbContext.ProductionProcessFlow.Remove(existingProcess);

                        }
                        else if (row.ExistingFlag == true && row.ClassicFlag == true)
                        {
                            // Update existing bom
                            existingProcess.MachineOrder = row.MachineOrder;
                            existingProcess.ProductionTime = row.ProductionTime;
                        }
                    }
                    dbContext.SaveChanges();
                    return true;

                }
            }
            catch (Exception ex)
            {
                LogError(ex, "SavePPFData", "Notes");
                return false;
            }
        }
        #endregion
        #endregion

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
                command.CommandText = string.Format(@"select CountryId,CountryCode,CountryDescr,IsDeleted from Country Where 1=1 {0}",FilterStr);
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
                                                    Inner JOIN  Country on Prefecture.CountryId = Country.CountryId Where 1=1 {0}",FilterStr);
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
                        else if (existingrow !=null)
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

        #region Routes

        public bool SaveRoutesData(ObservableCollection<RoutesData> Data)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {


                    bool hasChanges = false;
                    foreach (var row in Data)
                    {
                        var existingrow = dbContext.Routes.SingleOrDefault(b => b.RouteId == row.RoutesId);

                        if (existingrow == null)
                        {
                            RoutesDataEntity newrow = new RoutesDataEntity();
                            newrow.CityFrom = row.CityFrom.CityId;
                            newrow.CityTo = row.CityTo.CityId;
                            newrow.AverageTravelingSpeed = row.AvgSpeed;
                            newrow.Distance = row.Distance;

                            newrow.IsDeleted = false;
                            dbContext.Routes.Add(newrow);
                            hasChanges = true;
                        }
                        else if (existingrow != null)
                        {

                            existingrow.CityFrom = row.CityFrom.CityId;
                            existingrow.CityTo = row.CityTo.CityId;
                            existingrow.AverageTravelingSpeed = row.AvgSpeed;
                            existingrow.Distance = row.Distance;

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
                LogError(ex, "SaveRoutesData", "Notes");
                return false;
            }
        }

        public ObservableCollection<RoutesData> GetRoutesData(bool ShowDeleted)
        {
            ObservableCollection<RoutesData> DataList = new ObservableCollection<RoutesData>();
            string FilterStr = "";
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;


                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = String.Format(@" and Routes.IsDeleted =@ShowDeleted");
                }

                command.CommandText = string.Format(@"select Routes.RouteId,Routes.IsDeleted,CityFrom.CityId as CityFromId,CityFrom.CityCode as CityFromCode, CityFrom.CityDescr as CityFromDescr, PrefFrom.PrefCode as PrefFromCode, PrefFrom.PrefDescr as PrefFromDescr, CountryFrom.CountryCode as CountryFromCode, CountryFrom.CountryDescr as CountryFromDescr, CityFrom.Longitude as CityFromLongitude, CityFrom.Latitude as CityFromLatitude, 
                                               CityTo.CityId as CityToId,CityTo.CityCode as CityToCode, CityTo.CityDescr as CityToDescr, PrefTo.PrefCode as PrefToCode, PrefTo.PrefDescr as PrefToDescr, CountryTo.CountryCode as CountryToCode, CountryTo.CountryDescr as CountryToDescr, CityTo.Longitude as CityToLongitude, CityTo.Latitude as CityToLatitude,
                                               Distance, AverageTravelingSpeed as AvgSpeed
                                               from Routes
                                               inner join City as CityFrom on Routes.CityFrom = CityFrom.CityId
                                               inner join Prefecture as PrefFrom on CityFrom.PrefId = PrefFrom.PrefId
                                               inner join Country as CountryFrom on PrefFrom.CountryId = CountryFrom.CountryId
                                               inner join City as CityTo on Routes.CityTo = CityTo.CityId
                                               inner join Prefecture as PrefTo on CityTo.PrefId = PrefTo.PrefId
                                               inner join Country as CountryTo on PrefTo.CountryId = CountryTo.CountryId
Where 1=1 {0}",FilterStr);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RoutesData data = new RoutesData();
                        data.RoutesId = int.Parse(reader["RouteId"].ToString());
                        data.Distance = float.Parse(reader["Distance"].ToString());
                        data.AvgSpeed = int.Parse(reader["AvgSpeed"].ToString());
                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());

                        CityData cityFrom = new CityData();
                        cityFrom.CityId = int.Parse(reader["CityFromId"].ToString());
                        cityFrom.CityCode = reader["CityFromCode"].ToString();
                        cityFrom.CityDescr = reader["CityFromDescr"].ToString();
                        cityFrom.PrefCode = reader["PrefFromCode"].ToString();
                        cityFrom.PrefDescr = reader["PrefFromDescr"].ToString();
                        cityFrom.CountryCode = reader["CountryFromCode"].ToString();
                        cityFrom.CountryDescr = reader["CountryFromDescr"].ToString();
                        cityFrom.Longitude = float.Parse(reader["CityFromLongitude"].ToString());
                        cityFrom.Latitude = float.Parse(reader["CityFromLatitude"].ToString());
                        data.CityFrom = cityFrom;

                        CityData cityTo = new CityData();
                        cityTo.CityId = int.Parse(reader["CityToId"].ToString());
                        cityTo.CityCode = reader["CityToCode"].ToString();
                        cityTo.CityDescr = reader["CityToDescr"].ToString();
                        cityTo.PrefCode = reader["PrefToCode"].ToString();
                        cityTo.PrefDescr = reader["PrefToDescr"].ToString();
                        cityTo.CountryCode = reader["CountryToCode"].ToString();
                        cityTo.CountryDescr = reader["CountryToDescr"].ToString();
                        cityTo.Longitude = float.Parse(reader["CityToLongitude"].ToString());
                        cityTo.Latitude = float.Parse(reader["CityToLatitude"].ToString());
                        data.CityTo = cityTo;



                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }
        #endregion

        #region Vehicles

        public ObservableCollection<CountryData> GetVehicleData(bool ShowDeleted)
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

        public bool SaveVehicleData(ObservableCollection<CountryData> Data)
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

        #endregion

        #region Customer

        #region CustomerInfo 1st Tab



        public int SaveCustomerInfoData(CustomerData flatData) 
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    int customerId = flatData.Id;
                    var existingCustomerQuery = dbContext.Customer.Where(c => c.CustomerId == customerId);
                    var existingCustomer = existingCustomerQuery.SingleOrDefault();
                    // Execute the query and get the result
                    var CityIdQuery2 = dbContext.City.Where(c => c.CityCode == flatData.City.CityCode);
                    var City = CityIdQuery2.SingleOrDefault();

                    if (existingCustomer != null)
                    {


                        // Update existing customer
                        existingCustomer.CustomerCode = flatData.Code;
                        existingCustomer.CustomerDescr = flatData.Descr;
                        existingCustomer.Email = flatData.Email;
                        existingCustomer.Phone = flatData.Phone;
                        existingCustomer.Adress = flatData.Adress;
                        existingCustomer.CustomerType = flatData.CustomerType.ToString();
                        existingCustomer.PostalCode = flatData.PostalCode;
                        existingCustomer.PromptPayer = flatData.PromptPayer;
                        existingCustomer.IsDeleted = flatData.IsDeleted;



                        existingCustomer.CityId = City.CityId; // You may need to adjust this according to your database structure
                        existingCustomer.PriceListId = flatData.PriceList.Id; // Ayto prepei na fugei

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
                LogError(ex, "SaveCustomerData", "Notes");
                return -1;
            }
        }
        public int AddCustomerData(CustomerData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    var existingItemQuery = dbContext.Customer.Where(r => r.CustomerCode == flatData.Code);
                    var existingItem = existingItemQuery.SingleOrDefault();
                    // Execute the query and get the result


                    if (existingItem == null)
                    {
                        var newItem = new CustomerDataEntity();
                        // Insert new item
                        newItem.CustomerCode = flatData.Code;
                        newItem.CustomerDescr = flatData.Descr;
                        newItem.Email = " ";
                        newItem.Phone = 0;
                        newItem.Adress = " ";
                        newItem.CustomerType = " ";
                        newItem.CityId = 1;
                        newItem.PriceListId = 2;
                        newItem.PromptPayer = false;
                        newItem.CustomerType = "Retail";
                        newItem.IsDeleted = false;


                        dbContext.Customer.Add(newItem);

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
                LogError(ex, "AddCustomerData", "Notes");
                return 2;

            }
        }

        public CustomerData GetCustomerChooserData(int Id ,string Code)
        {
            CustomerData FlatData = new CustomerData();
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
                        FilterStr = String.Format(@" and Customer.CustomerId =@ID");

                    }

                    else if (!string.IsNullOrWhiteSpace(Code))
                    {
                        command.Parameters.AddWithValue("@Code", Code);
                        FilterStr = String.Format(@" and Customer.CustomerCode =@Code");

                    }
                    command.CommandText = string.Format(@"SELECT Customer.CustomerId, Customer.CustomerCode, Customer.CustomerDescr, Customer.Email, Customer.Phone, Customer.Adress, Customer.CityId, Customer.CustomerType,Customer.PostalCode, Customer.PromptPayer,
                                                  City.CityCode, City.CityDescr, Prefecture.PrefCode, Prefecture.PrefDescr,
                                                  Country.CountryCode, Country.CountryDescr,
                                                  CPriceList.PriceListCode, CPriceList.PriceListDescr,CPriceList.PriceListId
                                                  FROM Customer
                                                  INNER JOIN City ON Customer.CityId = City.CityId
                                                  INNER JOIN Prefecture ON City.PrefId = Prefecture.PrefId
                                                  INNER JOIN Country ON Prefecture.CountryId = Country.CountryId
                                                  FULL JOIN CPriceList ON CPriceList.PriceListId = Customer.PriceListId
                                                  WHERE 1=1 {0}",FilterStr);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            CityData City = new CityData();
                            PrefectureData Prefecture = new PrefectureData();
                            CountryData Country = new CountryData();
                            PriceListData PriceList = new PriceListData();

                            FlatData.Id = int.Parse(reader["CustomerId"].ToString());
                            FlatData.Code = reader["CustomerCode"].ToString();
                            FlatData.Descr = reader["CustomerDescr"].ToString();
                            FlatData.Email = reader["Email"].ToString();
                            FlatData.Phone = int.Parse(reader["Phone"].ToString());
                            FlatData.Adress = reader["Adress"].ToString();
                            FlatData.CustomerType = (BasicEnums.CustomerType)Enum.Parse(typeof(BasicEnums.CustomerType), reader["CustomerType"].ToString());

                            FlatData.PostalCode = reader["PostalCode"].ToString();


                            FlatData.PromptPayer = bool.Parse(reader["PromptPayer"].ToString());



                            City.CityCode = reader["CityCode"].ToString();
                            City.CityDescr = reader["CityDescr"].ToString();
                            Prefecture.PrefCode = reader["PrefCode"].ToString();
                            Prefecture.PrefDescr = reader["PrefDescr"].ToString();
                            Country.CountryCode = reader["CountryCode"].ToString();
                            Country.CountryDescr = reader["CountryDescr"].ToString();

                            PriceList.Id = int.Parse(reader["PriceListId"].ToString());
                            PriceList.Code = reader["PriceListCode"].ToString();
                            PriceList.Descr = reader["PriceListDescr"].ToString();

                            FlatData.City = City;
                            FlatData.Prefecture = Prefecture;
                            FlatData.Country = Country;

                            FlatData.PriceList = PriceList;
                        }
                    }

                    connection.Close();
                }

                return FlatData;
            }
            catch (Exception ex)
            {
                LogError(ex, "GetCustomerChooserData", "Notes");
                return null;
            }
        }


        public ObservableCollection<CustomerData> GetCustomerData(bool ShowDeleted)
        {
            ObservableCollection<CustomerData> DataList = new ObservableCollection<CustomerData>();

            string FilterStr = "";

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = String.Format(@" and Customer.IsDeleted =@ShowDeleted");
                }
                command.CommandText = string.Format(@"SELECT Customer.CustomerId, Customer.CustomerCode, Customer.CustomerDescr, Customer.Email,Customer.PostalCode, Customer.Phone, Customer.Adress, Customer.CityId, Customer.CustomerType, Customer.PromptPayer,Customer.IsDeleted,
                                              City.CityId, City.CityCode, City.CityDescr, Prefecture.PrefId, Prefecture.PrefCode, Prefecture.PrefDescr,
                                              Country.CountryId, Country.CountryCode, Country.CountryDescr,
                                              CPriceList.PriceListId, CPriceList.PriceListCode, CPriceList.PriceListDescr
                                              FROM Customer
                                              INNER JOIN City ON Customer.CityId = City.CityId
                                              INNER JOIN Prefecture ON City.PrefId = Prefecture.PrefId
                                              INNER JOIN Country ON Prefecture.CountryId = Country.CountryId
                                              INNER JOIN CPriceList ON CPriceList.PriceListId = Customer.PriceListId
                                              Where 1=1 {0}",FilterStr);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CustomerData data = new CustomerData();

                        CityData City = new CityData();
                        PrefectureData Prefecture = new PrefectureData();
                        CountryData Country = new CountryData();
                        PriceListData PriceList = new PriceListData();

                        data.Id = int.Parse(reader["CustomerId"].ToString());
                        data.Code = reader["CustomerCode"].ToString();
                        data.Descr = reader["CustomerDescr"].ToString();
                        data.Email = reader["Email"].ToString();
                        data.Phone = int.Parse(reader["Phone"].ToString());
                        data.Adress = reader["Adress"].ToString();
                        data.PostalCode = reader["PostalCode"].ToString();
                        data.CustomerType = (BasicEnums.CustomerType)Enum.Parse(typeof(BasicEnums.CustomerType), reader["CustomerType"].ToString());

                        data.PromptPayer = !string.IsNullOrWhiteSpace(reader["PromptPayer"].ToString()) && bool.Parse(reader["PromptPayer"].ToString());
                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());

                        City.CityCode = reader["CityCode"].ToString();
                        City.CityDescr = reader["CityDescr"].ToString();

                        Prefecture.PrefCode = reader["CityDescr"].ToString();
                        Prefecture.PrefDescr = reader["PrefDescr"].ToString();

                        Country.CountryCode = reader["CountryCode"].ToString();
                        Country.CountryDescr = reader["CountryDescr"].ToString();

                        data.City = City;
                        data.Prefecture = Prefecture;
                        data.Country = Country;

                        PriceList.Id = int.Parse(reader["PriceListId"].ToString());
                        PriceList.Code = reader["PriceListCode"].ToString();
                        PriceList.Descr = reader["PriceListDescr"].ToString();

                        data.PriceList = PriceList;




                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }
        public ObservableCollection<SupplierInfoData> GetCustomerInfoSearchData(SupplierInfoSearchFilterData Filter)
        {
            ObservableCollection<SupplierInfoData> DataList = new ObservableCollection<SupplierInfoData>();
            String str = "";


            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                #region Filters

                if (!string.IsNullOrWhiteSpace(Filter.SupplierCodeFrom))
                {
                    str = String.Concat(str, "and S.SupCode Between @SupCodeFrom and @SupCodeTo");
                    command.Parameters.AddWithValue("@SupCodeFrom", Filter.SupplierCodeFrom);
                    command.Parameters.AddWithValue("@SupCodeTo", Filter.SupplierCodeTo);

                }


                if (!string.IsNullOrWhiteSpace(Filter.CountryCodeFrom))
                {
                    str = String.Concat(str, "and Country.CountryCode Between @CountryCodeFrom and @CountryCodeTo");
                    command.Parameters.AddWithValue("@CountryCodeFrom", Filter.CountryCodeFrom);
                    command.Parameters.AddWithValue("@CountryCodeTo", Filter.CountryCodeTo);

                }

                if (!string.IsNullOrWhiteSpace(Filter.PrefectureCodeFrom))
                {
                    str = String.Concat(str, "and Pref.PrefCode Between @PrefCodeFrom and @PrefCodeTo");
                    command.Parameters.AddWithValue("@PrefCodeFrom", Filter.PrefectureCodeFrom);
                    command.Parameters.AddWithValue("@PrefCodeTo", Filter.PrefectureCodeTo);
                }


                if (!string.IsNullOrWhiteSpace(Filter.CityCodeFrom))
                {
                    str = String.Concat(str, "and City.CityCode Between @CityCodeFrom and @CityCodeTo");
                    command.Parameters.AddWithValue("@CityCodeFrom", Filter.CityCodeFrom);
                    command.Parameters.AddWithValue("@CityCodeTo", Filter.CityCodeTo);

                }


                #endregion

                command.CommandText = string.Concat(@"select S.SupCode,S.SupDescr,S.SupEmail,S.SupPhone,S.SupAdress,
S.SupCityId,S.SupType,
Country.CountryCode,Country.CountryDescr,Pref.PrefCode,Pref.PrefDescr , City.CityCode,City.CityDescr
from Supplier as S
Inner Join City on S.SupCityId = City.CityId
Inner Join Prefecture as Pref on Pref.PrefId = City.PrefId
Inner Join Country on Country.CountryId = Pref.PrefId
Where 1=1 ", str);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SupplierInfoData data = new SupplierInfoData();

                        data.SupplierCode = reader["SupCode"].ToString();
                        data.SupplierDescr = reader["SupDescr"].ToString();
                        data.Email = reader["SupEmail"].ToString();
                        data.Phone = reader["SupPhone"].ToString();
                        data.Adress1 = reader["SupAdress"].ToString();
                        data.SupplierType = reader["SupType"].ToString();
                        data.CountryCode = reader["CountryCode"].ToString();

                        data.CountryDescr = reader["CountryDescr"].ToString();
                        data.PrefCode = reader["PrefCode"].ToString();
                        data.PrefDescr = reader["PrefDescr"].ToString();
                        data.CityCode = reader["CityCode"].ToString();
                        data.CityDescr = reader["CityDescr"].ToString();
                        DataList.Add(data);
                    }
                }

                connection.Close();

            }

            return DataList;
        }

        #endregion

        #region CustomerInfo 2nd Tab /Items per Supplier

        //        public bool SaveItemPerSupData(ObservableCollection<SupItemData> Data, string SupplierCode)
        //        {
        //            ObservableCollection<CountryData> DataList = new ObservableCollection<CountryData>();

        //            try
        //            {

        //                using (var connection = GetConnection())
        //                using (var command = new SqlCommand())
        //                {
        //                    connection.Open();
        //                    command.Connection = connection;
        //                    command.Parameters.AddWithValue("@SupCode", SupplierCode);
        //                    command.CommandText = string.Format(@"Select SupId from Supplier Where SupCode=@SupCode ");
        //                    command.Parameters.AddWithValue("@SupplierId", command.ExecuteScalar());




        //                    foreach (var item in Data)
        //                    {
        //                        if (item.SupplierFlag == true)
        //                        {
        //                            command.Parameters.AddWithValue("@ItemId", item.ItemId);

        //                            command.CommandText = string.Format(@"insert  into SupRmaster
        //                                                            (ItemId,SupplierId)
        //                                                            SELECT @ItemId,@SupplierId
        //                                                            WHERE NOT EXISTS 
        //(SELECT 1 FROM SupRmaster WHERE ItemId = @ItemId AND SupplierId = @SupplierId)");
        //                            command.ExecuteScalar();
        //                            command.Parameters.RemoveAt(command.Parameters.IndexOf("@ItemId"));

        //                        }
        //                    }


        //                    connection.Close();

        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                return false;
        //            }

        //            return true;

        //        }
        //        public ObservableCollection<SupItemData> GetItemPerSupData(string SupplierCode)
        //        {
        //            ObservableCollection<SupItemData> DataList = new ObservableCollection<SupItemData>();

        //            using (var connection = GetConnection())
        //            using (var command = new SqlCommand())
        //            {
        //                connection.Open();
        //                command.Connection = connection;

        //                command.Parameters.AddWithValue("@SupplierCode", SupplierCode);

        //                command.CommandText = string.Format(@"Select SupId from Supplier 
        //Where SupCode = @SupplierCode");

        //                //EXECUTE SCALAR

        //                command.Parameters.AddWithValue("@SupplierId", command.ExecuteScalar());

        //                command.CommandText = string.Format(@"select ItemId,ItemCode,ItemDescr,MesUnit,ItemType,Assembly 
        //                                                    from Rmaster
        //                                                    Where Assembly < 4");
        //                using (var reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        SupItemData data = new SupItemData();



        //                        data.ItemId = int.Parse(reader["ItemId"].ToString());

        //                        data.ItemCode = reader["ItemCode"].ToString();
        //                        data.ItemDescr = reader["ItemDescr"].ToString();
        //                        data.MesUnit = reader["MesUnit"].ToString();
        //                        data.ItemType = reader["ItemType"].ToString();
        //                        data.Assembly = reader["Assembly"].ToString();
        //                        data.SupplierFlag = false;

        //                        DataList.Add(data);
        //                    }
        //                }

        //                foreach (var item in DataList)
        //                {
        //                    command.Parameters.AddWithValue("@ItemId2", item.ItemId);


        //                    command.CommandText = string.Format(@"select SupCode from  SupRmaster 
        //Inner Join Supplier as Sup on Sup.SupId = SupRmaster.SupplierId
        //Where Sup.SupCode =@SupplierCode and SupRmaster.ItemId =@ItemId2");


        //                    string result = (string)command.ExecuteScalar();

        //                    if (!result.IsNullOrWhiteSpace())
        //                    {
        //                        item.SupplierFlag = true;


        //                    }
        //                    else
        //                    {
        //                        item.SupplierFlag = false;
        //                    }

        //                    command.Parameters.RemoveAt(command.Parameters.IndexOf("@ItemId2"));


        //                }
        //                connection.Close();
        //            }
        //            return DataList;
        //        }

        #endregion


        #region PriceList
        public ObservableCollection<PriceListData> GetPriceListChooserData(bool ShowDeleted)
        {
            ObservableCollection<PriceListData> DataList = new ObservableCollection<PriceListData>();

            string FilterStr= "";

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

                command.CommandText = string.Format(@"SELECT PriceListId,PriceListCode,PriceListDescr,IsDeleted,
Retail,Wholesale,DateStart,DateEnd,IsDeleted
From CPriceList Where 1=1 {0}", FilterStr);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PriceListData data = new PriceListData();
                        data.Id = int.Parse(reader["PriceListId"].ToString());

                        data.Code = reader["PriceListCode"].ToString();
                        data.Descr = reader["PriceListDescr"].ToString();
                        data.Retail = bool.Parse(reader["Retail"].ToString());
                        data.Wholesale = bool.Parse(reader["Wholesale"].ToString());
                        data.DateStart = DateTime.Parse(reader["DateStart"].ToString());
                        data.DateEnd = DateTime.Parse(reader["DateEnd"].ToString());
                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());


                        DataList.Add(data);


                    }
                }

                connection.Close();

            }

            return DataList;
        }

        public PriceListData GetPriceListData(PriceListData Filter)
        {
            PriceListData FlatData = new PriceListData();
            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;

                    command.Parameters.AddWithValue("@PriceListCode", Filter.Code);

                    command.CommandText = string.Format(@"SELECT CPriceList.PriceListId,CPriceList.PriceListCode,CPriceList.PriceListDescr,
CPriceList.Retail,CPriceList.Wholesale,CPriceList.DateStart,CPriceList.DateEnd
From CPriceList
Where CpricelIST.PriceListCode=@PriceListCode");
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            FlatData.Id = int.Parse(reader["PriceListId"].ToString());
                            FlatData.Code = reader["PriceListCode"].ToString();
                            FlatData.Descr = reader["PriceListDescr"].ToString();
                            FlatData.Retail = bool.Parse(reader["Retail"].ToString());
                            FlatData.Wholesale = bool.Parse(reader["Wholesale"].ToString());
                            FlatData.DateStart = DateTime.Parse(reader["DateStart"].ToString());
                            FlatData.DateEnd = DateTime.Parse(reader["DateEnd"].ToString());

                        }
                    }

                    connection.Close();
                }

                return FlatData;
            }
            catch (Exception ex)
            {
                LogError(ex, "GetPriceListData", "Notes");
                return null;
            }
        }
        public ObservableCollection<CustomerData> GetCustomersOfPriceListData(int PriceListId)
        {
            ObservableCollection<CustomerData> DataList = new ObservableCollection<CustomerData>();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.Parameters.AddWithValue("@PriceListId", PriceListId);

                command.CommandText = string.Format(@"SELECT Customer.CustomerId, Customer.CustomerCode, Customer.CustomerDescr, Customer.Email, Customer.Phone, Customer.Adress, Customer.CityId, Customer.CustomerType, Customer.PromptPayer,
                                              City.CityId, City.CityCode, City.CityDescr, Prefecture.PrefId, Prefecture.PrefCode, Prefecture.PrefDescr, Country.CountryId, Country.CountryCode, Country.CountryDescr
                                              FROM Customer
                                              INNER JOIN City ON Customer.CityId = City.CityId
                                              INNER JOIN Prefecture ON City.PrefId = Prefecture.PrefId
                                              INNER JOIN Country ON Prefecture.CountryId = Country.CountryId
                                              WHERE Customer.PriceListId = @PriceListId");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CustomerData data = new CustomerData();
                        data.Id = int.Parse(reader["CustomerId"].ToString());

                        data.Code = reader["CustomerCode"].ToString();
                        data.Descr = reader["CustomerDescr"].ToString();
                        data.Email = reader["Email"].ToString();
                        data.Phone = int.Parse(reader["Phone"].ToString());
                        data.Adress = reader["Adress"].ToString();
                        data.CustomerType = (BasicEnums.CustomerType)Enum.Parse(typeof(BasicEnums.CustomerType), reader["CustomerType"].ToString());
                        data.PromptPayer = !string.IsNullOrWhiteSpace(reader["PromptPayer"].ToString()) && bool.Parse(reader["PromptPayer"].ToString());

                        data.City = new CityData
                        {
                            CityCode = reader["CityCode"].ToString(),
                            CityDescr = reader["CityDescr"].ToString()
                        };

                        data.Prefecture = new PrefectureData
                        {
                            PrefCode = reader["PrefCode"].ToString(),
                            PrefDescr = reader["PrefDescr"].ToString()
                        };

                        data.Country = new CountryData
                        {
                            CountryCode = reader["CountryCode"].ToString(),
                            CountryDescr = reader["CountryDescr"].ToString()
                        };

                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }

        public ObservableCollection<PriceListItemData> GetPriceListItemData(PriceListData PriceListData, bool AddItemFlag)
        {
            ObservableCollection<PriceListItemData> DataList = new ObservableCollection<PriceListItemData>();
            String FilterStr = "";
            var a = PriceListData.Id.ToString();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                if (PriceListData.Id != 0)
                {
                    command.Parameters.AddWithValue("@Id", PriceListData.Id);
                    FilterStr = String.Format(@"and CPriceListItems.Id = @Id");
                }

                command.CommandText = string.Format(@"select Rmaster.ItemId,Rmaster.ItemCode,Rmaster.ItemDescr,Rmaster.Assembly,Rmaster.ItemType,
CPriceListItems.UnitCost,
CPriceListItems.Discount ,CPriceListItems.Qmin,Rmaster.MesUnit
from CPriceListItems
Inner Join Rmaster on Rmaster.ItemId = CPriceListItems.ItemId
Where 1=1 {0}", FilterStr);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PriceListItemData data = new PriceListItemData();
                        ItemData itemdata = new ItemData();
                        List<string> ExistingItems = new List<string>();


                        itemdata.ItemId = int.Parse(reader["ItemId"].ToString());
                        itemdata.ItemCode = reader["ItemCode"].ToString();
                        itemdata.ItemDescr = reader["ItemDescr"].ToString();
                        itemdata.Assembly = (BasicEnums.Assembly)Enum.Parse(typeof(BasicEnums.Assembly), reader["Assembly"].ToString());
                        itemdata.ItemType = (BasicEnums.ItemType)Enum.Parse(typeof(BasicEnums.ItemType), reader["ItemType"].ToString());
                        itemdata.MesUnit = reader["MesUnit"].ToString();

                        data.SalesPrice = float.Parse(reader["UnitCost"].ToString());
                        data.Discount = float.Parse(reader["Discount"].ToString());
                        data.Qmin = float.Parse(reader["Qmin"].ToString());
                        data.Item = itemdata;
                        data.IsChecked = true;
                        data.Existing = true;


                        DataList.Add(data);

                        ExistingItems.Add(itemdata.ItemCode);


                    }
                }
                if (AddItemFlag == true)
                {
                    command.CommandText = string.Format(@"SELECT ItemId, ItemCode, ItemDescr, Assembly, ItemType,MesUnit
FROM Rmaster as Rm1
WHERE  Rm1.ItemId NOT IN (
    SELECT Rm2.ItemId
    FROM CPriceListItems
    INNER JOIN Rmaster as Rm2 ON Rm2.ItemId = CPriceListItems.ItemId
    WHERE  Rm2.IsDeleted = 0 {0}
)", FilterStr);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ItemData itemData = new ItemData();
                            PriceListItemData data = new PriceListItemData();


                            itemData.ItemId = int.Parse(reader["ItemId"].ToString());
                            itemData.ItemCode = reader["ItemCode"].ToString();
                            itemData.ItemDescr = reader["ItemDescr"].ToString();
                            itemData.Assembly = (BasicEnums.Assembly)Enum.Parse(typeof(BasicEnums.Assembly), reader["Assembly"].ToString());
                            itemData.ItemType = (BasicEnums.ItemType)Enum.Parse(typeof(BasicEnums.ItemType), reader["ItemType"].ToString());
                            itemData.MesUnit = reader["MesUnit"].ToString();

                            data.SalesPrice = 0;
                            data.Discount = 0;
                            data.Qmin = 0;
                            data.Item = itemData;
                            data.IsChecked = false;
                            data.Existing = false;

                            DataList.Add(data);
                        }
                    }
                    connection.Close();

                }

                return DataList;
            }
        }

        public int AddPriceListData(PriceListData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    var existingItemQuery = dbContext.CPriceList.Where(r => r.PriceListCode == flatData.Code);
                    var existingItem = existingItemQuery.SingleOrDefault();
                    // Execute the query and get the result


                    if (existingItem == null)
                    {
                        var newItem = new CPriceListEntity();
                        // Insert new item
                        newItem.PriceListCode = flatData.Code;
                        newItem.PriceListDescr = flatData.Descr;
                        newItem.Retail = false;
                        newItem.Wholesale = false;
                        newItem.DateStart = DateTime.Now;
                        newItem.DateEnd = DateTime.Now.AddYears(1);
                        newItem.IsDeleted = false;


                        dbContext.CPriceList.Add(newItem);

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
                LogError(ex, "AddPriceListData", "Notes");
                return 2;

            }
        }


        public int SavePriceListData(PriceListData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    int PriceListId = flatData.Id;
                    var existingItemQuery = dbContext.CPriceList.Where(r => r.PriceListId == PriceListId);
                    var existingItem = existingItemQuery.SingleOrDefault();
                    // Execute the query and get the result

                    if (existingItem != null)
                    {

                        // Update existing item
                        existingItem.PriceListCode = flatData.Code;
                        existingItem.PriceListDescr = flatData.Descr;
                        existingItem.Retail = flatData.Retail;
                        existingItem.Wholesale = flatData.Wholesale;
                        existingItem.DateStart = flatData.DateStart;
                        existingItem.DateEnd = flatData.DateEnd;
                        existingItem.IsDeleted = flatData.IsDeleted;

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
                LogError(ex, "SavePriceListData", "Notes");
                return 2;

            }
        }

        public bool SavePriceListItemData(PriceListData FlatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    var Data = FlatData.ItemsInfo;
                    var CpriceList = dbContext.CPriceList.SingleOrDefault(r => r.PriceListId == FlatData.Id);

                    int CpriceListItemsId = (int)CpriceList.PriceListId;

                    bool hasChanges = false;
                    foreach (var row in Data)
                    {
                        var existingitem = dbContext.CPriceListItems.SingleOrDefault(b => b.Id == CpriceListItemsId && b.ItemId == row.Item.ItemId);

                        if (existingitem == null)
                        {
                            dbContext.CPriceListItems.Add(new CPriceListItemsEntity
                            {
                                Id = CpriceListItemsId,
                                ItemId = row.Item.ItemId,
                                UnitCost = row.SalesPrice,
                                Discount = row.Discount,
                                Qmin = row.Qmin

                            });
                            hasChanges = true;

                        }
                        else if (row.Existing == true && row.IsChecked == false)
                        {
                            dbContext.CPriceListItems.Remove(existingitem);
                            hasChanges = true;
                        }
                        else
                        {
                            bool itemChanged = UpdateExistingItem(existingitem, row);
                            hasChanges = hasChanges || itemChanged;
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
                LogError(ex, "SavePriceListItemData", "Notes");
                return false;
            }
        }

        private static bool UpdateExistingItem(CPriceListItemsEntity existingitem, PriceListItemData row)
        {
            bool hasChanged = false;

            if (existingitem.UnitCost != row.SalesPrice)
            {
                existingitem.UnitCost = row.SalesPrice;
                hasChanged = true;
            }
            if (existingitem.Discount != row.Discount)
            {
                existingitem.Discount = row.Discount;
                hasChanged = true;
            }
            if (existingitem.Qmin != row.Qmin)
            {
                existingitem.Qmin = row.Qmin;
                hasChanged = true;
            }

            return hasChanged;
        }
        #endregion

        #region CustomerOrder
        public ObservableCollection<CustomerOrderData> GetCOrderChooserData(bool ShowDeleted)
        {
            ObservableCollection<CustomerOrderData> DataList = new ObservableCollection<CustomerOrderData>();
            string FilterStr = "";

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = String.Format(@" and Co.IsDeleted =@ShowDeleted");
                }
                command.CommandText = string.Format(@"select Co.COrderId,Co.OrderStatus,Co.CCartId,Cust.CustomerId,Cust.CustomerCode,Cust.CustomerDescr,Cust.CustomerType,Co.Notes,City.CityId,City.CityCode,City.CityDescr,Pref.PrefDescr,COUNTRY.CountryDescr,Co.DateCreated,Co.Incoterms,
City.CityId, City.CityCode, City.CityDescr, PREF.PrefId, PREF.PrefCode, PREF.PrefDescr, Country.CountryId, Country.CountryCode, Country.CountryDescr,Pr.pricelistcode
from CustomerOrder as Co
Inner Join Customer as Cust on Cust.CustomerId= CO.CustomerId
Inner Join CPriceList as Pr on Cust.PriceListId = Pr.PriceListId
Inner Join City on Cust.CityId = City.CityId
iNNER JOIN PREFECTURE AS PREF ON PREF.PrefId = CITY.PrefId	
INNER JOIN COUNTRY ON PREF.CountryId = Country.CountryId
Where 1=1 {0} ",FilterStr);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CustomerOrderData data = new CustomerOrderData();
                        data.Customer = new CustomerData();
                        data.PriceList = new PriceListData();

                        data.CustOrderId = reader["COrderId"].ToString();

                        data.DateCreated = DateTime.Parse(reader["DateCreated"].ToString());
                        data.Notes = reader["Notes"].ToString();
                        data.Incoterms = (BasicEnums.Incoterms)Enum.Parse(typeof(BasicEnums.Incoterms), reader["Incoterms"].ToString());
                        data.OrderStatus = (BasicEnums.OrderStatus)Enum.Parse(typeof(BasicEnums.OrderStatus), reader["OrderStatus"].ToString());
                        data.CartId = reader["CCartId"].ToString();

                        data.Customer.Id = int.Parse(reader["CustomerId"].ToString());
                        data.Customer.Code = reader["CustomerCode"].ToString();
                        data.Customer.Descr = reader["CustomerDescr"].ToString();
                        data.Customer.CustomerType = (BasicEnums.CustomerType)Enum.Parse(typeof(BasicEnums.CustomerType), reader["CustomerType"].ToString());


                        data.Customer.City = new CityData
                        {
                            CityId = int.Parse(reader["CityId"].ToString()),
                            CityCode = reader["CityCode"].ToString(),
                            CityDescr = reader["CityDescr"].ToString()
                        };

                        data.Customer.Prefecture = new PrefectureData
                        {
                            PrefId = int.Parse(reader["PrefId"].ToString()),
                            PrefCode = reader["PrefCode"].ToString(),
                            PrefDescr = reader["PrefDescr"].ToString()
                        };

                        data.Customer.Country = new CountryData
                        {
                            CountryId = int.Parse(reader["CountryId"].ToString()),
                            CountryCode = reader["CountryCode"].ToString(),
                            CountryDescr = reader["CountryDescr"].ToString()
                        };


                        var Code = reader["PriceListCode"].ToString();

                        data.PriceList.Code = Code;


                        data.DateCreated = DateTime.Parse(reader["DateCreated"].ToString());

                        DataList.Add(data);


                    }
                }

                connection.Close();

            }

            return DataList;
        }

        public ObservableCollection<ItemData> GetItemsForSaleData()
        {
            ObservableCollection<ItemData> DataList = new ObservableCollection<ItemData>();


            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = string.Format(@"select ItemId,ItemCode,ItemDescr,MesUnit,ItemType,Assembly
                                                      from Rmaster Where Rmaster.OutputOrderFlag = 1");
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ItemData data = new ItemData();

                        data.ItemId = int.Parse(reader["ItemId"].ToString());
                        data.ItemCode = reader["ItemCode"].ToString();
                        data.ItemDescr = reader["ItemDescr"].ToString();
                        data.MesUnit = reader["MesUnit"].ToString();
                        data.Assembly = (BasicEnums.Assembly)Enum.Parse(typeof(BasicEnums.Assembly), reader["Assembly"].ToString());
                        data.ItemType = (BasicEnums.ItemType)Enum.Parse(typeof(BasicEnums.ItemType), reader["ItemType"].ToString());


                        DataList.Add(data);
                    }
                }

                connection.Close();

            }

            return DataList;
        }
        public ObservableCollection<ItemQuantityOrderData> GetBasicCorderCartData(string CartId)
        {
            ObservableCollection<ItemQuantityOrderData> DataList = new ObservableCollection<ItemQuantityOrderData>();


            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.Parameters.AddWithValue("@CartId", CartId);

                command.CommandText = string.Format(@"select Rmaster.ItemId,Rmaster.ItemCode,Rmaster.ItemDescr,Rmaster.MesUnit,Rmaster.ItemType,Rmaster.Assembly,City.CityCode,City.CityDescr,
COrder_Cart.CCartId,COrder_Cart.UnitCost,COrder_Cart.Quantity,COrder_Cart.TotalCost,COrder_Cart.UnitDiscount,COrder_Cart.DeliveryDate
from COrder_Cart
inner Join City ON cITY.CityId = COrder_Cart.CityId
inner join Rmaster on Rmaster.ItemId = COrder_Cart.ItemId
Where COrder_Cart.CCartId =@CartId
");
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ItemQuantityOrderData data = new ItemQuantityOrderData();
                        data.Item = new ItemData();
                        data.CityDelivered = new CityData();

                        data.Item.ItemId = int.Parse(reader["ItemId"].ToString());
                        data.Item.ItemCode = reader["ItemCode"].ToString();
                        data.Item.ItemDescr = reader["ItemDescr"].ToString();
                        data.Item.Assembly = (BasicEnums.Assembly)Enum.Parse(typeof(BasicEnums.Assembly), reader["Assembly"].ToString());
                        data.Item.ItemType = (BasicEnums.ItemType)Enum.Parse(typeof(BasicEnums.ItemType), reader["ItemType"].ToString());
                        data.Item.MesUnit = reader["MesUnit"].ToString();

                        data.CityDelivered.CityCode = reader["CityCode"].ToString();
                        data.CityDelivered.CityDescr = reader["CityDescr"].ToString();

                        data.UnitCost = float.Parse(reader["UnitCost"].ToString());
                        //data.TotalCost = float.Parse(reader["TotalCost"].ToString());
                        data.UnitDiscount = float.Parse(reader["UnitDiscount"].ToString());
                        data.DeliveryDate = DateTime.Parse(reader["DeliveryDate"].ToString());
                        data.Quantity = float.Parse(reader["Quantity"].ToString());


                        DataList.Add(data);


                    }
                }

                connection.Close();

            }

            return DataList;
        }
        public ObservableCollection<ItemQuantityOrderData> GetCorderCartData(CustomerOrderData FlatData, bool AddItemFlag)
        {
            ObservableCollection<ItemQuantityOrderData> DataList = new ObservableCollection<ItemQuantityOrderData>();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                if (!string.IsNullOrWhiteSpace(FlatData.CartId))
                {
                    command.Parameters.AddWithValue("@CartId", FlatData.CartId);

                }

                command.CommandText = string.Format(@"select Rmaster.ItemId,Rmaster.ItemCode,Rmaster.ItemDescr,Rmaster.MesUnit,Rmaster.ItemType,Rmaster.Assembly,City.CityCode,City.CityDescr,
COrder_Cart.CCartId,COrder_Cart.UnitCost,COrder_Cart.Quantity,COrder_Cart.TotalCost,COrder_Cart.UnitDiscount,COrder_Cart.DeliveryDate
from COrder_Cart
inner Join City ON cITY.CityId = COrder_Cart.CityId
inner join Rmaster on Rmaster.ItemId = COrder_Cart.ItemId
Where COrder_Cart.CCartId =@CartId");
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ItemQuantityOrderData data = new ItemQuantityOrderData();

                        List<string> ExistingItems = new List<string>();

                        data.Item = new ItemData();
                        data.CityDelivered = new CityData();

                        data.Item.ItemId = int.Parse(reader["ItemId"].ToString());
                        data.Item.ItemCode = reader["ItemCode"].ToString();
                        data.Item.ItemDescr = reader["ItemDescr"].ToString();

                        data.Item.Assembly = (BasicEnums.Assembly)Enum.Parse(typeof(BasicEnums.Assembly), reader["Assembly"].ToString());
                        data.Item.ItemType = (BasicEnums.ItemType)Enum.Parse(typeof(BasicEnums.ItemType), reader["ItemType"].ToString());
                        data.Item.MesUnit = reader["MesUnit"].ToString();

                        data.CityDelivered.CityCode = reader["CityCode"].ToString();
                        data.CityDelivered.CityDescr = reader["CityDescr"].ToString();

                        data.UnitCost = float.Parse(reader["UnitCost"].ToString());
                        //data.TotalCost = float.Parse(reader["TotalCost"].ToString());
                        data.UnitDiscount = float.Parse(reader["UnitDiscount"].ToString());
                        data.DeliveryDate = DateTime.Parse(reader["DeliveryDate"].ToString());
                        data.Quantity = float.Parse(reader["Quantity"].ToString());
                        data.IsChecked = true;
                        data.ExistingFlag = true;
                        data.NewItemFlag = false;

                        DataList.Add(data);


                    }

                }
                if (AddItemFlag == true)
                {
                    command.CommandText = string.Format(@"SELECT ItemId, ItemCode, ItemDescr, Assembly, ItemType,MesUnit
FROM Rmaster as Rm1
WHERE Rm1.OutputOrderFlag = 1 AND Rm1.ItemId NOT IN (select Rm2.ItemId
from COrder_Cart
inner join Rmaster as Rm2  on Rm2.ItemId = COrder_Cart.ItemId 
Where  COrder_Cart.CCartId =@CartId)");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            ItemQuantityOrderData data = new ItemQuantityOrderData();
                            data.CityDelivered = new CityData();
                            data.Item = new ItemData();
                            data.CityDelivered = new CityData();

                            data.Item.ItemId = int.Parse(reader["ItemId"].ToString());
                            data.Item.ItemCode = reader["ItemCode"].ToString();
                            data.Item.ItemDescr = reader["ItemDescr"].ToString();



                            data.Item.Assembly = (BasicEnums.Assembly)Enum.Parse(typeof(BasicEnums.Assembly), reader["Assembly"].ToString());
                            data.Item.ItemType = (BasicEnums.ItemType)Enum.Parse(typeof(BasicEnums.ItemType), reader["ItemType"].ToString());


                            data.Item.MesUnit = reader["MesUnit"].ToString();

                            data.CityDelivered = FlatData.Customer.City;

                            data.UnitCost = FlatData.PriceList.ItemsInfo
                                        .Where(i => i.Item != null && i.Item.ItemId == data.Item.ItemId)
                                        .Select(i => i.SalesPrice)
                                        .FirstOrDefault();

                            data.UnitDiscount = 0;
                            data.Quantity = 0;


                            data.DeliveryDate = DateTime.Now;
                            data.IsChecked = false;
                            data.ExistingFlag = false;
                            data.NewItemFlag = true;

                            DataList.Add(data);
                        }
                    }
                    connection.Close();

                }

                return DataList;
            }
        }

        public bool SaveCorderCartData(CustomerOrderData FlatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    var CartData = FlatData.CartData;
                    var CustomerOrder = dbContext.CustomerOrder.SingleOrDefault(r => r.COrderId == FlatData.CustOrderId);

                    String CartId = CustomerOrder.CCartId;

                    bool hasChanges = false;
                    foreach (var row in CartData)
                    {
                        var existingitem = dbContext.COrder_Cart.SingleOrDefault(b => b.CCartId == CartId && b.ItemId == row.Item.ItemId);

                        if (existingitem == null)
                        {
                            dbContext.COrder_Cart.Add(new COrderCartEntity
                            {
                                CCartId = CartId,
                                ItemId = row.Item.ItemId,
                                Quantity = row.Quantity,
                                UnitCost = row.UnitCost,
                                UnitDiscount = row.UnitDiscount,
                                TotalCost = row.TotalCost,
                                CityId = FlatData.Customer.City.CityId,
                                DeliveryDate = row.DeliveryDate

                            });
                            hasChanges = true;
                        }
                        else if (row.ExistingFlag == true && row.IsChecked == false)
                        {
                            dbContext.COrder_Cart.Remove(existingitem);
                            hasChanges = true;
                        }
                        else if (row.ExistingFlag == true && row.IsChecked == true)
                        {
                            bool itemChanged = UpdateExistingCartItem(existingitem, row);
                            hasChanges = hasChanges || itemChanged;
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
                LogError(ex, "SavePriceListItemData", "Notes");
                return false;
            }
        }
        private static bool UpdateExistingCartItem(COrderCartEntity existingitem, ItemQuantityOrderData row)
        {
            bool hasChanged = false;

            if (existingitem.UnitCost != row.UnitCost)
            {
                existingitem.UnitCost = row.UnitCost;
                hasChanged = true;
            }
            if (existingitem.Quantity != row.Quantity)
            {
                existingitem.Quantity = row.Quantity;
                hasChanged = true;
            }
            if (existingitem.TotalCost != row.TotalCost)
            {
                existingitem.TotalCost = row.TotalCost;
                hasChanged = true;
            }

            return hasChanged;
        }


        #endregion
        #endregion

        #region SupplierInfo

        #region SupplierInfo 1st Tab


        public int SaveSupplierInfoData(SupplierInfoData FlatData)
        {
            int flag2 = new int();
            try
            {

                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;

                    #region Ελεγχος αν υπάρχει ήδη η εγγραφή στην Βάση Δεδομένων

                    command.Parameters.AddWithValue("@SupCode", FlatData.SupplierCode);
                    command.Parameters.AddWithValue("@SupDescr", FlatData.SupplierDescr);

                    command.CommandText = string.Format(@"Select SupCode from Supplier where SupCode = @SupCode");

                    //string flag = command.ExecuteScalar().ToString();
                    var flag = command.ExecuteScalar();


                    if (flag == null)
                    {

                        command.CommandText = string.Format(@"insert  into  Supplier
                                                            (SupCode,SupDescr)
                                                            Values
                                                            (@SupCode,@SupDescr)");
                        command.ExecuteScalar();

                        flag2 = 0;
                    }
                    else
                    {
                        flag2 = 1;
                    }
                    #endregion

                    command.Parameters.AddWithValue("@CityCode", FlatData.CityCode);
                    command.CommandText = string.Format(@"Select CityId from City Where CityCode = @CityCode");

                    command.Parameters.AddWithValue("@CityId", command.ExecuteScalar().ToString());


                    command.Parameters.AddWithValue("@Email", FlatData.Email);
                    command.Parameters.AddWithValue("@Phone", FlatData.Phone);
                    command.Parameters.AddWithValue("@Adress", FlatData.Adress1);
                    command.Parameters.AddWithValue("@Fason", FlatData.Fason);


                    command.CommandText = string.Format(@"UPDATE Supplier
                                                        SET SupEmail = @Email,SupPhone=@Phone,SupAdress=@Adress,SupCityId=@CityId,IsFason=@Fason
                                                        WHERE SupCode = @SupCode;");
                    command.ExecuteScalar();

                    connection.Close();

                    if (flag2 == 0)
                    {
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
                return 2;
            }
        }
        public SupplierInfoData GetSupplierInfoData(SupplierInfoData Filter)
        {
            SupplierInfoData FlatData = new SupplierInfoData();
            try
            {

                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;

                    command.Parameters.AddWithValue("@SupplierCode", Filter.SupplierCode);


                    command.CommandText = string.Format(@"select Supplier.SupCode,Supplier.SupDescr,Supplier.SupEmail,Supplier.SupPhone,Supplier.SupAdress,Supplier.SupCityId,Supplier.SupType,Supplier.IsFason ,
                                                    City.CityCode,City.CityDescr ,Prefecture.PrefCode,Prefecture.PrefDescr ,Country.CountryCode ,Country.CountryDescr
                                                    from Supplier
                                                    Inner Join City on Supplier.SupCityId = City.CityId
                                                    Inner Join Prefecture on City.PrefId = Prefecture.PrefId
                                                    Inner JOIN  Country on Prefecture.CountryId = Country.CountryId
                                                    Where Supplier.SupCode=@SupplierCode");
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            FlatData.SupplierCode = reader["SupCode"].ToString();
                            FlatData.SupplierDescr = reader["SupDescr"].ToString();
                            FlatData.Email = reader["SupEmail"].ToString();
                            FlatData.Phone = reader["SupPhone"].ToString();
                            FlatData.Adress1 = reader["SupAdress"].ToString();
                            FlatData.SupplierType = reader["SupType"].ToString();
                            if (reader["IsFason"].ToString().IsNullOrWhiteSpace())
                            {
                                FlatData.Fason = false;

                            }
                            else
                            {
                                FlatData.Fason = true;
                            }
                            FlatData.CityCode = reader["CityCode"].ToString();
                            FlatData.CityDescr = reader["CityDescr"].ToString();
                            FlatData.PrefCode = reader["PrefCode"].ToString();
                            FlatData.PrefDescr = reader["PrefDescr"].ToString();
                            FlatData.CountryCode = reader["CountryCode"].ToString();
                            FlatData.CountryDescr = reader["CountryDescr"].ToString();



                        }
                    }


                    connection.Close();

                }

                return FlatData;
            }
            catch (Exception ex)
            {
                return FlatData = null;
            }
        }

        public ObservableCollection<SupplierInfoData> GetSupplierInfoChooserData()
        {
            ObservableCollection<SupplierInfoData> DataList = new ObservableCollection<SupplierInfoData>();


            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = string.Format(@"select SupCode,SupDescr from Supplier");
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SupplierInfoData data = new SupplierInfoData();

                        data.SupplierCode = reader["SupCode"].ToString();
                        data.SupplierDescr = reader["SupDescr"].ToString();

                        DataList.Add(data);
                    }
                }

                connection.Close();

            }

            return DataList;
        }

        public ObservableCollection<SupplierInfoData> GetSupplierInfoSearchData(SupplierInfoSearchFilterData Filter)
        {
            ObservableCollection<SupplierInfoData> DataList = new ObservableCollection<SupplierInfoData>();
            String str = "";


            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                #region Filters

                if (!string.IsNullOrWhiteSpace(Filter.SupplierCodeFrom))
                {
                    str = String.Concat(str, "and S.SupCode Between @SupCodeFrom and @SupCodeTo");
                    command.Parameters.AddWithValue("@SupCodeFrom", Filter.SupplierCodeFrom);
                    command.Parameters.AddWithValue("@SupCodeTo", Filter.SupplierCodeTo);

                }


                if (!string.IsNullOrWhiteSpace(Filter.CountryCodeFrom))
                {
                    str = String.Concat(str, "and Country.CountryCode Between @CountryCodeFrom and @CountryCodeTo");
                    command.Parameters.AddWithValue("@CountryCodeFrom", Filter.CountryCodeFrom);
                    command.Parameters.AddWithValue("@CountryCodeTo", Filter.CountryCodeTo);

                }

                if (!string.IsNullOrWhiteSpace(Filter.PrefectureCodeFrom))
                {
                    str = String.Concat(str, "and Pref.PrefCode Between @PrefCodeFrom and @PrefCodeTo");
                    command.Parameters.AddWithValue("@PrefCodeFrom", Filter.PrefectureCodeFrom);
                    command.Parameters.AddWithValue("@PrefCodeTo", Filter.PrefectureCodeTo);
                }


                if (!string.IsNullOrWhiteSpace(Filter.CityCodeFrom))
                {
                    str = String.Concat(str, "and City.CityCode Between @CityCodeFrom and @CityCodeTo");
                    command.Parameters.AddWithValue("@CityCodeFrom", Filter.CityCodeFrom);
                    command.Parameters.AddWithValue("@CityCodeTo", Filter.CityCodeTo);

                }


                #endregion

                command.CommandText = string.Concat(@"select S.SupCode,S.SupDescr,S.SupEmail,S.SupPhone,S.SupAdress,
S.SupCityId,S.SupType,
Country.CountryCode,Country.CountryDescr,Pref.PrefCode,Pref.PrefDescr , City.CityCode,City.CityDescr
from Supplier as S
Inner Join City on S.SupCityId = City.CityId
Inner Join Prefecture as Pref on Pref.PrefId = City.PrefId
Inner Join Country on Country.CountryId = Pref.PrefId
Where 1=1 ", str);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SupplierInfoData data = new SupplierInfoData();

                        data.SupplierCode = reader["SupCode"].ToString();
                        data.SupplierDescr = reader["SupDescr"].ToString();
                        data.Email = reader["SupEmail"].ToString();
                        data.Phone = reader["SupPhone"].ToString();
                        data.Adress1 = reader["SupAdress"].ToString();
                        data.SupplierType = reader["SupType"].ToString();
                        data.CountryCode = reader["CountryCode"].ToString();

                        data.CountryDescr = reader["CountryDescr"].ToString();
                        data.PrefCode = reader["PrefCode"].ToString();
                        data.PrefDescr = reader["PrefDescr"].ToString();
                        data.CityCode = reader["CityCode"].ToString();
                        data.CityDescr = reader["CityDescr"].ToString();
                        DataList.Add(data);
                    }
                }

                connection.Close();

            }

            return DataList;
        }

        #endregion

        #region SupplierInfo 2nd Tab /Items per Supplier

        public bool SaveItemPerSupData(ObservableCollection<SupItemData> Data, string SupplierCode)
        {
            ObservableCollection<CountryData> DataList = new ObservableCollection<CountryData>();

            try
            {

                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.Parameters.AddWithValue("@SupCode", SupplierCode);
                    command.CommandText = string.Format(@"Select SupId from Supplier Where SupCode=@SupCode ");
                    command.Parameters.AddWithValue("@SupplierId", command.ExecuteScalar());




                    foreach (var item in Data)
                    {
                        if (item.SupplierFlag == true)
                        {
                            command.Parameters.AddWithValue("@ItemId", item.ItemId);

                            command.CommandText = string.Format(@"insert  into SupRmaster
                                                            (ItemId,SupplierId)
                                                            SELECT @ItemId,@SupplierId
                                                            WHERE NOT EXISTS 
(SELECT 1 FROM SupRmaster WHERE ItemId = @ItemId AND SupplierId = @SupplierId)");
                            command.ExecuteScalar();
                            command.Parameters.RemoveAt(command.Parameters.IndexOf("@ItemId"));

                        }
                    }


                    connection.Close();

                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;

        }
        public ObservableCollection<SupItemData> GetItemPerSupData(string SupplierCode)
        {
            ObservableCollection<SupItemData> DataList = new ObservableCollection<SupItemData>();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.Parameters.AddWithValue("@SupplierCode", SupplierCode);

                command.CommandText = string.Format(@"Select SupId from Supplier 
Where SupCode = @SupplierCode");

                //EXECUTE SCALAR

                command.Parameters.AddWithValue("@SupplierId", command.ExecuteScalar());

                command.CommandText = string.Format(@"select ItemId,ItemCode,ItemDescr,MesUnit,ItemType,Assembly 
                                                    from Rmaster
                                                    Where Assembly < 4");
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SupItemData data = new SupItemData();



                        data.ItemId = int.Parse(reader["ItemId"].ToString());

                        data.ItemCode = reader["ItemCode"].ToString();
                        data.ItemDescr = reader["ItemDescr"].ToString();
                        data.MesUnit = reader["MesUnit"].ToString();
                        data.ItemType = reader["ItemType"].ToString();
                        data.Assembly = reader["Assembly"].ToString();
                        data.SupplierFlag = false;

                        DataList.Add(data);
                    }
                }

                foreach (var item in DataList)
                {
                    command.Parameters.AddWithValue("@ItemId2", item.ItemId);


                    command.CommandText = string.Format(@"select SupCode from  SupRmaster 
Inner Join Supplier as Sup on Sup.SupId = SupRmaster.SupplierId
Where Sup.SupCode =@SupplierCode and SupRmaster.ItemId =@ItemId2");


                    string result = (string)command.ExecuteScalar();

                    if (!result.IsNullOrWhiteSpace())
                    {
                        item.SupplierFlag = true;


                    }
                    else
                    {
                        item.SupplierFlag = false;
                    }

                    command.Parameters.RemoveAt(command.Parameters.IndexOf("@ItemId2"));


                }
                connection.Close();
            }
            return DataList;
        }

        #endregion

        #endregion

        #region Inventory

        #endregion

        #region Αρχείο Αποθηκών
        #region 1st Tab/Γενικές Πληροφορίες
        public int SaveInventoryData(InventoryData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    int InventoryId = flatData.InvId;
                    var existingInvQuery = dbContext.Inventory.Where(r => r.InvId == InventoryId);
                    var existingInv = existingInvQuery.SingleOrDefault();
                    // Execute the query and get the result

                    if (existingInv != null)
                    {

                        // Update existing item
                        existingInv.InvCode = flatData.InvCode;
                        existingInv.InvDescr = flatData.InvDescr;
                        existingInv.Capacity = flatData.Capacity;
                        existingInv.Location = flatData.Location;
                        existingInv.IsDeleted = flatData.IsDeleted;

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
                LogError(ex, "SaveInventoryData", "Notes");
                return -1;

            }
        }

        public int AddInventoryData(InventoryData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    var existingInvQuery = dbContext.Inventory.Where(r => r.InvCode == flatData.InvCode);
                    var existingInv= existingInvQuery.SingleOrDefault();
                    // Execute the query and get the result


                    if (existingInv == null)
                    {
                        var newInv = new InventoryDataEntity();
                        // Insert new ForeCast
                        newInv.InvCode = flatData.InvCode;
                        newInv.InvDescr = flatData.InvDescr;
                        newInv.Location = flatData.Location;
                        newInv.Capacity = flatData.Capacity;

                        newInv.IsDeleted = false;
                        dbContext.Inventory.Add(newInv);

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
                LogError(ex, "AddInventoryData", "Notes");
                return 2;

            }
        }

        public InventoryData GetInventoryChooserData(int id, string Code)
        {
            InventoryData FlatData = new InventoryData();
            try
            {

                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;


                    if (id > 0)
                    {
                        command.Parameters.AddWithValue("@ID", id);

                        command.CommandText = string.Format(@"select InvId,InvCode,InvDescr,Location,Capacity,IsDeleted from Inventory 
                                                             Where InvId=@ID");
                    }
                    else if (!string.IsNullOrWhiteSpace(Code))
                    {
                        command.Parameters.AddWithValue("@Code", Code); // Corrected variable name

                        command.CommandText = string.Format(@"select InvId,InvCode,InvDescr,Location,Capacity,IsDeleted from Inventory 
                                                            Where InvCode=@Code");
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            FlatData.InvId = int.Parse(reader["InvId"].ToString());
                            FlatData.InvCode = reader["InvCode"].ToString();
                            FlatData.InvDescr = reader["InvDescr"].ToString();
                            FlatData.Location = reader["Location"].ToString();
                            FlatData.Capacity = double.Parse(reader["Capacity"].ToString());
                            FlatData.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());

                        }
                    }


                    connection.Close();

                }

                return FlatData;
            }
            catch (Exception ex)
            {
                return FlatData = null;
            }
        }

        public ObservableCollection<InventoryData> GetInventoryData(bool ShowDeleted)
        {
            ObservableCollection<InventoryData> DataList = new ObservableCollection<InventoryData>();


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

                command.CommandText = string.Format(@"select InvId,InvCode,InvDescr,Location,Capacity,IsDeleted from Inventory 
Where 1=1 {0}",FilterStr);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        InventoryData data = new InventoryData();

                        data.InvId = int.Parse(reader["InvId"].ToString());
                        data.InvCode = reader["InvCode"].ToString();
                        data.InvDescr = reader["InvDescr"].ToString();
                        data.Location = reader["Location"].ToString();
                        data.Capacity = double.Parse(reader["Capacity"].ToString());
                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());
                        DataList.Add(data);
                    }
                }

                connection.Close();

            }

            return DataList;
        }

        #endregion

        #region  2nd Tab /Stock

        public bool SaveStockData(ObservableCollection<StockData> Data, int InventoryId)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Retrieve the final item from the Rmaster table
                    var inventory = dbContext.Inventory.SingleOrDefault(r => r.InvId == InventoryId);

                    if (inventory == null)
                    {
                        return false;
                    }

                    int finalInvId = inventory.InvId;

                    int result = 0;
                    foreach (var row in Data)
                    {
                        int stockItemId = row.StockItem.ItemId;
                        var existingStock = dbContext.Stock.SingleOrDefault(b => b.InvId == finalInvId && b.ItemId == stockItemId);

                        if (existingStock == null && row.StockItemFlag == true && row.NewItemFlag == true)
                        {
                            // Insert new bom
                            StockDataEntity newStock = new StockDataEntity
                            {
                                InvId = finalInvId,
                                ItemId = stockItemId,
                                Quantity = (float)row.Stock,
                                InvMax = (float)row.InvMax,
                                InvMin = (float)row.InvMin,

                            };

                            dbContext.Stock.Add(newStock);
                            result += 1;
                        }
                        else if (row.ExistingFlag == true && row.StockItemFlag == false)
                        {
                            dbContext.Stock.Remove(existingStock);

                        }
                        else if (row.ExistingFlag == true && row.StockItemFlag == true)
                        {
                            // Update existing bom
                            existingStock.Quantity = (float)row.Stock;
                            existingStock.InvMax = (float)row.InvMax;
                            existingStock.InvMin = (float)row.InvMin;

                        }
                    }

                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "SaveStockData", "Notes");
                return false;
            }
        }
        public ObservableCollection<StockData> GetStockData(string InvCode, bool addItemFlag)
        {
            ObservableCollection<StockData> DataList = new ObservableCollection<StockData>();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.Parameters.AddWithValue("@InvCode", InvCode);

                command.CommandText = string.Format(@"Select InvId from Inventory 
Where InvCode = @InvCode");

                //EXECUTE SCALAR

                int InventoryId = (int)command.ExecuteScalar();
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@InvId", InventoryId);


                command.CommandText = string.Format(@"
 SELECT Rmaster.ItemId,Rmaster.ItemCode,Rmaster.ItemDescr,Rmaster.MesUnit,Rmaster.ItemType,Rmaster.Assembly,
Stock.Quantity ,Stock.InvMax,Stock.InvMin
FROM STOCK
Inner JOIN Rmaster ON Rmaster.ItemId = Stock.ItemId
Where Stock.InvId = @InvId");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        StockData stockdata = new StockData();
                        ItemData itemData = new ItemData();


                        itemData.ItemId = int.Parse(reader["ItemId"].ToString());

                        itemData.ItemCode = reader["ItemCode"].ToString();
                        itemData.ItemDescr = reader["ItemDescr"].ToString();
                        itemData.MesUnit = reader["MesUnit"].ToString();
                        itemData.Assembly = (BasicEnums.Assembly)Enum.Parse(typeof(BasicEnums.Assembly), reader["Assembly"].ToString());
                        itemData.ItemType = (BasicEnums.ItemType)Enum.Parse(typeof(BasicEnums.ItemType), reader["ItemType"].ToString());
                        stockdata.StockItem = itemData;
                        stockdata.Stock = double.Parse(reader["Quantity"].ToString());
                        stockdata.InvMax = double.Parse(reader["InvMax"].ToString());
                        stockdata.InvMin = double.Parse(reader["InvMin"].ToString());

                        stockdata.NewItemFlag = false;
                        stockdata.ExistingFlag = true;
                        stockdata.StockItemFlag = true;

                        DataList.Add(stockdata);
                    }
                }


                if (addItemFlag)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@InvId", InventoryId);

                    command.CommandText = @"
select Rmaster.ItemId  , Rmaster.ItemCode, Rmaster.ItemDescr, Rmaster.ItemType,
Rmaster.Assembly, Rmaster.MesUnit
From Rmaster

WHERE NOT EXISTS (SELECT 1 FROM Stock WHERE ItemId = Rmaster.ItemId AND InvId = @InvId)
And Rmaster.AssemblyNumber <=2 And Rmaster.IsDeleted = 0";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ItemData itemData = new ItemData();
                            StockData stockdata = new StockData();

                            stockdata.FinalItemId = 15;

                            itemData.ItemId = int.Parse(reader["ItemId"].ToString());
                            itemData.ItemCode = reader["ItemCode"].ToString();
                            itemData.ItemDescr = reader["ItemDescr"].ToString();
                            itemData.Assembly = (BasicEnums.Assembly)Enum.Parse(typeof(BasicEnums.Assembly), reader["Assembly"].ToString());
                            itemData.ItemType = (BasicEnums.ItemType)Enum.Parse(typeof(BasicEnums.ItemType), reader["ItemType"].ToString());
                            itemData.MesUnit = reader["MesUnit"].ToString();

                            stockdata.StockItem = itemData;

                            stockdata.Stock = 0;
                            stockdata.InvMax = 1000;
                            stockdata.InvMin = 0;
                            stockdata.NewItemFlag = true;
                            stockdata.ExistingFlag = false;

                            DataList.Add(stockdata);
                        }
                    }
                }

                connection.Close();
            }
            return DataList;
        }

        public ObservableCollection<StockData> GetMRPItemData(string InvCode)
        {
            ObservableCollection<StockData> DataList = new ObservableCollection<StockData>();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.Parameters.AddWithValue("@InvCode", InvCode);

                command.CommandText = string.Format(@"Select InvId from Inventory 
Where InvCode = @InvCode");

                //EXECUTE SCALAR

                int InventoryId = (int)command.ExecuteScalar();
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@InvId", InventoryId);


                command.CommandText = string.Format(@"
 SELECT Rmaster.ItemId,Rmaster.ItemCode,Rmaster.ItemDescr,Rmaster.MesUnit,
 Rmaster.ItemType,Rmaster.Assembly,Lot.LotPolicyCode,Lot.LeadTime,Lot.BatchSize,
Stock.Quantity,Stock.InvMin,Stock.InvMax  
FROM STOCK
Inner JOIN Rmaster ON Rmaster.ItemId = Stock.ItemId
Inner Join LotPolicy as Lot on Lot.ItemId = Rmaster.ItemId
Where Lot.MainPolicy = 1 and Stock.InvId = @InvId");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        StockData stockdata = new StockData();
                        ItemData itemData = new ItemData();
                        itemData.LotPolicy = new LotPolicyData();

                        itemData.ItemId = int.Parse(reader["ItemId"].ToString());

                        itemData.ItemCode = reader["ItemCode"].ToString();
                        itemData.ItemDescr = reader["ItemDescr"].ToString();
                        itemData.MesUnit = reader["MesUnit"].ToString();
                        itemData.Assembly = (BasicEnums.Assembly)Enum.Parse(typeof(BasicEnums.Assembly), reader["Assembly"].ToString());
                        itemData.ItemType = (BasicEnums.ItemType)Enum.Parse(typeof(BasicEnums.ItemType), reader["ItemType"].ToString());
                        itemData.LotPolicy.Code = reader["LotPolicyCode"].ToString();
                        itemData.LotPolicy.LeadTime =double.Parse(reader["LeadTime"].ToString());
                        itemData.LotPolicy.BatchSize = double.Parse(reader["BatchSize"].ToString());


                        stockdata.StockItem = itemData;
                        stockdata.Stock = double.Parse(reader["Quantity"].ToString());
                        stockdata.InvMin = double.Parse(reader["InvMin"].ToString());
                        stockdata.InvMax = double.Parse(reader["InvMax"].ToString());


                        DataList.Add(stockdata);
                    }
                }


                connection.Close();
            }
            return DataList;
        }
        #endregion

        #endregion

        #region Visualisation

        public ObservableCollection<OptimizationResultsInvData> GetOptimisationInvData(string ItemCode, string InvCode, InvDiagramsSearchData FilterData)
        {
            ObservableCollection<OptimizationResultsInvData> DataList = new ObservableCollection<OptimizationResultsInvData>();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (!string.IsNullOrEmpty(InvCode))
                {
                    command.Parameters.AddWithValue("@InvCode", InvCode);
                    command.CommandText = string.Format(@"Select InvId from Inventory 
Where InvCode = @InvCode");

                    command.Parameters.AddWithValue("@InvId", command.ExecuteScalar());

                    var InviD = command.ExecuteScalar();

                }
                if (!string.IsNullOrEmpty(ItemCode))
                {
                    command.Parameters.AddWithValue("@ItemCode", ItemCode);
                    command.CommandText = string.Format(@"Select ItemId from Rmaster 
Where ItemCode = @ItemCode");

                    var ItemId = command.ExecuteScalar();
                    command.Parameters.AddWithValue("@ItemId", ItemId);

                }


                var DateStart = FilterData.DateStart.ToString("yyyy-dd-MM");
                var DateEnd = FilterData.DateEnd.ToString("yyyy-dd-MM");

                command.Parameters.AddWithValue("@DateStart", DateStart);
                command.Parameters.AddWithValue("@DateEnd", DateEnd);

                command.CommandText = string.Format(@"
SELECT Demand,ForecastedDemand,Batchsize,StockQ,ProductionBool,iDay FROM Inventory_Demand
Where iDay Between  @DateStart AND @DateEnd and ItemId=@ItemId and InventoryId=@InvId");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        OptimizationResultsInvData Data = new OptimizationResultsInvData();


                        Data.Demand = float.Parse(reader["Demand"].ToString());

                        Data.DemandForecast = float.Parse(reader["ForecastedDemand"].ToString());
                        Data.BatchSize = float.Parse(reader["Batchsize"].ToString());
                        Data.StockQ = float.Parse(reader["StockQ"].ToString());
                        Data.ProductionBool = bool.Parse(reader["ProductionBool"].ToString());
                        Data.iDay = DateTime.Parse(reader["iDay"].ToString());



                        DataList.Add(Data);
                    }
                }

                connection.Close();
            }
            return DataList;
        }

        #endregion

        #region Data Analytics
        #region Mrp Visualisation
        public ObservableCollection<HeatMapData> GetPlannedOrdersData()
        {
            List<PlannedOrdersData> DataList = new List<PlannedOrdersData>();
            ObservableCollection<HeatMapData> HeatmapDataList = new ObservableCollection<HeatMapData>();
            HeatMapData heatdata = new HeatMapData();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = string.Format(@"select Rmaster.ItemCode,Orders.LotpolicyId,Orders.Quantity,Orders.DateStart,Orders.DateEnd
from PlannedOrders as Orders
Inner Join Rmaster on Rmaster.ItemId = Orders.ItemId
Order By Orders.ItemId,Orders.DateStart");
                using (var reader = command.ExecuteReader())
                {
                    var i = 1;
                    heatdata = new HeatMapData();
                    while (reader.Read())
                    {
                        PlannedOrdersData data = new PlannedOrdersData();

                        data.ItemCode = reader["ItemCode"].ToString();
                        data.LotPolicyId = int.Parse(reader["LotpolicyId"].ToString());
                        data.Quantity = float.Parse(reader["Quantity"].ToString());
                        data.DateStart = DateTime.Parse(reader["DateStart"].ToString());
                        data.DateEnd = DateTime.Parse(reader["DateEnd"].ToString());

                        DataList.Add(data);

                        var property = heatdata.GetType().GetProperty($"D{i}");
                        property.SetValue(heatdata, data.Quantity);



                        if (i < 12)
                        {
                            i++;
                        }
                        else
                        {
                            heatdata.ItemCode = reader["ItemCode"].ToString();
                            HeatmapDataList.Add(heatdata);
                            i = 1;
                            heatdata = new HeatMapData();
                        }
                    }
                }

                connection.Close();

            }

            return HeatmapDataList;

        }
        #endregion

        #region SalesDashBoard
        public ObservableCollection<OptimizationResultsInvData> GetItemsProfitData(string ItemCode, string InvCode, InvDiagramsSearchData FilterData)
        {
            ObservableCollection<OptimizationResultsInvData> DataList = new ObservableCollection<OptimizationResultsInvData>();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.Parameters.AddWithValue("@InvCode", InvCode);
                command.CommandText = string.Format(@"Select InvId from Inventory 
Where InvCode = @InvCode");

                command.Parameters.AddWithValue("@InvId", command.ExecuteScalar());

                var InviD = command.ExecuteScalar();

                command.Parameters.AddWithValue("@ItemCode", ItemCode);
                command.CommandText = string.Format(@"Select ItemId from Rmaster 
Where ItemCode = @ItemCode");

                var ItemId = command.ExecuteScalar();
                command.Parameters.AddWithValue("@ItemId", ItemId);

                var DateStart = FilterData.DateStart.ToString("yyyy-dd-MM");
                var DateEnd = FilterData.DateEnd.ToString("yyyy-dd-MM");

                command.Parameters.AddWithValue("@DateStart", DateStart);
                command.Parameters.AddWithValue("@DateEnd", DateEnd);

                command.CommandText = string.Format(@"
SELECT Demand,ForecastedDemand,Batchsize,StockQ,ProductionBool,iDay FROM Inventory_Demand
Where iDay Between  @DateStart AND @DateEnd and ItemId=@ItemId and InventoryId=@InvId");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        OptimizationResultsInvData Data = new OptimizationResultsInvData();


                        Data.Demand = float.Parse(reader["Demand"].ToString());

                        Data.DemandForecast = float.Parse(reader["ForecastedDemand"].ToString());
                        Data.BatchSize = float.Parse(reader["Batchsize"].ToString());
                        Data.StockQ = float.Parse(reader["StockQ"].ToString());
                        Data.ProductionBool = bool.Parse(reader["ProductionBool"].ToString());
                        Data.iDay = DateTime.Parse(reader["iDay"].ToString());



                        DataList.Add(Data);
                    }
                }

                connection.Close();
            }
            return DataList;
        }
        #endregion

        #region Forecast
        public int SetMainForecastForMRP(ForecastInfoData FlatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    int ForecastId = FlatData.ID;

                    var selectedForecastquery = dbContext.ForecastInfo.Where(r => r.ID == ForecastId);
                    var selectedForecast = selectedForecastquery.SingleOrDefault();


                    var MRPForecastquery = dbContext.ForecastInfo.Where(r => r.MRPForecast == true);
                    var MainMRPForecast = MRPForecastquery.SingleOrDefault();
                    // Execute the query and get the result


                    var result = System.Windows.MessageBox.Show($"The Forecast with Code {FlatData.ForCode}  will be set as the Main Forecast for the MRP . Proceed?", "Confirmation", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        if (MainMRPForecast != null)
                        {

                            MainMRPForecast.MRPForecast = false;

                        }
                        else
                        {
                            selectedForecast.MRPForecast = true;
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
                LogError(ex, "SetMainForecastForMRP", "Notes");
                return -1;

            }
        }

        public int SaveForecastData(ForecastInfoData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    int ForecastId = flatData.ID;
                    var existingItemQuery = dbContext.ForecastInfo.Where(r => r.ID == ForecastId);
                    var existingItem = existingItemQuery.SingleOrDefault();
                    // Execute the query and get the result

                    if (existingItem != null)
                    {

                        // Update existing item
                        existingItem.ID = flatData.ID;
                        existingItem.ForDescr = flatData.ForDescr;
                        existingItem.ForCode = flatData.ForCode;
                        existingItem.Notes = flatData.Notes;
                        existingItem.TimeBucket = flatData.TimeBucket.ToString();
                        existingItem.PeriodType = flatData.PeriodType.ToString();
                        existingItem.PeriodNum = flatData.PeriodNumber;
                        existingItem.HoursPerTimeBucket = flatData.HoursPerTimeBucket;
                        existingItem.DateFrom = flatData.DateFrom;
                        existingItem.DateTo = flatData.DateTo;
                        existingItem.IsDeleted = flatData.IsDeleted;
                        existingItem.NumberOfBuckets = flatData.NumberOfBuckets;



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
                LogError(ex, "SaveForecastData", "Notes");
                return -1;

            }
        }

        public int AddForecastData(ForecastInfoData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    var existingItemQuery = dbContext.ForecastInfo.Where(r => r.ForCode == flatData.ForCode);
                    var existingItem = existingItemQuery.SingleOrDefault();
                    // Execute the query and get the result


                    if (existingItem == null)
                    {
                        var newItem = new ForecastInfoDataEntity();
                        // Insert new ForeCast
                        newItem.ForDescr = flatData.ForDescr;
                        newItem.ForCode = flatData.ForCode;
                        newItem.Notes = flatData.Notes;
                        newItem.TimeBucket = flatData.TimeBucket.ToString();
                        newItem.PeriodType = flatData.PeriodType.ToString();
                        newItem.PeriodNum = flatData.PeriodNumber;
                        newItem.HoursPerTimeBucket = flatData.HoursPerTimeBucket;
                        newItem.DateFrom = flatData.DateFrom;
                        newItem.DateTo = flatData.DateTo;
                        newItem.IsDeleted = false;
                        newItem.MRPForecast = false;
                        newItem.NumberOfBuckets = 1;

                        dbContext.ForecastInfo.Add(newItem);

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
                LogError(ex, "AddForecastData", "Notes");
                return 2;

            }
        }
        public bool SaveDemandForecast(ForecastInfoData FlatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    var DemandForecast = FlatData.DemandForecast;
                    var forecast = dbContext.ForecastInfo.SingleOrDefault(r => r.ID == FlatData.ID);

                    String ForCode = forecast.ForCode.ToString();

                    bool hasChanges = false;
                    foreach (var row in DemandForecast)
                    {

                            var currentItemId = row.Item.ItemId;
                            var DateStr = row.DateStr;

                            var existingRows = dbContext.DemandForecast.Where(b => b.ForCode == ForCode && b.ItemId == currentItemId && b.DateStr == row.DateStr);

                            var existingrow = dbContext.DemandForecast.FirstOrDefault(b => b.ForCode == ForCode && b.ItemId == currentItemId && b.DateStr == row.DateStr);

                            if (existingrow == null && row.Selected == true )
                            {
                                dbContext.DemandForecast.Add(new DemandForecastEntity
                                {
                                    ForCode = ForCode,
                                    ItemId = row.Item.ItemId,
                                    Date = row.Date,
                                    DateStr = row.DateStr,

                                    Demand = row.Demand


                                });
                                hasChanges = true;
                            }
                            else if (existingrow != null)
                            {

                                if (existingrow.Demand != row.Demand && row.Selected == true)
                                {
                                    existingrow.Demand = row.Demand;
                                    hasChanges = true;

                                }
                                else if(row.Selected == false)
                            {
                                dbContext.DemandForecast.Remove(existingrow);
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
                LogError(ex, "SaveDemandForecast", "Notes");
                return false;
            }
        }
        public ObservableCollection<ForecastInfoData> GetForecastInfoData(bool ShowDeleted)
        {
            ObservableCollection<ForecastInfoData> DataList = new ObservableCollection<ForecastInfoData>();


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

                command.CommandText = string.Format(@"select ID,Forcode,ForDescr,TIMEBUCKET,PERIODTYPE,PeriodNum,HoursPerTimeBucket,NumberOfBuckets,IsDeleted,MRPForecast,
ForDescr,Notes,Datefrom,DateTo from ForecastInfo
Where  1=1 {0}", FilterStr);




                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ForecastInfoData data = new ForecastInfoData();

                        data.ID = int.Parse(reader["ID"].ToString());
                        data.ForCode = reader["Forcode"].ToString();
                        data.ForDescr = reader["Fordescr"].ToString();
                        data.Notes = reader["Notes"].ToString();

                        data.TimeBucket = (BasicEnums.Timebucket)Enum.Parse(typeof(BasicEnums.Timebucket), reader["TIMEBUCKET"].ToString());
                        data.PeriodType = (BasicEnums.PeriodType)Enum.Parse(typeof(BasicEnums.PeriodType), reader["PeriodType"].ToString());
                        data.PeriodNumber = int.Parse(reader["PeriodNum"].ToString());
                        data.HoursPerTimeBucket = int.Parse(reader["HoursPerTimeBucket"].ToString());

                        data.DateFrom = Convert.ToDateTime(reader["Datefrom"]);
                        data.DateTo = Convert.ToDateTime(reader["DateTo"]);
                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());
                        data.MRPForecast = bool.Parse(reader["MRPForecast"].ToString());
                        data.NumberOfBuckets = int.Parse(reader["NumberOfBuckets"].ToString());


                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }

        public MRPInputData GetDemandFItemData(MRPInputData MRPData, string ForCode)
        {
            ObservableCollection<DemandForecastData> ForecastDataList = new ObservableCollection<DemandForecastData>();
            ObservableCollection<ItemData> ItemsDataList = new ObservableCollection<ItemData>();
            var existingItemCodes = new HashSet<string>(); // To keep track of existing ItemCodes
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.Parameters.AddWithValue("@Code", ForCode);

                command.CommandText = string.Format(@"select D.ForCode,D.ForecastId,D.ItemId,D.Date,D.Demand,D.DateStr,
a.ItemId,a.ItemCode,a.MesUnit,a.CanBeProduced,A.OutputOrderFlag,A.InputOrderFlag
from DemandForecast as D
Inner  Join Rmaster as A on A.ItemID = D.ItemId
Where ForCode =@Code");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DemandForecastData fdata = new DemandForecastData();

                        ItemData idata = new ItemData();
                        fdata.Item = new ItemData();

                        fdata.ForCode = reader["Forcode"].ToString();
                        fdata.Item.ItemId = int.Parse(reader["ItemId"].ToString());
                        fdata.Item.ItemCode = reader["ItemCode"].ToString();
                        fdata.Demand = decimal.Parse(reader["Demand"].ToString());
                        fdata.Date = Convert.ToDateTime(reader["Date"]);
                        fdata.DateStr = reader["DateStr"].ToString();
                        fdata.Selected = true;
                        ForecastDataList.Add(fdata);
                        // Only process if this ItemCode hasn't been processed before
                        if (!existingItemCodes.Contains(fdata.Item.ItemCode))
                        {


                            // Get additional info for this ItemCode
                            idata = GetItemChooserData(0, fdata.Item.ItemCode);
                            ItemsDataList.Add(idata);

                            // Add this ItemCode to the set of processed ItemCodes
                            existingItemCodes.Add(fdata.Item.ItemCode);
                        }
                    }
                }

                connection.Close();
            }
            MRPData.Forecast.DemandForecast = ForecastDataList;
            MRPData.EndItems = ItemsDataList;
            return MRPData;
        }


        public ObservableCollection<DemandForecastData> GetDemandForecast(string ForCode)
        {
            ObservableCollection<DemandForecastData> DataList = new ObservableCollection<DemandForecastData>();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.Parameters.AddWithValue("@Code", ForCode);

                command.CommandText = string.Format(@"select D.ForCode,D.ForecastId,D.ItemId,D.Date,D.Demand,D.DateStr,
a.ItemId,a.ItemCode,a.MesUnit,a.CanBeProduced,A.OutputOrderFlag,A.InputOrderFlag
from DemandForecast as D
Inner  Join Rmaster as A on A.ItemID = D.ItemId
Where ForCode =@Code");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        DemandForecastData data = new DemandForecastData();
                        data.Item = new ItemData();

                        data.ForCode = reader["Forcode"].ToString();

                        data.Item.ItemId = int.Parse(reader["ItemId"].ToString());

                        data.Item.ItemCode = reader["ItemCode"].ToString();


                        data.Demand = decimal.Parse(reader["Demand"].ToString());
                        data.Date = Convert.ToDateTime(reader["Date"]);

                        data.DateStr = reader["DateStr"].ToString();
                        data.Selected = true;

                        DataList.Add(data);

                    }
                }

                connection.Close();
            }

            return DataList;
        }

        public ForecastInfoData GetForecastInfoChooserData(int id, string Code)
        {
            ForecastInfoData data = new ForecastInfoData();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (id > 0)
                {
                    command.Parameters.AddWithValue("@ID", id);

                    command.CommandText = string.Format(@"select ID, Forcode, ForDescr, TIMEBUCKET, PERIODTYPE, PeriodNum, HoursPerTimeBucket,NumberOfBuckets,IsDeleted,MRPForecast,
        ForDescr, Notes, Datefrom, DateTo from ForecastInfo
        Where ID=@ID");
                }
                else if (!string.IsNullOrWhiteSpace(Code))
                {
                    command.Parameters.AddWithValue("@Code", Code); // Corrected variable name

                    command.CommandText = string.Format(@"select ID, Forcode, ForDescr, TIMEBUCKET, PERIODTYPE, PeriodNum, HoursPerTimeBucket,NumberOfBuckets,IsDeleted,MRPForecast,
        ForDescr, Notes, Datefrom, DateTo from ForecastInfo
        Where Forcode=@Code"); // Corrected parameter name
                }



                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        data.ID = int.Parse(reader["ID"].ToString());
                        data.ForCode = reader["Forcode"].ToString();
                        data.ForDescr = reader["Fordescr"].ToString();
                        data.Notes = reader["Notes"].ToString();

                        data.TimeBucket = (BasicEnums.Timebucket)Enum.Parse(typeof(BasicEnums.Timebucket), reader["TIMEBUCKET"].ToString());
                        data.PeriodType = (BasicEnums.PeriodType)Enum.Parse(typeof(BasicEnums.PeriodType), reader["PeriodType"].ToString());
                        data.PeriodNumber = int.Parse(reader["PeriodNum"].ToString());
                        data.HoursPerTimeBucket = int.Parse(reader["HoursPerTimeBucket"].ToString());

                        data.DateFrom = Convert.ToDateTime(reader["Datefrom"]);
                        data.DateTo = Convert.ToDateTime(reader["DateTo"]);
                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());
                        data.MRPForecast = bool.Parse(reader["MRPForecast"].ToString());
                        data.NumberOfBuckets = int.Parse(reader["NumberOfBuckets"].ToString());


                    }
                }

                connection.Close();
            }

            return data;
        }
        #endregion

        #endregion
        

        #region Manufacture

        #region MPS

        public bool SaveMachRepairOnlyData(MPSInputData Data)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {


                    if (Data.NumberOfRepairsOnly == true)
                    {
                        var matchingRows = dbContext.NumberOfRepairsOnlyMPS.Where(b => b.MPSId == Data.MPSId).ToList();
                        bool HasRows = matchingRows.Count > 0;

                        foreach(var Machine in Data.PrimaryMachines)
                        {
                            var databaseRow = dbContext.NumberOfRepairsOnlyMPS.SingleOrDefault(b => b.MPSId == Data.MPSId && b.MachId == Machine.MachID);
                            var currentRow = Data.MachRepairOnlyData.SingleOrDefault(b => b.MPSId == Data.MPSId && b.Mach.MachID == Machine.MachID);
                           
                            if(currentRow!= null)
                            {
                                if (databaseRow == null && currentRow.NumberOfRepairsMPS != 0)
                                {
                                    NumberOfRepairsOnlyMPSEntity newRow = new NumberOfRepairsOnlyMPSEntity();
                                    newRow.MPSId = Data.MPSId;
                                    newRow.MachId = Machine.MachID;
                                    newRow.NumberOfRepairs = (int)currentRow.NumberOfRepairsMPS;


                                    dbContext.NumberOfRepairsOnlyMPS.Add(newRow);
                                }
                                else if (databaseRow != null && currentRow.NumberOfRepairsMPS == 0)
                                {
                                    dbContext.NumberOfRepairsOnlyMPS.Remove(databaseRow);
                                }
                                else if (databaseRow != null && databaseRow.NumberOfRepairs != currentRow.NumberOfRepairsMPS && currentRow.NumberOfRepairsMPS != 0)
                                {
                                    databaseRow.NumberOfRepairs = currentRow.NumberOfRepairsMPS;
                                }
                            }

                        }


                        dbContext.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                }
            }
            catch (Exception ex)
            {
                LogError(ex, "SaveMachRepairOnlyData", "Notes");
                return false;
            }
        }

        public bool SaveMachRepairDateData(MPSInputData Data)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {


                    if (Data.NumberDatesOfRepairs == true)
                    {
                        var matchingRows = dbContext.NumberDatesOfRepairsMPS.Where(b => b.MPSId == Data.MPSId).ToList();
                        bool HasRows = matchingRows.Count > 0;

                        foreach (var Machine in Data.PrimaryMachines)
                        {
                            foreach (var SelectedDateStr in Data.DatesStr)
                            { 

                            var databaseRow = dbContext.NumberDatesOfRepairsMPS.SingleOrDefault(b => b.MPSId == Data.MPSId && b.MachId == Machine.MachID
                                                                                               && b.RepairDateStr == SelectedDateStr);
                            var currentRow = Data.MachRepairDateData.SingleOrDefault(b => b.MPSId == Data.MPSId && b.Mach.MachID == Machine.MachID && b.RepairDateStr == SelectedDateStr);
                            if (databaseRow == null && currentRow.NumberOfRepairsMPS !=0)
                            {
                                NumberDatesOfRepairsMPSEntity newRow = new NumberDatesOfRepairsMPSEntity();
                                newRow.MPSId = Data.MPSId;
                                newRow.MachId = Machine.MachID;
                                newRow.RepairDateStr = SelectedDateStr;
                                newRow.RepairDate = DateTime.Now;

                                newRow.NumberOfRepairs = (int)currentRow.NumberOfRepairsMPS;


                                dbContext.NumberDatesOfRepairsMPS.Add(newRow);
                            }
                            else if (databaseRow != null && currentRow.NumberOfRepairsMPS == 0)
                            {
                                dbContext.NumberDatesOfRepairsMPS.Remove(databaseRow);
                            }
                            else if(databaseRow != null && databaseRow.NumberOfRepairs != currentRow.NumberOfRepairsMPS && currentRow.NumberOfRepairsMPS != 0)
                            {
                                databaseRow.NumberOfRepairs = currentRow.NumberOfRepairsMPS;
                            }
                         }
                        }


                        dbContext.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }


                }
            }
            catch (Exception ex)
            {
                LogError(ex, "SaveMachRepairOnlyData", "Notes");
                return false;
            }
        }
        public int SaveMpsInputData(MPSInputData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    int mpsId= flatData.MPSId;
                    var existingMPSQuery = dbContext.MPSInput.Where(c => c.MPSID == mpsId);
                    var existingMPS = existingMPSQuery.SingleOrDefault();

                    // Execute the query and get the result


                    if (existingMPS != null)
                    {


                        // Update existing customer
                        existingMPS.MPSCODE = flatData.MPSCode;
                        existingMPS.MPSDESCR = flatData.MPSDescr;
                        existingMPS.ItemsDefaultSettings = flatData.ItemsDefaultSettings;

                        existingMPS.ExistingSchedule = flatData.ExistingSchedule;
                        existingMPS.NumberOfRepairsOnly = flatData.NumberOfRepairsOnly;
                        existingMPS.NumberDatesOfRepairs = flatData.NumberDatesOfRepairs;
                        existingMPS.IsDeleted = flatData.IsDeleted;
                        existingMPS.HoldingCost = (double)flatData.HoldingCost;
                        existingMPS.MaxInventory = flatData.MaxInventory;
                        existingMPS.StoreTarget = flatData.InvStoreTarget;


                        //var ForecastIdQuery = dbContext.ForecastInfo.Where(c => c.ForCode == flatData.Forecast.ForCode);
                        //var Forecast = ForecastIdQuery.SingleOrDefault();
                        existingMPS.FORECASTID = flatData.Forecast.ID;
                        existingMPS.PriceListId = flatData.PriceList.Id;

                        #region MachineRepairsSave
                        if(flatData.NumberOfRepairsOnly == true)
                        {
                            var a = SaveMachRepairOnlyData(flatData);
                        }
                        else if(flatData.NumberDatesOfRepairs == true)
                        {
                            var b = SaveMachRepairDateData(flatData);
                        }
                        #endregion

                        dbContext.SaveChanges();
                        return 1;


                        #region PriceList Custom Settings //FTIAKSE AUTO
                        #endregion

                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "SaveMpsInputData", "Notes");
                return -1;
            }
        }
        public int AddMpsInputData(MPSInputData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    var existingMPSQuery = dbContext.MPSInput.Where(r => r.MPSCODE == flatData.MPSCode);
                    var existingMPS = existingMPSQuery.SingleOrDefault();
                    // Execute the query and get the result


                    if (existingMPS == null)
                    {
                        var newMPS = new MPSInputDataEntity();
                        // Insert new item
                        newMPS.MPSCODE = flatData.MPSCode;
                        newMPS.MPSDESCR = flatData.MPSDescr;
                        newMPS.ItemsDefaultSettings = false;
                        newMPS.ExistingSchedule = false;
                        newMPS.NumberOfRepairsOnly = false;
                        newMPS.NumberDatesOfRepairs = false;
                        newMPS.IsDeleted = false;
                        newMPS.HoldingCost = 0;
                        newMPS.MaxInventory = 0;
                        newMPS.StoreTarget = 0;

                        var firstForecast = dbContext.ForecastInfo.FirstOrDefault(r => r.IsDeleted == false);
                        newMPS.FORECASTID = firstForecast.ID;


                        var firstPriceList = dbContext.CPriceList.FirstOrDefault(r => r.IsDeleted == false);
                        newMPS.PriceListId = firstPriceList.PriceListId;

                        dbContext.MPSInput.Add(newMPS);

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
                LogError(ex, "AddCustomerData", "Notes");
                return 2;

            }
        }

        public ObservableCollection<MPSInputData> GetMPSData(bool ShowDeleted)
        {
            ObservableCollection<MPSInputData> DataList = new ObservableCollection<MPSInputData>();

            string FilterStr = "";
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = String.Format(@"and A.IsDeleted = @ShowDeleted");

                }

                command.CommandText = string.Format(@"SELECT A.MPSID, A.MPSCODE, A.MPSDESCR,A.ItemsDefaultSettings,A.IsDeleted,A.HoldingCost,A.StoreTarget,A.MaxInventory,
    A.ForecastId, F.FORCODE, F.TIMEBUCKET, F.PeriodNum, F.PERIODTYPE,F.HoursPerTimeBucket,
    D.PriceListId, D.PriceListCode,
    A.NumberDatesOfRepairs, A.NumberOfRepairsOnly, A.ExistingSchedule
FROM MPSINPUT AS A
Left JOIN ForecastInfo AS F ON F.ID = A.FORECASTID
Left JOIN CPriceList AS D ON D.PriceListId = A.PriceListId
Where 1=1 {0}", FilterStr);


                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MPSInputData data = new MPSInputData();
                        data.Forecast = new ForecastInfoData();
                        data.PriceList = new PriceListData();

                        data.MPSId = Convert.ToInt32(reader["MPSID"]);
                        data.MPSCode = reader["MPSCODE"].ToString();
                        data.MPSDescr = reader["MPSDESCR"].ToString();
                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());

                        data.NumberDatesOfRepairs = bool.Parse(reader["NumberDatesOfRepairs"].ToString());
                        data.NumberOfRepairsOnly = bool.Parse(reader["NumberOfRepairsOnly"].ToString());
                        data.ExistingSchedule = bool.Parse(reader["ExistingSchedule"].ToString());
                        data.ItemsDefaultSettings = bool.Parse(reader["ItemsDefaultSettings"].ToString());


                        data.Forecast.ID = int.Parse(reader["ForecastId"].ToString());

                        data.Forecast.ForCode = reader["FORCODE"].ToString();
                        data.Forecast.TimeBucket = (BasicEnums.Timebucket)Enum.Parse(typeof(BasicEnums.Timebucket), reader["TIMEBUCKET"].ToString());
                        data.Forecast.PeriodType = (BasicEnums.PeriodType)Enum.Parse(typeof(BasicEnums.PeriodType), reader["PeriodType"].ToString());
                        data.Forecast.PeriodNumber = int.Parse(reader["PeriodNum"].ToString());
                        data.Forecast.HoursPerTimeBucket = int.Parse(reader["HoursPerTimeBucket"].ToString());

                        data.PriceList.Id = Convert.ToInt32(reader["PriceListId"]);
                        data.PriceList.Code = reader["PriceListCode"].ToString();

                        data.InvStoreTarget = int.Parse(reader["StoreTarget"].ToString());
                        data.HoldingCost = double.Parse(reader["HoldingCost"].ToString());
                        data.MaxInventory = int.Parse(reader["MaxInventory"].ToString());

                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }

        public MPSInputData GetMPSChooserData(int MPSId,string MPSCode,MPSInputData Data)
        {

            string FilterStr = "";
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (MPSId > 0)
                {
                    command.Parameters.AddWithValue("@ID", MPSId);
                    FilterStr = String.Format(@" and A.MPSID =@ID");

                }

                else if (!string.IsNullOrWhiteSpace(MPSCode))
                {
                    command.Parameters.AddWithValue("@MPSCode", MPSCode);
                    FilterStr = String.Format(@" and A.MPSCode =@MPSCode");

                }

                command.CommandText = string.Format(@"SELECT A.MPSID, A.MPSCODE, A.MPSDESCR,A.ItemsDefaultSettings,A.IsDeleted,A.HoldingCost,A.StoreTarget,A.MaxInventory,
    A.ForecastId, F.FORCODE,F.FORDESCR, F.TIMEBUCKET, F.PeriodNum, F.PERIODTYPE,F.HoursPerTimeBucket,
    D.PriceListId, D.PriceListCode,
    A.NumberDatesOfRepairs, A.NumberOfRepairsOnly, A.ExistingSchedule
FROM MPSINPUT AS A
INNER JOIN ForecastInfo AS F ON F.ID = A.FORECASTID
INNER JOIN CPriceList AS D ON D.PriceListId = A.PriceListId
Where 1=1 {0}", FilterStr);


                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {


                        Data.MPSId = Convert.ToInt32(reader["MPSID"]);
                        Data.MPSCode = reader["MPSCODE"].ToString();
                        Data.MPSDescr = reader["MPSDESCR"].ToString();
                        Data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());

                        Data.NumberDatesOfRepairs = bool.Parse(reader["NumberDatesOfRepairs"].ToString());
                        Data.NumberOfRepairsOnly = bool.Parse(reader["NumberOfRepairsOnly"].ToString());
                        Data.ExistingSchedule = bool.Parse(reader["ExistingSchedule"].ToString());
                        Data.ItemsDefaultSettings = bool.Parse(reader["ItemsDefaultSettings"].ToString());

                        Data.Forecast.ID = int.Parse(reader["ForecastId"].ToString());
                        Data.Forecast.ForCode = reader["FORCODE"].ToString();
                        Data.Forecast.ForDescr = reader["FORCODE"].ToString();
                        Data.Forecast.TimeBucket = (BasicEnums.Timebucket)Enum.Parse(typeof(BasicEnums.Timebucket), reader["TIMEBUCKET"].ToString());
                        Data.Forecast.PeriodType = (BasicEnums.PeriodType)Enum.Parse(typeof(BasicEnums.PeriodType), reader["PeriodType"].ToString());
                        Data.Forecast.PeriodNumber = int.Parse(reader["PeriodNum"].ToString());
                        Data.Forecast.HoursPerTimeBucket = int.Parse(reader["HoursPerTimeBucket"].ToString());

                        Data.PriceList.Id = Convert.ToInt32(reader["PriceListId"]);
                        Data.PriceList.Code = reader["PriceListCode"].ToString();

                        Data.InvStoreTarget = int.Parse(reader["StoreTarget"].ToString());
                        Data.HoldingCost = double.Parse(reader["HoldingCost"].ToString());
                        Data.MaxInventory = int.Parse(reader["MaxInventory"].ToString());

                    }
                }

                connection.Close();
            }
            if(Data.NumberOfRepairsOnly == true || Data.NumberDatesOfRepairs == true)
            {
                Data.MachRepairOnlyData = GetMPSMachRepairData(Data);
            }
            return Data;
        }

        public ObservableCollection<MachineRepairData> GetMPSMachRepairData(MPSInputData InputData)
        {
            ObservableCollection<MachineRepairData> data = new ObservableCollection<MachineRepairData>();
            try
            {
                using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.Parameters.AddWithValue("@MPSId", InputData.MPSId);
                if (InputData.NumberOfRepairsOnly == true)
                {
                    command.CommandText = @"
                        select A.Id,A.MPSId,A.NumberOfRepairs,M.MachId,M.MachCode,M.MachDescr
                        from NumberOfRepairsOnlyMPS as A
						Inner Join Machines as M on A.MachId = M.MachID
                        Where MPSId = @MPSId";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MachineRepairData repairData = new MachineRepairData();
                            MachineData machData = new MachineData();
                            repairData.Id = int.Parse(reader["Id"].ToString());
                            repairData.MPSId = int.Parse(reader["MPSId"].ToString());
                            repairData.NumberOfRepairsMPS = int.Parse(reader["NumberOfRepairs"].ToString());

                            machData.MachID = int.Parse(reader["MachID"].ToString());
                            machData.MachCode = reader["MachCode"].ToString();
                            machData.MachDescr = reader["MachDescr"].ToString();

                            repairData.Mach = machData;


                            data.Add(repairData);
                        }
                    }
                }
                else if (InputData.NumberDatesOfRepairs == true)
                {
                    command.CommandText = @"
                    select A.Id,A.MPSId,A.NumberOfRepairs,A.RepairDate,A.RepairDateStr,M.MachId,M.MachCode,M.MachDescr
                    from NumberDatesOfRepairsMPS as A
                    Inner Join Machines as M on A.MachId = M.MachID
                    Where MPSId = @MPSId";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MachineRepairData repairData = new MachineRepairData();
                            MachineData machData = new MachineData();
                            repairData.Id = int.Parse(reader["Id"].ToString());
                            repairData.MPSId = int.Parse(reader["MPSId"].ToString());
                            repairData.NumberOfRepairsMPS = int.Parse(reader["NumberOfRepairs"].ToString());
                            repairData.RepairDateStr = reader["RepairDateStr"].ToString();
                            repairData.RepairDate = DateTime.Parse(reader["RepairDate"].ToString());

                                machData.MachID = int.Parse(reader["MachID"].ToString());
                            machData.MachCode = reader["MachCode"].ToString();
                            machData.MachDescr = reader["MachDescr"].ToString();

                            repairData.Mach = machData;


                            data.Add(repairData);
                        }
                    }
                }

                connection.Close();
                return data;


            }


            }
            catch (Exception ex)
            {
                LogError(ex, "GetMPSMachRepairData", "Notes");
                return data = null;
            }
        }


        public ObservableCollection<OptimizationResultsInvData> GetOptimisationMPSData(string ItemCode, InvDiagramsSearchData FilterData)
        {
            ObservableCollection<OptimizationResultsInvData> DataList = new ObservableCollection<OptimizationResultsInvData>();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;


                if (!string.IsNullOrEmpty(ItemCode))
                {
                    command.Parameters.AddWithValue("@ItemCode", ItemCode);
                    command.CommandText = string.Format(@"Select ItemId from Rmaster 
Where ItemCode = @ItemCode");

                    var ItemId = command.ExecuteScalar();
                    command.Parameters.AddWithValue("@ItemId", ItemId);

                }


                var DateStart = FilterData.DateStart.ToString("yyyy-dd-MM");
                var DateEnd = FilterData.DateEnd.ToString("yyyy-dd-MM");

                command.Parameters.AddWithValue("@DateStart", DateStart);
                command.Parameters.AddWithValue("@DateEnd", DateEnd);

                command.CommandText = string.Format(@"
SELECT Demand,ForecastedDemand,Batchsize,StockQ,ProductionBool,iDay FROM Inventory_Demand
Where iDay Between  @DateStart AND @DateEnd and ItemId=@ItemId");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        OptimizationResultsInvData Data = new OptimizationResultsInvData();


                        Data.Demand = float.Parse(reader["Demand"].ToString());

                        Data.DemandForecast = float.Parse(reader["ForecastedDemand"].ToString());
                        Data.BatchSize = float.Parse(reader["Batchsize"].ToString());
                        Data.StockQ = float.Parse(reader["StockQ"].ToString());
                        Data.ProductionBool = bool.Parse(reader["ProductionBool"].ToString());
                        Data.iDay = DateTime.Parse(reader["iDay"].ToString());



                        DataList.Add(Data);
                    }
                }

                connection.Close();
            }
            return DataList;
        }

        //public ObservableCollection<DemandForecastData> 
        #endregion
        #region Machines

        public ObservableCollection<MachineData> GetMachineChooserData(bool ShowDeleted)
        {
            ObservableCollection<MachineData> DataList = new ObservableCollection<MachineData>();

            string FilterStr = "";

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = String.Format(@" and Machines.IsDeleted =@ShowDeleted");
                }

                command.CommandText = string.Format(@"SELECT 
            Machines.MachID,Machines.MachCode,Machines.MachDescr,Machines.FactoryID,Machines.LastMaintenance,Machines.NextMaintenance,Machines.IsDeleted,
            Machines.TotalOperatingHours,Machines.FailureRate,Machines.ProductionRate,Machines.EfficiencyRate,
            Machines.AverageRepairTime,Machines.NumberOfFailures,Machines.MachineType,Machines.ModelYear,Machines.PrimaryModel,Machines.NumberOfMachines,
            Machines.DateInstalled,Machines.Status,Factory.FactoryID,Factory.Code,Factory.Descr
        FROM 
            Machines
        INNER JOIN
            Factory ON Machines.FactoryID = Factory.FactoryID
Where 1=1 {0}",FilterStr);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MachineData data = new MachineData();
                        data.Factory = new FactoryData();

                        data.MachID = Convert.ToInt32(reader["MachID"]);
                        data.MachCode = reader["MachCode"].ToString();
                        data.MachDescr = reader["MachDescr"].ToString();
                        data.LastMaintenance = Convert.ToDateTime(reader["LastMaintenance"]);
                        data.NextMaintenance = Convert.ToDateTime(reader["NextMaintenance"]);
                        data.TotalOperatingHours = Convert.ToInt32(reader["TotalOperatingHours"]);
                        data.FailureRate = Convert.ToSingle(reader["FailureRate"]);
                        data.ProductionRate = Convert.ToInt32(reader["ProductionRate"]);
                        data.EfficiencyRate = Convert.ToSingle(reader["EfficiencyRate"]);
                        data.AverageRepairTime = Convert.ToSingle(reader["AverageRepairTime"]);
                        data.NumberOfFailures = Convert.ToInt32(reader["NumberOfFailures"]);
                        data.MachineType = (BasicEnums.MachType)Enum.Parse(typeof(BasicEnums.MachType), reader["MachineType"].ToString());
                        data.ModelYear = Convert.ToInt32(reader["ModelYear"]);
                        data.DateInstalled = Convert.ToDateTime(reader["DateInstalled"]);
                        data.Status = (BasicEnums.MachStatus)Enum.Parse(typeof(BasicEnums.MachStatus), reader["Status"].ToString());
                        data.PrimaryModel = bool.Parse(reader["PrimaryModel"].ToString());
                        data.NumberOfMachines = int.Parse(reader["NumberOfMachines"].ToString());

                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());

                        data.Factory.FactoryID = int.Parse(reader["FactoryID"].ToString());
                        data.Factory.Code = reader["Code"].ToString();
                        data.Factory.Descr = reader["Descr"].ToString();

                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }

        public MachineData GetMachineData(MachineData Filter)
        {
            MachineData Data = new MachineData();
            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;

                    command.Parameters.AddWithValue("@MachCode", Filter.MachCode);

                    command.CommandText = string.Format(@"SELECT 
                Machines.MachID,Machines.MachCode,Machines.MachDescr,Machines.FactoryID,Machines.LastMaintenance,Machines.NextMaintenance,
                Machines.TotalOperatingHours,Machines.FailureRate,Machines.ProductionRate,Machines.EfficiencyRate,
                Machines.AverageRepairTime,Machines.NumberOfFailures,Machines.MachineType,Machines.ModelYear,Machines.PrimaryModel,Machines.NumberOfMachines,
                Machines.DateInstalled,Machines.Status,Factory.FactoryID,Factory.Code,Factory.Descr
            FROM 
                Machines
            INNER JOIN
                Factory ON Machines.FactoryID = Factory.FactoryID
            WHERE 
                Machines.MachCode = @MachCode;");
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Data.Factory = new FactoryData();

                            Data.MachID = Convert.ToInt32(reader["MachID"]);
                            Data.MachCode = reader["MachCode"].ToString();
                            Data.MachDescr = reader["MachDescr"].ToString();
                            Data.LastMaintenance = Convert.ToDateTime(reader["LastMaintenance"]);
                            Data.NextMaintenance = Convert.ToDateTime(reader["NextMaintenance"]);
                            Data.TotalOperatingHours = Convert.ToInt32(reader["TotalOperatingHours"]);
                            Data.FailureRate = Convert.ToSingle(reader["FailureRate"]);
                            Data.ProductionRate = Convert.ToInt32(reader["ProductionRate"]);
                            Data.EfficiencyRate = Convert.ToSingle(reader["EfficiencyRate"]);
                            Data.AverageRepairTime = Convert.ToSingle(reader["AverageRepairTime"]);
                            Data.NumberOfFailures = Convert.ToInt32(reader["NumberOfFailures"]);
                            Data.ModelYear = Convert.ToInt32(reader["ModelYear"]);
                            Data.DateInstalled = Convert.ToDateTime(reader["DateInstalled"]);
                            Data.MachineType = (BasicEnums.MachType)Enum.Parse(typeof(BasicEnums.MachType), reader["MachineType"].ToString());
                            Data.Status = (BasicEnums.MachStatus)Enum.Parse(typeof(BasicEnums.MachStatus), reader["Status"].ToString());
                            Data.PrimaryModel = bool.Parse(reader["PrimaryModel"].ToString());
                            Data.NumberOfMachines = int.Parse(reader["NumberOfMachines"].ToString());
                            Data.Factory.Code = reader["Code"].ToString();
                            Data.Factory.Descr = reader["Descr"].ToString();
                        }
                    }

                    connection.Close();
                }

                return Data;
            }
            catch (Exception ex)
            {
                LogError(ex, "SaveMachineData", "Notes");
                return Data = null;
            }
        }

        public int SaveMachineData(MachineData FlatData) //prepei na alaksw to factoryId stin nea mhxanh giati vazw apla to id =1
        {

            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    var machcode = FlatData.MachCode;
                    var existingMachineQuery = dbContext.Machines.Where(m => m.MachCode == machcode);
                    var existingMachine = existingMachineQuery.SingleOrDefault();

                    if (existingMachine == null && FlatData.PrimaryModel == false) //AUTO TI INE?
                    {
                        return 99;
                    };


                    if (existingMachine == null && FlatData.PrimaryModel == true)
                    {

                        #region Messagebox
                        if (FlatData.PrimaryModel == true)
                        {
                            var MachCodesList = new List<string>();
                            MachCodesList.Add($"{FlatData.MachCode}");

                            for (var i = 1; i <= FlatData.NumberOfMachines; i++)
                            {
                                MachCodesList.Add($"{FlatData.MachCode}{i}");
                            }
                            var allCodes = string.Join(", ", MachCodesList); // joining all codes with ", "
                            var result = System.Windows.MessageBox.Show($"A primary model and {FlatData.NumberOfMachines} Machines will be created with the Machine Codes : {allCodes}. Proceed?", "Confirmation", MessageBoxButton.YesNo);

                            if (result == MessageBoxResult.No)
                            {
                                return -2;
                            }

                        }
                        #endregion
                        MachineDataEntity newMachine;

                        for (var i = 0; i <= FlatData.NumberOfMachines; i++)
                        {
                            if (i == 0)
                            {
                                newMachine = new MachineDataEntity();

                                newMachine.MachCode = FlatData.MachCode;
                                newMachine.MachDescr = FlatData.MachDescr;
                                newMachine.FactoryID = 1;
                                newMachine.LastMaintenance = DateTime.Now;
                                newMachine.NextMaintenance = DateTime.Now;
                                newMachine.TotalOperatingHours = FlatData.TotalOperatingHours;
                                newMachine.FailureRate = FlatData.FailureRate;
                                newMachine.ProductionRate = FlatData.ProductionRate;
                                newMachine.EfficiencyRate = FlatData.EfficiencyRate;
                                newMachine.AverageRepairTime = FlatData.AverageRepairTime;
                                newMachine.NumberOfFailures = FlatData.NumberOfFailures;
                                newMachine.MachineType = FlatData.MachineType.ToString();
                                newMachine.ModelYear = FlatData.ModelYear;
                                newMachine.DateInstalled = DateTime.Now;
                                newMachine.Status = FlatData.Status.ToString();
                                newMachine.PrimaryModel = FlatData.PrimaryModel;
                                newMachine.NumberOfMachines = FlatData.NumberOfMachines;
                                newMachine.IsDeleted = false;


                                dbContext.Machines.Add(newMachine);
                            }
                            else
                            {
                                newMachine = new MachineDataEntity();

                                newMachine.MachCode = FlatData.MachCode + i.ToString();

                                newMachine.MachDescr = FlatData.MachDescr;
                                newMachine.FactoryID = 1;
                                newMachine.LastMaintenance = DateTime.Now;
                                newMachine.NextMaintenance = DateTime.Now;
                                newMachine.TotalOperatingHours = FlatData.TotalOperatingHours;
                                newMachine.FailureRate = FlatData.FailureRate;
                                newMachine.ProductionRate = FlatData.ProductionRate;
                                newMachine.EfficiencyRate = FlatData.EfficiencyRate;
                                newMachine.AverageRepairTime = FlatData.AverageRepairTime;
                                newMachine.NumberOfFailures = FlatData.NumberOfFailures;
                                newMachine.MachineType = FlatData.MachineType.ToString();
                                newMachine.ModelYear = FlatData.ModelYear;
                                newMachine.DateInstalled = DateTime.Now;
                                newMachine.Status = FlatData.Status.ToString();
                                newMachine.PrimaryModel = false;
                                newMachine.NumberOfMachines = 0;
                                newMachine.IsDeleted = false;


                            }

                            dbContext.Machines.Add(newMachine);
                        }

                        dbContext.SaveChanges();
                        return 0;
                    }

                    else
                    {

                        // Update existing machine
                        existingMachine.MachCode = FlatData.MachCode;
                        existingMachine.MachDescr = FlatData.MachDescr;
                        existingMachine.FactoryID = FlatData.Factory.FactoryID;
                        existingMachine.LastMaintenance = FlatData.LastMaintenance;
                        existingMachine.NextMaintenance = FlatData.NextMaintenance;
                        existingMachine.TotalOperatingHours = FlatData.TotalOperatingHours;
                        existingMachine.FailureRate = FlatData.FailureRate;
                        existingMachine.ProductionRate = FlatData.ProductionRate;
                        existingMachine.EfficiencyRate = FlatData.EfficiencyRate;
                        existingMachine.AverageRepairTime = FlatData.AverageRepairTime;
                        existingMachine.NumberOfFailures = FlatData.NumberOfFailures;
                        existingMachine.MachineType = FlatData.MachineType.ToString();
                        existingMachine.ModelYear = FlatData.ModelYear;
                        existingMachine.DateInstalled = FlatData.DateInstalled;
                        existingMachine.Status = FlatData.Status.ToString();
                        existingMachine.PrimaryModel = FlatData.PrimaryModel;
                        existingMachine.NumberOfMachines = FlatData.NumberOfMachines;
                        existingMachine.IsDeleted = FlatData.IsDeleted;

                        dbContext.SaveChanges();
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "SaveMachineData", "Notes");
                return -1;
            }
        }

        public ObservableCollection<MachineData> GetPrimaryMachineChooserData()
        {
            ObservableCollection<MachineData> DataList = new ObservableCollection<MachineData>();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = string.Format(@"SELECT 
            Machines.MachID,Machines.MachCode,Machines.MachDescr,Machines.FactoryID,Machines.LastMaintenance,Machines.NextMaintenance,
            Machines.TotalOperatingHours,Machines.FailureRate,Machines.ProductionRate,Machines.EfficiencyRate,
            Machines.AverageRepairTime,Machines.NumberOfFailures,Machines.MachineType,Machines.ModelYear,Machines.PrimaryModel,Machines.NumberOfMachines,
            Machines.DateInstalled,Machines.Status,Machines.IsDeleted,Factory.FactoryID,Factory.Code,Factory.Descr
        FROM 
            Machines
        INNER JOIN
            Factory ON Machines.FactoryID = Factory.FactoryID
			Where Machines.PrimaryModel = 1 and Machines.IsDeleted = 0;");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MachineData data = new MachineData();
                        data.Factory = new FactoryData();

                        data.MachID = Convert.ToInt32(reader["MachID"]);
                        data.MachCode = reader["MachCode"].ToString();
                        data.MachDescr = reader["MachDescr"].ToString();
                        data.LastMaintenance = Convert.ToDateTime(reader["LastMaintenance"]);
                        data.NextMaintenance = Convert.ToDateTime(reader["NextMaintenance"]);
                        data.TotalOperatingHours = Convert.ToInt32(reader["TotalOperatingHours"]);
                        data.FailureRate = Convert.ToSingle(reader["FailureRate"]);
                        data.ProductionRate = Convert.ToInt32(reader["ProductionRate"]);
                        data.EfficiencyRate = Convert.ToSingle(reader["EfficiencyRate"]);
                        data.AverageRepairTime = Convert.ToSingle(reader["AverageRepairTime"]);
                        data.NumberOfFailures = Convert.ToInt32(reader["NumberOfFailures"]);
                        data.MachineType = (BasicEnums.MachType)Enum.Parse(typeof(BasicEnums.MachType), reader["MachineType"].ToString());
                        data.ModelYear = Convert.ToInt32(reader["ModelYear"]);
                        data.DateInstalled = Convert.ToDateTime(reader["DateInstalled"]);
                        data.Status = (BasicEnums.MachStatus)Enum.Parse(typeof(BasicEnums.MachStatus), reader["Status"].ToString());
                        data.PrimaryModel = bool.Parse(reader["PrimaryModel"].ToString());
                        data.NumberOfMachines = int.Parse(reader["NumberOfMachines"].ToString());

                        data.Factory.FactoryID = int.Parse(reader["FactoryID"].ToString());
                        data.Factory.Code = reader["Code"].ToString();
                        data.Factory.Descr = reader["Descr"].ToString();

                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }

        public ObservableCollection<MachineData> GetNonPrimaryMachineChooserData()
        {
            ObservableCollection<MachineData> DataList = new ObservableCollection<MachineData>();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = string.Format(@"SELECT 
            Machines.MachID,Machines.MachCode,Machines.MachDescr,Machines.FactoryID,Machines.LastMaintenance,Machines.NextMaintenance,
            Machines.TotalOperatingHours,Machines.FailureRate,Machines.ProductionRate,Machines.EfficiencyRate,
            Machines.AverageRepairTime,Machines.NumberOfFailures,Machines.MachineType,Machines.ModelYear,Machines.PrimaryModel,Machines.NumberOfMachines,
            Machines.DateInstalled,Machines.Status,Factory.FactoryID,Factory.Code,Factory.Descr
        FROM 
            Machines
        INNER JOIN
            Factory ON Machines.FactoryID = Factory.FactoryID
			Where Machines.PrimaryModel = 0 and Machines.isDeleted = 0;");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MachineData data = new MachineData();
                        data.Factory = new FactoryData();

                        data.MachID = Convert.ToInt32(reader["MachID"]);
                        data.MachCode = reader["MachCode"].ToString();
                        data.MachDescr = reader["MachDescr"].ToString();
                        data.LastMaintenance = Convert.ToDateTime(reader["LastMaintenance"]);
                        data.NextMaintenance = Convert.ToDateTime(reader["NextMaintenance"]);
                        data.TotalOperatingHours = Convert.ToInt32(reader["TotalOperatingHours"]);
                        data.FailureRate = Convert.ToSingle(reader["FailureRate"]);
                        data.ProductionRate = Convert.ToInt32(reader["ProductionRate"]);
                        data.EfficiencyRate = Convert.ToSingle(reader["EfficiencyRate"]);
                        data.AverageRepairTime = Convert.ToSingle(reader["AverageRepairTime"]);
                        data.NumberOfFailures = Convert.ToInt32(reader["NumberOfFailures"]);
                        data.MachineType = (BasicEnums.MachType)Enum.Parse(typeof(BasicEnums.MachType), reader["MachineType"].ToString());
                        data.ModelYear = Convert.ToInt32(reader["ModelYear"]);
                        data.DateInstalled = Convert.ToDateTime(reader["DateInstalled"]);
                        data.Status = (BasicEnums.MachStatus)Enum.Parse(typeof(BasicEnums.MachStatus), reader["Status"].ToString());
                        data.PrimaryModel = bool.Parse(reader["PrimaryModel"].ToString());
                        data.NumberOfMachines = int.Parse(reader["NumberOfMachines"].ToString());

                        data.Factory.FactoryID = int.Parse(reader["FactoryID"].ToString());
                        data.Factory.Code = reader["Code"].ToString();
                        data.Factory.Descr = reader["Descr"].ToString();

                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }

        #endregion

        #region Mrp

        public ObservableCollection<ForecastInfoData> GetMRPForecast(bool ShowDeleted)
        {
            ObservableCollection<ForecastInfoData> DataList = new ObservableCollection<ForecastInfoData>();


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

                command.CommandText = string.Format(@"select ID,Forcode,ForDescr,TIMEBUCKET,PERIODTYPE,PeriodNum,HoursPerTimeBucket,NumberOfBuckets,IsDeleted,MRPForecast,
ForDescr,Notes,Datefrom,DateTo from ForecastInfo
Where TimeBucket='Daily' and NumberOfBuckets < 15{0}", FilterStr);




                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ForecastInfoData data = new ForecastInfoData();

                        data.ID = int.Parse(reader["ID"].ToString());
                        data.ForCode = reader["Forcode"].ToString();
                        data.ForDescr = reader["Fordescr"].ToString();
                        data.Notes = reader["Notes"].ToString();

                        data.TimeBucket = (BasicEnums.Timebucket)Enum.Parse(typeof(BasicEnums.Timebucket), reader["TIMEBUCKET"].ToString());
                        data.PeriodType = (BasicEnums.PeriodType)Enum.Parse(typeof(BasicEnums.PeriodType), reader["PeriodType"].ToString());
                        data.PeriodNumber = int.Parse(reader["PeriodNum"].ToString());
                        data.HoursPerTimeBucket = int.Parse(reader["HoursPerTimeBucket"].ToString());

                        data.DateFrom = Convert.ToDateTime(reader["Datefrom"]);
                        data.DateTo = Convert.ToDateTime(reader["DateTo"]);
                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());
                        data.MRPForecast = bool.Parse(reader["MRPForecast"].ToString());
                        data.NumberOfBuckets = int.Parse(reader["NumberOfBuckets"].ToString());


                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }

        public ObservableCollection<MRPInputData> GetMRPData(bool ShowDeleted)
        {
            ObservableCollection<MRPInputData> DataList = new ObservableCollection<MRPInputData>();

            string FilterStr = "";
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = String.Format(@"and A.IsDeleted = @ShowDeleted");

                }

                command.CommandText = string.Format(@"SELECT A.MRPID, A.MRPCODE, A.MRPDESCR,A.FORECASTID,A.ISDELETED,A.INVID,A.FORECASTFLAG,A.ORDERSFLAG,A.SELECTEDITEMS,F.NumberOfBuckets,
    A.ORDERSDATEFROM,A.ORDERSDATETO, F.FORCODE, F.TIMEBUCKET, F.PeriodNum, F.PERIODTYPE,F.HoursPerTimeBucket,
    D.InvId, D.InvCode
FROM MRPINPUT AS A
Left JOIN ForecastInfo AS F ON F.ID = A.FORECASTID
Left JOIN Inventory AS D ON D.InvId = A.INVID
Where 1=1 {0}", FilterStr);


                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MRPInputData data = new MRPInputData();
                        data.Forecast = new ForecastInfoData();
                        data.Inventory =  new InventoryData();

                        data.MRPID = Convert.ToInt32(reader["MRPID"]);
                        data.MRPCode = reader["MRPCODE"].ToString();
                        data.MRPDescr = reader["MRPDESCR"].ToString();
                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());

                        data.OrdersFlag = bool.Parse(reader["ORDERSFLAG"].ToString());
                        data.ForecastFlag = bool.Parse(reader["FORECASTFLAG"].ToString());
                        data.SelectedItems = bool.Parse(reader["SELECTEDITEMS"].ToString());
                        data.OrdersDateFrom = DateTime.Parse(reader["ORDERSDATEFROM"].ToString());
                        data.OrdersDateTo = DateTime.Parse(reader["ORDERSDATETO"].ToString());


                        data.Forecast.ID = int.Parse(reader["ForecastId"].ToString());

                        data.Forecast.ForCode = reader["FORCODE"].ToString();
                        data.Forecast.TimeBucket = (BasicEnums.Timebucket)Enum.Parse(typeof(BasicEnums.Timebucket), reader["TIMEBUCKET"].ToString());
                        data.Forecast.PeriodType = (BasicEnums.PeriodType)Enum.Parse(typeof(BasicEnums.PeriodType), reader["PeriodType"].ToString());
                        data.Forecast.PeriodNumber = int.Parse(reader["PeriodNum"].ToString());
                        data.Forecast.HoursPerTimeBucket = int.Parse(reader["HoursPerTimeBucket"].ToString());
                        data.Forecast.NumberOfBuckets = int.Parse(reader["NumberOfBuckets"].ToString());

                        data.Inventory.InvId = Convert.ToInt32(reader["InvId"]);
                        data.Inventory.InvCode = reader["InvCode"].ToString();


                        DataList.Add(data);
                    }
                }

                connection.Close();
            }

            return DataList;
        }

        public MRPInputData GetMRPChooserData(int MRPId, string MRPCode)
        {
            MRPInputData data = new MRPInputData();

            string FilterStr = "";
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (MRPId > 0)
                {
                    command.Parameters.AddWithValue("@ID", MRPId);
                    FilterStr = String.Format(@" and A.MRPID =@ID");

                }
                else if (!string.IsNullOrWhiteSpace(MRPCode))
                {
                    command.Parameters.AddWithValue("@MRPCODE", MRPCode);
                    FilterStr = String.Format(@" and A.MRPCODE =@MRPCODE");

                }

                command.CommandText = string.Format(@"SELECT A.MRPID, A.MRPCODE, A.MRPDESCR,A.FORECASTID,A.ISDELETED,A.INVID,A.FORECASTFLAG,A.ORDERSFLAG,A.SELECTEDITEMS,
    A.ORDERSDATEFROM,A.ORDERSDATETO, F.FORCODE, F.TIMEBUCKET, F.PeriodNum, F.PERIODTYPE,F.HoursPerTimeBucket,F.NumberOfBuckets,
    D.InvId, D.InvCode
FROM MRPINPUT AS A
Inner JOIN ForecastInfo AS F ON F.ID = A.FORECASTID
Inner JOIN Inventory AS D ON D.InvId = A.INVID
Where 1=1 {0}", FilterStr);


                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        data.Forecast = new ForecastInfoData();
                        data.Inventory = new InventoryData();

                        data.MRPID = Convert.ToInt32(reader["MRPID"]);
                        data.MRPCode = reader["MRPCODE"].ToString();
                        data.MRPDescr = reader["MRPDESCR"].ToString();
                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());

                        data.OrdersFlag = bool.Parse(reader["ORDERSFLAG"].ToString());
                        data.ForecastFlag = bool.Parse(reader["FORECASTFLAG"].ToString());
                        data.SelectedItems = bool.Parse(reader["SELECTEDITEMS"].ToString());
                        data.OrdersDateFrom = DateTime.Parse(reader["ORDERSDATEFROM"].ToString());
                        data.OrdersDateTo = DateTime.Parse(reader["ORDERSDATETO"].ToString());


                        data.Forecast.ID = int.Parse(reader["ForecastId"].ToString());

                        data.Forecast.ForCode = reader["FORCODE"].ToString();
                        data.Forecast.TimeBucket = (BasicEnums.Timebucket)Enum.Parse(typeof(BasicEnums.Timebucket), reader["TIMEBUCKET"].ToString());
                        data.Forecast.PeriodType = (BasicEnums.PeriodType)Enum.Parse(typeof(BasicEnums.PeriodType), reader["PeriodType"].ToString());
                        data.Forecast.PeriodNumber = int.Parse(reader["PeriodNum"].ToString());
                        data.Forecast.HoursPerTimeBucket = int.Parse(reader["HoursPerTimeBucket"].ToString());
                        data.Forecast.NumberOfBuckets = int.Parse(reader["NumberOfBuckets"].ToString());

                        data.Inventory.InvId = Convert.ToInt32(reader["InvId"]);
                        data.Inventory.InvCode = reader["InvCode"].ToString();

                    }
                }

                connection.Close();
            }

            return data;
        }

        public int SaveMRPInputData(MRPInputData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    int mrpId = flatData.MRPID;
                    var existingMRPQuery = dbContext.MRPInput.Where(c => c.MRPID == mrpId);
                    var existingMRP = existingMRPQuery.SingleOrDefault();

                    // Execute the query and get the result


                    if (existingMRP != null)
                    {


                        // Update existing customer
                        existingMRP.MRPCODE = flatData.MRPCode;
                        existingMRP.MRPDescr = flatData.MRPDescr;
                        existingMRP.FORECASTFLAG = flatData.ForecastFlag;
                        existingMRP.ORDERSFLAG = flatData.OrdersFlag;
                        existingMRP.SELECTEDITEMS = flatData.SelectedItems;
                        existingMRP.ORDERSDATEFROM = DateTime.Now;
                        existingMRP.ORDERSDATETO = DateTime.Now;
                        existingMRP.ISDELETED = flatData.IsDeleted;


                        existingMRP.FORECASTID = flatData.Forecast.ID;


                        existingMRP.INVID = flatData.Inventory.InvId;


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
                LogError(ex, "SaveMRPInputData", "Notes");
                return -1;
            }
        }
        public int AddMRPInputData(MRPInputData flatData)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Separate query from execution
                    var existingMRPQuery = dbContext.MRPInput.Where(r => r.MRPCODE == flatData.MRPCode);
                    var existingMRP = existingMRPQuery.SingleOrDefault();
                    // Execute the query and get the result


                    if (existingMRP == null)
                    {
                        var newMRP = new MRPInputDataEntity();
                        // Insert new item
                        newMRP.MRPCODE = flatData.MRPCode;
                        newMRP.MRPDescr = flatData.MRPDescr;
                        newMRP.FORECASTFLAG = flatData.ForecastFlag;
                        newMRP.ORDERSFLAG = flatData.OrdersFlag;
                        newMRP.SELECTEDITEMS = flatData.SelectedItems;
                        newMRP.ORDERSDATEFROM = DateTime.Now;
                        newMRP.ORDERSDATETO = DateTime.Now.AddDays(10);
                        newMRP.ISDELETED = flatData.IsDeleted;



                        newMRP.INVID = flatData.Inventory.InvId;


                        var mainMRPForecast = new ForecastInfoDataEntity();

                        var existingForecastQuery = dbContext.ForecastInfo.Where(r => r.MRPForecast == true);
                        mainMRPForecast = existingForecastQuery.SingleOrDefault();

                        newMRP.FORECASTID = mainMRPForecast.ID;



                        var existingmainInventoryQuery = dbContext.Inventory.Where(r => r.Production == true);//auto borei na thelei diorthosh .Tha prepei na epilegete apo proepilegmenh epilogh h estw na valw flag sto inventory ws maininventory tou sugkekriemnou factory
                        var mainInventory = existingmainInventoryQuery.SingleOrDefault();


                        newMRP.INVID = mainInventory.InvId;


                        dbContext.MRPInput.Add(newMRP);

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
                LogError(ex, "AddMRPInputData", "Notes");
                return 2;

            }
        }


        public MRPOutputData CalculateMRP(MRPInputData InputData)
        {
            GRBEnv env = new GRBEnv("mrplogfile.log");
            GRBModel model = new GRBModel(env);
            MRPOutputData OutputData = new MRPOutputData();
            OutputData.XData = new ObservableCollection<DecisionVariableX>();
            OutputData.YData = new ObservableCollection<DecisionVariableY>();
            OutputData.InvData = new ObservableCollection<DecisionVariablesInvStatus>();

            try
            {
                #region Optimization

                #region Optimization paramaters

                int T = InputData.T; //Planning Horizon
                int P = InputData.P; //Number Of MPS end-item Products
                int Q = InputData.Q; //Number Of MRP Component Products
                int W = InputData.W; //Number Of Capacitated Work Cenetrs
                int N = new int(); //Number Of Seuts

                string[] Dates = InputData.Dates; //Πινακας με τα Dates

                Dictionary<string, List<string>> Pw_String = InputData.Pw; // P(w) is the set of end-items that can be processed on work center w.
                Dictionary<string, List<string>> Qw_String = InputData.Qw; // Q(w) is the set of products that can be processed on work center w.
                Dictionary<(string, string), double> Dit_String = InputData.Dit; // Dit represents the demand for product i at the end of period t.
                Dictionary<string, List<string>> Ci_String = InputData.Ci; // C(i) represents the set of direct subcomponents of each component or end-item 
                Dictionary<(string, string), double> Rij_String = InputData.Rij; // Rij represents the number of units of direct subcomponent i required in each unit of component or end-item j.
                Dictionary<(string, string), double> Awt_String = InputData.Awt; // Awt represents the available capacity at work center w during period t.
                Dictionary<(string, string, string), double> Miwt_String = InputData.Miwt; // Mjwt represents the maximum production quantity for product j at work center w during period t
                Dictionary<(string, string, string), double> Sijw_String = InputData.Sijw; // Sijw represents the setup times for all products and workstations. 
                Dictionary<(string, string, string), double> SCijw_String = InputData.SCijw; // SCijw represents the setup costs for all products and workstations. 

                Dictionary<(string, string), double> Uiw_String = InputData.Uiw; // Ujw represents the unit production times for all products and workstations
                Dictionary<string, double> Hi_String = InputData.Hi; // Ujw represents the unit production times for all products and workstations
                Dictionary<string, double> Gi_String = InputData.Gi; // Ujw represents the unit production times for all products and workstations
                Dictionary<string, string> I0W_String = InputData.I0W; // Ujw represents the unit production times for all products and workstations

                Dictionary<string, (double, double)> Ii_String = InputData.Ii; // Ujw represents the unit production times for all products and workstations
                Dictionary<string, (double, double)> Imax_min_String = InputData.Imax_min; // Ujw represents the unit production times for all products and workstations


                Dictionary<int, List<int>> Pw = new Dictionary<int, List<int>>(); // P(w) is the set of end-items that can be processed on work center w.
                Dictionary<int, List<int>> Qw = new Dictionary<int, List<int>>(); // Q(w) is the set of products that can be processed on work center w.
                Dictionary<(int, int), double> Dit = new Dictionary<(int, int), double>(); // Dit represents the demand for product i at the end of period t.
                Dictionary<int, List<int>> Ci = new Dictionary<int, List<int>>(); // C(i) represents the set of direct subcomponents of each component or end-item 
                Dictionary<(int, int), double> Rij = new Dictionary<(int, int), double>(); // Rij represents the number of units of direct subcomponent i required in each unit of component or end-item j.
                Dictionary<(int, int), double> Awt = new Dictionary<(int, int), double>(); // Awt represents the available capacity at work center w during period t.
                Dictionary<(int, int, int), double> Miwt = new Dictionary<(int, int, int), double>(); // Mjwt represents the maximum production quantity for product j at work center w during period t
                Dictionary<(int, int, int), double> Sijw = new Dictionary<(int, int, int), double>(); // Sijw represents the setup times for all products and workstations. 
                Dictionary<(int, int, int), double> SCijw = new Dictionary<(int, int, int), double>(); // Sijw represents the setup times for all products and workstations. 

                Dictionary<(int, int), double> Uiw = new Dictionary<(int, int), double>(); // Ujw represents the unit production times for all products and workstations

                Dictionary<int, double> Hi = new Dictionary<int, double>(); // Ujw represents the unit production times for all products and workstations
                Dictionary<int, double> Gi = new Dictionary<int, double>(); // Ujw represents the unit production times for all products and workstations
                Dictionary<int, int> I0W = new Dictionary<int, int>(); // Ujw represents the unit production times for all products and workstations

                Dictionary<int, (double, double)> Ii = new Dictionary<int, (double, double)>();
                Dictionary<int, (double, double)> Imax_min = new Dictionary<int, (double, double)>();




                #region from string to int 
                    // Mapping from string keys to integer indices
                Dictionary<string, int> workCenterIndexMap = new Dictionary<string, int>();
                Dictionary<string, int> productIndexMap = new Dictionary<string, int>();
                Dictionary<string, int> dateIndexMap = new Dictionary<string, int>();

                // Initialize integer index counters
                int workCenterIndexCounter = 0;
                int productIndexCounter = 0;

                // Transform Pw_String to Pw
                foreach (var kvp in Pw_String)
                {
                    workCenterIndexMap[kvp.Key] = workCenterIndexCounter++;
                    Pw.Add(workCenterIndexMap[kvp.Key], new List<int>());

                    foreach (var product in kvp.Value)
                    {
                        if (!productIndexMap.ContainsKey(product))
                        {
                            productIndexMap[product] = productIndexCounter++;
                        }
                        Pw[workCenterIndexMap[kvp.Key]].Add(productIndexMap[product]);
                    }
                }

                // Transform Qw_String to Qw
                foreach (var kvp in Qw_String)
                {
                    workCenterIndexMap[kvp.Key] = workCenterIndexCounter++;
                    Qw.Add(workCenterIndexMap[kvp.Key], new List<int>());

                    foreach (var product in kvp.Value)
                    {
                        if (!productIndexMap.ContainsKey(product))
                        {
                            productIndexMap[product] = productIndexCounter++;
                        }
                        Qw[workCenterIndexMap[kvp.Key]].Add(productIndexMap[product]);
                    }
                }
                foreach (var kvp in I0W_String)
                {
                    // Assuming work center and product are represented by integer indices
                    int workCenterIndex = workCenterIndexMap[kvp.Key];
                    int productIndex = productIndexMap[kvp.Value];

                    // Assign the product index to the work center index
                    I0W[workCenterIndex] = productIndex;
                }
                #region Merge Pw into Qw
                foreach (var kvp in Pw)
                {
                    int workCenter = kvp.Key;
                    List<int> endItems = kvp.Value;

                    // If the work center already exists in Qw, append the end items to its list
                    if (Qw.ContainsKey(workCenter))
                    {
                        Qw[workCenter].AddRange(endItems);
                    }
                    // If the work center doesn't exist in Qw, add it along with its end items
                    else
                    {
                        Qw[workCenter] = new List<int>(endItems);
                    }
                }
                N = Qw.Count; //Number Of Seuts

                #endregion

                #region Calculate N
                // Create a new dictionary to store the setup counts for each w
                Dictionary<int, int> NDict = new Dictionary<int, int>();

                // Initialize the maximum count to zero
                int maxListCount = 0;

                // Iterate through each list in the Qw dictionary
                foreach (var kvp in Qw)
                {
                    // Get the count of items in the current list
                    int listCount = kvp.Value.Count;

                    // Add the list count to the new dictionary
                    NDict.Add(kvp.Key, listCount);

                    // Update the maximum count if the current list count is greater
                    if (listCount > maxListCount)
                    {
                        maxListCount = listCount;
                    }
                }

                // Update the N variable to hold the maximum list count
                N = maxListCount;


                #endregion
                // Now maxListCount contains the count of the maximum list

                // Transform Dates string list to integer indices
                int dateIndexCounter = 1; // Start index from 1

                foreach (var date in Dates)
                {
                    dateIndexMap[date] = dateIndexCounter++;
                }

                // Transform Dit_String to Dit
                foreach (var kvp in Dit_String)
                {
                    // Get the integer index corresponding to the string date
                    int ItemIndex = productIndexMap[kvp.Key.Item1];
                    int dateIndex = dateIndexMap[kvp.Key.Item2];

                    Dit[(ItemIndex, dateIndex)] = kvp.Value;

                }

                // Transform Ci_String to Ci
                foreach (var kvp in Ci_String)
                {
                    // Assuming component or end-item i is represented by integer index
                    int enditemindex = productIndexMap[kvp.Key];
                    if (!Ci.ContainsKey(enditemindex))
                    {
                        Ci[enditemindex] = new List<int>();
                    }
                    // Convert string indices to integers and add them to Ci
                    foreach (var subcomponentName in kvp.Value)
                    {
                        int subcomponentIndex = productIndexMap[subcomponentName]; // Assuming productIndexMap is a mapping from product names to their integer indices
                        Ci[enditemindex].Add(subcomponentIndex);
                    }
                }

                // Transform Rij_String to Rij
                foreach (var kvp in Rij_String)
                {
                    // Assuming component or end-item i and j are represented by integer indices
                    int endItemIndex = productIndexMap[kvp.Key.Item1];
                    int componentIndex = productIndexMap[kvp.Key.Item2];

                    Rij[(endItemIndex, componentIndex)] = kvp.Value;
                }

                // Transform Awt_String to Awt
                foreach (var kvp in Awt_String)
                {
                    // Assuming work center w is represented by integer index
                    int workCenterIndex = workCenterIndexMap[kvp.Key.Item1];
                    int dateIndex = dateIndexMap[kvp.Key.Item2];

                    Awt[(workCenterIndex, dateIndex)] = kvp.Value;
                    // Assuming period t is represented by integer index
                }

                // Transform Miwt_String to Miwt
                foreach (var kvp in Miwt_String)
                {
                    // Assuming work center w is represented by integer index
                    int productIndex = productIndexMap[kvp.Key.Item1];

                    int workCenterIndex = workCenterIndexMap[kvp.Key.Item2];
                    // Assuming period t is represented by integer index
                    int dateIndex = dateIndexMap[kvp.Key.Item3];
                    if (!Miwt.ContainsKey((productIndex, workCenterIndex, dateIndex)))
                    {
                        Miwt[(productIndex, workCenterIndex, dateIndex)] = kvp.Value;
                    }
                }

                // Transform Sijw_String to Sijw
                foreach (var kvp in Sijw_String)
                {
                    // Assuming fromWorkCenter, fromProduct, and toProduct are represented by integer indices
                    int fromProductIndex = productIndexMap[kvp.Key.Item1];
                    int toProductIndex = productIndexMap[kvp.Key.Item2];

                    int workCenterIndex = workCenterIndexMap[kvp.Key.Item3];

                    Sijw[(fromProductIndex, toProductIndex, workCenterIndex)] = kvp.Value;
                }
                foreach (var kvp in SCijw_String)
                {
                    // Assuming fromWorkCenter, fromProduct, and toProduct are represented by integer indices
                    int fromProductIndex = productIndexMap[kvp.Key.Item1];
                    int toProductIndex = productIndexMap[kvp.Key.Item2];

                    int workCenterIndex = workCenterIndexMap[kvp.Key.Item3];

                    SCijw[(fromProductIndex, toProductIndex, workCenterIndex)] = kvp.Value;
                }

                // Transform Uiw_String to Uiw
                foreach (var kvp in Uiw_String)
                {
                    // Assuming work center w is represented by integer index
                    int productIndex = productIndexMap[kvp.Key.Item1];
                    // Assuming product j is represented by integer index
                    int workCenterIndex = workCenterIndexMap[kvp.Key.Item2];

                    Uiw[(productIndex, workCenterIndex)] = kvp.Value;
                }

                // Transform Hi_String to Hi
                foreach (var kvp in Hi_String)
                {
                    // Assuming component or end-item i is represented by integer index
                    int productIndex = productIndexMap[kvp.Key];
                    Hi[productIndex] = kvp.Value;
                }

                // Transform Gi_String to Gi
                foreach (var kvp in Gi_String)
                {
                    // Assuming component or end-item i is represented by integer index
                    int productIndex = productIndexMap[kvp.Key];
                    Gi[productIndex] = kvp.Value;
                }

                // Transform Ii_String to Ii
                Ii = Ii_String.ToDictionary(
                kvp => productIndexMap[kvp.Key],
                kvp => kvp.Value
            );

                // Transform Imax_min_String to Imax_min
                Imax_min = Imax_min_String.ToDictionary(
                kvp => productIndexMap[kvp.Key],
                kvp => kvp.Value
            );
                #endregion

                #region Print Dictionaries
                Console.WriteLine("Pw:");
                foreach (var kvp in Pw)
                {
                    Console.Write($"Work Center {kvp.Key}: ");
                    Console.WriteLine(string.Join(", ", kvp.Value));
                }

                Console.WriteLine("\nQw:");
                foreach (var kvp in Qw)
                {
                    Console.Write($"Work Center {kvp.Key}: ");
                    Console.WriteLine(string.Join(", ", kvp.Value));
                }

                Console.WriteLine("\nDit:");
                foreach (var kvp in Dit)
                {
                    Console.Write($"Period {kvp.Key}: ");
                    Console.WriteLine(string.Join(", ", kvp.Value));
                }

                Console.WriteLine("\nCi:");
                foreach (var kvp in Ci)
                {
                    Console.Write($"Component {kvp.Key}: ");
                    Console.WriteLine(string.Join(", ", kvp.Value));
                }

                Console.WriteLine("\nRij:");
                foreach (var kvp in Rij)
                {
                    Console.WriteLine($"({kvp.Key.Item1}, {kvp.Key.Item2}): {kvp.Value}");
                }

                Console.WriteLine("\nAwt:");
                foreach (var kvp in Awt)
                {
                    Console.Write($"Work Center {kvp.Key}: ");
                    Console.WriteLine(string.Join(", ", kvp.Value));
                }

                Console.WriteLine("\nMiwt:");
                foreach (var kvp in Miwt)
                {
                    Console.Write($"({kvp.Key.Item1}, {kvp.Key.Item2}): ");
                    Console.WriteLine(string.Join(", ", kvp.Value));
                }

                Console.WriteLine("\nSijw:");
                foreach (var kvp in Sijw)
                {
                    Console.WriteLine($"({kvp.Key.Item1}, {kvp.Key.Item2}, {kvp.Key.Item3}): {kvp.Value}");
                }

                Console.WriteLine("\nUiw:");
                foreach (var kvp in Uiw)
                {
                    Console.WriteLine($"({kvp.Key.Item1}, {kvp.Key.Item2}): {kvp.Value}");
                }

                Console.WriteLine("\nHi:");
                foreach (var kvp in Hi)
                {
                    Console.WriteLine($"Component {kvp.Key}: {kvp.Value}");
                }

                Console.WriteLine("\nGi:");
                foreach (var kvp in Gi)
                {
                    Console.WriteLine($"Component {kvp.Key}: {kvp.Value}");
                }
                #endregion

                #endregion

                #region Optimization Algorithm

                #region Decision Variables 
                // Decision variables
                GRBVar[,,,,] Y = new GRBVar[Q, Q, W, T + 1, N];
                GRBVar[,,,] X = new GRBVar[Q, W, T + 1, N];
                GRBVar[,] SI = new GRBVar[Q, T + 1];
                GRBVar[,] BI = new GRBVar[Q, T + 1];


                // Create decision variables Y
                for (int i = 0; i < Q; i++)
                {
                    for (int j = 0; j < Q; j++)
                    {
                        for (int w = 0; w < W; w++)
                        {
                            if (Qw.TryGetValue(w, out var productList) && productList.Contains(i) && productList.Contains(j))
                            {
                                for (int t = 0; t <= T; t++)
                                {
                                    // Store the count to avoid multiple lookups

                                    for (int n = 0; n < NDict[w]; n++)
                                    {
                                        // Define the variable name
                                        string varName = $"Y{i + 1}_{j + 1}_{w + 1}_{t}_{n + 1}";

                                        // Create the binary variable with a name
                                        Y[i, j, w, t, n] = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, varName);
                                    }
                                }
                                
                            }
                        }
                    }
                }

                // Create decision variables X
                for (int i = 0; i < Q; i++)
                {
                    for (int w = 0; w < W; w++)
                    {
                        if (Qw.TryGetValue(w, out var productList) && productList.Contains(i))
                        {
                            for (int t = 1; t <= T; t++)
                            { 
                            // Check if the key w exists in NDict dictionary

                                for (int n = 0; n < NDict[w]; n++)
                                {
                                    // Define the variable name
                                    string varName = $"X{i + 1}_{w + 1}_{t}_{n + 1}";

                                    // Create the binary variable with a name
                                    X[i, w, t, n] = model.AddVar(0.0, GRB.INFINITY, 0.0, GRB.INTEGER, varName);
                                }
                            }
                        }
                    }
                }


                // Create decision variables SI
                for (int i = 0; i < Q; i++)
                {
                    for (int t = 0; t <= T; t++)
                    {
                        string varName = $"SI{i + 1}_{t}";
                        SI[i, t] = model.AddVar(0.0, GRB.INFINITY, 0.0, GRB.INTEGER, varName);
                    }
                };
                // Create decision variables BI
                for (int i = 0; i < Q; i++)
                {
                    for (int t = 0; t <= T; t++)
                    {
                        string varName = $"BI{i + 1}_{t}";
                        BI[i, t] = model.AddVar(0.0, GRB.INFINITY, 0.0, GRB.INTEGER, varName);
                    }
                };
                #endregion

                #region Objective Function

                GRBLinExpr MinObjective = 0;

                for (int i = 0; i < Q; i++)
                {
                    for (int t = 1; t <= T; t++)
                    {

                        var hi = Hi[i];
                        var gi = Gi[i];
                        MinObjective.AddTerm(hi, SI[i, t]);
                        MinObjective.AddTerm(gi, BI[i, t]);

                    }
                }

                for (int w = 0; w < W; w++)
                {
                    List<int> productsInW = Qw.TryGetValue(w, out var productList) ? productList : new List<int>();

                    for (int t = 1; t <= T; t++)
                    {
                        GRBLinExpr sum = 0;

                        foreach (int j in productsInW)
                        {
                            for (int n = 0; n < NDict[w]; n++)
                            {
                                foreach (int i in productsInW)
                                {
                                    MinObjective.AddTerm(Sijw[(i, j, w)] ,Y[i, j, w, t, n]);
                                }

                            }

                        }
                    }
                }
                model.SetObjective(MinObjective, GRB.MINIMIZE);

                #endregion

                #region Constrains
                #region 2,3,4 Constrains
                // #2. Link inventory and production to the independent demand for the MPS end-items
                for (int t = 1; t <= T; t++)
                {
                    for (int i = 0; i < P; i++)
                    {
                        // Define the left-hand side of the equation
                        GRBLinExpr InvStatus = SI[i, t - 1] - BI[i, t - 1] - SI[i, t] + BI[i, t];

                        // Sum over work centers where i is an end-item
                        GRBLinExpr Sum_X = 0;
                        foreach (int w in Pw.Where(kv => kv.Value.Contains(i)).Select(kv => kv.Key))
                        {
                            for (int n = 0; n < NDict[w]; n++)
                            {
                                Sum_X += X[i, w, t, n];
                            }
                        }

                        // Add to the left-hand side
                        GRBLinExpr FinalSum = InvStatus + Sum_X;

                        // Retrieve the demand for product i at the end of period t from the Dit dictionary
                        double demand = Dit[(i, t)];

                        // Add the constraint
                        model.AddConstr(FinalSum == demand, "Con1_" + i + 1 + "_" + t);
                    }
                }


                // Constraint 3: Inventory balance equation for components
                for (int t = 1; t <= T; t++)
                {
                    for (int i = P; i < Q; i++) // Loop through components
                    {
                        // Define the left-hand side of the equation
                        GRBLinExpr InvStatus = SI[i, t - 1] - BI[i, t - 1] - SI[i, t] + BI[i, t];

                        // Sum over work centers where i is processed
                        GRBLinExpr ProductionSum = 0;
                        foreach (int w in Qw.Where(kv => kv.Value.Contains(i)).Select(kv => kv.Key))
                        {
                            foreach (int n in Enumerable.Range(0, NDict[w]))
                            {
                                ProductionSum += X[i, w, t, n];
                            }
                        }

                        // Retrieve the demand for component i at the end of period t from the Dit dictionary
                        double demand = Dit[(i, t)];

                        // Sum over all components j where i is a subcomponent of j

                        GRBLinExpr SubcomponentDemandSum = 0;
                        foreach (int j in Ci.Where(kv => kv.Value.Contains(i)).Select(kv => kv.Key))
                        {
                            foreach (int w in Qw.Where(kv => kv.Value.Contains(j)).Select(kv => kv.Key))
                            {

                                GRBLinExpr XSum = 0;
                                foreach (int n in Enumerable.Range(0, NDict[w]))
                                {
                                    XSum += X[j, w, t, n];
                                }
                                SubcomponentDemandSum += Rij[(j, i)] * XSum;
                            }
                        }


                        var a = 1;

                        // Add the constraint
                        model.AddConstr(InvStatus + ProductionSum == SubcomponentDemandSum + demand, "Inv_Balance_Component_" + i + "_" + t);
                    }
                }


                //#4. It prohibits backlogs of dependent demand and limits any growth in component backlogs to
                //be solely due to independent demand.

                for (int t = 1; t <= T; t++)
                {
                    for (int i = P; i < Q; i++)
                    {
                        // Define the left-hand side of the equation
                        GRBLinExpr InvStatus = BI[i, t] - BI[i, t - 1];

                        // Retrieve the demand for product i at the end of period t from the Dit dictionary
                        double demand = Dit[(i, t)];

                        // Add the constraint
                        model.AddConstr(InvStatus <= demand, "Const4_" + i + "_" + t);
                    }
                }
                #endregion

                #region Io ,Imax,Imin
                for (int t = 1; t <= T; t++)
                {
                    for (int i = 0; i < Q; i++)
                    {

                        // Retrieve the demand for product i at the end of period t from the Dit dictionary
                        double InvMax = Imax_min[i].Item1;
                        double InvMin = Imax_min[i].Item2;

                        // Add the constraint
                        model.AddConstr(SI[i, t] <= InvMax, "Imax" + i + "_" + t);
                        model.AddConstr(SI[i, t] >= InvMin, "Imin" + i + "_" + t);

                    }
                }

                for (int i = 0; i < Q; i++)
                {

                    // Retrieve the demand for product i at the end of period t from the Dit dictionary
                    double SI0 = Ii[i].Item1;
                    double BI0 = Ii[i].Item2;

                    // Add the constraint
                    model.AddConstr(SI[i, 0] == SI0, "SI0" + i);
                    model.AddConstr(BI[i, 0] == BI0, "BI0" + i);

                }


                #endregion

                #region 5,6,7,8,9,10 Constrains

                //#5.
                for (int w = 0; w < W; w++)
                {
                    List<int> productsInW = Qw.TryGetValue(w, out var productList) ? productList : new List<int>();
                    foreach (int i in productsInW)
                    {
                        foreach (int j in productsInW)
                        {
                            if (i != I0W[w]) // Exclude i != i0w and i != j
                            {

                                model.AddConstr(Y[i, j, w, 1, 0] == 0, $"Constraint_5_w{i + 1}_j{j + 1}_w{w + 1}");


                            }
                        }
                    }
                }


                //#6.
                for (int w = 0; w < W; w++)
                {
                    GRBLinExpr setupSum = 0;
                    List<int> productsInW = Qw.TryGetValue(w, out var productList) ? productList : new List<int>();

                    foreach (int j in productsInW)
                    {

                        setupSum += Y[I0W[w], j, w, 1, 0]; //thelei diorthosh
                    }
                    model.AddConstr(setupSum == 1, $"Constraint_6_w{w + 1}");
                }

                //#7. 
                for (int w = 0; w < W; w++)
                {
                    List<int> productsInW = Qw.TryGetValue(w, out var productList) ? productList : new List<int>();

                    foreach (int i in productsInW)
                    {
                        foreach (int j in productsInW)
                        {
                            for (int t = 1; t <= T; t++)
                            {
                                if (i != j) //  i != j
                                {
                                    model.AddConstr(Y[i, j, w, t, 0] == 0, "Constrain7_" + i + "_" + j + "_" + w + "_" + t);

                                }
                            }
                        }
                    }
                }
                //#8.
                for (int w = 0; w < W; w++)
                {
                    List<int> productsInW = Qw.TryGetValue(w, out var productList) ? productList : new List<int>();

                    for (int t = 1; t <= T; t++)
                    {
                        GRBLinExpr sumY = 0;

                        foreach (int i in productsInW)
                        {
                            sumY += Y[i, i, w, t, 0]; 

                        }
                        model.AddConstr(sumY == 1, "Constrain8_" + w + t);
                    }
                }

               //#9.
                for (int w = 0; w < W; w++)
                {
                    List<int> productsInW = Qw.TryGetValue(w, out var productList) ? productList : new List<int>();

                    for (int t = 1; t <= T; t++)
                    {
                        foreach (int j in productsInW)
                        {
                            for (int n = 0; n < NDict[w] - 1; n++)
                            {
                                GRBLinExpr sumYi = 0;
                                foreach (int i in productsInW)
                                {
                                    sumYi += Y[i, j, w, t, n]; 
                                }
                                GRBLinExpr sumYk = 0;

                                foreach (int k in productsInW)
                                {
                                    sumYk += Y[j, k, w, t, n + 1]; 
                                }
                                model.AddConstr(sumYi == sumYk, "Constrain_9" + w + t);
                            }
                        }
                    }
                }

                //#10
                for (int w = 0; w < W; w++)
                {
                    List<int> productsInW = Qw.TryGetValue(w, out var productList) ? productList : new List<int>();

                    for (int t = 2; t <= T; t++)
                    {
                        foreach (int j in productsInW)
                        {
                            GRBLinExpr sumYi = 0;
                            foreach (int i in productsInW)
                            {
                                sumYi += Y[i, j, w, t - 1, NDict[w] - 1]; 
                            }
                            GRBLinExpr sumYk = 0;

                            foreach (int k in productsInW)
                            {
                                sumYk += Y[j, k, w, t, 0]; 
                            }
                            model.AddConstr(sumYi == sumYk, "Constrain_10" + w + t);
                        }
                    }
                }

                #endregion

                #region 11,12 Constrains
                //#11 
                for (int w = 0; w < W; w++)
                {
                    List<int> productsInW = Qw.TryGetValue(w, out var productList) ? productList : new List<int>();

                    for (int t = 1; t <= T; t++)
                    {
                        for (int n = 0; n < NDict[w]; n++)
                        {
                            foreach (int j in productsInW)
                            {
                                GRBLinExpr sumY = 0;
                                foreach (int i in productsInW)
                                {
                                    sumY += Y[i, j, w, t, n]; 
                                }
                                model.AddConstr(sumY * 10000000 >= X[j, w, t, n], "Constrain11_" + w + t);

                            }

                        }
                    }
                }
                ////#12 
                
                for (int w = 0; w < W; w++)
                {
                    List<int> productsInW = Qw.TryGetValue(w, out var productList) ? productList : new List<int>();

                    for (int t = 1; t <= T; t++)
                    {
                        GRBLinExpr sum = 0;
                        GRBLinExpr sumY = 0;
                        GRBLinExpr sumX = 0;

                        foreach (int j in productsInW)
                        {
                            for (int n = 0; n < NDict[w]; n++)
                            {
  
                                sumX +=  Uiw[(j, w)] * X[j, w, t, n];

                                foreach (int i in productsInW)
                                {
                                    sumY += Y[i, j, w, t, n] * Sijw[(i, j, w)] ;

                                    var uiw = Uiw[(j, w)];
                                    var sijw = Sijw[(i, j, w)];

                                }

                            }

                        }
                        sum = sumX + sumY;
                        model.AddConstr(sum <= Awt[(w, t)], "Constrain12_" + w + t);
                    }
                }
                #endregion
                #endregion

                #region Optimization Solver


                model.Parameters.TimeLimit = 20;

                model.Update();
                model.Optimize();

                #endregion

                #region Populate Records
                // Check if optimization was successful
                if (model.Status == GRB.Status.OPTIMAL || model.Status == GRB.Status.TIME_LIMIT)
                {
                    #region Print Model
                    model.Write("outMRPmst.mst");
                    model.Write("outMRPsol.sol");
                    model.Write("MRPFeasable.lp");
                    model.Write("MRPFeasableMPS.mps");
                    #endregion



                    #region Retrieve Data Original
                    OutputData.Diagram1 = new DiagramsMRPData();
                    OutputData.Diagram1.DataPerDayMRP = new ObservableCollection<DataPerDayMRP>();
                    OutputData.Diagram2 = new DiagramsMRPData();
                    // Retrieve solution for decision variables Y
                    ObservableCollection<DecisionVariableY> YVariables = new ObservableCollection<DecisionVariableY>();

                    for (int i = 0; i < Q; i++)
                    {
                        for (int j = 0; j < Q; j++)
                        {
                            for (int w = 0; w < W; w++)
                            {
                                if (Qw.TryGetValue(w, out var productList) && productList.Contains(i) && productList.Contains(j))
                                {
                                    for (int t = 0; t <= T; t++)
                                    {
                                        for (int n = 0; n < NDict[w]; n++)
                                        {
                                            var value = Y[i, j, w, t, n].X;
                                            DecisionVariableY YVar = new DecisionVariableY();

                                            YVar.ItemCodeFrom = productIndexMap.FirstOrDefault(x => x.Value == i).Key;
                                            YVar.ItemCodeTo = productIndexMap.FirstOrDefault(x => x.Value == j).Key;
                                            YVar.WorkCenter = workCenterIndexMap.FirstOrDefault(x => x.Value == w).Key;
                                            YVar.Date = dateIndexMap.FirstOrDefault(x => x.Value == t).Key;
                                            YVar.Setup = n + 1;
                                            YVar.Value = value;
                                            YVariables.Add(YVar);
                                        }
                                    }
                                }
                               
                            }
                        }
                    }



                    ObservableCollection<DecisionVariableX> XVariables = new ObservableCollection<DecisionVariableX>();

                    for (int i = 0; i < Q; i++)
                    {
                        for (int t = 1; t <= T; t++)
                        {
                            DataPerDayMRP row = new DataPerDayMRP();
                            row.ItemCode = productIndexMap.FirstOrDefault(x => x.Value == i).Key;
                            row.Make = 0;
                            row.Date = dateIndexMap.FirstOrDefault(x => x.Value == t).Key;                          
                            for (int w = 0; w < W; w++)
                            {
                                if (Qw.TryGetValue(w, out var productList) && productList.Contains(i))
                                {
                                    for (int n = 0; n < NDict[w]; n++)
                                    {
                                        DecisionVariableX XVar = new DecisionVariableX();
                                        double value = X[i, w, t, n].X;

                                        XVar.ItemCode = productIndexMap.FirstOrDefault(x => x.Value == i).Key;

                                        XVar.WorkCenter = workCenterIndexMap.FirstOrDefault(x => x.Value == w).Key;
                                        XVar.Date = dateIndexMap.FirstOrDefault(x => x.Value == t).Key;
                                        XVar.Setup = n + 1;
                                        XVar.Value = value;
                                        XVariables.Add(XVar);
                                        row.Make += value;
                                    }
                                }
                                   
                            }
                            OutputData.Diagram1.DataPerDayMRP.Add(row);
                        }
                    }

                    // Retrieve solution for inventory status variables SI and BI
                    ObservableCollection<DecisionVariablesInvStatus> InventoryStatusVariables = new ObservableCollection<DecisionVariablesInvStatus>();
                    for (int i = 0; i < Q; i++)
                    {
                        for (int t = 0; t <= T; t++)
                        {
                            double siValue = SI[i, t].X;
                            double biValue = BI[i, t].X;
                            DecisionVariablesInvStatus InvVar = new DecisionVariablesInvStatus();

                            InvVar.ItemCode = productIndexMap.FirstOrDefault(x => x.Value == i).Key;
                            InvVar.Date = dateIndexMap.FirstOrDefault(x => x.Value == t).Key;
                            InvVar.SIValue = siValue;
                            InvVar.BIValue = biValue;

                            InventoryStatusVariables.Add(InvVar);


                            // Find the corresponding row in DataPerDay based on the item code (i) and date (t)
                            var correspondingRow = OutputData.Diagram1.DataPerDayMRP.FirstOrDefault(row => row.ItemCode == InvVar.ItemCode && row.Date == InvVar.Date);

                            // If a corresponding row is found, update its SIValue and BIValue
                            if (correspondingRow != null)
                            {
                                correspondingRow.Stock = siValue;
                                correspondingRow.Backlog = biValue;
                            };
                        }
                    }

                    foreach (var kvp in Dit_String)
                    {
                        var correspondingRow = OutputData.Diagram1.DataPerDayMRP.FirstOrDefault(row => row.ItemCode == kvp.Key.Item1 && row.Date == kvp.Key.Item2);
                        if (correspondingRow != null)
                        {
                            correspondingRow.Demand = kvp.Value;
                        };



                    }




                    OutputData.XData = XVariables;
                    OutputData.YData = YVariables;
                    OutputData.InvData = InventoryStatusVariables;

                    OutputData.ObjValue = model.ObjVal;


                    #endregion
                }
                else
                {
                    //model.Write("outMRP.mst");
                    //model.Write("outMRP.sol");
                    model.Write("MRPFeasable.lp");
                    model.Write("MRPFeasableMPS.mps");

                    // Model is infeasible

                    // Compute an Irreducible Infeasible Subsystem (IIS) to obtain the infeasibility proof
                    model.ComputeIIS();
                    model.Write("MRPModel.ilp");


                }

                #endregion
                #endregion
                #endregion

                #region Print CSV,TXT

                #region SortDictionaries

                Pw = SortDictionary(Pw);
                Qw = SortDictionary(Qw);
                Dit = SortDictionary(Dit);
                var newDit = Dit.ToDictionary(kvp => (kvp.Key.Item1, kvp.Key.Item2 - 1), kvp => kvp.Value);
                Ci = SortDictionary(Ci);
                Rij = SortDictionary(Rij);
                Awt = SortDictionary(Awt);
                var newAwt = Awt.ToDictionary(kvp => (kvp.Key.Item1, kvp.Key.Item2 - 1), kvp => kvp.Value);

                Miwt = SortDictionary(Miwt);
                Sijw = SortDictionary(Sijw);
                SCijw = SortDictionary(SCijw);

                Uiw = SortDictionary(Uiw);
                Hi = SortDictionary(Hi);
                Gi = SortDictionary(Gi);
                I0W = SortDictionary(I0W);
                Ii = SortDictionary(Ii);
                Imax_min = SortDictionary(Imax_min);
                NDict = SortDictionary(NDict);

                #endregion

                #region CSV Files

                // Create a message box to ask the user if they want to save the results
                MessageBoxResult result = MessageBox.Show("Do you want to save the results to CSV files?", "Save Results", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Define the path to the directory
                    string desktopPath = "C:\\Users\\npoly\\Source\\Repos\\Optimization\\";
                    string directoryPath = Path.Combine(desktopPath, "CSVFiles");

                    // Create the directory if it doesn't exist
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Prompt the user to enter a unique code for the CSV group
                    string csvGroupCode = InputData.MRPCode +"_"+"W"+W;
                    string codeListPath = Path.Combine(directoryPath, "CSVGroupCodes.txt");
                    // Check if the csvGroupCode already exists in the codeListPath
                    if (!File.Exists(codeListPath) || !File.ReadLines(codeListPath).Contains(csvGroupCode))
                    {
                        File.AppendAllText(codeListPath, csvGroupCode + Environment.NewLine);
                    }



                    // Append the group code to each CSV filename
                    WriteToCsv(Path.Combine(directoryPath, $"Pw_{csvGroupCode}.csv"),InputData.Pw);
                    WriteToCsv(Path.Combine(directoryPath, $"Qw_{csvGroupCode}.csv"), InputData.Qw);
                    WriteToCsv(Path.Combine(directoryPath, $"Dit_{csvGroupCode}.csv"), InputData.Dit);
                    WriteToCsv(Path.Combine(directoryPath, $"Ci_{csvGroupCode}.csv"), InputData.Ci);
                    WriteToCsv(Path.Combine(directoryPath, $"Rij_{csvGroupCode}.csv"), InputData.Rij);
                    WriteToCsv(Path.Combine(directoryPath, $"Awt_{csvGroupCode}.csv"), InputData.Awt);
                    WriteToCsv(Path.Combine(directoryPath, $"Miwt_{csvGroupCode}.csv"), InputData.Miwt);
                    WriteToCsv(Path.Combine(directoryPath, $"Sijw_{csvGroupCode}.csv"), InputData.Sijw);
                    WriteToCsv(Path.Combine(directoryPath, $"SCijw_{csvGroupCode}.csv"), InputData.SCijw);
                    WriteToCsv(Path.Combine(directoryPath, $"Uiw_{csvGroupCode}.csv"), InputData.Uiw);
                    WriteToCsv(Path.Combine(directoryPath, $"Hi_{csvGroupCode}.csv"), InputData.Hi);
                    WriteToCsv(Path.Combine(directoryPath, $"Gi_{csvGroupCode}.csv"), InputData.Gi);
                    WriteToCsv(Path.Combine(directoryPath, $"I0W_{csvGroupCode}.csv"), InputData.I0W);
                    WriteToCsv(Path.Combine(directoryPath, $"Ii_{csvGroupCode}.csv"), InputData.Ii);
                    WriteToCsv(Path.Combine(directoryPath, $"Imax_min_{csvGroupCode}.csv"), InputData.Imax_min);

                    // Write variables T, P, Q, W, N to a separate CSV file
                    WriteVariablesToCsv(Path.Combine(directoryPath, $"Variables_{csvGroupCode}.csv"), T, P, Q, W, N);
                }

                #endregion

                #region TxtFiles

                MessageBoxResult result2 = MessageBox.Show("Do you want to save the results to TXT files?", "Save Results", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result2 == MessageBoxResult.Yes)
                {
                    // Define the path to the directory
                    string desktopPath = "C:\\Users\\npoly\\Source\\Repos\\Optimization\\";
                    string directoryPath = Path.Combine(desktopPath, "TxtFiles");

                    // Create the directory if it doesn't exist
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Prompt the user to enter a unique code for the CSV group
                    string txtGroupCode = InputData.MRPCode + "_" + "W" + W;
                    string codeListPath = Path.Combine(directoryPath, "TXTGroupCodes.txt");
                    // Check if the csvGroupCode already exists in the codeListPath
                    if (!File.Exists(codeListPath) || !File.ReadLines(codeListPath).Contains(txtGroupCode))
                    {
                        File.AppendAllText(codeListPath, txtGroupCode + Environment.NewLine);
                    }

                    #region Fill Txt Files

                    //Dit[(i, t)]
                    //List<int> productsInW = Qw.TryGetValue(w, out var productList) ? productList : new List<int>();
                    #endregion
                    // Append the group code to each CSV filename


                    WriteVariablesToTxt(Path.Combine(directoryPath, $"1_tpqw.txt"), T, P, Q, W, N);
                    WriteToTxt(Path.Combine(directoryPath, $"2_pwqwmax.txt"), Pw, Qw, T, P, Q, W, "pwqwnax"); 
                    WriteToTxt(Path.Combine(directoryPath, $"3_qw_data.txt"), Qw, T, P, Q, W, "Qw");
                    WriteToTxt(Path.Combine(directoryPath, $"4_cij.txt"), Ci, T, P, Q, W, "Ci");
                    WriteToTxt(Path.Combine(directoryPath, $"5_rij_data.txt"), Rij, T, P, Q, W, "Rij");
                    WriteToTxt(Path.Combine(directoryPath, $"6_Pw_data.txt"), Pw, T, P, Q, W, "Pw");
                    WriteToTxt(Path.Combine(directoryPath, $"7_Wi_data.txt"), Qw, T, P, Q, W, "Wi");
                    WriteToTxt(Path.Combine(directoryPath, $"8_dit_data.txt"), newDit, T, P, Q, W, "Dit");

                    WriteToTxt(Path.Combine(directoryPath, $"9_hgi_data.txt"), Hi, Gi, T, P, Q, W, "Hi");
                    WriteToTxt(Path.Combine(directoryPath, $"9_hgi_data.txt"), Hi, Gi, T, P, Q, W, "Gi");

                    WriteToTxt(Path.Combine(directoryPath, $"10_sc_data.txt"), SCijw, T, P, Q, W, "SCijw");
                    WriteToTxt(Path.Combine(directoryPath, $"11_Awt_data.txt"), newAwt, T, P, Q, W, "Awt");
                    WriteToTxt(Path.Combine(directoryPath, $"12_s_data.txt"), Sijw, T, P, Q, W, "Sijw");
                    WriteToTxt(Path.Combine(directoryPath, $"13_uiw_data.txt"), Uiw, T, P, Q, W, "Uiw");
                    WriteToTxt(Path.Combine(directoryPath, $"14_Inv_data.txt"), Imax_min, Ii, T, P, Q, W, "Inv_data");
                    WriteToTxt(Path.Combine(directoryPath, $"15_i0_data.txt"), I0W, T, P, Q, W, "I0W");

                    Ii = SortDictionary(Ii);
                    Imax_min = SortDictionary(Imax_min);
                    // Write variables T, P, Q, W, N to a separate CSV file
                }
                #endregion
                #endregion
                return OutputData;

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return OutputData;
            }
        }

        #region Write to Txt
        // Function to save dictionary to a text file

        //Dit,Rij,Awt,Uiw
        void WriteToTxt(string filePath, Dictionary<(int, int), double> dictionary, int T, int P, int Q, int W, string Type)
        {
            using (StreamWriter file = new StreamWriter(filePath))
            {
                if (Type == "Dit")
                {
                    for (int i = 0; i < Q; i++)
                    {
                        for (int t = 0; t < T; t++)
                        {
                            double value;
                            if (!dictionary.TryGetValue((i , t ), out value))
                            {
                                value = 0.0;
                            }
                            file.Write($"{value} ");
                        }
                        file.WriteLine();
                    }
                }
                if (Type == "Rij")
                {
                    for (int i = 0; i < Q; i++)
                    {
                        for (int j = 0; j < Q; j++)
                        {
                            double value;
                            if (!dictionary.TryGetValue((i , j ), out value))
                            {
                                value = 0.0;
                            }
                            file.Write($"{value} ");
                        }
                        file.WriteLine();
                    }
                }
                if (Type == "Awt")
                {
                    for (int w = 0; w < W; w++)
                    {
                        for (int t = 0; t < T; t++)
                        {
                            double value;
                            if (!dictionary.TryGetValue((w , t ), out value))
                            {
                                value = 0.0;
                            }
                            file.Write($"{value} ");
                        }
                        file.WriteLine();
                    }
                }
                if (Type == "Uiw")
                {
                    for (int i = 0; i < Q; i++)
                    {
                        for (int w = 0; w < W; w++)
                        {
                            double value;
                            if (!dictionary.TryGetValue((i , w ), out value))
                            {
                                value = 0.0;
                            }
                            file.Write($"{value} ");
                        }
                        file.WriteLine();
                    }
                }
            }
        }

        //Hi,Gi
        void WriteToTxt(string filePath, Dictionary<int, double> hiDictionary, Dictionary<int, double> giDictionary, int T, int P, int Q, int W, string Type)
        {
            using (StreamWriter file = new StreamWriter(filePath))
            {
                if (Type == "Hi")
                {
                    for (int i = 0; i < Q; i++)
                    {
                        double value;
                        if (!hiDictionary.TryGetValue(i, out value))
                        {
                            value = 0.0;
                        }
                        file.WriteLine($"{value}");
                    }
                }
                else if (Type == "Gi")
                {
                    for (int i = 0; i < Q; i++)
                    {
                        double hiValue;
                        if (!hiDictionary.TryGetValue(i, out hiValue))
                        {
                            hiValue = 0.0;
                        }

                        double giValue;
                        if (!giDictionary.TryGetValue(i, out giValue))
                        {
                            giValue = 0.0;
                        }

                        file.WriteLine($"{hiValue} {giValue}");
                    }
                }
            }
        }

        //I0W
        void WriteToTxt(string filePath, Dictionary<int, int> dictionary, int T, int P, int Q, int W, string Type)
        {
            using (StreamWriter file = new StreamWriter(filePath))
            {
                if (Type == "I0W")
                {
                    for (int w = 0; w < W; w++)
                    {

                            int value;
                            if (!dictionary.TryGetValue((w), out value))
                            {
                                value = 0;
                            }
                            file.Write($"{value} ");
                        
                        file.WriteLine();
                    }
                }

            }
        }

        //Ii,Imax_min,NDict
        void WriteToTxt(string filePath, Dictionary<int, (double, double)> ImaxMin, Dictionary<int, (double, double)> Ii, int T, int P, int Q, int W, string Type)
        {
            using (StreamWriter file = new StreamWriter(filePath))
            {
                for (int i = 0; i < Q; i++)
                {
                    (double, double) ImaxMin_Values; //Max,Min
                    (double, double) Ii_Values;      //Stock,Backlog

                    if (!ImaxMin.TryGetValue(i, out ImaxMin_Values))
                    {
                        ImaxMin_Values = (0, 0);
                    }

                    if (!Ii.TryGetValue(i, out Ii_Values))
                    {
                        Ii_Values = (0, 0);
                    }



                    var StartingSTOCK = Ii_Values.Item1;
                    var StartingBACKLOG = Ii_Values.Item2;

                    if (StartingSTOCK > 0)
                    {
                        file.Write($"{StartingSTOCK} ");
                    }
                    else if (StartingBACKLOG > 0 )
                    {
                        file.Write($"{-StartingBACKLOG} ");
                    }
                    else
                    {
                        file.Write($"{StartingSTOCK} ");
                    }

                    file.Write($"{ImaxMin_Values.Item2} "); // Min
                    file.Write($"{ImaxMin_Values.Item1} "); // Max

                    file.WriteLine();
                }
            }
        }


        //Sijw,Sc
        void WriteToTxt(string filePath, Dictionary<(int, int, int), double> dictionary, int T, int P, int Q, int W, string Type)
        {
            using (StreamWriter file = new StreamWriter(filePath))
            {
                if (Type == "Sijw")
                {
                    for(int w = 0; w < W; w++)
                    {
                        for (int i = 0; i < Q; i++)
                        {
                            for (int j = 0; j < Q; j++)
                            {
                                double value;
                                if (!dictionary.TryGetValue((i , j ,w), out value))
                                {
                                    value = 0.0;
                                }
                                file.Write($"{value} ");
                            }
                            file.WriteLine();
                        }

                    }

                }
                if (Type == "SCijw")
                {
                    for (int w = 0; w < W; w++)
                    {
                        for (int i = 0; i < Q; i++)
                        {
                            for (int j = 0; j < Q; j++)
                            {
                                double value;
                                if (!dictionary.TryGetValue((i , j , w ), out value))
                                {
                                    value = 0;
                                }

                                file.Write($"{value} ");
                            }
                            file.WriteLine();
                        }

                    }

                }

            }
        }

        //Pw,Qw,Ci,Wi
        void WriteToTxt(string filePath, Dictionary<int, List<int>> dictionary, int T, int P, int Q, int W, string Type)
        {
            using (StreamWriter file = new StreamWriter(filePath))
            {
                if (Type == "Qw")
                {
                    for (int w = 0; w < W; w++)
                    {
                        List<int> productsInW = dictionary.TryGetValue(w, out var productList) ? productList : new List<int>();

                        for (int i = 0; i < Q; i++)
                        {
                            int value = 1;
                            if (!productsInW.Contains(i))
                            {
                                value = 0;
                            }
                            file.Write($"{value} ");
                        }
                        file.WriteLine();
                    }
                }
                if (Type == "Wi")
                {
                    for (int i = 0; i < Q; i++)
                    {
                        
                        for (int w = 0; w < W; w++)
                        {
                            int value = 1;
                            List<int> productsInW = dictionary.TryGetValue(w, out var productList) ? productList : new List<int>();
                            if (!productsInW.Contains(i))
                            {
                                value = 0;
                            }
                            file.Write($"{value} ");
                        }
                        file.WriteLine();
                    }
                }
                if (Type == "Pw")
                {
                    for (int w = 0; w < W; w++)
                    {
                        List<int> productsInW = dictionary.TryGetValue(w, out var productList) ? productList : new List<int>();

                        for (int i = 0; i < P; i++)
                        {
                            int value = 1;
                            if (!productsInW.Contains(i))
                            {
                                value = 0;
                            }
                            file.Write($"{value} ");
                        }
                        file.WriteLine();
                    }
                }
                if (Type == "Ci")
                {
                    for (int i = 0; i < Q; i++)
                    {
                        List<int> bomCi = dictionary.TryGetValue(i, out var productList) ? productList : new List<int>();

                        for (int j = 0; j < Q; j++)
                        {
                            int value = 1;
                            if (!bomCi.Contains(j))
                            {
                                value = 0;
                            }
                            file.Write($"{value} ");
                        }
                        file.WriteLine();
                    }
                }
            }
        }
        void WriteToTxt(string filePath, Dictionary<int, List<int>> Pw, Dictionary<int, List<int>> Qw, int T, int P, int Q, int W, string Type)
        {
            using (StreamWriter file = new StreamWriter(filePath))
            {

                    for (int w = 0; w < W; w++)
                    {
                      

                        List<int> QproductsInW = Qw.TryGetValue(w, out var productList) ? productList : new List<int>();


                        var EndItems = QproductsInW.Where(d => d < P).ToList();
                        var Componets = QproductsInW.ToList();

                        file.Write($"{EndItems.Count} ");
                        file.Write($"{Componets.Count} ");

                        file.WriteLine();
                    }

            }
        }


        // Function to save variables to a text file
        void WriteVariablesToTxt(string filePath, int T, int P, int Q, int W, int N)
        {
            using (StreamWriter file = new StreamWriter(filePath))
            {
                file.WriteLine($"{T}");
                file.WriteLine($"{P}");
                file.WriteLine($"{Q-P}");
                file.WriteLine($"{W}");
            }
        }
        #endregion
        #region Write To CSV
        static Dictionary<TKey, TValue> SortDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
            return dict.OrderBy(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        static void WriteToCsv(string fileName, Dictionary<string, List<string>> dictionary)
        {
            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLine("Key,Values");
                foreach (var kvp in dictionary)
                {
                    writer.WriteLine($"{kvp.Key},{string.Join(";", kvp.Value)}");
                }
            }
        }

        static void WriteToCsv(string fileName, Dictionary<(string, string), double> dictionary)
        {
            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLine("Key1,Key2,Value");
                foreach (var kvp in dictionary)
                {
                    writer.WriteLine($"{kvp.Key.Item1},{kvp.Key.Item2},{kvp.Value}");
                }
            }
        }

        static void WriteToCsv(string fileName, Dictionary<(string, string, string), double> dictionary)
        {
            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLine("Key1,Key2,Key3,Value");
                foreach (var kvp in dictionary)
                {
                    writer.WriteLine($"{kvp.Key.Item1},{kvp.Key.Item2},{kvp.Key.Item3},{kvp.Value}");
                }
            }
        }

        static void WriteToCsv(string fileName, Dictionary<string, double> dictionary)
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

        static void WriteToCsv(string fileName, Dictionary<string, string> dictionary)
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

        static void WriteVariablesToCsv(string fileName, int T, int P, int Q, int W, int N)
        {
            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLine("Variable,Value");
                writer.WriteLine($"T,{T}");
                writer.WriteLine($"P,{P}");
                writer.WriteLine($"Q,{Q}");
                writer.WriteLine($"W,{W}");
                writer.WriteLine($"N,{N}");
            }
        }

        #endregion
        public ObservableCollection<MrpInputData> GetMrpInputData(string FinalItemCode)
        {
            ObservableCollection<MrpInputData> DataList = new ObservableCollection<MrpInputData>();

            IDictionary<string, float> MRPDictionary = new Dictionary<string, float>();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.Parameters.AddWithValue("@ItemCode", FinalItemCode);

                command.CommandText = string.Format(@"Select ItemId from Rmaster 
Where ItemCode = @ItemCode");

                //EXECUTE SCALAR

                var ItemId = command.ExecuteScalar();

                command.Parameters.AddWithValue("@ItemId", ItemId);

                command.CommandText = string.Format(@"
select BOM.ItemId,BOM.ComponentId,Rmaster.ItemCode,BOM.Percentage from BOM 
Inner Join Rmaster on Rmaster.ItemId= BOM.ComponentId
Where BOM.ItemId = @ItemId");

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var ItemCode = reader["ItemCode"].ToString();
                        var Percentage = reader["Percentage"].ToString();

                        MRPDictionary.Add(ItemCode, float.Parse(Percentage));
                    }
                }

                foreach (var i in MRPDictionary)
                {
                    MrpInputData data = new MrpInputData();


                    data.EndItemCode = FinalItemCode;
                    data.ItemCode = i.Key;
                    data.Percentage = i.Value;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@ItemCode", i.Key);

                    command.CommandText = string.Format(@"Select ItemId from Rmaster 
                     Where ItemCode = @ItemCode");
                    command.Parameters.AddWithValue("@ItemId", command.ExecuteScalar());

                    command.CommandText = string.Format(@"
                    select Quantity from stock 
                    Where ItemId= @ItemId");

                    var CurrentStock = command.ExecuteScalar().ToString();
                    data.CurrentStock = float.Parse(CurrentStock);
                    data.DeliveredDate = DateTime.Now;
                    data.DeliveredQuantity = 0;

                    command.CommandText = string.Format(@"Select LotPolicyCode,LeadTime from LotPolicy 
                     Where ItemId = @ItemId and MainPolicy = 1");
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data.LotPolicy = reader["LotPolicyCode"].ToString();
                            data.SafetyLeadTime = int.Parse(reader["LeadTime"].ToString());


                        }
                    }
                    DataList.Add(data);


                }

                connection.Close();

            }
            return DataList;
        }

        public ObservableCollection<MrpResultData> GetMrpResultsData()
        {
            ObservableCollection<MrpResultData> dataList = new ObservableCollection<MrpResultData>();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"select Rmaster.ItemCode,Rmaster.Assembly,MrpResults.* from MrpResults
Inner Join Rmaster on Rmaster.ItemId = MrpResults.ItemId";

                using (var reader = command.ExecuteReader())
                {
                    int totalColumns = reader.FieldCount;

                    while (reader.Read())
                    {
                        MrpResultData data = new MrpResultData();
                        data.Item = new ItemData();
                        data.Id = int.Parse(reader["Id"].ToString());
                        data.RowDescr = reader["Row Descr"].ToString();
                        data.ItemId = int.Parse(reader["ItemId"].ToString());
                        data.Item.ItemCode = reader["ItemCode"].ToString();
                        data.Item.Assembly = (BasicEnums.Assembly)Enum.Parse(typeof(BasicEnums.Assembly), reader["Assembly"].ToString());
                        data.Quantities = new List<float>();


                        for (int i = 4; i < totalColumns; i++) // loop through the remaining columns
                        {
                            data.Quantities.Add(float.Parse(reader[i].ToString()));
                        }

                        dataList.Add(data);
                    }
                }

                connection.Close();
            }

            return dataList;
        }

        #endregion

        #region GaantDiagram

        public ObservableCollection<ProjectData> GetProjects()
        {
            var projects = new ObservableCollection<ProjectData>();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.CommandText = "SELECT Id, ProjectCode, ProjectName, Description, DurationDays, StartDate, EndDate FROM Projects";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var project = new ProjectData
                        {
                            Id = (int)reader["Id"],
                            ProjectCode = (string)reader["ProjectCode"],
                            ProjectName = (string)reader["ProjectName"],
                            Description = reader["Description"] is DBNull ? null : (string)reader["Description"],
                            DurationDays = reader["DurationDays"] is DBNull ? 0 : (int)reader["DurationDays"],
                            StartDate = reader["StartDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["StartDate"],
                            EndDate = reader["EndDate"] is DBNull ? DateTime.MaxValue : (DateTime)reader["EndDate"],
                            Processes = new List<ProcessData>()
                        };

                        projects.Add(project);
                    }
                }

                foreach (var project in projects)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@ProjectId", project.Id);
                    command.CommandText = @"SELECT Id, ProcessCode, ProcessName, Description, DurationDays, StartDate, EndDate FROM Processes WHERE ProjectId = @ProjectId";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var process = new ProcessData
                            {
                                Id = (int)reader["Id"],
                                ProcessCode = (string)reader["ProcessCode"],
                                ProcessName = (string)reader["ProcessName"],
                                Description = reader["Description"] is DBNull ? null : (string)reader["Description"],
                                DurationDays = reader["DurationDays"] is DBNull ? 0 : (int)reader["DurationDays"],
                                StartDate = reader["StartDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["StartDate"],
                                EndDate = reader["EndDate"] is DBNull ? DateTime.MaxValue : (DateTime)reader["EndDate"],
                                Tasks = new List<TaskData>()
                            };

                            project.Processes.Add(process);
                        }
                    }

                    foreach (var process in project.Processes)
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@ProcessId", process.Id);
                        command.CommandText = @"SELECT Id, TaskCode, TaskName, Description, DurationDays, StartDate, EndDate FROM Tasks WHERE ProcessId = @ProcessId";

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var task = new TaskData
                                {
                                    Id = (int)reader["Id"],
                                    TaskCode = (string)reader["TaskCode"],
                                    TaskName = (string)reader["TaskName"],
                                    Description = reader["Description"] is DBNull ? null : (string)reader["Description"],
                                    DurationDays = (int)reader["DurationDays"],
                                    StartDate = reader["StartDate"] is DBNull ? DateTime.MinValue : (DateTime)reader["StartDate"],
                                    EndDate = reader["EndDate"] is DBNull ? DateTime.MaxValue : (DateTime)reader["EndDate"]
                                };

                                process.Tasks.Add(task);
                            }
                        }
                    }
                }

                connection.Close();
            }

            return projects;
        }

        #endregion

        #endregion

        #region Data Analytics


        #endregion

        #region Supply Chain Management
        #endregion

        #region Gurobi
        #region Inventory Control
        public BasicEOQData CalculateInvControl2(InvControlData Filter)
        {
            BasicEOQData Data = new BasicEOQData();
            GRBEnv env = new GRBEnv("logfile.log");
            GRBModel model = new GRBModel(env);

            try
            {
                // Define the parameters
                //double K = Filter.EOQData.K; // Fixed order cost (€ per order)
                //double lambda = Filter.EOQData.Demand; // Demand rate (boxes of bottles per month)
                //double c = Filter.EOQData.C; // Cost per unit (€ per bottle)
                //double h = Filter.EOQData.H; // Inventory holding cost (monthly interest rate)

                double K = 144; // Fixed order cost (€ per order)
                double lambda = 72; // Demand rate (boxes of bottles per month)
                double c = 28.8; // Cost per unit (€ per bottle)
                double h = 0.36; // Inventory holding cost (monthly interest rate)
                // Add variables
                GRBVar Q = model.AddVar(0, GRB.INFINITY, 0, GRB.CONTINUOUS, "Q");
                GRBVar Z = model.AddVar(0, GRB.INFINITY, 0, GRB.CONTINUOUS, "Z");  // 1/Q

                // Set the objective function 
                GRBLinExpr obj = K * lambda * Z + c * lambda + h * Q * 0.5;
                model.SetObjective(obj, GRB.MINIMIZE);

                // Add constraints
                GRBQuadExpr expr = new GRBQuadExpr();
                expr.AddTerm(1.0, Z, Q);
                model.AddQConstr(expr, GRB.EQUAL, 1.0, "C0");

                // Set NonConvex parameter
                model.Parameters.NonConvex = 2;

                // Optimize model
                model.Optimize();

                // Check if optimization was successful
                if (model.Status == GRB.Status.OPTIMAL)
                {
                    // Retrieve the optimal solution
                    double optimalQ = Q.Get(GRB.DoubleAttr.X);
                    double optimalZ = Z.Get(GRB.DoubleAttr.X);

                    Data.Q = (double)optimalQ;
                    Data.T = (double)(optimalQ / lambda);
                    Data.N = 1 / Data.T;

                    // Print the optimal solution
                    Console.WriteLine("Optimal quantity of beer to order: " + optimalQ);
                    Console.WriteLine("1/Q: " + optimalZ);

                    // Get the optimal objective function value from Gurobi
                    double optimalObjective = model.ObjVal;
                    Console.WriteLine("Optimal objective function value (from Gurobi): " + optimalObjective);
                    Data.ObjFunc = optimalObjective;
                }
                else
                {
                    Console.WriteLine("No optimal solution found.");
                }

                // Dispose of the model and environment
                model.Dispose();
                env.Dispose();
                // Debug prints
                Console.WriteLine("Value of K: " + K);
                Console.WriteLine("Value of lambda: " + lambda);
                Console.WriteLine("Value of c: " + c);
                Console.WriteLine("Value of h: " + h);
                Console.WriteLine("Value of Q (Data.Q): " + Data.Q);


                // Print the value of Data.ObjFunc

                var TestObjFunc = K * lambda * (1 / Data.Q) + c * lambda + h * Data.Q * 0.5;
                Console.WriteLine("TestObjFunc : " + TestObjFunc);

                return Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return Data;
            }
        }

        public InvControlData CalculateInvControlConstant(InvControlData Filter)
        {
            GRBEnv env = new GRBEnv("logfile.log");
            GRBModel model = new GRBModel(env);
            InvControlData Data = new InvControlData();

            try
            {
                // Define the parameters
                //double K = Filter.EOQData.K; // Fixed order cost (€ per order)
                //double lambda = Filter.EOQData.Demand; // Demand rate (boxes of bottles per month)
                //double c = Filter.EOQData.C; // Cost per unit (€ per bottle)
                //double h = Filter.EOQData.H; // Inventory holding cost (monthly interest rate)




                if (Filter.DemandType == BasicEnums.DemandType.Constant_Demand)
                {
                    if (Filter.ConstantDemandType == BasicEnums.ConstantDemandType.Basic_EOQ)
                    {

                        double K = 144; // Fixed order cost (€ per order)
                        double lambda = 72; // Demand rate (boxes of bottles per month)
                        double c = 28.8; // Cost per unit (€ per bottle)
                        double h = 0.36; // Inventory holding cost (monthly interest rate)


                        GRBVar Q = model.AddVar(0, GRB.INFINITY, 0, GRB.CONTINUOUS, "Q");
                        GRBVar Z = model.AddVar(0, GRB.INFINITY, 0, GRB.CONTINUOUS, "Z");  // 1/Q

                        GRBLinExpr obj = K * lambda * Z + c * lambda + h * Q * 0.5;

                        model.SetObjective(obj, GRB.MINIMIZE);
                        // Set the objective function 

                        #region Constrains 
                        // Add constraints

                        GRBQuadExpr expr = new GRBQuadExpr();
                        expr.AddTerm(1.0, Z, Q);
                        model.AddQConstr(expr, GRB.EQUAL, 1.0, "C0");

                        if (Filter.EOQData.QminmaxConstr == true)
                        {
                            // Add the constraints
                            model.AddConstr(Q, GRB.GREATER_EQUAL, (double)Filter.EOQData.Qmin, "QminConstraint");
                            model.AddConstr(Q, GRB.LESS_EQUAL, (double)Filter.EOQData.Qmax, "QmaxConstraint");
                        }

                        #endregion
                        // Set NonConvex parameter
                        model.Parameters.NonConvex = 2;

                        // Optimize model
                        model.Optimize();

                        // Check if optimization was successful
                        if (model.Status == GRB.Status.OPTIMAL)
                        {
                            // Retrieve the optimal solution
                            double optimalQ = Q.Get(GRB.DoubleAttr.X);
                            double optimalZ = Z.Get(GRB.DoubleAttr.X);

                            Data.EOQData.Q = (double)optimalQ;
                            Data.EOQData.T = (double)(optimalQ / lambda);
                            Data.EOQData.N = 1 / Data.EOQData.T;

                            // Get the optimal objective function value from Gurobi
                            double optimalObjective = model.ObjVal;
                            // Print the optimal solution
                            Console.WriteLine("Optimal quantity of beer to order: " + optimalQ);
                            Console.WriteLine("1/Q: " + optimalZ);


                            Console.WriteLine("Optimal objective function value (from Gurobi): " + optimalObjective);
                            Data.EOQData.ObjFunc = optimalObjective;

                        }
                        else
                        {
                            Console.WriteLine("No optimal solution found.");

                        }
                        var TestObjFunc = K * lambda * (1 / Data.EOQData.Q) + c * lambda + h * Data.EOQData.Q * 0.5;
                        Console.WriteLine("TestObjFunc : " + TestObjFunc);
                    }
                    else if (Filter.ConstantDemandType == BasicEnums.ConstantDemandType.Pending_Orders)
                    {
                        PendingOrdersData PordersData = new PendingOrdersData();

                        double K = 144; // Fixed order cost (€ per order)
                        double lambda = 72; // Demand rate (boxes of bottles per month)
                        double c = 28.8; // Cost per unit (€ per bottle)
                        double h = 0.36; // Inventory holding cost (monthly interest rate)
                        double b = 1.08; // Inventory holding cost (monthly interest rate)
                        double F = b / (h + b);


                        GRBVar Q = model.AddVar(0, GRB.INFINITY, 0, GRB.CONTINUOUS, "Q");
                        GRBVar Z = model.AddVar(0, GRB.INFINITY, 0, GRB.CONTINUOUS, "Z");  // 1/Q

                        // Add a constant term
                        GRBVar constant = model.AddVar(1, 1, 0, GRB.CONTINUOUS, "constant");
                        // Create the objective function

                        GRBQuadExpr obj = new GRBQuadExpr();
                        obj.AddTerm(K * lambda, Z); // K * lambda * Z
                        obj.AddConstant(c * lambda); // c * lambda
                        obj.AddTerm(h * F * F * 0.5, Q); // h * F^2 * Q * 0.5
                        obj.AddTerm(b * Math.Pow((1 - F), 2) * 0.5, Q); // b * (1-F)^2 * Q * 0.5

                        model.SetObjective(obj, GRB.MINIMIZE);



                        #region Constraints
                        // Create the constraints
                        GRBQuadExpr expr1 = new GRBQuadExpr();
                        expr1.AddTerm(1.0, Z, Q);
                        model.AddQConstr(expr1, GRB.EQUAL, 1.0, "C0");
                        #endregion



                        // Set NonConvex parameter
                        model.Parameters.NonConvex = 2;

                        // Optimize model
                        model.Optimize();

                        // Check if optimization was successful
                        if (model.Status == GRB.Status.OPTIMAL)
                        {
                            // Retrieve the optimal solution
                            double optimalQ = Q.Get(GRB.DoubleAttr.X);
                            double optimalZ = Z.Get(GRB.DoubleAttr.X);
                            double optimalF = F; // Retrieve the optimal value of F

                            PordersData.Q = optimalQ;
                            PordersData.T = optimalQ / lambda;
                            PordersData.N = 1 / PordersData.T;
                            PordersData.F = optimalF;

                            // Get the optimal objective function value from Gurobi
                            double optimalObjective = model.ObjVal;
                            // Print the optimal solution
                            Console.WriteLine("Optimal quantity of beer to order: " + optimalQ);
                            Console.WriteLine("1/Q: " + optimalZ);
                            Console.WriteLine("Optimal F: " + optimalF); // Print the optimal value of F
                            Console.WriteLine("Optimal objective function value (from Gurobi): " + optimalObjective);
                            PordersData.ObjFunc = optimalObjective;
                            Data.PendingOrdersData = PordersData;
                        }
                        else
                        {
                            Console.WriteLine("No optimal solution found.");
                            return Data;

                        }

                    }
                    else if (Filter.ConstantDemandType == BasicEnums.ConstantDemandType.Lost_Sales)
                    {
                        PendingOrdersData PordersData = new PendingOrdersData();

                        double K = 144; // Fixed order cost (€ per order)
                        double lambda = 72; // Demand rate (boxes of bottles per month)
                        double r = 72; // Έσοδα (τιμή) ανά μονάδα πώλησης: 𝑟 (€ ανά μονάδα)

                        double c = 28.8; // Cost per unit (€ per bottle)
                        double h = 0.36; // Inventory holding cost (monthly interest rate)
                        double bl = r - c; // Κόστος ανά μονάδα έλλειψης: 𝑏𝐿 (€ κέρδος ανά μονάδα χαμένης πώλησης)
                        double Q = Math.Sqrt((2 * K * lambda) / h);


                        GRBVar F = model.AddVar(0, GRB.INFINITY, 0, GRB.CONTINUOUS, "F");
                        GRBVar G = model.AddVar(0, GRB.INFINITY, 0, GRB.CONTINUOUS, "G");  // 1/Q

                        // Add a constant term
                        GRBVar constant = model.AddVar(1, 1, 0, GRB.CONTINUOUS, "constant");
                        // Create the objective function


                        GRBQuadExpr obj = new GRBQuadExpr();

                        obj.AddTerm(bl * lambda, F); // Adding term: bl * F * lambda
                        obj.AddTerm(-K * lambda / Q, F); // Adding term: - K * F * lambda / Q
                        obj.AddTerm(-h * Q * 0.5, F, F); // Adding term: - h * F * F * Q * 0.5

                        model.SetObjective(obj, GRB.MINIMIZE);



                        #region Constraints
                        // Create the constraints
                        GRBQuadExpr expr1 = new GRBQuadExpr();
                        model.AddConstr(G, GRB.EQUAL, 1 - F, "G_Constraint");
                        #endregion



                        // Set NonConvex parameter
                        model.Parameters.NonConvex = 2;

                        // Optimize model
                        model.Optimize();

                        // Check if optimization was successful
                        if (model.Status == GRB.Status.OPTIMAL)
                        {
                            // Retrieve the optimal solution
                            double optimalQ = Q;
                            double optimalF = F.Get(GRB.DoubleAttr.X);
                            double optimalG = G.Get(GRB.DoubleAttr.X);

                            PordersData.Q = optimalQ;
                            PordersData.T = optimalQ / lambda;
                            PordersData.N = 1 / PordersData.T;
                            PordersData.F = optimalF;

                            // Get the optimal objective function value from Gurobi
                            double optimalObjective = model.ObjVal;
                            // Print the optimal solution
                            Console.WriteLine("Optimal quantity of beer to order: " + optimalQ);
                            Console.WriteLine("Optimal F: " + optimalF); // Print the optimal value of F
                            Console.WriteLine("Optimal objective function value (from Gurobi): " + optimalObjective);
                            PordersData.ObjFunc = optimalObjective;
                            Data.PendingOrdersData = PordersData;
                        }
                        else
                        {
                            Console.WriteLine("No optimal solution found.");
                            return Data;

                        }

                    }
                    else if (Filter.ConstantDemandType == BasicEnums.ConstantDemandType.Discount_Large_Orders)
                    {
                        PendingOrdersData PordersData = new PendingOrdersData();

                        double K = 144; // Fixed order cost (€ per order)
                        double lambda = 72; // Demand rate (boxes of bottles per month)
                        double c = 28.8; // Cost per unit (€ per bottle)
                        double h = 0.36; // Inventory holding cost (monthly interest rate)
                        double b = 1.08; // Inventory holding cost (monthly interest rate)
                        double F = b / (h + b);


                        GRBVar Q = model.AddVar(0, GRB.INFINITY, 0, GRB.CONTINUOUS, "Q");
                        GRBVar Z = model.AddVar(0, GRB.INFINITY, 0, GRB.CONTINUOUS, "Z");  // 1/Q

                        // Add a constant term
                        GRBVar constant = model.AddVar(1, 1, 0, GRB.CONTINUOUS, "constant");
                        // Create the objective function

                        GRBQuadExpr obj = new GRBQuadExpr();
                        obj.AddTerm(K * lambda, Z); // K * lambda * Z
                        obj.AddConstant(c * lambda); // c * lambda
                        obj.AddTerm(h * F * F * 0.5, Q); // h * F^2 * Q * 0.5
                        obj.AddTerm(b * Math.Pow((1 - F), 2) * 0.5, Q); // b * (1-F)^2 * Q * 0.5

                        model.SetObjective(obj, GRB.MINIMIZE);



                        #region Constraints
                        // Create the constraints
                        GRBQuadExpr expr1 = new GRBQuadExpr();
                        expr1.AddTerm(1.0, Z, Q);
                        model.AddQConstr(expr1, GRB.EQUAL, 1.0, "C0");
                        #endregion



                        // Set NonConvex parameter
                        model.Parameters.NonConvex = 2;

                        // Optimize model
                        model.Optimize();

                        // Check if optimization was successful
                        if (model.Status == GRB.Status.OPTIMAL)
                        {
                            // Retrieve the optimal solution
                            double optimalQ = Q.Get(GRB.DoubleAttr.X);
                            double optimalZ = Z.Get(GRB.DoubleAttr.X);
                            double optimalF = F; // Retrieve the optimal value of F

                            PordersData.Q = optimalQ;
                            PordersData.T = optimalQ / lambda;
                            PordersData.N = 1 / PordersData.T;
                            PordersData.F = optimalF;

                            // Get the optimal objective function value from Gurobi
                            double optimalObjective = model.ObjVal;
                            // Print the optimal solution
                            Console.WriteLine("Optimal quantity of beer to order: " + optimalQ);
                            Console.WriteLine("1/Q: " + optimalZ);
                            Console.WriteLine("Optimal F: " + optimalF); // Print the optimal value of F
                            Console.WriteLine("Optimal objective function value (from Gurobi): " + optimalObjective);
                            PordersData.ObjFunc = optimalObjective;
                            Data.PendingOrdersData = PordersData;
                        }
                        else
                        {
                            Console.WriteLine("No optimal solution found.");
                            return Data;

                        }

                    }

                }
                model.Dispose();
                env.Dispose();

                return Data;


            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return Data;

            }
        }

        public InvControlData CalculateInvControlTimeVar(InvControlData Filter)
        {
            GRBEnv env = new GRBEnv("logfile.log");

            GRBModel model = new GRBModel(env);
            InvControlData Data = new InvControlData();
            Data.TimeVarInfiniteData = new ObservableCollection<TimeVaryingInvData>();
            double bigM = 10000;

            try
            {
                // Define the parameters
                if (Filter.DemandType == BasicEnums.DemandType.Time_Varying_Demand)
                {
                    // Assuming that Filter carries all necessary information for optimization.
                    // Adjust the code below if it's stored differently.
                    int numDays = Filter.TimeVarInfiniteData.Count;
                    double[] demands = Filter.TimeVarInfiniteData.Select(d => d.Demand).ToArray();
                    double[] holdingCosts = Filter.TimeVarInfiniteData.Select(d => d.HoldingCost).ToArray();
                    double[] setupCosts = Filter.TimeVarInfiniteData.Select(d => d.SetupCost).ToArray();

                    if (Filter.TimeVaryingDemandType == BasicEnums.TimeVaryingDemandType.Infinite_Capacity)
                    {
                        // Decision variables
                        GRBVar[] quantityOrdered = model.AddVars(numDays, GRB.CONTINUOUS);
                        GRBVar[] inventoryLevel = model.AddVars(numDays, GRB.CONTINUOUS);
                        GRBVar[] isOrdering = model.AddVars(numDays, GRB.BINARY);

                        // Objective function
                        GRBLinExpr objective = 0;
                        for (int t = 0; t < numDays; t++)
                        {
                            objective.AddTerm(holdingCosts[t], inventoryLevel[t]); // holding cost
                            objective.AddTerm(setupCosts[t], isOrdering[t]); // setup cost
                        }
                        model.SetObjective(objective, GRB.MINIMIZE);

                        // Constraints
                        for (int t = 0; t < numDays; t++)
                        {
                            // Inventory balance constraint
                            if (t == 0)
                                model.AddConstr(inventoryLevel[t] == quantityOrdered[t] - demands[t], $"InventoryBalance_{t}");
                            else
                                model.AddConstr(inventoryLevel[t] == inventoryLevel[t - 1] + quantityOrdered[t] - demands[t], $"InventoryBalance_{t}");

                            // Big-M constraint for ordering decision
                            model.AddConstr(quantityOrdered[t] <= isOrdering[t] * bigM, $"BigM_{t}");

                            // Non-negativity constraints
                            model.AddConstr(inventoryLevel[t] >= 0, $"InventoryNonNegative_{t}");
                            model.AddConstr(quantityOrdered[t] >= 0, $"QuantityNonNegative_{t}");
                        }

                        // Solve the model
                        model.Optimize();

                        // Populate Records with the optimal solution
                        if (model.Status == GRB.Status.OPTIMAL)
                        {

                            // Populate Records with the optimal solution
                            for (int t = 0; t < numDays; t++)
                            {
                                TimeVaryingInvData singleDataRecord = new TimeVaryingInvData();

                                singleDataRecord.Day = t + 1;
                                singleDataRecord.Demand = demands[t];

                                double quantityOrderedValue = quantityOrdered[t].Get(GRB.DoubleAttr.X);
                                singleDataRecord.QuantityOrdered = quantityOrderedValue;

                                double inventoryLevelValue = inventoryLevel[t].Get(GRB.DoubleAttr.X);
                                singleDataRecord.InventoryLevel = inventoryLevelValue;

                                double isOrderingValue = isOrdering[t].Get(GRB.DoubleAttr.X);
                                singleDataRecord.IsOrdering = isOrderingValue;

                                singleDataRecord.SetupCost = setupCosts[t] * isOrderingValue;
                                singleDataRecord.HoldingCost = holdingCosts[t] * inventoryLevelValue;
                                singleDataRecord.TotalCost = singleDataRecord.HoldingCost + singleDataRecord.SetupCost;



                                Data.TimeVarInfiniteData.Add(singleDataRecord);
                            }
                            Data.ObjValue = model.ObjVal;

                            //... Rest of the code



                            // Convert ObservableCollection to DataFrame
                            var frame = Deedle.Frame.FromRecords(Data.TimeVarInfiniteData);
                            var transposedFrame = frame.Transpose();
                            Console.WriteLine(transposedFrame.Format());




                            foreach (var item in Data.TimeVarInfiniteData)
                            {
                                Console.WriteLine($"Day: {item.Day}, Demand: {item.Demand}, QuantityOrdered: {item.QuantityOrdered}, " +
                                    $"InventoryLevel: {item.InventoryLevel}, IsOrdering: {item.IsOrdering}, " +
                                    $"SetupCost: {item.SetupCost}, HoldingCost: {item.HoldingCost}, TotalCost: {item.TotalCost}");
                            }
                        }

                        else
                        {
                            Console.WriteLine("Model could not be solved optimally. Status: " + model.Status);
                        }
                    }
                    else if (Filter.TimeVaryingDemandType == BasicEnums.TimeVaryingDemandType.Finite_Capacity)
                    {
                        // ...
                    }
                }

                model.Dispose();
                env.Dispose();

                return Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return Data;
            }
        }

        public List<string> GetDailyItemQuantityData(InvControlData Filter, ItemData Item)
        {
            List<string> DemandList = new List<string>();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                if (Filter.DateFrom < new DateTime(1753, 1, 1) || Filter.DateFrom > new DateTime(9999, 12, 31))
                {
                    throw new Exception("Invalid DateFrom value.");
                }

                if (Filter.DateTo < new DateTime(1753, 1, 1) || Filter.DateTo > new DateTime(9999, 12, 31))
                {
                    throw new Exception("Invalid DateTo value.");
                }
                System.Diagnostics.Debug.WriteLine($"DateFrom: {Filter.DateFrom}, DateTo: {Filter.DateTo}");




                command.Parameters.AddWithValue("@DateFrom", Filter.DateFrom);
                command.Parameters.AddWithValue("@DateTo", Filter.DateTo);
                command.Parameters.AddWithValue("@ItemId", Item.ItemId);
                command.CommandText = string.Format(@"SELECT Rmaster.ItemId,
D.Date,D.TotalQuantity 
FROM DailyItemQuantity as D 
Inner Join Rmaster on Rmaster.ItemId = D.ItemId
Where D.Date >= @DateFrom and D.Date <=@DateTo and Rmaster.ItemId = @ItemId
ORDER BY Date ASC"
 );

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DemandList.Add(reader["TotalQuantity"].ToString());
                    }
                }

                connection.Close();
            }

            return DemandList;
        }

        public ObservableCollection<TimeVaryingInvResultsData> CalculateInvControlTimeVar2(InvControlData Filter, ObservableCollection<TimeVaryingInvResultsData> DataList)
        {
            GRBEnv env = new GRBEnv("logfile.log");

            GRBModel model = new GRBModel(env);
            InvControlData Data = new InvControlData();
            Data.TimeVarInfiniteData = new ObservableCollection<TimeVaryingInvData>();
            double bigM = 10000;

            try
            {
                // Define the parameters
                if (Filter.DemandType == BasicEnums.DemandType.Time_Varying_Demand)
                {
                    ItemData[] Items = Filter.Item.ToArray();




                    // Assuming that Filter carries all necessary information for optimization.
                    // Adjust the code below if it's stored differently.
                    int numDays = (Filter.DateTo - Filter.DateFrom).Days;
                    double[] demands = DataList[1].Values.Select(x => double.Parse(x)).ToArray();
                    double[] setupCosts = DataList[2].Values.Select(x => double.Parse(x)).ToArray();
                    double[] holdingCosts = DataList[3].Values.Select(x => double.Parse(x)).ToArray();

                    if (Filter.TimeVaryingDemandType == BasicEnums.TimeVaryingDemandType.Infinite_Capacity)
                    {
                        // Decision variables
                        GRBVar[] quantityOrdered = model.AddVars(numDays, GRB.INTEGER);
                        GRBVar[] inventoryLevel = model.AddVars(numDays, GRB.INTEGER);
                        GRBVar[] isOrdering = model.AddVars(numDays, GRB.BINARY);

                        // Objective function
                        GRBLinExpr objective = 0;
                        for (int t = 0; t < numDays; t++)
                        {
                            objective.AddTerm(holdingCosts[t], inventoryLevel[t]); // holding cost
                            objective.AddTerm(setupCosts[t], isOrdering[t]); // setup cost
                        }
                        model.SetObjective(objective, GRB.MINIMIZE);

                        // Constraints
                        for (int t = 0; t < numDays; t++)
                        {
                            // Inventory balance constraint
                            if (t == 0)
                                model.AddConstr(inventoryLevel[t] == quantityOrdered[t] - demands[t], $"InventoryBalance_{t}");
                            else
                                model.AddConstr(inventoryLevel[t] == inventoryLevel[t - 1] + quantityOrdered[t] - demands[t], $"InventoryBalance_{t}");

                            // Big-M constraint for ordering decision
                            model.AddConstr(quantityOrdered[t] <= isOrdering[t] * bigM, $"BigM_{t}");

                            // Non-negativity constraints
                            model.AddConstr(inventoryLevel[t] >= 0, $"InventoryNonNegative_{t}");
                            model.AddConstr(quantityOrdered[t] >= 0, $"QuantityNonNegative_{t}");
                        }

                        // Solve the model
                        model.Optimize();

                        // Populate Records with the optimal solution
                        if (model.Status == GRB.Status.OPTIMAL)
                        {

                            // Populate Records with the optimal solution
                            for (int t = 0; t < numDays; t++)
                            {


                                TimeVaryingInvResultsData data = new TimeVaryingInvResultsData();


                                data.Values = new List<string>();

                                double quantityOrderedValue = quantityOrdered[t].Get(GRB.DoubleAttr.X);
                                double inventoryLevelValue = inventoryLevel[t].Get(GRB.DoubleAttr.X);

                                double isOrderingValue, SetupCost, HoldingCost, TotalCost;

                                if (quantityOrderedValue == 0 && inventoryLevelValue == 0)
                                {
                                    isOrderingValue = 0;
                                    SetupCost = 0;
                                    HoldingCost = 0;
                                    TotalCost = 0;
                                }
                                else
                                {
                                    isOrderingValue = isOrdering[t].Get(GRB.DoubleAttr.X);
                                    SetupCost = setupCosts[t] * isOrderingValue;
                                    HoldingCost = holdingCosts[t] * inventoryLevelValue;
                                    TotalCost = HoldingCost + SetupCost;
                                }





                                //ChatGPt round results because i get prices like : 79.99999781 maybe we should do something inside the optimisation and not here because it may alter the resuklts
                                DataList[4].Values.Add(quantityOrderedValue.ToString());
                                DataList[5].Values.Add(inventoryLevelValue.ToString());
                                DataList[6].Values.Add(isOrderingValue.ToString());
                                DataList[7].Values.Add(SetupCost.ToString());
                                DataList[8].Values.Add(HoldingCost.ToString());
                                DataList[9].Values.Add(TotalCost.ToString());
                            }
                            Data.ObjValue = model.ObjVal;

                            //... Rest of the code



                            // Convert ObservableCollection to DataFrame

                        }

                        else
                        {
                            Console.WriteLine("Model could not be solved optimally. Status: " + model.Status);
                        }

                    }

                    else if (Filter.TimeVaryingDemandType == BasicEnums.TimeVaryingDemandType.Finite_Capacity)
                    {
                        // ...
                    }
                }

                model.Dispose();
                env.Dispose();

                return DataList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return DataList;
            }
        }


        #endregion

        #region MPS

        public MPSOutputData CalculateMPS1(MPSInputData InputData) //Factory Planning 1 SpecificDates
        {
            GRBEnv env = new GRBEnv("mpslogfile.log");

            GRBModel model = new GRBModel(env);
            MPSOutputData Data = new MPSOutputData();
            Data.MPSOptResultsData = new ObservableCollection<MPSOptResultsData>();
            //Data.TimeVarInfiniteData = new ObservableCollection<TimeVaryingInvData>();
            double bigM = 10000;



            try
            {
                // Define the parameters
                if (InputData.Forecast.TimeBucket != BasicEnums.Timebucket.Quarterly )
                {
                    // Assuming that Filter carries all necessary information for optimization.
                    // Adjust the code below if it's stored differently.
                    int num = InputData.Forecast.PeriodNumber;
                    string[] Products = InputData.Items.Select(d => d.ItemCode).ToArray();
                    string[] Machines = InputData.PrimaryMachines.Select(d => d.MachCode).ToArray();
                    string[] Dates = InputData.DatesStr;

                    var time_req = InputData.TimeReq;
                    var down = InputData.MachDownReq;
                    var installed = InputData.MachInstalled;
                    var max_sales = InputData.MaxDemand;

                    int numProducts = Products.Length;

                    var holdingCost = InputData.HoldingCostDict;
                    var profit = InputData.ProfitDict;
                    var max_inventory = InputData.MaxInventoryDict;
                    var store_target = InputData.StoreTargetDict;

                    var hours_per_month = InputData.HoursPerMonth;



                    if (InputData.Forecast.TimeBucket != BasicEnums.Timebucket.Quarterly)
                    {

                        // Decision variables

                        GRBVar[,] make = new GRBVar[Dates.Length, Products.Length];
                        GRBVar[,] store = new GRBVar[Dates.Length, Products.Length];
                        GRBVar[,] sell = new GRBVar[Dates.Length, Products.Length];

                        for (int i = 0; i < Dates.Length; i++)
                        {
                            for (int j = 0; j < Products.Length; j++)
                            {
                                var key = (Dates[i], Products[j]);

                                // Add the data to the MaxDemand dictionary

                                make[i, j] = model.AddVar(0.0, GRB.INFINITY, 0.0, GRB.CONTINUOUS, "make_" + Dates[i] + "_" + Products[j]);
                                store[i, j] = model.AddVar(0.0, ub: max_inventory[Products[j]], GRB.INFINITY, GRB.CONTINUOUS, "store_" + Dates[i] + "_" + Products[j]);

                                var upperboundsales = max_sales[key];

                                sell[i, j] = model.AddVar(0.0, ub: upperboundsales, GRB.INFINITY, GRB.CONTINUOUS, "sell_" + Dates[i] + "_" + Products[j]);

                            }
                        }



                        // Objective function
                        GRBLinExpr objective = 0;
                        // Objective function
                        for (int i = 0; i < Dates.Length; i++)
                        {
                            for (int j = 0; j < Products.Length; j++)
                            {
                                string productCode = Products[j]; // Get the product code
                                double productProfit = profit[productCode]; // Access the profit using the product code

                                objective.AddTerm(productProfit, sell[i, j]); // profit
                                objective.AddTerm(-holdingCost[productCode], store[i, j]); // holding cost
                            }
                        }


                        model.SetObjective(objective, GRB.MAXIMIZE);

                        // Constraints

                        // #1. Initial Balance
                        for (int j = 0; j < Products.Length; j++)
                        {
                            var product = Products[j];
                            var month = Dates[0];

                            model.AddConstr(make[0, j] == sell[0, j] + store[0, j], $"Initial_Balance_{product}");

                        }

                        // #2. Balance
                        for (int j = 0; j < Products.Length; j++)
                        {
                            for (int i = 1; i < Dates.Length; i++) // Start from the second month (index 1)
                            {
                                var product = Products[j];
                                var month = Dates[i];

                                // Get the previous month
                                var previousMonth = Dates[i - 1];

                                model.AddConstr(store[i - 1, j] + make[i, j] == sell[i, j] + store[i, j], $"Balance_{month}_{product}");
                            }
                        }
                        // #3. Inventory Target
                        for (int j = 0; j < Products.Length; j++)
                        {
                            var product = Products[j];
                            var month = Dates[Dates.Length -1];

                            model.AddConstr(store[Dates.Length - 1, j] == store_target[product], $"End_Balance_{product}");
                        }

                        // #4. Machine Capacity
                        for (int k = 0; k < Machines.Length; k++)
                        {
                            var machine = Machines[k];

                            for (int i = 0; i < Dates.Length; i++)
                            {
                                var month = Dates[i];

                                var productSum = new GRBLinExpr();

                                foreach (var product in time_req[machine].Keys)
                                {
                                    int j = Array.IndexOf(Products, product);

                                    productSum.Add(time_req[machine][product] * make[i, j]);
                                }

                                model.AddConstr(productSum <= hours_per_month * (installed[machine] - (down.ContainsKey((month, machine)) ? down[(month, machine)] : 0)), $"Capacity_{machine}_{month}");
                            }
                        }


                        // Solve the model
                        model.Optimize();

                        #region OutputResults
                        if (model.Status == GRB.Status.OPTIMAL)
                        {
                            // Create a data structure to store the optimal solution
                            List<string> rows = new List<string>();
                            List<string> columns = new List<string>();
                            Dictionary<(string, string), double> make_plan = new Dictionary<(string, string), double>();

                            // Extract the optimal solution for the 'make' variables
                            for (int i = 0; i < Dates.Length; i++)
                            {
                                for (int j = 0; j < Products.Length; j++)
                                {
                                    string month = Dates[i];
                                    string product = Products[j];
                                    double makeValue = make[i, j].X;

                                    // Store the optimal 'make' value in the data structure
                                    make_plan[(month, product)] = makeValue;

                                    // Add 'month' and 'product' to the respective lists if they are not already there
                                    if (!rows.Contains(month))
                                        rows.Add(month);
                                    if (!columns.Contains(product))
                                        columns.Add(product);
                                }
                            }

                            // Print the optimal solution
                            Console.WriteLine("Optimal Solution:");
                            Console.Write("          "); // Add some padding for alignment

                            // Print column headers (product names)
                            foreach (string column in columns)
                            {
                                Console.Write($"{column}    ");
                            }
                            Console.WriteLine(); // Move to the next line

                            // Print the 'make' values for each month and product
                            foreach (string row in rows)
                            {
                                Console.Write($"{row}     ");
                                foreach (string column in columns)
                                {
                                    double makeValue = make_plan[(row, column)];
                                    Console.Write($"{makeValue,6:F1}    "); // Format the value with 1 decimal place
                                }
                                Console.WriteLine(); // Move to the next line
                            }

                            // ...

                            // Extract the optimal solution for the 'sell' variables
                            Dictionary<(string, string), double> sell_plan = new Dictionary<(string, string), double>();
                            for (int i = 0; i < Dates.Length; i++)
                            {
                                for (int j = 0; j < Products.Length; j++)
                                {
                                    string month = Dates[i];
                                    string product = Products[j];
                                    double sellValue = sell[i, j].X;

                                    // Store the optimal 'sell' value in the data structure
                                    sell_plan[(month, product)] = sellValue;
                                }
                            }

                            // Extract the optimal solution for the 'store' variables
                            Dictionary<(string, string), double> store_plan = new Dictionary<(string, string), double>();
                            for (int i = 0; i < Dates.Length; i++)
                            {
                                for (int j = 0; j < Products.Length; j++)
                                {
                                    string month = Dates[i];
                                    string product = Products[j];
                                    double storeValue = store[i, j].X;

                                    // Store the optimal 'store' value in the data structure
                                    store_plan[(month, product)] = storeValue;
                                }
                            }

                            // Print the optimal sales plan
                            Console.WriteLine("Optimal Sales Plan:");
                            Console.Write("          "); // Add some padding for alignment

                            foreach (string column in columns)
                            {
                                Console.Write($"{column}    ");
                            }
                            Console.WriteLine(); // Move to the next line

                            foreach (string row in rows)
                            {
                                Console.Write($"{row}     ");
                                foreach (string column in columns)
                                {
                                    double sellValue = sell_plan[(row, column)];
                                    Console.Write($"{sellValue,6:F1}    "); // Format the value with 1 decimal place
                                }
                                Console.WriteLine(); // Move to the next line
                            }

                            // Print the optimal store plan
                            Console.WriteLine("Optimal Store Plan:");
                            Console.Write("          "); // Add some padding for alignment

                            foreach (string column in columns)
                            {
                                Console.Write($"{column}    ");
                            }
                            Console.WriteLine(); // Move to the next line

                            foreach (string row in rows)
                            {
                                Console.Write($"{row}     ");
                                foreach (string column in columns)
                                {
                                    double storeValue = store_plan[(row, column)];
                                    Console.Write($"{storeValue,6:F1}    "); // Format the value with 1 decimal place
                                }
                                Console.WriteLine(); // Move to the next line
                            }

                            #region Print Excel 
                            // Assuming you've already obtained the optimal solution

                            ExcelPackage.LicenseContext = LicenseContext.Commercial;

                            // Create a new Excel package
                            using (var package = new ExcelPackage())
                            {
                                // Add a worksheet for the 'make' values
                                var makeWorksheet = package.Workbook.Worksheets.Add("Make");

                                // Add headers to the 'make' worksheet
                                makeWorksheet.Cells[1, 1].Value = "Month";
                                makeWorksheet.Cells[1, 2].Value = "Product";
                                makeWorksheet.Cells[1, 3].Value = "Make Value";

                                int row = 2;
                                foreach (var kvp in make_plan)
                                {
                                    makeWorksheet.Cells[row, 1].Value = kvp.Key.Item1; // Month
                                    makeWorksheet.Cells[row, 2].Value = kvp.Key.Item2; // Product
                                    makeWorksheet.Cells[row, 3].Value = kvp.Value;     // Make Value
                                    row++;
                                }

                                // Add a worksheet for the 'sell' values
                                var sellWorksheet = package.Workbook.Worksheets.Add("Sell");

                                // Add headers to the 'sell' worksheet
                                sellWorksheet.Cells[1, 1].Value = "Month";
                                sellWorksheet.Cells[1, 2].Value = "Product";
                                sellWorksheet.Cells[1, 3].Value = "Sell Value";

                                row = 2;
                                foreach (var kvp in sell_plan)
                                {
                                    sellWorksheet.Cells[row, 1].Value = kvp.Key.Item1; // Month
                                    sellWorksheet.Cells[row, 2].Value = kvp.Key.Item2; // Product
                                    sellWorksheet.Cells[row, 3].Value = kvp.Value;     // Sell Value
                                    row++;
                                }

                                // Add a worksheet for the 'store' values
                                var storeWorksheet = package.Workbook.Worksheets.Add("Store");

                                // Add headers to the 'store' worksheet
                                storeWorksheet.Cells[1, 1].Value = "Month";
                                storeWorksheet.Cells[1, 2].Value = "Product";
                                storeWorksheet.Cells[1, 3].Value = "Store Value";

                                row = 2;
                                foreach (var kvp in store_plan)
                                {
                                    storeWorksheet.Cells[row, 1].Value = kvp.Key.Item1; // Month
                                    storeWorksheet.Cells[row, 2].Value = kvp.Key.Item2; // Product
                                    storeWorksheet.Cells[row, 3].Value = kvp.Value;     // Store Value
                                    row++;
                                }

                                // Save the Excel package to a file
                                var excelFile = new FileInfo("OptimizationResults.xlsx");
                                package.SaveAs(excelFile);
                            }


                            #endregion

                        }
                        else
                        {
                            Console.WriteLine("Optimization did not converge to an optimal solution.");
                        }

                        #endregion

                        #region Populate Records with the optimal solution

                        if (model.Status == GRB.Status.OPTIMAL)
                        {

                            // Populate Records with the optimal solution
                            for (int j = 0; j < Products.Length; j++)
                            {
                                for (int i = 0; i < Dates.Length; i++) // Start from the second month (index 1)
                                {
                                    MPSOptResultsData singleDataRecord = new MPSOptResultsData();
                                    singleDataRecord.Item = new ItemData();
                                    singleDataRecord.Date = Dates[i];

                                    var key = (Dates[i], Products[j]);
                                    singleDataRecord.Demand = InputData.MaxDemand[key];

                                    double makevalue = make[i, j].Get(GRB.DoubleAttr.X);
                                    singleDataRecord.Make = makevalue;

                                    double sellvalue = sell[i, j].Get(GRB.DoubleAttr.X);
                                    singleDataRecord.Sell = sellvalue;

                                    double inventoryLevelValue = store[i, j].Get(GRB.DoubleAttr.X);
                                    singleDataRecord.Store = inventoryLevelValue;



                                    singleDataRecord.Item.ItemCode = Products[j];
                                    singleDataRecord.ItemCode = Products[j];

                                    Data.MPSOptResultsData.Add(singleDataRecord);

                                }

                            }
                            Data.ObjValue = model.ObjVal;

                            //... Rest of the code


                            #region  Populate  Maintenance Plan with the optimal solution

                            Data.MachRepairResultsData = new ObservableCollection<MachRepairResultsData>();

                            for (int j = 0; j < Machines.Length; j++)
                            {
                                for (int i = 0; i < Dates.Length; i++)
                                {
                                    string date = Dates[i];
                                    string machine = Machines[j];
                                    int repairValue = 0;

                                    // Check if the (date, machine) pair exists in the dictionary
                                    if (InputData.MachDownReq.TryGetValue((date, machine), out int value))
                                    {
                                        repairValue = value;
                                    }

                                    MachRepairResultsData singleDataRecord = new MachRepairResultsData();
                                    singleDataRecord.Mach = new MachineData();

                                    singleDataRecord.Mach.MachCode = Machines[j];
                                    singleDataRecord.MachCode = Machines[j];

                                    singleDataRecord.Date = date;
                                    singleDataRecord.NumberOfRepairs = repairValue;

                                    Data.MachRepairResultsData.Add(singleDataRecord);
                                }
                            }

                            #endregion


                        }

                        else
                        {
                            Console.WriteLine("Model could not be solved optimally. Status: " + model.Status);
                        }
                        #endregion



                    }

                }

                model.Dispose();
                env.Dispose();

                return Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return Data;
            }
        } //Factory Planning 2 

        public MPSOutputData CalculateMPS2(MPSInputData Filter)
        {
            GRBEnv env = new GRBEnv("mpslogfile.log");

            GRBModel model = new GRBModel(env);
            MPSOutputData Data = new MPSOutputData();
            Data.MPSOptResultsData = new ObservableCollection<MPSOptResultsData>();
            Data.MachRepairResultsData = new ObservableCollection<MachRepairResultsData>();

            //Data.TimeVarInfiniteData = new ObservableCollection<TimeVaryingInvData>();
            double bigM = 10000;



            try
            {
                // Define the parameters
                if (Filter.Forecast.TimeBucket == BasicEnums.Timebucket.Monthly || Filter.Forecast.TimeBucket == BasicEnums.Timebucket.Weekly || Filter.Forecast.TimeBucket == BasicEnums.Timebucket.Daily)
                {
                    // Assuming that Filter carries all necessary information for optimization.
                    // Adjust the code below if it's stored differently.
                    string[] Products = Filter.Items.Select(d => d.ItemCode).ToArray();
                    string[] Machines = Filter.PrimaryMachines.Select(d => d.MachCode).ToArray();
                    string[] Dates = Filter.DatesStr;

                    var time_req = Filter.TimeReq;
                    var down_req = Filter.MachDownReq2;
                    var installed = Filter.MachInstalled;
                    var max_sales = Filter.MaxDemand;

                    int numProducts = Products.Length;

                    // Create an array of holding costs with all elements set to 0.5

                    var holdingCost = Filter.HoldingCostDict;
                    var profit = Filter.ProfitDict;
                    var max_inventory = Filter.MaxInventoryDict;
                    var store_target = Filter.StoreTargetDict;

                    var hours_per_month = Filter.HoursPerMonth;



                    if (Filter.Forecast.TimeBucket != BasicEnums.Timebucket.Quarterly)
                    {
                        // Decision variables
                        GRBVar[,] make = new GRBVar[Dates.Length, Products.Length];
                        GRBVar[,] store = new GRBVar[Dates.Length, Products.Length];
                        GRBVar[,] sell = new GRBVar[Dates.Length, Products.Length];
                        GRBVar[,] repair = new GRBVar[Dates.Length, Machines.Length];

                        for (int i = 0; i < Dates.Length; i++)
                        {
                            for (int j = 0; j < Products.Length; j++)
                            {
                                var key = (Dates[i], Products[j]);

                                // Add the data to the MaxDemand dictionary

                                make[i, j] = model.AddVar(0.0, GRB.INFINITY, 0.0, GRB.INTEGER, "make_" + Dates[i] + "_" + Products[j]);
                                store[i, j] = model.AddVar(0.0, ub: max_inventory[Products[j]], GRB.INFINITY, GRB.INTEGER, "store_" + Dates[i] + "_" + Products[j]);

                                var upperboundsales = max_sales[key];

                                sell[i, j] = model.AddVar(0.0, ub: upperboundsales, GRB.INFINITY, GRB.INTEGER, "sell_" + Dates[i] + "_" + Products[j]);
                                Console.WriteLine(j);
                            }

                            for (int j = 0; j < Machines.Length; j++)
                            {
                                var upperbounddownReq = down_req[Machines[j]] ;
                                repair[i, j] = model.AddVar(0.0, ub:upperbounddownReq, GRB.INFINITY, GRB.INTEGER, "repair_" + Dates[i] + "_" + Machines[j]);
                                //repair[i, j] = model.AddVar(0.0, GRB.INFINITY, 0.0, GRB.INTEGER, "repair_" + Dates[i] + "_" + Machines[j]);
                            }
                        }



                        // Objective function
                        GRBLinExpr objective = 0;
                        // Objective function
                        for (int i = 0; i < Dates.Length; i++)
                        {
                            for (int j = 0; j < Products.Length; j++)
                            {
                                string productCode = Products[j]; // Get the product code
                                double productProfit = profit[productCode]; // Access the profit using the product code

                                objective.AddTerm(productProfit, sell[i, j]); // profit
                                objective.AddTerm(-holdingCost[productCode], store[i, j]); // holding cost
                            }
                        }


                        model.SetObjective(objective, GRB.MAXIMIZE);

                        // Constraints
                        // Constraints

                        // #1. Initial Balance
                        for (int j = 0; j < Products.Length; j++)
                        {
                            var product = Products[j];
                            var month = Dates[0];

                            model.AddConstr(make[0, j] == sell[0, j] + store[0, j], $"Initial_Balance_{product}");
                        }

                        // #2. Balance
                        for (int j = 0; j < Products.Length; j++)
                        {
                            for (int i = 1; i < Dates.Length; i++) // Start from the second month (index 1)
                            {
                                var product = Products[j];
                                var month = Dates[i];

                                // Get the previous month
                                var previousMonth = Dates[i - 1];

                                model.AddConstr(store[i - 1, j] + make[i, j] == sell[i, j] + store[i, j], $"Balance_{month}_{product}");
                            }
                        }
                        // #3. Inventory Target
                        for (int j = 0; j < Products.Length; j++)
                        {
                            var product = Products[j];
                            var month = Dates[Dates.Length - 1];

                            model.AddConstr(store[Dates.Length - 1, j] == store_target[product], $"End_Balance_{product}");
                        }

                        // #4. Machine Capacity
                        for (int k = 0; k < Machines.Length; k++)
                        {
                            var machine = Machines[k];

                            for (int i = 0; i < Dates.Length; i++)
                            {
                                var month = Dates[i];

                                var productSum = new GRBLinExpr();

                                foreach (var product in time_req[machine].Keys)
                                {
                                    int j = Array.IndexOf(Products, product);

                                    productSum.Add(time_req[machine][product] * make[i, j]);
                                }

                                model.AddConstr(productSum <= hours_per_month * (installed[machine] - repair[i, k]), $"Capacity_{machine}_{month}");
                            }
                        }

                        // #5. Maintenance
                        for (int k = 0; k < Machines.Length; k++)
                        {
                            var machine = Machines[k];

                            var maintenanceConstraint = new GRBLinExpr();
                            var repairSum = new GRBLinExpr();

                            for (int i = 0; i < Dates.Length; i++)
                            {
                                var month = Dates[i];



                                repairSum.Add(repair[i, k]);


                            }
                            model.AddConstr(repairSum == down_req[machine], $"Maintenance_{machine}");
                        }


                        // Solve the model
                        model.Optimize();

                        #region OutputResults
                        if (model.Status == GRB.Status.OPTIMAL)
                        {


                            #region Extract/Print Results

                            #region Extract Plans
                            // Create a data structure to store the optimal solution
                            List<string> rows = new List<string>();
                            List<string> columns = new List<string>();
                            List<string> Machcolumns = new List<string>();

                            Dictionary<(string, string), double> make_plan = new Dictionary<(string, string), double>();

                            // Extract the optimal solution for the 'make' variables
                            for (int i = 0; i < Dates.Length; i++)
                            {
                                for (int j = 0; j < Products.Length; j++)
                                {
                                    string month = Dates[i];
                                    string product = Products[j];
                                    double makeValue = make[i, j].X;

                                    // Store the optimal 'make' value in the data structure
                                    make_plan[(month, product)] = makeValue;

                                    // Add 'month' and 'product' to the respective lists if they are not already there
                                    if (!rows.Contains(month))
                                        rows.Add(month);
                                    if (!columns.Contains(product))
                                        columns.Add(product);
                                }
                            }

                            // Extract the optimal solution for the 'sell' variables
                            Dictionary<(string, string), double> sell_plan = new Dictionary<(string, string), double>();
                            for (int i = 0; i < Dates.Length; i++)
                            {
                                for (int j = 0; j < Products.Length; j++)
                                {
                                    string month = Dates[i];
                                    string product = Products[j];
                                    double sellValue = sell[i, j].X;

                                    // Store the optimal 'sell' value in the data structure
                                    sell_plan[(month, product)] = sellValue;
                                }
                            }

                            // Extract the optimal solution for the 'store' variables
                            Dictionary<(string, string), double> store_plan = new Dictionary<(string, string), double>();
                            for (int i = 0; i < Dates.Length; i++)
                            {
                                for (int j = 0; j < Products.Length; j++)
                                {
                                    string month = Dates[i];
                                    string product = Products[j];
                                    double storeValue = store[i, j].X;

                                    // Store the optimal 'store' value in the data structure
                                    store_plan[(month, product)] = storeValue;
                                }
                            }
                            // Extract the optimal solution for the 'repair' variables
                            Dictionary<(string, string), double> repair_plan = new Dictionary<(string, string), double>();
                            for (int i = 0; i < Dates.Length; i++)
                            {
                                for (int j = 0; j < Machines.Length; j++)
                                {
                                    string month = Dates[i];
                                    string machine = Machines[j];
                                    double repairValue = repair[i, j].X;

                                    // Store the optimal 'store' value in the data structure
                                    repair_plan[(month, machine)] = repairValue;

                                    if (!Machcolumns.Contains(machine))
                                        Machcolumns.Add(machine);
                                }
                            }

                            #endregion



                            // ...


                            #region Print Optimal Solution and Plans

                            // Print the optimal solution
                            Console.WriteLine("Optimal Solution:");
                            Console.Write("          "); // Add some padding for alignment


                            #region Production Plan
                            // Print the optimal Production plan
                            Console.WriteLine("Optimal Production Plan:");
                            Console.Write("          "); // Add some padding for alignment

                            // Print column headers (product names)
                            foreach (string column in columns)
                            {
                                Console.Write($"{column}    ");
                            }
                            Console.WriteLine(); // Move to the next line

                            // Print the 'make' values for each month and product
                            foreach (string row in rows)
                            {
                                Console.Write($"{row}     ");
                                foreach (string column in columns)
                                {
                                    double makeValue = make_plan[(row, column)];
                                    Console.Write($"{makeValue,6:F1}    "); // Format the value with 1 decimal place
                                }
                                Console.WriteLine(); // Move to the next line
                            }

                            #endregion

                            #region Sales Plan

                            // Print the optimal sales plan
                            Console.WriteLine("Optimal Sales Plan:");
                            Console.Write("          "); // Add some padding for alignment

                            foreach (string column in columns)
                            {
                                Console.Write($"{column}    ");
                            }
                            Console.WriteLine(); // Move to the next line

                            foreach (string row in rows)
                            {
                                Console.Write($"{row}     ");
                                foreach (string column in columns)
                                {
                                    double sellValue = sell_plan[(row, column)];
                                    Console.Write($"{sellValue,6:F1}    "); // Format the value with 1 decimal place
                                }
                                Console.WriteLine(); // Move to the next line
                            }

                            #endregion

                            #region Inventory Plan
                            // Print the optimal store plan
                            Console.WriteLine("Optimal Inventory Plan:");
                            Console.Write("          "); // Add some padding for alignment

                            foreach (string column in columns)
                            {
                                Console.Write($"{column}    ");
                            }
                            Console.WriteLine(); // Move to the next line

                            foreach (string row in rows)
                            {
                                Console.Write($"{row}     ");
                                foreach (string column in columns)
                                {
                                    double storeValue = store_plan[(row, column)];
                                    Console.Write($"{storeValue,6:F1}    "); // Format the value with 1 decimal place
                                }
                                Console.WriteLine(); // Move to the next line
                            }
                            #endregion

                            #region Repair Plan
                            // Print the optimal repair plan
                            Console.WriteLine("Optimal Repair Plan:");
                            Console.Write("          "); // Add some padding for alignment

                            foreach (string Machcolumn in Machcolumns)
                            {
                                Console.Write($"{Machcolumn}    ");
                            }
                            Console.WriteLine(); // Move to the next line

                            foreach (string row in rows)
                            {
                                Console.Write($"{row}     ");
                                foreach (string Machcolumn in Machcolumns)
                                {
                                    var repairValue = repair_plan[(row, Machcolumn)];
                                    Console.Write($"{repairValue,6:F1}    "); // Format the value with 1 decimal place
                                }
                                Console.WriteLine(); // Move to the next line
                            }
                            #endregion

                            #endregion

                            #endregion

                            #region Print Excel 
                            // Assuming you've already obtained the optimal solution

                            ExcelPackage.LicenseContext = LicenseContext.Commercial;

                            // Create a new Excel package
                            using (var package = new ExcelPackage())
                            {
                                // Add a worksheet for the 'make' values
                                var makeWorksheet = package.Workbook.Worksheets.Add("Make");

                                // Add headers to the 'make' worksheet
                                makeWorksheet.Cells[1, 1].Value = "Month";
                                makeWorksheet.Cells[1, 2].Value = "Product";
                                makeWorksheet.Cells[1, 3].Value = "Make Value";

                                int row = 2;
                                foreach (var kvp in make_plan)
                                {
                                    makeWorksheet.Cells[row, 1].Value = kvp.Key.Item1; // Month
                                    makeWorksheet.Cells[row, 2].Value = kvp.Key.Item2; // Product
                                    makeWorksheet.Cells[row, 3].Value = kvp.Value;     // Make Value
                                    row++;
                                }

                                // Add a worksheet for the 'sell' values
                                var sellWorksheet = package.Workbook.Worksheets.Add("Sell");

                                // Add headers to the 'sell' worksheet
                                sellWorksheet.Cells[1, 1].Value = "Month";
                                sellWorksheet.Cells[1, 2].Value = "Product";
                                sellWorksheet.Cells[1, 3].Value = "Sell Value";

                                row = 2;
                                foreach (var kvp in sell_plan)
                                {
                                    sellWorksheet.Cells[row, 1].Value = kvp.Key.Item1; // Month
                                    sellWorksheet.Cells[row, 2].Value = kvp.Key.Item2; // Product
                                    sellWorksheet.Cells[row, 3].Value = kvp.Value;     // Sell Value
                                    row++;
                                }

                                // Add a worksheet for the 'store' values
                                var storeWorksheet = package.Workbook.Worksheets.Add("Store");

                                // Add headers to the 'store' worksheet
                                storeWorksheet.Cells[1, 1].Value = "Month";
                                storeWorksheet.Cells[1, 2].Value = "Product";
                                storeWorksheet.Cells[1, 3].Value = "Store Value";

                                row = 2;
                                foreach (var kvp in store_plan)
                                {
                                    storeWorksheet.Cells[row, 1].Value = kvp.Key.Item1; // Month
                                    storeWorksheet.Cells[row, 2].Value = kvp.Key.Item2; // Product
                                    storeWorksheet.Cells[row, 3].Value = kvp.Value;     // Store Value
                                    row++;
                                }

                                // Save the Excel package to a file
                                var excelFile = new FileInfo("OptimizationResults.xlsx");
                                package.SaveAs(excelFile);
                            }


                            #endregion

                        }
                        else
                        {
                            Console.WriteLine("Optimization did not converge to an optimal solution.");
                        }

                        #endregion

                        #region Populate Records with the optimal solution

                        if (model.Status == GRB.Status.OPTIMAL)
                        {

                            #region  Populate  Prod/Sales/Inv Plan with the optimal solution

                            for (int j = 0; j < Products.Length; j++)
                            {
                                for (int i = 0; i < Dates.Length; i++) // Start from the second month (index 1)
                                {
                                    MPSOptResultsData singleDataRecord = new MPSOptResultsData();
                                    singleDataRecord.Item = new ItemData();
                                    singleDataRecord.Date = Dates[i];

                                    var key = (Dates[i], Products[j]);
                                    // Add the data to the MaxDemand dictionary
                                    singleDataRecord.Demand = Filter.MaxDemand[key];

                                    double makevalue = make[i, j].Get(GRB.DoubleAttr.X);
                                    singleDataRecord.Make = makevalue;

                                    double sellvalue = sell[i, j].Get(GRB.DoubleAttr.X);
                                    singleDataRecord.Sell = sellvalue;

                                    double inventoryLevelValue = store[i, j].Get(GRB.DoubleAttr.X);
                                    singleDataRecord.Store = inventoryLevelValue;



                                    singleDataRecord.Item.ItemCode = Products[j];
                                    singleDataRecord.ItemCode = Products[j];

                                    Data.MPSOptResultsData.Add(singleDataRecord);

                                }

                            }
                            #endregion


                            #region  Populate  Maintenance Plan with the optimal solution
                            for (int j = 0; j < Machines.Length; j++)
                            {
                                for (int i = 0; i < Dates.Length; i++) // Start from the second month (index 1)
                                {
                                    MachRepairResultsData singleDataRecord = new MachRepairResultsData();
                                    singleDataRecord.Mach = new MachineData();
                                    singleDataRecord.Date = Dates[i];

                                    var key = (Dates[i], Machines[j]); //auto prepei na figei logika

                                    // Add the data to the MaxDemand dictionary
                                    //singleDataRecord.NumberOfRepairs = Filter.MaxDemand[key];
                                    var repairvalue = repair[i, j].Get(GRB.DoubleAttr.X);
                                    singleDataRecord.NumberOfRepairs = repairvalue;




                                    singleDataRecord.Mach.MachCode = Machines[j];
                                    singleDataRecord.MachCode = Machines[j];

                                    Data.MachRepairResultsData.Add(singleDataRecord);

                                }

                            }
                            #endregion
                            Data.ObjValue = model.ObjVal;

                        }

                        else
                        {
                            Console.WriteLine("Model could not be solved optimally. Status: " + model.Status);
                        }
                        #endregion



                    }

                }

                model.Dispose();
                env.Dispose();

                return Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return Data;
            }
        } //Factory Planning 2 OnlyNumberofRepairs

        #endregion

        #endregion


        public string[] CalculateDatesFormat(ForecastInfoData InputData)
        {
            var DateList = new string[5];
            try
            {

                int loopCounter = 0;
                decimal quarterDemand = 0;
                var periodtype = InputData.PeriodType;
                var periodnum = InputData.PeriodNumber;
                var timeBucket = InputData.TimeBucket;

                periodnum = 1;


                TimeSpan timeDifference = InputData.DateTo - InputData.DateFrom;

                int totalDays2 = timeDifference.Days;

                int numberOfYears2 = totalDays2 / 365;
                int remainingDaysAfterYears2 = totalDays2 % 365;

                int numberOfMonths2 = remainingDaysAfterYears2 / 30;
                int numberOfDays2 = remainingDaysAfterYears2 % 30;


                int numberOfYears = timeDifference.Days / 365;
                int numberOfMonths = (timeDifference.Days % 365) / 30;
                int numberOfDays = timeDifference.Days % 30;

                if (periodtype == BasicEnums.PeriodType.Yearly)
                {


                }

                if (periodtype == BasicEnums.PeriodType.Monthly)
                {

                }

                if (timeBucket == Timebucket.Monthly || timeBucket == Timebucket.Weekly || timeBucket == Timebucket.Daily)
                {

                }

                if (timeBucket == Timebucket.Quarterly && loopCounter == 3)
                {



                }


                return DateList;
            }
            catch
            {
                return DateList;

            }
        }

        public ForecastInfoData ChangeDatesFormat(ForecastInfoData Input)
        {
            ForecastInfoData Output = new ForecastInfoData();
            Output = Input;

            var PeriodType = Input.PeriodType;
            var TimeBucket = Input.TimeBucket;
            var DateFrom = Input.DateFrom;
            var DateTo = Input.DateTo;

            if (TimeBucket == BasicEnums.Timebucket.Monthly)
            {
                Output.DateFromStr = DateFrom.ToString("MMM/yyyy");
                Output.DateToStr = DateTo.ToString("MMM/yyyy");
            }
            else if (TimeBucket == BasicEnums.Timebucket.Weekly)
            {
                // Calculate the week of the month for DateFrom and DateTo
                int weekNumber = GetWeekOfMonth(DateFrom);
                int weekNumber2 = GetWeekOfMonth(DateTo);

                Output.DateFromStr = $"WEEK{weekNumber}/{DateFrom.ToString("MMM/yyyy")}";
                Output.DateToStr = $"WEEK{weekNumber2}/{DateTo.ToString("MMM/yyyy")}";
            }
            else if (TimeBucket == BasicEnums.Timebucket.Daily)
            {
                Output.DateFromStr = DateFrom.ToString("dd/MM/yyyy");
                Output.DateToStr = DateTo.ToString("dd/MM/yyyy");
            }
            return Output;
        }
        public string ChangeSpecificDateFormat(ForecastInfoData Input,DateTime DateIn)
        {
            string DateOut = "";

            var TimeBucket = Input.TimeBucket;

            if (TimeBucket == BasicEnums.Timebucket.Monthly)
            {
                DateOut = DateIn.ToString("MMM/yyyy");
            }
            else if (TimeBucket == BasicEnums.Timebucket.Weekly)
            {
                // Calculate the week of the month for DateFrom and DateTo
                int weekNumber = GetWeekOfMonth(DateIn);

                DateOut = $"WEEK{weekNumber}/{DateIn.ToString("MMM/yyyy")}";
            }
            else if (TimeBucket == BasicEnums.Timebucket.Daily)
            {
                DateOut = DateIn.ToString("dd/MM/yyyy");
            }
            return DateOut;
        }

        public static int GetWeekOfMonth(DateTime date)
        {
            // Calculate the week of the month
            return (date.Day - 1) / 7 + 1;
        }

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
