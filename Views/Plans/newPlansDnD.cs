using BuildSqliteCF.Entity;
using GalaSoft.MvvmLight;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Linq;
using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;
using System.Windows;

namespace Planner
{
    public partial class PlansViewModel : ViewModelBase //, IDropTarget, IDragSource // CrudVMBaseTDT
    {
        public static void New2HandleDropping(TreeViewItemViewModel source, TreeViewItemViewModel target, string drpAction)
        {
            // Set up data connections and logging
            TDTDbContext db = ((App)Application.Current).db;

            LoggerLib.Logger mLogger = LoggerLib.Logger.Instance;
            mLogger.AddLogMessage("------ Dropping ====== New2HandleDropping ======================");

            System.Collections.Generic.List<Project> origSet = new List<Project>();
            origSet = (from c in db.Projects
                       orderby c.FolderID, c.PPartNum
                       select c).ToList();
            LogArray(origSet, "origSet");

            if (target.DbID.Contains("F"))
            {
                // Dropping on Folder

            }

            // Set up common variables
            System.Collections.Generic.List<Project> renumList = new List<Project>();

            // Code works with PSortOrder then updates any changes to PPartNum

            // Retrieve source from database, including any children
            Project sourceProj = db.Projects.Find(Int32.Parse(source.DbID));
            string sType = GetCC(sourceProj.PSortOrder);
            //string sBatch = sType.Substring(0, sType.Length - 3);
            string sB = "PPPTTTSSS".Substring(0, sType.Length) + " == " + sType;
            mLogger.AddLogMessage("sourceChildBatch: " + sB);

            //  Used in both cases: dropping on folder or project item
            System.Collections.Generic.List<Project> sourceChildBatch = new List<Project>();
            sourceChildBatch = (from c in db.Projects
                                where (c.PSortOrder.Substring(0, sType.Length) == sType
                                       &&
                                       c.FolderID == sourceProj.FolderID
                                       &&
                                      c.ProjectID != sourceProj.ProjectID) // exclude chance of Source in Batch
                                orderby c.FolderID, c.PSortOrder
                                select c).ToList();
            LogArray(sourceChildBatch, "sourceChildBatch");

            //   Idea is to process renumList, using first value as starting point 
            //      and then update subsequent PSortOrder entries with "next" value

            int? targetFolderID = 0;

            if (target.DbID.Contains("F"))
            {
                //  Dropping on a folder, Get target (folder)
                //  string tFolder0 = t.Substring(1);
                Folder targetFolder = db.Folders.Find(Int32.Parse(target.DbID.Substring(1)));
                targetFolderID = targetFolder.FolderID; // Int32.Parse(tFolder0);

                //  Get last project in folder and then check if Folder is empty
                var folderProjs = (from c in db.Projects
                                   where (c.FolderID == targetFolderID
                                          && c.PPartNum.Substring(3, 6) == "000000"
                                       )
                                   orderby c.PPartNum descending
                                   select c).FirstOrDefault();

                sourceProj.FolderID = targetFolderID;

                if (folderProjs == null)
                {
                    // No projs currently in the folder 
                    //  Source should be first followed by SourceChildren.  No Target.
                    mLogger.AddLogMessage("Dropping on Empty Folder.");
                    // Make sure FolderID of first item reflects targetProj
                    //sourceProj.FolderID = targetFolderID;
                    sourceProj.PSortOrder = "001000000"; // default to first PPartNum
                }
                else
                {
                    // Folder has an existing project(s), the last one will be first in renumList
                    mLogger.AddLogMessage("Folder last proj=" + folderProjs.Item + "-" + folderProjs.PSortOrder);
                    renumList.Add(folderProjs);
                    targetFolderID = folderProjs.FolderID;
                }

                renumList.Add(sourceProj);
                // Add any children of sourceProj
                foreach (var prj in sourceChildBatch)
                {
                    renumList.Add(prj);
                }

            }
            else
            {
                // Dropping on another project item (not a folder) +++++++++++++++++++++++++++++
                // Normal case, NOT dropping on Folder

                var s = source.DbID;
                var t = target.DbID;

                Project targetProj = db.Projects.Find(Int32.Parse(t));
                targetFolderID = targetProj.FolderID;
                string tType = GetCC(targetProj.PSortOrder);     // Remove any trailing blocks of '000'
                string tBatch = tType.Substring(0, tType.Length - 3);  // Move back one block 0, 3, 6

                string sBatch = sType.Substring(0, sType.Length - 3);

                mLogger.AddLogMessage("Source: " + s + ": " + sourceProj.FolderID + " - " + sourceProj.PSortOrder + "  sType: " + sType + "  sBatch: ->" + sBatch + "<-  " + " " + sourceProj.Item + " ");
                mLogger.AddLogMessage("      drpAction: " + drpAction);
                mLogger.AddLogMessage("Target: " + t + ": " + targetProj.FolderID + " - " + targetProj.PSortOrder + "  tType: " + tType + "  tBatch: ->" + tBatch + "<-  " + " " + targetProj.Item + " ");

                int sFolder = sourceProj.FolderID ?? 0;
                int tFolder = targetProj.FolderID ?? 0;

                // Build Batch for processing to update numbers (PSortOrder)
                //   1) All items in Target that may need updating
                //         Exclude any source items
                //   2) Any children of Source that will need updating
                //   Three options:
                //       AFTER       ON         BEFORE
                //       Target      Source     Source
                //       Source      Target     Target
                //
                //             Batch
                // 
                // --------------------------------------------------------------------------

                // All like tBatch, exclude possible source
                int test = Convert.ToInt32(tType);

                string tB = "PPPTTTSSS".Substring(0, tType.Length) + " > " + test;
                mLogger.AddLogMessage("targetBatch: " + tB);

                // --------------------------------------------------------------------------
                //string tBatch = tType.Substring(0, tType.Length - 3);  // Move back one block 0, 3, 6
                mLogger.AddLogMessage("tBatch->" + tBatch);
                mLogger.AddLogMessage("sType->" + sType);
                mLogger.AddLogMessage("tType->" + tType);

                System.Collections.Generic.List<Project> targetBatch = new List<Project>();
                targetBatch = (from d in db.Projects.AsEnumerable()
                               where
                               (d.FolderID == targetProj.FolderID)
                               &&
                               d.PSortOrder.Substring(0, tType.Length - 3) == tBatch
                               &&
                               ChkPSortOrder(d.PSortOrder, tType) == true
                               &&
                               d.PSortOrder.Substring(0, sType.Length) != sType
                               orderby d.FolderID, d.PSortOrder
                               select d).ToList();

                LogArray(targetBatch, "Target Batch");

                // --------------------------------------------------------------------------
                System.Collections.Generic.List<Project> targetChildBatch = new List<Project>();
                // All like tBatch, exclude possible source

                string tCB = "PPPTTTSSS".Substring(0, tType.Length) + " == " + tType;
                mLogger.AddLogMessage("targetChildBatch: " + tCB);

                targetChildBatch = (from c in db.Projects
                                    where (c.PSortOrder.Substring(0, tType.Length) == tType
                                    &&
                                    c.FolderID == targetProj.FolderID
                                    &&
                                    c.ProjectID != targetProj.ProjectID  // exclude chance of Target in Batch
                                    &&
                                    c.ProjectID != sourceProj.ProjectID  // exclude chance of Source in Batch
                                    )
                                    orderby c.FolderID, c.PSortOrder
                                    select c).ToList();

                LogArray(targetChildBatch, "Target Child Batch");

                // --------------------------------------------------------------------------
                // System.Collections.Generic.List<Project> renumList = SetUpBatch(a, sourceProj, targetProj, sourceChildBatch, targetBatch, targetChildBatch);
                renumList = SetUpBatch(drpAction, sourceProj, targetProj, sourceChildBatch, targetBatch, targetChildBatch);
                LogArray(renumList, "Renumber List");

                mLogger.AddLogMessage("====== Process renumList ==========");
                if (renumList.Count == 0)
                {
                    mLogger.AddLogMessage("PROBLEM->renumList is empty!");
                    return;
                }
                ProcessBatch(ref renumList, targetProj.FolderID ?? 0);

                LogArray(renumList, "After Process Batch");

                mLogger.AddLogMessage("===== END  Process renumList ===========");
                //    string chkOrder = sourceProj.PSortOrder;
                //CheckForChanges();
            }
            // ====================================>
            // */
            ProcessBatch(ref renumList, targetFolderID);

            LogArray(renumList, "Renumber List After ProcessBatch");

            mLogger.AddLogMessage("^^^^ End TV Drag'n Drop");
            // ===================================

            //  Update PPartNum, PSortOrder used as intermediate variable
            System.Collections.Generic.List<Project> adjustBatch = new List<Project>();
            adjustBatch = (from d in db.Projects.AsEnumerable()
                           where
                               d.PPartNum != d.PSortOrder
                           select d).ToList();
            LogArray(adjustBatch, "Adjust Batch");

            mLogger.AddLogMessage("Processing adjustBatch");
            int np = 0;  // np not zero based!
            foreach (var prj in adjustBatch)
            {
                prj.PPartNum = prj.PSortOrder;
                mLogger.AddLogMessage(np + "-" + prj.PSortOrder + "-" + prj.PPartNum + "-" + prj.Item + "-" + prj.ProjectID + "-" + prj.FolderID);
                np++;
            }
            //int nAdjChanges = db.SaveChanges();
            mLogger.AddLogMessage("=== End Adjustment " + np + " changes were saved to database ===");
            // =====================================================

            int nChanges0 = db.SaveChanges();
            //ShowUserMessage("Database Updated with " + nChanges.ToString() + " changes.");
            mLogger.AddLogMessage("UpdateDB successfully completed. with " + nChanges0.ToString() + " changes.");
            mLogger.AddLogMessage("#### End Drop!");
            return;

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
        }

    }
}
