using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase
{
    public class ItemDataEntity
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string ItemCode { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string ItemDescr { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string MesUnit { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string ItemType { get; set; }

        [Column(TypeName = "varchar(15)")]
        public string Assembly { get; set; }

        [Column(TypeName = "int")]
        public int AssemblyNumber { get; set; }


        [Column(TypeName = "bit")]
        public bool? CanBeProduced { get; set; }

        [Column(TypeName = "bit")]
        public bool? InputOrderFlag { get; set; }

        [Column(TypeName = "bit")]
        public bool? OutputOrderFlag { get; set; }

        [Column(TypeName = "float")]
        public double? BomPercentage { get; set; }

        [Column(TypeName = "float")]
        public float Profit { get; set; }

        [Column(TypeName = "float")]
        public float SalesPrice { get; set; }

        [Column(TypeName = "float")]
        public float ManufacturingCost { get; set; }

        [Column(TypeName = "float")]
        public float HoldingCost { get; set; }

        [Column(TypeName = "float")]
        public float ShortageCost { get; set; }

        [Column(TypeName = "float")]
        public float LeadTime { get; set; }

        [Column(TypeName = "bool")]
        public bool IsDeleted { get; set; }
        [Required]
        public int LotPolicyId { get; set; }

        [ForeignKey("LotPolicyId")]
        public virtual LotPolicyDataEntity LotPolicy { get; set; }
    }
}
