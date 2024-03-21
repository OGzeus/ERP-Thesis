using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase.Τhesis
{
    public class EMPLanguagesDataEntity
    {
        [Key]
        [Column(TypeName = "int")]
        public int ElId { get; set; }

        [Required]
        [ForeignKey("Employees")]
        [Column(TypeName = "int")]
        public int EmpId { get; set; }

        [Required]
        [ForeignKey("Language")]
        [Column(TypeName = "int")]
        public int LanguageId { get; set; }



        public virtual EmployeeDataEntity Employees { get; set; }
        public virtual LanguageDataEntity Language { get; set; }
    }
}
