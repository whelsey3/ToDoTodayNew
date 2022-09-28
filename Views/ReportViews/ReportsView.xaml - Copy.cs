using System.Windows;
using System.Windows.Controls;
using CodeFirst.EFcf;
using System.Linq;

namespace Planner
{
    /// <summary>
    /// Description for ReportsView.
    /// </summary>
    public partial class ReportsView : Window
    {
        private bool _isReportViewerLoaded;
        private string cReptName;
        /// <summary>
        /// Initializes a new instance of the ReportsView class.
        /// </summary>
        public ReportsView(string theRept = "ProjSummary")
        {
            InitializeComponent();
            cReptName = theRept;
            _reportViewer.Load += ReportViewer_Load;

            DataContext = new ReportsViewModel();

        }

        private void ReportViewer_Load(object sender, System.EventArgs e)
        {
            if (!_isReportViewerLoaded)
            {
                switch (cReptName)
                {
                    case "Projects":
                        Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        testTDTDataSet datasetP = new testTDTDataSet();

                        datasetP.BeginInit();

                        reportDataSource1.Name = "DataSet1"; //  Name of dataset in .RDLC file
                        reportDataSource1.Value = datasetP.ProjSummary; // datasetR.Tracks;  // dataset.Tracks;
                        _reportViewer.LocalReport.DataSources.Add(reportDataSource1);
                        _reportViewer.LocalReport.ReportEmbeddedResource = "Planner.Reports.Projects.rdlc";

                        datasetP.EndInit();

                        // fill data into DataSet
                        testTDTDataSetTableAdapters.ProjSummaryTableAdapter projsTableAdapter = new testTDTDataSetTableAdapters.ProjSummaryTableAdapter();
                        projsTableAdapter.ClearBeforeFill = true;
                        projsTableAdapter.Fill(datasetP.ProjSummary);

                        _reportViewer.RefreshReport();
                        _isReportViewerLoaded = true;

                        break;

                    case "Tracks":
                        Microsoft.Reporting.WinForms.ReportDataSource reportDataSource2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        theTrackInfo datasetT = new theTrackInfo();

                        datasetT.BeginInit();

                        reportDataSource2.Name = "DataSet1"; //  Name of dataset in .RDLC file
                        reportDataSource2.Value = datasetT.Tracks; 
                        _reportViewer.LocalReport.DataSources.Add(reportDataSource2);
                        _reportViewer.LocalReport.ReportEmbeddedResource = "Planner.Reports.Tracks.rdlc";

                        datasetT.EndInit();

                        // fill data into DataSet
                        theTrackInfoTableAdapters.TracksTableAdapter tracksTableAdapter = new theTrackInfoTableAdapters.TracksTableAdapter();
                        tracksTableAdapter.ClearBeforeFill = true;
                        tracksTableAdapter.FillByEndDate(datasetT.Tracks);

                        _reportViewer.RefreshReport();
                        _isReportViewerLoaded = true;

                        break;

                    case "ToDos":
                        Microsoft.Reporting.WinForms.ReportDataSource reportDataSource3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                        testTDTDataSet datasetD = new testTDTDataSet();

                        datasetD.BeginInit();

                        reportDataSource3.Name = "DataSet1"; //  Name of dataset in .RDLC file
                        reportDataSource3.Value = datasetD.ProjSummary; // datasetR.Tracks;  // dataset.Tracks;
                        _reportViewer.LocalReport.DataSources.Add(reportDataSource3);
                        _reportViewer.LocalReport.ReportEmbeddedResource = "Planner.Reports.Projects.rdlc";

                        datasetD.EndInit();

                        // fill data into DataSet
                        testTDTDataSetTableAdapters.ProjSummaryTableAdapter projsTableAdapter = new testTDTDataSetTableAdapters.ProjSummaryTableAdapter();
                        projsTableAdapter.ClearBeforeFill = true;
                        projsTableAdapter.Fill(datasetD.ProjSummary);

                        _reportViewer.RefreshReport();
                        _isReportViewerLoaded = true;

                        break;

                    default:
                        break;
                }

            }
        }

    }
}