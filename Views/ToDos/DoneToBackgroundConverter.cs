using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Planner
{
    class DoneToBackgroundConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var d = (CodeFirst.EFcf.ToDo)value;
            //if (value is CodeFirst.EFcf.ToDo. ToDoVM.TheEntity.Done)
            //{

            //}
            if (value is BuildSqliteCF.Entity.ToDo)
            {
                BuildSqliteCF.Entity.ToDo todo = (BuildSqliteCF.Entity.ToDo)value;
                // Setting row color based on values of data
                //if (todo.Priority == "R" && !todo.Done)
                //{
                //    return Brushes.LightGray;
                //}
                //if (todo.Priority == "R" && todo.Done)
                //{
                //    return Brushes.Lime;
                //}
                string t = todo.Status;
                switch (t)
                {
                    case "O":
                        if (todo.Priority == "R")
                        {
                            return Brushes.LightGray;
                        }
                        else
                        {
                            return Brushes.WhiteSmoke;
                        }

                    case "A":
                        return Brushes.WhiteSmoke;
                    case "I":
                        return Brushes.LightPink;
                    case "W":
                        return Brushes.LawnGreen;
                    case "F":
                        return Brushes.SpringGreen;
                    default:
                        return Brushes.WhiteSmoke;
                }

            }
         //   value is BuildSqliteCF.Entity.ToDo
            //if (value is bool)
            //{
            //     bool doneFlag = (bool)value;
            //    //bool doneFlag = d.Done;
            //    if (doneFlag)
            //    {
            //        return Brushes.LightGreen;
            //    }
            //    //int quantity = (int)value;
            //    //if (quantity >= 100) return Brushes.White;
            //    //if (quantity >= 10) return Brushes.WhiteSmoke;
            //    //if (quantity >= 0) return Brushes.LightGray;
            //    //return Brushes.White; //quantity should not be below 0
            //}
            //value is not an integer. Do not throw an exception in the converter, but return something that is obviousl ywrong
            return Brushes.SlateGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class StatusToFontSizeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var d = (CodeFirst.EFcf.ToDo)value;
            //if (value is CodeFirst.EFcf.ToDo. ToDoVM.TheEntity.Done)
            //{

            //}
            if (value is string)
            {
                // bool doneFlag = (bool)value;
                string doneFlag = (string)value;
                if (doneFlag == "A")
                {
                    return "24";
                }
                //int quantity = (int)value;
                //if (quantity >= 100) return Brushes.White;
                //if (quantity >= 10) return Brushes.WhiteSmoke;
                //if (quantity >= 0) return Brushes.LightGray;
                //return Brushes.White; //quantity should not be below 0
            }
            //value is not an integer. Do not throw an exception in the converter, but return something that is obviousl ywrong
            return "14";  // Brushes.Yellow;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    class ToDoStatusToBackgroundConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var d = (CodeFirst.EFcf.ToDo)value;
            //if (value is CodeFirst.EFcf.ToDo. ToDoVM.TheEntity.Done)
            //{

            //}
            if (value is string)
            {
                // bool doneFlag = (bool)value;
                string doneFlag = (string)value;
                if (doneFlag == "A")
                {
                    return "24";
                }
                //int quantity = (int)value;
                //if (quantity >= 100) return Brushes.White;
                //if (quantity >= 10) return Brushes.WhiteSmoke;
                //if (quantity >= 0) return Brushes.LightGray;
                //return Brushes.White; //quantity should not be below 0
            }
            //value is not an integer. Do not throw an exception in the converter, but return something that is obviousl ywrong
            return "14";  // Brushes.Yellow;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsObsoleteToTextDecorationsConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value)
                {
                    TextDecorationCollection redStrikthroughTextDecoration = TextDecorations.Strikethrough.CloneCurrentValue();
                    redStrikthroughTextDecoration[0].Pen = new Pen { Brush = Brushes.Red, Thickness = 3 };
                    return redStrikthroughTextDecoration;
                }
            }
            return new TextDecorationCollection();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class QuantityToForegroundConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string test = (string)value;
            if (value is string)
            {
                //int quantity;
                //if (int.TryParse((string)value, out quantity))
                //{
                //    if (quantity >= 0) return Brushes.Black;
                //}
                if (test == "A")
                {
                    return Brushes.IndianRed;
                }
                else if (test == "B")
                {
                    return Brushes.DarkTurquoise;
                }
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
