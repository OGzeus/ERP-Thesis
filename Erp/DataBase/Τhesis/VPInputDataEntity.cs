using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Erp.DataBase.Τhesis
{
    public class VPInputDataEntity
    {
        [Key]
        public int VPID { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string VPCODE { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string VPDESCR { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string EMPLOYEETYPE { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string VPLOGICTYPE { get; set; }

        [Column(TypeName = "int")]
        public int? MaxSatisfiedBids { get; set; }


        [Column(TypeName = "int")]
        public int? SeparValue { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }

        public int? SCHEDULEID { get; set; }

        [ForeignKey("SCHEDULEID")]
        public virtual ReqScheduleInfoDataEntity Schedule { get; set; }

    }
}
