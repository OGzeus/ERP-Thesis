using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase
{
    public class CPriceListItemsEntity
    {
        [Key]
        public int Primaryid { get; set; }
        public int Id { get; set; }

        public int ItemId { get; set; }

        [ForeignKey("ItemId")]
        public virtual ItemDataEntity Item { get; set; }

        [Column(TypeName = "float")]
        public double UnitCost { get; set; }

        [Column(TypeName = "float")]
        public double Discount { get; set; }

        [Column(TypeName = "float")]
        public double Qmin { get; set; }
    }
}
