using BuildSqliteCF.Entity;
//using BuildSqliteCF.DbContexts;
using GalaSoft.MvvmLight;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace Planner
{
    public partial class PlansViewModel : ViewModelBase, IDropTarget, IDragSource // CrudVMBaseTDT
    {
        // ======== Derived from CountryViewModel ================
        //    TOP LEVEL FOR PLANS

        #region Data

      //  readonly ReadOnlyCollection<TreeViewItemViewModel> _firstGeneration;
       // readonly TreeViewItemViewModel _rootPerson;
        //readonly ICommand _searchCommand;
        //IEnumerator<TreeViewItemViewModel> _matchingPeopleEnumerator;
        string _searchText = string.Empty;

        #endregion // Datau
        //public PlanDetails Details { get; set; }
        // Change to use VM for Projects
        protected object editEntity;   // From legacy, needed???

        private ProjectVM detailsVM;
        public ProjectVM DetailsVM
        {
            get
            {
                return detailsVM;
            }
            set
            {
                detailsVM = value;
                if (detailsVM != null)
                {
                    editEntity = detailsVM.TheEntity;
                }
                RaisePropertyChanged();
            }
        }

        private ProjectVM editPlan;
        public ProjectVM EditPlan
        {
            get
            {
                return editPlan;
            }
            set
            {
                editPlan = value;
                if (editPlan != null)
                {
                    editEntity = editPlan;
                }
                RaisePropertyChanged();
            }
        }
        public PlanDetails dispDetailsVM { get; set; }  // was ProjectVM
        //private ProjectVM dispdetailsVM;
        //public ProjectVM dispDetailsVM
        //{
        //    get
        //    {
        //        return dispdetailsVM;
        //    }
        //    set
        //    {
        //        dispdetailsVM = value;
        //        if (dispdetailsVM != null)
        //        {
        //           // editEntity = dispdetailsVM.TheEntity;
        //        }
        //       // RaisePropertyChanged();
        //    }
        //}

        // public ObservableCollection<dbFolderViewModel> _folders { get; set; }
        public ObservableCollection<CommandVM> Commands { get; set; }
        public ObservableCollection<ViewVM> Views { get; set; }
        public ObservableCollection<ReportVM> Reports { get; set; }  // not really needed

        public Logger mLogger = Logger.Instance;
        public TDTDbContext db; // = new BuildSqliteCF.DbContexts.TDTDbContext("BillWork.db");
        //        protected BuildSqliteCF.DbContexts.TDTDbContext db = new BuildSqliteCF.DbContexts.TDTDbContext();
        public CustomDragHandlerWH CustomDragHandler { get; private set; }

        private Project selectedProject;
        public Project SelectedProject
        {
            get
            {
                return selectedProject;
            }
            set
            {
                selectedProject = value;
                //selectedEntity = value;
                if (value != null)
                {
                   // mLogger.AddLogMessage("set in ToDosViewModel SelectedToDo->" + ((PlanDetails)value).TheEntity.Item);
                }
                RaisePropertyChanged();
            }
        }

        TreeView theTV;
        TreeViewItem theTVItem;

        public TreeViewItemViewModel selectedTVItem;
        public int? RoutineTasksFolderID;

        public CommandVM SaveEditCmd { get; set; }
        public CommandVM QuitEditCmd { get; set; }

        #region GetFolders

        public BuildSqliteCF.Entity.Folder[] GetFolders()
        {
            var theFolders0 = (from c in db.Folders
                                    orderby c.FSortOrder
                                    select c).AsNoTracking().ToArray();

            var theFolders = (from c in db.Folders
                              orderby c.FSortOrder
                              select c).AsNoTracking().ToList();
            BuildSqliteCF.Entity.Folder[] fs = new BuildSqliteCF.Entity.Folder[theFolders.Count];
            int i = 0;
            foreach (var item in theFolders)
            {
                fs[i] = (BuildSqliteCF.Entity.Folder)theFolders[i];
                fs[i].Projects = null;
                i++;
            }
            return fs; //  theFolders.ToArray<Folder>;

        }
        #endregion // GetFolders


        ///     public PlansViewModel(TreeViewItem rootPerson) : base()
//        public PlansViewModel(Folder[] folders, bool LazyLoading) : base()
        public PlansViewModel() : base()
        {
            #region Constructor
            mLogger.AddLogMessage("--- PlansViewModel ---");

            string theFileName = (string)App.Current.Properties["destFilePath"];
            RoutineTasksFolderID = (int?)App.Current.Properties["RoutineTasksFolderID"];

            //db = new TDTDbContext(theFileName);
            db = ((App)Application.Current).db;   // new TDTDbContext();
            //db = new TDTDbContext();
            ////db.Folders.AsNoTracking();
            ////db.Projects.AsNoTracking();
            this.CustomDragHandler = new CustomDragHandlerWH();

            #endregion // Constructor
            // mLogger = LoggerLib.Logger.Instance;

            //   Folder[] folders = GetFolders();
            //   bool LazyLoading = false;

            //   var theFVM = (from folder in folders
            //                 orderby folder.FSortOrder
            //                 select new dbFolderViewModel(folder, LazyLoading, db))
            //           .ToList();

            //   System.Collections.Generic.List<dbFolderViewModel> newList = new System.Collections.Generic.List<dbFolderViewModel>();
            //   newList = FindParent(theFVM);
            //   System.Collections.Generic.List<dbFolderViewModel> newList2 = new System.Collections.Generic.List<dbFolderViewModel>();
            //   newList2.Add(newList[0]);
            //   dbFolderViewModel root = null;
            //   dbFolderViewModel lastP = null;
            //   dbFolderViewModel lastT = null;
            //   foreach (dbFolderViewModel aFolder in theFVM)
            //   {
            //       string FID = aFolder._folder.FSortOrder;
            //       if (FID == "000000000")
            //       {
            //           // this is root, assume always there
            //           root = aFolder;
            //           //previousParent = "RootFolder";
            //       }
            //       else if (FID.Substring(3, 6) == "000000")
            //       {
            //           // this is project level
            //           lastP = aFolder;
            //           aFolder.Parent = root;
            //           root.Children.Add(aFolder);
            //           dbFolderViewModel curParent = (dbFolderViewModel)aFolder.Parent;
            //           if (curParent != null)
            //           {
            //               curParent.Children.Remove(aFolder);
            //           }
            //           // theFVM.Remove(aFolder);
            //       }
            //       else if (FID.Substring(6, 3) == "000")
            //       {
            //           // this is task level
            //           lastT = aFolder;
            //           aFolder.Parent = lastP;
            //           lastP.Children.Add(aFolder);
            //           dbFolderViewModel curParent = (dbFolderViewModel)aFolder.Parent;
            //           if (curParent != null)
            //           {
            //               curParent.Children.Remove(aFolder);
            //           }
            //           // theFVM.Remove(aFolder);
            //       }
            //       else
            //       {
            //           // Must be subtask
            //           aFolder.Parent = lastT;
            //           lastT.Children.Add(aFolder);
            //           dbFolderViewModel curParent = (dbFolderViewModel)aFolder.Parent;
            //           if (curParent != null)
            //           {
            //               curParent.Children.Remove(aFolder);
            //           }
            //           //string FID = aFolder._folder.FSortOrder;
            //           theFVM.Remove(aFolder);

            //       }

            //       if (aFolder.Parent != null)
            //       {
            ////           mLogger.AddLogMessage(aFolder.DbID + "  " + aFolder.FolderName + " ->" + aFolder._folder.FSortOrder + " Parent - " + aFolder.Parent.DbID);
            //       }
            //       else
            //       {
            // //          mLogger.AddLogMessage(aFolder.DbID + "  " + aFolder.FolderName + " ->" + aFolder._folder.FSortOrder + " Parent - ");
            //       }
            //   }
            //   //newList.Add(root);

            //   //_folders = new ReadOnlyCollection<dbFolderViewModel>(
            //   //        (from folder in folders
            //   //         orderby folder.FSortOrder
            //   //         select new dbFolderViewModel(folder, LazyLoading))
            //   //        .ToList());

            //   _folders = new ReadOnlyCollection<dbFolderViewModel>(newList2);

            Folders = GetData();
            //_folders = GetData();

            //readonly ReadOnlyCollection<dbFolderViewModel> _AllFoldersWithRoot;
            //_AllFoldersWithRoot = 
      //      string parentID = "";
      //      string previousParent = "";

            //RaisePropertyChanged("_folders");  MOVED TO GetData()
            // Set up Commands and Views for menu display ============>
            ObservableCollection<ViewVM> views = new ObservableCollection<ViewVM>
            {
                //Views[2],
               // new ViewVM{ ViewDisplay="ToDos", ViewType = typeof(ToDosView), ViewModelType = typeof(ToDosViewModel)},
                new ViewVM{ ViewDisplay="ToDos",  ViewType = typeof(ToDosView),  ViewModelType = typeof(ToDosViewModel)},
                new ViewVM{ ViewDisplay="Tracks", ViewType = typeof(TracksView), ViewModelType = typeof(TracksViewModel)},
          //      new ViewVM{ ViewDisplay="Reports", ViewType = typeof(ReportsView), ViewModelType = typeof(ReportsViewModel)}
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
               // new CommandVM{ CommandDisplay="Start Timing", IconGeometry=Application.Current.Resources["StartTimingIcon"] as Geometry , Message=new CommandMessage{ Command = CommandType.StartTiming} },
              //  new CommandVM{ CommandDisplay="Stop Timing",    IconGeometry=Application.Current.Resources["StopTimingIcon"]  as Geometry , Message=new CommandMessage{ Command = CommandType.StopWork} },
              //  new CommandVM{ CommandDisplay="New ToDo List",     IconGeometry=Application.Current.Resources["CleanUpIcon"]     as Geometry , Message=new CommandMessage{ Command = CommandType.CleanUp} }
            };
            Commands = commands;
            RaisePropertyChanged("Commands");


            ObservableCollection<ReportVM> _reports = new ObservableCollection<ReportVM>
            {
                new ReportVM { ReportDisplay = "aReport1" },
                new ReportVM { ReportDisplay = "aReport2" }
            };
            Reports = _reports;
            RaisePropertyChanged("Reports");

      //      DetailsVM = new ProjectVM();    // working on display of TV details
      //      DetailsVM.Selection = "NOTHINg Selected";

            dispDetailsVM = new PlanDetails();    // working on display of TV details
            dispDetailsVM.Selection = "Nothing Selected";

            //inWorkToDo = null;

            Messenger.Default.Register<CommandMessage>(this, (action) => HandlePlansCommand(action));
            // Listen for Navigation messages to update isCurrentView.
         //   Messenger.Default.Register<NavigateMessage>(this, (action) => CurrentUserControl(action));
            Messenger.Default.Register<CurrentViewMessage>(this, (action) => CurrentUserControl(action));

            SaveCmd = new CommandVM
            {
                CommandDisplay = "Commit",
                IconGeometry = Application.Current.Resources["SaveIcon"] as Geometry,
                Message = new CommandMessage { Command = CommandType.Commit }
            };

            //CommitStartTimingCmd = new CommandVM
            //{
            //    CommandDisplay = "Commit Start Timing",
            //    IconGeometry = Application.Current.Resources["SaveIcon"] as Geometry,
            //    Message = new CommandMessage { Command = CommandType.CommitStartTiming }
            //};

            QuitCmd = new CommandVM
            {
                CommandDisplay = "Quit",
                IconGeometry = Application.Current.Resources["QuitIcon"] as Geometry,
                Message = new CommandMessage { Command = CommandType.Quit }
            };

            QuitEditCmd = new CommandVM
            {
                CommandDisplay = "Quit Edit",
                IconGeometry = Application.Current.Resources["QuitIcon"] as Geometry,
                Message = new CommandMessage { Command = CommandType.QuitEdit }
            };

            SaveEditCmd = new CommandVM
            {
                CommandDisplay = "Commit Edit",
                IconGeometry = Application.Current.Resources["SaveIcon"] as Geometry,
                Message = new CommandMessage { Command = CommandType.CommitEdit }
            };

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
                //    mLogger.AddLogMessage(aFolder.FolderName + "-->FindParent: " + aFolder.DbID + " " + theParent.DbID + " " + theParent.Children.Count.ToString() );
                }
            }
            //dbFolderViewModel find = (from sFolder in theFVM
            //                          where sFolder._folder.FSortOrder == "000000000"
            //                          select sFolder).FirstOrDefault();
            return baseFolders;
        }

        public string GetParentID(string FID)
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
          //  mLogger.AddLogMessage("GetParentID:  " + FID + " - ParentID->" + ParentID);
            return ParentID;
        }

        //public ReadOnlyCollection<TreeViewItemViewModel> FirstGeneration
        //{
        //    get { return _firstGeneration; }
        //}
        private ObservableCollection<dbFolderViewModel> _folders;
        public ObservableCollection<dbFolderViewModel> Folders
        {
            get { return _folders; }
            set { _folders = value; }
        }

        public void HandleOnItemSelected(object sender, System.Windows.RoutedEventArgs e)
        {
            theTV = (TreeView)sender;
            theTVItem = (TreeViewItem)(e.OriginalSource);

            object item = theTVItem.Header;
            selectedTVItem = (TreeViewItemViewModel)item;
            //TreeView theTV = (TreeView)sender;
            //TreeViewItem theTVItem = (TreeViewItem)(e.OriginalSource);

            if (item == null)
            {
                e.Handled = true;
                return;
            }
       //     theTVItem.Focus();
            Type theTypeType = item.GetType();
            string theType = theTypeType.Name;

            //tv.Tag = e.OriginalSource;
            //mLogger.AddLogMessage("OnItemSelected-" + ((TreeViewItem)(tv.Tag)).Header.GetType());
            //Type theTypeType = ((System.Windows.Controls.TreeViewItem)(tv.Tag)).Header.GetType();
            //object item = ((TreeViewItem)(tv.Tag)).Header;
            //string theType = theTypeType.Name;
            string tbName = "";
            Project headProj = null;
          //  dispDetailsVM = new ProjectVM();

            if (theType.Contains("Folder"))
            {
                // Don't delete Folders
                // Don't display details for folders
                dispDetailsVM = new PlanDetails();
                dispDetailsVM.Selection = "Folder Selected";
                dispDetailsVM.Item = ((dbFolderViewModel)item).FolderName;
                dispDetailsVM.FolderID = DbIDtoFolderID(((dbFolderViewModel)item).DbID);  // display FolderID
                RaisePropertyChanged("dispDetailsVM");
                SelectedProject = null;
                return;
            }
            else if (theType.Contains("Project"))
            {
                dispDetailsVM.Selection = "Selected Project:";
                // Database theDB = new Database();
                headProj = ((dbProjectViewModel)item)._project;
                tbName = ((dbProjectViewModel)item).ProjectName;
                //headID = headProj.ProjectID;
                //srchString = headProj.PPartNum.Substring(0, 3);// + "000000";

            //    ((PlansViewModel)DataContext).Details.Item = tbName;

            }
            else if (theType.Contains("SubTask"))
            {
                dispDetailsVM.Selection = "Selected SubTask:";
               //Database theDB = new Database();
                headProj = ((dbSubTaskViewModel)item)._subTask;
                tbName = ((dbSubTaskViewModel)item).SubTaskName;
                //headID = headProj.ProjectID;
                // srchString = headProj.PPartNum.Substring(0, 3);// + "";

            }
            else if (theType.Contains("Task"))
            {
                dispDetailsVM.Selection = "Selected Task:";
               //Database theDB = new Database();
                headProj = ((dbTaskViewModel)item)._task;
                tbName = ((dbTaskViewModel)item).TaskName;
                //headID = headProj.ProjectID;
                //srchString = headProj.PPartNum.Substring(0, 6);// + "000";

            }

            //  this.theItem.Text = tbName;
          //  Details = new PlanDetails();
            dispDetailsVM.Item = headProj.Item;
       //     dispDetailsVM.Selection = dispDetailsVM.Selection + " " + dispDetailsVM.Item;
            dispDetailsVM.PPartNum = headProj.PPartNum;
            dispDetailsVM.Priority = headProj.Priority;
            dispDetailsVM.Status = headProj.Status;
            dispDetailsVM.RevDueDate = headProj.RevDueDate;
            dispDetailsVM.DetailedDesc = headProj.DetailedDesc;
            dispDetailsVM.FolderID = headProj.FolderID;
            //dispDetailsVM.PSortOrder = headProj.PSortOrder;
            //dispDetailsVM.RespPerson = headProj.RespPerson;
            //dispDetailsVM.DispLevel = headProj.DispLevel;
            //dispDetailsVM.DoneDate = headProj.DoneDate;
            //dispDetailsVM.ProjectID = headProj.ProjectID;

            RaisePropertyChanged("dispDetailsVM");

            SelectedProject = headProj;



        }
        // ((PlansViewModel)(((PlansView)(Holder.Content)).DataContext)
        public ObservableCollection<dbFolderViewModel> GetData()
        {
          //  FixGetData();
            Folder[] folders = GetFolders();
            bool LazyLoading = false;

            var theFVM = (from folder in folders
                          orderby folder.FSortOrder
                          select new dbFolderViewModel(folder, LazyLoading, db))
                    .ToList();

            System.Collections.Generic.List<dbFolderViewModel> newList = new System.Collections.Generic.List<dbFolderViewModel>();
            newList = FindParent(theFVM);
            System.Collections.Generic.List<dbFolderViewModel> newList2 = new System.Collections.Generic.List<dbFolderViewModel>();
            newList2.Add(newList[0]);
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
              //      theFVM.Remove(aFolder);

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

            ObservableCollection<dbFolderViewModel> _folders = new ObservableCollection<dbFolderViewModel>(newList2);
            Folders = _folders;
            RaisePropertyChanged("Folders");
       ///     RaisePropertyChanged("_folders");
            mLogger.AddLogMessage("Finished GetData for PlansViewModel.");
            return _folders;
        }

        public Project GetNewProj(string name, TreeViewItemViewModel parent)
        {
          //  string parentType = parent.GetType().ToString();
            Project newProj = new Project();
            //if (DetailsVM.IsNew)
            //{
            //    // Using full dialog
            //    DetailsVM.IsNew = false;
            //    newProj = DetailsVM.TheEntity;
            //    newProj.ProjectID = 0;
            //}
            //else
            //{
            //    newProj = new Project(name);  // Need to add additional detail values
            ////// Set FolderID based on the Parent
            ////string f = parent.DbID;
            ////if (f.Contains("F"))
            ////{
            ////    f = f.Substring(1);
            ////}
            ////newProj.FolderID = Convert.ToInt32(f);  // GetFolderID(parent);
            ///
            newProj.FolderID = GetFolderID(parent);

                newProj.DetailedDesc = "Details. . .";
            newProj.Priority = " ";  // "A";
                newProj.Status = "O";
                newProj.StartDate = DateTime.Today;
                newProj.DueDate = DateTime.Today.AddDays(5);
                newProj.RevDueDate = DateTime.Today.AddDays(5);
                newProj.DoneDate = null;  // DateTime.MinValue;
                newProj.RespPerson = "Bill";
                newProj.Hide = false;
                newProj.DispLevel = "1";
                newProj.Done = false;
            //}

            //  Get PPartNum for the new project and add to database.
            GetPPartNum(parent, newProj);

            return newProj;
        }

        private void GetPPartNum(TreeViewItemViewModel parent, Project newProj)
        {
            string parentType = parent.GetType().ToString();

            if (parentType.Contains("Folder"))
            {
                newProj.PSortOrder = newProj.PPartNum = this.newPPartNumForProj(newProj.FolderID ?? 0);
            }
            else if (parentType.Contains("Project"))
            {
                newProj.PSortOrder = newProj.PPartNum = newPPartNumForTask(newProj.FolderID ?? 0, ((dbProjectViewModel)parent)._project.PPartNum.Substring(0, 3));
            }
            else if (parentType.Contains("SubTask"))
            {
                //  If parent is SubTask the new item will also be treated as SubTask
                dbTaskViewModel theParentTask = ((dbTaskViewModel)((dbSubTaskViewModel)parent).Parent);
                newProj.PSortOrder = newProj.PPartNum = newPPartNumForSubTask(newProj.FolderID ?? 0, ((dbSubTaskViewModel)parent)._subTask.PPartNum.Substring(0, 6));
            }
            else if (parentType.Contains("Task"))
            {
                newProj.PSortOrder = newProj.PPartNum = newPPartNumForSubTask(newProj.FolderID ?? 0, ((dbTaskViewModel)parent)._task.PPartNum.Substring(0, 6));
            }

            // Save project to database and get good ProjectID
            db.Projects.Add(newProj);
          //  db.Projects.AddOrUpdate()
          ////  int numChanges = CheckChanges(db, "GetPPartNum");
          //////  numChanges = 2;
          ////  if (numChanges > 1)
          ////  {
          ////      ShowQuitingDialog(numChanges.ToString());
          ////      //App.Current.Shutdown(numChanges);
          ////      mLogger.AddLogMessage("PROBLEM: Exiting to avoid data loss!!");
          ////      mLogger.AddLogMessage("======   numChanges excessive -- " + numChanges);
          ////      Environment.Exit(numChanges);
          ////  }
            //   PlansViewModel.db.Projects.Add(newProj);
            int savedMany = db.SaveChanges();
            //LmLogger.AddLogMessage("GetPPartNum: " + savedMany + " savedChanges " + newProj.Item + " " + newProj.ProjectID + " " + newProj.FolderID + " " + newProj.PPartNum);
            //int savedMany = PlansViewModel.db.SaveChanges();
            newProj.ProjectID = newProj.ProjectID;  // Capture key value added by database
        }

        public string ShowQuitingDialog(string probCount)
        {
            ShutDownDialog dlg = new ShutDownDialog(probCount);
            //    InputDialog dlg = new InputDialog();
            dlg.CategoryName = "defaultValue";
            Window parentWindow = Application.Current.MainWindow; //  Window.GetWindow(this);
            dlg.Owner = parentWindow;  // (Window)(this.Parent);
                                       // dlg.DialogResult = false;
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                if (dlg.CategoryName.Length != 0)
                {
                    // User clicked OK and entered a name
                }
                else
                {
                    // Clicked OK but name is blank, really not needed
                    //    OK button not enabled unless name is NOT blank, see XAML
                    dlg.CategoryName = "ZPQ";
                }
            }
            else
            {
                dlg.CategoryName = "ZPQ";
            }
            return dlg.CategoryName;
        }



        //public Project GetNewProj0(string name, TreeViewItemViewModel parent)
        //{
        //    //  string parentType = parent.GetType().ToString();
        //    Project newProj = null;
        //    if (DetailsVM.IsNew)
        //    {
        //        // Using full dialog
        //        DetailsVM.IsNew = false;
        //        newProj = DetailsVM.TheEntity;
        //        newProj.ProjectID = 0;
        //    }
        //    else
        //    {
        //        newProj = new Project(name);  // Need to add additional detail values
        //                                      // Set FolderID based on the Parent
        //        newProj.FolderID = GetFolderID(parent);
        //        newProj.DetailedDesc = "Details. . .";
        //        newProj.Priority = "A";
        //        newProj.Status = "O";
        //        newProj.StartDate = DateTime.Today;
        //        newProj.DueDate = DateTime.Today.AddDays(5);
        //        newProj.RevDueDate = DateTime.Today.AddDays(5);
        //        newProj.DoneDate = null;  // DateTime.MinValue;
        //        newProj.RespPerson = "Bill";
        //        newProj.Hide = false;
        //        newProj.DispLevel = "1";
        //        newProj.Done = false;
        //    }

        //    //  Get PPartNum for the new project and add to database.
        //    GetPPartNum(parent, newProj);

        //    return newProj;
        //}
        public void AddingNewOne(TreeViewItemViewModel parent, string name)
        {
            // Got name for new work item name from ShowInputDialog
            if (name != "ZPQ")
            {
                // Get Parent type and FolderID
                //   int theFolderID = 0;
                string theType = parent.GetType().ToString();
                //   theFolderID = GetFolderID(parent, theType);

                // Could be F, P, T, S
                // new is   P, T, S but really they all are all stored as P objects in database
                mLogger.AddLogMessage("Adding New One ->'" + name + "' - Parent type-> " + theType);
                // Process each type:

                //    1) Get newProj =====================>
                //       Project newProj = GetNewProj(name, parent);

                Project newProj = GetNewProject(parent, name);

                //  Get PPartNum for the new project and add to database.
                GetPPartNum(parent, newProj);

                // Adjust base version, appropriately based on desired type
                //    2) Get PPartNum (and PSortOrder)
                //    4) Save to database

                AddToChildrenParent(parent, newProj); int n = newProj.ProjectID;
            }
            else
            {
                return;  // User did not agree to enter name
            }
        }

        private void AddToChildrenParent(TreeViewItemViewModel parent, Project newProj)
        {
            string theType = parent.GetType().ToString();
            //    3) Get appropriate TreeViewItemViewModel
            //    5) Add new child to Parent.Children
            if (theType.Contains("Folder"))
            {
                // Get appropriate view model
                dbProjectViewModel newDBItem = new dbProjectViewModel(newProj, (dbFolderViewModel)parent, false, db);
                // Add new child to parent
                parent.Children.Add(newDBItem);
            }
            else if (theType.Contains("Project"))
            {
                // Get appropriate view model
                dbTaskViewModel newDBItem = new dbTaskViewModel(newProj, (dbProjectViewModel)parent, false, db);
                // Add new child to parent
                parent.Children.Add(newDBItem);
            }
            else if (theType.Contains("SubTask"))
            {
                //  If parent is SubTask the new item will also be treated as SubTask
                dbTaskViewModel theParentTask = ((dbTaskViewModel)((dbSubTaskViewModel)parent).Parent);
                //parent.Children.Add(newDBItem);
                dbSubTaskViewModel newDBItem = new dbSubTaskViewModel(newProj, theParentTask, false);
                theParentTask.Children.Add(newDBItem);
            }
            else if (theType.Contains("Task"))
            {
                dbSubTaskViewModel newDBItem = new dbSubTaskViewModel(newProj, (dbTaskViewModel)parent, false);
                parent.Children.Add(newDBItem);
            }
        }

        private Project GetNewProject(TreeViewItemViewModel parent, string name)
        {
            Project newProj = new Project();
            // Set FolderID based on the Parent ==============
            string f = parent.DbID;
            //if (f.Contains("F"))
            //{
            //    f = f.Substring(1);
            //}
            //     newProj.FolderID = Convert.ToInt32(f);  // GetFolderID(parent);
            ///newProj.FolderID = DbIDtoFolderID(f);  // GetFolderID(parent);

            // NewOne should be in same folder as parent
            newProj.FolderID = GetFolderID(parent);
            // Fill out newProj with default valuse
            newProj.DetailedDesc = "Details. . .";
            if (newProj.FolderID == RoutineTasksFolderID)
            {
                newProj.Priority = "R";
            }
            else
            {
                newProj.Priority = "A";
            }
            newProj.Status = " ";  // "O";
            newProj.StartDate = DateTime.Today;
            newProj.DueDate = DateTime.Today.AddDays(5);
            newProj.RevDueDate = DateTime.Today.AddDays(5);
            newProj.DoneDate = null;  // DateTime.MinValue;
            newProj.RespPerson = "Bill";
            newProj.Hide = false;
            newProj.DispLevel = "1";
            newProj.Done = false;
            newProj.Item = name;
            // Check this for refactoring review.
            newProj.DateCreated = DateTime.Now;
            newProj.DateModified = newProj.DateCreated;
            //}
            return newProj;
        }

        int DbIDtoFolderID(string f)
        {
            if (f.Contains("F"))
            {
                f = f.Substring(1);
            }
            return Convert.ToInt32(f);
        }

        public object FolderFix(string dbID)
        {
            mLogger.AddLogMessage("Incoming dbID: " + dbID);
            if (dbID.Contains("F"))
            {
                //  Dealing with Folder
                dbID = dbID.Substring(1);
            }
            mLogger.AddLogMessage("Outgoing dbID: " + dbID);
            return dbID;
        }

        private int GetFolderID(TreeViewItemViewModel parent)
        {
            string theType = parent.GetType().ToString();
            TreeViewItemViewModel test = parent;

            int theFolderID;
            //    1) Either a folder  (F)
            //    2) or must be in the Project database (P T S)
            // Check type of Parent
            if (theType.Contains("Folder"))
            {
                theFolderID = Convert.ToInt32(FolderFix(parent.DbID));  // parentProj.FolderID;
            }
            else
            {
                ////  Must be working with a "project" database item
                //Project parentProj = db.Projects.Find(Convert.ToInt32(parent.DbID));
                //theFolderID = parentProj.FolderID ?? 0;

                while (!(test.DbID.Contains("F")) && test != null)
                {
                    test = test.Parent;
                }

                if (test != null)
                {
                    // must have an "F"
                    string f = test.DbID.Substring(1);
                    theFolderID = Convert.ToInt32(f);
                }
                else
                {
                    theFolderID = 0;
                }
             
            }
            //LmLogger.AddLogMessage("GetFolderID - " + theFolderID + " - " + test.GetType().ToString());
            return theFolderID;
        }

        public string newPPartNumForProj(int theFolderID)
        {
            /*
            var item = db.Items.OrderByDescending(i => i.Value).FirstOrDefault();
            IQueryable<Project> theProjs = (from c in db.Projects
                                            where c.FolderID == folder.FolderID &&
                                                  c.PPartNum.Substring(3, 6) == "000000"
                                            orderby c.PPartNum
                                            select c);
            Project[] projects = theProjs.ToArray();
            return projects;
            */
           // string theFileName = (string)App.Current.Properties["destFilePath"];
            ///        TDTDbContext theDB = new TDTDbContext(theFileName);
            //TDTDbContext theDB = new TDTDbContext();
            //theDB = db
            //  TDTDbContext theDB = new TDTDbContext("BillWork.db");
            string PartNum = "";
            TDTDbContext theDB = db;   // see Util.newPPartNum...
            //using (TDTDbContext theDB = new TDTDbContext())
            //{
                string theProjs = (from c in theDB.Projects
                                   where c.FolderID == theFolderID &&
                                         c.PPartNum.Substring(3, 6) == "000000"
                                   orderby c.PPartNum descending
                                   select c.PPartNum.Substring(0, 3)).FirstOrDefault();
                //Project[] projects = theProjs.ToArray();

                PartNum = newNum(theProjs) + "000000";
                mLogger.AddLogMessage("NewProject: " + PartNum + " - " + theFolderID);
            //}
            return PartNum;
        }

        public string newNum(string lastNum)
        {
            // Increment the old maximum and format with leading zeros
            int nNew = Convert.ToInt32(lastNum);
            nNew++;
            nNew = nNew + 1000;
            string newNumStr = nNew.ToString().Substring(1, 3);
            return newNumStr;
        }

        public string newPPartNumForTask(int theFolderID, string ProjNum)
        {
            string PartNum = "";
            TDTDbContext theDB = db;   // see Util.newPPartNum...
            //using (TDTDbContext theDB = new TDTDbContext())
            //{
                //TDTDbContext theDB = new TDTDbContext();
                string theProjs = (from c in theDB.Projects
                                   where c.FolderID == theFolderID &&
                                         c.PPartNum.Substring(0, 3) == ProjNum
                                   orderby c.PPartNum descending
                                   select c.PPartNum.Substring(3, 3)).FirstOrDefault();

                //string PartNum = newNum(theProjs) + "000";
                PartNum = ProjNum + newNum(theProjs) + "000";
                mLogger.AddLogMessage("New Task: " + PartNum + "  Input: " + ProjNum + " - " + theFolderID);
            //}

            //string theProjs = (from c in db.Projects
            //                   where c.FolderID == theFolderID &&
            //                         c.PPartNum.Substring(0, 3) == ProjNum
            //                   orderby c.PPartNum descending
            //                   select c.PPartNum.Substring(3, 3)).FirstOrDefault();

            ////string PartNum = newNum(theProjs) + "000";
            //PartNum = ProjNum + newNum(theProjs) + "000";
            //mLogger.AddLogMessage("New Task: " + PartNum + "  Input: " + ProjNum);

            return PartNum;
        }
        public string newPPartNumForSubTask(int theFolderID, string ProjNum)
        {
            // string theFileName = (string)App.Current.Properties["destFilePath"];
            ///         TDTDbContext theDB = new TDTDbContext(theFileName);
            // TDTDbContext theDB = new TDTDbContext();
            // TDTDbContext theDB = new TDTDbContext("BillWork.db");

            string PartNum = "";
            TDTDbContext theDB = db;   // see Util.newPPartNum...
            //using (TDTDbContext theDB = new TDTDbContext())
            //{
                string theProjs = (from c in theDB.Projects
                                   where c.FolderID == theFolderID &&
                                         c.PPartNum.Substring(0, 6) == ProjNum
                                   orderby c.PPartNum descending
                                   select c.PPartNum.Substring(6, 3)).FirstOrDefault();

                //string PartNum = newNum(theProjs) + "000";
                PartNum = ProjNum + newNum(theProjs);
                mLogger.AddLogMessage("New SubTask: " + PartNum + "  Input: " + ProjNum + " - " + theFolderID);
            //}

            // string PartNum = ProjNum + "001";
            return PartNum;
        }

        // transfers from PlansVMpartial.cs======================================

        public bool isCurrentView = true;  // false;

        public CommandVM SaveCmd { get; set; }
        //public CommandVM CommitStartTimingCmd { get; set; }
        //public CommandVM StopWorkCmd { get; set; }
        public CommandVM QuitCmd { get; set; }

        private void CurrentUserControl(CurrentViewMessage nm)
        {
            string curObj = this.GetType().Name;
            if (this.GetType() == nm.ViewModelType)
            {
                mLogger.AddLogMessage("PlansVMpartial CurrentUserControl - isCurentView TRUE  " + curObj);
                isCurrentView = true;
            }
            else
            {
                mLogger.AddLogMessage("PlansVMpartial CurrentUserControl - isCurrentView FALSE  " + curObj);
                isCurrentView = false;
                //if (db.ChangeTracker.HasChanges())
                //{
                //    int numChanges = db.SaveChanges();
                //    mLogger.AddLogMessage("Leaving Tracks.  Saved " + numChanges);
                //}

            }
        }

        protected void HandlePlansCommand(CommandMessage action)
        {
            if (isCurrentView)
            {
                mLogger.AddLogMessage("### Command MSG received by Plans: " + action.Command);
                switch (action.Command)
                {
                    case CommandType.Insert:
                        //InsertNew();
                        if (GotSomethingSelected())
                        {
                            InsertNewFull();
                        }
                        break;
                    case CommandType.Edit:
                        if (GotSomethingSelected())
                        {
                            NowEditing = true;
                            EditCurrentPlan();
                        }
                        break;
                    //case CommandType.StartTiming:
                    //    if (GotSomethingSelected())
                    //    {
                    //        StartTiming();
                    //    }
                    //    break;
                    //case CommandType.CommitStartTiming:
                    //    if (GotSomethingSelected())
                    //    {
                    //        CommitStartTiming();
                    //    }
                    //    break;
                    //case CommandType.QuitStartTiming:
                    //    QuitStartTiming();
                    //    break;
                    //case CommandType.StopWork:
                    //    if (GotSomethingSelected())
                    //    {
                    //        StopWork();
                    //    }
                    //    break;
                    //case CommandType.CommitStopWork:
                    //    if (GotSomethingSelected())
                    //    {
                    //        CommitStopWork();
                    //    }
                    //    break;
                    //case CommandType.QuitStopWork:
                    //    QuitStopWork();
                    //    break;
                    //case CommandType.Delete:
                    //    if (GotSomethingSelected())
                    //    {
                    //        DeleteCurrent();
                    //    }
                    //    break;

                    case CommandType.QuitEdit:
                        QuitPlanEdit();
                        break;
                    case CommandType.CommitEdit:  //.SaveEdit:
                        CommitUpdates();
                        break;

                    case CommandType.Commit:
                        mLogger.AddLogMessage("Handle Command Commit in PlansViewModel line 1104");
                        CommitUpdates();
                        break;
                    case CommandType.Refresh:
                        mLogger.AddLogMessage("Handle Command Refresh in PlansViewModel");
                        RefreshData(theTVItem);
                        //   editEntity = null;
                        //   selectedEntity = null;
                        break;
                    //case CommandType.Quit:
                        //QuitPlanEdit();
                        //break;
                    //case CommandType.CleanUp:
                    //    CleanUp();
                    //    break;
                    case CommandType.None:
                        break;
                    default:
                        break;
                }
            }
        }

        private void InsertNewFull()
        {
            // From AddNewOne
            TreeViewItemViewModel parent = selectedTVItem; // SelectedProject GetCommandItem();

            // Set up DetailsVM to populate EditPopUp
            EditPlan = new ProjectVM();
            EditPlan.IsNew = true;
            //DetailsVM.Selection = "Adding New Work Item";
            //DetailsVM.TheEntity = SelectedProject;
            ////  DetailsVM.TheEntity = SelectedProject;
            //DetailsVM.TheEntity.Item = "New Work Item0";
            //DetailsVM.TheEntity.DetailedDesc = "New Work Item Details0";
            //DetailsVM.TheEntity.Status = " ";
            //RaisePropertyChanged("DetailsVM");

            // Set up EditPlan to populate popup
            EditPlan.TheEntity = newDefaultProject();   // DetailsVM.TheEntity;

            //((PlansViewModel)DataContext).DetailsVM.IsNew = true;
            //      AddingNewOne(parent, "New Work Item");
            //e.Handled = true;

            //TreeViewItemViewModel parent = (TreeViewItemViewModel)tv.SelectedItem;  // GetCommandItem();

            //DetailsVM = new ProjectVM();
            //DetailsVM.IsNew = true;
            //DetailsVM.TheEntity = SelectedProject;
            //DetailsVM.TheEntity.Item = "New Work Item";
            ////   DetailsVM.FolderID = SelectedProject.FolderID; // newAdHocProj.ProjectID;
        //    mLogger.AddLogMessage("Reached InsertNewFull in PlansViewModel with '" + DetailsVM.TheEntity.Item.Trim() + "' - " + DetailsVM.TheEntity.Status + "--->");
            mLogger.AddLogMessage("Reached InsertNewFull in PlansViewModel" + "--->");
            TitlePopUp = "Adding new Planning Work Item (full edit)";
            NowEditing = false;
            IsInEditMode = true;  // Opens EditPopUp
        }

        private Project newDefaultProject()
        {
            Project proj = new Project();
            //throw new NotImplementedException();

            // NewOne should be in same folder as parent
            proj.FolderID = GetFolderID(selectedTVItem);

            // Fill out proj with default valuse
            proj.DetailedDesc = "Details. . .";
            if (proj.FolderID == RoutineTasksFolderID)
            {
                proj.Priority = "R";
            }
            else
            {
                proj.Priority = "A";
            }
            proj.Priority = "C";
            proj.Status = " ";  // "O";
            proj.StartDate = DateTime.Today;
            proj.DueDate = DateTime.Today.AddDays(5);
            proj.RevDueDate = DateTime.Today.AddDays(5);
            proj.DoneDate = null;  // DateTime.MinValue;
            proj.RespPerson = "Bill";
            proj.Hide = false;
            proj.DispLevel = "1";
            proj.Done = false;
            proj.Item = "name";
            // Check this for refactoring review.
            proj.DateCreated = DateTime.Now;
            proj.DateModified = proj.DateCreated;

            return proj;

        }

        public virtual void RefreshData(TreeViewItem theFocusItem = null)
        {
            Messenger.Default.Register<CommandMessage>(this, (action) => HandlePlansCommand(action));

            mLogger.AddLogMessage("Reached RefreshData in PlansVM which will cause Data to be Refreshed!");
            GetData();
            Messenger.Default.Send<UserMessage>(new UserMessage { Message = "Data Refreshed" });
            if (theFocusItem != null)
            {
                // Set focus based on theFocusItem
                //(TreeViewItem)theFocusItem
            }
        }

        protected void EditCurrentPlan()
        {
            DetailsVM = new ProjectVM();
            DetailsVM.TheEntity = SelectedProject;
            DetailsVM.IsNew = false;

            // Retreive current project to populate PopUp
            EditPlan = DetailsVM; // SelectedProject;

            mLogger.AddLogMessage("Reached PlansView EditCurrent with '" + SelectedProject.Item.Trim() + "' - " + DetailsVM.TheEntity.Status + "----->");
            TitlePopUp = "Editing '" + SelectedProject.Item.Trim() + "' ?";
            DetailsVM.Selection = TitlePopUp;
            // Set flag for editing
            NowEditing = true;
            IsInEditMode = true;  // Opens EditPopUp
        }

        private bool GotSomethingSelected()
        {
            bool OK = true;
            if (SelectedProject == null)
            {
                OK = false;
                ShowUserMessage("You must select a work item");
            }
            return OK;
        }

        protected void QuitPlanEdit()
        {
            //if (EditVM == null || EditVM.TheEntity == null)
            //{
            mLogger.AddLogMessage("Reached  Quit Plans Edit -------------->");
            ////if (!EditVM.IsNew)
            ////{
            ////EditVM.TheEntity.ClearErrors();
            ////tryF
            ////{
            ////    //await db.Entry(EditVM.TheEntity).ReloadAsync();
            ////    db.Entry(EditVM.TheEntity).Reload();
            ////}
            ////catch (Exception ex)
            ////{
            ////    mLogger.AddLogMessage("DB Problem l 966 : " + ex.Message);
            ////}
            //////   await db.Entry(EditVM.TheEntity).ReloadAsync();
            //////db.Entry(EditVM.TheEntity).Reload();
            ////// Force the datagrid to realise the record has changed
            ////EditVM.TheEntity = EditVM.TheEntity;
            //////          EditVM = currentToDo;
            ////RaisePropertyChanged("EditVM");
            ////}
            //}
            //      mLogger.AddLogMessage("EditVM->" + EditVM.TheEntity.Item);
            //     SelectedToDo = EditVM;
            //     RefreshData();
            //RaisePropertyChanged("SelectedToDo");
            ////inWorkToDo = null;
            //////EditVM = null;
            //      NowEditing = false;
            ////IsInStartTimingMode = false;
            ////IsInStopWorkMode = false;
            if (NowEditing)
            {
                // Working with Edit
                // Need to reload from database (User may have changes that are being abandoned)
                // EditPlan.ClearErrors();  // not avvailable
                // db.Entry(EditPlan.TheEntity).Reload();
                //     db.Entry(DetailsVM.TheEntity).Reload();
                var test = db.Projects.Find(EditPlan.TheEntity.ProjectID);
                EditPlan.TheEntity = EditPlan.TheEntity;
                //DetailsVM.TheEntity = DetailsVM.TheEntity;
                RaisePropertyChanged("EditPlan");
                CheckForChanges();
                NowEditing = false;
            }

            //Close the popup =================>
            IsInEditMode = false; 

            //  RefreshData(theTVItem);
            theTVItem.Focus();
        }
        void CheckForChanges()
        {
            db.ChangeTracker.DetectChanges();

            // +++++++++++++++++++++++++++++++++++++++++->
            //   int numChanges = CheckChanges2(db, "GetPPartNum");
            string CallerMsg = "testing";
            int historyCount = 0;
            // ============================
            //TDTDbContext db = _db;   // ((App)Application.Current).db;
            ///TDTDbContext db = new TDTDbContext();
            string curType = "??";

            db.ChangeTracker.DetectChanges();
            bool PendingChanges = db.ChangeTracker.HasChanges();
            //db.ChangeTracker.
            if (PendingChanges)
            {
                int n = db.ChangeTracker.Entries().Count();
                mLogger.AddLogMessage("=== Running CheckChanges == " + CallerMsg + "== NumEntries-" + n);
                string changeType = "";
                foreach (
                    var history in db.ChangeTracker.Entries()
                        .Where(e => e.Entity is IModificationHistory && (e.State == System.Data.Entity.EntityState.Added ||
                                                                         e.State == System.Data.Entity.EntityState.Modified))
                        .Select(e => e.Entity as IModificationHistory)
                )
                {
                    historyCount++;
                    //history.DateModified = DateTime.Now;
                    //if (history.DateCreated == DateTime.MinValue)
                    //{
                    //    history.DateCreated = DateTime.Now;
                    //}
                    changeType = history.GetType().ToString();
                    if (changeType.Contains("Project"))
                    {
                        mLogger.AddLogMessage("CheckChanges -" + historyCount + "  Project Changed- '" +
                                              ((Project)history).Item + "' -" + changeType);
                        mLogger.AddLogMessage("CheckChanges -" + historyCount + "  Project.FolderID was- '" +
                                              ((Project)history).FolderID + "' - " + ((Project)history).ProjectID);
                    }
                    else if (changeType.Contains("ToDo"))
                    {
                        mLogger.AddLogMessage("CheckChanges -" + historyCount + "  ToDo Changed- '" + ((ToDo)history).Item +
                                              "' -" + changeType);
                        mLogger.AddLogMessage("CheckChanges -" + historyCount + "  ToDo.ToDoID was- '" +
                                              ((ToDo)history).ToDoID + "' - " + ((ToDo)history).ProjectID);
                    }
                    else
                    {
                        mLogger.AddLogMessage("CheckChanges -" + historyCount + "  Track Changed- '" + ((Track)history).Item +
                                              "' -" + changeType);
                        mLogger.AddLogMessage("CheckChanges -" + historyCount + "  Track.TrackID was- '" +
                                              ((Track)history).TrackID + "' - " + ((Track)history).ProjectID);
                    }
                }

                //db.su.
                int nSaved = db.SaveChanges();
                mLogger.AddLogMessage("!!!! Saved Pending Changes.  Count was " + nSaved);
                mLogger.AddLogMessage(" ============================");
            }
            else
            {
                mLogger.AddLogMessage("NO Pending Changes! --" + curType + "--");
            }

            // +++++++++++++++++++++++++++++++++++++++++->
        }

        private void CommitUpdates()
        {
            // Triggered from EditPopUp - Editing or Adding a new one

            //if (EditPlan == null || EditPlan.TheEntity == null)
            //{
            //    mLogger.AddLogMessage("CommitUpdates  EditPlan is null!!");
            //    if (db.ChangeTracker.HasChanges())
            //    {
            //        UpdateDB();
            //    }
            //    return;
            //}

            mLogger.AddLogMessage($"CommitUpdates  Plans Edit    EditPlan->{EditPlan.TheEntity.Item}");

            var x = EditPlan.TheEntity.Item;
            //  ShowUserMessage("WORKING! in CommitUpdates");

            //if (EditPlan.TheEntity.IsValid())
            //{
                if (EditPlan.IsNew)
                {
                    //  Adding a new project here. ====================

                    //EditPlan.IsDirty();
                    //     EditPlan.IsNew = false;
                    //// Add to collection
                    //ToDos.Add(EditPlan);
                    //// Added to collection, now to add to the database.

                    //  Get PPartNum for the new project and add to database.
                    GetPPartNum(selectedTVItem, EditPlan.TheEntity);

                    //// Save project to database and get good ProjectID
                    //db.Projects.Add(EditPlan);
                    //EditPlan.ProjectID = EditPlan.ProjectID;

                    // Update the UI by adding new child to Parent's Children
                    AddToChildrenParent(selectedTVItem, EditPlan.TheEntity);
                    //db.ToDos.Add(EditPlan.TheEntity);
                    //mLogger.AddLogMessage("Added new ToDo, calling UpdateDB. " + EditPlan.TheEntity.Item);
                    //UpdateDB();

                    //      Project theProj = GetNewProj(EditPlan.TheEntity.Item, selectedTVItem);
                    //     Project theProj = db.Projects.Find(EditPlan.TheEntity.ProjectID);

                    //theProj.Item = EditPlan.TheEntity.Item;
                    //theProj.DetailedDesc = EditPlan.TheEntity.DetailedDesc;
                    //theProj.Status = "A";

                    EditPlan.IsNew = false;
                    //////Project projEntry = GetProjectEntry(EditPlan.TheEntity);
                    //////db.Projects.Add(projEntry);
                    //mLogger.AddLogMessage("Edited project, calling UpdateDB.");
                    //UpdateDB();

                    //SelectedToDo = EditPlan;
                    //Quit();
                }
                //else if (db.ChangeTracker.HasChanges())
                else //if (NowEditing)
                {
                    // Handle the editing case
                    //NowEditing = false;
                    if (db.ChangeTracker.HasChanges())
                    {
                        // Update the database, save changed Project

                        // Update the UI elements, edit Parent's Children with adjusted values

                        ////////Project theProj = db.Projects.Find(EditPlan.TheEntity.ProjectID);
                        ////////theProj.Item = EditPlan.TheEntity.Item;
                        ////////theProj.DetailedDesc = EditPlan.TheEntity.DetailedDesc;
                        ////////theProj.Status = "A";
                        ////Project projEntry = GetProjectEntry(EditPlan.TheEntity);
                        ////db.Projects.Add(projEntry);

                        mLogger.AddLogMessage("Edited project, calling UpdateDB.");
                        UpdateDB();
                    }
                    else
                    {
                        ShowUserMessage("No changes to save");
                    }
                    // SelectedProject = theProj;
                   // QuitPlanEdit();


                    //UpdateDB();
                    ////  SelectedToDo = EditPlan;
                    //Quit();  // IsInEditMode = false;


                }


                //if (db.ChangeTracker.HasChanges())
                //{
                //    UpdateDB();
                //    //  SelectedToDo = EditPlan;
                //    QuitPlanEdit();  // IsInEditMode = false;
                //}

                IsInEditMode = false;
            //}
            //else
            //{
            //    ShowUserMessage("There are validation problems with the data entered");
            //    mLogger.AddLogMessage("There are validation problems with the data entered");
            //}

            QuitPlanEdit();

        }

        public void ShowUserMessage(string message)
        {
            mLogger.AddLogMessage("--UserMessage: '" + message + "'");
            UserMessage msg = new UserMessage { Message = message };
            Messenger.Default.Send<UserMessage>(msg);
        }

        // Used to control showing a pop up to edit an entity or StartTiming
        private bool isInEditMode = false;
        public bool IsInEditMode
        {
            get
            {
                return isInEditMode;
            }
            set
            {
                isInEditMode = value;
                InEdit inEdit = new InEdit { Mode = value };
              //  Messenger.Default.Send<InEdit>(inEdit);
                RaisePropertyChanged();
            }
        }

        // Used to indicate Adding or Editing a project
        private bool nowEditing = false;
        public bool NowEditing
        {
            get
            {
                return nowEditing;
            }
            set
            {
                nowEditing = value;
                //InEdit inEdit = new InEdit { Mode = value };
                //Messenger.Default.Send<InEdit>(inEdit);
                RaisePropertyChanged();
            }
        }

        private string titlePopUp;
        public string TitlePopUp
        {
            get
            {
                return titlePopUp;
            }
            set
            {
                titlePopUp = value;
                RaisePropertyChanged();
            }
        }

        public void UpdateDB()   // Update the Database
        {
            mLogger.AddLogMessage("==== UpdateDB ==PlansViewModel l 1352 ===============>");

            Util.ChkContext(db, "== PlansViewModel.UpdateDB == l 1370");

            ////db.db.ChangeTracker.Entries();
            ////  Database db = new Database();
            //foreach (var history in db.ChangeTracker.Entries()
            //              .Where(e => e.Entity is IModificationHistory && (e.State == System.Data.Entity.EntityState.Added ||
            //                      e.State == System.Data.Entity.EntityState.Modified))
            //               .Select(e => e.Entity as IModificationHistory)
            //              )
            //{
            //    //                           .Select(e => e.Entity as IModificationHistory)
            //    history.DateModified = DateTime.Now;
            //    if (history.DateCreated == DateTime.MinValue)
            //    {
            //        history.DateCreated = history.DateModified;  //DateTime.Now;
            //    }
            //    Project p = history as Project;
            //    if (p != null)
            //    {
            //        mLogger.AddLogMessage("PlansVM-ChangeTracker: " + p.ProjectID + "-" + p.FolderID + "-" + history.ToString());
            //    }
            //}
            try
            {
                int nChanges = db.SaveChanges();
                ///  ShowUserMessage("Database Updated with " + nChanges.ToString() + " changes.");
                mLogger.AddLogMessage("UpdateDB successfully completed. with " + nChanges.ToString() + " changes.");
            }
            catch (DbEntityValidationException dbEx)
            {
                // mLogger.AddLogMessage("DbEntityValidationException: =====");
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //        mLogger.AddLogMessage("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException deDbEx)
            {
                //foreach (var vErrors in deDbEx.InnerException)
                //{
                var theProblem = deDbEx.InnerException.Message;
                //}
                //  mLogger.AddLogMessage("DbUpdateException: " + deDbEx.InnerException.Message);
            }
            catch (Exception e)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    string ErrorMessage = e.InnerException.GetBaseException().ToString();
                }
                //   mLogger.AddLogMessage("Exception: " + e.InnerException.GetBaseException().ToString());
                ///  ShowUserMessage("There was a problem updating the database");
            }
            finally
            {
                mLogger.AddLogMessage("  ========= finally End UpdateDB in PlansViewModel.cs l 1413 =============");
            }
        }

        public void CloseOut()
        {
            mLogger.AddLogMessage("*** PlansViewModel CloseOut! ***");

            if (db.ChangeTracker.HasChanges())
            {
                mLogger.AddLogMessage("*** dbBase has Changes in PlansView.CloseOut! ***");
                UpdateDB();
            }
            //   dbBase.Dispose();

            Messenger.Default.Unregister<CommandMessage>(this);
            Messenger.Default.Unregister<NavigateMessage>(this);
            Messenger.Default.Unregister<CurrentViewMessage>(this);

        }

        public void StartUp()
        {
            Messenger.Default.Register<CommandMessage>(this, (action) => HandlePlansCommand(action));
            RefreshData();
        }

        #region Drag and Drop Handlers

        void IDragSource.StartDrag(IDragInfo dragInfo)
        {
            // What is being dragged?
            Type dragType = dragInfo.SourceItem.GetType();
            mLogger.AddLogMessage("Dragging type " + dragType.Name);
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDragHandler.StartDrag(dragInfo);
            ////ToDoVM todoVM = (ToDoVM)dragInfo.SourceItem;
            ////ToDo todo = todoVM.TheEntity;
            ////if (todo.TDTSortOrder != "000000")
            ////{
            ////    // Can't drag first item.
            ////    dragInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
            ////    dragInfo.Data = todoVM;
            ////    mLogger.AddLogMessage("Start Dragging '" + todo.Item + "' -  '" + todo.TDTSortOrder + "'");
            ////}
        }
        bool IDragSource.CanStartDrag(IDragInfo dragInfo)
        {
            bool CanDo;
            if (dragInfo.SourceItem is dbFolderViewModel)
            {
                // Don't allow Folder dragging
                CanDo = false;
            }
            else
            {
                CanDo = GongSolutions.Wpf.DragDrop.DragDrop.DefaultDragHandler.CanStartDrag(dragInfo);
                //bool CanDo = GongSolutions.Wpf.DragDrop.DragDrop.DefaultDragHandler.CanStartDrag(dragInfo);
                //return CanDo;

            }

            //bool CanDo = GongSolutions.Wpf.DragDrop.DragDrop.DefaultDragHandler.CanStartDrag(dragInfo);
            if (CanDo)
            {
                mLogger.AddLogMessage("CanStartDrag is TRUE!");
            }
            return CanDo;
        }
        void IDragSource.Dropped(IDropInfo dropInfo)
        {
            Type dragType = dropInfo.TargetItem.GetType();
            mLogger.AddLogMessage("Dropping target type " + dragType.Name);

            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDragHandler.Dropped(dropInfo);
           // DragDrop.DefaultDragHandler.Dropped(dropInfo);
            var d = dropInfo.Data;
            var dType = dropInfo.Data.GetType();
            var x1 = dropInfo.InsertIndex;
            var x2 = dropInfo.DestinationText;
            var x3 = dropInfo.InsertPosition;
            var x4 = dropInfo.UnfilteredInsertIndex;
            var x5 = dropInfo.TargetItem;
            //var x6 = dropInfo.;
            //DragDrop.g
            var aa = (TreeViewItemViewModel)d;
            var x = dropInfo.DropPosition;
            PlansViewModel.NewHandleDropping((TreeViewItemViewModel)(dropInfo.Data), (TreeViewItemViewModel)(dropInfo.TargetItem), dropInfo.InsertPosition.ToString());

            RefreshData();
        }
        void IDragSource.DragCancelled()
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDragHandler.DragCancelled();
        }
        bool IDragSource.TryCatchOccurredException(Exception exception)
        {
            bool CanDo = GongSolutions.Wpf.DragDrop.DragDrop.DefaultDragHandler.TryCatchOccurredException(exception);
            return CanDo;
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);
        }
        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
            mLogger.AddLogMessage("Drop Reached ==========");
            System.Collections.IEnumerable c = dropInfo.TargetCollection;

            //foreach (ToDoVM item in c)
            //{
            //    ToDo aToDo = ((ToDoVM)item).TheEntity;
            //    mLogger.AddLogMessage(aToDo.Item + " - " + aToDo.TDTSortOrder);
            //}
            //bool needSave = db.ChangeTracker.HasChanges();
            //VisualHelper.ListToDosGrid(mLogger, ToDos);
            //mLogger.AddLogMessage("ToDos ==========");
            //foreach (ToDoVM item in ToDos)
            //{
            //    ToDo aToDo = ((ToDoVM)item).TheEntity;
            //    mLogger.AddLogMessage(aToDo.Item + " - " + aToDo.TDTSortOrder);
            //}
            //ToDos
        }

        //void IDragSource.DragCancelled()
        //{
        //    GongSolutions.Wpf.DragDrop.DragDrop.DefaultDragHandler.DragCancelled();
        //}

        #endregion

    }
}