using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Motherland.BasicFiles
{
    public class PositionData : RecordBaseModel
    {
        private int _Id { get; set; }
        private string _Code { get; set; }
        private string _Descr { get; set; }
        private int _DepartId { get; set; }
        private string _DepartCode { get; set; }
        private DepartmentData _Department { get; set; }

        public int Id
        {
            get { return _Id; }
            set { _Id = value; OnPropertyChanged("Id"); }
        }


        public string PosCode
        {
            get { return _Code; }
            set { _Code = value; OnPropertyChanged("PosCode"); }
        }


        public string PosDescr
        {
            get { return _Descr; }
            set { _Descr = value; OnPropertyChanged("PosDescr"); }
        }

        public int DepartId
        {
            get { return _DepartId; }
            set { _DepartId = value; OnPropertyChanged("DepartId"); }
        }
        public string DepartCode
        {
            get { return _DepartCode; }
            set { _DepartCode = value; OnPropertyChanged("DepartCode"); }
        }

        public DepartmentData Department
        {
            get { return _Department; }
            set { _Department = value; OnPropertyChanged("Department"); }
        }
    }

}
