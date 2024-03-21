using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase.Τhesis
{
    public class CertificationsDataEntity
    {
        [Key]
        public int CertID { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string Code { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string Descr { get; set; }

        [Column(TypeName = "int")]
        public int? ValidityPeriod { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string ValidityTimeBucket { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string CertPosition { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateFrom { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateTo { get; set; }

        [Column(TypeName = "bool")]
        public bool IsDeleted { get; set; }

    }
}
