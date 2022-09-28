using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Reporting.WinForms;

namespace Planner
{
    /// <summary>
    /// Interaction logic for ReportViewer.xaml
    /// </summary>
    public partial class ReportViewer : UserControl
    {
        public ReportViewer()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            ToDoToday9DataSet dataset = new ToDoToday9DataSet();
            //dataset.Tracks.OrderByDescending(trac);

            dataset.BeginInit();

            reportDataSource1.Name = "DataSet1"; //  Name of dataset in .RDLC file
            reportDataSource1.Value = dataset.Tracks;
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportPath = @"C:\Projects\Working\Planner\Planner\Views\Reports\ReportTracks.rdlc";

            dataset.EndInit();

            // fill data into DataSet
            ToDoToday9DataSetTableAdapters.TracksTableAdapter tracksTableAdapter = new ToDoToday9DataSetTableAdapters.TracksTableAdapter();
            tracksTableAdapter.ClearBeforeFill = true;
            tracksTableAdapter.Fill(dataset.Tracks);
            //     tracksTableAdapter.FillByDateDesc(dataset.Tracks);
            // tracksTableAdapter.s

            reportViewer.RefreshReport();
            // _isReportViewerLoaded = true;

            ///theViewer.RefreshReport();
        }

        private void reportViewer_RenderingComplete(object sender, RenderingCompleteEventArgs e)
        {

        }
    }
}
