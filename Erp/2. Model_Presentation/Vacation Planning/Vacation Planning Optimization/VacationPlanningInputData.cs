using Erp.Model.Enums;
using Erp.Model.Interfaces;
using Erp.Model.Thesis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.V_Proxeiro.Vacation_Planning
{
    public class VacationPlanningInputData : RecordBaseModel
    {
        #region General 

        //Id Πειράματος
        public int VPId { get; set; }
        //Kωδικός Πειράματος
        public string VPCode { get; set; }
        //Περιγραφη Πειράματος
        public string VPDescr { get; set; }
        //Περιγραφη Πειράματος
        public BasicEnums.EmployeeType EmployeeType { get; set; }
        //Περιγραφη Πειράματος
        public BasicEnums.VPLogicType VPLogicType { get; set; }
        //Περιγραφη Πειράματος
        public int MaxSatisfiedBids { get; set; }
        //Περιγραφη Πειράματος
        public int SeparValue { get; set; }
        //Περιγραφη Πειράματος
        public ReqScheduleInfoData Schedule { get; set; }
        //Περιγραφη Πειράματος
        public ObservableCollection<EmployeeData> Employees { get; set; }

        //Βοηθητική λίστα με τις ημερομηνίες του προβλήματος 
        public DateTime[] Dates { get; set; }
        //Περιγραφη Πειράματος
        public string[] DatesStr { get; set; }
        #endregion

        #region Παράμετροι Βελτιστοποίησης

        //LLt στη μορφοποίηση : Αριθμός υπαλλήλων που επιτρέπεται να βρίσκονται σε άδεια την ημέρα t, tϵT
        public Dictionary<int, int> LL_Dict { get; set; }

        //Ei στη μορφοποίηση:  Ημέρες Άδειας που δικαιούται ο υπάλληλος i, iϵI
        public Dictionary<int, int> MaxD_Dict { get; set; }

        //JBi στη μορφοποιήση : Αριθμός Προτάσεων άδειας υπαλλήλου 
        public Dictionary<int, int> N_Dict { get; set; }

        //NDBij στη μορφοποίηση : Ημέρες άδειας της πρότασης άδειας  j του υπαλλήλου i iϵI, jϵJ  .
        //Αν ο τύπος άδειας είναι   Min-Max τότε οι ημέρες άδειας  = Μέγιστες ήμερες άδειας , 
        public Dictionary<(int, int), int> NDays_Dict { get; set; }

        //DFij στη μορφοποίηση : Ημερομηνία έναρξης της πρότασης άδειας j του υπαλλήλου  i, iϵI, jϵJ
        public Dictionary<(int, int), int> DateFrom_Dict { get; set; }

        //DΤij στη μορφοποίηση : Ημερομηνία λήξης της πρότασης άδειας j του υπαλλήλου  i, iϵI, jϵJ
        public Dictionary<(int, int), int> DateTo_Dict { get; set; }

        //RBij στη μορφοποίηση : Αριθμός Non-Specific αδειών  της πρότασης άδειας  j του υπαλλήλου  i, iϵI, jϵJ
        public Dictionary<(int, int), int> RBids_Dict { get; set; }

        //ZBijrz Αριθμός  Specific αδειών της non-specific άδειας r της πρότασης άδειας j του υπαλλήλου i, iϵI, jϵJ 
        public Dictionary<(int, int, int), int> ZBids_Dict { get; set; }

        #region Extra

        public int MaxLeaveBids { get; set; }
        public int MaxNonSpecific { get; set; }

        #endregion

        #endregion

    }


}
