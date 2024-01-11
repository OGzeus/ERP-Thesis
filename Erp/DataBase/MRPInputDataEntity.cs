using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase
{
    public class MRPInputDataEntity
    {
        [Key]
        public int MRPID { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string MRPCODE { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string MRPDescr { get; set; }

        [Column(TypeName = "bit")]
        public bool? FORECASTFLAG { get; set; }

        [Column(TypeName = "bit")]
        public bool? ORDERSFLAG { get; set; }

        [Column(TypeName = "bit")]
        public bool? SELECTEDITEMS { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ORDERSDATEFROM { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ORDERSDATETO { get; set; }


        [Column(TypeName = "bit")]
        public bool? ISDELETED { get; set; }

        public int? FORECASTID { get; set; }
        public int? INVID { get; set; }

        [ForeignKey("FORECASTID")]
        public virtual ForecastInfoDataEntity Forecast { get; set; }

        [ForeignKey("INVID")]
        public virtual InventoryDataEntity Inventory { get; set; }
    }
}