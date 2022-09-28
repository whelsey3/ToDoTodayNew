using CodeFirst.EFcf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;
using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;

namespace Planner
{
    /// <summary>
    /// The ViewModel for the LoadOnDemand demo.  This simply
    /// exposes a read-only collection of regions.
    /// </summary>
    public partial class Plans2ViewModel : ViewModelBase //, IDropTarget, IDragSource // CrudVMBaseTDT
    {
        // ======== Derived from CountryViewModel ================
        //    TOP LEVEL FOR PLANS

        #region Data

        readonly TreeViewItemViewModel _rootPerson;
        //readonly ICommand _searchCommand;
        //IEnumerator<TreeViewItemViewModel> _matchingPeopleEnumerator;
        string _searchText = string.Empty;
        public static CodeFirst.EFcf.TDTcfEntities db = new CodeFirst.EFcf.TDTcfEntities();

        public ReadOnlyCollection<TreeViewItemViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }
        readonly ReadOnlyCollection<TreeViewItemViewModel> _firstGeneration;

        public ReadOnlyCollection<dbFolderViewModel> Folders
        {
            get { return _folders; }
        }
        readonly ReadOnlyCollection<dbFolderViewModel> _folders;

        public ObservableCollection<CommandVM> Commands { get; set; }
        public ObservableCollection<ViewVM> Views { get; set; }

        public CustomDragHandlerWH CustomDragHandler { get; private set; }

        static LoggerLib.Logger mLogger = LoggerLib.Logger.Instance;
        #endregion // Data

        ///     public PlansViewModel(TreeViewItem rootPerson) : base()
        public Plans2ViewModel(Folder[] folders, bool LazyLoading) : base()
        {
            #region Constructor
            mLogger.AddLogMessage("--- PlansViewModel ---");
            ////public FamilyTreeViewModel(TreeViewItem rootPerson)
            ////{
            //    _rootPerson = new TreeViewItemViewModel(rootPerson);

            //    _firstGeneration = new ReadOnlyCollection<TreeViewItemViewModel>(
            //        new TreeViewItemViewModel[]
            //        {
            //            _rootPerson
            //        });

            //    //_searchCommand = new SearchFamilyTreeCommand(this);
            //    //}

            #endregion // Constructor
            this.CustomDragHandler = new CustomDragHandlerWH();
            // mLogger = LoggerLib.Logger.Instance;

            ShowUserMessage("Starting Plans2ViewModel!");

            var theFVM = (from folder in folders
                          orderby folder.FSortOrder
                          select new dbFolderViewModel(folder, LazyLoading))
                    .ToList();

            System.Collections.Generic.List<dbFolderViewModel> newList = new System.Collections.Generic.List<dbFolderViewModel>();
            newList = FindParent(theFVM);
            System.Collections.Generic.List<dbFolderViewModel> newList2 = new System.Collections.Generic.List<dbFolderViewModel>();
            newList2.Add(newList[0]);
            #region theFVM nonsense
            dbFolderViewModel root = null;
            dbFolderViewModel lastP = null;
            dbFolderViewModel lastT = null;
            foreach (dbFolderViewModel aFolder in theFVM)
            {
                string FID = aFolder._folder.FSortOrder;
                if (FID == "000000000")
                {
                    // this is root, assume always there
                    root = aFolder;
                    //previousParent = "RootFolder";
                }
                else if (FID.Substring(3, 6) == "000000")
                {
                    // this is project level
                    lastP = aFolder;
                    aFolder.Parent = root;
                    root.Children.Add(aFolder);
                    dbFolderViewModel curParent = (dbFolderViewModel)aFolder.Parent;
                    if (curParent != null)
                    {
                        curParent.Children.Remove(aFolder);
                    }
                    // theFVM.Remove(aFolder);
                }
                else if (FID.Substring(6, 3) == "000")
                {
                    // this is task level
                    lastT = aFolder;
                    aFolder.Parent = lastP;
                    lastP.Children.Add(aFolder);
                    dbFolderViewModel curParent = (dbFolderViewModel)aFolder.Parent;
                    if (curParent != null)
                    {
                        curParent.Children.Remove(aFolder);
                    }
                    // theFVM.Remove(aFolder);
                }
                else
                {
                    // Must be subtask
                    aFolder.Parent = lastT;
                    lastT.Children.Add(aFolder);
                    dbFolderViewModel curParent = (dbFolderViewModel)aFolder.Parent;
                    if (curParent != null)
                    {
                        curParent.Children.Remove(aFolder);
                    }
                    //string FID = aFolder._folder.FSortOrder;
                    theFVM.Remove(aFolder);

                }

                if (aFolder.Parent != null)
                {
                    //           mLogger.AddLogMessage(aFolder.DbID + "  " + aFolder.FolderName + " ->" + aFolder._folder.FSortOrder + " Parent - " + aFolder.Parent.DbID);
                }
                else
                {
                    //          mLogger.AddLogMessage(aFolder.DbID + "  " + aFolder.FolderName + " ->" + aFolder._folder.FSortOrder + " Parent - ");
                }
            }
            //newList.Add(root);

            //_folders = new ReadOnlyCollection<dbFolderViewModel>(
            //        (from folder in folders
            //         orderby folder.FSortOrder
            //         select new dbFolderViewModel(folder, LazyLoading))
            //        .ToList());

            _folders = new ReadOnlyCollection<dbFolderViewModel>(newList2);

            //readonly ReadOnlyCollection<dbFolderViewModel> _AllFoldersWithRoot;
            //_AllFoldersWithRoot = 
            string parentID = "";
            string previousParent = "";
            ////dbFolderViewModel root = null;
            ////dbFolderViewModel lastP = null;
            ////dbFolderViewModel lastT = null;
            //root = null;
            //lastP = null;
            //lastT = null;
            //foreach (dbFolderViewModel aFolder in _folders)
            //{
            //    string FID = aFolder._folder.FSortOrder;
            //    if (FID == "000000000")
            //    {
            //        // this is root
            //        root = aFolder;
            //        //previousParent = "RootFolder";
            //    }
            //    else if (FID.Substring(3,6) == "000000")
            //    {
            //        // this is project level
            //        lastP = aFolder;
            //        dbFolderViewModel curParent = (dbFolderViewModel)aFolder.Parent;
            // //       curParent.Children.Remove(aFolder);
            //        aFolder.Parent = root;
            //        root.Children.Add(aFolder);
            //        //_folders...r
            //    }
            //    else if (FID.Substring(6, 3) == "000")
            //    {
            //        // this is task level
            //        lastT = aFolder;
            //        aFolder.Parent = lastP;
            //    }
            //    else 
            //    {
            //        // Must be subtask
            //        aFolder.Parent = lastT;
            //        //string FID = aFolder._folder.FSortOrder;
            //    }

            //    if (aFolder.Parent != null)
            //    {
            //        mLogger.AddLogMessage(aFolder.FolderName + " ->" + aFolder._folder.FSortOrder + " Parent - " + aFolder.Parent.DbID);
            //    }
            //    else
            //    {
            //        mLogger.AddLogMessage(aFolder.FolderName + " ->" + aFolder._folder.FSortOrder + " Parent - ");
            //    }
            //}
            #endregion
            ShowUserMessage("Finished populating data.");
            RaisePropertyChanged("_folders");

            #region Commands and Views
            // Set up Commands and Views for menu display ============>
            ObservableCollection<ViewVM> views = new ObservableCollection<ViewVM>
            {
               // new ViewVM{ ViewDisplay="ToDos", ViewType = typeof(ToDosView), ViewModelType = typeof(ToDosViewModel)},
                new ViewVM{ ViewDisplay="ToDos",  ViewType = typeof(ToDosView),  ViewModelType = typeof(ToDosViewModel)},
                new ViewVM{ ViewDisplay="Tracks", ViewType = typeof(TracksView), ViewModelType = typeof(TracksViewModel)},
                new ViewVM{ ViewDisplay="Reports", ViewType = typeof(ReportsView), ViewModelType = typeof(ReportsViewModel)}
                //new ViewVM{ ViewDisplay="Projects", ViewType = typeof(ProjectView), ViewModelType = typeof(CountryViewModel)}

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
            #endregion
            System.Collections.Generic.List<Project> baseLineBatch = new List<Project>();
            baseLineBatch = (from c in db.Projects
                                 //where (c.PSortOrder.Substring(0, ccS.Length) == ccS
                                 //&&
                                 //c.FolderID == sourceProj.FolderID)
                                 //&&
                                 //c.ProjectID != sourceProj.ProjectID  // exclude chance of Source in Batch
                             orderby c.FolderID, c.PSortOrder
                             select c).ToList();
            LogArray(baseLineBatch, "baseLine TreeView");
            //inWorkToDo = null;
        }

        protected void ShowUserMessage(string message)
        {
            UserMessage msg = new UserMessage { Message = message };
            Messenger.Default.Send<UserMessage>(msg);
        }

        private System.Collections.Generic.List<dbFolderViewModel> FindParent(System.Collections.Generic.List<dbFolderViewModel> baseFolders)
        {
            foreach (var aFolder in baseFolders)
            {
                // Identify level P T S and get Parent ID
                string parentID = GetParentID(aFolder._folder.FSortOrder);
                if (parentID == "")
                {
                    aFolder.Parent = null;
                }
                else
                {
                    // Get Parent
                    dbFolderViewModel theParent = (from sFolder in baseFolders
                                                   where sFolder._folder.FSortOrder == parentID
                                                   select sFolder).FirstOrDefault();
                    // Set Parent
                    aFolder.Parent = theParent;
                    // Increment Parent.Children
                    theParent.Children.Add(aFolder);
                    mLogger.AddLogMessage(aFolder.FolderName + "-->FindParent: " + aFolder.DbID + " " + theParent.DbID + " " + theParent.Children.Count.ToString());
                }
            }
            //dbFolderViewModel find = (from sFolder in theFVM
            //                          where sFolder._folder.FSortOrder == "000000000"
            //                          select sFolder).FirstOrDefault();
            return baseFolders;
        }

        private string GetParentID(string FID)
        {
            string ParentID = "";
            //string FID = aFolder._folder.FSortOrder;
            if (FID == "000000000")
            {
                // this is root, assume always there
                ParentID = "";
            }
            else if (FID.Substring(3, 6) == "000000")
            {
                // this is project level, parent must be root
                ParentID = "000000000";
            }
            else if (FID.Substring(6, 3) == "000")
            {
                // this is task level
                ParentID = FID.Substring(0, 3) + "000000";
            }
            else
            {
                // Must be subtask
                ParentID = FID.Substring(0, 6) + "000";
            }
            mLogger.AddLogMessage(FID + " - " + ParentID);
            return ParentID;
        }

    }
}