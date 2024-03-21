using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.DataBase.Τhesis
{
    public class ReqScheduleInfoDataEntity
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string ReqCode { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string ReqDescr { get; set; }

        [Required]
        [Column(TypeName = "varchar(150)")]
        public string Notes { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateFrom { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateTo { get; set; }

        [Column(TypeName = "int")]
        public int? LimitLineFixed { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }

        [Column(TypeName = "bit")]
        public bool? MainSchedule { get; set; }
    }
}
