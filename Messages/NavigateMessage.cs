using System;
using System.Windows.Controls;

namespace Planner
{
    public class NavigateMessage
    {
        public Type ViewType { get; set; }
        public Type ViewModelType { get; set; }
        public UserControl View { get; set; }
        //  public UserControl previousView { get; set; }
    }
    public class ChangeViewMessage
    {
        public Type ViewType { get; set; }
        public Type ViewModelType { get; set; }
        public UserControl newView { get; set; }
        //  public UserControl previousView { get; set; }
    }

    public class CurrentViewMessage
    {
        public Type ViewType { get; set; }
        public Type ViewModelType { get; set; }
        public UserControl View { get; set; }
    }

    public class ReportMessage
    {
        public Type ReportType { get; set; }
        public Type ReportModelType { get; set; }
        public UserControl Report { get; set; }
    }
}
