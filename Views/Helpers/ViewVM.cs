using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
//using LoggerLib;
using nuT3;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Planner
{
    public class ViewVM
    {
        public Logger mLogger = Logger.Instance;
        public string ViewDisplay { get; set; }
        public Type ViewType { get; set; }
        public Type ViewModelType { get; set; }
        public UserControl View { get; set; }
        //public UserControl origView { get; set; }
        //public Window rView { get; set; }    // used for reports
        public RelayCommand Navigate { get; set; }

        // Collection of views that have been built and are available for reuse
        static ObservableCollection<UserControl> builtViews = new ObservableCollection<UserControl>();

        public ViewVM()
        {
            Navigate = new RelayCommand(NavigateExecute);
        }

        public void NavigateExecute()
        {
            // Triggered by Button Command,set by collection of ViewVM
            var oldView = App.Current.Properties["priorView"];

            // Handle close out of old view, if there =================
            if (oldView != null)
            {
                RunCloseOut(oldView);
            }
            else
            {
                mLogger.AddLogMessage("CloseOut Not needed.  oldView null");
            }

            UserControl theNewOne = null;
           // var t = thePrior.GetType().Name;
            mLogger.AddLogMessage("Navigate command:  Navigate to ViewType = " + ViewType.Name + " from " + oldView);

            bool alreadyThere = true;  // T=>already built,needs refresh   F=>building new

            // Get desired view--build or get previous build

            //if (View == null && ViewType != null)  // multiple copies available TRP
            if (ViewType != null)
            {
                // Do we already have an instance of ViewType?
                theNewOne = ChkBuiltStatus(ViewType);

                if (theNewOne == null)
                {
                    // Need a new instance. but build seuence important.
                    mLogger.AddLogMessage("Not previously built, Activator call will be needed for '" + ViewType.Name + "'");
                    //  activation automatically refreshes data
                    alreadyThere = false;
                }
                else
                {
                    // Prior instance available
                    alreadyThere = true;
                    mLogger.AddLogMessage("--NO Activation needed. Using->" + theNewOne);
                }


                //  Close out old one (if there) 
                //     before activation or refresh of the new one

                //var oldView = thePrior;  // origView; // passed in View 



                if (alreadyThere)
                {
                    // Handle refresh of existing theNewOne ===================
                    mLogger.AddLogMessage("--NO Activation needed. Using->" + theNewOne);

                    //  Need refresh data for theNewOne
                    Type oldType = theNewOne.GetType();
                    ToDosView a = theNewOne as ToDosView;
                    TracksView b = theNewOne as TracksView;
                    PlansView c = theNewOne as PlansView;

                    if (a != null)
                    {
                        mLogger.AddLogMessage("Refreshing previously used view: ToDosView");
                        //((ToDosViewModel) a.DataContext).RefreshData();
                        //// (((oldType)theNewOne).DataContext).RefreshData();
                        ToDosViewModel tvm = ((ToDosViewModel)a.DataContext);
                  //      Messenger.Default.Register<CommandMessage>(tvm, (action) => HandleCommand(action));
                        tvm.StartUp();
                    }
                    else if (b != null)
                    {
                        mLogger.AddLogMessage("Refreshing new view: TracksView");
                        //((TracksViewModel)b.DataContext).CloseOut();
                        ((TracksViewModel) b.DataContext).StartUp();
                    }
                    else if (c != null)
                    {
                        mLogger.AddLogMessage("Refreshing new view: PlansView");
                        ((PlansViewModel) c.DataContext).StartUp();
                    }
                }
                else
                {
                    //  Need to activate the new one  // Need a new instance
                    mLogger.AddLogMessage("Activator call for '" + ViewType.Name + "'");
                    //  activation automatically refreshes data
                    theNewOne = (UserControl)Activator.CreateInstance(ViewType);
                    View = theNewOne;
                    // Add View to collection of prior views
                    builtViews.Add(theNewOne);
                    mLogger.AddLogMessage("== No previous Used found. Activated and Added " + theNewOne.Name);
                }

                mLogger.AddLogMessage("SEND ChangeViewMessage from NavigateExecute");

                // Actually change view
                var msg1 = new ChangeViewMessage { newView = theNewOne, ViewModelType = ViewModelType, ViewType = ViewType };
                Messenger.Default.Send<ChangeViewMessage>(msg1);

                //  Currently used to set isCurrentView in CrudVMBaseTDT
                //var msg0 = new NavigateMessage { View = theNewOne, ViewModelType = ViewModelType, ViewType = ViewType };
                //Messenger.Default.Send<NavigateMessage>(msg0);

                ////  Currently used to set isCurrentView in PlansViewModel
                //mLogger.AddLogMessage("^^ Send CurrentViewMessage, finished NavigateExecute  ^^");
                //var msg = new CurrentViewMessage { View = theNewOne, ViewModelType = ViewModelType, ViewType = ViewType };
                //Messenger.Default.Send<CurrentViewMessage>(msg);
            }
            else
            {
                // ViewType was null
                mLogger.AddLogMessage("PROBLEM -- ViewType was null in NavigateExecute");
            }
        }

        private void RunCloseOut<T> (T oldView)
        {
            Type oldType = oldView.GetType();
            //            var oldView = Holder.Content;
            ToDosView a = oldView as ToDosView;
            TracksView b = oldView as TracksView;
            PlansView c = oldView as PlansView;
            mLogger.AddLogMessage("CloseOut needed of old view: " + oldView.GetType());

            //  ((T)oldView.DataContext).CloseOut();

            if (a != null)
            {
                mLogger.AddLogMessage("CloseOut needed of old view: ToDosView");
                ToDosViewModel tvm = ((ToDosViewModel)a.DataContext);
                tvm.CloseOut();
                //   ((ToDosViewModel)a.DataContext).CloseOut();
                // (((oldType)oldView).DataContext).RefreshData();
            }
            else if (b != null)
            {
                mLogger.AddLogMessage("CloseOut needed of old view: TracksView");
                ((TracksViewModel)b.DataContext).CloseOut();
            }
            else if (c != null)
            {
                mLogger.AddLogMessage("CloseOut needed of old view: PlansView");
                ((PlansViewModel)c.DataContext).CloseOut();
            }
        }

        private UserControl ChkBuiltStatus(Type curType)
        {
            UserControl prevBuiltView = null;

            //// ============================
            //TDTDbContext db = ((App)Application.Current).db;
            /////TDTDbContext db = new TDTDbContext();
            //db.ChangeTracker.DetectChanges();
            //bool PendingChanges = db.ChangeTracker.HasChanges();
            ////db.ChangeTracker.
            //if (PendingChanges)
            //{
            //    mLogger.AddLogMessage("Pending Changes not made! ==" + curType + "==");
            //    string changeType = "";
            //    int historyCount = 0;
            //    foreach (
            //        var history in db.ChangeTracker.Entries()
            //              .Where(e => e.Entity is IModificationHistory && (e.State == EntityState.Added ||
            //              e.State == EntityState.Modified))
            //              .Select(e => e.Entity as IModificationHistory)
            //      )
            //    {
            //        historyCount++;
            //        //history.DateModified = DateTime.Now;
            //        //if (history.DateCreated == DateTime.MinValue)
            //        //{
            //        //    history.DateCreated = DateTime.Now;
            //        //}
            //        changeType = history.GetType().ToString();
            //        if (changeType.Contains("Project"))
            //        {
            //            mLogger.AddLogMessage("ViewVM UpdateDB -" + historyCount + "  Project Changed- '" + ((Project)history).Item + "' -" + changeType);
            //            mLogger.AddLogMessage("ViewVM UpdateDB -" + historyCount + "  Project.FolderID was- '" + ((Project)history).FolderID + "' -" + changeType);
            //        }
            //        else if (changeType.Contains("ToDo"))
            //        {
            //            mLogger.AddLogMessage("ViewVM UpdateDB -" + historyCount + "  ToDo Changed- '" + ((ToDo)history).Item + "' -" + changeType);
            //        }
            //        else
            //        {
            //            mLogger.AddLogMessage("ViewVM UpdateDB -" + historyCount + "  Track Changed- '" + ((Track)history).Item + "' -" + changeType);
            //        }
            //    }
            //    //db.su.
            //    //      int nSaved = db.SaveChanges();
            //    //mLogger.AddLogMessage("!!!! Saved Pending Changes.  Count was " + nSaved);
            //    // ============================


            //}
            //else
            //{
            //    mLogger.AddLogMessage("NO Pending Changes! --" + curType + "--");
            //}

            foreach (var item in builtViews)
            {
                if (item.GetType() == curType)
                {
                    prevBuiltView = item;
                    return prevBuiltView;
                }
            }          
            return prevBuiltView;
        }
    }
}