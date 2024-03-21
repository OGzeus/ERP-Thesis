using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Erp.DataBase.Τhesis
{
    public class AircraftDataEntity
    {
        [Key]
        public int AircraftID { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string Code { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string Descr { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string AircraftType { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Model { get; set; }

        [Column(TypeName = "int")]
        public int Capacity { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LastMaintenanceDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NextMaintenanceDate { get; set; }

        [Column(TypeName = "bool")]
        public bool IsDeleted { get; set; }

        public int CurrentAirportID { get; set; }

        [ForeignKey("CurrentAirportID")]
        public virtual AirportsDataEntity CurrentAirport { get; set; }
    }
}
