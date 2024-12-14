using Erp.DataBase.Τhesis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.DataBase.Motherland
{
    public class MLEmployeeDataEntity
    {
        [Key]
        public int EmployeeID { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Code { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Descr { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string FirstName { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string LastName { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string Gender { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string ContactNumber { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string Email { get; set; }

        [Column(TypeName = "bool")]
        public bool IsDeleted { get; set; }
        public int? CityId { get; set; }
        public int? PositionId { get; set; }
        public int? DepartmentId { get; set; }

        [ForeignKey("CityId")]
        public virtual CityDataEntity City { get; set; }

        [ForeignKey("PositionId")]
        public virtual PositionDataEntity Position { get; set; }

    }
}
