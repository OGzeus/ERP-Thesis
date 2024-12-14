using Erp.Model.BasicFiles;
using Erp.Model.Enums;
using Erp.Model.Interfaces;
using Erp.Model.Motherland.BasicFiles;
using Erp.Model.Thesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Motherland.Employee
{
    public class MLEmployeeData : RecordBaseModel
    {
        private int _EmployeeId { get; set; }

        public int EmployeeId
        {
            get { return _EmployeeId; }
            set { _EmployeeId = value; OnPropertyChanged("EmployeeId"); }
        }
        private string _Code { get; set; }
        public string Code
        {
            get { return _Code; }
            set { _Code = value; OnPropertyChanged("Code"); }
        }
        private string _Descr { get; set; }
        public string Descr
        {
            get { return _Descr; }
            set { _Descr = value; OnPropertyChanged("Descr"); }
        }
        private string _FirstName { get; set; }
        public string FirstName
        {
            get { return _FirstName; }
            set { _FirstName = value; OnPropertyChanged("FirstName"); }
        }
        private string _LastName { get; set; }
        public string LastName
        {
            get { return _LastName; }
            set { _LastName = value; OnPropertyChanged("LastName"); }
        }
        private string _ContactNumber { get; set; }
        public string ContactNumber
        {
            get { return _ContactNumber; }
            set { _ContactNumber = value; OnPropertyChanged("ContactNumber"); }
        }
        private string _Email { get; set; }
        public string Email
        {
            get { return _Email; }
            set { _Email = value; OnPropertyChanged("Email"); }
        }

        private BasicEnums.Gender _Gender { get; set; }
        public BasicEnums.Gender Gender
        {
            get { return _Gender; }
            set { _Gender = value; OnPropertyChanged("Gender"); }
        }
        private PositionData _Position { get; set; }
        public PositionData Position
        {
            get { return _Position; }
            set { _Position = value; OnPropertyChanged("Position"); }
        }
        private DepartmentData _Department { get; set; }
        public DepartmentData Department
        {
            get { return _Department; }
            set { _Department = value; OnPropertyChanged("Department"); }
        }
        private CityData _City { get; set; }
        public CityData City
        {
            get { return _City; }
            set { _City = value; OnPropertyChanged("City"); }
        }



    }
}
