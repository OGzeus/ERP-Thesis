using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase
{
    public class NumberOfRepairsOnlyMPSEntity
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "int")]
        public int? NumberOfRepairs { get; set; }

        public int? MachId { get; set; }
        public int? MPSId { get; set; }


        [ForeignKey("MachId")]
        public virtual MachineDataEntity Machine { get; set; }

        [ForeignKey("MPSId")]
        public virtual MPSInputDataEntity MPSinput { get; set; }
    }
}
