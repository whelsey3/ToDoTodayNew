using BuildSqliteCF.Entity;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using nuT3;
//LoggerLib;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Planner
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public partial class TracksViewModel : ViewModelBase
    {
        #region Properties

        public ObservableCollection<CommandVM> Commands { get; set; }
        public ObservableCollection<ViewVM> Views { get; set; }
        public ObservableCollection<ReportVM> Reports { get; set; }

        private TrackVM editTrack;
        public TrackVM EditTrack
        {
            get
            {
                return editTrack;
            }
            set
            {
                editTrack = value;
                //editEntity = editTrack.TheEntity;
                RaisePropertyChanged();
            }
        }

        //private TrackVM currentTrack;
        // Current selection in data grid
        private TrackVM selectedTrack;
        public TrackVM SelectedTrack
        {
            get
            {
                return selectedTrack;
            }
            set
            {
                selectedTrack = value;
                trackEntity = value;    // acts as flag in GotSomethingSelected
            }
        }

        private int selectedTrackIndex;
        public int SelectedTrackIndex
        {
            get
            {
                return selectedTrackIndex;
            }
            set
            {
                selectedTrackIndex = value;
                //trackEntity = value;    // acts as flag in GotSomethingSelected
            }
        }
        //  Headers for editing screen
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

        public ObservableCollection<TrackVM> Tracks { get; set; }

        #endregion
        
        public DataGrid TheDG;

        #region Constructor

        public TracksViewModel()
        {
            mLogger = Logger.Instance;
            string theFileName = (string)App.Current.Properties["destFilePath"];
            ///         db = new TDTDbContext(theFileName);
            dbBase = ((App)Application.Current).db;
            ///      dbBase = new TDTDbContext();
            mLogger.AddLogMessage("Constructor TracksViewModel =========");
            GetData();

            Messenger.Default.Register<CommandMessage>(this, (action) => HandleCommand(action));
            // Listen for Navigation messages to update isCurrentView.
            //Messenger.Default.Register<NavigateMessage>(this, (action) => CurrentUserControl(action));
            //   Messenger.Default.Register<CurrentViewMessage>(this, (action) => CurrentUserControl(action));
            
            // Commands bound to UI actions
            SaveEditCmd = new CommandVM
            {
                CommandDisplay = "Commit",
                IconGeometry = Application.Current.Resources["SaveIcon"] as Geometry,
                Message = new TrackCommandMessage { Command = CommandType.Commit }
            };

            QuitEditCmd = new CommandVM
            {
                CommandDisplay = "Quit",
                IconGeometry = Application.Current.Resources["QuitIcon"] as Geometry,
                Message = new TrackCommandMessage { Command = CommandType.Quit }
            };

            // Views bound to buttons for changing view.  
            ObservableCollection<ViewVM> views = new ObservableCollection<ViewVM>
            {
                new ViewVM{ ViewDisplay="ToDos", ViewType = typeof(ToDosView), ViewModelType = typeof(ToDosViewModel)},
                new ViewVM{ ViewDisplay="Planning", ViewType = typeof(PlansView), ViewModelType = typeof(PlansViewModel)}
            };
            Views = views;

            RaisePropertyChanged("Views");

            ObservableCollection<CommandVM> commands = new ObservableCollection<CommandVM>
            {
                new CommandVM{ CommandDisplay="Edit",        IconGeometry=Application.Current.Resources["EditIcon"]             as Geometry , Message=new TrackCommandMessage{ Command = CommandType.Edit} },
                new CommandVM{ CommandDisplay="Delete Work Item",      IconGeometry=Application.Current.Resources["DeleteIcon"] as Geometry , Message=new TrackCommandMessage{ Command = CommandType.Delete} },
                new CommandVM{ CommandDisplay="Refresh Data",     IconGeometry=Application.Current.Resources["RefreshIcon"]     as Geometry , Message=new TrackCommandMessage{ Command = CommandType.Refresh} },
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
            mLogger.AddLogMessage("--- TracksViewModel end constructor ---");

        }
        #endregion

        #region Commands
        //protected override void RefreshData()
        //{
        //    currentTrack = SelectedTrack;
        //    base.RefreshData();
        //    EditTrack = currentTrack;
        //    SelectedTrack = EditTrack;
        //    SelectedTrack.TheEntity = SelectedTrack.TheEntity;
        //    RaisePropertyChanged("Tracks");
        //}

        //protected override void EditCurrent()
        protected void EditCurrent()
        {
            //int theIndex = this.
            EditTrack = SelectedTrack;
            mLogger.AddLogMessage("Reached Track EditCurrent with '" + EditTrack.TheEntity.Item.Trim() + "' - " + EditTrack.TheEntity.Status + "------------------>");
            TitlePopUp = "Editing Track '" + EditTrack.TheEntity.Item.Trim() + "' ?";
            IsInEditMode = true;
        }

        protected void CommitUpdates()
        {
            //var x = EditTrack.TheEntity.TheEntity;
            ShowUserMessage("WORKING! in Tracks CommitUpdates");
            if (EditTrack == null || EditTrack.TheEntity == null)
            {
                mLogger.AddLogMessage("Tracks CommitUpdates  EditTrack is null!!");
                if (dbBase.ChangeTracker.HasChanges())
                {
                    UpdateDB();
                }
                return;
            }

            mLogger.AddLogMessage("Reached Tracks CommitUpdates with " + EditTrack.TheEntity.Item.Trim() + " - " + EditTrack.TheEntity.Status + "------------------>");
            if (EditTrack.TheEntity.IsValid())
            {
                if (EditTrack.IsNew)
                {
                    EditTrack.IsNew = false;
                    Tracks.Add(EditTrack);
                    dbBase.Tracks.Add(EditTrack.TheEntity);
                    UpdateDB();
             //       Quit();
                    SelectedTrack = EditTrack;
                }
                else if (dbBase.ChangeTracker.HasChanges())
                {
                    int n = SelectedTrackIndex;
                    UpdateDB();
                    SelectedTrack = EditTrack;
                    //       Quit();  // IsInEditMode = false;
                    Util.SelectRowByIndex(TheDG, n);
                }
                else
                {
                    ShowUserMessage("No changes to save");
                }
                Quit();
            }
            else
            {
                ShowUserMessage("There are problems with the data entered");
                mLogger.AddLogMessage("There are problems commiting the data entered!!!");
            }
        }

        private void UpdateDB()   // Update the Database
        {
            mLogger.AddLogMessage("==== UpdateDB ==== TracksViewModel =============>");
#if DEBUG
            Util.ChkContext(dbBase, "== TracksViewModel.UpdateDB == l 225");
#endif
            try
            {
                int nChanges = dbBase.SaveChanges();
                ShowUserMessage("Database Updated with " + nChanges.ToString() + " changes.");
                mLogger.AddLogMessage("UpdateDB successfully completed. with " + nChanges.ToString() + " changes.");
            }
            catch (DbEntityValidationException dbEx)
            {
                mLogger.AddLogMessage("DbEntityValidationException: =====");
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        mLogger.AddLogMessage("Tracks.  Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
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
                mLogger.AddLogMessage("DbUpdateException: " + deDbEx.InnerException.Message);
            }
            catch (Exception e)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    ErrorMessage = e.InnerException.GetBaseException().ToString();
                }
                mLogger.AddLogMessage("Exception: " + e.InnerException.GetBaseException().ToString());
                ShowUserMessage("There was a problem updating the database");
            }
            //ReFocusRow();
        }

        protected void DeleteCurrent() // Delete current selection
        {
            int n = SelectedTrackIndex;
            EditTrack = SelectedTrack;
            if (Util.CheckDeleteOK("Are you sure you want to delete the current Track, '" + EditTrack.TheEntity.Item.Trim() + "'?"))
            {
                mLogger.AddLogMessage("Confirmed DeleteCurrent Tracks with " + EditTrack.TheEntity.Item.Trim() + " - " + EditTrack.TheEntity.Status + "------------------>");
             
                dbBase.Tracks.Remove(SelectedTrack.TheEntity);
                Tracks.Remove(SelectedTrack);
                RaisePropertyChanged("Tracks");

                UpdateDB();    //     CommitUpdates();

                // Delete occurred adjust focus to previous value (n -1)
                n = n >= 1 ? n - 1 : 0;
            }

            // Set focus to n
            Util.SelectRowByIndex(TheDG, n);
        }

        protected void Quit()
        {
            mLogger.AddLogMessage("Tracks Quit reached" + "------------------>");
            if (!EditTrack.IsNew)
            {
                /// ReFocusRow();
                //  Not a new one (Track)
                EditTrack.TheEntity.ClearErrors();
                mLogger.AddLogMessage("Before Reload->" + EditTrack.TheEntity.Item);
                try
                {
                    //await db.Entry(EditTrack.TheEntity).ReloadAsync();
                    dbBase.Entry(EditTrack.TheEntity).Reload();
                }
                catch (Exception ex)
                {
                    mLogger.AddLogMessage("EXCEPTION:  DB Problem l 327 : " + ex.Message);
                }
                mLogger.AddLogMessage("After Reload->" + EditTrack.TheEntity.Item);
                // Force the datagrid to realise the record has changed
                EditTrack.TheEntity = EditTrack.TheEntity;
                //EditTrack = currentTrack;
                RaisePropertyChanged("EditTrack");
            }
            SelectedTrack = EditTrack;
            Util.SelectRowByIndex(TheDG, SelectedTrackIndex);
            mLogger.AddLogMessage("SelectedTrack->" + SelectedTrack.TheEntity.Item);
            //          RefreshData();
            IsInEditMode = false;
            //IsInStartTimingMode = false;
            //IsInStopWorkMode = false;
        }
        #endregion

        #region Data
       // protected async override void GetData()  // Loads ToDos and sets empty Tracks
        protected void GetData()  // Loads Tracks
        {
            mLogger.AddLogMessage("Starting GetData in TracksViewModel ---------------------->");
            ThrobberVisible = Visibility.Visible;
            ObservableCollection<TrackVM> _Tracks = new ObservableCollection<TrackVM>();
         //   dbBase.Tracks.
            var tracks =   (from c in dbBase.Tracks
                                orderby c.StartDate descending
                                select c).AsEnumerable();
            int n = 0;
            foreach (Track tempTrack in tracks)
            {
                mLogger.AddLogMessage("Getting data: " + n + " - " + tempTrack.Item + " - " + tempTrack.TrackID);
                _Tracks.Add(new TrackVM { IsNew = false, TheEntity = tempTrack });
                n++;
            }
            Tracks = _Tracks;

            ////if (Tracks.Count != 0)
            ////{
            ////    SelectedTrack = Tracks[0];
            ////}
            if (EditTrack != null)
            {
                SelectedTrack = EditTrack;
                //ReFocusTheRow(EditTrack);
            }
            
            RaisePropertyChanged("Tracks");
            ThrobberVisible = Visibility.Collapsed;

            //mLogger.AddLogMessage("SelectedTrack->" + SelectedTrack.TheEntity.Item);
            //mLogger.AddLogMessage("selectedEntity->" + ((TrackVM)selectedEntity).TheEntity.Item);
            mLogger.AddLogMessage("Ending GetData in TracksViewModel" + "------------------>");
        }
        #endregion

        // https://social.msdn.microsoft.com/Forums/en-US/e339024a-08ea-44fc-8f05-2bdab8f5f22b/how-to-programmatically-select-and-focus-a-row-in-datagrid
        //  https://blog.magnusmontin.net/2013/11/08/how-to-programmatically-select-and-focus-a-row-or-cell-in-a-datagrid-in-wpf/
        // https://social.msdn.microsoft.com/Forums/vstudio/en-US/cc5976d9-f117-4acb-9943-b7f16fa79d0c/in-wpf-mvvm-how-to-set-the-rows-selected-from-the-view-model

        protected async void ReFocusRow(bool withReload = true)
        {
            int id = EditTrack.TheEntity.TrackID;
            SelectedTrack = null;
            await dbBase.Entry(EditTrack.TheEntity).ReloadAsync();
            await Application.Current.Dispatcher.InvokeAsync(new Action(() =>
            {
                SelectedTrack = Tracks.Where(e => e.TheEntity.TrackID == id).FirstOrDefault();
                SelectedTrack.TheEntity = SelectedTrack.TheEntity;
                SelectedTrack.TheEntity.ClearErrors();
            }), DispatcherPriority.ContextIdle);
            IsInEditMode = false;
        }

        protected async void ReFocusTheRow(TrackVM theTrack)
        {
            int id = EditTrack.TheEntity.TrackID;
            SelectedTrack = null;
           // await dbBase.Entry(theTrack.TheEntity).ReloadAsync();
            await Application.Current.Dispatcher.InvokeAsync(new Action(() =>
            {
                //SelectedTrack = Tracks.Where(e => e.TheEntity.TrackID == id).FirstOrDefault();
                SelectedTrack = theTrack;
                SelectedTrack.TheEntity = SelectedTrack.TheEntity;
                SelectedTrack.TheEntity.ClearErrors();
            }), DispatcherPriority.ContextIdle);
            IsInEditMode = false;
        }
    }
}