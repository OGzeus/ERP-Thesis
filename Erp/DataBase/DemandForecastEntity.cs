using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase
{
    public class DemandForecastEntity
    {
        [Key]
        public int ForecastId { get; set; }

        public int ItemId { get; set; }

        [ForeignKey("ItemId")]
        public virtual ItemDataEntity Item { get; set; }


        [Column(TypeName = "datetime")]
        public DateTime? Date { get; set; }

        [Column(TypeName = "Decimal")]
        public decimal? Demand { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string ForCode { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string DateStr { get; set; }
    }
}
