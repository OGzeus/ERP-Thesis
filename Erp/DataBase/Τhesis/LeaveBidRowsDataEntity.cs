using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase.Τhesis
{
    public class LeaveBidRowsDataEntity
    {
        [Key]
        public int RowId { get; set; }
        public int BidId { get; set; }
        public int EmpId { get; set; }
        public int ScheduleId { get; set; }

        [ForeignKey("BidId")]
        public virtual LeaveBidsDataEntity LeaveBids { get; set; }

        [ForeignKey("EmpId")]
        public virtual EmployeeDataEntity Employee { get; set; }

        [ForeignKey("ScheduleId")]
        public virtual ReqScheduleInfoDataEntity Schedule { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? Date { get; set; }

        [Column(TypeName = "varchar(35)")]
        public string DateStr { get; set; }



    }
}
