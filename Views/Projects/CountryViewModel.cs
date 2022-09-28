using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CodeFirst.EFcf;

using GalaSoft.MvvmLight;

namespace Planner
{
    /// <summary>
    /// The ViewModel for the LoadOnDemand demo.  This simply
    /// exposes a read-only collection of regions.
    /// </summary>
    public class CountryViewModel : ViewModelBase // CrudVMBaseTDT
    {
        readonly ReadOnlyCollection<FolderViewModel> _folders;
        public ObservableCollection<CommandVM> Commands { get; set; }
        public ObservableCollection<ViewVM> Views { get; set; }

        public CountryViewModel(Folder[] folders, bool LazyLoading) : base()
        {
            _folders = new ReadOnlyCollection<FolderViewModel>(
                (from folder in folders
                 select new FolderViewModel(folder, LazyLoading))
                .ToList());

            ObservableCollection<ViewVM> views = new ObservableCollection<ViewVM>
            {
               // new ViewVM{ ViewDisplay="ToDos", ViewType = typeof(ToDosView), ViewModelType = typeof(ToDosViewModel)},
                new ViewVM{ ViewDisplay="ToDos",  ViewType = typeof(ToDosView),  ViewModelType = typeof(ToDosViewModel)},
                new ViewVM{ ViewDisplay="Tracks", ViewType = typeof(TracksView), ViewModelType = typeof(TracksViewModel)}
                //new ViewVM{ ViewDisplay="Folders", ViewType = typeof(FoldersView), ViewModelType = typeof(FolderViewModel)}
            };
            Views = views;
            RaisePropertyChanged("Views");
            //Views[0].NavigateExecute();   // Navigate to first ViewVM in Views

            ObservableCollection<CommandVM> commands = new ObservableCollection<CommandVM>
            {
               // new CommandVM{CommandDisplay="Insert", Application.Current.Resources. x:Key="InsertIcon" , Message=new CommandMessage{ Command =CommandType.Insert}},
                new CommandVM{ CommandDisplay="New Work Item",      IconGeometry=Application.Current.Resources["InsertIcon"]      as Geometry , Message=new CommandMessage{ Command =CommandType.Insert} },
                new CommandVM{ CommandDisplay="Edit",        IconGeometry=Application.Current.Resources["EditIcon"]        as Geometry , Message=new CommandMessage{ Command = CommandType.Edit} },
                new CommandVM{ CommandDisplay="Delete Work Item",      IconGeometry=Application.Current.Resources["DeleteIcon"]      as Geometry , Message=new CommandMessage{ Command = CommandType.Delete} },
                new CommandVM{ CommandDisplay="Refresh Data",     IconGeometry=Application.Current.Resources["RefreshIcon"]     as Geometry , Message=new CommandMessage{ Command = CommandType.Refresh} },
                new CommandVM{ CommandDisplay="Start Timing", IconGeometry=Application.Current.Resources["StartTimingIcon"] as Geometry , Message=new CommandMessage{ Command = CommandType.StartTiming} },
                new CommandVM{ CommandDisplay="Stop Timing",    IconGeometry=Application.Current.Resources["StopTimingIcon"]  as Geometry , Message=new CommandMessage{ Command = CommandType.StopWork} },
                new CommandVM{ CommandDisplay="New ToDo List",     IconGeometry=Application.Current.Resources["CleanUpIcon"]     as Geometry , Message=new CommandMessage{ Command = CommandType.CleanUp} }
            };
            Commands = commands;
            RaisePropertyChanged("Commands");
            //inWorkToDo = null;

        }

        public ReadOnlyCollection<FolderViewModel> Folders
        {
            get { return _folders; }
        }
    }
}