using Erp.Model.Interfaces;
using Erp.Model.Thesis.VacationPlanning;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.V_Proxeiro.Vacation_Planning.Vacation_Planning_Optimization
{
public class VPCGInputData : RecordBaseModel
{
    #region General
    public string[] Dates { get; set; }
    public ObservableCollection<VPXiResultData> VP_Xt_ResultsDataGrid { get; set; }

    #endregion

    #region Dictionaries

    //REi στη μορφοποίηση : Υπολειπόμενες ήμερες άδειας δυο δικαιούται ο υπάλληλος i, iϵI
    //να βρίσκονται σε άδεια την ημέρα t, tϵT
    public Dictionary<int, int> LeaveDays { get; set; }

       
    //RRLLt στη μορφοποίηση  : Υπολειπόμενος Αριθμός υπαλλήλων που επιτρέπεται να βρίσκονται σε άδεια την ημέρα t, tϵT
    public Dictionary<int, int> LLiDict { get; set; }

    #endregion
}


}
