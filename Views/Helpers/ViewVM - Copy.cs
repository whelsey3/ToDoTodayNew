using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LoggerLib;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Planner
{
    public class ViewVM
    {
        public Logger mLogger = LoggerLib.Logger.Instance;
        public string ViewDisplay { get; set; }
        public Type ViewType { get; set; }
        public Type ViewModelType { get; set; }
        public UserControl View { get; set; }
      //  public UserControl prevView { get; set; }
        public Window rView { get; set; }    // used for reports
        public RelayCommand Navigate { get; set; }

        // Collection of views that have been built and are available for reuse
        static ObservableCollection<UserControl> builtViews = new ObservableCollection<UserControl>();

        public ViewVM()
        {
            Navigate = new RelayCommand(NavigateExecute);
        }

        public void NavigateExecute()
        {
            // Triggered by Button Command
            var thePrior = App.Current.Properties["priorView"];
            UserControl theNewOne = null;
            if (View == null)
                mLogger.AddLogMessage("Navigate command  NavigateExecute View = null  -  " + ViewType.Name);
            //          Messenger.Default.Unregister()
            else
                mLogger.AddLogMessage("Navigate command  NavigateExecute View = " + View.Name + "  -  " + ViewType.Name);
         
            //    if (View == null && ViewType != null)  // multiple copies available TRP
            if (ViewType != null)
            {
                // Do we already have an instance of ViewType?
                UserControl prevUsed = ChkBuiltStatus(ViewType);
                if (prevUsed == null)
                {
                    // Need a new instance
                    mLogger.AddLogMessage("Activator call for '" + ViewType.Name + "'");
                //  var theView = Activator.CreateInstance(ViewType);
                    theNewOne = (UserControl)Activator.CreateInstance(ViewType);
                    // Add View to instance
                    builtViews.Add(theNewOne);
                    mLogger.AddLogMessage("== No prevUsed found. Activated and Added " + theNewOne.Name);
                }
                else
                {
                    // Prior instance available
                    mLogger.AddLogMessage("--NO Activation needed. Using->" + prevUsed);
                    theNewOne = prevUsed;
                    //  var temp = (ViewType)prevUsed;

                    var oldView = prevUsed;
                    Type oldType = oldView.GetType();
                    mLogger.AddLogMessage("ChangeViewMessage(ShellView)->ShowUserControl - current Holder content: '" + oldType.Name + "'");
                    Type oldType2 = oldView.GetType().BaseType;
                    if (prevUsed != null)
                    {
                        //            var oldView = Holder.Content;
                        ToDosView a = oldView as ToDosView;
                        TracksView b = oldView as TracksView;
                        PlansView c = oldView as PlansView;
                        if (a != null)
                        {
                            mLogger.AddLogMessage("Refreshing old view: ToDosView");
                            ((ToDosViewModel)a.DataContext).RefreshData();
                        }
                        else if (b != null)
                        {
                            mLogger.AddLogMessage("Refreshing old view: TracksView");
                            ((TracksViewModel)b.DataContext).CloseOut();

                            //((TracksViewModel)b.DataContext).RefreshData();
                        }
                        else if (c != null)
                        {
                            mLogger.AddLogMessage("Refreshing old view: PlansView");
                            ((PlansViewModel)c.DataContext).RefreshData();
                        }
                    }
                }
            }
            
            ////var msg2 = new CommandMessage { Command = CommandType.Refresh };
            ////Messenger.Default.Send<CommandMessage>(msg2);

            mLogger.AddLogMessage("SEND Navigate Message");

            // Actually change view
            var msg1 = new ChangeViewMessage { newView = theNewOne, ViewModelType = ViewModelType, ViewType = ViewType };
            Messenger.Default.Send<ChangeViewMessage>(msg1);

            //  Currently used to set isCurrentView in CrudVMBaseTDT
            var msg0 = new NavigateMessage { View = theNewOne, ViewModelType = ViewModelType, ViewType = ViewType };
            Messenger.Default.Send<NavigateMessage>(msg0);

            //  Currently used to set isCurrentView in PlansViewModel
            mLogger.AddLogMessage("^^ Send CurrentViewMessage ^^");
            var msg = new CurrentViewMessage { View = theNewOne, ViewModelType = ViewModelType, ViewType = ViewType };
            Messenger.Default.Send<CurrentViewMessage>(msg);

        }

        private UserControl ChkBuiltStatus(Type curType)
        {
            UserControl returnValue = null;

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

            // False => not previously built
            foreach (var item in builtViews)
            {
            //    Messenger.Default.Unregister<CommandMessage>(item);
            //    Messenger.Default.Unregister<NavigateMessage>(item);
            //    Messenger.Default.Unregister<CurrentViewMessage>(item);
            //    builtViews.Remove(item);
                //item = null;
            //    return null;
                if (item.GetType() == curType)
                {
                    returnValue = item;
                    return returnValue;
                }
            }          
            return returnValue;
        }
    }
}