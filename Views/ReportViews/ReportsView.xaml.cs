using System.Windows;

namespace Planner
{
    /// <summary>
    /// Description for ReportsView.
    /// </summary>
    public partial class ReportsView : Window
    {
        private bool _isReportViewerLoaded;
        private string cReptNameIn;
        public System.Windows.Forms.BindingSource ReportBindingSource;
        public Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1;

        //    public string cReptName { get; set; }
        /// <summary>
        /// Initializes a new instance of the ReportsView class.
        /// </summary>
        public ReportsView(string theRept = "Projects")
        {
            InitializeComponent();
            if (theRept != "Utility")
            {

                this.Title =  " Reports: " + theRept;
                cReptNameIn = theRept;
                _reportViewer.Load += ReportViewer_Load;

                ReportBindingSource = new System.Windows.Forms.BindingSource();
                reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();

                DataContext = new ReportsViewModel(theRept);
            }
        }

        private void ReportViewer_Load(object sender, System.EventArgs e)
        {
            if (!_isReportViewerLoaded)
            {
                switch (cReptNameIn)
                {

                    case "Projects":

                        // ==============================================>
                        reportDataSource1.Name = "dsDispProj";
                        reportDataSource1.Value = ReportBindingSource;

                        _reportViewer.LocalReport.DataSources.Add(reportDataSource1);
                        _reportViewer.LocalReport.ReportEmbeddedResource = "Planner.Reports.ReptNewOne.rdlc";

                        dispProjects m_Projects = new dispProjects();

                        ReportBindingSource.DataSource = m_Projects.GetProjects();

                        _reportViewer.RefreshReport();

                        // ==============================================>


                        //// ==============================================>
                        //reportDataSource1.Name = "DataSet1";
                        //reportDataSource1.Value = ReportBindingSource;

                        //_reportViewer.LocalReport.DataSources.Add(reportDataSource1);
                        //_reportViewer.LocalReport.ReportEmbeddedResource = "Planner.Reports.Projects.rdlc";

                        //reptProjects m_Projects = new reptProjects();

                        //ReportBindingSource.DataSource = m_Projects.GetProjects();

                        //_reportViewer.RefreshReport();

                        //// ==============================================>

                        break;

                    case "Tracks":

                        // ==============================================>
                        reportDataSource1.Name = "DataSet1";
                        reportDataSource1.Value = ReportBindingSource;

                        _reportViewer.LocalReport.DataSources.Add(reportDataSource1);
                        _reportViewer.LocalReport.ReportEmbeddedResource = "Planner.Reports.Tracks.rdlc";

                        reptTracks m_Tracks = new reptTracks();

                        ReportBindingSource.DataSource = m_Tracks.GetTracks();  // typeof(Track);

                        _reportViewer.RefreshReport();

                        // ==============================================>

                        break;

                    case "ToDos":

                        // ==============================================>
                        reportDataSource1.Name = "DataSet1";
                        reportDataSource1.Value = ReportBindingSource;

                        _reportViewer.LocalReport.DataSources.Add(reportDataSource1);
                        _reportViewer.LocalReport.ReportEmbeddedResource = "Planner.Reports.ToDos.rdlc";

                        reptToDos m_ToDos = new reptToDos();

                        ReportBindingSource.DataSource = m_ToDos.GetToDos();

                        _reportViewer.RefreshReport();

                        // ==============================================>

                        break;

                    case "sumTracks":

                        // ==============================================>
                        reportDataSource1.Name = "DataSet1";
                        reportDataSource1.Value = ReportBindingSource;

                        _reportViewer.LocalReport.DataSources.Add(reportDataSource1);
                        _reportViewer.LocalReport.ReportEmbeddedResource = "Planner.Reports.Report2b.rdlc";

                        reptSumTracks m_Tracks2 = new reptSumTracks();

                       ReportBindingSource.DataSource = m_Tracks2.GetTracks();  // typeof(Track);
                        //ReportBindingSource.DataSource = reptSumTracks();  // typeof(Track);

                        _reportViewer.RefreshReport();

                        // ==============================================>

                        break;

                    default:
                        break;
                }

            }
        }

    }
}