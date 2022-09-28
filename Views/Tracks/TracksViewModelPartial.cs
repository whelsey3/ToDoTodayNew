//using LoggerLib;
using nuT3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace Planner  //.Views.Tracks
{
    public partial class TracksViewModel : ViewModelBase
    {
        protected Logger mLogger = Logger.Instance;
        public TDTDbContext dbBase; // = new TDTDbContext("BillWork.db");
        protected object trackEntity;  // Acts as flag in GotSomethingSelected

        public CommandVM SaveEditCmd { get; set; }
        public CommandVM QuitEditCmd { get; set; }

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

        #region Methods
        protected void HandleCommand(CommandMessage action)
        {
            if (!(action is TrackCommandMessage))
            {
                return;
            }
            mLogger.AddLogMessage("### Command MSG received by Track " + action.Command);
            switch (action.Command)
            {
                case CommandType.Edit:
                    if (GotSomethingSelected())
                    {
                        EditCurrent();
                    }
                    break;
                case CommandType.Delete:
                    if (GotSomethingSelected())
                    {
                        DeleteCurrent();
                    }
                    break;
                case CommandType.Quit:
                    Quit();
                    break;
                case CommandType.Commit:
                    CommitUpdates();
                    break;
                case CommandType.Refresh:
                    RefreshData();
                    //editEntity = null;
                    //selectedEntity = null;
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
            if (trackEntity == null)
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
            //Messenger.Default.Register<CommandMessage>(this, (action) => HandleCommand(action));
            int n = SelectedTrackIndex;
            mLogger.AddLogMessage("Reached RefreshData in TracksViewModel which will cause Data to be Refreshed!");
            GetData();
            Util.SelectRowByIndex(TheDG, n);
            Messenger.Default.Send<UserMessage>(new UserMessage { Message = "Data Refreshed" });
        }

        public void RefreshDataNew()
        {
            Util.ChkContext(dbBase, "== RefreshData ==");

            //mLogger.AddLogMessage("Reached RefreshData in TracksViewModelPartial which will cause Data to be Refreshed!");
            int Tindex = 0;
            string OldItem = "";
            foreach (var item0 in Tracks)
            {
                if (item0 == SelectedTrack)
                {
                    mLogger.AddLogMessage("Matched SelectedTrack: " + SelectedTrack.TheEntity.Item + "  " + SelectedTrack.TheEntity.TrackID + "  " + Tindex.ToString() + "  " + SelectedTrackIndex);
                    Util.SelectRowByIndex(TheDG, SelectedTrackIndex);
                }
                else
                {
                    mLogger.AddLogMessage("Did not match: " + item0.TheEntity.Item + "  " + item0.TheEntity.TrackID + "  " + Tindex.ToString() + "  " );
                }

                OldItem = item0.TheEntity.Item;
                dbBase.Entry(item0.TheEntity).Reload();
                mLogger.AddLogMessage(OldItem + "- " + item0.TheEntity.Item);
                Tindex++;
            }

        }

        public void CloseOut()
        {
            mLogger.AddLogMessage("*** TracksViewModel CloseOut! ***");

            if (dbBase.ChangeTracker.HasChanges())
            {
                mLogger.AddLogMessage("*** dbBase has Changes in Tracks.CloseOut! ***");
                UpdateDB();
            }
            //   dbBase.Dispose();

            Messenger.Default.Unregister<CommandMessage>(this);
            //Messenger.Default.Unregister<NavigateMessage>(this);
            //Messenger.Default.Unregister<CurrentViewMessage>(this);
            SelectedTrack.IsSelected = true;
            EditTrack = SelectedTrack;
        }

        public void StartUp()
        {
            Messenger.Default.Register<CommandMessage>(this, (action) => HandleCommand(action));
            RefreshData();
            SelectedTrack = EditTrack;
        }

    }
    #endregion

}

