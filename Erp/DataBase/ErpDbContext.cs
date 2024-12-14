using Erp.DataBase.Motherland;
using Erp.DataBase.Τhesis;
using Erp.Model;
using MahApps.Metro.Controls;
using Microsoft.EntityFrameworkCore;
using System;


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

        public DbSet<CityDataEntity> City { get; set; }
        public DbSet<PrefectureEntity> Prefecture { get; set; }
        public DbSet<CountryDataEntity> Country { get; set; }

        public DbSet<VPYijzResultsDataEntity> VPYijzResults { get; set; }
        public DbSet<AircraftDataEntity> Aircrafts { get; set; }
        public DbSet<EMPLanguagesDataEntity> EmpLanguages { get; set; }
        public DbSet<LanguageDataEntity> Language { get; set; }
        public DbSet<CertificationsDataEntity> Certifications { get; set; }
        public DbSet<Log> Loge { get; set; }

        #endregion

        #endregion

        #region Motherland
        public DbSet<DepartmentDataEntity> Department { get; set; }

        public DbSet<PositionDataEntity> Position { get; set; }


        #endregion
        #region Database Related Commands
        public static DbContextOptions<ErpDbContext> DbOptions { get; set; }
        public ErpDbContext(DbContextOptions<ErpDbContext> options)
          : base(options ?? throw new ArgumentNullException(nameof(options), "DbContextOptions must be initialized before creating a new ErpDbContext instance."))
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Log>().HasKey(l => l.Id);
        }

        #endregion



    }
}