using Erp.Model.Enums;
using Erp.Model.Interfaces;
using Erp.Model.Thesis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.V_Proxeiro.Schedule
{
    public class ReqScheduleInfoData : RecordBaseModel
    {
        #region General
        public int ID { get; set; }
        public string ReqCode { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public BasicEnums.EmployeeType Position { get; set; }
        public bool MainSchedule { get; set; }

        #endregion

        #region Schedule Rows Related
        public int LimitLineFixed { get; set; }
        public ObservableCollection<ReqScheduleRowsData> ReqScheduleRowsData { get; set; }

        #endregion

        #region Extra
        public string ReqDescr { get; set; }
        public string DateFromStr { get; set; }
        public string DateToStr { get; set; }
        public string Notes { get; set; }

        #endregion

    }
}
