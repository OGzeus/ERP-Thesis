using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase
{
    public  class ItemProcessDataEntity
    {
        [Key]
        public int ProcessFlowId { get; set; }

        public int? ItemId { get; set; }
        public int? MachineId { get; set; }

        [Column(TypeName = "int")]
        public int? MachineOrder { get; set; }

        [Column(TypeName = "decimal(18, 3)")] 
        public float? ProductionTime { get; set; }

      

        [ForeignKey("ItemId")]
        public virtual ItemDataEntity Item { get; set; }

        [ForeignKey("MachineId")]
        public virtual MachineDataEntity Machine { get; set; }
    }
}
