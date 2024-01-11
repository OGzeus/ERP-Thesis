using ControlzEx.Standard;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Erp.Converters
{
    public static class MainStackPanelReadonly
    {
        public static readonly DependencyProperty IsEnabledBasedOnPropertyProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabledBasedOnProperty",
                typeof(string),
                typeof(MainStackPanelReadonly),
                new PropertyMetadata(null, OnIsEnabledBasedOnPropertyChanged));

        public static string GetIsEnabledBasedOnProperty(DependencyObject obj) =>
            (string)obj.GetValue(IsEnabledBasedOnPropertyProperty);

        public static void SetIsEnabledBasedOnProperty(DependencyObject obj, string value) =>
            obj.SetValue(IsEnabledBasedOnPropertyProperty, value);

        private static void OnIsEnabledBasedOnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StackPanel stackPanel)
            {
                // Check if the StackPanel is loaded
                if (stackPanel.IsLoaded)
                {
                    // StackPanel is loaded, trigger the update method
                    UpdateIsEnabledBasedOnProperty(stackPanel, (string)e.NewValue);
                }
                else
                {
                    // Subscribe to the Loaded event and perform the update when it's raised
                    RoutedEventHandler loadedHandler = null;
                    loadedHandler = (sender, args) =>
                    {
                        // Unsubscribe to avoid multiple calls
                        stackPanel.Loaded -= loadedHandler;

                        // Trigger the update method
                        UpdateIsEnabledBasedOnProperty(stackPanel, (string)e.NewValue);
                    };

                    // Subscribe to the Loaded event
                    stackPanel.Loaded += loadedHandler;
                }
            }
        }


            private static void UpdateIsEnabledBasedOnProperty(StackPanel stackPanel, string propertyToCheck)
            {
            var IsEnabled = string.IsNullOrWhiteSpace(propertyToCheck);
            TabControl tabControl = null;

            if (IsEnabled)
            {
                tabControl = stackPanel.Children.OfType<TabControl>().FirstOrDefault();

                if (tabControl != null)
                {
                    for (int i = 0; i < tabControl.Items.Count; i++)
                    {
                        if (tabControl.Items[i] is TabItem tabItem)
                        {
                            if (tabItem.Content is StackPanel tabPanel)
                            {
                                foreach (UIElement child in tabPanel.Children)
                                {
                                    if (child is SfDataGrid sfGrid)
                                    {
                                        bool isGridEnabled = i == 0;
                                        sfGrid.IsEnabled = isGridEnabled;
                                    }
                                    else
                                    {
                                        child.IsEnabled = false;
                                    }
                                }
                            }

                            tabItem.IsEnabled = i == 0;
                        }
                    }
                }
            }
            else
            {
                tabControl = stackPanel.Children.OfType<TabControl>().FirstOrDefault();

                if (tabControl != null)
                {
                    for (int i = 0; i < tabControl.Items.Count; i++)
                    {
                        if (tabControl.Items[i] is TabItem tabItem)
                        {
                            tabItem.IsEnabled = true;

                            if (tabItem.Content is StackPanel tabPanel)
                            {
                                foreach (UIElement child in tabPanel.Children)
                                {
                                    if (child is SfDataGrid sfGrid)
                                    {
                                        sfGrid.IsEnabled = true;
                                    }
                                    else
                                    {
                                        child.IsEnabled = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
