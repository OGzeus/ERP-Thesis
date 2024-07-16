using Erp.DataBase.Τhesis;
using Erp.Model;
using Erp.Model.BasicFiles;
using Erp.Model.Customers;
using Erp.Model.Thesis.VacationPlanning;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.DataBase
{
    public class ErpDbContext : Microsoft.EntityFrameworkCore.DbContext
    {

        #region Thesis

        #region Vacation Planning

        #region Employee

        public DbSet<EmployeeDataEntity> Employees { get; set; }
        public DbSet<LeaveBidsDataEntity> LeaveBids { get; set; }
        public DbSet<LeaveStatusDataEntity> LeaveStatus { get; set; }
        #endregion

        #region Schedule

        public DbSet<ReqScheduleInfoDataEntity> ReqScheduleInfo { get; set; }
        public DbSet<ReqScheduleRowsDataEntity> ReqScheduleRows { get; set; }
        #endregion

        #region Vacation Planning Optimization

        public DbSet<VPInputDataEntity> VPInput { get; set; }
        #endregion

        #endregion

        #region Crew Scheduling

        #region Airport

        public DbSet<AirportsDataEntity> Airports { get; set; }
        #endregion

        #region Crew Scheduling Optimization

        public DbSet<CSInputDataEntity> CSInput { get; set; }
        #endregion

        #region Flights
        public DbSet<FlightRoutesDataEntity> FlightRoutes { get; set; }
        public DbSet<FlightLegsDataEntity> FlightLegs { get; set; }
        #endregion

        #endregion

        #region Extra
        public DbSet<VPYijzResultsDataEntity> VPYijzResults { get; set; }
        public DbSet<AircraftDataEntity> Aircrafts { get; set; }
        public DbSet<EMPLanguagesDataEntity> EmpLanguages { get; set; }
        public DbSet<LanguageDataEntity> Language { get; set; }
        public DbSet<CertificationsDataEntity> Certifications { get; set; }
        #endregion

        #endregion

        #region Database Related Commands
        public static DbContextOptions<ErpDbContext> DbOptions { get; set; }
        public ErpDbContext(DbContextOptions<ErpDbContext> options)
          : base(options ?? throw new ArgumentNullException(nameof(options), "DbContextOptions must be initialized before creating a new ErpDbContext instance."))
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemDataEntity>().HasKey(i => i.ItemId);
            modelBuilder.Entity<Log>().HasKey(l => l.Id);
        }

        #endregion

        #region Extra
        public DbSet<ItemDataEntity> Rmaster { get; set; }
        public DbSet<ItemProcessDataEntity> ProductionProcessFlow { get; set; }
        public DbSet<ForecastInfoDataEntity> ForecastInfo { get; set; }
        public DbSet<LotPolicyDataEntity> LotPolicy { get; set; }
        public DbSet<RoutesDataEntity> Routes { get; set; }

        public DbSet<DemandForecastEntity> DemandForecast { get; set; }
        public DbSet<InventoryDataEntity> Inventory { get; set; }
        public DbSet<StockDataEntity> Stock { get; set; }

        public DbSet<Log> Loge { get; set; }
        public DbSet<BomDataEntity> Bom { get; set; }
        public DbSet<CustomerDataEntity> Customer { get; set; }
        public DbSet<FactoryDataEntity> Factory { get; set; }
        public DbSet<MachineDataEntity> Machines { get; set; }

        public DbSet<MPSInputDataEntity> MPSInput { get; set; }
        public DbSet<NumberOfRepairsOnlyMPSEntity> NumberOfRepairsOnlyMPS { get; set; }
        public DbSet<NumberDatesOfRepairsMPSEntity> NumberDatesOfRepairsMPS { get; set; }
        public DbSet<MRPInputDataEntity> MRPInput { get; set; }
        public DbSet<CityDataEntity> City { get; set; }
        public DbSet<PrefectureEntity> Prefecture { get; set; }
        public DbSet<CountryDataEntity> Country { get; set; }
        public DbSet<CPriceListEntity> CPriceList { get; set; }
        public DbSet<CPriceListItemsEntity> CPriceListItems { get; set; }

        public DbSet<COrderCartEntity> COrder_Cart { get; set; }
        public DbSet<CustomerOrderEntity> CustomerOrder { get; set; }

        #endregion

    }
}