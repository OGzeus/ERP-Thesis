using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Enums
{
    public class BasicEnums : INotifyPropertyChanged
    {
        #region Thesis

        public enum VPLogicType
        {
         Strict_Seniority,
         Fair_Assignment,
         Bid_By_Bid
        }
        public enum BidType
        {
            Specific,
            Non_Specific,
            Min_Max
        }
        public enum EmployeeType
        {
            Captain,
            FO,
            Cabin_Manager,
            Flight_Attendant
        }

        public enum CertPosition
        {
            PNT, 
            PNC
        }

        public enum Timebucket
        {
            Daily,
            Weekly,
            Monthly,
            Quarterly,
            Yearly
        }

        public enum AircraftType
        {
            Commercial,
            Cargo
        }
        #endregion


        #region Clustering_VRP
        public enum Clustering_Techniques
        {
            K_means,
            Hierarchical,
            DBSCAN

        }
        public enum VRP_Techniques
        {
            Simulation_Annealing,
            Optimization,
            Tabu_Search,
            Ant_Colony
        }
        public enum TSP_Techniques
        {
            Simulation_Annealing,
            Ant_Colony_Optimization,
            Optimization
            
        }
        #endregion

        #region Inventory Control


        public enum PeriodType
        {
         Monthly,
         Yearly
        }

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

        public enum DemandType
        {
            Constant_Demand,
            Time_Varying_Demand,
            Uncertain_Demand

        }
        public enum ConstantDemandType
        {
            Basic_EOQ,
            Refill_Time_EOQ,
            Pending_Orders,
            Lost_Sales,
            Discount_Large_Orders,
            Multiple_Products_Single_Supplier,
            Multiple_Products_Multiple_Supplier,

        }
        public enum TimeVaryingDemandType
        {
            Infinite_Capacity,
            Finite_Capacity

        }
        #endregion

        #region General 
        public enum CustomerType
        {
            Retail,
            Wholesale
        }
        public enum Gender
        {
            Male,
            Female
            
        }


        public enum ItemType
        {
            Cement,
            Dust,
            NoType
        }

        public enum Assembly
        {
            Finished,
            SemiFinished,
            RawMaterial
        }
        public enum MachStatus
        {
            Inactive,  // Machine is not currently in use
            Active,    // Machine is currently in use
            Maintenance, // Machine is under maintenance
            Error,     // Machine has encountered an error
            Offline,   // Machine is not connected
            Online     // Machine is connected

        }
        public enum MachType
        {
            Grinding,
            Vertical_Drilling,
            Horizontal_Drilling,
            Boring,
            Planning,
            AssemblyLine,  // Machine used in an assembly line process
            Packaging,     // Machine used for packaging products
            Sorting,       // Machine used for sorting inventory
            QualityControl, // Machines used for testing or quality control
            Conveyor,      // Conveyor systems for moving products
            Loading,       // Machines used for loading goods (like forklift)
            Palletizing,   // Machines used for palletizing goods
            Labelling,     // Machines used for labeling products
            RoboticArm,    // Robotic arm used in various tasks
            CNCMachining,  // Computer numerical control machine for precise tasks
            Welding,       // Machines used for welding tasks
            Printing       // Machines used for printing labels, instructions, etc.
        }
        public enum OrderStatus
        {
            Ready,
            Processing,
            Shipped,
            Delivered,
            Cancelled
            
        }

        public enum Incoterms
        {
            EXW,
            FCA,
            FAS,
            FOB,
            CFR,
            CIF,
            CPT,
            CIP,
            DAP,
            DPU,
            DDP
        }

        #endregion


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

        #endregion
    }
}
