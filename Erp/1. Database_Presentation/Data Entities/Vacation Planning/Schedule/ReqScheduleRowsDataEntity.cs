using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBasePresenation
{
    public class ReqScheduleRowsDataEntity
    {
        [Key]
        public int ReqId { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string ReqCode { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string Position { get; set; }

        [Column(TypeName = "int")]
        public int? LimitLine { get; set; }


        [Column(TypeName = "varchar(40)")]
        public string DateStr { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? Date { get; set; }
    }
}
