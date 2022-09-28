using BuildSqliteCF.Entity;
using BuildSqliteCF;
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
using nuT3;
//using LoggerLib;
using Planner;
using System.Data;
using System.Data.Entity;

namespace Planner
{

    public partial class PlansViewModel : ViewModelBase
    {

        public int CheckChanges(TDTDbContext _db, string CallerMsg = "TBD")
        {
            Logger mLogger = Logger.Instance;
            int historyCount = 0;
            // ============================
            TDTDbContext db = _db;   // ((App)Application.Current).db;
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
                    history.DateModified = DateTime.Now;
                    if (history.DateCreated == DateTime.MinValue)
                    {
                        history.DateCreated = DateTime.Now;
                    }
                    changeType = history.GetType().ToString();
                    if (changeType.Contains("Project"))
                    {
                        mLogger.AddLogMessage("CheckChanges -" + historyCount + "  Project Changed- '" + ((Project)history).Item + "' -" + changeType);
                        mLogger.AddLogMessage("CheckChanges -" + historyCount + "  Project.FolderID was- '" + ((Project)history).FolderID + "' - " + ((Project)history).ProjectID);
                    }
                    else if (changeType.Contains("ToDo"))
                    {
                        mLogger.AddLogMessage("CheckChanges -" + historyCount + "  ToDo Changed- '" + ((ToDo)history).Item + "' -" + changeType);
                        mLogger.AddLogMessage("CheckChanges -" + historyCount + "  ToDo.ToDoID was- '" + ((ToDo)history).ToDoID + "' - " + ((ToDo)history).ProjectID);
                    }
                    else
                    {
                        mLogger.AddLogMessage("CheckChanges -" + historyCount + "  Track Changed- '" + ((Track)history).Item + "' -" + changeType);
                        mLogger.AddLogMessage("CheckChanges -" + historyCount + "  Track.TrackID was- '" + ((Track)history).TrackID + "' - " + ((Track)history).ProjectID);
                    }
                }
                //db.su.
                //      int nSaved = db.SaveChanges();
                //mLogger.AddLogMessage("!!!! Saved Pending Changes.  Count was " + nSaved);
                mLogger.AddLogMessage(" ============================");
            }
            else
            {
                mLogger.AddLogMessage("NO Pending Changes! --" + curType + "--");
            }
            return historyCount;
        }

        public ObservableCollection<dbFolderViewModel> FixGetData()
        {
            ////Folder[] folders = GetFolders();

           // public BuildSqliteCF.Entity.Folder[] GetFolders()
           // {
                //Folder[] theFolders0 = (from c in db.Folders
                //                        orderby c.FSortOrder
                //                        select c).ToArray();

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
            //    return fs; //  theFolders.ToArray<Folder>;

            //}
            bool LazyLoading = false;

            var theFVM = (from folder in fs //folders
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
    }
}