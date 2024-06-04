using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase
{
    public class CityDataEntity
    {
        [Key]
        public int CityId { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string CityCode { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string CityDescr { get; set; }


        [Column(TypeName = "double")]
        public double? Longitude { get; set; }

        [Column(TypeName = "double")]
        public double? Latitude { get; set; }

        [Column(TypeName = "int")]
        public int? Population { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }

        public int PrefId { get; set; }

        [ForeignKey("PrefId")]
        public virtual PrefectureEntity Prefecture { get; set; }
    }
}
