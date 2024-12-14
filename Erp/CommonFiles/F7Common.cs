using Erp.Model;
using Erp.Model.Thesis;
using Erp.Model.Thesis.CrewScheduling;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.UI.WebControls;
using System.Windows.Data;

namespace Erp.CommonFiles
{
    public class F7Common 
    {

        CommonFunctions CommonFunctions = new CommonFunctions();

        #region Thesis

        public F7Data F7Country(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetCountryData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CountryCode", HeaderText = "Country Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CountryDescr", HeaderText = "Country Descr" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });

            GridData.F7key = "CountryFrom";
            return GridData;
        }

        public F7Data F7City(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetCityData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CityId", HeaderText = "City Id ", IsHidden = true });

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CityCode", HeaderText = "City Code " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CityDescr", HeaderText = "City Descr " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "PrefDescr", HeaderText = "Prefecture Descr " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CountryDescr", HeaderText = "Country Descr" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });

            GridData.F7key = "City";
            return GridData;
        }


        public F7Data F7CrewScheduling(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetCSInputData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Id", HeaderText = "CS ID" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Code", HeaderText = "CS Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Descr", HeaderText = "CS Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "DateFrom_Str", HeaderText = "Date From" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "DateTo_Str", HeaderText = "Date To" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Position", HeaderText = "Position" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Is Deleted" });

                                              
            GridData.F7key = "CSCODE";
            return GridData;
        }
        public F7Data F7CSFlightRoutes(CSInputData InputData)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();



            // Assuming GridData is an instance of SfDataGrid
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Code", HeaderText = "Route Code", AllowEditing = false });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Descr", HeaderText = "Route Descr", AllowEditing = false });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Airport.Code", HeaderText = "Airport Code", AllowEditing = false });
            //GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Airport.City.CityDescr", HeaderText = "City Descr", AllowEditing = false });
            //GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Airport.City.PrefDescr", HeaderText = "Pref Descr", AllowEditing = false });
            //GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Airport.City.CountryDescr", HeaderText = "Country Descr", AllowEditing = false });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "StartDate_String", HeaderText = "Start Date", AllowEditing = false });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "EndDate_String", HeaderText = "End Date ", AllowEditing = false });
            //GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "TotalTime", HeaderText = "Total Time", AllowEditing = false });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "FlightTime", HeaderText = "Flight Time ", AllowEditing = false });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Complement_Captain", HeaderText = "Complement Captain", AllowEditing = false });





            GridData.F7key = "FlightRoutes";
            return GridData;
        }
        public F7Data F7CSEmployee(CSInputData InputData)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetEmployeesByTypeData(InputData.Position,false).ToList();

            
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Code", HeaderText = "Employee Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Descr", HeaderText = "Employee Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Position", HeaderText = "Position" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Seniority", HeaderText = "Seniority" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "EmpCrSettings.LowerBound", HeaderText = "Lower Bound" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "EmpCrSettings.UpperBound", HeaderText = "Upper Bound" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "BaseAirport.Code", HeaderText = "Airport Code" });



            GridData.F7key = "Employee";
            return GridData;
        }
        public F7Data F7VacationPlanning(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetVPInputData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "VPCode", HeaderText = "VP Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "VPDescr", HeaderText = "VP Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "EmployeeType", HeaderText = "Employee Type" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "VPLogicType", HeaderText = "VP Logic Type" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Bmax", HeaderText = "Max Satisfied Bids" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Se", HeaderText = "Separation Value" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Schedule.ReqCode", HeaderText = "Schedule Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Schedule.DateFromStr", HeaderText = "Date From" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Schedule.DateToStr", HeaderText = "Date To" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });


            GridData.F7key = "VPCode";
            return GridData;
        }
        public F7Data F7ReqSchedule (bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetReqScheduleInfoData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ID", HeaderText = "Id" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ReqCode", HeaderText = "Schedule Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ReqDescr", HeaderText = "Schedule Description" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Notes", HeaderText = "Notes" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "DateFromStr", HeaderText = "Date From" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "DateToStr", HeaderText = "Date To" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "LimitLineFixed", HeaderText = "Limit Line Fixed" });

            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "MainSchedule", HeaderText = " Main Schedule" });

            GridData.F7key = "ReqSchedule";
            return GridData;
        }
        public F7Data F7Employee(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetEmployeeData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Code", HeaderText = "Employee Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Descr", HeaderText = "Employee Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "FirstName", HeaderText = "First Name " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "LastName", HeaderText = "Last Name " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Gender", HeaderText = "Gender" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Position", HeaderText = "Position" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Seniority", HeaderText = "Seniority" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "TotalFlightHours", HeaderText = "Total Flight Hours" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });


            GridData.F7key = "Employee";
            return GridData;
        }

        public F7Data F7Certification(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetCertificationData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Code", HeaderText = "Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Descr", HeaderText = "Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CertPosition", HeaderText = "Certification Position" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ValidityTimeBucket", HeaderText = "Validity TimeBucket" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "ValidityPeriod", HeaderText = "Validity Period" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "DateFrom", HeaderText = "Date From" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "DateTo", HeaderText = "Date To" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });



            GridData.F7key = "Certification";
            return GridData;
        }

        public F7Data F7Airports(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetAirportsData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Code", HeaderText = "Airport Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Descr", HeaderText = "Airport Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "City.CityCode", HeaderText = "City Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "City.CityDescr", HeaderText = "City Description" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "City.PrefDescr", HeaderText = "Prefecture" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "City.CountryDescr", HeaderText = "Country" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });

            GridData.F7key = "Airport";
            return GridData;
        }
        public F7Data F7FL_Airports(bool ShowDeleted,AirportData Airport)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetAirportsData(ShowDeleted).ToList();
            if (!string.IsNullOrWhiteSpace(Airport.Code))
            {
                Data = Data.Where(d => d.Code != Airport.Code).ToList();
            }
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Code", HeaderText = "Airport Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Descr", HeaderText = "Airport Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "City.CityDescr", HeaderText = "City" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "City.PrefDescr", HeaderText = "Prefecture" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "City.CountryDescr", HeaderText = "Country" });

            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });



            GridData.F7key = "Airport";
            return GridData;
        }
        public F7Data F7FlightLegs(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetFlightLegsData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Code", HeaderText = "Flight Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Descr", HeaderText = "Flight Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "AirportDataFrom.Code", HeaderText = "Airport From" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "AirportDataTo.Code", HeaderText = "Airport To" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "StartDate_String", HeaderText = "Start Date" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "EndDate_String", HeaderText = "End Date" });


            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });



            GridData.F7key = "FlightLeg";
            return GridData;
        }
        public F7Data F7FlightRoutes(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetFlightRoutesData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Code", HeaderText = "Flight Code" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Descr", HeaderText = "Flight Descr" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "Airport.Code", HeaderText = "Airport" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "StartDate_String", HeaderText = "Start Date" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "EndDate_String", HeaderText = "End Date" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "TotalTime", HeaderText = "Total Time" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "FlightTime", HeaderText = "Flight Time" });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "GroundTime", HeaderText = "Ground Time" });

            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });



            GridData.F7key = "FlightRoute";
            return GridData;
        }

        #endregion

        #region Motherland


        public F7Data F7MLEmployee(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetCityData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CityId", HeaderText = "City Id ", IsHidden = true });

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CityCode", HeaderText = "City Code " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CityDescr", HeaderText = "City Descr " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "PrefDescr", HeaderText = "Prefecture Descr " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CountryDescr", HeaderText = "Country Descr" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });

            GridData.F7key = "Department";
            return GridData;
        }
        public F7Data F7Department(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetCityData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CityId", HeaderText = "City Id ", IsHidden = true });

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CityCode", HeaderText = "City Code " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CityDescr", HeaderText = "City Descr " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "PrefDescr", HeaderText = "Prefecture Descr " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CountryDescr", HeaderText = "Country Descr" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });

            GridData.F7key = "Department";
            return GridData;
        }

        public F7Data F7Position(bool ShowDeleted)
        {
            F7Data GridData = new F7Data();
            GridData.SfGridColumns = new Columns();
            var Data = CommonFunctions.GetCityData(ShowDeleted).ToList();
            GridData.CollectionView = CollectionViewSource.GetDefaultView(Data);

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CityId", HeaderText = "City Id ", IsHidden = true });

            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CityCode", HeaderText = "City Code " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CityDescr", HeaderText = "City Descr " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "PrefDescr", HeaderText = "Prefecture Descr " });
            GridData.SfGridColumns.Add(new GridTextColumn() { MappingName = "CountryDescr", HeaderText = "Country Descr" });
            GridData.SfGridColumns.Add(new GridCheckBoxColumn() { MappingName = "IsDeleted", HeaderText = "Deleted" });

            GridData.F7key = "Department";
            return GridData;
        }
        #endregion
    }
}
