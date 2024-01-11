using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase
{
    public class MPSInputDataEntity
    {
        [Key]
        public int MPSID { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string MPSCODE { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string MPSDESCR { get; set; }

        [Column(TypeName = "bit")]
        public bool? ItemsDefaultSettings { get; set; }

        [Column(TypeName = "bit")]
        public bool? ExistingSchedule { get; set; }

        [Column(TypeName = "bit")]
        public bool? NumberOfRepairsOnly { get; set; }

        [Column(TypeName = "bit")]
        public bool? NumberDatesOfRepairs { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }


        [Column(TypeName = "float")]
        public double? HoldingCost { get; set; }

        [Column(TypeName = "int")]
        public int? MaxInventory { get; set; }

        [Column(TypeName = "int")]
        public int? StoreTarget { get; set; }

        public int? FORECASTID { get; set; }
        public int? PriceListId { get; set; }

        [ForeignKey("FORECASTID")]
        public virtual ForecastInfoDataEntity Forecast { get; set; }

        [ForeignKey("PriceListId")]
        public virtual CPriceListEntity PriceList { get; set; }
    }
}
