using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Erp.DataBase.Τhesis;

namespace Erp.DataBase
{
    public class FlightRoutesDataEntity
    {
        [Key]
        public int FlightRouteId { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string Code { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? EndDate { get; set; }


        [Column(TypeName = "float")]
        public float? FlightTime { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }
        public int CityFrom { get; set; }
        public int CityTo { get; set; }


        [ForeignKey("CityFrom")]
        public virtual CityDataEntity City1 { get; set; }


        [ForeignKey("CityTo")]
        public virtual CityDataEntity City2 { get; set; }

    }
}
