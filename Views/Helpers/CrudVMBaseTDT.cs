using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using LoggerLib;
using System.Windows;
using System.Windows.Media;
////using BuildSqliteCF.DbContexts;

namespace Planner
{
    public class CrudVMBaseTDT : ViewModelBase
    {
        #region Properties

        protected Logger mLogger;
        public TDTDbContext dbBase; // = new TDTDbContext("BillWork.db");
        protected object selectedEntity;
        protected object editEntity;
        protected object trackEntity;

        public CommandVM SaveCmd { get; set; }
        public CommandVM CommitStartTimingCmd { get; set; }
        public CommandVM StopWorkCmd { get; set; }
        public CommandVM QuitCmd { get; set; }
        public CommandVM QuitStartTimingCmd { get; set; }
        public CommandVM CleanUpCmd { get; set; }
        public CommandVM CommitStopWorkCmd { get; set; }
        public CommandVM QuitStopWorkCmd { get; set; }

        private Visibility throbberVisible = Visibility.Visible;
        public Visibility ThrobberVisible
        {
            get { return throbberVisible; }
            set
            {
                throbberVisible = value;
                RaisePropertyChanged();
            }
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                RaisePropertyChanged();
            }
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

        private bool isInStartTimingMode = false;
        public bool IsInStartTimingMode
        {
            get
            {
                return isInStartTimingMode;
            }
            set
            {
                isInStartTimingMode = value;
                InStartTiming inStartTiming = new InStartTiming { Mode = value };
                Messenger.Default.Send<InStartTiming>(inStartTiming);
                RaisePropertyChanged();
            }
        }

        private bool isInStopWorkMode = false;
        public bool IsInStopWorkMode
        {
            get
            {
                return isInStopWorkMode;
            }
            set
            {
                isInStopWorkMode = value;
                InStopWork inStopWork = new InStopWork { Mode = value };
                Messenger.Default.Send<InStopWork>(inStopWork);
                RaisePropertyChanged();
            }
        }
        protected bool isCurrentView = false;
        ////private bool isCurrentView = false;
        ////public bool IsCurrentView
        ////{
        ////    get
        ////    {
        ////        return isCurrentView;
        ////    }
        ////    set
        ////    {
        ////        isCurrentView = value;
        ////        //InStopWork inStopWork = new InStopWork { Mode = value };
        ////        //Messenger.Default.Send<InStopWork>(inStopWork);
        ////        RaisePropertyChanged();
        ////    }
        ////}
        #endregion

        #region Methods
        protected void HandleCommand(CommandMessage action)
        {
            if (isCurrentView)
            {
                switch (action.Command)
                {
                    case CommandType.Insert:
                        InsertNew();
                        break;
                    case CommandType.Edit:
                        if (GotSomethingSelected())
                        {
                            EditCurrent();
                        }
                        break;
                    case CommandType.StartTiming:
                        if (GotSomethingSelected())
                        {
                            StartTiming();
                        }
                        break;
                    case CommandType.CommitStartTiming:
                        if (GotSomethingSelected())
                        {
                            CommitStartTiming();
                        }
                        break;
                    case CommandType.QuitStartTiming:
                        QuitStartTiming();
                        break;
                    case CommandType.StopWork:
                        if (GotSomethingSelected())
                        {
                            StopWork();
                        }
                        break;
                    case CommandType.CommitStopWork:
                        if (GotSomethingSelected())
                        {
                            CommitStopWork();
                        }
                        break;
                    case CommandType.QuitStopWork:
                        QuitStopWork();
                        break;
                    case CommandType.Delete:
                        if (GotSomethingSelected())
                        {
                            DeleteCurrent();
                        }
                        break;
                    case CommandType.Commit:
                        CommitUpdates();
                        break;
                    case CommandType.Refresh:
                        RefreshData();
                        editEntity = null;
                        selectedEntity = null;
                        break;
                    case CommandType.Quit:
                        Quit();
                        break;
                    case CommandType.CleanUp:
                        CleanUp();
                        break;
                    case CommandType.None:
                        break;
                    default:
                        break;
                }
            }
        }

        private bool GotSomethingSelected()
        {
            bool OK = true;
            if (selectedEntity == null)
            {
                OK = false;
                ShowUserMessage("You must select a work item");
            }
            return OK;
        }
        public void ShowUserMessage(string message)
        {
            mLogger.AddLogMessage("--UserMessage: '" + message + "'");
            UserMessage msg = new UserMessage { Message = message };
            Messenger.Default.Send<UserMessage>(msg);
        }
        
        //private void CurrentUserControl(CurrentViewMessage nm)
        private void CurrentUserControl(NavigateMessage nm)
        {
            string curObj = this.GetType().Name;
            if (this.GetType() == nm.ViewModelType)
            {
                mLogger.AddLogMessage("NavigateMessage(CrudVMBaseTDT)  CurrentUserControl - isCurentView TRUE  " + curObj);
                isCurrentView = true;
            }
            else
            {
                mLogger.AddLogMessage("NavigateMessage(CrudVMBaseTDT)  CurrentUserControl - isCurentView FALSE  " + curObj);
                isCurrentView = false;
                //if (db.ChangeTracker.HasChanges())
                //{
                //    int numChanges = db.SaveChanges();
                //    mLogger.AddLogMessage("Leaving Tracks.  Saved " + numChanges);
                //}
                
            }
        }
        #endregion

        #region VirtualMethods
        protected virtual void Quit()
        {
        }
        protected virtual void QuitStartTiming()
        {
        }
        protected virtual void CommitStartTiming()
        {
        }
        protected virtual void InsertNew()
        {
        }
        protected virtual void EditCurrent()
        {
        }
        protected virtual void CommitUpdates()
        {
        }
        protected virtual void DeleteCurrent()
        {
        }
        protected virtual void StartTiming()
        {
        }
        protected virtual void QuitStopTiming()
        {
        }
        protected virtual void CommitStopTiming()
        {
        }
        protected virtual void StopWork()
        {
        }
        protected virtual void QuitStopWork()
        {
        }
        protected virtual void CommitStopWork()
        {
        }
        protected virtual void CleanUp()
        {
        }
        public virtual void RefreshData()
        {
            mLogger.AddLogMessage("Reached RefreshData in CrudVMBaseTDT which will cause Data to be Refreshed!");
            GetData();
            Messenger.Default.Send<UserMessage>(new UserMessage { Message = "Data Refreshed" });
        }
        protected virtual void GetData()
        {
        }
        #endregion

        public CrudVMBaseTDT()
        {
            mLogger = Logger.Instance;
            string theFileName = (string)App.Current.Properties["destFilePath"];
            ///         db = new TDTDbContext(theFileName);
            dbBase = ((App)Application.Current).db;
      ///      dbBase = new TDTDbContext();
            mLogger.AddLogMessage("Constructor CrudVMBaseTDT =========");
            GetData();
            Messenger.Default.Register<CommandMessage>(this, (action) => HandleCommand(action));
            // Listen for Navigation messages to update isCurrentView.
            Messenger.Default.Register<NavigateMessage>(this, (action) => CurrentUserControl(action));
         //   Messenger.Default.Register<CurrentViewMessage>(this, (action) => CurrentUserControl(action));

            SaveCmd = new CommandVM
            {
                CommandDisplay = "Commit",
                IconGeometry = Application.Current.Resources["SaveIcon"] as Geometry,
                Message = new CommandMessage { Command = CommandType.Commit }
            };

            CommitStartTimingCmd = new CommandVM
            {
                CommandDisplay = "Commit Start Timing",
                IconGeometry = Application.Current.Resources["SaveIcon"] as Geometry,
                Message = new CommandMessage { Command = CommandType.CommitStartTiming }
            };

            QuitCmd = new CommandVM
            {
                CommandDisplay = "Quit",
                IconGeometry = Application.Current.Resources["QuitIcon"] as Geometry,
                Message = new CommandMessage { Command = CommandType.Quit }
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

        }

    }
}