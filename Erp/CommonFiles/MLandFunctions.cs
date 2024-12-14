using Erp.DataBase.Τhesis;
using Erp.DataBase;
using Erp.Model.BasicFiles;
using Erp.Model.Thesis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erp.Repositories;
using Erp.DataBase;
using Microsoft.Data.SqlClient;
using Erp.Model.Motherland.BasicFiles;
using Erp.DataBase.Motherland;

namespace Erp.CommonFiles
{
    public class MLandFunctions : RepositoryBase
    {
        #region Department

        public ObservableCollection<DepartmentData> GetDepartmentData(bool ShowDeleted)
        {
            ObservableCollection<DepartmentData> DataList = new ObservableCollection<DepartmentData>();

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
                command.CommandText = string.Format(@"select Id,Code,Descr,IsDeleted from Department Where 1=1 {0}", FilterStr);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DepartmentData data = new DepartmentData();

                        data.Id = int.Parse(reader["Id"].ToString());
                        data.DepartCode = reader["Code"].ToString();
                        data.DepartDescr = reader["Descr"].ToString();
                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());

                        DataList.Add(data);
                    }
                }

                connection.Close();

            }

            return DataList;
        }

        public bool SaveDepartmentData(ObservableCollection<DepartmentData> Data)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {


                    bool hasChanges = false;
                    foreach (var row in Data)
                    {
                        var existingrow = dbContext.Department.SingleOrDefault(b => b.Id == row.Id);

                        if (existingrow == null)
                        {
                            DepartmentDataEntity newrow = new DepartmentDataEntity();
                            newrow.Code = row.DepartCode;
                            newrow.Descr = row.DepartDescr;
                            newrow.IsDeleted = false;
                            dbContext.Department.Add(newrow);
                            hasChanges = true;
                        }
                        else if (existingrow != null)
                        {

                            existingrow.Code = row.DepartCode;
                            existingrow.Descr = row.DepartDescr;
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
                LogError(ex, "SaveDepartmentData", "Notes");
                return false;
            }
        }



        #endregion

        #region Position
        public ObservableCollection<PositionData> GetPositionData(bool ShowDeleted)
        {
            ObservableCollection<PositionData> DataList = new ObservableCollection<PositionData>();

            string FilterStr = "";

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                if (ShowDeleted == false)
                {
                    command.Parameters.AddWithValue("@ShowDeleted", ShowDeleted);
                    FilterStr = String.Format(@" and P.IsDeleted =@ShowDeleted");
                }
                command.CommandText = string.Format(@"select P.Id,P.Code,P.Descr ,D.Id as DepartId,D.Code as DepartCode,D.Descr as DepartDescr ,P.IsDeleted
                                                    from Position as P 
                                                    Inner Join Department as D on P.DepartId = D.Id Where 1=1 {0}", FilterStr);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PositionData data = new PositionData();
                        data.Id = int.Parse(reader["Id"].ToString());

                        data.PosCode = reader["Code"].ToString();
                        data.PosDescr = reader["Descr"].ToString();

                        data.DepartId = int.Parse(reader["DepartId"].ToString());
                        data.DepartCode = reader["DepartCode"].ToString();
                        data.Department = new DepartmentData();
                        data.Department.Id = int.Parse(reader["DepartId"].ToString());
                        data.Department.DepartCode = reader["DepartCode"].ToString();
                        data.Department.DepartDescr = reader["DepartDescr"].ToString();

                        data.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());


                        DataList.Add(data);
                    }
                }

                connection.Close();

            }

            return DataList;
        }

        public bool SavePositionData(ObservableCollection<PositionData> Data)
        {
            try
            {
                using (var dbContext = new ErpDbContext(options))
                {
                    // Get list of CityIds from the database

                    foreach (var row in Data)
                    {
                        var existingrow = dbContext.Position.SingleOrDefault(b => b.Id == row.Id);

                        if (existingrow == null)
                        {
                            // Insert new city
                            PositionDataEntity newPos = new PositionDataEntity
                            {
                                Code = row.PosCode,
                                Descr = row.PosDescr,
                                DepartId = dbContext.Department.SingleOrDefault(b => b.Code == row.DepartCode).Id,
                                IsDeleted = false

                            };

                            dbContext.Position.Add(newPos);
                        }
                        else if (existingrow != null)
                        {
                            // Update existing city
                            var existingPos = dbContext.Position.Single(c => c.Id == row.Id);

                            existingPos.Code = row.PosCode;
                            existingPos.Descr = row.PosDescr;
                            existingPos.DepartId = dbContext.Department.SingleOrDefault(b => b.Code == row.DepartCode).Id;
                            existingPos.IsDeleted = row.IsDeleted;

                        }
                    }


                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "SavePositionData");
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
                            FlatData.City = new CityData();


                            FlatData.Id = int.Parse(reader["AirportID"].ToString());
                            FlatData.Code = reader["AirportCode"].ToString();
                            FlatData.Descr = reader["AirportDescr"].ToString();

                            FlatData.IsDeleted = bool.Parse(reader["IsDeleted"].ToString());


                            FlatData.City.CityId = int.Parse(reader["CityId"].ToString());
                            FlatData.City.CityCode = reader["CityCode"].ToString();
                            FlatData.City.CityDescr = reader["CityDescr"].ToString();
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
    }
}
