using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase
{
    public class BomDataEntity
    {
        [Key]
        [Column(TypeName = "int")]
        public int Id { get; set; }

        [Required]
        [ForeignKey("MainItem")]
        [Column(TypeName = "int")]
        public int ItemId { get; set; }

        [Required]
        [ForeignKey("BomItem")]
        [Column(TypeName = "int")]
        public int ComponentId { get; set; }

        [Column(TypeName = "float(5,2)")]
        public float Percentage { get; set; }



        public virtual ItemDataEntity MainItem { get; set; }
        public virtual ItemDataEntity BomItem { get; set; }
    }
}