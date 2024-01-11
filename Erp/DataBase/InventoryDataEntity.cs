using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase
{
    public class InventoryDataEntity
    {
        [Key]
        public int InvId { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string InvCode { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string InvDescr { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string Location { get; set; }

        [Column(TypeName = "float")]
        public double Capacity { get; set; }

        [Column(TypeName = "bit")]
        public bool? Production { get; set; }

        [Column(TypeName = "bit")]
        public bool? Packaging { get; set; }

        [Column(TypeName = "bit")]
        public bool? FinishedGoods { get; set; }

        [Column(TypeName = "bit")]
        public bool IsDeleted { get; set; }


    }
}
