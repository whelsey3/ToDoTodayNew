using CodeFirst.EFcf;
using System;
using System.Linq;
using System.Windows.Controls;

namespace Planner
{
    /// <summary>
    /// Description for ProjectsView.
    /// </summary>
    public partial class Plans2View : UserControl
    {
       // LoggerLib.Logger mLogger;

        void AddingNewOne(TreeViewItemViewModel parent)
        {
            //get the processed item related to context menu
            ///TreeViewItemViewModel parent = GetCommandItem();

            // Get name for new item
            string name = ShowInputDialog(null);

            Database theDB = new Database();
            Project parentProj = theDB.db.Projects.Find(parent.DbID);

            // Get base version of Project
            Project newProj = GetNewProject(name);   // new Project(name);
            // Check type of Parent
            string theType = parent.GetType().ToString();
            // Could be F, P, T, S
            // new is   P, T, S, S  but really are all P objects
            mLogger.AddLogMessage("Adding New One ->" + name + " - " + theType);

            // Adjust base version, appropriately based on desired type 
            if (theType.Contains("Folder"))
            {
                // Set FolderID based on the Parent
                newProj.FolderID = ((dbFolderViewModel)parent)._folder.FolderID;
                newProj.PSortOrder = newProj.PPartNum = newPPartNumForProj(newProj.FolderID);
               // dbProjectViewModel newDBItem = new dbProjectViewModel(newProj, (dbFolderViewModel)parent, false);
                //item.DbID = ((dbProjectViewModel)item).ProjectName;
                theDB.db.Projects.Add(newProj);
                int savedMany = theDB.db.SaveChanges();
                dbProjectViewModel newDBItem = new dbProjectViewModel(newProj, (dbFolderViewModel)parent, false);

                parent.Children.Add(newDBItem);
            }
            else if (theType.Contains("Project"))
            {
                // Set FolderID based on the Parent

                newProj.FolderID = ((dbProjectViewModel)parent)._project.FolderID;
                // newProj.PSortOrder = newProj.PPartNum = ((dbProjectViewModel)parent)._project.PPartNum.Substring(0, 3) + "001000";
                newProj.PSortOrder = newProj.PPartNum = newPPartNumForTask(newProj.FolderID, ((dbProjectViewModel)parent)._project.PPartNum.Substring(0, 3));
                dbTaskViewModel newDBItem = new dbTaskViewModel(newProj, (dbProjectViewModel)parent, false);
                //item.DbID = ((dbProjectViewModel)item).ProjectName;
                theDB.db.Projects.Add(newProj);
                int savedMany = theDB.db.SaveChanges();
                mLogger.AddLogMessage("Added new task: " + newProj.Item);
       ////         dbTaskViewModel newDBItem = new dbTaskViewModel(newProj, (dbProjectViewModel)parent, false);
                parent.Children.Add(newDBItem);
            }
            else if (theType.Contains("SubTask"))
            {
                // Set FolderID based on the Parent
                newProj.FolderID = ((dbSubTaskViewModel)parent)._subTask.FolderID;
                dbTaskViewModel theParentTask = ((dbTaskViewModel)((dbSubTaskViewModel)parent).Parent);
                //newProj.PSortOrder = newProj.PPartNum = theParentTask._task.PPartNum.Substring(0, 6) + "000";
                newProj.PSortOrder = newProj.PPartNum = newPPartNumForSubTask(newProj.FolderID, ((dbSubTaskViewModel)parent)._subTask.PPartNum.Substring(0, 6));
                //dbSubTaskViewModel newDBItem = new dbSubTaskViewModel(newProj, theParentTask, false);
                //item.DbID = ((dbProjectViewModel)item).ProjectName;
                theDB.db.Projects.Add(newProj);
                int savedMany = theDB.db.SaveChanges();

                dbSubTaskViewModel newDBItem = new dbSubTaskViewModel(newProj, theParentTask, false);
                //parent.Children.Add(newDBItem);
                theParentTask.Children.Add(newDBItem);
            }
            else if (theType.Contains("Task"))
            {
                // Set FolderID based on the Parent
                newProj.FolderID = ((dbTaskViewModel)parent)._task.FolderID;
                newProj.PSortOrder = newProj.PPartNum = newPPartNumForSubTask(newProj.FolderID, ((dbTaskViewModel)parent)._task.PPartNum.Substring(0, 6));
                    //((dbTaskViewModel)parent)._task.PPartNum.Substring(0, 6) + "000";

            //    dbSubTaskViewModel newDBItem = new dbSubTaskViewModel(newProj, (dbTaskViewModel)parent, false);
                //item.DbID = ((dbProjectViewModel)item).ProjectName;
                theDB.db.Projects.Add(newProj);
                int savedMany = theDB.db.SaveChanges();

                dbSubTaskViewModel newDBItem = new dbSubTaskViewModel(newProj, (dbTaskViewModel)parent, false);
                parent.Children.Add(newDBItem);
            }
            // Need to get DbID set in newDBItem (after save to database)    **********
            int n = newProj.ProjectID;
        }

        void AddingNewFolder(dbFolderViewModel parentFolder)
        {
            //get the processed item related to context menu
            ///TreeViewItemViewModel parent = GetCommandItem();

            // Get name for new Folder
            string name = ShowInputDialog(null);

            Database theDB = new Database();

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

            // Get base version of Project
            //Project theTask = GetNewProject(name);   // new Project(name);
            // Check type of Parent
            //string theType = parent.GetType().ToString();
            // Could be F, P, T, S
            // new is   P, T, S, S  but really are all P objects
         //   mLogger.AddLogMessage("Adding New Folder ->" + name + " - " );

            theDB.db.Folders.Add(newFolder);
            int savedMany = theDB.db.SaveChanges();

            dbFolderViewModel newDBFolder = new dbFolderViewModel(newFolder, false);
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


        private string newPPartNumForProj(int theFolderID)
        {
            /*
            var item = db.Items.OrderByDescending(i => i.Value).FirstOrDefault();
            IQueryable<Project> theProjs = (from c in db.Projects
                                            where c.FolderID == folder.FolderID &&
                                                  c.PPartNum.Substring(3, 6) == "000000"
                                            orderby c.PPartNum
                                            select c);
            Project[] projects = theProjs.ToArray();
            return projects;
            */
            //TDTcfEntities theDB = new TDTcfEntities();
            TDTcfEntities theDB = (TDTcfEntities)App.Current.Resources["theData"]; 
            string theProjs = (from c in theDB.Projects
                                            where c.FolderID == theFolderID &&
                                                  c.PPartNum.Substring(3, 6) == "000000"
                                            orderby c.PPartNum descending
                                            select c.PPartNum.Substring(0, 3)).FirstOrDefault();
            //Project[] projects = theProjs.ToArray();

            string PartNum = newNum(theProjs) + "000000";
            mLogger.AddLogMessage("FolderID " + theFolderID + "  NewProject: " + PartNum);
            return PartNum;
            //throw new NotImplementedException();
        }

        private string newNum(string lastNum)
        {
            // Increment the old maximum and format with leading zeros
            int nNew = Convert.ToInt32(lastNum);
            nNew++;
            nNew = nNew + 1000;
            string newNumStr = nNew.ToString().Substring(1, 3);
            return newNumStr;
            //throw new NotImplementedException();
        }

        private string newPPartNumForTask(int theFolderID, string ProjNum)
        {
            TDTcfEntities theDB = (TDTcfEntities)App.Current.Resources["theData"];

            //TDTcfEntities theDB = new TDTcfEntities();
            string theProjs = (from c in theDB.Projects
                               where c.FolderID == theFolderID &&
                                     c.PPartNum.Substring(0, 3) == ProjNum
                               orderby c.PPartNum descending
                               select c.PPartNum.Substring(3, 3)).FirstOrDefault();

            //string PartNum = newNum(theProjs) + "000";
            string PartNum = ProjNum + newNum(theProjs) + "000";
            mLogger.AddLogMessage("New Task: " + PartNum + "  Input: " + ProjNum);
            return PartNum;
            //throw new NotImplementedException();
        }
        private string newPPartNumForSubTask(int theFolderID, string ProjNum)
        {
            //TDTcfEntities theDB = new TDTcfEntities();
            TDTcfEntities theDB = (TDTcfEntities)App.Current.Resources["theData"];
            string theProjs = (from c in theDB.Projects
                               where c.FolderID == theFolderID &&
                                     c.PPartNum.Substring(0, 6) == ProjNum
                               orderby c.PPartNum descending
                               select c.PPartNum.Substring(6, 3)).FirstOrDefault();

            //string PartNum = newNum(theProjs) + "000";
            string PartNum = ProjNum + newNum(theProjs) ;
            mLogger.AddLogMessage("New SubTask: " + PartNum + "  Input: " + ProjNum);


            // string PartNum = ProjNum + "001";
            return PartNum;
            //throw new NotImplementedException();
        }
        private Project GetNewProject(string name)
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
            newProj.RespPerson = "Bill";
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
