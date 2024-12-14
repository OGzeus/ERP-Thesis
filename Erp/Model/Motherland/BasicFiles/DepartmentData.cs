using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Motherland.BasicFiles
{
    public class DepartmentData : RecordBaseModel
    {
        private int _Id { get; set; }
        private string _Code { get; set; }

        private string _Descr { get; set; }

        public int Id
        {
            get { return _Id; }
            set { _Id = value; OnPropertyChanged("Id"); }
        }


        public string DepartCode
        {
            get { return _Code; }
            set { _Code = value; OnPropertyChanged("DepartCode"); }
        }


        public string DepartDescr
        {
            get { return _Descr; }
            set { _Descr = value; OnPropertyChanged("DepartDescr"); }
        }

    }

}
