using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase
{
    public class CPriceListEntity
    {
        [Key]
        public int PriceListId { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string PriceListCode { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string PriceListDescr { get; set; }

        [Column(TypeName = "bit")]
        public bool? Retail { get; set; }

        [Column(TypeName = "bit")]
        public bool? Wholesale { get; set; }


        [Column(TypeName = "datetime")]
        public DateTime? DateStart { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateEnd { get; set; }

    }
}