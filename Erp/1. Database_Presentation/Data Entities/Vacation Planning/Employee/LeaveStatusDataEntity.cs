using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBasePresenation
{
    public class LeaveStatusDataEntity
    {
        [Key]
        public int LSId { get; set; }

        [Column(TypeName = "int")]
        public int? Total { get; set; }

        [Column(TypeName = "int")]
        public int? Used { get; set; }

        [Column(TypeName = "int")]
        public int? CurrentBalance { get; set; }

        [Column(TypeName = "int")]
        public int? ProjectedBalance { get; set; }

        public int? EmpId { get; set; }

        [ForeignKey("EmpId")]
        public virtual EmployeeDataEntity Employee { get; set; }
    }
}
