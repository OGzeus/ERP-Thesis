using Erp.Model.Thesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.V_Proxeiro.Vacation_Planning.VP_Output
{
public class VPYijResultsData
{
    #region General

    // Υπάλληλος i
    public EmployeeData Employee { get; set; }
    // Προτίμηση άδειας j
    public LeaveBidsDataStatic LeaveBidData { get; set; }

    //Ονομα προτίμησης άδειας Yij
    public string Yij { get; set; }
    //Oνομα μεταβλητής απόφασης Rijr
    public string Rijr { get; set; }
    //Oνομα μεταβλητής Yijrz
    public string Yijrz { get; set; }
    //Ακριβής ημερομηνία παραγώμενης άδειας
    public string Date { get; set; }
    //Τιμη μεταβλητής απόφασης , χορήγηση άδειας
    public double YijrzFlag { get; set; }  

    #endregion

}

}
