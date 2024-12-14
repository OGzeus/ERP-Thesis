using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Erp.DataBase.Motherland
{
    public class DepartmentDataEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Code { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Descr { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsDeleted { get; set; }

    }
}
