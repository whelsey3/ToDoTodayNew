using GalaSoft.MvvmLight.Command;
using System;
using System.Windows;

namespace Planner
{
    // http://www.codemag.com/article/0811081   http://www.codemag.com/article/0902061
    public class ReportVM
    {
        public string ReportDisplay { get; set; }
        public Type ReportType { get; set; }
        //public Type ViewModelType { get; set; }
        //public UserControl View { get; set; }
        public Window rView { get; set; }
        public RelayCommand RunReport1 { get; set; }
        public RelayCommand RunReport2 { get; set; }
        public RelayCommand RunReport3 { get; set; }
        public RelayCommand RunReport4 { get; set; }

        public ReportVM()
        {
            RunReport1 = new RelayCommand(RunReportTracks);
            RunReport2 = new RelayCommand(RunReportProjects);
            RunReport3 = new RelayCommand(RunReportToDos);
            RunReport4 = new RelayCommand(RunReportUtility);
        }

        public void RunReportTracks()
        {
            rView = new ReportsView("Tracks");
            rView.Show();
            rView.Focus();
        }
        public void RunReportProjects()
        {
            rView = new ReportsView("Projects");
            rView.Show();
            rView.Focus();
        }
        public void RunReportToDos()
        {
            rView = new ReportsView("ToDos");
            rView.Show();
            rView.Focus();
        }
        public void RunReportUtility()
        {
            rView = new ReportsView("sumTracks");
            rView.Show();
            rView.Focus();
        }
    }

    public class ReptParams
    {

    }
}
