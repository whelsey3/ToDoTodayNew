using CodeFirst.EFcf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;
using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;

namespace Planner
{
    /// <summary>
    /// The ViewModel for the LoadOnDemand demo.  This simply
    /// exposes a read-only collection of regions.
    /// </summary>
    public partial class Plans2ViewModel : ViewModelBase //, IDropTarget, IDragSource // CrudVMBaseTDT
    {
        public static void HandleDropping(TreeViewItemViewModel source, TreeViewItemViewModel target, string drpAction)
        {
            var s = source.DbID;
            var t = target.DbID;
            string a = drpAction;
            mLogger.AddLogMessage("------ Dropping ============================");
            //mLogger.AddLogMessage("------ Dropping=  Source: " + s + " at Target: " + t + " Action: " + a);
            bool OnFolder = false;
            int adjIndex = 0;

            Project sourceProj = Plans2ViewModel.db.Projects.Find(Int32.Parse(s));
            string sType = GetCC(sourceProj.PSortOrder);
            string sBatch = sType.Substring(0, sType.Length - 3);
            // mLogger.AddLogMessage("Source PSortOrder: " + sourceProj.PSortOrder + "  sType: " + sType + "  sBatch: ->" + sBatch + "<-  trial: " + trial);
           // mLogger.AddLogMessage("Source PSortOrder: " + sourceProj.PSortOrder + "  sType: " + sType + "  sBatch: ->" + sBatch + "<-  " + sourceProj.Item);

            Project targetProj;

            if (t.Contains("F"))
            {
                #region Folder
                System.Collections.Generic.List<Project> renumListF = new List<Project>();
                OnFolder = true;

                // Dropping on Folder need to adjust target to last project in that folder.
                string tFolder0 = t.Substring(1);
                Folder targetFolder = db.Folders.Find(Int32.Parse(tFolder0));
                int targetFolderID = targetFolder.FolderID;  // Int32.Parse(tFolder0);
                mLogger.AddLogMessage("  ***** Dropping on Folder: " + targetFolder.FolderName + "-" + targetFolder.FPartNum);

                System.Collections.Generic.List<Project> sourceBatch2 = new List<Project>();

                sourceBatch2 = (from c in db.Projects
                                where (c.PSortOrder.Substring(0, sType.Length) == sType
                                &&
                                c.FolderID == sourceProj.FolderID)
                                &&
                                c.ProjectID != sourceProj.ProjectID  // exclude chance of Source in Batch
                                orderby c.FolderID, c.PSortOrder
                                select c).ToList();
                LogArray(sourceBatch2, "Source Children2");

                //  Check if Folder is empty
                //   CodeFirst.EFcf.TDTcfEntities db = new CodeFirst.EFcf.TDTcfEntities();
                var folderProjs = (from c in db.Projects
                                   where c.FolderID == targetFolderID
                                   orderby c.PPartNum descending
                                   select c).FirstOrDefault();
                if (folderProjs == null)
                {
                    // No projs in the folder currently
                    //  Source should be first followed by SourceChildren.  No Target.
                    //        System.Collections.Generic.List<Project> renumListF = new List<Project>();
                    //if (a == "BeforeTargetItem")
                    //{
                    // Make sure FolderID of first item reflects targetProj
                    sourceProj.FolderID = targetFolderID;
                    sourceProj.PPartNum = "001000000";

                    renumListF.Add(sourceProj);
                    //System.Collections.Generic.List<Project> sourceBatch2 = new List<Project>();

                    //sourceBatch2 = (from c in db.Projects
                    //               where (c.PSortOrder.Substring(0, sType.Length) == sType
                    //               &&
                    //               c.FolderID == sourceProj.FolderID)
                    //               &&
                    //               c.ProjectID != sourceProj.ProjectID  // exclude chance of Source in Batch
                    //               orderby c.FolderID, c.PSortOrder
                    //               select c).ToList();
                    LogArray(sourceBatch2, "Source Children2");

                    foreach (var prj in sourceBatch2)
                    {
                        renumListF.Add(prj);
                    }

                    //}
                }
                else
                {
                    //  There are projs in Folder
                    targetProj = folderProjs;
                    renumListF.Add(sourceProj);
                    foreach (var prj in sourceBatch2)
                    {
                        renumListF.Add(prj);
                    }

                }

                ProcessBatch(ref renumListF, targetFolderID );
                //////int rNum = 0;
                //////foreach (var dgRow in list)
                //////{
                //////    CodeFirst.EFcf.ToDo toDo = ((ToDoVM)dgRow).TheEntity;
                //////    toDo.TDTSortOrder = (rNum + 1000000).ToString().Substring(1, 6);
                //////    //toDo.
                //////    ToDo thisOne = db.ToDos.Find(toDo.ToDoID);
                //////    thisOne.TDTSortOrder = toDo.TDTSortOrder;

                //////    mLogger.AddLogMessage(rNum.ToString() + "  " + toDo.ToDoID.ToString() + " - " + toDo.TDTSortOrder);
                //////    rNum++;
                //////}
                //////// db.
                int nChanges0 = db.SaveChanges();
                //ShowUserMessage("Database Updated with " + nChanges.ToString() + " changes.");
                mLogger.AddLogMessage("UpdateDB successfully completed. with " + nChanges0.ToString() + " changes.");
                mLogger.AddLogMessage("#### End Drop on Folder");
                return;
                #endregion Folder end
            }
            else
            {
                // Normal case, NOT dropping on Folder
                if (a.Contains("TargetItemCenter"))
                {
                    adjIndex = 0;
                    mLogger.AddLogMessage("Dropping ON existing item.");
                }
                else if (a.Contains("BeforeTargetItem"))
                {
                    adjIndex = -1;
                    mLogger.AddLogMessage("Drop BEFORE.");
                }
                else if (a.Contains("AfterTargetItem"))
                {
                    adjIndex = 1;
                    mLogger.AddLogMessage("Drop AFTER");
                }
                else
                {
                    mLogger.AddLogMessage("drpAction PROBLEM!!!");
                }
                //        CodeFirst.EFcf.TDTcfEntities db = new CodeFirst.EFcf.TDTcfEntities();
                sourceProj = Plans2ViewModel.db.Projects.Find(Int32.Parse(s));
                targetProj = db.Projects.Find(Int32.Parse(t));
                string tType = GetCC(targetProj.PSortOrder);     // Remove any trailing blocks of '000'
                string tBatch = tType.Substring(0, tType.Length - 3);  // Move back one block 0, 3, 6
             
                mLogger.AddLogMessage("Source: " + s + ": " + sourceProj.FolderID + " - " + sourceProj.PSortOrder + "  sType: " + sType + "  sBatch: ->" + sBatch + "<-  " + " " + sourceProj.Item + " ");
                mLogger.AddLogMessage("      Action: " + a + "  adjIndex: " + adjIndex);
                mLogger.AddLogMessage("Target: " + t + ": " + targetProj.FolderID + " - " + targetProj.PSortOrder + "  tType: " + tType + "  tBatch: ->" + tBatch + "<-  " + " " + targetProj.Item + " ");

                //int adjIndex = (a == "BeforeTargetItem") ? -1 : 1;
                //adjIndex = (a == "BeforeTargetItem") ? -1 : 1;

                string trial22 = TrialNum(adjIndex, tType);
                string trial = tType;  // TrialNum(adjIndex, tType);
                trial = (trial + "000000").Substring(0, 9);
                mLogger.AddLogMessage("Target PSortOrder: " + targetProj.PSortOrder + "  tType: " + tType + "  tBatch: ->" + tBatch + "<-  trial: " + trial);

                //string sType = GetCC(sourceProj.PSortOrder);
                //string sBatch = sType.Substring(0, sType.Length - 3);
                mLogger.AddLogMessage("Source PSortOrder: " + sourceProj.PSortOrder + "  sType: " + sType + "  sBatch: ->" + sBatch + "<-  trial: " + trial);

                int sFolder = sourceProj.FolderID;
                int tFolder = targetProj.FolderID;

                // Build Batch for processing to update numbers (PSortOrder)
                //   1) All items in Target that may need updating
                //   2) Any children of Source that will need updating
                //   Two options:
                //       Target      Source
                //       Source      Target
                //             Batch
                // 
                // --------------------------------------------------------------------------
                System.Collections.Generic.List<Project> sourceChildBatch = new List<Project>();

                string sB = "PPPTTTSSS".Substring(0, sType.Length) + " == " + sType;
                mLogger.AddLogMessage("sourceChildBatch: " + sB);

                sourceChildBatch = (from c in db.Projects
                               where (c.PSortOrder.Substring(0, sType.Length) == sType
                               &&
                               c.FolderID == sourceProj.FolderID)
                               &&
                               c.ProjectID != sourceProj.ProjectID  // exclude chance of Source in Batch
                               orderby c.FolderID, c.PSortOrder
                               select c).ToList();
                LogArray(sourceChildBatch, "Source Children");


                // --------------------------------------------------------------------------
                System.Collections.Generic.List<Project> targetBatch = new List<Project>();
                // All like tBatch, exclude possible source
                int test = Convert.ToInt32(tType);

                string tB = "PPPTTTSSS".Substring(0, tType.Length) + " > " + test;
                mLogger.AddLogMessage("targetBatch: " + tB);

                targetBatch = (from d in db.Projects.AsEnumerable()
                               where (ChkPSortOrder(d.PSortOrder, tType) == true
                               //where ( ChkPSortOrder(d.PSortOrder,  tType) > test
                               //where (((Convert.ToInt32(d.PSortOrder.Substring(0, tType.Length))) > test)
                               &&
                               d.FolderID == targetProj.FolderID)
                               &&
                               d.PSortOrder.Substring(0, tType.Length) != tType
                               //d.ProjectID != sourceProj.ProjectID  
                               orderby d.FolderID, d.PSortOrder
                               select d).ToList();
                LogArray(targetBatch, "Target Batch");

                // --------------------------------------------------------------------------
                System.Collections.Generic.List<Project> targetChildBatch = new List<Project>();
                // All like tBatch, exclude possible source
                //int test = Convert.ToInt32(tType);

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
                //targetChildBatch = (from d in db.Projects.AsEnumerable()
                //               where (ChkPSortOrder(d.PSortOrder, tType) == true
                //               //where ( ChkPSortOrder(d.PSortOrder,  tType) > test
                //               //where (((Convert.ToInt32(d.PSortOrder.Substring(0, tType.Length))) > test)
                //               &&
                //               d.FolderID == targetProj.FolderID)
                //               &&
                //               d.PSortOrder.Substring(0, sType.Length) != sType
                //               //d.ProjectID != sourceProj.ProjectID  
                //               orderby d.FolderID, d.PSortOrder
                               //select d).ToList();
                LogArray(targetChildBatch, "Target Child Batch");

                // --------------------------------------------------------------------------
                System.Collections.Generic.List<Project> renumList = SetUpBatch(a, sourceProj, targetProj, sourceChildBatch, targetBatch, targetChildBatch);
                LogArray(renumList, "Renumber List");

                mLogger.AddLogMessage("====== Process renumList ==========");
                ProcessBatch(ref renumList, targetProj.FolderID);

                LogArray(renumList, "After Process Batch");

                mLogger.AddLogMessage("===== END  Process renumList ===========");

                int nChanges = db.SaveChanges();
                //ShowUserMessage("Database Updated after TV Drag n Drop with " + nChanges.ToString() + " changes.");
                mLogger.AddLogMessage("UpdateDB successfully completed after TV Drag n Drop with " + nChanges.ToString() + " changes.");

                //if (sFolder == tFolder)
                //{
                //    // Same folder, renumber PSortOrder

                //    if (a != "TargetItemCenter")
                //    {
                // Just a rearrangement

                //if (a == "BeforeTargetItem")
                //{
                // Before
                // Same as Target
                //    Project ahProj = (from c in db.Projects
                //                      where c.PSortOrder == trial &&
                //                      c.FolderID == targetProj.FolderID
                //                      select c).FirstOrDefault();
                //if (ahProj != null)
                //{
                //     mLogger.AddLogMessage("ahProj not null " + ahProj.PPartNum + " " + ahProj.Item);
                // Already exists, renumber core

                //int batchStart = 0;
                //int targetIndex = targetBatch.IndexOf(targetProj);  // Array.IndexOf(targetBatch, trial);
                //mLogger.AddLogMessage("targetIndex: " + targetIndex + " Len targetBatch: " + targetBatch.Count + " tType: " + tType + " tBatch->" + tBatch + "<-");

                //System.Collections.Generic.List<Project> renumList = new List<Project>();

                //System.Collections.Generic.List<Project> renumList = SetUpBatch(a, sourceProj, targetProj, sourceBatch, targetBatch);

                ////  mLogger.AddLogMessage("After Insert.  targetIndex: " + targetIndex + " Len targetBatch: " + targetBatch.Count + " tType: " + tType + " tBatch->" + tBatch + "<-  batchStart: " + batchStart);
                //LogArray(renumList, "Renumber List");
                //mLogger.AddLogMessage("================");

                //string cInitial = (targetProj.PSortOrder.Substring(tBatch.Length, 3) + "000000").;
                //  int Initial = Convert.ToInt32(targetProj.PSortOrder.Substring(tBatch.Length, 3));

                //}
                //else
                //{
                //    // Not there, so can use trial
                //    mLogger.AddLogMessage("Trial should work!" + trial);

                //    // }
                //    //}
                //    //else
                //    //{
                //    //    // After
                //    //}
                //}
                //else
                //{
                //    // Adjustment needed.  Make source Folder = target Folder

                //    }
                //}
            }
            mLogger.AddLogMessage("^^^^ End TV Drag'n Drop");
        }

        private static void ProcessBatch(ref List<Project> renumList, int theFolder)
        {
            string cv = renumList[0].PSortOrder;
            string ct = getType(cv);
            string nv = "";
            string nt = "";
            int np = 0;
            int nFolderID = theFolder;

            foreach (var prj in renumList)
            {
                string newPSortNum = "";
                if (np > 0)
                {
                    mLogger.AddLogMessage(np + "-" + prj.PSortOrder + "-" + prj.Item + "-" + prj.ProjectID + "-" + prj.FolderID);

                    nv = prj.PSortOrder;
                    nt = getType(nv);
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

                    prj.PSortOrder = newPSortNum;
                    prj.FolderID = nFolderID;

                    //ct = getType(cv);
                    mLogger.AddLogMessage(np + "-" + newPSortNum + "-" + prj.Item + "-" + prj.ProjectID + "-" + prj.FolderID);
                    mLogger.AddLogMessage("-");

                    cv = newPSortNum;

                    //string Inc = (Initial + 1000).ToString().Substring(1, 3);
                    //string newPSortNum = "";
                    //if (tBatch.Length == 0)
                    //{
                    //    newPSortNum = Inc + prj.PSortOrder.Substring(3, 6);
                    //}
                    //else if (tBatch.Length == 3)
                    //{
                    //    newPSortNum = prj.PSortOrder.Substring(0, 3) + Inc + prj.PSortOrder.Substring(6, 3);
                    //}
                    //else if (tBatch.Length == 6)
                    //{
                    //    newPSortNum = prj.PSortOrder.Substring(0, 6) + Inc;

                    //}
                    //mLogger.AddLogMessage(np + "-" + newPSortNum + "-" + prj.Item + "-" + prj.ProjectID + "-" + prj.FolderID);
                    //mLogger.AddLogMessage("-");

                    //Initial++;
                }
                else
                {
                    // First one (np == 0)
                    cv = prj.PSortOrder;  // First handled before method call

                    mLogger.AddLogMessage(np + "-" + prj.PSortOrder + "-" + prj.Item + "-" + prj.ProjectID + "-" + prj.FolderID);
                    mLogger.AddLogMessage(np + "-" + prj.PSortOrder);
                    mLogger.AddLogMessage("-");
                }
                np++;
                //prj.PSortOrder = newPSortNum;
                //prj.FolderID = nFolderID;
            }
        }

        private static System.Collections.Generic.List<Project> SetUpBatch(string a, Project sourceProj,
            Project targetProj, List<Project> sourceChildBatch,
            List<Project> targetBatch, List<Project> targetChildBatch)
        {
            System.Collections.Generic.List<Project> renumList = new List<Project>();
            if (a == "BeforeTargetItem")
            {
                // Make sure FolderID of first item reflects targetProj
                sourceProj.FolderID = targetProj.FolderID;

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

                // Renumber S as T
                renumList[0].PSortOrder = targetProj.PSortOrder;  // renumList[1].PSortOrder 

            }
            else
            {
                // AFTER or ON

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
            int test = Convert.ToInt32(tType);
            int chk = ((Convert.ToInt32(pSort.Substring(0, tType.Length))));
            if ((chk > test))
            {
                mLogger.AddLogMessage("--ChkPSortOrder: " + chk + "-" + test + " -pSort: " + pSort + " - tType: " + tType);
            }
            else
            {
                mLogger.AddLogMessage("false-ChkPSortOrder: " + chk + "-" + test + " -pSort: " + pSort + " - tType: " + tType);
            }
            return (chk > test);
        }

        private static void LogArray(List<Project> renumProjs, string note = "-")
        {
            mLogger.AddLogMessage(" ---LogArray--- " + note);
            int np = 0;  // np not zero based!
            //int Initial = Convert.ToInt32(targetProj.PSortOrder.Substring(tBatch.Length, 3));
            foreach (var prj in renumProjs)
            {
                mLogger.AddLogMessage(np + "-" + prj.PSortOrder + "-" + prj.PPartNum + "-" + prj.Item + "-" + prj.ProjectID + "-" + prj.FolderID);
                np++;
            }
            mLogger.AddLogMessage("=== End " + note + " LogArray ===");
        }

        private static string TrialNum(int v, string tType)
        {
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
        public virtual void StartDrag(IDragInfo dragInfo)
        {
            // nothing special here, use the default way
            DragDrop.DefaultDragHandler.StartDrag(dragInfo);
        }

        public bool CanStartDrag(IDragInfo dragInfo)
        {
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
            Plans2ViewModel.HandleDropping((TreeViewItemViewModel)(dropInfo.Data), (TreeViewItemViewModel)(dropInfo.TargetItem), dropInfo.InsertPosition.ToString());
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
