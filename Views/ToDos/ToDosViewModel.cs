//using TDT_EF;  // 
using BuildSqliteCF.Entity;
using GongSolutions.Wpf.DragDrop;
//using BuildSqliteCF.DbContexts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
//using LoggerLib;
using nuT3;
using System.Windows.Controls;

namespace Planner
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
   // public partial class ToDosViewModel : CrudVMBaseTDT, IDragSource, IDropTarget // ViewModelBase
    public partial class ToDosViewModel : ViewModelBase, IDragSource, IDropTarget // 
    {
        #region Properties
        public DataGrid TheDGToDos;
        public ObservableCollection<CommandVM> Commands { get; set; }
        public ObservableCollection<ViewVM> Views { get; set; }
        public ObservableCollection<ReportVM> Reports { get; set; }  // not really needed

        private ToDoVM editVM;
        public ToDoVM EditVM
        {
            get
            {
                return editVM;
            }
            set
            {
                editVM = value;
                if (editVM != null)
                {
                    editEntity = editVM.TheEntity;
                }
                RaisePropertyChanged();
            }
        }

        private ToDoVM selectedToDo;
        public ToDoVM SelectedToDo
        {
            get
            {
                return selectedToDo;
            }
            set
            {
                selectedToDo = value;
                selectedEntity = value;
                if (value != null)
                {
                    mLogger.AddLogMessage("set in ToDosViewModel SelectedToDo->" + SelectedToDo.TheEntity.Item);
                }
               // RaisePropertyChanged();
            }
        }

        private ToDoVM selectedToDos;
        public ToDoVM SelectedToDos
        {
            get
            {
                return selectedToDos;
            }
            set
            {
                selectedToDos = value;
                selectedEntity = value;
                if (value != null)
                {
                    //mLogger.AddLogMessage("set in ToDosViewModel SelectedToDo->" + SelectedToDo.TheEntity.Item);
                }
                // RaisePropertyChanged();
            }
        }

        private ToDoVM inWorkToDo;
        public ToDoVM InWorkToDo
        {
            get
            {
                return inWorkToDo;
            }
            set
            {
                inWorkToDo = value;
                //selectedEntity = value;
                if (value != null)
                {
                    mLogger.AddLogMessage("set in ToDosViewModel InWorkToDo->" + ((ToDoVM)value).TheEntity.Item);
                }
                RaisePropertyChanged();
            }
        }

        private ToDoVM nextToDo;
        public ToDoVM NextToDo
        {
            get
            {
                return nextToDo;
            }
            set
            {
                nextToDo = value;
                //selectedEntity = value;
                if (value != null)
                {
                    mLogger.AddLogMessage("set in ToDosViewModel NextToDo->" + ((ToDoVM)value).TheEntity.Item);
                }
                RaisePropertyChanged();
            }
        }
        private int selectedToDoIndex;
        public int SelectedToDoIndex
        {
            get
            {
                return selectedToDoIndex;
            }
            set
            {
                selectedToDoIndex = value;
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

        private string titlePopUp1;
        public string TitlePopUp1
        {
            get
            {
                return titlePopUp1;
            }
            set
            {
                titlePopUp1 = value;
                RaisePropertyChanged();
            }
        }

        private TrackVM trackVM;
        public TrackVM TrackVM
        {
            get
            {
                return trackVM;
            }
            set
            {
                trackVM = value;
                //  trackEntity = trackVM.TheEntity;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<ToDoVM> ToDos { get; set; }
        public ObservableCollection<TrackVM> Tracks { get; set; }

        private StatusOptions statusOpt = StatusOptions.F;
        public StatusOptions StatusOpt
        {
            get { return statusOpt; }
            set { statusOpt = value; RaisePropertyChanged("StatusOpt"); }
        }
        public Window rView { get; set; }
    //    public RelayCommand RunReport1 { get; set; }
     //   ToDo inWorkToDo;
        DateTime SwitchDate;
        string curTDTSortOrder;
        public int? AdHocFolderID;
        public DateTime StopStartTime;
        #endregion
        //public ObservableCollection<IMenuItem> MainMenu { get; set; }

        /// <summary>
        /// Initializes a new instance of the ToDosViewModel class.
        /// </summary>
        public ToDosViewModel() : base()
        {
            mLogger = Logger.Instance;
            string theFileName = (string)App.Current.Properties["destFilePath"];
            ///         db = new TDTDbContext(theFileName);
            dbBase = ((App)Application.Current).db;
            var dbBase2 = ((App)Application.Current).db;
            ///      dbBase = new TDTDbContext();
            mLogger.AddLogMessage("Constructor ToDosViewModel =========");
            ShowUserMessage("User messages shown here!");

            GetData();
            RebuildTDTSortOrder();
            Messenger.Default.Register<CommandMessage>(this, (action) => HandleCommand(action));
            // Listen for Navigation messages to update isCurrentView.
            //Messenger.Default.Register<NavigateMessage>(this, (action) => CurrentUserControl(action));
            //   Messenger.Default.Register<CurrentViewMessage>(this, (action) => CurrentUserControl(action));

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

            CommitStartTimingCmd = new CommandVM
            {
                CommandDisplay = "Commit Start Timing",
                IconGeometry = Application.Current.Resources["SaveIcon"] as Geometry,
                Message = new CommandMessage { Command = CommandType.CommitStartTiming }
            };

            QuitStartTimingCmd = new CommandVM
            {
                CommandDisplay = "Quit Timing Start",
                IconGeometry = Application.Current.Resources["QuitIcon"] as Geometry,
                Message = new CommandMessage { Command = CommandType.QuitStartTiming }
            };

            StopWorkCmd = new CommandVM
            {
                CommandDisplay = "Stop Work",
                IconGeometry = Application.Current.Resources["StopTimingIcon"] as Geometry,
                Message = new CommandMessage { Command = CommandType.StopWork }
            };

            CommitStopWorkCmd = new CommandVM
            {
                CommandDisplay = "Commit Stop Work",
                IconGeometry = Application.Current.Resources["SaveIcon"] as Geometry,
                Message = new CommandMessage { Command = CommandType.CommitStopWork }
            };

            QuitStopWorkCmd = new CommandVM
            {
                CommandDisplay = "Quit Stop Work",
                IconGeometry = Application.Current.Resources["QuitIcon"] as Geometry,
                Message = new CommandMessage { Command = CommandType.QuitStopWork }
            };

            CommitStopStartWorkCmd = new CommandVM
            {
                CommandDisplay = "Commit Stop Start Work",
                IconGeometry = Application.Current.Resources["SaveIcon"] as Geometry,
                Message = new CommandMessage { Command = CommandType.CommitStopStartWork }
            };

            QuitStopStartWorkCmd = new CommandVM
            {
                CommandDisplay = "Quit Stop Start Work",
                IconGeometry = Application.Current.Resources["QuitIcon"] as Geometry,
                Message = new CommandMessage { Command = CommandType.QuitStopStartWork }
            };

            //throw new NotImplementedException();
            #region Views_and_Commands
            ObservableCollection<ViewVM> views = new ObservableCollection<ViewVM>
            {
                // new ViewVM{ ViewDisplay="ToDos", ViewType = typeof(ToDosView), ViewModelType = typeof(ToDosViewModel)},
                new ViewVM{ ViewDisplay="Tracks",   ViewType = typeof(TracksView), ViewModelType = typeof(TracksViewModel)},
                //new ViewVM{ ViewDisplay="Projects", ViewType = typeof(ProjectView), ViewModelType = typeof(CountryViewModel)},
                new ViewVM{ ViewDisplay="Planning", ViewType = typeof(PlansView), ViewModelType = typeof(PlansViewModel)}
          //      new ViewVM{ ViewDisplay="Planning", ViewType = typeof(Plans2View), ViewModelType = typeof(Plans2ViewModel)}
                //new ViewVM{ ViewDisplay="Reports", ViewType = typeof(ReportsView), ViewModelType = typeof(ReportsViewModel)}
            };
            Views = views;
            // Set value for originating view
            ////string curObj = this.GetType().Name;
            ////mLogger.AddLogMessage("NavigateMessage(ToDosViewModel)  CurrentUserControl - isCurentView TRUE  " + curObj);
            //foreach (var item in Views)
            //{
            //    item.origView = parentUserControl;
            //}
            RaisePropertyChanged("Views");
            //Views[0].NavigateExecute();   // Navigate to first ViewVM in Views

            ObservableCollection<CommandVM> commands = new ObservableCollection<CommandVM>
            {
               // new CommandVM{CommandDisplay="Insert", Application.Current.Resources. x:Key="InsertIcon" , Message=new CommandMessage{ Command =CommandType.Insert}},
                new CommandVM{ CommandDisplay="Start Timing", IconGeometry=Application.Current.Resources["StartTimingIcon"]     as Geometry , Message=new CommandMessage{ Command = CommandType.StartTiming} },
                new CommandVM{ CommandDisplay="Stop Timing",  IconGeometry=Application.Current.Resources["StopTimingIcon"]    as Geometry , Message=new CommandMessage{ Command = CommandType.StopWork} },
                new CommandVM{ CommandDisplay="New Work Item", IconGeometry=Application.Current.Resources["InsertIcon"]    as Geometry , Message=new CommandMessage{ Command = CommandType.Insert} },
                new CommandVM{ CommandDisplay="Edit",          IconGeometry=Application.Current.Resources["EditIcon"]             as Geometry , Message=new CommandMessage{ Command = CommandType.Edit} },
                new CommandVM{ CommandDisplay="Delete Work Item", IconGeometry=Application.Current.Resources["DeleteIcon"] as Geometry , Message=new CommandMessage{ Command = CommandType.Delete} },
                new CommandVM{ CommandDisplay="Refresh Data",     IconGeometry=Application.Current.Resources["RefreshIcon"]     as Geometry , Message=new CommandMessage{ Command = CommandType.Refresh} },
                new CommandVM{ CommandDisplay="End of Day",       IconGeometry=Application.Current.Resources["CleanUpIcon"]       as Geometry , Message=new CommandMessage{ Command = CommandType.CleanUp} }
            };
            Commands = commands;
            RaisePropertyChanged("Commands");

            ObservableCollection<ReportVM> _reports = new ObservableCollection<ReportVM>
            {
                new ReportVM { ReportDisplay = "aReport1" },
                new ReportVM { ReportDisplay = "aReport2" }
                // new CommandVM{CommandDisplay="Insert", Application.Current.Resources. x:Key="InsertIcon" , Message=new CommandMessage{ Command =CommandType.Insert}},
                //new CommandVM{ CommandDisplay="New Work Item",      IconGeometry=Application.Current.Resources["InsertIcon"]    as Geometry , Message=new CommandMessage{ Command =CommandType.Insert} },
                //new CommandVM{ CommandDisplay="Edit",        IconGeometry=Application.Current.Resources["EditIcon"]             as Geometry , Message=new CommandMessage{ Command = CommandType.Edit} },
                //new CommandVM{ CommandDisplay="Delete Work Item",      IconGeometry=Application.Current.Resources["DeleteIcon"] as Geometry , Message=new CommandMessage{ Command = CommandType.Delete} },
                //new CommandVM{ CommandDisplay="Refresh Data",     IconGeometry=Application.Current.Resources["RefreshIcon"]     as Geometry , Message=new CommandMessage{ Command = CommandType.Refresh} },
                //new CommandVM{ CommandDisplay="Start Timing", IconGeometry=Application.Current.Resources["StartTimingIcon"]     as Geometry , Message=new CommandMessage{ Command = CommandType.StartTiming} },
                //new CommandVM{ CommandDisplay="Stop Timing",    IconGeometry=Application.Current.Resources["StopTimingIcon"]    as Geometry , Message=new CommandMessage{ Command = CommandType.StopWork} },
                //new CommandVM{ CommandDisplay="End of Day",     IconGeometry=Application.Current.Resources["CleanUpIcon"]       as Geometry , Message=new CommandMessage{ Command = CommandType.CleanUp} }
            };
            Reports = _reports;
            RaisePropertyChanged("Reports");


            //ObservableCollection<ReportVM> reports = new ObservableCollection<ReportVM>
            //{ new ReportVM{ ReportDisplay="Report1", ReportType=null, ViewModelType=null }
            //};
            #endregion

            inWorkToDo = null;
            SwitchDate = DateTime.MinValue;
            curTDTSortOrder = "";

            AdHocFolderID = (int?)App.Current.Properties["AdHocFolderID"];
            //  ChkContext("Starting ToDos");
            mLogger.AddLogMessage("--- ToDosViewModel end constructor ---");
        }
        //public ReportVM()
        //{
        //}
        //      public void RunReport1Execute()
        //      {
        //          //if (View == null && ViewType != null)
        //          //{
        //          //    if (ViewType != typeof(ReportsView))
        //          //    {
        //          //        View = (UserControl)Activator.CreateInstance(ViewType);
        //          //        var msg = new NavigateMessage { View = View, ViewModelType = ViewModelType, ViewType = ViewType };
        //          //        Messenger.Default.Send<NavigateMessage>(msg);

        //          //    }
        //          //    else
        //          //    {
        //          //  Displaying a report window
        ////          rView = (Window)Activator.CreateInstance(ReportType);
        //          rView = new ReportsView();
        //          rView.Show();
        //          rView.Focus();
        //          //}
        //          //}
        //          //var msg = new NavigateMessage { View = View, ViewModelType = ViewModelType, ViewType = ViewType };
        //          //Messenger.Default.Send<NavigateMessage>(msg);
        //          //  Displaying a report window
        //          //rView = (Window)Activator.CreateInstance(ViewType);
        //          //rView.Show();
        //          //rView.Focus();

        //      }
        protected void EditCurrent()
        {
            EditVM = SelectedToDo;
            mLogger.AddLogMessage("(ToDosViewModel) EditVM->" + EditVM.TheEntity.Item);

            EditVM.IsNew = false;
            mLogger.AddLogMessage("Reached ToDo EditCurrent with '" + EditVM.TheEntity.Item.Trim() + "' - " + EditVM.TheEntity.Status + "------------------>");
            TitlePopUp = "Editing '" + EditVM.TheEntity.Item.Trim() + "' ?";
            IsInEditMode = true;
        }

        protected void InsertNew()
        {
            if (IsInEditMode)
            {
                // Already in edit mode
                return;  
            }
            // Adding new ToDo from ToDo List
            //    First need new Project in AdHoc
   //        Project newAdHocProj = AddAdHocProject();   // 2A problem ??
            //    Then add new ToDo to that Project
            EditVM = new ToDoVM();
          //  EditVM.TheEntity.ProjectID = newAdHocProj.ProjectID;
            mLogger.AddLogMessage("Reached InsertNew begin editing in ToDosViewModel with '" + EditVM.TheEntity.Item.Trim() + "' - " + EditVM.TheEntity.Status + "------------------>");
            TitlePopUp = "Adding new ToDo Item";  // Used as flag for Adding New
            IsInEditMode = true;  // Opens EditPopUp
        }

        private Project AddAdHocProject()
        {
            Project newProj = new Project();
            //var adHocFolderID = (from c in db.Folders
            //                   where c.FolderName == "AdHoc"
            //                   select c.FolderID).FirstOrDefault();

            newProj.FolderID = AdHocFolderID; //adHocFolderID;
            //newProj.Folder = new Folder();
            string temp9 = Util.newPPartNumForProj(AdHocFolderID, dbBase);
            newProj.PPartNum = temp9;  // "987000000";
            newProj.PSortOrder = temp9;  // "987000000";
            newProj.Item = "AdHocPlaceHolder";
            newProj.DetailedDesc = "Details. . .";
            newProj.Priority = "A";
            newProj.Status = "O";  // "W";
            newProj.StartDate = DateTime.Today;
            newProj.DueDate = DateTime.Today.AddDays(5);
            newProj.RevDueDate = DateTime.Today.AddDays(5);
            newProj.DoneDate = null;  // DateTime.MinValue;
            newProj.RespPerson = "AdHoc";  // "Bill";
            newProj.Hide = false;
            newProj.DispLevel = "1";
            newProj.Done = false;

            int nProj0 = dbBase.Projects.Count();
            dbBase.Projects.Add(newProj);
            int nProj = dbBase.Projects.Count();
            mLogger.AddLogMessage("Added new AdHoc project, calling UpdateDB. - " + newProj.Item + " nProj: " + nProj0 + "-" + nProj);
            UpdateDB();
            newProj.ProjectID = newProj.ProjectID;
          return newProj;
        }

        protected void CommitUpdates()
        {
            string t = TitlePopUp;
            mLogger.AddLogMessage("$$CommitUpdates() TitlePopUp - " + t);
            if (EditVM == null || EditVM.TheEntity == null)
            {
                mLogger.AddLogMessage("(ToDosViewModel) CommitUpdates  EditVM is null!!");
                if (dbBase.ChangeTracker.HasChanges())  // Presumed carry over when dg was directly editable
                {
                    UpdateDB();
                }
                return;
            }
            mLogger.AddLogMessage("(ToDosViewModel) CommitUpdates  EditVM->" + EditVM.TheEntity.Item);
            var theItem = EditVM.TheEntity.Item;
          //  ShowUserMessage("WORKING! in CommitUpdates");

            mLogger.AddLogMessage("Reached CommitUpdates with '" + EditVM.TheEntity.Item.Trim() + "' - '" + EditVM.TheEntity.Status + "' ------------->");
            if (EditVM.TheEntity.IsValid())
            {
                if (EditVM.IsNew)
                {
                    // Adding a new ToDo, need to have a project added in AdHocFolder to cover in Planning
                    Project newAdHocProj = AddAdHocProject();
                    newAdHocProj.Item = theItem.Trim();
                    EditVM.IsNew = false;
                    EditVM.TheEntity.ProjectID = newAdHocProj.ProjectID;
                    ToDos.Add(EditVM);
                    // Added to collection, now to add to the database.
                    dbBase.ToDos.Add(EditVM.TheEntity);
                    mLogger.AddLogMessage("Added new ToDo, calling UpdateDB. " + EditVM.TheEntity.Item);
                    UpdateDB();

                    Project theProj = dbBase.Projects.Find(EditVM.TheEntity.ProjectID);
                    theProj.Item = EditVM.TheEntity.Item;
                    theProj.DetailedDesc = EditVM.TheEntity.DetailedDesc;
                    theProj.Status = "O";  // "W";
                    ////Project projEntry = GetProjectEntry(EditVM.TheEntity);
                    ////db.Projects.Add(projEntry);
                    mLogger.AddLogMessage("Edited project, calling UpdateDB.");
                    UpdateDB();

                    SelectedToDo = EditVM;
                    Quit();
                }
                else if (dbBase.ChangeTracker.HasChanges())
                {
                    UpdateDB();
                    //  SelectedToDo = EditVM;
                    Quit();  // IsInEditMode = false;
                }
                else
                {
                    ShowUserMessage("No changes to save");
                }

            }
            else
            {
                ShowUserMessage("There are validation problems with the data entered");
                mLogger.AddLogMessage("There are validation problems with the data entered");
            }
        }

        private Project GetProjectEntry(ToDo theToDo)
        {
            Project newPEntry = new Project(theToDo.Item);

            newPEntry.DetailedDesc = theToDo.DetailedDesc;
            newPEntry.DispLevel = theToDo.DispLevel;
            newPEntry.Done = theToDo.Done;
            newPEntry.DoneDate = theToDo.DoneDate;
            newPEntry.DueDate = theToDo.DueDate;
            newPEntry.Hide = theToDo.Hide;
            // ++++++++++++++++++++++++++++++
            newPEntry.FolderID = 8;     // Hardcoded for "AdHoc"  ************************
            newPEntry.PPartNum = newPEntry.PSortOrder = Util.newPPartNumForProj(theToDo.ProjectID ?? 0, dbBase);
            //newPEntry.PSortOrder = theToDo;
            // ++++++++++++++++++++++++++++++
            newPEntry.Priority = theToDo.Priority;  // "B";
            newPEntry.ProjectID = theToDo.ProjectID ?? 0;
            newPEntry.RespPerson = theToDo.RespPerson;
            newPEntry.RevDueDate = theToDo.RevDueDate;
            newPEntry.StartDate = theToDo.StartDate;
            newPEntry.Status = theToDo.Status;
            return newPEntry;
        }
        //private string newPPartNumForProj(int theProjectID)
        //{
        //    /*
        //    var item = db.Items.OrderByDescending(i => i.Value).FirstOrDefault();
        //    IQueryable<Project> theProjs = (from c in db.Projects
        //                                    where c.FolderID == folder.FolderID &&
        //                                          c.PPartNum.Substring(3, 6) == "000000"
        //                                    orderby c.PPartNum
        //                                    select c);
        //    Project[] projects = theProjs.ToArray();
        //    return projects;
        //    */
        //    mLogger.AddLogMessage("newPPartNumForProj: " + theProjectID);
        //    TDTDbContext theDB = new TDTDbContext();
        //    //TDTDbContext theDB = (TDTDbContext)App.Current.Resources["theData"];
        //    Project ahProj = (from c in theDB.Projects
        //                      where c.ProjectID == theProjectID
        //                      select c).FirstOrDefault();
        //    if (ahProj != null)
        //    {
        //        string theProjs = (from c in theDB.Projects
        //                           where c.FolderID == ahProj.FolderID &&
        //                                 c.PPartNum.Substring(0, 3) == ahProj.PPartNum.Substring(0, 3) &&
        //                                 c.PPartNum.Substring(6, 3) == "000"
        //                           orderby c.PPartNum descending
        //                           select c.PPartNum.Substring(3, 3)).FirstOrDefault();
        //        string PartNum = ahProj.PPartNum.Substring(0, 3) + newNum(theProjs) + "000";
        //        mLogger.AddLogMessage("NewProject PartNum: " + PartNum);
        //        return PartNum;
        //    }
        //    // Problem
        //    mLogger.AddLogMessage("ahProj: " + ahProj.Item);
        //    return "999999999";

        //    //string theProjs = (from c in theDB.Projects
        //    //                   where c.FolderID == theFolderID &&
        //    //                         c.PPartNum.Substring(3, 6) == "000000"
        //    //                   orderby c.PPartNum descending
        //    //                   select c.PPartNum.Substring(0, 3)).FirstOrDefault();
        //    //Project[] projects = theProjs.ToArray();

        //    //string PartNum = newNum(theProjs) + "000000";
        //    //mLogger.AddLogMessage("NewProject: " + PartNum);
        //    //return PartNum;
        //    //throw new NotImplementedException();
        //}

        //private string newNum(string lastNum)
        //{
        //    // Increment the old maximum and format with leading zeros
        //    int nNew = Convert.ToInt32(lastNum);
        //    nNew++;
        //    nNew = nNew + 1000;
        //    string newNumStr = nNew.ToString().Substring(1, 3);
        //    return newNumStr;
        //    //throw new NotImplementedException();
        //}

        public void UpdateDB()   // Update the Database
        {
            mLogger.AddLogMessage("==== UpdateDB ==== ToDosViewModel l 505 =============>");
            try
            {
                Util.ChkContext(dbBase, "== ToDosViewModel.UpdateDB == l 510" );
                // ===========================================================
                //////string changeType = "";
                //////int historyCount = 0;
                //////foreach (var history in dbBase.ChangeTracker.Entries()
                //////  .Where(e => e.Entity is IModificationHistory && (e.State == EntityState.Added ||
                //////          e.State == EntityState.Modified))
                //////   .Select(e => e.Entity as IModificationHistory)
                //////  )
                //////{
                //////    historyCount++;
                //////    //history.DateModified = DateTime.Now;
                //////    //if (history.DateCreated == DateTime.MinValue)
                //////    //{
                //////    //    history.DateCreated = DateTime.Now;
                //////    //}
                //////    changeType = history.GetType().ToString();
                //////    if (changeType.Contains("Project"))
                //////    {
                //////        mLogger.AddLogMessage("T UpdateDB -" + historyCount + "  Project Changed- '" + ((Project)history).Item + "' -" + changeType);
                //////    }
                //////    else if (changeType.Contains("ToDo"))
                //////    {
                //////        mLogger.AddLogMessage("T UpdateDB -" + historyCount + "  ToDo Changed- '" + ((ToDo)history).Item + "' -" + changeType);
                //////    }
                //////    else
                //////    {
                //////        mLogger.AddLogMessage("T UpdateDB -" + historyCount + "  Track Changed- '" + ((Track)history).Item + "' -" + changeType);
                //////    }
                //////}
                // ================================================================



                //if (db.ChangeTracker.HasChanges())
                //{
                int nChanges = dbBase.SaveChanges();
                    ShowUserMessage("Database Updated with " + nChanges.ToString() + " changes.");
                    mLogger.AddLogMessage("(ToDosViewModel) UpdateDB successfully completed. with " + nChanges.ToString() + " changes.");
                //}
               // RefreshData();  // >>>>>>>>>>>>>>>>>>>>>>
            }
            catch (DbEntityValidationException dbEx)
            {
                mLogger.AddLogMessage("DbEntityValidationException: =====");
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        mLogger.AddLogMessage("ToDos. Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        mLogger.AddLogMessage("ToDos. Property: " + validationError.PropertyName + " Error: " + validationError.ToString());
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException deDbEx)
            {
                //foreach (var vErrors in deDbEx.InnerException)
                //{
                var theProblem = deDbEx.InnerException.Message;
                //}  deDbEx.InnerException.InnerException.Message
                if (deDbEx.InnerException.InnerException != null)
                {
                    mLogger.AddLogMessage("DbUpdateException: " + deDbEx.InnerException.InnerException.Message);
                }
                else
                {
                    mLogger.AddLogMessage("DbUpdateException0: " + deDbEx.InnerException.Message);
                }
            }
            catch (Exception e)
            {
                if (System.Diagnostics.Debugger.IsAttached && (e.InnerException.GetBaseException() != null))
                {
                   ErrorMessage = e.InnerException.GetBaseException().ToString();
                }
                mLogger.AddLogMessage("Exception: " + e.InnerException.GetBaseException().ToString());
                ShowUserMessage("There was a problem updating the database");
            }
            //RaisePropertyChanged("ToDos");
            //Quit();
        }

        //protected void DeleteCurrent() // Delete current selection
        //{
        //    int n = SelectedToDoIndex;
        //    EditVM = SelectedToDo;
        //    if (EditVM.TheEntity.Status == "A")
        //    {
        //        // Can't delete active ToDo
        //        ShowUserMessage("Can't delete the active ToDo.  Stop timing and then delete.");
        //        return;
        //    }

        //    if (Util.CheckDeleteOK("Are you sure you want to delete the current ToDo, '" + EditVM.TheEntity.Item.Trim() + "'?"))
        //    {
        //        mLogger.AddLogMessage("Confirmed DeleteCurrent ToDo DeleteCurrent with '" + EditVM.TheEntity.Item.Trim() + "' - " + EditVM.TheEntity.Status + "------------------>");
        //        // Need to adjust Project status or remove Project if AdHoc
        //        Project toDoProj = dbBase.Projects.Find(SelectedToDo.TheEntity.ProjectID);
        //        Util.AdjustProjStatusForToDoDelete(EditVM.TheEntity, dbBase);
        //        // Delete ToDo
        //        dbBase.ToDos.Remove(SelectedToDo.TheEntity);
        //        ToDos.Remove(SelectedToDo);
        //        RaisePropertyChanged("ToDos");

        //        //  NEED TO REVIEW, multiple ToDos for a given project, don't delete without checking
        //        //if (toDoProj.FolderID == AdHocFolderID)
        //        //{
        //        //    //toDoProj.Status = SelectedToDo.TheEntity.Status;
        //        //    // other Project adjustments, then delete ToDo
        //        //    db.Projects.Remove(toDoProj);
        //        //}

        //        UpdateDB();
        //        //CommitUpdates();
        //        //selectedEntity = null;  // Nothing should be selected // Delete occurred set focus to previous value (n -1)
        //        // Delete occurred set focus to previous value (n -1)
        //        n = n >= 1 ? n - 1 : 0;
        //    }
        //    // Delete did NOT occur, reset focus to n

        //    Util.SelectRowByIndex(TheDGToDos, n);
        //}

        protected void CleanUp() // Delete 'F' work Items
        {
           // CleanUpProjs();
            //EditVM = SelectedToDo;
            //ToDosWithF = 
            mLogger.AddLogMessage("Reached CleanUp List of ToDos:");
         //   ShowUserMessage("CleanUp List of ToDos");
            foreach (ToDo toDo in dbBase.ToDos)
            {
                mLogger.AddLogMessage("dbToDos Item: " + toDo.Item + " - " + toDo.Status + " - " + toDo.ProjectID);

                //if (toDo.Status == "A")
                //{
                //    toDo.ElapsedTime = 0.0M;
                //    continue;  // Future version will close an existing 'A'.
                //}
                Project p = dbBase.Projects.Find(toDo.ProjectID);
                ProcessToDo(toDo, p);
                //db.ToDos.r
                mLogger.AddLogMessage("dbToDos Item: " + toDo.Item + " - " + toDo.ProjectID + " - " + p.Status + " - " + p.Item);

                //_ToDos.Add(new ToDoVM { IsNew = false, TheEntity = cust });
            }

            //var ToDosWithF = (from c in dbBase.ToDos
            //                  where ((c.Status == "F" || c.Done) && c.Priority != "R")
            //                  select c);
            //if (ToDosWithF.Count() != 0)
            //{
            //    mLogger.AddLogMessage("Reached CleanUp with " + ToDosWithF.Count().ToString() + " F/Done items. " + "------------------>");
            //    dbBase.ToDos.RemoveRange(ToDosWithF);
            //    //db.ToDos.Remove(SelectedToDo.TheEntity);
            //    //ToDos.Remove(SelectedToDo
            //    //return;
            //}
            //else
            //{
            //    mLogger.AddLogMessage("CleanUp found there were no non 'R' items in work with 'F'");
            //}

            //var ToDosWithR = (from c in dbBase.ToDos
            //                  where ( c.Priority == "R")
            //                  select c);
            //if (ToDosWithR.Count() != 0)
            //{
            //    foreach (var todo in ToDosWithR)
            //    {
            //        todo.Status = "O";
            //        todo.ElapsedTime = 0.0M;
            //    }


            //}
            //else
            //{
            //    mLogger.AddLogMessage("CleanUp found there were no non 'R' items in work with 'F'");
            //}

       //     RebuildTDTSortOrder();

            RaisePropertyChanged("ToDos");

            UpdateDB();
            //CommitUpdates();
            RefreshData();
            selectedEntity = null;  // Nothing should be selected
            ShowUserMessage("CleanUp List of ToDos complete");
        }

        private void CleanUpProjs()
        {
            foreach (Project proj in dbBase.Projects)
            {
                proj.Status = " ";
            }
        }

        private void ProcessToDo(ToDo toDo, Project p)
        {
            //toDo.ElapsedTime = 0.0M;
            // Reset timing variables
            //toDo.ElapsedTime = 0.0M;
            //toDo.StartDate = DateTime.MinValue;
            //toDo.DoneDate = DateTime.MaxValue;

            if (toDo.Priority == "R")
            {
                dbBase.ToDos.Remove(toDo);
                MarkProjectStatus(p.ProjectID, " ");
                //    // Project adjustments
                //    //string t2 = toDo.Status;
                //    //switch (t2)
                //    //{
                //    //    case "F":   // Finished, mark as "O"
                //    //        toDo.Status = "O";
                //    //        toDo.Done = false;s
                //    //        MarkProjectStatus(p.ProjectID, "F");
                //    //        toDo.ElapsedTime = 0.0M;
                //    //        //dbBase.ToDos.Remove(toDo);
                //    //        break;
                //    //    //case "R":   // Reminder only
                //    //    //    //toDo.Status = "F";
                //    //    //    MarkProjectStatus(p.ProjectID, "F");
                //    //    //    dbBase.ToDos.Remove(toDo);
                //    //    //    break;
                //    //    //case "C":   // Continuing
                //    //    //            //toDo.Status = "C";
                //    //    //    MarkProjectStatus(p.ProjectID, "W");
                //    //        //break;
                //    //    case "A":   // Active
                //    //                //toDo.Status = "A";
                //    //        MarkProjectStatus(p.ProjectID, "A");
                //    //        break;
                //    //    //case "O":   // Work Ready
                //    //    //    MarkProjectStatus(p.ProjectID, "O");
                //    //    //    break;
                //    //    case "I":   // Interrupted.  Change to 'W' or leave 'I' ????
                //    //        toDo.Status = "I";
                //    //        MarkProjectStatus(p.ProjectID, "I");
                //    //        break;
                //    //    case "W":   // Partially complete, done for today
                //    //        toDo.Status = "I";
                //    //        MarkProjectStatus(p.ProjectID, "I");
                //    //        break;

                //    //    default:
                //    //        break;
                //    //}
            }
            else
            {
                // toDo.Priority != "R"
                // Project adjustments
                string t = toDo.Status;
                switch (t)
                {
                    case "F":   // Finished
                        MarkProjectStatus(p.ProjectID, "F");
                        dbBase.ToDos.Remove(toDo);
                        break;
                    
                    case "A":   // Active
                                //toDo.Status = "A";
                        MarkProjectStatus(p.ProjectID, "A");
                        break;
                    case "O":   // Work Ready
                        MarkProjectStatus(p.ProjectID, "O");
                        break;
                    case "I":   // Interrupted.  Change to 'W' or leave 'I' ????
                        toDo.Status = "I";
                        MarkProjectStatus(p.ProjectID, "I");
                        break;
                    case "W":   // Partially complete, done for today
                        toDo.Status = "I";
                        MarkProjectStatus(p.ProjectID, "I");
                        break;

                    default:
                        break;
                }
            }

        }

        private void RebuildTDTSortOrder()
        {
            // Get desired ordered version
      //      ObservableCollection<ToDoVM> _ToDos = new ObservableCollection<ToDoVM>();
            var toDos = dbBase.ToDos
                           .AsEnumerable()
                           .OrderBy(ToDo => ToDo.Status, new CustomSort()).ThenBy(ToDo => ToDo.Priority).ThenBy(ToDo => ToDo.Item);//.ToListAsync();
                          // .OrderBy(todo => todo.Status, new CustomSort()).ThenBy(todo => todo.Item);//.ToListAsync();
                                                                                                     // Process list and set TDTSortOrder
            int i = 3;
           foreach (ToDo t in toDos)
            {
                if (t.Status == "A")
                {
                    t.TDTSortOrder = "000000000";
                }
                else
                {
                    string so = ((i + 1000000000).ToString()).Substring(1);
                    t.TDTSortOrder = so;
                    i = i + 3;
                }
                // _ToDos.Add(new ToDoVM { IsNew = false, TheEntity = cust });
                mLogger.AddLogMessage("RebuildTDTSortOrder: " + t.Item + " - " + t.Status + "-" + t.TDTSortOrder);
            }
        }

        protected void StartTiming()  // Show popup to get started on work
        {
           // StartTimingButton();
          //  return;

            mLogger.AddLogMessage(" ======== StartTiming ==============");
            // Selection required to get here
            EditVM = SelectedToDo;
            
            // Make sure not already in Active status
            if (EditVM.TheEntity.Status == "A")
            {
                ShowUserMessage("'" + EditVM.TheEntity.Item.Trim() + "' is already in work!");
                mLogger.AddLogMessage("'" + EditVM.TheEntity.Item.Trim() + "' is already in work!");
                return;
            }
            // Check if eligible for starting work
            if (EditVM.TheEntity.Status == "R")
            {
                ShowUserMessage("'" + EditVM.TheEntity.Item.Trim() + "' is a Reminder!");
                mLogger.AddLogMessage("'" + EditVM.TheEntity.Item.Trim() + "' is a Reminder!");
                HandleReminder();
                return;
            }

            // Should now have W, I, C, F, O as Status ===========================
            mLogger.AddLogMessage("Reached StartTiming with '" + EditVM.TheEntity.Item.Trim() + "' - " + EditVM.TheEntity.Status + " ------------------>");
            //  mLogger.AddLogMessage("StartTiming beginning for " + "'" + EditVM.TheEntity.Item.Trim() + "'");
            //     SwitchDate = DateTime.Now;
            curTDTSortOrder = EditVM.TheEntity.TDTSortOrder;
            if (SwitchDate == DateTime.MinValue)
            {
                //No Stop screen so need to set value
                EditVM.TheEntity.StartDate = DateTime.Now;
            }
            else
            {
                // SwitchDate set in StopWorkPopUp
                EditVM.TheEntity.StartDate = SwitchDate;
                SwitchDate = DateTime.MinValue;
            }
            EditVM.TheEntity.RaisePropertyChanged("StartDate");

            inWorkToDo = CheckForInWork();  // returns the ToDo, if any, with 'A' , i.e. in WORK

            // Display StartTiming popup
            if (inWorkToDo == null)
            {
                // Fresh Start, nothing already in work.
                TitlePopUp1 = "Starting work on '" + EditVM.TheEntity.Item.Trim() + "' ?";
                TitlePopUp = "";
                // Nominal start with nothing currently in work
                mLogger.AddLogMessage("Popup 0: '" + TitlePopUp + "'");
                mLogger.AddLogMessage("---DIALOG: IsInStartTimingMode, Fresh Start.  '" + EditVM.TheEntity.Item.Trim() + "'");
                //EditVM.TheEntity.StartDate = SwitchDate;
                IsInStartTimingMode = true;  // Display PopUp for Start Timing
            }
            else
            {
                // Have to stop inWorkToDo, then start work on EditVM.TheEntity.Item
                StopWork();
          //     StopStartWork();
            }
        }

        private void HandleReminder()
        {
            mLogger.AddLogMessage("HandleReminder reached.");
            throw new NotImplementedException();
        }

        protected int IndexOfA()
        {
            // Find element of ToDos with status of A
            int i = -1;
            int AIndex = -1;
            foreach (var aTodo in ToDos)
            {
                i++;
                mLogger.AddLogMessage(i + " : " + aTodo.TheEntity.Item);
                if (aTodo.TheEntity.Status == "A")
                {
                    // found an "A", could be last element
                    AIndex = i;
                    break;  
                }
                else
                {
                    continue;
                }
            }
            ////if (i == (ToDos.Count - 1))
            ////{
            ////    // Nothing found
            ////    i = -1;
            ////}
            mLogger.AddLogMessage( "IndexOfA -> " + AIndex + " : " + ToDos.Count);
            return AIndex;
        }

        private ToDoVM CheckForInWork()
        {
            ToDoVM workingToDoVM;
            ToDo workingToDo = null;
            // Look for any Item in work, current "A".  Should be at most one
            //mLogger.AddLogMessage(" CheckForInWork --------------------->");
            var workingToDos = (from c in dbBase.ToDos
                                where c.Status == "A"
                                select c);
            int nW = workingToDos.Count();
            if (nW == 0)
            {
                mLogger.AddLogMessage(" CheckForInWork -- There were no items in work with 'A'");
                return workingToDoVM = null;
            }
            else
            {
                workingToDo = workingToDos.First();
                workingToDo.DoneDate = DateTime.Now;

                mLogger.AddLogMessage(" CheckForInWork -- Already in work ->" + workingToDo.Item.Trim());
                mLogger.AddLogMessage("workingToDos count: " + nW);
                workingToDoVM = new ToDoVM();
                workingToDoVM.IsNew = false;
                workingToDoVM.TheEntity = workingToDo;
                return workingToDoVM;
            }
        }

      //  protected void CommitStartTiming()  // Popup is up and we want to process and close
      //  {
      //      mLogger.AddLogMessage("Reached CommitStartTiming started" + "------------------>");
      //      // Proceeding to start work with current item

      //      //   SwitchAtoI();  // Need to interrupt any active work Item with Status = 'A'
      //      // Back to working with the current work Item
      //      EditVM.TheEntity.Status = "A";
      //      EditVM.TheEntity.TDTSortOrder = "000000000";
      ////      EditVM.TheEntity.ElapsedTime = 0.0M;
      //      //EditVM.TheEntity.StartDate = DateTime.Now;
      //      if (inWorkToDo != null)
      //      {
      //          // Stopping an active so need to capture end date from dialog.
      //          SwitchDate = (DateTime)EditVM.TheEntity.DoneDate;  // from dialog
      //      }

      //      mLogger.AddLogMessage("* Changed selected ToDo to active status: '" + EditVM.TheEntity.Item.Trim() + "' - " + EditVM.TheEntity.Status);
      //      //int nProjectID = this.SelectedToDo.TheEntity.ProjectID;  // int.Parse(listViewToDos.FocusedItem.SubItems[6].Text);
      //      //int nToDoID = this.SelectedToDo.TheEntity.ToDoID;  // int.Parse(listViewToDos.FocusedItem.SubItems[8].Text);

      //      // Build Track record and save it
      //      TrackVM = new Planner.TrackVM();
      //      TrackVM.TheEntity.Item = EditVM.TheEntity.Item;
      //      TrackVM.TheEntity.ProjectID = EditVM.TheEntity.ProjectID;
      //      TrackVM.TheEntity.StartDate = EditVM.TheEntity.StartDate;
      //      TrackVM.TheEntity.DetailedDesc = EditVM.TheEntity.DetailedDesc;
      //      TrackVM.TheEntity.ElapsedTime = 0.0M;
      //      TrackVM.TheEntity.BillTime = 0.0M;
      //      TrackVM.TheEntity.BillRef = "tBd";
      //      //     TrackVM.TheEntity.EndDate = DateTime.Now;
      //      TrackVM.TheEntity.Expenses = 0.0M;
      //      TrackVM.TheEntity.Mileage = 0.0M;
      //      //  TrackVM.TheEntity.Project = EditVM.TheEntity.Project;
      //      TrackVM.TheEntity.RespPerson = EditVM.TheEntity.RespPerson;
      //      TrackVM.TheEntity.Status = EditVM.TheEntity.Status;  // starts out as 'A'
      //                                                           // TrackVM.TheEntity.SortOrder = "000001";  // EditVM.TheEntity.TDTSortOrder;

      //      Tracks.Add(TrackVM);
      //      dbBase.Tracks.Add(TrackVM.TheEntity);
      //      mLogger.AddLogMessage("Created New Track with '" + TrackVM.TheEntity.Item.Trim() + "' - " + TrackVM.TheEntity.Status);

      //      MarkProjectStatus(EditVM.TheEntity.ProjectID ?? 0, "A");

      //      UpdateDB();

      //      QuitStartTiming();  // Close popup and Refresh Data

      //      SelectedToDo = EditVM;  // Set Selected Item
      //  }

        //private void SwitchAtoI()
        //{
        //    // Look for any Item in work, current "A".  Should be at most one
        //    mLogger.AddLogMessage(" A to I --------------------->");
        //    var workingToDos = (from c in dbBase.ToDos
        //                        where c.Status == "A"
        //                        select c);
        //    if (workingToDos.Count() == 0)
        //    {
        //        mLogger.AddLogMessage("---    There were no items in work with 'A'");
        //        return;
        //    }
        //    //var workingToDo = (from c in db.ToDos
        //    //                  where c.Status == "A"
        //    //                  select c).First();
        //    //if (workingToDo == null)
        //    //{
        //    //    mLogger.AddLogMessage("No ToDo with 'A'");
        //    //    return;
        //    //}
        //    else
        //    {
        //        // Found an 'A'
        //        var workingToDo = workingToDos.First();
        //        mLogger.AddLogMessage("'" + workingToDo.Item.Trim() + "'  -  active, already in work, Stop it.");
        //        DateTime stopStart = EditVM.TheEntity.StartDate;  // Start from newly selected ToDo

        //        //   Update corresponding ToDo to "F" and add ElapsedTime to DetailedDesc
        //        workingToDo.Status = "I";  // was "F" show "F" in Track
        //        var workingTrack = (from c in dbBase.Tracks
        //                            where c.ProjectID == workingToDo.ProjectID
        //                            orderby c.ProjectID, c.StartDate descending
        //                            select c).First();
        //        if (workingTrack == null)
        //        {
        //            mLogger.AddLogMessage("Did not find expected 'A' Track with ID = " + workingToDo.ProjectID.ToString());
        //            return;
        //        }
        //        //   Switch to "I" and calculate ElapsedTime for Track
        //        workingTrack.Status = "I";
        //        // Calculate time worked
        //        DateTime StartDate = workingTrack.StartDate;
        //        TimeSpan WorkTime = stopStart - StartDate;
        //        workingTrack.EndDate = stopStart;
        //        workingTrack.ElapsedTime = Math.Round((decimal)WorkTime.TotalMinutes, 2);  // formatting
        //        // Update the ToDo
        //        workingToDo.Status = "F";
        //        workingToDo.DetailedDesc += "\n Interrupted after " + workingTrack.ElapsedTime.ToString() + " minutes. ";
        //        mLogger.AddLogMessage("WorkingTrack found, Elapsed Time calculated -> " + workingTrack.ElapsedTime.ToString() + " minutes. ");
        //        workingToDo.ElapsedTime = (decimal)workingTrack.ElapsedTime + workingToDo.ElapsedTime;

        //        workingToDo.TDTSortOrder = curTDTSortOrder;
                
        //        //db.Tracks.Find
        //        //throw new NotImplementedException();
        //        //StopWork("I");
        //        //bool t = db.ChangeTracker.HasChanges();
        //        int nChanges = dbBase.SaveChanges();
        //        mLogger.AddLogMessage("AtoI Updated Database with " + nChanges.ToString() + " changes.");

        //    }
        //}

        //protected void QuitStartTiming()
        //{
        //    mLogger.AddLogMessage("Reached QuitStartTiming " + "------->" + EditVM.TheEntity.Item);
        //    IsInEditMode = false;     // close popup
        //    IsInStartTimingMode = false;
        //    IsInStopWorkMode = false;

        //    RefreshData();  // In CrudVMBaseTDT  ??
        //    //  Calls GetData() and sets UserMessage to 'Data Refreshed'
        //}

        protected void StopWork()
        {
            EditVM = SelectedToDo;  // WorkItem to start work on
            //if (EditVM.TheEntity.Status != "A")
            if (SelectedToDo.TheEntity.Status != "A")
            {
                mLogger.AddLogMessage("Reached StopWork erroneously with '" + SelectedToDo.TheEntity.Item.Trim() + "' - " + SelectedToDo.TheEntity.Status + " --should be A ");
                return;
            }

            mLogger.AddLogMessage("===== Reached StopWork with '" + EditVM.TheEntity.Item.Trim() + "' - " + EditVM.TheEntity.Status + " ====>");
            mLogger.AddLogMessage("Reached StopWork");
            // Need to either Stop current item
            //    OR  Stop active item and then start current item.
            //  What is work item to be stopped?   inWorkToDo = CheckForInWork();
                                    //     SwitchDate = DateTime.MinValue;
            //if (inWorkToDo == null)
            //{
                // Just stop the current item
                mLogger.AddLogMessage("Stopping work on active WorkItem.");  //  starting only
                TitlePopUp1 = "Stopping work on '" + EditVM.TheEntity.Item.Trim() + "' ?";
                TitlePopUp = "";
                EditVM.TheEntity.DoneDate = DateTime.Now;
                EditVM.TheEntity.RaisePropertyChanged("DoneDate");
                EditVM.TheEntity.TDTSortOrder = "999999990";// curTDTSortOrder;  // stopping single
                RaisePropertyChanged();
                //mLogger.AddLogMessage("Popup 0 " + TitlePopUp);
                //mLogger.AddLogMessage("Popup 1 " + TitlePopUp1);
                IsInStopWorkMode = true;
            //}
            //else
            //{
            //    mLogger.AddLogMessage("Stopping inWork before starting another.");

            //    TitlePopUp = "Confirm stopping work on '" + InWorkToDo.TheEntity.Item.Trim() + "' ?";
            //    TitlePopUp1 = "Then starting work on '" + EditVM.TheEntity.Item.Trim() + "' ?";
            //    // Switch EditVM to be inWorkToDo
            //    EditVM = new ToDoVM();  // Dialog elements bound to EditVM
            //    EditVM.TheEntity = InWorkToDo.TheEntity;
            //    EditVM.TheEntity.DoneDate = DateTime.Now;  // Adjust to current time
            //    EditVM.TheEntity.TDTSortOrder = "000000001";//curTDTSortOrder;  // stopping by interupt
            //                                                //      inWorkToDo.Status = "I";
            //                                                //    EditVM.TheEntity.RaisePropertyChanged();
            //    EditVM.TheEntity.RaisePropertyChanged("DoneDate");
            //    // Have to stop inWorkToDo, then start on EditVM.TheEntity.Item
            //    // IsInStopWorkMode = true;
            //    //IsInStartTimingMode = true;
            //}
            // EditVM = SelectedToDo;
            mLogger.AddLogMessage("Title for Popup 0 '" + TitlePopUp + "'");
            mLogger.AddLogMessage("Title for Popup 1 '" + TitlePopUp1 + "'");
            //if (EditVM.TheEntity.Status != "A")
            //{
            //    mLogger.AddLogMessage("Reached StopWork erroneously with '" + EditVM.TheEntity.Item.Trim() + "' - " + EditVM.TheEntity.Status + " ------------------>");
            //    return;
            //}
            mLogger.AddLogMessage("Reached StopWork with '" + EditVM.TheEntity.Item.Trim() + "' - " + EditVM.TheEntity.Status + " ------------------>");
            mLogger.AddLogMessage("---DIALOG: IsInStopWorkMode = true;");
            mLogger.AddLogMessage(" --- ");
            // Display StopWorkPopUp
           // IsInStopStartWorkMode = true;
        }
        protected void StopStartWork()
        {
            //   Called from StartTimingButton
            mLogger.AddLogMessage("Reached StopStartWork");
            // Need to either Stop current item
            //    OR  Stop active item and then start current item.
            //  What is work item to be stopped?   inWorkToDo = CheckForInWork();
            EditVM = SelectedToDo;  // WorkItem to start work on
                                    //     SwitchDate = DateTime.MinValue;
            if (InWorkToDo == null)
            {
                // Just stop the current item  NOT REACHED
                mLogger.AddLogMessage("Stopping work on active WorkItem.");  //  starting only
                TitlePopUp1 = "Stopping work on '" + EditVM.TheEntity.Item.Trim() + "' ?";
                TitlePopUp = "";
                EditVM.TheEntity.DoneDate = DateTime.Now;
                EditVM.TheEntity.RaisePropertyChanged("DoneDate");
                EditVM.TheEntity.TDTSortOrder = "999999990";// curTDTSortOrder;  // stopping single
                RaisePropertyChanged();
                //mLogger.AddLogMessage("Popup 0 " + TitlePopUp);
                //mLogger.AddLogMessage("Popup 1 " + TitlePopUp1);
            }
            else
            {
                mLogger.AddLogMessage("Stopping inWork before starting another.");

                mLogger.AddLogMessage("InWorkToDo.TheEntity.Item = '" + InWorkToDo.TheEntity.Item + "'");
                mLogger.AddLogMessage("EditVM.TheEntity.Item = '" + EditVM.TheEntity.Item + "'");
                mLogger.AddLogMessage("SelectedToDo.TheEntity.Item = '" + SelectedToDo.TheEntity.Item + "'");

                TitlePopUp = "Confirm stoping work on '" + InWorkToDo.TheEntity.Item.Trim() + "' ?**";
                TitlePopUp1 = "Then starting work on '" + SelectedToDo.TheEntity.Item.Trim() + "' ?**";
                ////// Switch EditVM to be inWorkToDo
                ////EditVM = new ToDoVM();  // Dialog elements bound to EditVM
                ////EditVM.TheEntity = InWorkToDo.TheEntity;
                ////EditVM.TheEntity.DoneDate = DateTime.Now;  // Adjust to current time
                ////EditVM.TheEntity.TDTSortOrder = "000000001";//curTDTSortOrder;  // stopping by interupt
                ////                                            //      inWorkToDo.Status = "I";
                ////                                            //    EditVM.TheEntity.RaisePropertyChanged();
                ////EditVM.TheEntity.RaisePropertyChanged("DoneDate");

                InWorkToDo.TheEntity.DoneDate = DateTime.Now;  // Adjust to current time
                InWorkToDo.TheEntity.TDTSortOrder = "000000001";//curTDTSortOrder;  // stopping by interupt


                // Have to stop inWorkToDo, then start on EditVM.TheEntity.Item
                // IsInStopWorkMode = true;
                //IsInStartTimingMode = true;
            }
            // EditVM = SelectedToDo;
            mLogger.AddLogMessage("Title for Popup 0 '" + TitlePopUp + "'");
            mLogger.AddLogMessage("Title for Popup 1 '" + TitlePopUp1 + "'");
            if (InWorkToDo.TheEntity.Status != "A")
            {
                mLogger.AddLogMessage("Reached StopStartWork erroneously with '" + SelectedToDo.TheEntity.Item.Trim() + "' - " + SelectedToDo.TheEntity.Status + " ------------------>");
                return;
            }
            mLogger.AddLogMessage("Reached StopWork with '" + EditVM.TheEntity.Item.Trim() + "' - " + EditVM.TheEntity.Status + " ------------------>");
            mLogger.AddLogMessage("---DIALOG: IsInStopWorkMode = true;");
            mLogger.AddLogMessage(" --- ");
            // Display StopWorkPopUp
            //IsInStopWorkMode = true;
            mLogger.AddLogMessage("InWorkToDo.TheEntity.Item = '" + InWorkToDo.TheEntity.Item + "'");
            mLogger.AddLogMessage("EditVM.TheEntity.Item = '" + EditVM.TheEntity.Item + "'");
            mLogger.AddLogMessage("SelectedToDo.TheEntity.Item = '" + SelectedToDo.TheEntity.Item + "'");
            IsInStopStartWorkMode = true;
        }

        protected void CommitStopWork()
        {
            //base.CommitStopWork();  // doesn't do anything!
            // EditVM       stopping ToDo
            // SelectedToDO starting ToDo
            mLogger.AddLogMessage("Reached CommitStopWork with '" + EditVM.TheEntity.Item + "' and '" + EditVM.TheEntity.Item + "'");
            // Close out current track
            DateTime StopStartDate = DateTime.Now;   // (DateTime)EditVM.TheEntity.DoneDate;  // from Dialog
            try
            {
                StoppingActiveToDo(StopStartDate, SelectedToDo);
                //var workingTrack = (from c in dbBase.Tracks
                //                    where c.ProjectID == EditVM.TheEntity.ProjectID
                //                    orderby c.ProjectID, c.StartDate descending
                //                    select c).First();

                //// Update ToDo Item and DetailedDesc just in case editing occurred
                //workingTrack.Item = EditVM.TheEntity.Item;
                //workingTrack.DetailedDesc = EditVM.TheEntity.DetailedDesc;

                //// Calculate time worked
                //DateTime StartDate = workingTrack.StartDate;
                //TimeSpan WorkTime = StopDate - StartDate;
                //decimal nnn = Math.Round((decimal)WorkTime.TotalMinutes, 2);
                //workingTrack.EndDate = StopDate;
                //////if (workingTrack.ElapsedTime == 0.0M)
                //////{
                //////    workingTrack.ElapsedTime = nnn;
                //////}
                //////else
                //////{
                //////    workingTrack.ElapsedTime = workingTrack.ElapsedTime + nnn;
                //////}
                //workingTrack.ElapsedTime = Math.Round((decimal)WorkTime.TotalMinutes, 2);
                ////  workingTrack.DetailedDesc += "\n Finished after " + workingTrack.ElapsedTime.ToString() + " minutes. ";
                //mLogger.AddLogMessage(" Finished after " + workingTrack.ElapsedTime.ToString() + " minutes. ");

                //// Update track record with some defaults
                //workingTrack.RespPerson = "Henry";
                //workingTrack.EndDate = StopDate;
                //workingTrack.Mileage = 0.01M;
                //workingTrack.BillRef = "tbd";
                //workingTrack.BillTime = workingTrack.ElapsedTime;
                ////    string temp = Flag;
                //workingTrack.Status = "F";

                //// Update ToDo record, want ElapsedTime to accumuate over work intervals for the day
                //EditVM.TheEntity.ElapsedTime = EditVM.TheEntity.ElapsedTime + (decimal)workingTrack.ElapsedTime;

                ////  string F = StatusOpt.ToString();
                //mLogger.AddLogMessage("Adjusting Status: " + StatusOpt.ToString());
                //if (StatusOpt == StatusOptions.F)
                //{
                //    EditVM.TheEntity.Status = "F";  // mark F for Finished
                //    EditVM.TheEntity.Done = true;
                //    MarkProjectStatus(EditVM.TheEntity.ProjectID ?? 0, "F");
                //}
                ////else if (StatusOpt == StatusOptions.C)
                ////{
                ////    EditVM.TheEntity.Status = "C";  // mark C for Continuing
                ////    EditVM.TheEntity.Done = false;
                ////    MarkProjectStatus(EditVM.TheEntity.ProjectID ?? 0, "C");
                ////}
                //else if (StatusOpt == StatusOptions.I)
                //{
                //    EditVM.TheEntity.Status = "I";  // mark I for interrupted
                //    EditVM.TheEntity.Done = false;
                //    MarkProjectStatus(EditVM.TheEntity.ProjectID ?? 0, "I");
                //}
                //else if (StatusOpt == StatusOptions.W)
                //{
                //    EditVM.TheEntity.Status = "W";  // mark W for partially finished, done for today
                //    EditVM.TheEntity.Done = false;
                //    MarkProjectStatus(EditVM.TheEntity.ProjectID ?? 0, "I");
                //}

                //// EditVM.TheEntity.Status = "C";  // mark F for Finished
                //EditVM.TheEntity.RespPerson = "Johnny";
                //EditVM.TheEntity.DoneDate = StopDate;
                //// EditVM.TheEntity.Done = true;

                //if (inWorkToDo == null)
                //{
                //    mLogger.AddLogMessage("--- Normal Stop Work ----");
                //    // Normal stop work
                //    SwitchDate = DateTime.MinValue;
                //    UpdateDB();
                //    Quit();
                //}
                //else
                //{
                //    mLogger.AddLogMessage("Stopped previous item, Now calling StartWork");
                //    // Stop inWorkToDo then start another todo
                //    //IsInStartTimingMode = true;
                //    SwitchDate = StopDate;
                //    mLogger.AddLogMessage("SwitchDate: " + SwitchDate.ToShortTimeString());
                //    UpdateDB();
                //    StartTiming();
                //}
                ////UpdateDB();
                ////Quit();
                QuitStopWork();
            }
            catch (Exception e)
            {
                // PROBLEM
                mLogger.AddLogMessage(" ========== EXCEPTION in CommitStopWork =================");
                mLogger.AddLogMessage(e.InnerException.Message);
                throw;
            }
        }

        public void MarkProjectStatus(int projectID, string newStatus)
        {
            Project proj = dbBase.Projects.Find(projectID);
            if (proj != null)
            {
                mLogger.AddLogMessage("Reached MarkProjectStatus  --" + proj.Item + "--" + proj.Status + " change to " + newStatus);
                proj.Status = newStatus;
            }
        }

        protected void QuitStopWork()
        {
            mLogger.AddLogMessage("Reached QuitStopWork");
            //base.QuitStopWork();
            EditVM = SelectedToDo;
            Quit();
        }

        protected void Quit()
        {
            //if (EditVM == null || EditVM.TheEntity == null)
            //{
                mLogger.AddLogMessage("Reached ToDos Quit ------------------>");
            if (!EditVM.IsNew)
            {
                EditVM.TheEntity.ClearErrors();
                try
                {
                    //await db.Entry(EditVM.TheEntity).ReloadAsync();
                    dbBase.Entry(EditVM.TheEntity).Reload();
                }
                catch (Exception ex)
                {
                    mLogger.AddLogMessage("DB Problem l 966 : " + ex.Message);
                }
                //   await db.Entry(EditVM.TheEntity).ReloadAsync();
                //db.Entry(EditVM.TheEntity).Reload();
                // Force the datagrid to realise the record has changed
                EditVM.TheEntity = EditVM.TheEntity;
                //          EditVM = currentToDo;
                RaisePropertyChanged("EditVM");
            }
            //}
            mLogger.AddLogMessage("EditVM->" + EditVM.TheEntity.Item);
       //     SelectedToDo = EditVM;
       //     RefreshData();
            //RaisePropertyChanged("SelectedToDo");
         //   inWorkToDo = null;
            //EditVM = null;
            IsInEditMode = false;
            IsInStartTimingMode = false;
            IsInStopWorkMode = false;

            IsInStopStartWorkMode = false;

            //mLogger.AddLogMessage("InWorkToDo.TheEntity.Item = '" + InWorkToDo.TheEntity.Item + "'");
            mLogger.AddLogMessage("EditVM.TheEntity.Item = '" + EditVM.TheEntity.Item + "'");
            mLogger.AddLogMessage("SelectedToDo.TheEntity.Item = '" + SelectedToDo.TheEntity.Item + "'");
        }

        /// protected async override void GetData()  // Loads ToDos and sets empty Tracks
        protected void GetData()  // Loads ToDos and sets empty Tracks
        {
            mLogger.AddLogMessage("Reached GetData in ToDosViewModel ---------->");
            ThrobberVisible = Visibility.Visible;

            //var toDos0 =  (from c in db.ToDos
            //                   orderby c.Status, c.Priority  // c.RevDueDate, c.Status, c.Priority
            //                   select c);

      //      ObservableCollection<ToDoVM> _ToDos = new ObservableCollection<ToDoVM>();
            //var toDos0 = await (from c in db.ToDos
            //                   orderby c.TDTSortOrder, c.Status, c.Priority  // c.RevDueDate, c.Status, c.Priority
            //                   select c).ToListAsync();
            //var toDos1 = await db.ToDos.OrderBy(todo => todo.TDTSortOrder).ToListAsync();
            try
            {
                ObservableCollection<ToDoVM> _ToDos = new ObservableCollection<ToDoVM>();
                var toDos = dbBase.ToDos
                               .AsEnumerable()
                               .OrderBy(todo => todo.TDTSortOrder);
                               //.OrderBy(todo => todo.Status, new CustomSort());
                foreach (ToDo cust in toDos)
                {
                    _ToDos.Add(new ToDoVM { IsNew = false, TheEntity = cust });
                }
                ToDos = _ToDos;
                RaisePropertyChanged("ToDos");
            }
            catch (Exception e)
            {
                string dbProblem = (e.InnerException != null) ? e.InnerException.ToString() : e.Message;
                mLogger.AddLogMessage("############## Database Problem ##########");
                mLogger.AddLogMessage("EXCEPTION->" + dbProblem);
                //  throw e;
                ShowUserMessage("Database Problem - Shutting down, check log.");
                System.Threading.Thread.Sleep(10000);
                App.Current.Shutdown();
            }
            finally
            {
               // VisualHelper(db);
            }
           
            //ToDos = _ToDos;
            //RaisePropertyChanged("ToDos");

            ObservableCollection<TrackVM> _Tracks = new ObservableCollection<Planner.TrackVM>();
            Tracks = _Tracks;
            RaisePropertyChanged("Tracks");

            ThrobberVisible = Visibility.Collapsed;
        }

        public void procDoneChecked(object sender, RoutedEventArgs e, object theSelected)
        {
            //bool Editing = this.is  .ed IsInStopWorkMode = true EditVM
            System.Windows.Controls.CheckBox cb = sender as CheckBox;
            bool? test = cb.IsChecked;
            //var ProjID;
      //x      mLogger.AddLogMessage("DoneChecked :: CheckBox Focus is " + cb.IsFocused + " ***********");
            if (theSelected != null && cb.IsFocused)
            {
                int? ToDoID = ((ToDoVM)theSelected).TheEntity.ToDoID;
                int? ProjID = ((ToDoVM)theSelected).TheEntity.ProjectID;
                string theItem = ((ToDoVM)theSelected).TheEntity.Item;
                string theStatus = ((ToDoVM)theSelected).TheEntity.Status;
                if (theStatus == "A" || theStatus == "F")
                {
                    // Don't allow "quick" finish for Active items or already marked Finish
                    Messenger.Default.Send<UserMessage>(new UserMessage { Message = "ToDo is active or finished, no quick finish" });
            //cc        mLogger.AddLogMessage("DoneChecked not available.");
                    cb.IsChecked = false;
                    return;
                }
                else
                {
                    // Quick Finish for this ToDo
                    Messenger.Default.Send<UserMessage>(new UserMessage { Message = "ToDo quick finish" });
                    dbBase = ((App)Application.Current).db;
                    ToDo t = dbBase.ToDos.Find(ToDoID);
                    Project p = dbBase.Projects.Find(ProjID);
                    p.Status = "F";
                    t.Status = "F";
                    t.Done = true;
                    t.DetailedDesc = t.DetailedDesc + "  Quick Finished! ";
                    t.ElapsedTime = t.ElapsedTime + 5.0M;
                    // Create Track record
                    TrackVM newTrack = new TrackVM();
                    newTrack.TheEntity.Item = t.Item;
                    newTrack.TheEntity.ProjectID = t.ProjectID;
                    newTrack.TheEntity.StartDate = t.StartDate;
                    newTrack.TheEntity.DetailedDesc = "Quick Finished, " + t.DetailedDesc;
                    newTrack.TheEntity.EndDate = DateTime.Now;
                    newTrack.TheEntity.ElapsedTime = 5.0M;
                    newTrack.TheEntity.BillTime = 0.0M;
                    newTrack.TheEntity.BillRef = "tBd";
                    //     newTrack.TheEntity.EndDate = DateTime.Now;
                    newTrack.TheEntity.Expenses = 0.0M;
                    newTrack.TheEntity.Mileage = 0.0M;
                    //  newTrack.TheEntity.Project = t.Project;
                    newTrack.TheEntity.RespPerson = t.RespPerson;
                    newTrack.TheEntity.Status = "F";  // starts out as 'A'
                    // newTrack.TheEntity.SortOrder = "000001";  // t.TDTSortOrder;

                    //Tracks.Add(newTrack);
                    dbBase.Tracks.Add(newTrack.TheEntity);
                    mLogger.AddLogMessage("Created New Track with '" + newTrack.TheEntity.Item.Trim() + "' - " + t.Status);

                    //     ((ToDosViewModel)this.DataContext).MarkProjectStatus(ProjID ?? 0, "F");

                    //((ToDosViewModel)this.DataContext).UpdateDB();
                    //((ToDosViewModel)this.DataContext).RefreshData();
                    UpdateDB();
                    RefreshData();
                }
            }
        }

        public void procDoneUnchecked(object sender, RoutedEventArgs e, object theSelected)
        {
            System.Windows.Controls.CheckBox cb = sender as CheckBox;
            bool? test = cb.IsChecked;
            mLogger.AddLogMessage("DoneUnchecked :: CheckBox Focus is " + cb.IsFocused + " ***********");

            if (theSelected != null && cb.IsFocused)
            {
                int? ToDoID = ((ToDoVM)theSelected).TheEntity.ToDoID;
                int? ProjID = ((ToDoVM)theSelected).TheEntity.ProjectID;
                string theItem = ((ToDoVM)theSelected).TheEntity.Item;
                string theStatus = ((ToDoVM)theSelected).TheEntity.Status;
                if (theStatus != "A" && theStatus == "F")
                {
                    // Make this todo active


                    // Quick Restart for this ToDo
                    Messenger.Default.Send<UserMessage>(new UserMessage { Message = "ToDo quick restart" });
                    dbBase = ((App)Application.Current).db;
                    ToDo t = dbBase.ToDos.Find(ToDoID);
                    Project p = dbBase.Projects.Find(ProjID);
                    p.Status = "I";
                    t.Status = "I";
                    t.Done = false;
                    t.DetailedDesc = t.DetailedDesc + "\n Quick Restart!";

                    //// Create Track record
                    //TrackVM newTrack = new TrackVM();
                    //newTrack.TheEntity.Item = t.Item;
                    //newTrack.TheEntity.ProjectID = t.ProjectID;
                    //newTrack.TheEntity.StartDate = t.StartDate;
                    //newTrack.TheEntity.DetailedDesc = "Quick Finished, " + t.DetailedDesc;
                    //newTrack.TheEntity.EndDate = DateTime.Now;
                    //newTrack.TheEntity.ElapsedTime = 5.0M;
                    //newTrack.TheEntity.BillTime = 0.0M;
                    //newTrack.TheEntity.BillRef = "tBd";
                    ////     newTrack.TheEntity.EndDate = DateTime.Now;
                    //newTrack.TheEntity.Expenses = 0.0M;
                    //newTrack.TheEntity.Mileage = 0.0M;
                    ////  newTrack.TheEntity.Project = t.Project;
                    //newTrack.TheEntity.RespPerson = t.RespPerson;
                    //newTrack.TheEntity.Status = "F";  // starts out as 'A'
                    //// newTrack.TheEntity.SortOrder = "000001";  // t.TDTSortOrder;

                    ////Tracks.Add(newTrack);
                    //dbBase.Tracks.Add(newTrack.TheEntity);
                    //mLogger.AddLogMessage("Created New Track with '" + newTrack.TheEntity.Item.Trim() + "' - " + t.Status);

                    // ((ToDosViewModel)this.DataContext).MarkProjectStatus(ProjID ?? 0, "O");

                    //((ToDosViewModel)this.DataContext).UpdateDB();
                    //((ToDosViewModel)this.DataContext).RefreshData();
                    UpdateDB();
                    RefreshData();

                }
                else
                {
                    // Don't allow "quick" finish for Active items or already marked Finish
                    Messenger.Default.Send<UserMessage>(new UserMessage { Message = "ToDo is active or finished, no quick finish" });
         //x           mLogger.AddLogMessage("DoneChecked not available.");
                    //cb.IsChecked = 
                    return;





                }

            }
        }

        #region Drag and Drop Handlers

        void IDragSource.StartDrag(IDragInfo dragInfo)
        {
            ToDoVM todoVM = (ToDoVM)dragInfo.SourceItem;
            ToDo todo = todoVM.TheEntity;
            if (todo.TDTSortOrder != "000000000")
            {
                // Can't drag first item.
                dragInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
                dragInfo.Data = todoVM;
                mLogger.AddLogMessage("Start Dragging '" + todo.Item + "' -  '" + todo.TDTSortOrder + "'");
            }
        }
        bool IDragSource.CanStartDrag(IDragInfo dragInfo)
        {
            //GongSolutions.Wpf.DragDrop.DefaultDragHandler.
            bool CanDo = GongSolutions.Wpf.DragDrop.DragDrop.DefaultDragHandler.CanStartDrag(dragInfo);
            return CanDo;
        }
        void IDragSource.Dropped(IDropInfo dropInfo)
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDragHandler.Dropped(dropInfo);

            ToDoVM target = dropInfo.TargetItem as ToDoVM;
            ToDo todo = target.TheEntity;
            mLogger.AddLogMessage("Dropping Target: '" + todo.Item + "' - '" + todo.TDTSortOrder + "'");
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

            foreach (ToDoVM item in c)
            {
                ToDo aToDo = ((ToDoVM)item).TheEntity;
                mLogger.AddLogMessage(aToDo.Item + " - " + aToDo.TDTSortOrder);
            }
            bool needSave = dbBase.ChangeTracker.HasChanges();
            //VisualHelper.ListToDosGrid(mLogger, ToDos);
            ListToDosGrid(mLogger, ToDos, dbBase);
            mLogger.AddLogMessage("ToDos ==========");
            foreach (ToDoVM item in ToDos)
            {
                ToDo aToDo = ((ToDoVM)item).TheEntity;
                mLogger.AddLogMessage(aToDo.Item + " - " + aToDo.TDTSortOrder);
            }
            //ToDos
            //RaisePropertyChanged("TheEntity.TDTSortOrder");
            GetData();
            RaisePropertyChanged("ToDos");
        }
        #endregion

        //public void DragOver(IDropInfo dropInfo)
        //{
        //    if (dropInfo.Data is ToDoVM && dropInfo.TargetItem is ToDoVM)
        //    {
        //        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
        //        dropInfo.Effects = DragDropEffects.Move;
        //    }
        //    //throw new NotImplementedException();
        //}

        //public override void Drop(IDropInfo dropInfo)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Drop(IDropInfo dropInfo)
        //{
        //    throw new NotImplementedException();
        //}

        //public void StartDrag(IDragInfo dragInfo)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool CanStartDrag(IDragInfo dragInfo)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Dropped(IDropInfo dropInfo)
        //{
        //    throw new NotImplementedException();
        //}

        //public void DragCancelled()
        //{
        //    throw new NotImplementedException();
        //}

        //private static void ListToDosGrid(LoggerLib.Logger mLogger, IList list)
        static void ListToDosGrid(Logger mLogger, ObservableCollection<ToDoVM> list, TDTDbContext dbBase)
        {
            //CodeFirst.EFcf.TDTDbContext db = new CodeFirst.EFcf.TDTDbContext();
            //     TDTDbContext db = _db;  // (TDTDbContext)App.Current.Resources["theData"];
            var toDos = (from c in dbBase.ToDos

                         select c);
            int rNum = 2;
            foreach (var dgRow in list)
            {
                BuildSqliteCF.Entity.ToDo toDo = ((ToDoVM)dgRow).TheEntity;
                // See also RebuildTDTSortOrder() duplicate code!
                if (toDo.Status == "A")
                {
                    toDo.TDTSortOrder = "000000000";// (rNum + 1000000).ToString().Substring(1, 6);
                }
                else
                {
                    toDo.TDTSortOrder = (rNum + 1000000000).ToString().Substring(1);
                }
                //toDo.
                toDo.RaisePropertyChanged("TheEntity.TDTSortOrder");
          //      ToDo thisOne = dbBase.ToDos.Find(toDo.ToDoID);
          //      thisOne.TDTSortOrder = toDo.TDTSortOrder;
               // RaisePropertyChanged("thisOne");
                mLogger.AddLogMessage(rNum.ToString() + "  " + toDo.ToDoID.ToString() + " - " + toDo.TDTSortOrder);
                //rNum++;
                rNum = rNum + 3;
            }
            // db.
            //RaisePropertyChanged();

            int nChanges = dbBase.SaveChanges();

            //ShowUserMessage("Database Updated with " + nChanges.ToString() + " changes.");
            mLogger.AddLogMessage("ListToDosGrid->UpdateDB successfully completed with " + nChanges.ToString() + " changes.");
        }
    }


    public interface IMenuItem //: RelayCommand // ICommand
    {
        string Header { get; }
        IEnumerable<IMenuItem> Items { get; }
        object Icon { get; }
        bool IsCheckable { get; }
        bool IsChecked { get; set; }
        bool Visible { get; }
        bool IsSeparator { get; }
        string ToolTip { get; }
    }

    #region CustomSort
    public class CustomSort : IComparer<string>
    {
        public static string[] statusOrder = new[] { "A", "I", "C",  "W", "O", "F" };
        public CustomSort()
        { }
        public int Compare(string x, string y)
        //public int Compare(object x1, object y1)
        {
            
            //string x = (string)x1;
            //string y = (string)y1;
            if (x == y)
            {
                return 0;
            }
            else
            {
                if (statusOrder.Any(a => a == x) && statusOrder.Any(a => a == y))
                {
                    if (Array.IndexOf(statusOrder, x) < Array.IndexOf(statusOrder, y))
                        return -1;
                    return 1;
                }
                else if (statusOrder.Any(a => a == x)) // only one item in customordered array (and its x)
                    return -1;
                else if (statusOrder.Any(a => a == y)) // only one item in customordered array (and its y)
                    return 1;
                else
                    return string.Compare(x, y);
            }
        }
    }
    #endregion CustomSort
}