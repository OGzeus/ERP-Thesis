using Erp.ViewModel;
using OfficeOpenXml;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Erp.Model;
using Erp.Repositories;
using FontAwesome.Sharp;
using System.Net.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Collections.ObjectModel;
using Erp.Model.BasicFiles;
using Syncfusion.Windows.Shared;
using Erp.Model.Suppliers;
using Erp.View;
using System.Windows.Markup;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices.ComTypes;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using DevExpress.DashboardWeb.Native;
using Erp.Model.Inventory;
using Erp.ViewModel.Inventory;
using static IronPython.Modules.PythonDateTime;
using Erp.Model.Manufacture;
using System.Windows.Forms;
using System.Drawing;
using static Erp.ViewModel.Data_Analytics.MRPVisualisationViewModel;
using Erp.Model.Data_Analytics;
using DevExpress.Utils;
using Erp.Model.Customers;
using IronPython.Runtime;
using Erp.DataBase;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Erp.Model.Enums;
using Microsoft.Office.Interop.Excel;
using static IronPython.Runtime.Profiler;
using Gurobi;
using Erp.Model.Inventory.InvControl_ConstantDemand;
using Microsoft.Scripting.Hosting;
using Erp.Model.Inventory.InvControl_TimeVaryingDemand;
using System.Windows.Controls;
using Deedle;
using System.Drawing.Drawing2D;
using System.Data;
using Microsoft.Identity.Client;
using System.Web.UI.WebControls;
using Syncfusion.Windows.Controls.Input;
using Erp.Model.Data_Analytics.Forecast;
using OxyPlot;
using Erp.Model.Manufacture.MPS;
using Syncfusion.Windows.Controls.Gantt;
using DevExpress.Data.Helpers;
using System.Windows.Media;
using static IronPython.Modules.PythonIterTools;
using ControlzEx.Standard;
using static Erp.Model.Enums.BasicEnums;
using System.Globalization;
using System.Reflection.Emit;
using DevExpress.XtraReports.Native;
using Erp.Model.Manufacture.MRP;
using Syncfusion.Windows.Controls;

namespace Erp.CommonFiles
{

    public class CommonFunctions : RepositoryBase
    {
        public DbContextOptions<ErpDbContext> options = new DbContextOptionsBuilder<ErpDbContext>()
            .UseSqlServer("Server=DESKTOP-F2TG0LU\\SQLEXPRESS;Database=ERPDatabase;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True")
            .Options;

        public static void LogError(Exception ex, string methodName, string additionalInfo = "")
        {
            using (var dbContext = new ErpDbContext(ErpDbContext.DbOptions))
            {
                dbContext.Loge.Add(new Log
                {
                    ExceptionType = ex.GetType().ToString(),
                    ExceptionMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                    Source = ex.Source,
                    MethodName = methodName,
                    OccurredAt = DateTime.Now,
                    AdditionalInfo = additionalInfo
                });

                dbContext.SaveChanges();
            }
        }

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
                    FilterStr = String.Format(@"and IsDeleted = @ShowDeleted");

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
    L.IsDeleted,
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
City.Latitude,City.IsDeleted
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
                using (var dbContext = new ErpDbContext(ErpDbContext.DbOptions))
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
                                Quantity = (float)row.Quantity,
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
                            existingStock.Quantity = (float)row.Quantity;

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
 SELECT Rmaster.ItemId,Rmaster.ItemCode,Rmaster.ItemDescr,Rmaster.MesUnit,Rmaster.ItemType,Rmaster.Assembly,Stock.Quantity 
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
                        stockdata.Quantity = float.Parse(reader["Quantity"].ToString());

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
And Rmaster.AssemblyNumber<=2";

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

                            stockdata.Quantity = 0;
                            stockdata.StockItemFlag = false;
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
                    var MainMRPForecast= MRPForecastquery.SingleOrDefault();
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
                LogError(ex, "SaveForecastData", "Notes");
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

                        if (existingrow == null)
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

                            if (existingrow.Demand != row.Demand)
                            {
                                existingrow.Demand = row.Demand;
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

                        data.MRPID = Convert.ToInt32(reader["MPSID"]);
                        data.MRPCode = reader["MPSCODE"].ToString();
                        data.MRPDescr = reader["MPSDESCR"].ToString();
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
                        existingMRP.ORDERSDATEFROM = flatData.OrdersDateFrom;
                        existingMRP.ORDERSDATETO = flatData.OrdersDateTo;
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
        public bool CalculateMRP(MRPInputData Data)
        {
            var Items = Data.Items.OrderByDescending(item => item.AssemblyNumber).ToList();
            var TotalDemandDict = Data.TotalDemandDict;
            var SubItems = Data.Bom;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                foreach (var item in Items)
                {
                    if (item.OutputOrderFlag == true)
                    {
                        var DemandList = Data.Forecast.DemandForecast
                            .Where(a => a.Item.ItemCode == item.ItemCode)
                            .Select(a => a.Demand)
                            .ToList();

                        TotalDemandDict.Add("Demand For: " + item.ItemCode.ToString(), DemandList);
                    }
                    if (item.AssemblyNumber < 2)
                    {
                        #region 1st Part
                        command.Parameters.AddWithValue("@ItemId", item.ItemId);

                        command.CommandText = string.Format(@"Select ItemId,ComponentId,Percentage From Bom
                                                            Where Bom.ComponentId =@ItemId 
                                                            Order By ItemId");

                        using (var reader = command.ExecuteReader())
                        {
                            SubItems = new ObservableCollection<BomData>();
                            while (reader.Read())
                            {
                                var BomItem = new BomData();

                                BomItem.FinalItemId = int.Parse(reader["ItemId"].ToString());
                                BomItem.BomItem.ItemId = int.Parse(reader["ComponentId"].ToString());
                                BomItem.BomPercentage = float.Parse(reader["Percentage"].ToString());

                                SubItems.Add(BomItem);
                            }
                        }

                        #endregion
                        #region 2nd Part
                        foreach(var row in SubItems)
                        {
                            command.Parameters.AddWithValue("@SelectedItemId", row.BomItem.ItemId);

                            command.CommandText = string.Format(@"select top 1 * 
                                                                from MrpResults
                                                                where itemid =@SelectedItemId
                                                                Order by id desc");
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {


                                    //TotalDemandDict.Add("Requirements From: " + item.ItemCode.ToString(), DemandList);
                                }
                            }
                        }

                        #endregion

                    }


                    #region Calculate MRP
                    var StartingStock = Data.Inventory.StockData
                        .Where(row => row.StockItem.ItemId == item.ItemId)
                        .Select(row => row.Quantity)
                        .FirstOrDefault(); 
                    var LeadTime = item.LotPolicy.LeadTime;

                    var sizeOfDemand = Data.Forecast.NumberOfBuckets;
                    decimal[,] demandArray = new decimal[1, sizeOfDemand];


                    List<List<decimal>> demands = TotalDemandDict.Values.ToList();

                    // Fill demand_array with demand values
                    for (int i = 0; i < TotalDemandDict.Count; i++)
                    {
                        List<decimal> demandList = demands[i];
                        for (int j = 0; j < demandList.Count; j++)
                        {
                            demandArray[0, j] = demandList[j];
                        }

                        double[,] data = new double[TotalDemandDict.Count + 1, sizeOfDemand];
                        Array.Copy(demandArray, 0, data, 1, sizeOfDemand);
                    }

                    // MRP calculations




                    #endregion

                }



                return true;
            }


        }

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
                command.CommandText = @"SELECT * FROM MrpResults";

                using (var reader = command.ExecuteReader())
                {
                    int totalColumns = reader.FieldCount;

                    while (reader.Read())
                    {
                        MrpResultData data = new MrpResultData
                        {
                            Id = int.Parse(reader["Id"].ToString()),
                            RowDescr = reader["Row Descr"].ToString(),
                            ItemId = int.Parse(reader["ItemId"].ToString()),
                            Quantities = new List<float>()
                        };

                        for (int i = 3; i < totalColumns; i++) // loop through the remaining columns
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
        #region Forecast

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

                                make[i, j] = model.AddVar(0.0, GRB.INFINITY, 0.0, GRB.INTEGER, "make_" + Dates[i] + "_" + Products[j]);
                                store[i, j] = model.AddVar(0.0, ub: max_inventory[Products[j]], GRB.INFINITY, GRB.INTEGER, "store_" + Dates[i] + "_" + Products[j]);

                                var upperboundsales = max_sales[key];

                                sell[i, j] = model.AddVar(0.0, ub: upperboundsales, GRB.INFINITY, GRB.INTEGER, "sell_" + Dates[i] + "_" + Products[j]);

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

                                objective.AddTerm(productProfit, make[i, j]); // profit
                                objective.AddTerm(holdingCost[productCode], store[i, j]); // holding cost
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

                                objective.AddTerm(productProfit, make[i, j]); // profit
                                objective.AddTerm(holdingCost[productCode], store[i, j]); // holding cost
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



    }
}
