using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Erp.DataBase;
using Erp.View;
using Microsoft.EntityFrameworkCore;

namespace Erp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected void ApplicationStart(object sender, EventArgs e)
        {
            //var loginView = new LoginView();
            //loginView.Show();
            //loginView.IsVisibleChanged += (s, ev) =>
            //{
            //    if (loginView.IsVisible == false && loginView.IsLoaded)
            //    {
                    var mainView = new MainView();
                    mainView.Show();
            //        loginView.Close();
            //    }
            //};

        }
        public App()
        {
            ErpDbContext.DbOptions = new DbContextOptionsBuilder<ErpDbContext>()
    .UseSqlServer("Server=DESKTOP-F2TG0LU\\SQLEXPRESS;Database=ERPDatabase;Trusted_Connection=True;")
    .Options;
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTQ2ODE0N0AzMjMxMmUzMTJlMzMzNWpUREZVSnc5a0hxMzA3WFo2ZWsxLzIrbWJDWmlEbERJczJtOENUTUtBWXM9");
        }
    }
}
