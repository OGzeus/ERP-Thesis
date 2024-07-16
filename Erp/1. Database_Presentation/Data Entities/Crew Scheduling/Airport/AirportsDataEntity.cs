using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBasePresenation
{
    public class AirportsDataEntity
    {
        [Key]
        public int AirportID { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string AirportCode { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string AirportDescr { get; set; }

        [Column(TypeName = "bool")]
        public bool IsDeleted { get; set; }
        public int? CityId { get; set; }


        [ForeignKey("CityId")]
        public virtual CityDataEntity City { get; set; }
    }
}
