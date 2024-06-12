using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.DataBase.Τhesis
{
    public class FlightRoutesDataEntity
    {
        [Key]
        public int FlightRouteId { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string Code { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string Descr { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? EndDate { get; set; }

        [Column(TypeName = "float")]
        public float? FlightTime { get; set; }

        [Column(TypeName = "float")]
        public float? GroundTime { get; set; }

        [Column(TypeName = "float")]
        public float? TotalTime { get; set; }

        [Column(TypeName = "int")]
        public int? Complement_Captain { get; set; }

        [Column(TypeName = "int")]
        public int? Complement_FO { get; set; }

        [Column(TypeName = "int")]
        public int? Complement_Cabin_Manager { get; set; }

        [Column(TypeName = "int")]
        public int? Complement_Flight_Attendant { get; set; }
        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }
        public int AirportId { get; set; }


        [ForeignKey("AirportId")]
        public virtual AirportsDataEntity Airport { get; set; }

    }
}
