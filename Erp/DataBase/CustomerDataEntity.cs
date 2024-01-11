using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase
{
    public class CustomerDataEntity
    {
        [Key]
        public int CustomerId { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string CustomerCode { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string CustomerDescr { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Email { get; set; }

        [Column(TypeName = "int")]
        public int? Phone { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Adress { get; set; }
        public int? CityId { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string CustomerType { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string PostalCode { get; set; }

        public int? PriceListId { get; set; }

        [Column(TypeName = "bit")]
        public bool? PromptPayer { get; set; }

        [ForeignKey("CityId")]
        public virtual CityDataEntity City { get; set; }

        [ForeignKey("PriceListId")]
        public virtual CPriceListEntity CPriceList { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }
    }
}