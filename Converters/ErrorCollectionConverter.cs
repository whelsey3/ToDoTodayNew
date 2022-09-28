﻿using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Planner
{
    public class ErrorCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            ObservableCollection<string> _Errors = new ObservableCollection<string>();
            ReadOnlyObservableCollection<ValidationError> errors = (ReadOnlyObservableCollection<ValidationError>)value;
            foreach (ValidationError err in errors)
            {
                _Errors.Add(err.ErrorContent.ToString());
            }
            return new ListBox
            {
                ItemsSource = _Errors,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent
            };
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
