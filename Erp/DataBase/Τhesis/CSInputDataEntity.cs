using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.DataBase.Τhesis
{
    public class CSInputDataEntity
    {
        [Key]
        public int CSID { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string CSCODE { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string CSDESCR { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string EMPLOYEETYPE { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateFrom { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateTo { get; set; }

        [Column(TypeName = "int")]
        public int? RoutesPenalty { get; set; }

        [Column(TypeName = "int")]
        public int? BoundsPenalty { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }


    }
}
