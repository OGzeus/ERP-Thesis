using Erp.Model.Enums;
using Erp.Model.Interfaces;
using Erp.Model.Thesis.CrewScheduling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis
{
    public class EmployeeData : RecordBaseModel
    {

        #region Employee CrewScheduling Settings

        private EmployeeCR_Settings _EmpCrSettings { get; set; }
        public EmployeeCR_Settings EmpCrSettings
        {
            get { return _EmpCrSettings; }
            set { _EmpCrSettings = value; OnPropertyChanged("EmpCrSettings"); }
        }
        #endregion 

        private int _EmployeeId { get; set; }

        public int EmployeeId
        {
            get { return _EmployeeId; }
            set { _EmployeeId = value; OnPropertyChanged("EmployeeId"); }
        }
        private string _Code { get; set; }

        private string _Descr { get; set; }
        private string _FirstName { get; set; }
        private string _LastName { get; set; }
        private BasicEnums.Gender _Gender { get; set; }
        private DateTime _DateOfBirth { get; set; }
        private string _ContactNumber { get; set; }
        private string _Email { get; set; }
        private string _Address { get; set; }
        private BasicEnums.EmployeeType _Position { get; set; }
        private DateTime _HireDate { get; set; }
        private float _TotalFlightHours { get; set; }
        private int _Seniority { get; set; }
        private int _Language { get; set; }
        private AirportData _BaseAirport { get; set; }

        private CertificationData _Certification { get; set; }

        private LeaveBidsData _LeaveBidInfo { get; set; }
        private LeaveStatusData _LeaveStatus { get; set; }

        private ObservableCollection<LeaveBidsData> _LeaveBidDataGrid { get; set; }
        private LeaveBidsDataStatic _LeaveBidsInfoStatic;
        private ObservableCollection<LeaveBidsDataStatic> _LeaveBidDataGridStatic;

        private ObservableCollection<LeaveBidRowData> _LeaveBidRows;

        public ObservableCollection<LeaveBidRowData> LeaveBidRows
        {
            get { return _LeaveBidRows; }
            set { _LeaveBidRows = value; OnPropertyChanged("LeaveBidRows"); }
        }
        public LeaveBidsData LeaveBidInfo
        {
            get { return _LeaveBidInfo; }
            set { _LeaveBidInfo = value; OnPropertyChanged("LeaveBidInfo"); }
        }

        public LeaveStatusData LeaveStatus
        {
            get { return _LeaveStatus; }
            set { _LeaveStatus = value; OnPropertyChanged("LeaveStatus"); }
        }
        public ObservableCollection<LeaveBidsData> LeaveBidDataGrid
        {
            get { return _LeaveBidDataGrid; }
            set { _LeaveBidDataGrid = value; OnPropertyChanged("LeaveBidDataGrid"); }
        }
        public AirportData BaseAirport
        {
            get { return _BaseAirport; }
            set { _BaseAirport = value; OnPropertyChanged("BaseAirport"); }
        }

        public CertificationData Certification
        {
            get { return _Certification; }
            set { _Certification = value; OnPropertyChanged("Certification"); }
        }
        public int Seniority
        {
            get { return _Seniority; }
            set { _Seniority = value; OnPropertyChanged("Seniority"); }
        }
        public int Language
        {
            get { return _Language; }
            set { _Language = value; OnPropertyChanged("Language"); }
        }
        public float TotalFlightHours
        {
            get { return _TotalFlightHours; }
            set { _TotalFlightHours = value; OnPropertyChanged("TotalFlightHours"); }
        }

        public BasicEnums.EmployeeType Position
        {
            get { return _Position; }
            set { _Position = value; OnPropertyChanged("Position"); }
        }
        public DateTime HireDate
        {
            get { return _HireDate; }
            set { _HireDate = value; OnPropertyChanged("HireDate"); }
        }

        public string ContactNumber
        {
            get { return _ContactNumber; }
            set { _ContactNumber = value; OnPropertyChanged("ContactNumber"); }
        }
        public string Email
        {
            get { return _Email; }
            set { _Email = value; OnPropertyChanged("Email"); }
        }
        public string Address
        {
            get { return _Address; }
            set { _Address = value; OnPropertyChanged("Address"); }
        }
        public string Code
        {
            get { return _Code; }
            set { _Code = value; OnPropertyChanged("Code"); }
        }

        public string Descr
        {
            get { return _Descr; }
            set { _Descr = value; OnPropertyChanged("Descr"); }
        }

        public string FirstName
        {
            get { return _FirstName; }
            set { _FirstName = value; OnPropertyChanged("FirstName"); }
        }
        public string LastName
        {
            get { return _LastName; }
            set { _LastName = value; OnPropertyChanged("LastName"); }
        }

        public BasicEnums.Gender Gender
        {
            get { return _Gender; }
            set { _Gender = value; OnPropertyChanged("Gender"); }
        }
        public DateTime DateOfBirth
        {
            get { return _DateOfBirth; }
            set { _DateOfBirth = value; OnPropertyChanged("DateOfBirth"); }
        }


        public LeaveBidsDataStatic LeaveBidsInfoStatic
        {
            get { return _LeaveBidsInfoStatic; }
            set
            {
                _LeaveBidsInfoStatic = value;
                INotifyPropertyChanged(nameof(LeaveBidsInfoStatic));
            }
        }

        private ReqScheduleInfoData _MainSchedule;

        public ReqScheduleInfoData MainSchedule
        {
            get { return _MainSchedule; }
            set
            {
                _MainSchedule = value;
                INotifyPropertyChanged(nameof(MainSchedule));
            }
        }
        public ObservableCollection<LeaveBidsDataStatic> LeaveBidDataGridStatic
        {
            get { return _LeaveBidDataGridStatic; }
            set { _LeaveBidDataGridStatic = value; INotifyPropertyChanged("LeaveBidDataGridStatic"); }
        }
    }
}
