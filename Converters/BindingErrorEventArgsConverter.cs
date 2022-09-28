using BuildSqliteCF.Entity;
using GalaSoft.MvvmLight.Command;
using System.Windows.Controls;
//using BuildSqliteCF.DbContexts;

namespace Planner
{
    public class BindingErrorEventArgsConverter : IEventArgsConverter
    {
        public object Convert(object value, object parameter)
        {
            ValidationErrorEventArgs e = (ValidationErrorEventArgs)value;
            PropertyError err = new PropertyError();
            err.PropertyName = ((System.Windows.Data.BindingExpression)(e.Error.BindingInError)).ResolvedSourcePropertyName;
            err.Error = e.Error.ErrorContent.ToString();
            // Validation.ErrorEvent fires for addition and removal
            if (e.Action.ToString() == "Added")
            {
                err.Added = true;
            }
            else
            {
                err.Added = false;
            }
            return err;
        }
    }
}
