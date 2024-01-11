using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Erp.DataBase
{
    public class CountryDataEntity
    {
        [Key]
        public int CountryId { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string CountryCode { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string CountryDescr { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }
    }
}