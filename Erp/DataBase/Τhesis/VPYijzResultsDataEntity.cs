using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Erp.DataBase.Τhesis
{
    public class VPYijzResultsDataEntity
    {
        [Key]
        public int VPResultsId { get; set; }

        [Column(TypeName = "int")]
        public int ReplicationNumber { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string DateFromStr { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string DateToStr { get; set; }

        [Column(TypeName = "int")]
        public int NumberOfDays { get; set; }

        [Column(TypeName = "int")]
        public int NumberOfDaysMin { get; set; }

        [Column(TypeName = "int")]
        public int NumberOfDaysMax { get; set; }
        [Column(TypeName = "varchar(10)")]
        public string Yij { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string Yijr { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string Yijrz { get; set; }
        [Column(TypeName = "bit")]
        public bool? Confirmed { get; set; }
        [Required]
        [ForeignKey("VPInput")]
        [Column(TypeName = "int")]
        public int VPID { get; set; }

        public virtual VPInputDataEntity VPInput { get; set; }

        public int EmpId { get; set; }

        [ForeignKey("EmpId")]
        public virtual EmployeeDataEntity Employee { get; set; }
        public int BidId { get; set; }

        [ForeignKey("BidId")]
        public virtual LeaveBidsDataEntity LeaveBid { get; set; }
    }

}
