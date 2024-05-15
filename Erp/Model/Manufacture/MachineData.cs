using Erp.Model.BasicFiles;
using Erp.Model.Enums;
using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Manufacture
{
    public class MachineData : RecordBaseModel
    {
        private int _MachID;
        private string _MachCode;
        private string _MachDescr;
        private FactoryData _Factory;
        private DateTime _LastMaintenance;
        private DateTime _NextMaintenance;
        private int _TotalOperatingHours;
        private float _FailureRate;
        private int _ProductionRate;
        private float _EfficiencyRate;
        private float _AverageRepairTime;
        private int _NumberOfFailures;
        private BasicEnums.MachType _MachineType;
        private int _ModelYear;
        private DateTime _DateInstalled;
        private BasicEnums.MachStatus _Status;
        private bool _PrimaryModel;
        private int _NumberOfMachines;
        private int _NumberOfRepairsMPS;

        private bool _AddMachine;


        public bool AddMachine
        {
            get { return _AddMachine; }
            set { _AddMachine = value; OnPropertyChanged("AddMachine"); }
        }

        public bool PrimaryModel
        {
            get { return _PrimaryModel; }
            set { _PrimaryModel = value; OnPropertyChanged("PrimaryModel"); }
        }
        public int NumberOfMachines
        {
            get { return _NumberOfMachines; }
            set { _NumberOfMachines = value; OnPropertyChanged("NumberOfMachines"); }
        }
        public int MachID
        {
            get { return _MachID; }
            set { _MachID = value; OnPropertyChanged("MachID"); }
        }
        public string MachCode
        {
            get { return _MachCode; }
            set { _MachCode = value; OnPropertyChanged("MachCode"); }
        }

        public string MachDescr
        {
            get { return _MachDescr; }
            set { _MachDescr = value; OnPropertyChanged("MachDescr"); }
        }

        public FactoryData Factory
        {
            get { return _Factory; }
            set { _Factory = value; OnPropertyChanged("Factory"); }
        }

        public DateTime LastMaintenance
        {
            get { return _LastMaintenance; }
            set { _LastMaintenance = value; OnPropertyChanged("LastMaintenance"); }
        }

        public DateTime NextMaintenance
        {
            get { return _NextMaintenance; }
            set { _NextMaintenance = value; OnPropertyChanged("NextMaintenance"); }
        }

        public int TotalOperatingHours
        {
            get { return _TotalOperatingHours; }
            set { _TotalOperatingHours = value; OnPropertyChanged("TotalOperatingHours"); }
        }

        public float FailureRate
        {
            get { return _FailureRate; }
            set { _FailureRate = value; OnPropertyChanged("FailureRate"); }
        }

        public int ProductionRate
        {
            get { return _ProductionRate; }
            set { _ProductionRate = value; OnPropertyChanged("ProductionRate"); }
        }

        public float EfficiencyRate
        {
            get { return _EfficiencyRate; }
            set { _EfficiencyRate = value; OnPropertyChanged("EfficiencyRate"); }
        }

        public float AverageRepairTime
        {
            get { return _AverageRepairTime; }
            set { _AverageRepairTime = value; OnPropertyChanged("AverageRepairTime"); }
        }

        public int NumberOfFailures
        {
            get { return _NumberOfFailures; }
            set { _NumberOfFailures = value; OnPropertyChanged("NumberOfFailures"); }
        }

        public BasicEnums.MachType MachineType
        {
            get { return _MachineType; }
            set { _MachineType = value; OnPropertyChanged("MachineType"); }
        }

        public int ModelYear
        {
            get { return _ModelYear; }
            set { _ModelYear = value; OnPropertyChanged("ModelYear"); }
        }

        public DateTime DateInstalled
        {
            get { return _DateInstalled; }
            set { _DateInstalled = value; OnPropertyChanged("DateInstalled"); }
        }

        public BasicEnums.MachStatus Status
        {
            get { return _Status; }
            set { _Status = value; OnPropertyChanged("Status"); }
        }


    }
}
