using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.V_Proxeiro.Vacation_Planning.VP_Output
{
public class VPXitResultsData
{
    //Kωδικός Υπαλλήλου
    public EmployeeData Employee { get; set; }
       
    //Ονομα μεταβλητης αποφασης Xit
    public string Xit { get; set; }
       
    //Ημερομηνία
    public string Date { get; set; }

    //Τιμή μεταβλήτης απόφασης Xit
    public bool Granted{ get; set; } 
}

}
