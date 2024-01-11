using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.DataBase
{
    public class ForecastInfoDataEntity
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string ForCode { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string ForDescr { get; set; }


        [Required]
        [Column(TypeName = "varchar(150)")]
        public string Notes { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string TimeBucket { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string PeriodType { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateFrom { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateTo { get; set; }

        [Column(TypeName = "int")]
        public int? PeriodNum { get; set; }

        [Column(TypeName = "int")]
        public int? HoursPerTimeBucket { get; set; }

        [Column(TypeName = "int")]
        public int? NumberOfBuckets { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }

        [Column(TypeName = "bit")]
        public bool? MRPForecast { get; set; }
    }
}
