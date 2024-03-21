using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase.Τhesis
{
    public class LanguageDataEntity
    {
        [Key]
        public int LId { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string LCode { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string LDescr { get; set; }

        [Column(TypeName = "bool")]
        public bool IsDeleted { get; set; }
    }
}
