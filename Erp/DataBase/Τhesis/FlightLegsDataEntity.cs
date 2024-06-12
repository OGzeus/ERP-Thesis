using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.DataBase.Τhesis
{
    public class FlightLegsDataEntity
    {
        [Key]
        public int FlightLegId { get; set; }

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

        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }
        public int AirportFrom { get; set; }
        public int AirportTo { get; set; }

        [ForeignKey("AirportFrom")]
        public virtual AirportsDataEntity Airport1 { get; set; }

        [ForeignKey("AirportTo")]
        public virtual AirportsDataEntity Airport2 { get; set; }

    }
}
