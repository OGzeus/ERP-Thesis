using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MyControls
{
    public class F7TextBox : Control
    {
        static F7TextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(F7TextBox), new FrameworkPropertyMetadata(typeof(F7TextBox)));
        }

        public override void OnApplyTemplate()
        {
          base.OnApplyTemplate();
          // Get a reference to the StackPanel in the control template
          var stackPanel = GetTemplateChild("PART_StackPanel") as StackPanel;

          if (stackPanel != null)
          {
                // Set the StackPanel's orientation
                stackPanel.Orientation = Orientation.Horizontal;
                // Get a reference to the TextBox and Button in the control template
                var textBox = GetTemplateChild("PART_TextBox") as TextBox;
            var button = GetTemplateChild("PART_Button") as Button;
            if (textBox != null && button != null)
            {
                // Set the TextBox's properties
                textBox.Text = TextBoxValue;
                textBox.IsReadOnly = !AllowNull;

                // Set the Button's properties
                button.IsEnabled = AllowNull;
                button.Command = ShowWindowCommand;

                // Handle the Button's Click event
                button.Click += (sender, args) =>
                {
                    // Create a new window
                    var window = new Window()
                    {
                        // Set the window's properties
                        Title = WindowTitle,
                        Width = 600,
                        Height = 400,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        ResizeMode = ResizeMode.NoResize,

                        // Add a DataGrid to the window
                        Content = new DataGrid()
                        {
                            // Set the DataGrid's properties
                            AutoGenerateColumns = false,
                            CanUserAddRows = false,
                            CanUserDeleteRows = false,
                            CanUserResizeColumns = false,
                            CanUserResizeRows = false,
                            CanUserSortColumns = false,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch,
                        }
                    };

                    // Add the DataGrid's columns
                    foreach (var c in DataGridColumns)
                    {
                        var column = new DataGridTextColumn()
                        {
                            Header = c,
                            Binding = new Binding(c)
                        };
                        (window.Content as DataGrid)?.Columns.Add(column);
                    }

                   // Set the DataGrid's items source
                   //(window.Content as DataGrid)?.SetValue(DataGrid.ItemsSourceProperty, DataGridContext());
                    // Show the window as a modal dialog
                    window.ShowDialog();

                    // Get the selected item from the DataGrid
                    var selectedItem = (window.Content as DataGrid)?.SelectedItem;
                    if (selectedItem != null)
                    {
                        // Set the TextBox's Text property to the value from the selected row
                        textBox.Text = selectedItem.GetType().GetProperty(DataGridColumns.First())?.GetValue(selectedItem)?.ToString();
                    }
                };
            }
        }
    }

        // Define the DependencyProperties for the F7TextBox's properties

        public static readonly DependencyProperty TextBoxValueProperty = DependencyProperty.Register(
            nameof(TextBoxValue), typeof(string), typeof(F7TextBox), new PropertyMetadata(""));

        public string TextBoxValue
        {
            get { return (string)GetValue(TextBoxValueProperty); }
            set
            {
                SetValue(TextBoxValueProperty, value);
            }
        }

        public static readonly DependencyProperty AllowNullProperty = DependencyProperty.Register(
            nameof(AllowNull), typeof(bool), typeof(F7TextBox), new PropertyMetadata(true));

        public bool AllowNull
        {
            get { return (bool)GetValue(AllowNullProperty); }
            set { SetValue(AllowNullProperty, value); }
        }

        public static readonly DependencyProperty ShowWindowCommandProperty = DependencyProperty.Register(
            nameof(ShowWindowCommand), typeof(ICommand), typeof(F7TextBox));

        public ICommand ShowWindowCommand
        {
            get { return (ICommand)GetValue(ShowWindowCommandProperty); }
            set { SetValue(ShowWindowCommandProperty, value); }
        }

        public static readonly DependencyProperty WindowTitleProperty = DependencyProperty.Register(
            nameof(WindowTitle), typeof(string), typeof(F7TextBox), new PropertyMetadata(""));

        public string WindowTitle
        {
            get { return (string)GetValue(WindowTitleProperty); }
            set { SetValue(WindowTitleProperty, value); }
        }

        public static readonly DependencyProperty DataGridColumnsProperty = DependencyProperty.Register(
            nameof(DataGridColumns), typeof(ObservableCollection<string>), typeof(F7TextBox));

        public ObservableCollection<string> DataGridColumns
        {
            get { return (ObservableCollection<string>)GetValue(DataGridColumnsProperty); }
            set { SetValue(DataGridColumnsProperty, value); }
        }

        public static readonly DependencyProperty DataGridContextProperty = DependencyProperty.Register(
            nameof(DataGridContext), typeof(Func<IEnumerable>), typeof(F7TextBox));

        public object DataGridContext
        {
            get => GetValue(DataGridContextProperty);
            set => SetValue(DataGridContextProperty, value);
        }
    }
}