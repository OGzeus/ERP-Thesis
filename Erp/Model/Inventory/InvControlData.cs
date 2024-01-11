using Erp.Model.BasicFiles;
using Erp.Model.Enums;
using Erp.Model.Inventory.InvControl_ConstantDemand;
using Erp.Model.Inventory.InvControl_TimeVaryingDemand;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Inventory
{
    public class InvControlData : INotifyPropertyChanged
    {

        public enum ActivePanel
        {
            None,
            BasicEOQ,
            RefillTimeEOQ,
            PendingOrders,
            LostSales,
            Discount_Large_Orders,
            Multiple_Products_Single_Supplier,
            Multiple_Products_Multiple_Supplier,
            TimeVarying_Infinite_Capacity,
            TimeVarying_Finite_Capacity
        }

        private ActivePanel _activePanel = ActivePanel.None;
        public ActivePanel CurrentActivePanel
        {
            get { return _activePanel; }
            private set
            {
                if (_activePanel != value)
                {
                    _activePanel = value;
                    OnPropertyChanged("CurrentActivePanel");
                }
            }
        }
        private BasicEnums.DemandType _DemandType;
        public BasicEnums.DemandType DemandType
        {
            get { return _DemandType; }
            set
            {
                _DemandType = value;
                UpdateActivePanel();
                OnPropertyChanged("DemandType");
            }
        }

        #region Constant Demand
        private BasicEnums.ConstantDemandType _ConstantDemandType;
        public BasicEnums.ConstantDemandType ConstantDemandType
        {
            get { return _ConstantDemandType; }
            set
            {
                _ConstantDemandType = value;
                UpdateActivePanel();
                OnPropertyChanged("ConstantDemandType");
            }
        }



        private BasicEOQData _EOQData;
        private RefillEOQData _RefillEOQData;
        private PendingOrdersData _PendingOrdersData;
        private LostSalesData _LostSalesData;


        public BasicEOQData EOQData
        {
            get { return _EOQData; }
            set
            {
                if (_EOQData != value)
                {
                    _EOQData = value;
                    RaisePropertyChanged(nameof(EOQData));
                }
            }
        }
        public RefillEOQData RefillEOQData
        {
            get { return _RefillEOQData; }
            set
            {
                if (_RefillEOQData != value)
                {
                    _RefillEOQData = value;
                    RaisePropertyChanged(nameof(RefillEOQData));
                }
            }
        }
        public PendingOrdersData PendingOrdersData
        {
            get { return _PendingOrdersData; }
            set
            {
                if (_PendingOrdersData != value)
                {
                    _PendingOrdersData = value;
                    RaisePropertyChanged(nameof(PendingOrdersData));
                }
            }
        }
        public LostSalesData LostSalesData
        {
            get { return _LostSalesData; }
            set
            {
                if (_LostSalesData != value)
                {
                    _LostSalesData = value;
                    RaisePropertyChanged(nameof(LostSalesData));
                }
            }
        }
        private void UpdateActivePanel()
        {
            if (DemandType == BasicEnums.DemandType.Constant_Demand)
            {
                switch (ConstantDemandType)
                {
                    case BasicEnums.ConstantDemandType.Basic_EOQ:
                        CurrentActivePanel = ActivePanel.BasicEOQ;
                        break;

                    case BasicEnums.ConstantDemandType.Refill_Time_EOQ:
                        CurrentActivePanel = ActivePanel.RefillTimeEOQ;
                        break;
                    case BasicEnums.ConstantDemandType.Pending_Orders:
                        CurrentActivePanel = ActivePanel.PendingOrders;
                        break;
                    case BasicEnums.ConstantDemandType.Lost_Sales:
                        CurrentActivePanel = ActivePanel.LostSales;
                        break;
                    case BasicEnums.ConstantDemandType.Discount_Large_Orders:
                        CurrentActivePanel = ActivePanel.Discount_Large_Orders;
                        break;
                    case BasicEnums.ConstantDemandType.Multiple_Products_Single_Supplier:
                        CurrentActivePanel = ActivePanel.Multiple_Products_Single_Supplier;
                        break;
                    case BasicEnums.ConstantDemandType.Multiple_Products_Multiple_Supplier:
                        CurrentActivePanel = ActivePanel.Multiple_Products_Multiple_Supplier;
                        break;

                    default:
                        CurrentActivePanel = ActivePanel.None;
                        break;
                }
            }
            else if (DemandType == BasicEnums.DemandType.Time_Varying_Demand)
            {
                switch (TimeVaryingDemandType)
                {
                    case BasicEnums.TimeVaryingDemandType.Infinite_Capacity:
                        CurrentActivePanel = ActivePanel.TimeVarying_Infinite_Capacity;
                        break;
                    case BasicEnums.TimeVaryingDemandType.Finite_Capacity:
                        CurrentActivePanel = ActivePanel.TimeVarying_Finite_Capacity;
                        break;
                    default:
                        CurrentActivePanel = ActivePanel.None;
                        break;
                }
            }
            else
            {
                CurrentActivePanel = ActivePanel.None;
            }
        }

        #endregion

        #region TimeVarying Demand

        private DateTime _DateFrom { get; set; }
        private DateTime _DateTo { get; set; }

        public DateTime DateFrom
        {
            get { return _DateFrom; }
            set
            {
                _DateFrom = value;
                OnPropertyChanged(nameof(DateFrom));
            }
        }
        public DateTime DateTo
        {
            get { return _DateTo; }
            set
            {
                _DateTo = value;
                OnPropertyChanged(nameof(DateTo));
            }
        }
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
        private ObservableCollection<ItemData> _Item { get; set;}
        public ObservableCollection<ItemData> Item
        {
            get { return _Item; }
            set
            {
                _Item = value;
                OnPropertyChanged(nameof(Item));
            }
        }
        private BasicEnums.TimeVaryingDemandType _TimeVaryingDemandType;
        public BasicEnums.TimeVaryingDemandType TimeVaryingDemandType
        {
            get { return _TimeVaryingDemandType; }
            set
            {
                _TimeVaryingDemandType = value;
                UpdateActivePanel();
                OnPropertyChanged("ConstantDemandType");
            }
        }

        private ObservableCollection<TimeVaryingInvData> _TimeVarInfiniteData;

        public ObservableCollection<TimeVaryingInvData> TimeVarInfiniteData
        {
            get { return _TimeVarInfiniteData; }
            set
            {
                _TimeVarInfiniteData = value;
                OnPropertyChanged(nameof(TimeVarInfiniteData));
            }
        }

        private ObservableCollection<TimeVaryingInvResultsData> _TimeVarInfiniteDataResults;

        public ObservableCollection<TimeVaryingInvResultsData> TimeVarInfiniteDataResults
        {
            get { return _TimeVarInfiniteDataResults; }
            set
            {
                _TimeVarInfiniteDataResults = value;
                OnPropertyChanged(nameof(TimeVarInfiniteDataResults));
            }
        }

        #endregion
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public void RaisePropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
        #endregion
    }
}
