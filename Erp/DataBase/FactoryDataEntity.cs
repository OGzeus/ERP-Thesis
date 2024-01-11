using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase
{
    public class FactoryDataEntity
    {
        [Key]
        public int FactoryID { get; set; }

        public int? CityID { get; set; }

        [Column(TypeName = "int")]
        public int? ProductionCapacity { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string Code { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string Descr { get; set; }



        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }


        [ForeignKey("CityID")]
        public virtual CityDataEntity City { get; set; }
    }
}