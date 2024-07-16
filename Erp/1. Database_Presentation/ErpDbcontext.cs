using Erp.DataBase.Τhesis;
using Erp.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using Erp.Model;

namespace Erp._1._Database_Presentation
{
    public class ErpDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        #region Database 
        public DbSet<EmployeeDataEntity> Employees { get; set; }
        public DbSet<LeaveBidsDataEntity> LeaveBids { get; set; }
        public DbSet<LeaveStatusDataEntity> LeaveStatus { get; set; }
        public DbSet<ReqScheduleInfoDataEntity> ReqScheduleInfo { get; set; }
        public DbSet<ReqScheduleRowsDataEntity> ReqScheduleRows { get; set; }
        public DbSet<VPInputDataEntity> VPInput { get; set; }
        public DbSet<CountryDataEntity> Country { get; set; }
        public DbSet<PrefectureEntity> Prefecture { get; set; }
        public DbSet<CityDataEntity> City { get; set; }
        public DbSet<AirportsDataEntity> Airports { get; set; }
        public DbSet<FlightLegsDataEntity> FlightLegs { get; set; }
        public DbSet<FlightRoutesDataEntity> FlightRoutes { get; set; }
        public DbSet<CSInputDataEntity> CSInput { get; set; }
 

        #endregion

        #region Database Related Commands
        public static DbContextOptions<ErpDbContext> DbOptions { get; set; }
        public ErpDbContext(DbContextOptions<ErpDbContext> options)
          : base(options ?? throw new ArgumentNullException(nameof(options), 
              "DbContextOptions must be initialized before creating a " +
              "new ErpDbContext instance."))
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemDataEntity>().HasKey(i => i.ItemId);
            modelBuilder.Entity<Log>().HasKey(l => l.Id);
        }
        #endregion
    }
}
