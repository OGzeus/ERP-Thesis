using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase
{
    public class MachineDataEntity
    {
        [Key]
        public int MachID { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string MachCode { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string MachDescr { get; set; }

        public int? FactoryID { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LastMaintenance { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NextMaintenance { get; set; }

        [Column(TypeName = "int")]
        public int? TotalOperatingHours { get; set; }

        [Column(TypeName = "float")]
        public float? FailureRate { get; set; }

        [Column(TypeName = "float")]
        public int? ProductionRate { get; set; }

        [Column(TypeName = "float")]
        public float? EfficiencyRate { get; set; }
      
        [Column(TypeName = "float")]
        public float? AverageRepairTime { get; set; }
       
        [Column(TypeName = "int")]
        public int? NumberOfFailures { get; set; }

        [Column(TypeName = "varchar(55)")]
        public string MachineType { get; set; }
       
        [Column(TypeName = "int")]
        public int? ModelYear { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateInstalled { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Status { get; set; }

        [Column(TypeName = "int")]
        public int? NumberOfMachines { get; set; }

        [Column(TypeName = "bit")]
        public bool? PrimaryModel { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }

        [ForeignKey("FactoryID")]
        public virtual FactoryDataEntity Factory { get; set; }
    }
}