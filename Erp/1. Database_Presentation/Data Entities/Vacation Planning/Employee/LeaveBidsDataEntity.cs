using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBasePresenation
{
    public class LeaveBidsDataEntity
    {
        [Key]
        public int BidId { get; set; }
        public int? EmpId { get; set; }
        public int? SceduleId { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string BidCode { get; set; }

        [Column(TypeName = "int")]
        public int? PriorityLevel { get; set; }

        [Column(TypeName = "varchar(35)")]
        public string BidType { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateFrom { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateTo { get; set; }

        [Column(TypeName = "varchar(35)")]
        public string DateFromStr { get; set; }

        [Column(TypeName = "varchar(35)")]
        public string DateToStr { get; set; }

        [Column(TypeName = "int")]
        public int? NumberOfDays { get; set; }

        [Column(TypeName = "int")]
        public int? NumberOfDaysMin { get; set; }

        [Column(TypeName = "int")]
        public int? NumberOfDaysMax { get; set; }

        [Column(TypeName = "bool")]
        public bool IsDeleted { get; set; }

        [ForeignKey("EmpId")]
        public virtual EmployeeDataEntity Employees { get; set; }

        [ForeignKey("SceduleId")]
        public virtual ReqScheduleInfoDataEntity ReqSchedule { get; set; }
    }
}
