using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Erp.View.Thesis.CustomButtons
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        public string Message { get; set; }

        public CustomMessageBox(string message)
        {
            InitializeComponent();
            Message = message;
            DataContext = this;
        }

        private void SaveOnly_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true; // Indicate that the dialog result is true (Save Only)
            Close();
        }

        private void SaveAndUpgrade_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true; // Indicate that the dialog result is true (Save and Upgrade)
            Close();
        }

        private void UpgradeOnly_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Indicate that the dialog result is false (Upgrade Only)
            Close();
        }
    }

    public class MainClass
    {
        public void ShowCustomMessageBox()
        {
            var customMessageBox = new CustomMessageBox("Do you want to Save the Results or Search for a better Solution?");
            if (customMessageBox.ShowDialog() == true)
            {
                // User clicked Save Only or Save and Upgrade
                if (customMessageBox.DialogResult == true)
                {
                    // User clicked Save Only or Save and Upgrade
                    var result = customMessageBox.Message.Contains("Save and Upgrade") ? "Save and Upgrade" : "Save Only";
                    Console.WriteLine($"User clicked {result}");

                    if (result == "Save and Upgrade")
                    {
                        // Handle Save and Upgrade scenario
                        Console.WriteLine("Saving and upgrading...");
                    }
                    else
                    {
                        // Handle Save Only scenario
                        Console.WriteLine("Saving only...");
                    }
                }
                else
                {
                    // User clicked Upgrade Only
                    Console.WriteLine("User clicked Upgrade Only");
                    // Handle Upgrade Only scenario
                }
            }
        }
    }
}
