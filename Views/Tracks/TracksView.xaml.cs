using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Planner
{
    public partial class TracksView : UserControl
    {
        // constructor
        public TracksView()
        {
            InitializeComponent();
            this.DataContext = new TracksViewModel();  // Noting parent UserControl
            this.Name = "TracksViewName";
            this.Loaded += TracksView_Loaded;

            ((TracksViewModel)(this.DataContext)).TheDG = dg;
        }

        private void TracksView_Loaded(object sender, RoutedEventArgs e)
        {
            Util.SelectRowByIndex(dg, 0);
            //throw new NotImplementedException();
        }

        private void DatabindingError(object sender, ValidationErrorEventArgs e)
        {
            Debug.WriteLine("ErrorContent Tracks " + e.Error.ErrorContent);
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

        private void dtpStart_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.OriginalSource is Xceed.Wpf.Toolkit.DateTimePicker)
            {
                string name = ((Xceed.Wpf.Toolkit.DateTimePicker)e.OriginalSource).Name;
            }
        }

        private void dg_GotFocus(object sender, RoutedEventArgs e)
        {
            int n = dg.SelectedIndex;
          //  Util.SelectRowByIndex(dg, n);
        }
    }
}