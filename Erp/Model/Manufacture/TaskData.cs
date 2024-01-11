using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Manufacture
{
    public class TaskData
    {
        public int Id { get; set; }
        public string TaskCode { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public int DurationDays { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
