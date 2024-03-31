using System;

using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set the culture to use the DD/MM/YYYY date format

            var culture = new CultureInfo("en-GB");
            culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

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
    .UseSqlServer("Server=(local);Database=ERPDatabase;Trusted_Connection=True;Encrypt=False")
    .Options;

            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzE5NDUzMEAzMjM1MmUzMDJlMzBFVm5sR2lkUm0zU2xOM1VzVjRrK1pvOWpwQ0xEN2pLalB3a1h5NzZjVHN3PQ==");
        }
    }
}
