using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
//using LoggerLib;
using nuT3;
using System.Windows;
using GongSolutions.Wpf.DragDrop;
using System.Windows.Controls;

namespace Planner   //.Views.ToDos
{
    public partial class ToDosViewModel : ViewModelBase, IDragSource, IDropTarget
    {
        protected Logger mLogger = Logger.Instance;
        public TDTDbContext dbBase; // = new TDTDbContext("BillWork.db");

        protected object selectedEntity;
        protected object editEntity;

        public CommandVM SaveEditCmd { get; set; }
        public CommandVM CommitStartTimingCmd { get; set; }
        public CommandVM StopWorkCmd { get; set; }
        public CommandVM QuitEditCmd { get; set; }
        public CommandVM QuitStartTimingCmd { get; set; }
        public CommandVM CleanUpCmd { get; set; }
        public CommandVM CommitStopWorkCmd { get; set; }
        public CommandVM QuitStopWorkCmd { get; set; }
        public CommandVM CommitStopStartWorkCmd { get; set; }
        public CommandVM QuitStopStartWorkCmd { get; set; }

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


        private bool isInStopStartWorkMode = false;
        public bool IsInStopStartWorkMode
        {
            get
            {
                return isInStopStartWorkMode;
            }
            set
            {
                isInStopStartWorkMode = value;
                InStopStartWork inStopStartWork = new InStopStartWork { Mode = value };
                Messenger.Default.Send<InStopStartWork>(inStopStartWork);
                RaisePropertyChanged();
            }
        }

        //protected bool isCurrentView = false;
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


        public void HandleCommand(CommandMessage action)
        {
            if (action is TrackCommandMessage)
            {
                return;
            }
            //if (isCurrentView)
            //{
            mLogger.AddLogMessage("### Command MSG received by ToDos: " + action.Command);
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
                        StartTimingButton();
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

                case CommandType.CommitEdit:
                    //TitlePopUp = "Adding new ToDo Item";
                    if (TitlePopUp == "Adding new ToDo Item")
                        CommitEdit();
                    else if (GotSomethingSelected())
                    {
                        CommitEdit();
                    }
                    break;

                case CommandType.QuitEdit:
                    QuitEdit();
                    break;

                case CommandType.CommitStopStartWork:
                    if (GotSomethingSelected())
                    {
                        CommitStopStartWork();
                    }
                    break;
                case CommandType.QuitStopStartWork:
                    QuitStopStartWork();
                    break;

                case CommandType.Delete:
                    if (GotSomethingSelected())
                    {
                       // DeleteCurrent();
                        DeleteSelectedToDos();
                    }
                    break;
                case CommandType.Commit:
                    CommitUpdates();
                    break;
                case CommandType.Refresh:
                    RefreshData();
                    //editEntity = null;
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
            //}
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

        public void RefreshData()
        {
            // Messenger.Default.Register+<CommandMessage>(this, (action) => HandleCommand(action));
            int n = SelectedToDoIndex;
            mLogger.AddLogMessage("Reached RefreshData in ToDosViewModel which will cause Data to be Refreshed!");
            GetData();
            Messenger.Default.Send<UserMessage>(new UserMessage { Message = "Data Refreshed" });
            RebuildTDTSortOrder();
            n = (n <= -1 ? 0 : n);
            Util.SelectRowByIndex(TheDGToDos, n);
        }

        public void CloseOut()
        {
            mLogger.AddLogMessage("*** ToDosViewModel CloseOut! ***");

            if (dbBase.ChangeTracker.HasChanges())
            {
                mLogger.AddLogMessage("*** dbBase has Changes in ToDos.CloseOut! ***");
                UpdateDB();
            }
            //   dbBase.Dispose();

            Messenger.Default.Unregister<CommandMessage>(this);
            //Messenger.Default.Unregister<NavigateMessage>(this);
            //Messenger.Default.Unregister<CurrentViewMessage>(this);
        }

        public void StartUp()
        {
            Messenger.Default.Register<CommandMessage>(this, (action) => HandleCommand(action));
            RefreshData();
        }

        //public void DoneChecked(object sender, RoutedEventArgs e)
        //{
        //    System.Windows.Controls.CheckBox cb = sender as CheckBox;
        //    bool? test = cb.IsChecked;
        //}


        //public void DoneUnchecked(object sender, RoutedEventArgs e)
        //{
        //    System.Windows.Controls.CheckBox cb = sender as CheckBox;
        //    bool? test = cb.IsChecked;
        //}

    }

}
