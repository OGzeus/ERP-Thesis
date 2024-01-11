using Erp.Model.Suppliers;
using Erp.ViewModel;
using Erp.ViewModel.BasicFiles;
using Erp.ViewModel.Suppliers;
using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model
{
    public class ChildViewModelData
    {
        public ViewModelBase ChildView { get; set; }

        public SupplierInfoSearchFilterData Filter { get; set; }
        public string Caption { get; set; }

        public IconChar Icon { get; set; }

        public ChildViewModelData(string Page)
        {

            if (Page.Contains("Supplier"))
            {

                if (Page == "SupplierInfoPage")
                {
                    ChildView = new SupplierInfoChooserViewModel();
                    Caption = "Αρχείο Προμηθευτών";
                    Icon = IconChar.EarthEurope;
                }
                if (Page == "SupplierInfoSearchPage")
                {
                    ChildView = new SupplierInfoSearchViewModel();
                    Caption = "Αναζήτηση Προμηθευτών";
                    Icon = IconChar.MountainCity;
                }
                if (Page == "SupplierOrderPage")
                {
                    ChildView = new SupplierOrderViewModel();
                    Caption = "Αρχείο Παραγγελιών Προμηθευτή";
                    Icon = IconChar.GlobeEurope;
                }

                if (Page == "SupplierOrderSearchPage")
                {
                    ChildView = new SupplierOrderSearchViewModel();
                    Caption = "Αναζήτση Παραγγελιών Προμηθευτή";
                    Icon = IconChar.MountainCity;
                }

                if (Page == "SupplierOrderSearchResultPage")
                {
                    ChildView = new SupplierInfoSearchResultViewModel(Filter);
                    Caption = "Αναζήτση Παραγγελιών Προμηθευτή";
                    Icon = IconChar.MountainCity;
                }

            }

            if (Page == "BasicCountryPage")
            {
                ChildView = new CountryViewModel();
                Caption = "Αρχείο Χωρών";
                Icon = IconChar.EarthEurope;
            }
            if (Page == "BasicPrefecturePage")
            {
                ChildView = new PrefectureViewModel();
                Caption = "Αρχείο Νομών";
                Icon = IconChar.GlobeEurope;
            }
            if (Page == "BasicCityPage")
            {
                ChildView = new CityViewModel();
                Caption = "Αρχείο Πόλεων";
                Icon = IconChar.MountainCity;
            }


        }

        public ChildViewModelData(string Page, SupplierInfoSearchFilterData Filter)
        {
            if (Page == "SupplierOrderSearchResultPage")
            {
                ChildView = new SupplierInfoSearchResultViewModel(Filter);
                Caption = "Αναζήτση Παραγγελιών Προμηθευτή";
                Icon = IconChar.MountainCity;
            }
        }
    }
}
