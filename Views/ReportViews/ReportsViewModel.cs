using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
//using LoggerLib;
using nuT3;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Reporting.WinForms;

namespace Planner
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ReportsViewModel : ViewModelBase
    {
        #region Properties
        Logger mLogger = Logger.Instance;

        public ObservableCollection<CommandVM> Commands { get; set; }
        public ObservableCollection<ViewVM> Views { get; set; }
        public CommandVM Report1Cmd { get; set; }

        //private ToDoVM editVM;
        //public ToDoVM EditVM
        //{
        //    get
        //    {
        //        return editVM;
        //    }
        //    set
        //    {
        //        editVM = value;
        //        editEntity = editVM.TheEntity;
        //        RaisePropertyChanged();
        //    }
        //}

        //private ToDoVM selectedFolder;
        //public ToDoVM SelectedFolder
        //{
        //    get
        //    {
        //        return selectedFolder;
        //    }
        //    set
        //    {
        //        selectedFolder = value;
        //        selectedEntity = value;
        //    }
        //}

        //private string titlePopUp;
        //public string TitlePopUp
        //{
        //    get
        //    {
        //        return titlePopUp;
        //    }
        //    set
        //    {
        //        titlePopUp = value;
        //        RaisePropertyChanged();
        //    }
        //}

        //private string titlePopUp1;
        //public string TitlePopUp1
        //{
        //    get
        //    {
        //        return titlePopUp1;
        //    }
        //    set
        //    {
        //        titlePopUp1 = value;
        //        RaisePropertyChanged();
        //    }
        //}

        //private TrackVM trackVM;
        //public TrackVM TrackVM
        //{
        //    get
        //    {
        //        return trackVM;
        //    }
        //    set
        //    {
        //        trackVM = value;
        //        trackEntity = trackVM.TheEntity;
        //        RaisePropertyChanged();
        //    }
        //}

        public ObservableCollection<ToDoVM> ToDos { get; set; }
        public ObservableCollection<TrackVM> Tracks { get; set; }
    //    ReportViewer theViewer;
        public string cReptName { get; set; }

        //private StatusOptions statusOpt = StatusOptions.F;
        //public StatusOptions StatusOpt
        //{
        //    get { return statusOpt; }
        //    set { statusOpt = value; RaisePropertyChanged("StatusOpt"); }
        //}

        //ToDo inWorkToDo;
        //DateTime SwitchDate;

        #endregion

        /// <summary>
        /// Initializes a new instance of the ReportsViewModel class.
        /// </summary>
        public ReportsViewModel(string theName)
        {
            mLogger.AddLogMessage("--- ReportsViewModel ---");
            //theViewer = rv;
            cReptName = theName;
            //#region Views_and_Commands
            //ObservableCollection<ViewVM> views = new ObservableCollection<ViewVM>
            //{
            //    new ViewVM{ ViewDisplay="ToDos", ViewType = typeof(ToDosView), ViewModelType = typeof(ToDosViewModel)},
            //    new ViewVM{ ViewDisplay="Tracks",   ViewType = typeof(TracksView), ViewModelType = typeof(TracksViewModel)},
            //    //new ViewVM{ ViewDisplay="Projects", ViewType = typeof(ProjectView), ViewModelType = typeof(CountryViewModel)},
            //    //new ViewVM{ ViewDisplay="DBProjects", ViewType = typeof(DBProjectView), ViewModelType = typeof(TopViewModel)},
            //    new ViewVM{ ViewDisplay="PlansViews", ViewType = typeof(PlansView), ViewModelType = typeof(PlansViewModel)}
            //    //new ViewVM{ ViewDisplay="Reports", ViewType = typeof(ReportsView), ViewModelType = typeof(ReportsViewModel)}
            //};
            //Views = views;
            //RaisePropertyChanged("Views");
            ////Views[0].NavigateExecute();   // Navigate to first ViewVM in Views

            //ObservableCollection<CommandVM> commands = new ObservableCollection<CommandVM>
            //{
            //   // new CommandVM{CommandDisplay="Insert", Application.Current.Resources. x:Key="InsertIcon" , Message=new CommandMessage{ Command =CommandType.Insert}},
            //    new CommandVM{ CommandDisplay="Run Report1",        IconGeometry=Application.Current.Resources["Report1Icon"]             as Geometry , Message=new CommandMessage{ Command = CommandType.Report1} },
            //    new CommandVM{ CommandDisplay="New Work Item",      IconGeometry=Application.Current.Resources["InsertIcon"]    as Geometry , Message=new CommandMessage{ Command =CommandType.Insert} },
            //    new CommandVM{ CommandDisplay="Edit",        IconGeometry=Application.Current.Resources["EditIcon"]             as Geometry , Message=new CommandMessage{ Command = CommandType.Edit} },
            //    new CommandVM{ CommandDisplay="Delete Work Item",      IconGeometry=Application.Current.Resources["DeleteIcon"] as Geometry , Message=new CommandMessage{ Command = CommandType.Delete} },
            //    new CommandVM{ CommandDisplay="Refresh Data",     IconGeometry=Application.Current.Resources["RefreshIcon"]     as Geometry , Message=new CommandMessage{ Command = CommandType.Refresh} },
            //    new CommandVM{ CommandDisplay="Start Timing", IconGeometry=Application.Current.Resources["StartTimingIcon"]     as Geometry , Message=new CommandMessage{ Command = CommandType.StartTiming} },
            //    new CommandVM{ CommandDisplay="Stop Timing",    IconGeometry=Application.Current.Resources["StopTimingIcon"]    as Geometry , Message=new CommandMessage{ Command = CommandType.StopWork} },
            //    new CommandVM{ CommandDisplay="End of Day",     IconGeometry=Application.Current.Resources["CleanUpIcon"]       as Geometry , Message=new CommandMessage{ Command = CommandType.CleanUp} }
            //};
            //Commands = commands;
            //RaisePropertyChanged("Commands");
            //#endregion

            Messenger.Default.Register<CommandMessage>(this, (action) => HandleCommand(action));

            Report1Cmd = new CommandVM
            {
                CommandDisplay = "Run Report1",
                IconGeometry = Application.Current.Resources["Report1Icon"] as Geometry,
                Message = new CommandMessage { Command = CommandType.Report1 }
            };
        }

        protected void HandleCommand(CommandMessage action)
        {
            mLogger.AddLogMessage("### Command MSG received by Reports: " + action.Command);

            //if (isCurrentView)
            //{

            switch (action.Command)
                {
                    case CommandType.Report1:
                       // runReport1();
                        break;
                    default:
                        mLogger.AddLogMessage("In default in ReportsViewModel.Handle()");
                        break;
                }
            //}

        }

       // protected void runReport1()
       // {
       //     mLogger.AddLogMessage("!!!  runReport1 now running");

       //     ShowReport rept = new ShowReport();
       //    // rept.SizeToContent = SizeToContent.WidthAndHeight;
       //     rept.WindowState = WindowState.Maximized;
       //     rept.ShowDialog();
       //     /*
       //     Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
       //     ToDoToday9DataSet dataset1 = new ToDoToday9DataSet();
       //     //dataset.Tracks.OrderByDescending(trac);

       //     dataset1.BeginInit();

       //     reportDataSource1.Name = "DataSet1"; //  Name of dataset in .RDLC file
       //     reportDataSource1.Value = dataset1.Tracks;
       //     this.theViewer.LocalReport.DataSources.Add(reportDataSource1);
       //     this.theViewer.LocalReport.ReportEmbeddedResource = "Planner.ReportTracks.rdlc";

       //     dataset1.EndInit();

       //     // fill data into DataSet
       //     ToDoToday9DataSetTableAdapters.TracksTableAdapter tracksTableAdapter = new ToDoToday9DataSetTableAdapters.TracksTableAdapter();
       //     tracksTableAdapter.ClearBeforeFill = true;
       //     tracksTableAdapter.Fill(dataset1.Tracks);
       ////     tracksTableAdapter.FillByDateDesc(dataset.Tracks);
       //     // tracksTableAdapter.s

       //     theViewer.RefreshReport();
       //    // _isReportViewerLoaded = true;

       //     ///theViewer.RefreshReport();
       //     */
       // }

    }
}