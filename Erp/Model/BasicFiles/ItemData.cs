using Erp.Model.Enums;
using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.BasicFiles
{
    public class ItemData : RecordBaseModel
    {
        private int itemid;
        private int assemblynumber;

        private string itemcode;
        private string itemdescr;
        private string mesunit;
        private BasicEnums.ItemType itemtype;
        private BasicEnums.Assembly assembly; 
        private bool? inputorderflag; 
        private bool? outputorderflag; 
        private bool? canbeproduced; 
        private float? bompercentage;
        private LotPolicyData lotpolicy;


        public BasicEnums.ItemType ItemType
        {
            get { return itemtype; }
            set { itemtype = value; OnPropertyChanged("ItemType"); }
        }

        public BasicEnums.Assembly Assembly // Changed to nullable
        {
            get { return assembly; }
            set { assembly = value; OnPropertyChanged("Assembly"); }
        }
        private float _Profit { get; set; }

        private float _SalesPrice { get; set; }

        private float _ManufacturingCost { get; set; }

        private float _HoldingCost { get; set; }

        private float _ShortageCost { get; set; }

        private float _LeadTime { get; set; }
        public LotPolicyData LotPolicy
        {
            get { return lotpolicy; }
            set { lotpolicy = value; OnPropertyChanged("LotPolicy"); }
        }

        public int AssemblyNumber
        {
            get { return assemblynumber; }
            set { assemblynumber = value; OnPropertyChanged("AssemblyNumber"); }
        }

        public float Profit
        {
            get { return _Profit; }
            set { _Profit = value; OnPropertyChanged("Profit"); }
        }
        public float SalesPrice
        {
            get { return _SalesPrice; }
            set { _SalesPrice = value; OnPropertyChanged("SalesPrice"); }
        }
        public float ManufacturingCost
        {
            get { return _ManufacturingCost; }
            set { _ManufacturingCost = value; OnPropertyChanged("ManufacturingCost"); }
        }

        public float HoldingCost
        {
            get { return _HoldingCost; }
            set
            {
                _HoldingCost = (float)Math.Round(value, 3);
                OnPropertyChanged("HoldingCost");
            }
        }
        public float ShortageCost
        {
            get { return _ShortageCost; }
            set { _ShortageCost = value; OnPropertyChanged("ShortageCost"); }
        }
        public float LeadTime
        {
            get { return _LeadTime; }
            set { _LeadTime = value; OnPropertyChanged("LeadTime"); }
        }

        public float? BomPercentage
        {
            get { return bompercentage; }
            set { bompercentage = value; OnPropertyChanged("BomPercentage"); }
        }

        public bool? InputOrderFlag
        {
            get { return inputorderflag; }
            set { inputorderflag = value; OnPropertyChanged("InputOrderFlag"); }
        }

        public bool? OutputOrderFlag
        {
            get { return outputorderflag; }
            set { outputorderflag = value; OnPropertyChanged("OutputOrderFlag"); }
        }

        public bool? CanBeProduced
        {
            get { return canbeproduced; }
            set { canbeproduced = value; OnPropertyChanged("CanBeProduced"); }
        }

        public int ItemId
        {
            get { return itemid; }
            set { itemid = value; OnPropertyChanged("ItemId"); }
        }

        public string ItemCode
        {
            get { return itemcode; }
            set { itemcode = value; OnPropertyChanged("ItemCode"); }
        }

        public string ItemDescr
        {
            get { return itemdescr; }
            set { itemdescr = value; OnPropertyChanged("ItemDescr"); }
        }

        public string MesUnit
        {
            get { return mesunit; }
            set { mesunit = value; OnPropertyChanged("MesUnit"); }
        }





        private int _MaxInventory { get; set; }

        public int MaxInventory
        {
            get { return _MaxInventory; }
            set
            {
                _MaxInventory = value; OnPropertyChanged("MaxInventory");
            }
        }

        private int _StoreTarget { get; set; }

        public int StoreTarget
        {
            get { return _StoreTarget; }
            set
            {
                _StoreTarget = value; OnPropertyChanged("StoreTarget");

            }
        }

    }
}
