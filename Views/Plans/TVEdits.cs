using BuildSqliteCF.Entity;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Planner
{
    /// <summary>
    /// Description for ProjectsView.
    /// </summary>
    public partial class PlansView : UserControl
//    public partial class PlansViewModel : ViewModelBase, IDropTarget, IDragSource // CrudVMBaseTDT

    {
        // LoggerLib.Logger mLogger;

        /// <summary>
        /// Creates a sub category for the clicked item
        /// and refreshes the tree.
        /// </summary>

        /// <summary>
        /// Displays an input dialog and returns the entered
        /// value.
        /// </summary>
        public string ShowInputDialog(string defaultValue)
        {
            InputDialog dlg = new InputDialog();
            dlg.CategoryName = defaultValue;
            Window parentWindow = Window.GetWindow(this);
            dlg.Owner = parentWindow;  // (Window)(this.Parent);
                                       // dlg.DialogResult = false;
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                if (dlg.CategoryName.Length != 0)
                {
                    // User clicked OK and entered a name
                }
                else
                {
                    // Clicked OK but name is blank, really not needed
                    //    OK button not enabled unless name is NOT blank, see XAML
                    dlg.CategoryName = "ZPQ";
                }
            }
            else
            {
                dlg.CategoryName = "ZPQ";
            }
            return dlg.CategoryName;
        }


        //public void AddingNewOne(TreeViewItemViewModel parent, string name)
        //{
        //    // Get name for new work item
        //    // string name = ShowInputDialog(null);
        //    if (name != "ZPQ")
        //    {
        //        // Get Parent type and FolderID
        //        int theFolderID = 0;
        //        string theType = parent.GetType().ToString();
        //     //   theFolderID = GetFolderID(parent, theType);

        //        // Could be F, P, T, S
        //        // new is   P, T, S, S  but really are all P objects in database
        //        mLogger.AddLogMessage("Adding New One ->'" + name + "' - Parent type-> " + theType);
        //        // Process each type:

        //        //    1) Get newProj
        //        Project newProj = GetNewProj(name, parent);

        //        // Adjust base version, appropriately based on desired type
        //        //    2) Get PPartNum (and PSortOrder)
        //        //    3) Get appropriate TreeViewItemViewModel
        //        //    4) Save to database
        //        //    5) Add new child to Parent.Children
        //        if (theType.Contains("Folder"))
        //        {
        //            // Get appropriate view model
        //            dbProjectViewModel newDBItem = new dbProjectViewModel(newProj, (dbFolderViewModel)parent, false, db);
        //            // Add new child to parent
        //            parent.Children.Add(newDBItem);
        //        }
        //        else if (theType.Contains("Project"))
        //        {
        //            // Get appropriate view model
        //            dbTaskViewModel newDBItem = new dbTaskViewModel(newProj, (dbProjectViewModel)parent, false, db);
        //            // Add new child to parent
        //            parent.Children.Add(newDBItem);
        //        }
        //        else if (theType.Contains("SubTask"))
        //        {
        //            //  If parent is SubTask the new item will also be treated as SubTask
        //            dbTaskViewModel theParentTask = ((dbTaskViewModel)((dbSubTaskViewModel)parent).Parent);
        //            //parent.Children.Add(newDBItem);
        //            dbSubTaskViewModel newDBItem = new dbSubTaskViewModel(newProj, theParentTask, false);
        //            theParentTask.Children.Add(newDBItem);
        //        }
        //        else if (theType.Contains("Task"))
        //        {
        //            dbSubTaskViewModel newDBItem = new dbSubTaskViewModel(newProj, (dbTaskViewModel)parent, false);
        //            parent.Children.Add(newDBItem);
        //        }
        //        int n = newProj.ProjectID;
        //    }
        //    else
        //    {
        //        return;  // User did not agree to enter name
        //    }
        //}

        //private Project GetNewProj(string name, TreeViewItemViewModel parent)
        //{
        //    string parentType = parent.GetType().ToString();
        //    Project newProj = null;
        //    if (DetailsVM.IsNew)
        //    {
        //        // Using full dialog
        //        newProj = DetailsVM.TheEntity;
        //    }
        //    else
        //    {
        //        newProj = new Project(name);  // Need to add additional detail values
        //                                              // Set FolderID based on the Parent
        //        newProj.FolderID = GetFolderID(parent, parentType);
        //        newProj.DetailedDesc = "Details. . .";
        //        newProj.Priority = "A";
        //        newProj.Status = "O";
        //        newProj.StartDate = DateTime.Today;
        //        newProj.DueDate = DateTime.Today.AddDays(5);
        //        newProj.RevDueDate = DateTime.Today.AddDays(5);
        //        newProj.DoneDate = null;  // DateTime.MinValue;
        //        newProj.RespPerson = "Bill";
        //        newProj.Hide = false;
        //        newProj.DispLevel = "1";
        //        newProj.Done = false;
        //    }
        //    if (parentType.Contains("Folder"))
        //    {
        //        newProj.PSortOrder = newProj.PPartNum = this.newPPartNumForProj(newProj.FolderID);
        //    }
        //    else if (parentType.Contains("Project"))
        //    {
        //        newProj.PSortOrder = newProj.PPartNum = newPPartNumForTask(newProj.FolderID, ((dbProjectViewModel)parent)._project.PPartNum.Substring(0, 3));
        //    }
        //    else if (parentType.Contains("SubTask"))
        //    {
        //        //  If parent is SubTask the new item will also be treated as SubTask
        //        dbTaskViewModel theParentTask = ((dbTaskViewModel)((dbSubTaskViewModel)parent).Parent);
        //        newProj.PSortOrder = newProj.PPartNum = newPPartNumForSubTask(newProj.FolderID, ((dbSubTaskViewModel)parent)._subTask.PPartNum.Substring(0, 6));
        //    }
        //    else if (parentType.Contains("Task"))
        //    {
        //        newProj.PSortOrder = newProj.PPartNum = newPPartNumForSubTask(newProj.FolderID, ((dbTaskViewModel)parent)._task.PPartNum.Substring(0, 6));
        //    }

        //    PlansViewModel.db.Projects.Add(newProj);
        //    int savedMany = PlansViewModel.db.SaveChanges();
        //    newProj.ProjectID = newProj.ProjectID;  // Capture key value added by database

        //    return newProj;
        //}

        //private int GetFolderID(TreeViewItemViewModel parent, string theType)
        //{
        //    int theFolderID;
        //    //    1) Either a folder  (F)
        //    //    2) or must be in the Project database (P T S)
        //    // Check type of Parent
        //    if (theType.Contains("Folder"))
        //    {
        //        theFolderID = Convert.ToInt32(FolderFix(parent.DbID));  // parentProj.FolderID;
        //    }
        //    else
        //    {
        //        //  Must be working with a "project" database item
        //        Project parentProj = db.Projects.Find(Convert.ToInt32(parent.DbID));
        //        theFolderID = parentProj.FolderID;
        //    }

        //    return theFolderID;
        //}

        void AddingNewOneOLD(TreeViewItemViewModel parent)
        {
            // Request input of name and continuing effort
            // Get name for new item
            string name = ShowInputDialog(null);
            if (name != "ZPQ")
            {
                // Get Parent type and FolderID
                int theFolderID = 0;  // Convert.ToInt32(FolderFix(parent.DbID));  // parentProj.FolderID;

                //    1) Either a folder  (F)
                //    2) or must be in the Project database (P T S)
                // Check type of Parent
                string theType = parent.GetType().ToString();

                if (theType.Contains("Folder"))
                {
                    theFolderID = Convert.ToInt32(((PlansViewModel)DataContext).FolderFix(parent.DbID));  // parentProj.FolderID;
                }
                else
                {
                    //  Must be working with a "project" database item
                    Project parentProj = db.Projects.Find(Convert.ToInt32(parent.DbID));
                    theFolderID = parentProj.FolderID ?? 0;
                }

                // Could be F, P, T, S
                // new is   P, T, S, S  but really are all P objects in database
                mLogger.AddLogMessage("Adding New One ->'" + name + "' - Parent type-> " + theType);
                // Process each type:

                //    1) Get newProj
                Project newProj = new Project(name);  // Need to add additional detail values
                // Set FolderID based on the Parent
                newProj.FolderID = theFolderID;
                newProj.DetailedDesc = "Details. . .";
                newProj.Priority = "A";
                newProj.Status = "O";
                newProj.StartDate = DateTime.Today;
                newProj.DueDate = DateTime.Today.AddDays(5);
                newProj.RevDueDate = DateTime.Today.AddDays(5);
                newProj.DoneDate = null;  // DateTime.MinValue;
                newProj.RespPerson = ""; // "Bill";
                newProj.Hide = false;
                newProj.DispLevel = "1";
                newProj.Done = false;

                // Adjust base version, appropriately based on desired type
                //    2) Get PPartNum (and PSortOrder)
                //    3) Get appropriate TreeViewItemViewModel
                //    4) Save to database
                //    5) Add new child to Parent.Children
                if (theType.Contains("Folder"))
                {
                    newProj.PSortOrder = newProj.PPartNum = ((PlansViewModel)DataContext).newPPartNumForProj(theFolderID);
                    db.Projects.Add(newProj);
                    int savedMany = db.SaveChanges();
                    newProj.ProjectID = newProj.ProjectID;  // Capture key value added by database

                    // Get appropriate view model
                    dbProjectViewModel newDBItem = new dbProjectViewModel(newProj, (dbFolderViewModel)parent, false, db);
                    // Add new child to parent
                    parent.Children.Add(newDBItem);
                }
                else if (theType.Contains("Project"))
                {
                    newProj.PSortOrder = newProj.PPartNum = ((PlansViewModel)DataContext).newPPartNumForTask(theFolderID, ((dbProjectViewModel)parent)._project.PPartNum.Substring(0, 3));
                    db.Projects.Add(newProj);

                    //string changeType = "";
                    //int historyCount = 0;
                    //foreach (var history in System.Data.Entity.Infrastructure.DbChangeTracker.Entries()
                    //  .Where(e => e.Entity is IModificationHistory && (e.State == EntityState.Added ||
                    //          e.State == EntityState.Modified))
                    //   .Select(e => e.Entity as IModificationHistory)
                    //  )
                    //{
                    //    historyCount++;
                    //    //history.DateModified = DateTime.Now;
                    //    //if (history.DateCreated == DateTime.MinValue)
                    //    //{
                    //    //    history.DateCreated = DateTime.Now;
                    //    //}
                    //    changeType = history.GetType().ToString();
                    //}




                    int savedMany = db.SaveChanges();
                    newProj.ProjectID = newProj.ProjectID; // Capture key value added by database

                    // Get appropriate view model
                    dbTaskViewModel newDBItem = new dbTaskViewModel(newProj, (dbProjectViewModel)parent, false, db);
            //        newDBItem.DbID = newProj.ProjectID.ToString();    // Fix for immediate Add to TDT
                    // Add new child to parent
                    parent.Children.Add(newDBItem);
                }
                else if (theType.Contains("SubTask"))
                {
                    //  If parent is SubTask the new item will also be treated as SubTask
                    dbTaskViewModel theParentTask = ((dbTaskViewModel)((dbSubTaskViewModel)parent).Parent);
                    //newProj.PSortOrder = newProj.PPartNum = theParentTask._task.PPartNum.Substring(0, 6) + "000";
                    newProj.PSortOrder = newProj.PPartNum = ((PlansViewModel)DataContext).newPPartNumForSubTask(newProj.FolderID ?? 0, ((dbSubTaskViewModel)parent)._subTask.PPartNum.Substring(0, 6));
                    //item.DbID = ((dbProjectViewModel)item).ProjectName;
                    //   theDB.db.Projects.Add(newProj);
                    db.Projects.Add(newProj);
                    int savedMany = db.SaveChanges();
                    newProj.ProjectID = newProj.ProjectID; // Capture key value added by database

                    //parent.Children.Add(newDBItem);
                    dbSubTaskViewModel newDBItem = new dbSubTaskViewModel(newProj, theParentTask, false);
                    theParentTask.Children.Add(newDBItem);
                }
                else if (theType.Contains("Task"))
                {
                    // Set FolderID based on the Parent
                    newProj.FolderID = ((dbTaskViewModel)parent)._task.FolderID;
                    newProj.PSortOrder = newProj.PPartNum = ((PlansViewModel)DataContext).newPPartNumForSubTask(newProj.FolderID ?? 0, ((dbTaskViewModel)parent)._task.PPartNum.Substring(0, 6));
                    //((dbTaskViewModel)parent)._task.PPartNum.Substring(0, 6) + "000";

                    //item.DbID = ((dbProjectViewModel)item).ProjectName;
                    //    theDB.db.Projects.Add(newProj);
                    db.Projects.Add(newProj);
                    int savedMany = db.SaveChanges();
                    newProj.ProjectID = newProj.ProjectID; // Capture key value added by database

                    dbSubTaskViewModel newDBItem = new dbSubTaskViewModel(newProj, (dbTaskViewModel)parent, false);
                    parent.Children.Add(newDBItem);
                }
                int n = newProj.ProjectID;
            }
            else
            {
                return;  // User did not agree to enter name
            }
        }

        //private object FolderFix(string dbID)
        //{
        //    mLogger.AddLogMessage("Incoming dbID: " + dbID);
        //    if (dbID.Contains("F"))
        //    {
        //        //  Dealing with Folder
        //        dbID = dbID.Substring(1);
        //    }
        //    mLogger.AddLogMessage("Outgoing dbID: " + dbID);
        //    return dbID;
        //   // throw new NotImplementedException();
        //}

        void AddingNewFolder(dbFolderViewModel parentFolder)
        {
            //get the processed item related to context menu
            ///TreeViewItemViewModel parent = GetCommandItem();

            // Get name for new Folder
            string name = ShowInputDialog(null);
            if (name != "ZPQ")
            {
         //      Database theDB = new Database();

                Folder newFolder = new Folder(name);
                newFolder.FPartNum = "987000000";
                newFolder.FSortOrder = "987000000";
                newFolder.DetailedDesc = "Details. . .";
                //newFolder.FolderName = "A";
                //newFolder.FPartNum = "O";
                //newFolder.Projects = DateTime.Today;
                newFolder.Hide = false;
                newFolder.DispLevel = "1";
                //newFolder = ;
                //newFolder = ;
                //newProj = ;
                //return newFolder;

                // =============================
                newFolder.FPartNum = GetFPartNum(parentFolder);
                newFolder.FSortOrder = newFolder.FPartNum;
                newFolder.DateCreated = DateTime.Now;
                // =============================

                // Get base version of Project
                //Project theTask = GetNewProject(name);   // new Project(name);
                // Check type of Parent
                //string theType = parent.GetType().ToString();
                // Could be F, P, T, S
                // new is   P, T, S, S  but really are all P objects
                //   mLogger.AddLogMessage("Adding New Folder ->" + name + " - " );

                db.Folders.Add(newFolder);
                int savedMany = db.SaveChanges();

                dbFolderViewModel newDBFolder = new dbFolderViewModel(newFolder, false, db);
                parentFolder.Children.Add(newDBFolder);

                //parent.Children.Add(newDBItem);
                /*
                // Adjust base version, appropriately based on desired type 
                if (theType.Contains("Folder"))
                {
                    // Set FolderID based on the Parent
                    theTask.FolderID = ((dbFolderViewModel)parent)._folder.FolderID;
                    theTask.PSortOrder = theTask.PPartNum = newPPartNumForProj(theTask.FolderID);
                    dbProjectViewModel newDBItem = new dbProjectViewModel(theTask, (dbFolderViewModel)parent, false);
                    //item.DbID = ((dbProjectViewModel)item).ProjectName;
                    theDB.db.Projects.Add(theTask);
                    int savedMany = theDB.db.SaveChanges();

                    parent.Children.Add(newDBItem);
                }
                else if (theType.Contains("Project"))
                {
                    // Set FolderID based on the Parent

                    theTask.FolderID = ((dbProjectViewModel)parent)._project.FolderID;
                    // theTask.PSortOrder = theTask.PPartNum = ((dbProjectViewModel)parent)._project.PPartNum.Substring(0, 3) + "001000";
                    theTask.PSortOrder = theTask.PPartNum = newPPartNumForTask(theTask.FolderID, ((dbProjectViewModel)parent)._project.PPartNum.Substring(0, 3));
                    dbTaskViewModel newDBItem = new dbTaskViewModel(theTask, (dbProjectViewModel)parent, false);
                    //item.DbID = ((dbProjectViewModel)item).ProjectName;
                    theDB.db.Projects.Add(theTask);
                    int savedMany = theDB.db.SaveChanges();

                    parent.Children.Add(newDBItem);
                }
                else if (theType.Contains("SubTask"))
                {
                    // Set FolderID based on the Parent
                    theTask.FolderID = ((dbSubTaskViewModel)parent)._subTask.FolderID;
                    dbTaskViewModel theParentTask = ((dbTaskViewModel)((dbSubTaskViewModel)parent).Parent);
                    //theTask.PSortOrder = theTask.PPartNum = theParentTask._task.PPartNum.Substring(0, 6) + "000";
                    theTask.PSortOrder = theTask.PPartNum = newPPartNumForSubTask(theTask.FolderID, ((dbSubTaskViewModel)parent)._subTask.PPartNum.Substring(0, 6));
                    dbSubTaskViewModel newDBItem = new dbSubTaskViewModel(theTask, theParentTask, false);
                    //item.DbID = ((dbProjectViewModel)item).ProjectName;
                    theDB.db.Projects.Add(theTask);
                    int savedMany = theDB.db.SaveChanges();

                    //parent.Children.Add(newDBItem);
                    theParentTask.Children.Add(newDBItem);
                }
                else if (theType.Contains("Task"))
                {
                    // Set FolderID based on the Parent
                    theTask.FolderID = ((dbTaskViewModel)parent)._task.FolderID;
                    theTask.PSortOrder = theTask.PPartNum = newPPartNumForSubTask(theTask.FolderID, ((dbTaskViewModel)parent)._task.PPartNum.Substring(0, 6));
                    //((dbTaskViewModel)parent)._task.PPartNum.Substring(0, 6) + "000";

                    dbSubTaskViewModel newDBItem = new dbSubTaskViewModel(theTask, (dbTaskViewModel)parent, false);
                    //item.DbID = ((dbProjectViewModel)item).ProjectName;
                    theDB.db.Projects.Add(theTask);
                    int savedMany = theDB.db.SaveChanges();

                    parent.Children.Add(newDBItem);
                }
                */
            }
        }

        private string GetFPartNum(dbFolderViewModel parentFolder)
        {
            string fNum = parentFolder._folder.FPartNum;  // "PPPTTTSSS"
            string P = fNum.Substring(0, 3);
            string T = fNum.Substring(3, 3);
            string S = fNum.Substring(6, 3);
            int? theFolderID = parentFolder._folder.FolderID;
            string PartNum = "";
            // New folder is under the parentFolder
            // -------------------------------------
            string comparer = "";
            if (fNum == "000000000" )
            {
                // Parent is root, need a Project
                // PPP + delta_T
                TDTDbContext theDB = db;   // see Util.newPPartNum...
                //using (TDTDbContext theDB = new TDTDbContext())
                //{
                    string lastFolder = (from c in theDB.Folders
                                         where c.FPartNum.Substring(3, 6) == "000000"  // only want P
                                         orderby c.FPartNum descending
                                         select c.FPartNum.Substring(0, 3)).FirstOrDefault();

                    PartNum = Util.newNum(lastFolder) + "000000";
                    mLogger.AddLogMessage("NewFolder(P): " + PartNum + " - " + theFolderID);
                //}
            }
            else
            {


                if (T + S == "000000")
                {
                    // Parent is Project, need a Task

                    // PPP + delta_T
                    TDTDbContext theDB = db;   // see Util.newPPartNum...
                    //using (TDTDbContext theDB = new TDTDbContext())
                    //{
                        string lastFolder = (from c in theDB.Folders
                                             where c.FPartNum.Substring(6, 3) == "000"  // only want Tasks
                                             && c.FPartNum.Substring(0, 3) == P
                                             orderby c.FPartNum descending
                                             select c.FPartNum.Substring(0, 3)).FirstOrDefault();

                        PartNum = P + Util.newNum(lastFolder) + "000";
                        mLogger.AddLogMessage("NewFolder(T): " + PartNum + " - " + theFolderID);
                    //}
                }
                else
                {
                    //if (S == "000")
                    //{
                        // Parent is Task, need a SubTask
                        comparer = P;

                    // P + T + delta_S
                    TDTDbContext theDB = db;   // see Util.newPPartNum...
                    //using (TDTDbContext theDB = new TDTDbContext())
                        //{
                            string lastFolder = (from c in theDB.Folders
                                                 where c.FPartNum.Substring(0, 6) == P + T
                                                 orderby c.FPartNum descending
                                                 select c.FPartNum.Substring(6, 3)).FirstOrDefault();

                            PartNum = P + T + Util.newNum(lastFolder);
                            mLogger.AddLogMessage("NewFolder(S): " + PartNum + " - " + theFolderID);
                        //}
                    //}
                    //else
                    //{
                    //    // Parent is SubTask, again, need a SubTask
                    //    comparer = P + T;

                    //    //  "PPPTTT" + SSSmax
                    //    using (TDTDbContext theDB = new TDTDbContext())
                    //    {
                    //        string lastFolder = (from c in theDB.Folders
                    //                             where c.FPartNum.Substring(0, 6) == P + T //&&
                    //                                                                       //c.FPartNum.Substring(0, 3) == P
                    //                             orderby c.FPartNum descending
                    //                             select c.FPartNum.Substring(6, 3)).FirstOrDefault();

                    //        PartNum = P + T + Util.newNum(lastFolder);
                    //        mLogger.AddLogMessage("NewFolder(S): " + PartNum + " - " + theFolderID);
                    //    }
                    //}
                }
            }
        
            //using (TDTDbContext theDB = new TDTDbContext())
            //{
            //    int compLength = comparer.Length;
            //    string lastFolder = (from c in theDB.Folders
            //                         where c.FPartNum.Substring(compLength - 3, compLength) == "000000"
            //                          &&   c.FPartNum.Substring(0, 3) == Porderby c.FPartNum descending
            //                         select c.FPartNum.Substring(0, 3)).FirstOrDefault();

            //    PartNum = Util.newNum(lastFolder) + "000000";
            //    mLogger.AddLogMessage("NewFolder(P): " + PartNum + " - " + theFolderID);
            //}
            // -------------------------------------


            //if (fNum.Substring(6,3) == "000")
            //{
            //    // Not a S
            //    if (fNum.Substring(3, 3) == "000")
            //    {
            //        // Not a T, must be a P
            //        // PPPmax + "000000"
            //        using (TDTDbContext theDB = new TDTDbContext())
            //        {
            //            string lastFolder = (from c in theDB.Folders
            //                                 where c.FPartNum.Substring(3, 6) == "000000"
            //                                 orderby c.FPartNum descending
            //                                 select c.FPartNum.Substring(0, 3)).FirstOrDefault();

            //            PartNum = Util.newNum(lastFolder) + "000000";
            //            mLogger.AddLogMessage("NewFolder(P): " + PartNum + " - " + theFolderID);
            //        }
            //    }
            //    else
            //    {
            //     // Must be a T
            //     // "PPP" + TTTmax + "000"
            //        using (TDTDbContext theDB = new TDTDbContext())
            //        {
            //            string lastFolder = (from c in theDB.Folders
            //                                 where c.FPartNum.Substring(6, 3) == "000" &&
            //                                       c.FPartNum.Substring(0, 3) == P
            //                                 orderby c.FPartNum descending
            //                                 select c.FPartNum.Substring(3, 3)).FirstOrDefault();

            //            PartNum = P + Util.newNum(lastFolder) + "000";
            //            mLogger.AddLogMessage("NewFolder(T): " + PartNum + " - " + theFolderID);
            //        }
            //    }
            //}
            //else
            //{
            //    // Must be a S
            //    //  "PPPTTT" + SSSmax
            //    using (TDTDbContext theDB = new TDTDbContext())
            //    {
            //        string lastFolder = (from c in theDB.Folders
            //                             where c.FPartNum.Substring(0, 6) == P + T //&&
            //                                   //c.FPartNum.Substring(0, 3) == P
            //                             orderby c.FPartNum descending
            //                             select c.FPartNum.Substring(6, 3)).FirstOrDefault();

            //        PartNum = P + T + Util.newNum(lastFolder) ;
            //        mLogger.AddLogMessage("NewFolder(S): " + PartNum + " - " + theFolderID);
            //    }
            //}
            return PartNum;
            //throw new NotImplementedException();
        }


        //public string newPPartNumForProj(int theFolderID)
        //{
        //    /*
        //    var item = db.Items.OrderByDescending(i => i.Value).FirstOrDefault();
        //    IQueryable<Project> theProjs = (from c in db.Projects
        //                                    where c.FolderID == folder.FolderID &&
        //                                          c.PPartNum.Substring(3, 6) == "000000"
        //                                    orderby c.PPartNum
        //                                    select c);
        //    Project[] projects = theProjs.ToArray();
        //    return projects;
        //    */
        //    TDTDbContext theDB = new TDTDbContext();
        //    string theProjs = (from c in theDB.Projects
        //                                    where c.FolderID == theFolderID &&
        //                                          c.PPartNum.Substring(3, 6) == "000000"
        //                                    orderby c.PPartNum descending
        //                                    select c.PPartNum.Substring(0, 3)).FirstOrDefault();
        //    //Project[] projects = theProjs.ToArray();

        //    string PartNum = newNum(theProjs) + "000000";
        //    mLogger.AddLogMessage("NewProject: " + PartNum);
        //    return PartNum;
        //    //throw new NotImplementedException();
        //}

        //public string newNum(string lastNum)
        //{
        //    // Increment the old maximum and format with leading zeros
        //    int nNew = Convert.ToInt32(lastNum);
        //    nNew++;
        //    nNew = nNew + 1000;
        //    string newNumStr = nNew.ToString().Substring(1, 3);
        //    return newNumStr;
        //    //throw new NotImplementedException();
        //}

        //public string newPPartNumForTask(int theFolderID, string ProjNum)
        //{
        //    TDTDbContext theDB = new TDTDbContext();
        //    string theProjs = (from c in theDB.Projects
        //                       where c.FolderID == theFolderID &&
        //                             c.PPartNum.Substring(0, 3) == ProjNum
        //                       orderby c.PPartNum descending
        //                       select c.PPartNum.Substring(3, 3)).FirstOrDefault();

        //    //string PartNum = newNum(theProjs) + "000";
        //    string PartNum = ProjNum + newNum(theProjs) + "000";
        //    mLogger.AddLogMessage("New Task: " + PartNum + "  Input: " + ProjNum);
        //    return PartNum;
        //    //throw new NotImplementedException();
        //}
        //public string newPPartNumForSubTask(int theFolderID, string ProjNum)
        //{
        //    TDTDbContext theDB = new TDTDbContext();
        //    string theProjs = (from c in theDB.Projects
        //                       where c.FolderID == theFolderID &&
        //                             c.PPartNum.Substring(0, 6) == ProjNum
        //                       orderby c.PPartNum descending
        //                       select c.PPartNum.Substring(6, 3)).FirstOrDefault();

        //    //string PartNum = newNum(theProjs) + "000";
        //    string PartNum = ProjNum + newNum(theProjs) ;
        //    mLogger.AddLogMessage("New SubTask: " + PartNum + "  Input: " + ProjNum);


        //    // string PartNum = ProjNum + "001";
        //    return PartNum;
        //    //throw new NotImplementedException();
        //}
        public Project GetNewProject(string name)
        {
            Project newProj = new Project(name);
            newProj.FolderID = 9;
            //newProj.Folder = new Folder();
            newProj.PPartNum = "987000000";
            newProj.PSortOrder = "987000000";
            newProj.DetailedDesc = "Details. . .";
            newProj.Priority = "A";
            newProj.Status = "O";
            newProj.StartDate = DateTime.Today;
            newProj.DueDate = DateTime.Today.AddDays(5);
            newProj.RevDueDate = DateTime.Today.AddDays(5);
            newProj.DoneDate = null;  // DateTime.MinValue;
            newProj.RespPerson = "";// "Bill";
            newProj.Hide = false;
            newProj.DispLevel = "1";
            newProj.Done = false;
            //newProj = ;
            //newProj = ;
            //newProj = ;
            //throw new NotImplementedException();
            return newProj;
        }

        //PartNum = "001000000"
 

    }

}

