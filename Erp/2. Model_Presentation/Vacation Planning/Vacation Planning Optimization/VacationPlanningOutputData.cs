using Erp.Model.Enums;
using Erp.Model.Interfaces;
using Erp.Model.Thesis.VacationPlanning;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.V_Proxeiro.Vacation_Planning
{
    public class VacationPlanningOutputData : RecordBaseModel
    {
        #region Optimization Data Output

        //Τιμή Αντικειμενικής συνάρτησης
        public double ObjValue { get; set; }
        //Καρτέλα Αποτελεσμάτων : Πλάνο Yij 
        public ObservableCollection<VPYijResultsData> VPYijResultsDataGrid { get; set; }
        //Καρτέλα Αποτελεσμάτων : Πλάνο Yijrz 
        public ObservableCollection<VPYijResultsData> VPYijrzResultsDataGrid { get; set; }
        //Καρτέλα Αποτελεσμάτων : Πλάνο Xit 
        public ObservableCollection<VPXijResultsData> VPXitResultsDataGrid { get; set; }

        //Καρτέλα Αποτελεσμάτων : RLLt στην μορφοποίηση
        public ObservableCollection<VPXiResultData> VPRLLtResultsDataGrid { get; set; }

        //Καρτέλα Αποτελεσμάτων : Entiltement στην μορφοποίηση
        public ObservableCollection<EmployeeData> EmpLeaveStatusData { get; set; }

        #endregion

        #region Extra
        public BasicEnums.EmployeeType EmployeeType { get; set; }
        public BasicEnums.VPLogicType VPLogicType { get; set; }
        public string[] Dates { get; set; }
        #endregion
    }

}
