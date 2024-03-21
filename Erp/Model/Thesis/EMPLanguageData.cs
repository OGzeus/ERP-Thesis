using Erp.Model.BasicFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis
{
    public class EMPLanguageData : INotifyPropertyChanged
    {
        private int finalemployeeid { get; set; }

        private LanguageData _Language { get; set; }

        private bool languageflag { get; set; }

        private bool newlanguageflag { get; set; }
        private bool existingflag { get; set; }

        public bool ExistingFlag
        {
            get { return existingflag; }
            set { existingflag = value; OnPropertyChanged("ExistingFlag"); }
        }
        public bool NewLanguageFlag
        {
            get { return newlanguageflag; }
            set { newlanguageflag = value; OnPropertyChanged("NewLanguageFlag"); }
        }
        public bool LanguageFlag
        {
            get { return languageflag; }
            set { languageflag = value; OnPropertyChanged("LanguageFlag"); }
        }

        public int FinalEmployeeId
        {
            get { return finalemployeeid; }
            set { finalemployeeid = value; OnPropertyChanged("FinalEmployeeId"); }
        }


        public LanguageData Language
        {
            get { return _Language; }
            set { _Language = value; OnPropertyChanged("Language"); }
        }




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
