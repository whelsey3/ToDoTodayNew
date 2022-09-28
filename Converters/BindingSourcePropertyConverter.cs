using GalaSoft.MvvmLight.Command;
using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace Planner
{
    public class BindingSourcePropertyConverter : IEventArgsConverter
    {
        public object Convert(object value, object parameter)
        {
            DataTransferEventArgs e = (DataTransferEventArgs)value;

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
            //else if (type == typeof(DateTimePicker))
            //{
            //    binding = ((DateTimePicker)e.TargetObject).GetBindingExpression(DateTimePicker.SelectedDateProperty);
            //}
            return binding.ResolvedSourcePropertyName ?? "";
        }
    }
}
