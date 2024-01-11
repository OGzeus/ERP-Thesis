using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Inventory.InvControl_ConstantDemand
{
    public class BasicEOQData : INotifyPropertyChanged
    {
        #region Basic Info
        private double _Demand; //Unit variable cost

        private double _C; //Unit variable cost

        private double _K; // Fixed order cost

        private double _I; // Cost (interest rate) of capital

        private double _H; // Inventory Holding Costs

        private double _Q; // Quantity


        private double _T; // Cycle length

        private double _N; // Frequency of orders

        public double Q
        {
            get { return _Q; }
            set
            {
                if (_Q != value)
                {
                    _Q = value;
                    INotifyPropertyChanged("Q");
                }
            }
        }
        public double Demand
        {
            get { return _Demand; }
            set
            {
                if (_Demand != value)
                {
                    _Demand = value;
                    INotifyPropertyChanged("Demand");
                }
            }
        }
        public double C
        {
            get { return _C; }
            set
            {
                if (_C != value)
                {
                    _C = value;
                    INotifyPropertyChanged(nameof(C));
                    INotifyPropertyChanged(nameof(H));
                }
            }
        }
        public double K
        {
            get { return _K; }
            set { _K = value; INotifyPropertyChanged("K"); }
        }
        public double I
        {
            get { return _I; }
            set
            {
                if (_I != value)
                {
                    _I = value;
                    INotifyPropertyChanged(nameof(I));
                    INotifyPropertyChanged(nameof(H));
                }
            }
        }
        public double H
        {
            get { return _I * _C; }
            set
            {
                if (value != _I * _C)
                {
                    _I = value / _C;
                    INotifyPropertyChanged(nameof(H));
                    INotifyPropertyChanged(nameof(I));
                }
            }
        }
        public double T
        {
            get { return _T; }
            set { _T = value; OnPropertyChanged("T"); }
        }
        public double N
        {
            get { return _N; }
            set { _N = value; OnPropertyChanged("N"); }
        }

        private double _ObjFunc;
        public double ObjFunc
        {
            get { return _ObjFunc; }
            set
            {
                _ObjFunc = value;
                INotifyPropertyChanged("Filter");
            }
        }

        #endregion

        #region Qmin,Qmax,Tmin,TMax
        private bool _QminmaxConstr;
        private float? _Qmin;
        private float? _Qmax;
        private float? _Tmin;
        private float? _Tmax;

        public bool QminmaxConstr
        {
            get { return _QminmaxConstr; }
            set { _QminmaxConstr = value; OnPropertyChanged("QminmaxConstr"); }
        }
        public float? Qmin
        {
            get { return _Qmin; }
            set { _Qmin = value; OnPropertyChanged("Qmin"); }
        }
        public float? Qmax
        {
            get { return _Qmax; }
            set { _Qmax = value; OnPropertyChanged("Qmax"); }
        }
        public float? Tmin
        {
            get { return _Tmin; }
            set { _Tmin = value; OnPropertyChanged("Tmin"); }
        }
        public float? Tmax
        {
            get { return _Tmax; }
            set { _Tmax = value; OnPropertyChanged("Tmax"); }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void INotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        #endregion
    }
}
