using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;

namespace Planner
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ShellViewModel : ViewModelBase
    {
       //// public ObservableCollection<ViewVM> Views { get; set; }

        /// <summary>
        /// Initializes a new instance of the ShellViewModel class.
        /// </summary>
        public ShellViewModel()
        {
            ////ObservableCollection<ViewVM> views = new ObservableCollection<ViewVM>
            ////{
            ////    new ViewVM{ ViewDisplay="ToDos", ViewType = typeof(ToDosView), ViewModelType = typeof(ToDosViewModel)},
            ////    new ViewVM{ ViewDisplay="Tracks", ViewType = typeof(TracksView), ViewModelType = typeof(TracksViewModel)},
            ////    new ViewVM{ ViewDisplay="Planning", ViewType = typeof(PlansView), ViewModelType = typeof(PlansViewModel)},
            ////    new ViewVM{ ViewDisplay="Reports", ViewType = typeof(ReportsView), ViewModelType = typeof(ReportsViewModel)}
            ////};
            ////Views = views;
            ////RaisePropertyChanged("Views");

            // Navigate to ToDos (as default view)
            var defaultView = new ViewVM { ViewDisplay = "ToDos", ViewType = typeof(ToDosView), ViewModelType = typeof(ToDosViewModel) };
            defaultView.NavigateExecute();

            //var msg1 = new ChangeViewMessage { ViewModelType = typeof(ToDosView), ViewType = typeof(ToDosViewModel) };
            //Messenger.Default.Send<ChangeViewMessage>(msg1);
            //  views[0].NavigateExecute();  // Move to an initial view.
            //mLogger.AddLogMessage();
        }

    }
}