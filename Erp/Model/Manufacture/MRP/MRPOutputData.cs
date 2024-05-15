using Erp.Model.Interfaces;
using Erp.Model.Manufacture.MPS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Manufacture.MRP
{
    public class MRPOutputData : RecordBaseModel
    {
        private double _ObjValue;

        public double ObjValue
        {
            get { return _ObjValue; }
            set
            {
                _ObjValue = value;
                OnPropertyChanged(nameof(ObjValue));
            }
        }

        private ObservableCollection<DecisionVariableX> _XData;

        public ObservableCollection<DecisionVariableX> XData
        {
            get { return _XData; }
            set
            {
                _XData = value;
                OnPropertyChanged(nameof(XData));
            }
        }
        private ObservableCollection<DecisionVariableY> _YData;

        public ObservableCollection<DecisionVariableY> YData
        {
            get { return _YData; }
            set
            {
                _YData = value;
                OnPropertyChanged(nameof(YData));
            }
        }
        private ObservableCollection<DecisionVariablesInvStatus> _InvData;

        public ObservableCollection<DecisionVariablesInvStatus> InvData
        {
            get { return _InvData; }
            set
            {
                _InvData = value;
                OnPropertyChanged(nameof(InvData));
            }
        }

        #region Diagram Data

        private DiagramsMRPData _Diagram1;
        public DiagramsMRPData Diagram1
        {
            get { return _Diagram1; }
            set
            {
                _Diagram1 = value;
                OnPropertyChanged(nameof(Diagram1));
            }
        }


        private DiagramsMRPData _Diagram2;
        public DiagramsMRPData Diagram2
        {
            get { return _Diagram2; }
            set
            {
                _Diagram2 = value;
                OnPropertyChanged(nameof(Diagram2));
            }
        }
        #endregion


        private string[] dates;
        public string[] Dates
        {
            get { return dates; }
            set { dates = value; OnPropertyChanged("Dates"); }
        }

    }
}
