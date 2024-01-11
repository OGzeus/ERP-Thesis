using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Erp.DataBase
{
    public class RoutesDataEntity
    {
        [Key]
        public int RouteId { get; set; }

        [Column(TypeName = "double")]
        public double? Distance { get; set; }

        [Column(TypeName = "double")]
        public double? AverageTravelingSpeed { get; set; }

        [Column(TypeName = "bit")]
        public bool? Twoway { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }

        public int CityFrom { get; set; }
        public int CityTo { get; set; }


        [ForeignKey("CityFrom")]
        public virtual CityDataEntity City1 { get; set; }


        [ForeignKey("CityTo")]
        public virtual CityDataEntity City2 { get; set; }
    }
}
