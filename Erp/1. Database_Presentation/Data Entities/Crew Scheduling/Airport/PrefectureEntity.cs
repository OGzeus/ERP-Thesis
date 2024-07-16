using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBasePresenation
{
    public class PrefectureEntity
    {
        [Key]
        public int PrefId { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string PrefCode { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string PrefDescr { get; set; }

        public int CountryId { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }

        [ForeignKey("CountryId")]
        public virtual CountryDataEntity Country { get; set; }
    }
}