using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations; 
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model
{
    public class Log
    {
        [Key]
        public int Id { get; set; }
        public string ExceptionType { get; set; }
        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public string Source { get; set; }
        public string MethodName { get; set; }
        public DateTime OccurredAt { get; set; }
        public string AdditionalInfo { get; set; }
    }
}
