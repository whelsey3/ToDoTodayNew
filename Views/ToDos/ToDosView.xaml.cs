using LoggerLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Planner
{
    /// <summary>
    /// Description for ToDosView.
    /// </summary>
    public partial class ToDosView : UserControl
    {
        public Logger mLogger;
        public TDTDbContext dbBase; 

        /// <summary>
        /// Initializes a new instance of the ToDosView class.
        /// </summary>
        public ToDosView()
        {
            InitializeComponent();
         //   MainMenu = new ObservableCollection<IMenuItem>();

            mLogger = LoggerLib.Logger.Instance;
            this.DataContext = new ToDosViewModel();
            this.Name = "ToDosViewName";
            this.Loaded += ToDosView_Loaded;

            ((ToDosViewModel)(this.DataContext)).TheDGToDos = dg;

        }

        private void ToDosView_Loaded(object sender, RoutedEventArgs e)
        {
            Util.SelectRowByIndex(dg, 0);
        }

        private void DatabindingError(object sender, ValidationErrorEventArgs e)
        {
            Debug.WriteLine("ErrorContent " + e.Error.ErrorContent);
            Debug.WriteLine("ResolvedSourcePropertyName " + ((System.Windows.Data.BindingExpression)(e.Error.BindingInError)).ResolvedSourcePropertyName);
        }

        private void Binding_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            string propertyName;
            BindingExpression binding = null;
            Type type = e.TargetObject.GetType();
            if (type == typeof(TextBox))
            {
                binding = ((TextBox)e.TargetObject).GetBindingExpression(TextBox.TextProperty);
            }
            else if (type == typeof(ComboBox))
            {
                binding = ((ComboBox)e.TargetObject).GetBindingExpression(ComboBox.SelectedValueProperty);

            }
            else if (type == typeof(DatePicker))
            {
                binding = ((DatePicker)e.TargetObject).GetBindingExpression(DatePicker.SelectedDateProperty);

            }
            propertyName = binding.ResolvedSourcePropertyName;

        }

        private void ReceiveInStartTimingMessage(InStartTiming inEdit)
        {
            //this.CommandTab.IsEnabled = !inEdit.Mode;
        }

        private void dtpStart_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.OriginalSource is Xceed.Wpf.Toolkit.DateTimePicker)
            {
                string name = ((Xceed.Wpf.Toolkit.DateTimePicker)e.OriginalSource).Name;
            }
        }
        private void dtpStopStart_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.OriginalSource is Xceed.Wpf.Toolkit.DateTimePicker)
            {
                string name = ((Xceed.Wpf.Toolkit.DateTimePicker)e.OriginalSource).Name;
            }
        }
        private void dtpStopStart2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.OriginalSource is Xceed.Wpf.Toolkit.DateTimePicker)
            {
                string name = ((Xceed.Wpf.Toolkit.DateTimePicker)e.OriginalSource).Name;
            }
        }
        public void DoneChecked(object sender, RoutedEventArgs e)
        {
            var theSelected = dg.SelectedItem;
            var theSelections = dg.SelectedItems;
            ((ToDosViewModel)this.DataContext).procDoneChecked(sender, e, theSelected);
        }
        public void DoneUnchecked(object sender, RoutedEventArgs e)
        {
            var theSelected = dg.SelectedItem;
            ((ToDosViewModel)this.DataContext).procDoneUnchecked(sender, e, theSelected);
        }
        //private void reptCombo_Loaded(object sender, RoutedEventArgs e)
        //{
        //    // ... A List.
        //    List<string> data = new List<string>();
        //    data.Add("Book");
        //    data.Add("Computer");
        //    data.Add("Chair");
        //    data.Add("Mug");

        //    // ... Get the ComboBox reference.
        //    var comboBox = sender as ComboBox;

        //    // ... Assign the ItemsSource to the List.
        //    comboBox.ItemsSource = data;

        //    // ... Make the first item selected.
        //    comboBox.SelectedIndex = 0;

        //}

        //private void reptCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    // ... Get the ComboBox.
        //    var comboBox = sender as ComboBox;

        //    // ... Set SelectedItem as Window Title.
        //    string value = comboBox.SelectedItem as string;
        //    //this.Parent.Title = "Selected: " + value;
        //    ((ToDosViewModel)this.DataContext).ShowUserMessage("Selected: " + value);
        //}
    }

    public class MenuItem
    {
        public string Text { get; set; }
        public List<MenuItem> Children { get; private set; }
        public System.Windows.Input.ICommand Command { get; set; }

        public MenuItem(string item)
        {
            Text = item;
            Children = new List<MenuItem>();
        }

    }
}