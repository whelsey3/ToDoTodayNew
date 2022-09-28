using BuildSqliteCF.Entity;
using GalaSoft.MvvmLight;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Linq;
using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;
using System.Windows;
using nuT3;

namespace Planner
{
    public partial class PlansViewModel : ViewModelBase //, IDropTarget, IDragSource // CrudVMBaseTDT
    {
        /*
         Some notes about handling of drag and drop (DnD)
          
         */

        //public static void NewHandleDropping(TreeViewItemViewModel source, TreeViewItemViewModel target, string drpAction, TDTDbContext db)
        public static void NewHandleDropping(TreeViewItemViewModel source, TreeViewItemViewModel target, string drpAction)
        {
            // Set up data connections and logging
            TDTDbContext db = ((App) Application.Current).db;

            Logger mLogger = Logger.Instance;
            mLogger.AddLogMessage("------ Dropping ====== NewHandleDropping ======================");

            System.Collections.Generic.List<Project> origSet = new List<Project>();
            origSet = (from c in db.Projects
                       orderby c.FolderID, c.PPartNum
                       select c).ToList();
            LogArray(origSet, "origSet");

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
            else  // Dropping on another project item (not a folder) +++++++++++++++++++++++++++++
            {
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
     //       ProcessBatch(ref renumList, targetFolderID);

     //       LogArray(renumList, "Renumber List After ProcessBatch");

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

       // public static void HandleDropping(TreeViewItemViewModel source, TreeViewItemViewModel target, string drpAction)
       // {
       //     // Set up data connections and logging
       //     TDTDbContext db = ((App)Application.Current).db;

       //     LoggerLib.Logger mLogger = LoggerLib.Logger.Instance;
       //     //if (target.DbID.Contains("F"))
       //     //{
       //     //    // Dropping on a folder
       //     NewHandleDropping(source, target, drpAction, db);
       //     return;
       //     //}

       //     // Set up common variables
       //     /*
       //     System.Collections.Generic.List<Project> renumList = new List<Project>();

       //     // Retrieve source from database, including any children
       //     Project sourceProj = db.Projects.Find(Int32.Parse(source.DbID));
       //     string sType = GetCC(sourceProj.PSortOrder);
       //     //string sBatch = sType.Substring(0, sType.Length - 3);

       //     System.Collections.Generic.List<Project> sourceProjects = new List<Project>();
       //     sourceProjects = (from c in db.Projects
       //         where (c.PSortOrder.Substring(0, sType.Length) == sType
       //                &&
       //                c.FolderID == sourceProj.FolderID)
       //               //&&
       //               //c.ProjectID != sourceProj.ProjectID  // exclude chance of Source in Batch
       //         orderby c.FolderID, c.PSortOrder
       //         select c).ToList();

       //     //  string tFolder0 = t.Substring(1);
       //     Folder targetFolder = db.Folders.Find(t.Substring(1);
       //     int targetFolderID = targetFolder.FolderID;  // Int32.Parse(tFolder0);

       //     //   idea is to process 
       //     if (target.DbID.Contains("F"))
       //     {
       //         // Dropping on a folder    
       //                         //  Check if Folder is empty
       //         var folderProjs = (from c in db.Projects
       //                            where c.FolderID == targetFolderID
       //                            orderby c.PPartNum descending
       //                            select c).FirstOrDefault();

       //         if (folderProjs == null)
       //         {
       //             // No projs currently in the folder 
       //             //  Source should be first followed by SourceChildren.  No Target.
       //             //        System.Collections.Generic.List<Project> renumListF = new List<Project>();
       //             //if (a == "BeforeTargetItem")
       //             //{
       //             // Make sure FolderID of first item reflects targetProj
       //             sourceProj.FolderID = targetFolderID;
       //             sourceProj.PPartNum = "001000000";

       //             renumListF.Add(sourceProj);
       //         }
       //         else
       //         {


       //         }
       //     }
       //     else
       //     {
       //         // Dropping on another item
       //     }
       //     // ====================================>
       //     */

       //     var s = source.DbID;
       //     var t = target.DbID;
       //     string a = drpAction;
       //     mLogger.AddLogMessage("------ Dropping ====== HandleDropping ======================");
       //     //mLogger.AddLogMessage("------ Dropping=  Source: " + s + " at Target: " + t + " Action: " + a);
       //     bool OnFolder = false;
       //     int adjIndex = 0;

       ////     Project sourceProj = Plans2ViewModel.db.Projects.Find(Int32.Parse(s));
       //     Project sourceProj = db.Projects.Find(Int32.Parse(s));
       //     string sType = GetCC(sourceProj.PSortOrder);
       //     string sBatch = sType.Substring(0, sType.Length - 3);
       //     // mLogger.AddLogMessage("Source PSortOrder: " + sourceProj.PSortOrder + "  sType: " + sType + "  sBatch: ->" + sBatch + "<-  trial: " + trial);
       //     // mLogger.AddLogMessage("Source PSortOrder: " + sourceProj.PSortOrder + "  sType: " + sType + "  sBatch: ->" + sBatch + "<-  " + sourceProj.Item);

       ////     dispDetailsVM.DAction = a;
       //     Project targetProj;
       //     System.Collections.Generic.List<Project> origSet = new List<Project>();
       //     origSet = (from c in db.Projects
       //                    //where
       //                    //c.FolderID == sourceProj.FolderID
       //                    //&&
       //                    //c.PSortOrder.Substring(0, sType.Length) == sType
       //                    ////&&
       //                    ////c.FolderID == sourceProj.FolderID)
       //                    //&&
       //                    //c.ProjectID != sourceProj.ProjectID  // exclude chance of Source in Batch
       //                orderby c.FolderID, c.PPartNum
       //                select c).ToList();
       //     LogArray(origSet, "origSet");

       //     if (t.Contains("F"))
       //     {
       //         #region Folder
       //         System.Collections.Generic.List<Project> renumListF = new List<Project>();
       //         OnFolder = true;

       //         // Dropping on Folder need to adjust target to last project in that folder.
       //         string tFolder0 = t.Substring(1);
       //         Folder targetFolder = db.Folders.Find(Int32.Parse(tFolder0));
       //         int targetFolderID = targetFolder.FolderID;  // Int32.Parse(tFolder0);
       //         mLogger.AddLogMessage("  ***** Dropping on Folder: " + targetFolder.FolderName + " - " + targetFolder.FPartNum);

       //         System.Collections.Generic.List<Project> sourceBatch2 = new List<Project>();

       //         sourceBatch2 = (from c in db.Projects
       //                         where (c.PSortOrder.Substring(0, sType.Length) == sType
       //                         &&
       //                         c.FolderID == sourceProj.FolderID)
       //                         &&
       //                         c.ProjectID != sourceProj.ProjectID  // exclude chance of Source in Batch
       //                         orderby c.FolderID, c.PSortOrder
       //                         select c).ToList();
       //         LogArray(sourceBatch2, "Source Children2");

       //         //  Check if Folder is empty
       //         //   CodeFirst.EFcf.TDTDbContext db = new CodeFirst.EFcf.TDTDbContext();
       //         var folderProjs = (from c in db.Projects
       //                            where c.FolderID == targetFolderID
       //                            orderby c.PPartNum descending
       //                            select c).FirstOrDefault();

       //         sourceProj.FolderID = targetFolderID;

       //         if (folderProjs == null)
       //         {
       //             // No projs currently in the folder 
       //             //  Source should be first followed by SourceChildren.  No Target.
       //             //        System.Collections.Generic.List<Project> renumListF = new List<Project>();
       //             //if (a == "BeforeTargetItem")
       //             //{
       //             // Make sure FolderID of first item reflects targetProj
       //             // sourceProj.FolderID = targetFolderID;
       //             //sourceProj.PPartNum = "001000000";

       //             renumListF.Add(sourceProj);
       //             //System.Collections.Generic.List<Project> sourceBatch2 = new List<Project>();

       //             //sourceBatch2 = (from c in db.Projects
       //             //               where (c.PSortOrder.Substring(0, sType.Length) == sType
       //             //               &&
       //             //               c.FolderID == sourceProj.FolderID)
       //             //               &&
       //             //               c.ProjectID != sourceProj.ProjectID  // exclude chance of Source in Batch
       //             //               orderby c.FolderID, c.PSortOrder
       //             //               select c).ToList();
       //             LogArray(sourceBatch2, "Source Children2");

       //             foreach (var prj in sourceBatch2)
       //             {
       //                 renumListF.Add(prj);
       //             }

       //             //}
       //         }
       //         else
       //         {
       //             //  There are projs in Folder
       //             targetProj = folderProjs;
       //             renumListF.Add(sourceProj);
       //             foreach (var prj in sourceBatch2)
       //             {
       //                 renumListF.Add(prj);
       //             }

       //         }

       //         ProcessBatch(ref renumListF, targetFolderID );
       //         //////int rNum = 0;
       //         //////foreach (var dgRow in list)
       //         //////{
       //         //////    CodeFirst.EFcf.ToDo toDo = ((ToDoVM)dgRow).TheEntity;
       //         //////    toDo.TDTSortOrder = (rNum + 1000000).ToString().Substring(1, 6);
       //         //////    //toDo.
       //         //////    ToDo thisOne = db.ToDos.Find(toDo.ToDoID);
       //         //////    thisOne.TDTSortOrder = toDo.TDTSortOrder;

       //         //////    mLogger.AddLogMessage(rNum.ToString() + "  " + toDo.ToDoID.ToString() + " - " + toDo.TDTSortOrder);
       //         //////    rNum++;
       //         //////}
       //         //////// db.
       //         int nChanges0 = db.SaveChanges();
       //         //ShowUserMessage("Database Updated with " + nChanges.ToString() + " changes.");
       //         mLogger.AddLogMessage("UpdateDB successfully completed. with " + nChanges0.ToString() + " changes.");
       //         mLogger.AddLogMessage("#### End Drop on Folder");
       //         return;
       //         #endregion Folder end
       //     }
       //     else
       //     {
       //         // Normal case, NOT dropping on Folder
       //         if (a.Contains("TargetItemCenter"))
       //         {
       //             adjIndex = 0;
       //             mLogger.AddLogMessage("Dropping ON existing item.");
       //         }
       //         else if (a.Contains("BeforeTargetItem"))
       //         {
       //             adjIndex = -1;
       //             mLogger.AddLogMessage("Drop BEFORE.");
       //         }
       //         else if (a.Contains("AfterTargetItem"))
       //         {
       //             adjIndex = 1;
       //             mLogger.AddLogMessage("Drop AFTER");
       //         }
       //         else
       //         {
       //             mLogger.AddLogMessage("**** drpAction PROBLEM!!!  ****");
       //         }
       //         //        CodeFirst.EFcf.TDTDbContext db = new CodeFirst.EFcf.TDTDbContext();
       //        // sourceProj = Plans2ViewModel.db.Projects.Find(Int32.Parse(s));
       //         targetProj = db.Projects.Find(Int32.Parse(t));
       //         string tType = GetCC(targetProj.PSortOrder);     // Remove any trailing blocks of '000'
       //         string tBatch = tType.Substring(0, tType.Length - 3);  // Move back one block 0, 3, 6
             
       //         mLogger.AddLogMessage("Source: " + s + ": " + sourceProj.FolderID + " - " + sourceProj.PSortOrder + "  sType: " + sType + "  sBatch: ->" + sBatch + "<-  " + " " + sourceProj.Item + " ");
       //         mLogger.AddLogMessage("      Action: " + a + "  adjIndex: " + adjIndex);
       //         mLogger.AddLogMessage("Target: " + t + ": " + targetProj.FolderID + " - " + targetProj.PSortOrder + "  tType: " + tType + "  tBatch: ->" + tBatch + "<-  " + " " + targetProj.Item + " ");

       //         //int adjIndex = (a == "BeforeTargetItem") ? -1 : 1;
       //         //adjIndex = (a == "BeforeTargetItem") ? -1 : 1;
       //         //string trial22 = TrialNum(adjIndex, tType);
       //         //string trial = tType;  // TrialNum(adjIndex, tType);
       //         //trial = (trial + "000000").Substring(0, 9);
       //         //  mLogger.AddLogMessage("Target PSortOrder: " + targetProj.PSortOrder + "  tType: " + tType + "  tBatch: ->" + tBatch + "<-");

       //         //string sType = GetCC(sourceProj.PSortOrder);
       //         //string sBatch = sType.Substring(0, sType.Length - 3);
       //         //  mLogger.AddLogMessage("Source PSortOrder: " + sourceProj.PSortOrder + "  sType: " + sType + "  sBatch: ->" + sBatch + "<-");

       //         int sFolder = sourceProj.FolderID ?? 0;
       //         int tFolder = targetProj.FolderID ?? 0;

       //         // Build Batch for processing to update numbers (PSortOrder)
       //         //   1) All items in Target that may need updating
       //         //         Exclude any source items
       //         //   2) Any children of Source that will need updating
       //         //   Three options:
       //         //       AFTER       ON         BEFORE
       //         //       Target      Source     Source
       //         //       Source      Target     Target
       //         //
       //         //             Batch
       //         // 
       //         // --------------------------------------------------------------------------

       //         System.Collections.Generic.List<Project> sourceChildBatch = new List<Project>();

       //         string sB = "PPPTTTSSS".Substring(0, sType.Length) + " == " + sType;
       //         mLogger.AddLogMessage("sourceChildBatch: " + sB);

       //         sourceChildBatch = (from c in db.Projects
       //                        where
       //                        c.FolderID == sourceProj.FolderID
       //                        &&
       //                        c.PSortOrder.Substring(0, sType.Length) == sType
       //                        //&&
       //                        //c.FolderID == sourceProj.FolderID)
       //                        &&
       //                        c.ProjectID != sourceProj.ProjectID  // exclude chance of Source in Batch
       //                        orderby c.FolderID, c.PSortOrder
       //                        select c).ToList();
       //         LogArray(sourceChildBatch, "sourceChildBatch");


       //         // --------------------------------------------------------------------------
       //         System.Collections.Generic.List<Project> targetBatchOld = new List<Project>();
       //         // All like tBatch, exclude possible source
       //         int test = Convert.ToInt32(tType);

       //         string tB = "PPPTTTSSS".Substring(0, tType.Length) + " > " + test;
       //         mLogger.AddLogMessage("targetBatch: " + tB);

       //         targetBatchOld = (from d in db.Projects.AsEnumerable()
       //                        where
       //                        (d.FolderID == targetProj.FolderID)
       //                        &&
       //                        ChkPSortOrder(d.PSortOrder, tType) == true
       //                        //where ( ChkPSortOrder(d.PSortOrder,  tType) > test
       //                        //where (((Convert.ToInt32(d.PSortOrder.Substring(0, tType.Length))) > test)
       //                        &&
       //                        d.PSortOrder.Substring(0, sType.Length) != sType
       //                        //&&
       //                        //d.ProjectID != sourceProj.ProjectID  
       //                        orderby d.FolderID, d.PSortOrder
       //                        select d).ToList();

       //         LogArray(targetBatchOld, "Target Batch Old");

       //         // --------------------------------------------------------------------------
       //         //string tBatch = tType.Substring(0, tType.Length - 3);  // Move back one block 0, 3, 6
       //         System.Collections.Generic.List<Project> targetBatch = new List<Project>();
       //         targetBatch = (from d in db.Projects.AsEnumerable()
       //                        where
       //                        (d.FolderID == targetProj.FolderID)
       //                        &&
       //                        d.PSortOrder.Substring(0, tType.Length - 3) == tBatch
       //                        && 
       //                        ChkPSortOrder(d.PSortOrder, tType) == true
       //                        //where ( ChkPSortOrder(d.PSortOrder,  tType) > test
       //                        //where (((Convert.ToInt32(d.PSortOrder.Substring(0, tType.Length))) > test)
       //                        &&
       //                        d.PSortOrder.Substring(0, sType.Length) != sType
       //                        //&&
       //                        //d.ProjectID != sourceProj.ProjectID  
       //                        orderby d.FolderID, d.PSortOrder
       //                        select d).ToList();

       //         LogArray(targetBatch, "Target Batch");

       //         // --------------------------------------------------------------------------
       //         System.Collections.Generic.List<Project> targetChildBatch = new List<Project>();
       //         // All like tBatch, exclude possible source
       //         //int test = Convert.ToInt32(tType);

       //         string tCB = "PPPTTTSSS".Substring(0, tType.Length) + " == " + tType;
       //         mLogger.AddLogMessage("targetChildBatch: " + tCB);

       //         targetChildBatch = (from c in db.Projects
       //                             where (c.PSortOrder.Substring(0, tType.Length) == tType
       //                             &&
       //                             c.FolderID == targetProj.FolderID
       //                             &&
       //                             c.ProjectID != targetProj.ProjectID  // exclude chance of Target in Batch
       //                             &&
       //                             c.ProjectID != sourceProj.ProjectID  // exclude chance of Source in Batch
       //                             )
       //                             orderby c.FolderID, c.PSortOrder
       //                             select c).ToList();
       //         //targetChildBatch = (from d in db.Projects.AsEnumerable()
       //         //               where (ChkPSortOrder(d.PSortOrder, tType) == true
       //         //               //where ( ChkPSortOrder(d.PSortOrder,  tType) > test
       //         //               //where (((Convert.ToInt32(d.PSortOrder.Substring(0, tType.Length))) > test)
       //         //               &&
       //         //               d.FolderID == targetProj.FolderID)
       //         //               &&
       //         //               d.PSortOrder.Substring(0, sType.Length) != sType
       //         //               //d.ProjectID != sourceProj.ProjectID  
       //         //               orderby d.FolderID, d.PSortOrder
       //                        //select d).ToList();
       //         LogArray(targetChildBatch, "Target Child Batch");

       //         // --------------------------------------------------------------------------
       //         System.Collections.Generic.List<Project> renumList = SetUpBatch(a, sourceProj, targetProj, sourceChildBatch, targetBatch, targetChildBatch);
       //         LogArray(renumList, "Renumber List");

       //         mLogger.AddLogMessage("====== Process renumList ==========");
       //         ProcessBatch(ref renumList, targetProj.FolderID ?? 0);

       //         LogArray(renumList, "After Process Batch");

       //         mLogger.AddLogMessage("===== END  Process renumList ===========");
       //     //    string chkOrder = sourceProj.PSortOrder;
       //         db.ChangeTracker.DetectChanges();

       //         // +++++++++++++++++++++++++++++++++++++++++->
       //         //   int numChanges = CheckChanges2(db, "GetPPartNum");
       //         string CallerMsg = "testing";
       //         int historyCount = 0;
       //         // ============================
       //         //TDTDbContext db = _db;   // ((App)Application.Current).db;
       //                                  ///TDTDbContext db = new TDTDbContext();
       //         string curType = "??";

       //         db.ChangeTracker.DetectChanges();
       //         bool PendingChanges = db.ChangeTracker.HasChanges();
       //         //db.ChangeTracker.
       //         if (PendingChanges)
       //         {
       //             int n = db.ChangeTracker.Entries().Count();
       //             mLogger.AddLogMessage("=== Running CheckChanges == " + CallerMsg + "== NumEntries-" + n);
       //             string changeType = "";
       //             foreach (
       //                 var history in db.ChangeTracker.Entries()
       //                       .Where(e => e.Entity is IModificationHistory && (e.State == System.Data.Entity.EntityState.Added ||
       //                       e.State == System.Data.Entity.EntityState.Modified))
       //                       .Select(e => e.Entity as IModificationHistory)
       //               )
       //             {
       //                 historyCount++;
       //                 //history.DateModified = DateTime.Now;
       //                 //if (history.DateCreated == DateTime.MinValue)
       //                 //{
       //                 //    history.DateCreated = DateTime.Now;
       //                 //}
       //                 changeType = history.GetType().ToString();
       //                 if (changeType.Contains("Project"))
       //                 {
       //                     mLogger.AddLogMessage("CheckChanges -" + historyCount + "  Project Changed- '" + ((Project)history).Item + "' -" + changeType);
       //                     mLogger.AddLogMessage("CheckChanges -" + historyCount + "  Project.FolderID was- '" + ((Project)history).FolderID + "' - " + ((Project)history).ProjectID);
       //                 }
       //                 else if (changeType.Contains("ToDo"))
       //                 {
       //                     mLogger.AddLogMessage("CheckChanges -" + historyCount + "  ToDo Changed- '" + ((ToDo)history).Item + "' -" + changeType);
       //                     mLogger.AddLogMessage("CheckChanges -" + historyCount + "  ToDo.ToDoID was- '" + ((ToDo)history).ToDoID + "' - " + ((ToDo)history).ProjectID);
       //                 }
       //                 else
       //                 {
       //                     mLogger.AddLogMessage("CheckChanges -" + historyCount + "  Track Changed- '" + ((Track)history).Item + "' -" + changeType);
       //                     mLogger.AddLogMessage("CheckChanges -" + historyCount + "  Track.TrackID was- '" + ((Track)history).TrackID + "' - " + ((Track)history).ProjectID);
       //                 }
       //             }
       //             //db.su.
       //             //      int nSaved = db.SaveChanges();
       //             //mLogger.AddLogMessage("!!!! Saved Pending Changes.  Count was " + nSaved);
       //             mLogger.AddLogMessage(" ============================");
       //         }
       //         else
       //         {
       //             mLogger.AddLogMessage("NO Pending Changes! --" + curType + "--");
       //         }

       //         // +++++++++++++++++++++++++++++++++++++++++->

       //         int nChanges = db.SaveChanges();
       //         //ShowUserMessage("Database Updated after TV Drag n Drop with " + nChanges.ToString() + " changes.");
       //         mLogger.AddLogMessage("UpdateDB successfully completed after TV Drag n Drop with " + nChanges.ToString() + " changes.");

       //     }
       //     mLogger.AddLogMessage("^^^^ End TV Drag'n Drop");
       //     // ===================================
       //     System.Collections.Generic.List<Project> adjustBatch = new List<Project>();
       //     adjustBatch = (from d in db.Projects.AsEnumerable()
       //                    where
       //                    d.PPartNum != d.PSortOrder
       //                    //(d.FolderID == targetProj.FolderID)
       //                    //&&
       //                    //d.PSortOrder.Substring(0, tType.Length - 3) == tBatch
       //                    //&&
       //                    //ChkPSortOrder(d.PSortOrder, tType) == true
       //                    ////where ( ChkPSortOrder(d.PSortOrder,  tType) > test
       //                    ////where (((Convert.ToInt32(d.PSortOrder.Substring(0, tType.Length))) > test)
       //                    //&&
       //                    //d.PSortOrder.Substring(0, sType.Length) != sType
       //                    ////&&
       //                    ////d.ProjectID != sourceProj.ProjectID  
       //                    //orderby d.FolderID, d.PSortOrder
       //                    select d).ToList();

       //     LogArray(adjustBatch, "Adjust Batch");
       //     int np = 0;  // np not zero based!
       //     foreach (var prj in adjustBatch)
       //     {
       //         prj.PPartNum = prj.PSortOrder;
       //         mLogger.AddLogMessage(np + "-" + prj.PSortOrder + "-" + prj.PPartNum + "-" + prj.Item + "-" + prj.ProjectID + "-" + prj.FolderID);
       //         np++;
       //     }
       //     int nAdjChanges = db.SaveChanges();
       //     mLogger.AddLogMessage("=== End Adjustment " + nAdjChanges + " changes were saved to database ===");
       //     // =====================================================
       // }

        public int CheckChanges2(TDTDbContext _db, string CallerMsg = "TBD")
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
                    //history.DateModified = DateTime.Now;
                    //if (history.DateCreated == DateTime.MinValue)
                    //{
                    //    history.DateCreated = DateTime.Now;
                    //}
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

        private static void ProcessBatch(ref List<Project> renumList, int? theFolder)
        {
            Logger mLogger = Logger.Instance;
            if (renumList.Count == 0)
            {
                mLogger.AddLogMessage("PROBLEM->renumList is empty!");
                return;
            }
            string cv = renumList[0].PSortOrder;
            string ct = getType(cv);
            string nv = "";
            string nt = "";
            int np = 0;
            int? nFolderID = theFolder;

            foreach (var prj in renumList)
            {
                string newPSortNum = "newNum";
                mLogger.AddLogMessage("Checking: '" + prj.Item + "'-" + prj.PPartNum + "-" + prj.PSortOrder);
                if (np > 0)
                {
                    // First one handled separately.
                    mLogger.AddLogMessage("orig:  " + np + "-" + ct + "-" + nt + "-" + prj.PSortOrder + "-'" + prj.Item + "'-" + prj.ProjectID + "-" + prj.FolderID + "-'" + prj.Item + "'");

                    nv = prj.PSortOrder;
                    nt = getType(nv);
                    #region Switch ct
                    switch (ct)
                    {
                        case "P":
                            switch (nt)
                            {
                                case "P":
                                    newPSortNum = Incr(cv.Substring(0, 3)) + "000000";
                                    ct = "P";
                                    break;
                                case "T":
                                    newPSortNum = cv.Substring(0, 3) + Incr(cv.Substring(3, 3)) + "000";
                                    ct = "T";
                                    break;
                                case "S":
                                    newPSortNum = cv.Substring(0, 6) + Incr(cv.Substring(6, 3)) + "000"; // BAD
                                    ct = "S";
                                    break;
                                default:
                                    break;
                            }

                            break;
                        case "S":
                            switch (nt)
                            {
                                case "P":
                                    newPSortNum = Incr(cv.Substring(0, 3)) + "000000";
                                    ct = "P";
                                    break;
                                case "T":
                                    newPSortNum = cv.Substring(0, 3) + Incr(cv.Substring(3, 3)) + "000";
                                    ct = "T";
                                    break;
                                case "S":
                                    newPSortNum = cv.Substring(0, 6) + Incr(cv.Substring(6, 3));
                                    ct = "S";
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "T":
                            switch (nt)
                            {
                                case "P":
                                    newPSortNum = Incr(cv.Substring(0, 3)) + "000000";
                                    ct = "P";
                                    break;
                                case "T":
                                    newPSortNum = cv.Substring(0, 3) + Incr(cv.Substring(3, 3)) + "000";
                                    ct = "T";
                                    break;
                                case "S":
                                    newPSortNum = cv.Substring(0, 6) + Incr(cv.Substring(6, 3));
                                    ct = "S";
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion Switch ct

                    prj.PSortOrder = newPSortNum;
                    prj.FolderID = nFolderID;

                    //ct = getType(cv);
                    mLogger.AddLogMessage("chng:  " + np + "-" + ct + "-" + nt + "-" + newPSortNum + "-'" + prj.Item + "'-" + prj.ProjectID + "-" + prj.FolderID + "---" + prj.PSortOrder);
                    mLogger.AddLogMessage("-");

                    cv = newPSortNum;

                }
                else
                {
                    // Process the First one (np == 0)
                    cv = prj.PSortOrder;  // First handled before method call

                    mLogger.AddLogMessage("first: " + np + "-" + ct + "-" + nt + "-" + prj.PSortOrder + "-'" + prj.Item + "'-" + prj.ProjectID + "-" + prj.FolderID);
                    mLogger.AddLogMessage("first: " + np + "-" + ct + "-" + nt + "-" + prj.PSortOrder);
                    mLogger.AddLogMessage("+");
                }
                np++;
          //      mLogger.AddLogMessage("Checking: " + prj.Item + "-" + prj.PPartNum + "-" + prj.PSortOrder + "-" + newPSortNum);

            }
        }

        private static System.Collections.Generic.List<Project> SetUpBatch(string a, Project sourceProj,
            Project targetProj, List<Project> sourceChildBatch,
            List<Project> targetBatch, List<Project> targetChildBatch)
        {
            Logger mLogger = Logger.Instance;

            System.Collections.Generic.List<Project> renumList = new List<Project>();

            if (a.Contains("TargetItemCenter"))
            {
                mLogger.AddLogMessage("--SetUpBatch TargetItemCenter: ");

                // DROPPING ON TARGET PROJ ----------------------------

                // Calculate new PSortOrder for sourceProj (tBatch)
                string pSort = targetProj.PSortOrder;
                string tType = GetCC(pSort);     // Remove any trailing blocks of '000'
                string newtType = "";
                string newOne = "";
                string lastChild = "";
                string tBatch = "";
                string tTypeLastChild = "";
                // Source becomes next child of Target
                if (targetChildBatch.Count == 0)
                {
                    // Currently no children, source becomes first
                    newtType = (tType + "001000").Substring(0, 9);
                    newtType = tType + Incr(pSort.Substring(tType.Length - 3, 3));
                    lastChild = pSort;
                    tTypeLastChild = GetCC(lastChild) + "000";
                    tTypeLastChild = tTypeLastChild.Substring(0, tTypeLastChild.Length);
                }
                else
                {
                    // Already has child, source becomes next in line
                    lastChild = targetChildBatch[targetChildBatch.Count - 1].PSortOrder;
                    tTypeLastChild = GetCC(lastChild);
                }
                    //tTypeLastChild = GetCC(lastChild);
                    newOne = tTypeLastChild.Substring(tTypeLastChild.Length - 3, 3);
                    //newtType = tTypeLastChild + Incr(pSort.Substring(tTypeLastChild.Length - 3, 3));
                    newtType = newOne + tTypeLastChild + Incr(pSort.Substring(tTypeLastChild.Length - 3, 3));
                    //tType.Substring(tType.Substring(0, tType.Length - 3, 3));
                    tBatch = tType + tType.Substring(0, tType.Length - 3) + Incr(newOne);
                    tBatch = tBatch + "000000";
                    tBatch = tBatch.Substring(0, 9);
                

                //string tType = GetCC(targetProj.PSortOrder);     // Remove any trailing blocks of '000'
                // string tBatch = (tType + "001000").Substring(0, tType.Length - 3);  // Move back one block 0, 3, 6
               // string tBatch = (tType + "001000").Substring(0, 9);  // 
                mLogger.AddLogMessage("ON reset->" + targetProj.PSortOrder + "-" + tType + "-" + tBatch);

                // Make sure FolderID of first item reflects targetProj
                sourceProj.FolderID = targetProj.FolderID;
                sourceProj.PSortOrder = tBatch;  // GetCC(targetProj.PSortOrder) + "001000";

                renumList.Add(sourceProj);

                foreach (var prj in sourceChildBatch)
                {
                    renumList.Add(prj);
                }

                mLogger.AddLogMessage("--SetUpBatch first PSortOrder: " + renumList[0].PSortOrder + "<- " + renumList[0].FolderID);
            }
            else if (a == "BeforeTargetItem")
            {
                mLogger.AddLogMessage("--SetUpBatch BeforeTargetItem: ");
                // Make sure FolderID of first item reflects targetProj
                sourceProj.FolderID = targetProj.FolderID;

                // Set first item's PSortNum using targetProj
                sourceProj.PSortOrder = targetProj.PSortOrder;

                renumList.Add(sourceProj);

                foreach (var prj in sourceChildBatch)
                {
                    renumList.Add(prj);
                }

                renumList.Add(targetProj);

                foreach (var prj in targetChildBatch)
                {
                    renumList.Add(prj);
                }

                foreach (var prj in targetBatch)
                {
                    renumList.Add(prj);
                }

            }
            else if (a == "AfterTargetItem")
            {
                // AFTER ---------------------------------------
                mLogger.AddLogMessage("--SetUpBatch AfterTargetItem: ");

                // Make sure FolderID of first item reflects targetProj
                sourceProj.FolderID = targetProj.FolderID;

                //renumList.Add(targetProj);
                //foreach (var prj in targetChildBatch)
                //{
                //    renumList.Add(prj);
                //}
                string sNewType = "";
                if (targetChildBatch.Count == 0)
                {
                    renumList.Add(targetProj);
                    sNewType = targetProj.PSortOrder;
                }
                else
                {
                    renumList.Add(targetChildBatch[targetChildBatch.Count - 1]);
                    sNewType = targetChildBatch[targetChildBatch.Count - 1].PSortOrder;
                }
                sourceProj.PSortOrder = sNewType;

                renumList.Add(sourceProj);

                foreach (var prj in sourceChildBatch)
                {
                    renumList.Add(prj);
                }

                foreach (var prj in targetBatch)
                {
                    renumList.Add(prj);
                }
                //// Start S with last TargetChild or the Target, so have correct type PTS
                //if (targetChildBatch.Count == 0)
                //{
                //    renumList[0].PSortOrder = targetProj.PSortOrder;
                //}
                //else
                //{
                //    renumList[0].PSortOrder = targetChildBatch[targetChildBatch.Count - 1].PSortOrder;
                //}
                mLogger.AddLogMessage("--SetUpBatch first PSortOrder: " + renumList[0].PSortOrder + "<-");
            }
            
            return renumList;
        }

        private static string getType(string cv)
        {
            if (cv.Substring(3, 6) == "000000")
            {
                // Target is a Project, need anything beyond PPP
                return "P"; // PPP
            }
            else if (cv.Substring(6, 3) == "000")
            {
                // Target is a Task, need anything beyond PPPTTT
                return "T"; // PPPTTT
            }
            else //if (cv.Substring(3, 6) == "000000")
            {
                // Must be Target is a SubTask, need anything beyond PPPTTTSSS
                return "S";  // PPPTTTSSS
            }
        }

        public static bool ChkPSortOrder(string pSort, string tType)
        {
            Logger mLogger = Logger.Instance;

            int test = Convert.ToInt32(tType);
            int chk = ((Convert.ToInt32(pSort.Substring(0, tType.Length))));
            if ((chk > test))
            {
                //mLogger.AddLogMessage("--ChkPSortOrder: " + chk + "-" + test + " -pSort: " + pSort + " - tType: " + tType);
            }
            else
            {
                //mLogger.AddLogMessage("false-ChkPSortOrder: " + chk + "-" + test + " -pSort: " + pSort + " - tType: " + tType);
            }
            return (chk > test);
        }

        private static void LogArray(List<Project> renumProjs, string note = "-")
        {
            Logger mLogger = Logger.Instance;

            mLogger.AddLogMessage(" ---LogArray--- '" + note + "'   Length->" + renumProjs.Count);
            int np = 0;  // np not zero based!
            //int Initial = Convert.ToInt32(targetProj.PSortOrder.Substring(tBatch.Length, 3));
            foreach (var prj in renumProjs)
            {
                mLogger.AddLogMessage(np + "-" + prj.PSortOrder + "-" + prj.PPartNum  + "'-" + prj.ProjectID + "-" + prj.FolderID + "-'" + prj.Item);
                np++;
            }
            mLogger.AddLogMessage("=== End '" + note + "' LogArray ===");
        }

        private static string TrialNum(int v, string tType)
        {
            LoggerLib.Logger mLogger = LoggerLib.Logger.Instance;

            //  PPP or PPPTTT or PPPTTTSSS
            int strt = tType.Length - 3;  // 0 3 6
            string t = tType.Substring(strt, 3);

            int nNew = Convert.ToInt32(t);
            //nNew++;
            //nNew = nNew + v;
            if (nNew == 0)
            {
                // At the first one
            }
            else
            {

            }
            nNew = nNew + 1000 + v;
            string newNumStr = nNew.ToString().Substring(1, 3);
            string n1 = tType.Substring(0, strt + 3) + newNumStr;
            mLogger.AddLogMessage("TrialNum->" + strt + "-" + tType + "-" + n1);
            return n1;
            //throw new NotImplementedException();
        }

        private static string Incr(string v)
        {
            int delta = 1;
            int nNew = Convert.ToInt32(v);
            nNew = nNew + 1000 + delta;
            string newNumStr = nNew.ToString().Substring(1, 3);

            return newNumStr;
        }

        private static string GetCC(string pSortOrder)
        {
            // Examine type of Target and determine extent of 
            //  adjustments to PSortOrder
            if (pSortOrder.Substring(3, 6) == "000000")
            {
                // Target is a Project, need anything beyond PPP
                return pSortOrder.Substring(0, 3); // PPP
            }
            else if (pSortOrder.Substring(6, 3) == "000")
            {
                // Target is a Task, need anything beyond PPPTTT
                return pSortOrder.Substring(0, 6); // PPPTTT
            }
            else //if (pSortOrder.Substring(3, 6) == "000000")
            {
                // Must be Target is a SubTask, need anything beyond PPPTTTSSS
                return pSortOrder.Substring(0, 9);  // PPPTTTSSS
            }
        }

        //// Implement IDropTarget
        //void DragOver(DropInfo dropInfo)
        //{
        //    //if (dropInfo.Data is PupilViewModel && dropInfo.TargetItem is SchoolViewModel)
        //    //{
        //    //    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
        //    //    dropInfo.Effects = DragDropEffects.Move;
        //    //}
        //}

        //void Drop(DropInfo dropInfo)
        //{
        //    //SchoolViewModel school = (SchoolViewModel)dropInfo.TargetItem;
        //    //PupilViewModel pupil = (PupilViewModel)dropInfo.Data;
        //    //school.Pupils.Add(pupil);
        //    //((IList)dropInfo.DragInfo.SourceCollection).Remove(pupil);
        //}

        //// Implement IDragSource
        //void StartDrag(DragInfo dragInfo)
        //{

        //    //PupilViewModel pupil = (PupilViewModel)dragInfo.SourceItem;

        //    //if (pupil.FullName != "Tom Jefferson")
        //    //{
        //    //    dragInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;
        //    //    dragInfo.Data = pupil;
        //    //}
        //}
        //protected async override void GetData()  // Loads ToDos and sets empty Tracks
        //{
        //    mLogger.AddLogMessage("GetData in ToDosViewModel" + "------------------>");
        //    ThrobberVisible = Visibility.Visible;
        //    ObservableCollection<ToDoVM> _ToDos = new ObservableCollection<ToDoVM>();
        //    var toDos = await (from c in db.ToDos
        //                       orderby c.Status, c.Priority  // c.RevDueDate, c.Status, c.Priority
        //                       select c).ToListAsync();
        //    foreach (ToDo cust in toDos)
        //    {
        //        _ToDos.Add(new ToDoVM { IsNew = false, TheEntity = cust });
        //    }
        //    ToDos = _ToDos;
        //    RaisePropertyChanged("ToDos");

        //    ObservableCollection<TrackVM> _Tracks = new ObservableCollection<Planner.TrackVM>();
        //    Tracks = _Tracks;
        //    RaisePropertyChanged("Tracks");

        //    ThrobberVisible = Visibility.Collapsed;
        //}
    }

    #region DragHandling
    public class CustomDragHandlerWH : IDragSource
    {
        public Logger mLogger = Logger.Instance;
        public virtual void StartDrag(IDragInfo dragInfo)
        {
//            PlansViewModel.mLogger.AddLogMessage("CustomDragHandlerWH.StartDrag");
            mLogger.AddLogMessage("CustomDragHandlerWH.StartDrag");
            // nothing special here, use the default way
            DragDrop.DefaultDragHandler.StartDrag(dragInfo);
        }

        public bool CanStartDrag(IDragInfo dragInfo)
        {
            mLogger.AddLogMessage("WH CanStartDrag  PlansDnD ");
            if (dragInfo.SourceItem is dbFolderViewModel)
            {
                // Don't allow Folder dragging
                return false;
            }
            else
            {
                return DragDrop.DefaultDragHandler.CanStartDrag(dragInfo);
            }
        }

        public virtual void Dropped(IDropInfo dropInfo)
        {
            mLogger.AddLogMessage("CustomDragHandlerWH.Dropped");
         DragDrop.DefaultDragHandler.Dropped(dropInfo);
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

//             PlansViewModel.HandleDropping((TreeViewItemViewModel)(dropInfo.Data), (TreeViewItemViewModel)(dropInfo.TargetItem), dropInfo.InsertPosition.ToString());
             PlansViewModel.NewHandleDropping((TreeViewItemViewModel)(dropInfo.Data), (TreeViewItemViewModel)(dropInfo.TargetItem), dropInfo.InsertPosition.ToString());
        
    }

    public virtual void DragCancelled()
        {
            DragDrop.DefaultDragHandler.DragCancelled();
        }

        public bool TryCatchOccurredException(Exception exception)
        {
            return false;
        }
    }

    #endregion

}
