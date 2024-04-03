using Erp.Helper;
using Erp.Model;
using Erp.Model.BasicFiles;
using Erp.Model.Inventory.InvControl_TimeVaryingDemand;
using Erp.Model.Manufacture.MPS;
using Erp.Model.Thesis;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Data;
using System.Windows.Input;

namespace Erp.CommonFiles
{
    public class F7Common 
    {

        CommonFunctions CommonFunctions = new CommonFunctions();


        #region Thesis

        public F7Data F7VacationPlanning(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetVPInputData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "VPCode", HeaderText = "VP Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "VPDescr", HeaderText = "VP Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "EmployeeType", HeaderText = "Employee Type" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "VPLogicType", HeaderText = "VP Logic Type" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "MaxSatisfiedBids", HeaderText = "Max Satisfied Bids" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "SeparValue", HeaderText = "Separation Value" });

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Schedule.ReqCode", HeaderText = "Schedule Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Schedule.ReqDescr", HeaderText = "Schedule Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Schedule.DateFrom", HeaderText = "Date From" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Schedule.DateTo", HeaderText = "Date To" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });


            GridData.F7key = "VPCode";
            return GridData;
        }
        public F7Data F7ReqSchedule (bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetReqScheduleInfoData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ID", HeaderText = "Id" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ReqCode", HeaderText = "Schedule Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ReqDescr", HeaderText = "Schedule Description" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Notes", HeaderText = "Notes" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "DateFrom", HeaderText = "Date From" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "DateTo", HeaderText = "Date To" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "LimitLineFixed", HeaderText = "Limit Line Fixed" });

            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "MainSchedule", HeaderText = " Main Schedule" });

            GridData.F7key = "ReqSchedule";
            return GridData;
        }
        public F7Data F7Employee(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetEmployeeData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Code", HeaderText = "Employee Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Descr", HeaderText = "Employee Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "FirstName", HeaderText = "First Name " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "LastName", HeaderText = "Last Name " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Gender", HeaderText = "Gender" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Position", HeaderText = "Position" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Seniority", HeaderText = "Seniority" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "TotalFlightHours", HeaderText = "Total Flight Hours" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });


            GridData.F7key = "Employee";
            return GridData;
        }

        public F7Data F7Certification(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetCertificationData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Code", HeaderText = "Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Descr", HeaderText = "Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CertPosition", HeaderText = "Certification Position" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ValidityTimeBucket", HeaderText = "Validity TimeBucket" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ValidityPeriod", HeaderText = "Validity Period" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "DateFrom", HeaderText = "Date From" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "DateTo", HeaderText = "Date To" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });



            GridData.F7key = "Certification";
            return GridData;
        }

        public F7Data F7Airports(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetAirportsData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Code", HeaderText = "Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Descr", HeaderText = "Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "City.CityCode", HeaderText = "City Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "City.CityDescr", HeaderText = "City Descr" });

            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });



            GridData.F7key = "Airport";
            return GridData;
        }
        #endregion

        public F7Data F7RoutesCity(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetCityData(false).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "MPSId", HeaderText = "MPS ID" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "MPSCode", HeaderText = "MPS Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "MPSDescr", HeaderText = "MPS Descr" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });


            GridData.F7key = "MPSCode";
            return GridData;
        }

        public F7Data F7MachRepairInput(MPSInputData inputData)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = new ObservableCollection<MachineRepairData>();

            if (inputData.NumberDatesOfRepairs)
            {
                Data = inputData.MachRepairDateData;

                GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Mach.MachCode" , IsReadOnly = true, HeaderText = "Machine Code"  });
                GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Mach.MachDescr", IsReadOnly = true, HeaderText = "Machine Description" });
                GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "NumberOfRepairsMPS" , AllowEditing= true, HeaderText = "Number Of Repairs"  });
                GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "RepairDateStr", IsReadOnly = true, HeaderText = "Repair Date String" });
                GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "RepairDate", IsReadOnly = true, HeaderText = "Repair Date" });

            }
            else if (inputData.NumberOfRepairsOnly)
            {
                Data = inputData.MachRepairOnlyData;
                GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Mach.MachCode", IsReadOnly = true, HeaderText = "Machine Code" });
                GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Mach.MachDescr"   ,IsReadOnly = true, HeaderText = "Machine Description" });
                GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "NumberOfRepairsMPS", AllowEditing = true, HeaderText = "Number Of Repairs" });
            }

            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);
            GridData.F7key = "MachRepairInput";
            return GridData;
        }

        public F7Data F7MPSCode(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetMPSData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "MPSId", HeaderText = "MPS ID" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "MPSCode", HeaderText = "MPS Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "MPSDescr", HeaderText = "MPS Descr" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });


            GridData.F7key = "MPSCode";
            return GridData;
        }

        public F7Data F7MRPCode(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetMRPData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "MRPID", HeaderText = "MRP ID" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "MRPCode", HeaderText = "MRP Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "MRPDescr", HeaderText = "MRP Descr" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "ForecastFlag", HeaderText = "Forecast Flag" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "OrdersFlag", HeaderText = "Orders Flag" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });


            GridData.F7key = "MRPCode";
            return GridData;
        }
        public F7Data F7MRPForecast(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetMRPForecast(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ID", HeaderText = "Id" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ForCode", HeaderText = "Forecast Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ForDescr", HeaderText = "Forecast Description" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Notes", HeaderText = "Notes" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "TimeBucket", HeaderText = "Time Bucket" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "PeriodType", HeaderText = "Period Type" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "HoursPerTimeBucket", HeaderText = "Hours Per TimeBucket" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "DateFrom", HeaderText = "Date From" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "DateTo", HeaderText = "Date To" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "MRPForecast", HeaderText = " Main MRP Forecast" });

            GridData.F7key = "MRPForecast";
            return GridData;
        }
        public F7Data F7Forecast(bool  ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetForecastInfoData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ID", HeaderText = "Id" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ForCode", HeaderText = "Forecast Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ForDescr", HeaderText = "Forecast Description" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Notes", HeaderText = "Notes" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "TimeBucket", HeaderText = "Time Bucket" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "PeriodType", HeaderText = "Period Type" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "HoursPerTimeBucket", HeaderText = "Hours Per TimeBucket" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "DateFrom", HeaderText = "Date From" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "DateTo"  , HeaderText = "Date To" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "MRPForecast", HeaderText = " Main MRP Forecast" });

            GridData.F7key = "MPSForecast";
            return GridData;
        }

        public F7Data F7Machine(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetMachineChooserData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "MachCode", HeaderText = "Machine Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "MachDescr", HeaderText = "Machine Description" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "MachineType", HeaderText = "Machine Type" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "PrimaryModel", HeaderText = "Primary Model" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Status", HeaderText = "Status" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Factory.Code", HeaderText = "Factory Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Factory.Descr", HeaderText = "Factory Description" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "LastMaintenance", HeaderText = "Last Maintenance" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "NextMaintenance", HeaderText = "Next Maintenance" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });

            GridData.F7key = "Mach";
            return GridData;
        }
        public F7Data F7LotPolicy(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetLotPolicyChooserData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Item.ItemCode" });

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "LotPolicyId", HeaderText = "Lot Policy Id" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Code", HeaderText = "Lot Policy Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Descr", HeaderText = "Lot Policy Description" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "LeadTime", HeaderText = "Lead Time" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "BatchSize", HeaderText = "Batch Size" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Period", HeaderText = "Period" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "MainPolicy", HeaderText = "Main Policy" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });


            GridData.F7key = "CustomerOrder";
            return GridData;
        }
        public F7Data F7SpecificLotPolicy(ItemData Item)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetLotPolicySpecificData(Item).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Item.ItemCode", HeaderText = "Item Code"  });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "LotPolicyId", HeaderText = "Lot Policy Id" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Code", HeaderText = "Lot Policy Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Descr", HeaderText = "Lot Policy Description" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "LeadTime", HeaderText = "Lead Time" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "BatchSize", HeaderText = "Batch Size" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Period", HeaderText = "Period" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "MainPolicy", HeaderText = "Main Policy" });


            GridData.F7key = "CustomerOrder";
            return GridData;
        }
        public F7Data F7Factory()
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetFactoryChooserData().ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Code" , HeaderText = "Factory Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Descr" , HeaderText = "Factory Description" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "City.CityDescr", HeaderText = "City Description" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "City.CountryDescr", HeaderText = "Country Description" });

            GridData.F7key = "Factory";
            return GridData;
        }
        public F7Data F7MrpResults()
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetMrpResultsData().ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ItemId", HeaderText = "Item Id" ,IsHidden = true });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Item.ItemCode", HeaderText = "Item Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Item.Assembly", HeaderText = "Assembly" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "RowDescr", HeaderText = "Row Descritpion" });
            // Assuming that all items have the same number of values in the list
            int valuesCount = Data.FirstOrDefault()?.Quantities.Count ?? 0;
            CreateDynamicColumns(GridData.SfGridColumns, valuesCount);



            GridData.F7key = "MrpResults";
            return GridData;
        }

        public F7Data F7InvControlInfDataResults(ObservableCollection<TimeVaryingInvResultsData> InputData)
        {

            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = InputData.ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ItemId", HeaderText = "Item Id " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Item.ItemCode", HeaderText = "Item Code " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "RowDescr", HeaderText = "Row Description" });
            // Assuming that all items have the same number of values in the list
            // Ensure all Values lists have the same length
            if (InputData.Select(data => data.Values.Count).Distinct().Count() > 1)
            {
                // Get the maximum count of values across all the data
                int maxValuesCount = InputData.Max(data => data.Values.Count);

                foreach (var data in InputData)
                {
                    // Fill missing elements with "adio"
                    while (data.Values.Count < maxValuesCount)
                    {
                        data.Values.Add("NULL");
                    }
                }
            }
            int valuesCount = Data.FirstOrDefault()?.Values.Count ?? 0;

            CreateDynamicColumns2(GridData.SfGridColumns, valuesCount);



            GridData.F7key = "InvControlInfDataResults";
            return GridData;
        }

        private void CreateDynamicColumns(Columns columns, int count)
        {
            for (int i = 1; i <= count; i++)
            {
                columns.Add(new GridTextColumn()
                {
                    MappingName = $"Quantities[{i - 1}]",
                    HeaderText = i.ToString()
                });
            }
        }
        private void CreateDynamicColumns2(Columns columns, int count)
        {
            for (int i = 1; i <= count; i++)
            {
                columns.Add(new GridTextColumn()
                {

                    MappingName = $"Values[{i - 1}]",
                    HeaderText = i.ToString()
                });
            }
        }
        public F7Data F7CustomerOrder(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetCOrderChooserData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CustOrderId", HeaderText = "Order Id" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "OrderStatus", HeaderText = "Order Status" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Customer.Code", HeaderText = "Customer Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Customer.Descr", HeaderText = "Customer Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Customer.City.CityDescr", HeaderText = "City Descr" });
            GridData.SfGridColumns.Add(new GridDateTimeColumn() { MappingName = "DateCreated", HeaderText = "Date Created" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });


            GridData.F7key = "CustomerOrder";
            return GridData;
        }
        public F7Data F7Customer_PriceList(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetPriceListChooserData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Code", HeaderText = "PriceList Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Descr", HeaderText = "PriceList Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Retail", HeaderText = "Retail" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Wholesale", HeaderText = "Wholesale" });
            GridData.SfGridColumns.Add(new GridDateTimeColumn() { MappingName = "DateStart", HeaderText = "Date Start" });
            GridData.SfGridColumns.Add(new GridDateTimeColumn() { MappingName = "DateEnd", HeaderText = "Date End" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });


            GridData.F7key = "PriceList";
            return GridData;
        }
        public F7Data F7CustomersOfPriceList(int PriceListId)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetCustomersOfPriceListData(PriceListId).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Code", HeaderText = "Customer Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Descr", HeaderText = "Customer Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Email", HeaderText = "Email" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Phone", HeaderText = "Phone" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Adress", HeaderText = "Address" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CustomerType", HeaderText = "Customer Type" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "City.CityCode", HeaderText = "City Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "City.CityDescr", HeaderText = "City Descr " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Country.CountryCode", HeaderText = "Country Code " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Country.CountryDescr", HeaderText = "Country Descr" });


            GridData.F7key = "PriceList";
            return GridData;
        }



        public F7Data F7Customer(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetCustomerData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Code", HeaderText = "Customer Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Descr", HeaderText = "Customer Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Email", HeaderText = "Email" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Phone", HeaderText = "Phone" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Adress", HeaderText = "Address" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CustomerType", HeaderText = "Customer Type" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "City.CityCode" ,HeaderText="City Code"});
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "City.CityDescr", HeaderText = "City Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Country.CountryCode", HeaderText = "Country Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Country.CountryDescr", HeaderText = "Country Descr" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });


            GridData.F7key = "Customer";
            return GridData;
        }

        public F7Data F7Inventory(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetInventoryData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "InvCode", HeaderText = "Inventory Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "InvDescr", HeaderText = "Inventory Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Location", HeaderText = "Location" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Capacity", HeaderText = "Capacity" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });

            GridData.F7key = "InvCode";
            return GridData;
        }
        public F7Data F7Item(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetItemData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ItemId", HeaderText = "Item Id" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ItemCode", HeaderText = "Item Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ItemDescr", HeaderText = "Item Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "MesUnit", HeaderText = "UOM" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ItemType", HeaderText = "Item Type" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Assembly", HeaderText = "Assembly" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CanBeProduced", HeaderText = "Can Be Produced" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "InputOrderFlag", HeaderText = "Can Be Sold" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "OutputOrderFlag", HeaderText = "Can Be Ordered" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });

            GridData.F7key = "ItemCode";
            return GridData;
        }

        public F7Data F7ItemMPSDiagrams()
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetItemsForSaleData().ToList();

            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ItemId", HeaderText = "Item Id" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ItemCode", HeaderText = "Item Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ItemDescr", HeaderText = "Item Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "MesUnit", HeaderText = "UOM" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ItemType", HeaderText = "Item Type" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Assembly", HeaderText = "Assembly" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CanBeProduced", HeaderText = "Can Be Produced" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "InputOrderFlag", HeaderText = "Can Be Sold" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "OutputOrderFlag", HeaderText = "Can Be Ordered" });

            GridData.F7key = "ItemCode";
            return GridData;
        }
        public F7Data F7ItemMPSInput()
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();

            var List = new List<string>();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(List);


            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ItemId", HeaderText = "Item Id" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ItemCode", HeaderText = "Item Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ItemDescr", HeaderText = "Item Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ItemType", HeaderText = "Item Type" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "HoldingCost", HeaderText = "Holding Cost" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "MaxInventory", HeaderText = "Max Inventory" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "StoreTarget", HeaderText = "Store Target" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Profit", HeaderText = "Profit" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "SalesPrice", HeaderText = "Sales Price" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ManufacturingCost", HeaderText = "Manufacturing Cost" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ShortageCost", HeaderText = "Shortage Cost" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "LeadTime", HeaderText = "Lead Time" });


            GridData.F7key = "ItemMPSInput";
            return GridData;
        }
        public F7Data F7ItemMRPInput()
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();

            var List = new List<string>();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(List);


            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ItemId", HeaderText = "Item Id" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ItemCode", HeaderText = "Item Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ItemDescr", HeaderText = "Item Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ItemType", HeaderText = "Item Type" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "HoldingCost", HeaderText = "Holding Cost" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "MaxInventory", HeaderText = "Max Inventory" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "StoreTarget", HeaderText = "Store Target" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Profit", HeaderText = "Profit" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "SalesPrice", HeaderText = "Sales Price" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ManufacturingCost", HeaderText = "Manufacturing Cost" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ShortageCost", HeaderText = "Shortage Cost" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "LeadTime", HeaderText = "Lead Time" });


            GridData.F7key = "ItemMRPInput";
            return GridData;
        }

        public F7Data F7MRPItemsInfo()
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();

            var List = new List<string>();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(List);

            // Assuming GridData is an instance of SfDataGrid
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "StockItem.ItemCode", HeaderText = "Item Code", AllowEditing = false });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "StockItem.ItemDescr", HeaderText = "Item Description", AllowEditing = false });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "StockItem.ItemType", HeaderText = "Item Type", AllowEditing = false });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "StockItem.Assembly", HeaderText = "Assembly", AllowEditing = false });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Quantity", HeaderText = "Quantity", AllowEditing = true });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "StockItem.LotPolicy.Code", HeaderText = "Lot Policy Code", AllowEditing = true });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "StockItem.LotPolicy.LeadTime", HeaderText = "Lead Time", AllowEditing = false });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "StockItem.LotPolicy.BatchSize", HeaderText = "Batch Size", AllowEditing = false });

            #region MultiColumnDropDownList

            //var LotPolicyList = new List<LotPolicyData>();
            //LotPolicyList = CommonFunctions.GetLotPolicyChooserData(false).ToList();
            //var LotPolicyMultiDropDownColumn= new GridMultiColumnDropDownList()
            //{
            //    MappingName = "StockItem.LotPolicy.Code",
            //    HeaderText = "LotPolicy Code",
            //    DisplayMember = "Code",
            //    AutoGenerateColumns = true,
            //    ItemsSource =LotPolicyList,
            //    AllowEditing = true
            //};
            //GridData.SfGridColumns.Add(LotPolicyMultiDropDownColumn);

            #endregion
            GridData.F7key = "MRPStock";
            return GridData;
        }
        public F7Data  F7SupplierFrom() 
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetSupplierInfoChooserData().ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "SupplierCode", HeaderText = "Supplier Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "SupplierDescr", HeaderText = "Supplier Description" });

            GridData.F7key = "Supplier";
            return GridData;
        }

        public F7Data F7SupplierTo()
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetSupplierInfoChooserData().ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);


            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "SupplierCode", HeaderText = "Supplier Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "SupplierDescr", HeaderText = "Supplier Description" });

            GridData.F7key = "SupplierTo";
            return GridData;
        }

        public F7Data F7Country(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetCountryData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CountryCode", HeaderText = "Country Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CountryDescr", HeaderText = "Country Descr" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });

            GridData.F7key = "CountryFrom";
            return GridData;
        }


        public F7Data F7City(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetCityData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CityId", HeaderText = "City Id " ,IsHidden = true});

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CityCode", HeaderText = "City Code " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CityDescr", HeaderText = "City Descr " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "PrefDescr", HeaderText = "Prefecture Descr " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CountryDescr", HeaderText = "Country Descr" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });

            GridData.F7key = "City";
            return GridData;
        }

    }
}
