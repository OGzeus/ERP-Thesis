using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase
{
    public class CustomerOrderEntity
    {
        [Key]
        public string COrderId { get; set; }

        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual CustomerDataEntity Customer { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string CCartId { get; set; }



        [Column(TypeName = "varchar(255)")]
        public string Incoterms { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string OrderStatus { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string Notes { get; set; }

        public int CityId { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }

        [ForeignKey("CityId")]
        public virtual CityDataEntity City { get; set; }
    }
}
