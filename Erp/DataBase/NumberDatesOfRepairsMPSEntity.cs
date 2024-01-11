using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase
{
    public class NumberDatesOfRepairsMPSEntity
    {

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "int")]
        public int? NumberOfRepairs { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? RepairDate { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string RepairDateStr { get; set; }
        public int? MachId { get; set; }
        public int? MPSId { get; set; }

        [ForeignKey("MachId")]
        public virtual MachineDataEntity Machine { get; set; }

        [ForeignKey("MPSId")]
        public virtual MPSInputDataEntity MPSinput { get; set; }

    }
}
