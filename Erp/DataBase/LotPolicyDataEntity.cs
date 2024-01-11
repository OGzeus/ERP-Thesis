using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase
{
    public class LotPolicyDataEntity
    {
        [Key]
        public int LotPolicyId { get; set; }


        [Column(TypeName = "varchar(50)")]
        public string LotPolicyCode { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string LotPolicyDescr { get; set; }

        [Column(TypeName = "int")]
        public int? LeadTime { get; set; }



        [Column(TypeName = "float")]
        public double? BatchSize { get; set; }

        [Column(TypeName = "int")]
        public int? Period { get; set; }


        [Column(TypeName = "bit")]
        public bool? MainPolicy { get; set; }


        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }

        public int ItemId { get; set; } // This should be here

        [ForeignKey("ItemId")]
        public virtual ItemDataEntity Item { get; set; }
    }
}
