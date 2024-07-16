using Erp.Model.Enums;
using Erp.Model.Interfaces;
using Erp.Model.Thesis.CrewScheduling;
using Erp.Model.Thesis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.V_Proxeiro
{
    public class EmployeeData : RecordBaseModel
    {
        #region General 

        public int EmployeeId { get; set; }
        public string Code { get; set; }

        public BasicEnums.EmployeeType Position { get; set; }
        public int Seniority { get; set; }

        public EmployeeCR_Settings EmpCrSettings { get; set; }
        public AirportData BaseAirport { get; set; }
        #endregion

        #region LeaveBids Related
        public ReqScheduleInfoData MainSchedule { get; set; }
        public LeaveStatusData LeaveStatus { get; set; }
        public LeaveBidsData LeaveBidInfo { get; set; } // 1 apo ta 2 tha fugei
        public LeaveBidsDataStatic LeaveBidsInfoStatic { get; set; } // 1 apo ta 2 tha fugei 
        public ObservableCollection<LeaveBidsDataStatic> LeaveBidDataGridStatic { get; set; }

        #endregion

        #region Extra 

        public string Descr { get; set; }

        public CertificationData Certification { get; set; }

        public int Language { get; set; }

        public float TotalFlightHours { get; set; }

        public DateTime HireDate { get; set; }

        public string ContactNumber { get; set; }

        public string Email { get; set; }
        public string Address { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public BasicEnums.Gender Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public bool IsSelected { get; set; }

        #endregion

    }
}
