using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Erp.DataBase
{
    public class StockDataEntity
    {

        [Key]
        [Column(TypeName = "int")]
        public int StockId { get; set; }

        [Required]
        [ForeignKey("Inventory")]
        [Column(TypeName = "int")]
        public int InvId { get; set; }

        [Required]
        [ForeignKey("StockItem")]
        [Column(TypeName = "int")]
        public int ItemId { get; set; }

        [Column(TypeName = "Decimal")]
        public float Quantity { get; set; }

        [Column(TypeName = "Decimal")]
        public float InvMax { get; set; }

        [Column(TypeName = "Decimal")]
        public float InvMin { get; set; }

        public virtual InventoryDataEntity Inventory { get; set; }
        public virtual ItemDataEntity StockItem { get; set; }
    }
}
