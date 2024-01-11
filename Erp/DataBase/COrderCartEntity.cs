using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase
{
    public class COrderCartEntity
    {
        [Key]
        public int CCartId_PK { get; set; }

        public string CCartId { get; set; }

        public int ItemId { get; set; }

        [ForeignKey("ItemId")]
        public virtual ItemDataEntity Item { get; set; }

        [Column(TypeName = "float")]
        public double? Quantity { get; set; }

        public int CityId { get; set; }


        [ForeignKey("CityId")]
        public virtual CityDataEntity City { get; set; }


        [Column(TypeName = "datetime")]
        public DateTime? DeliveryDate { get; set; }

        [Column(TypeName = "float")]
        public double? UnitCost { get; set; }

        [Column(TypeName = "float")]
        public double? TotalCost { get; set; }

        [Column(TypeName = "float")]
        public double? UnitDiscount { get; set; }
    }
}
