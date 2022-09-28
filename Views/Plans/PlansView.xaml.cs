using BuildSqliteCF.Entity;
//using BuildSqliteCF.DbContexts;
using System;
using nuT3;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Planner
{
    /// <summary>
    /// Description for ProjectsView.
    /// </summary>
    public partial class PlansView : UserControl
    {
        //readonly PlansViewModel _familyTree;
        Logger mLogger;
        TDTDbContext db;
        public PlansViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the ProjectsView class.
        /// </summary>
        public PlansView()
        {
            mLogger = LoggerLib.Logger.Instance;
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                string tt = e.InnerException.Message;
                mLogger.AddLogMessage("PlansView Startup Problem");
                mLogger.AddLogMessage(tt);
                throw;
            }
            //InitializeComponent();
            ////     // Get raw family tree data from a database.
            //            Database theDB = new Database();
            //            Folder[] folders = theDB.GetFolders();
            ////     TVItem rootPerson = DatabaseHC.GetFamilyTree();

            ////     // Create UI-friendly wrappers around the 
            ////     // raw data objects (i.e. the view-model).

            ////     _familyTree = new PlansViewModel(rootPerson);

            ////     // Let the UI bind to the view-model.
            ////     base.DataContext = _familyTree;

            ////     //string test = tv.SelectedItem.ToString();
            //////     ((TreeViewItem)tv.Items[0]).IsSelected = true;

            //tv.RootNode

            ////InitializeComponent();
            //aThrobber.t
            //ThrobberVisible  = Visibility.Visible;
            //            bool LazyLoading = false;
            ////Folder[] folders = DatabaseHC.GetFolders();
            //            PlansViewModel viewModel = new PlansViewModel(folders, LazyLoading);

            viewModel = new PlansViewModel();
          ///  db = PlansViewModel.db;
            db = ((App)Application.Current).db;   // new TDTDbContext();

            //   System.Collections.ObjectModel.ObservableCollection<dbFolderViewModel> saveIt = new System.Collections.ObjectModel.ObservableCollection<dbFolderViewModel>(viewModel.Folders);
            //   SaveData(saveIt);
            this.Name = "PlansViewName";
            this.DataContext = viewModel;
            //base.DataContext = viewModel;
            //Throbber.Visibility = Visibility
            //            var testing = PlansViewModel.db.Projects;
            //tv.RootNode = (TreeViewItem)((TreeViewItemViewModel)(viewModel.Folders[0]));
        }

        public void tv_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            object current;
            current = tv.SelectedItem;  // proper 
            mLogger.AddLogMessage("SelectedItemChanged-" + tv.SelectedItem.GetType());
            // ((TreeViewItem)tv.Items[0]).IsSelected = true;
            // ((TVItemViewModel)tv.Items[0]).IsExpanded = true;
            //  ((TVItemViewModel)tv.SelectedItem).IsExpanded = true;
        }

        private void tv_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = (TreeViewItem)SearchTreeView<TreeViewItem>((DependencyObject)e.OriginalSource);
            if (treeViewItem != null)
            {
                treeViewItem.IsSelected = true;
                e.Handled = true;
            }

        }

        private void OnItemSelected(object sender, System.Windows.RoutedEventArgs e)
        {
            var tvItem = tv.ItemContainerGenerator.ContainerFromItem(((TreeView)sender).SelectedItem);
            //tvItem.fo
            TreeViewItem item = (TreeViewItem)(tv.ItemContainerGenerator.ContainerFromItem(tv.SelectedItem));
            if (item != null)
            {
               //item.Focus();
            }
            ((PlansViewModel)DataContext).HandleOnItemSelected(sender, e);
           // HandleOnItemSelected(sender, e);
            //tv.Tag = e.OriginalSource;
            //mLogger.AddLogMessage("OnItemSelected-" + ((TreeViewItem)(tv.Tag)).Header.GetType());
            //Type theTypeType = ((TreeViewItem)(tv.Tag)).Header.GetType();
            //object item = ((TreeViewItem)(tv.Tag)).Header;
            //string theType = theTypeType.Name;
            //string tbName = "";
            //; Project headProj = null;

            //if (theType.Contains("Folder"))
            //{
            //    // Don't delete Folders
            //}
            //else if (theType.Contains("Project"))
            //{
            //    // Database theDB = new Database();
            //    headProj = ((dbProjectViewModel)item)._project;
            //    tbName = ((dbProjectViewModel)item).ProjectName;
            //    //headID = headProj.ProjectID;
            //    //srchString = headProj.PPartNum.Substring(0, 3);// + "000000";

            //    ((PlansViewModel)DataContext).Details.Item = tbName;

            //}
            //else if (theType.Contains("SubTask"))
            //{
            //    //Database theDB = new Database();
            //    headProj = ((dbSubTaskViewModel)item)._subTask;
            //    tbName = ((dbSubTaskViewModel)item).SubTaskName;
            //    //headID = headProj.ProjectID;
            //    // srchString = headProj.PPartNum.Substring(0, 3);// + "";

            //}
            //else if (theType.Contains("Task"))
            //{
            //    //Database theDB = new Database();
            //    headProj = ((dbTaskViewModel)item)._task;
            //    tbName = ((dbTaskViewModel)item).TaskName;
            //    //headID = headProj.ProjectID;
            //    //srchString = headProj.PPartNum.Substring(0, 6);// + "000";

            //}

            ////  this.theItem.Text = tbName;




        }

        private static DependencyObject SearchTreeView<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
            {
                source = VisualTreeHelper.GetParent(source);
            }
            return source;
            //throw new NotImplementedException();
        }

        ///// <summary>
        ///// Displays an input dialog and returns the entered
        ///// value.
        ///// </summary>
        //private string ShowInputDialog(string defaultValue)
        //{
        //    InputDialog dlg = new InputDialog();
        //    dlg.CategoryName = defaultValue;
        //    Window parentWindow = Window.GetWindow(this);
        //    dlg.Owner = parentWindow;  // (Window)(this.Parent);
        //                               // dlg.DialogResult = false;
        //    dlg.ShowDialog();
        //    if (dlg.DialogResult == true)
        //    {
        //        if (dlg.CategoryName.Length != 0)
        //        {
        //            // User clicked OK and entered a name
        //        }
        //        else
        //        {
        //            // Clicked OK but name is blank, really not needed
        //            //    OK button not enabled unless name is NOT blank, see XAML
        //            dlg.CategoryName = "ZPQ";
        //        }
        //    }
        //    else
        //    {
        //        dlg.CategoryName = "ZPQ";
        //    }
        //    return dlg.CategoryName;
        //}

        #region context menu command handling


        /// <summary>
        /// Checks whether it is allowed to delete a category, which is only
        /// allowed for nested categories, but not the root items.
        /// </summary>
        public void EvaluateCanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            //attention: this method is invoked all the time - also if the tree's context
            //menu has just been removed. if the latter is the case, GetCommandItem()
            //would fail, as it works on the context menu

            if (tv.ContextMenu != null)
            {
                //get the processed item
                TreeViewItemViewModel item = GetCommandItem();
                // This applies to work items!
                string theType = item.GetType().ToString();
                //if (theType.Contains("Folder"))
                if (!theType.Contains("Folder") && ((item.cStatus.Trim()).Length == 0 || item.cStatus == "F" || item.cStatus != "D"))
                {
                    e.CanExecute = true;
                  //  viewModel.ShowUserMessage("Admin required to delete folder.");
                }
                else
                {
                    e.CanExecute = false;
                    e.CanExecute = item.Parent != null;
                }

            }
            else
            {
                //we just removed our context menu assignment, while the menu is open...
                e.CanExecute = false;
            }

            e.Handled = true;
        }

        public void EvaluateCanDeleteFolder(object sender, CanExecuteRoutedEventArgs e)
        {
            //attention: this method is invoked all the time - also if the tree's context

            if (tv.ContextMenu != null)
            {
                //get the processed item
                TreeViewItemViewModel item = GetCommandItem();
                // This applies to work items!
                string theType = item.GetType().ToString();
                if (!theType.Contains("Folder") && item.cStatus != "O" && item.cStatus != "A" && item.cStatus != "W")
                {
                    //e.CanExecute = true;
                    e.CanExecute = item.Parent != null;  // Don't delete root folder
                    // viewModel.ShowUserMessage("Admin NOT required to delete folder.");
                }
                else
                {
                    e.CanExecute = false;
                }
            }
            else
            {
                //we just removed our context menu assignment, while the menu is open...
                e.CanExecute = false;
            }

            e.Handled = true;
        }

        /// <summary>
        /// Deletes the currently processed item. This can be a right-clicked
        /// item (context menu) or the currently selected item, if the user
        /// pressed delete.
        /// </summary>
        private void DeleteOne(object sender, ExecutedRoutedEventArgs e)
        {
            // get item, should be a Project data type
            //   Folders have been excluded
            TreeViewItemViewModel item = GetCommandItem();
            string theType = item.GetType().ToString();

            int theFolder = 0;   // FolderID of the selected item
            //  List of Projects to be delelted
            //  List of ToDos to be deleted
            //  Delete ToDos
            //  Delete Projects
            DeleteSet(item);

            //  ProcessChildren(item.Children);

            //// Adjust base version, appropriately based on desired type
         //   System.Collections.Generic.List<Project> projDelList = new List<Project>();

            //if (theType.Contains("Folder"))
            //{
                // Every prj with the FolderID should be removed
                //theFolder = ((dbFolderViewModel)item)._folder.FolderID;

                //using (TDTDbContext theDB = new TDTDbContext())
                //{
                //    projDelList = (from c in theDB.Projects.AsEnumerable()
                //                         where c.FolderID == theFolder
                //                         //c.FPartNum.Substring(0, 6) == P + T
                //                         //orderby c.FPartNum descending
                //                         select c).ToList();

                    //PartNum = P + T + Util.newNum(lastFolder);
                    //mLogger.AddLogMessage("NewFolder(S): " + PartNum + " - " + theFolderID);
             //   }

                //theTask.PSortOrder = theTask.PPartNum = "001000000";
                //dbProjectViewModel newDBItem = new dbProjectViewModel(theTask, (dbFolderViewModel)parent, false);
                ////item.DbID = ((dbProjectViewModel)item).ProjectName;
                //theDB.db.Projects.Add(theTask);
                //int savedMany = theDB.db.SaveChanges();

            //}
            //else if (theType.Contains("Project"))
            //{
            //}
            //else if (theType.Contains("SubTask"))
            //{
            //}


            //else if (theType.Contains("Task"))
            //{
            //}
                //else if (theType.Contains("Project"))
                //{
                //    Database theDB = new Database();
                //    Project proj = ((dbProjectViewModel)item)._project;
                //    Project delProj = theDB.db.Projects.Find(proj.ProjectID);
                //    theDB.db.Projects.Remove(delProj);
                //    int savedMany = theDB.db.SaveChanges();


                //    //// Set FolderID based on the Parent

                //    //theTask.FolderID = ((dbProjectViewModel)parent)._project.FolderID;
                //    //theTask.PSortOrder = theTask.PPartNum = ((dbProjectViewModel)parent)._project.PPartNum.Substring(0, 3) + "001000";
                //    //dbTaskViewModel newDBItem = new dbTaskViewModel(theTask, (dbProjectViewModel)parent, false);
                //    ////item.DbID = ((dbProjectViewModel)item).ProjectName;
                //    //theDB.db.Projects.Add(theTask);
                //    //int savedMany = theDB.db.SaveChanges();

                //    //parent.Children.Add(newDBItem);
                //}
                //else if (theType.Contains("SubTask"))
                //{
                //    Database theDB = new Database();
                //    Project subTsk = ((dbSubTaskViewModel)item)._subTask;
                //    Project delTsk = theDB.db.Projects.Find(subTsk.ProjectID);
                //    theDB.db.Projects.Remove(delTsk);
                //    int savedMany = theDB.db.SaveChanges();


                //    //// Set FolderID based on the Parent
                //    //theTask.FolderID = ((dbSubTaskViewModel)parent)._subTask.FolderID;
                //    //dbTaskViewModel theParentTask = ((dbTaskViewModel)((dbSubTaskViewModel)parent).Parent);
                //    //theTask.PSortOrder = theTask.PPartNum = theParentTask._task.PPartNum.Substring(0, 6) + "000";
                //    //dbSubTaskViewModel newDBItem = new dbSubTaskViewModel(theTask, theParentTask, false);
                //    ////item.DbID = ((dbProjectViewModel)item).ProjectName;
                //    //theDB.db.Projects.Add(theTask);
                //    //int savedMany = theDB.db.SaveChanges();

                //    //parent.Children.Add(newDBItem);
                //}
                //else if (theType.Contains("Task"))
                //{
                //    Database theDB = new Database();
                //    Project tsk = ((dbTaskViewModel)item)._task;
                //    Project delTsk = theDB.db.Projects.Find(tsk.ProjectID);
                //    theDB.db.Projects.Remove(delTsk);
                //    int savedMany = theDB.db.SaveChanges();

                //}

                ////remove from parent in the UI
                //mLogger.AddLogMessage("Removing from parent, " + item.DbID);
                //item.Parent.Children.Remove(item);

                //string theType = item.GetType().ToString();
                ////   dbTaskViewModel newDBItem = null;
                //if (theType.Contains("Task"))
                //{
                //    Database theDB = new Database();
                //    Project tsk = ((dbTaskViewModel)item)._task;
                //    Project delTsk = theDB.db.Projects.Find(tsk.ProjectID);
                //    theDB.db.Projects.Remove(delTsk);
                //    int savedMany = theDB.db.SaveChanges();

                //}

                //item.
                //db.ToDos.Remove(SelectedFolder.TheEntity);


                //mark event as handled
                e.Handled = true;
        }

        private void DeleteFolder(object sender, ExecutedRoutedEventArgs e)
        {
            //get item
            TreeViewItemViewModel item = GetCommandItem();
            string theType = item.GetType().ToString();   // should be a Folder

            //remove from parent in the UI (effectivey set by PPartNum)
            mLogger.AddLogMessage("Removing from parent, " + item.DbID);
            item.Parent.Children.Remove(item);

            int theFolderID = Int32.Parse(item.DbID.Substring(1));
            Folder targetFolder = db.Folders.Find(theFolderID);

        //    db.Folders.Remove(targetFolder);
            //  Assumes no child folder, always remove folders from innermost
            System.Collections.Generic.List<Project> targetFolderChildren = new List<Project>();
            targetFolderChildren = (from c in db.Projects
                                where (c.FolderID == theFolderID )
                                select c).ToList();

            db.Projects.RemoveRange(targetFolderChildren);

            db.Folders.Remove(targetFolder);

            // Remove
            // DeleteSet(item);
            //  ProcessChildren(item.Children);

            //// Adjust base version, appropriately based on desired type 
            //if (theType.Contains("Folder"))
            //{
            //    // Don't delete Folders

            //    //// Set FolderID based on the Parent
            //    //theTask.FolderID = ((dbFolderViewModel)parent)._folder.FolderID;
            //    //theTask.PSortOrder = theTask.PPartNum = "001000000";
            //    //dbProjectViewModel newDBItem = new dbProjectViewModel(theTask, (dbFolderViewModel)parent, false);
            //    ////item.DbID = ((dbProjectViewModel)item).ProjectName;
            //    //theDB.db.Projects.Add(theTask);
            //    //int savedMany = theDB.db.SaveChanges();

            //}
            //else if (theType.Contains("Project"))
            //{
            //    Database theDB = new Database();
            //    Project proj = ((dbProjectViewModel)item)._project;
            //    Project delProj = theDB.db.Projects.Find(proj.ProjectID);
            //    theDB.db.Projects.Remove(delProj);
            //    int savedMany = theDB.db.SaveChanges();


            //    //// Set FolderID based on the Parent

            //    //theTask.FolderID = ((dbProjectViewModel)parent)._project.FolderID;
            //    //theTask.PSortOrder = theTask.PPartNum = ((dbProjectViewModel)parent)._project.PPartNum.Substring(0, 3) + "001000";
            //    //dbTaskViewModel newDBItem = new dbTaskViewModel(theTask, (dbProjectViewModel)parent, false);
            //    ////item.DbID = ((dbProjectViewModel)item).ProjectName;
            //    //theDB.db.Projects.Add(theTask);
            //    //int savedMany = theDB.db.SaveChanges();

            //    //parent.Children.Add(newDBItem);
            //}
            //else if (theType.Contains("SubTask"))
            //{
            //    Database theDB = new Database();
            //    Project subTsk = ((dbSubTaskViewModel)item)._subTask;
            //    Project delTsk = theDB.db.Projects.Find(subTsk.ProjectID);
            //    theDB.db.Projects.Remove(delTsk);
            //    int savedMany = theDB.db.SaveChanges();


            //    //// Set FolderID based on the Parent
            //    //theTask.FolderID = ((dbSubTaskViewModel)parent)._subTask.FolderID;
            //    //dbTaskViewModel theParentTask = ((dbTaskViewModel)((dbSubTaskViewModel)parent).Parent);
            //    //theTask.PSortOrder = theTask.PPartNum = theParentTask._task.PPartNum.Substring(0, 6) + "000";
            //    //dbSubTaskViewModel newDBItem = new dbSubTaskViewModel(theTask, theParentTask, false);
            //    ////item.DbID = ((dbProjectViewModel)item).ProjectName;
            //    //theDB.db.Projects.Add(theTask);
            //    //int savedMany = theDB.db.SaveChanges();

            //    //parent.Children.Add(newDBItem);
            //}
            //else if (theType.Contains("Task"))
            //{
            //    Database theDB = new Database();
            //    Project tsk = ((dbTaskViewModel)item)._task;
            //    Project delTsk = theDB.db.Projects.Find(tsk.ProjectID);
            //    theDB.db.Projects.Remove(delTsk);
            //    int savedMany = theDB.db.SaveChanges();

            //}

            ////remove from parent in the UI
            //mLogger.AddLogMessage("Removing from parent, " + item.DbID);
            //item.Parent.Children.Remove(item);

            //string theType = item.GetType().ToString();
            ////   dbTaskViewModel newDBItem = null;
            //if (theType.Contains("Task"))
            //{
            //    Database theDB = new Database();
            //    Project tsk = ((dbTaskViewModel)item)._task;
            //    Project delTsk = theDB.db.Projects.Find(tsk.ProjectID);
            //    theDB.db.Projects.Remove(delTsk);
            //    int savedMany = theDB.db.SaveChanges();

            //}

            //item.
            //db.ToDos.Remove(SelectedFolder.TheEntity);

            int deletedMany = db.SaveChanges();
            mLogger.AddLogMessage("DeleteFolder removed " + deletedMany);
            //mark event as handled
            e.Handled = true;
        }


        private void DeleteSet(TreeViewItemViewModel item)
        {
            //remove from parent in the UI (effectivey set by PPartNum)
            mLogger.AddLogMessage("Deleting removing from parent, " + item.DbID);
            item.Parent.Children.Remove(item);

            // First must delete any ToDos and Tracks
            //  Start with list of ProjectIDs

            System.Collections.Generic.List<int> projDelList = new List<int>();
            Project headProj = null;
          //  int headID = 0;  // ID of top item to be deleted; remember everything not folder is project.
            string srchString = "";
            string theType = item.GetType().ToString();
            //           Database theDB = new Database();
            if (theType.Contains("Folder"))
            {
                // Don't delete Folders
            }
            else if (theType.Contains("Project"))
            {
                headProj = ((dbProjectViewModel)item)._project;
                //headID = headProj.ProjectID;
                srchString = headProj.PPartNum.Substring(0, 3);// + "000000";

            }
            else if (theType.Contains("SubTask"))
            {
                headProj = ((dbSubTaskViewModel)item)._subTask;
                //headID = headProj.ProjectID;
                srchString = headProj.PPartNum.Substring(0, 9);// + "";

            }
            else if (theType.Contains("Task"))
            {
                headProj = ((dbTaskViewModel)item)._task;
                //headID = headProj.ProjectID;
                srchString = headProj.PPartNum.Substring(0, 6);// + "000";

            }
            mLogger.AddLogMessage("headProj: '" + headProj.Item + "' Search: '" + srchString + "'");

            System.Collections.Generic.List<Project> projToDelete = new List<Project>();
            projToDelete = (from c in db.Projects
                            where (c.PPartNum.Substring(0, srchString.Length) == srchString
                            &&
                            c.FolderID == headProj.FolderID)
                            orderby c.PPartNum descending
                            select c).ToList();

            System.Collections.Generic.List<ToDo> delToDoBatch = new List<ToDo>();
            //delToDoBatch = (from c in db.ToDos where c.ProjectID == theProjectID select c).ToList();
            System.Collections.Generic.List<ToDo> interimDelToDoBatch = new List<ToDo>();
            System.Collections.Generic.List<Track> interimDelTrackBatch = new List<Track>();
            int NumToDos = 0;
            int NumTracks = 0;
            foreach (Project proj in projToDelete)
            {
                interimDelToDoBatch = new List<ToDo>();
                interimDelToDoBatch = (from c in db.ToDos where c.ProjectID == proj.ProjectID select c).ToList();
                NumToDos = NumToDos + interimDelToDoBatch.Count;
                mLogger.AddLogMessage("Checking for ToDos with ProjectID: " + proj.Item + "-" + proj.ProjectID + "-" + proj.PPartNum + " -> " + interimDelToDoBatch.Count);

                foreach (ToDo todo in interimDelToDoBatch)
                {
                   // delToDoBatch.Add(todo);
                    mLogger.AddLogMessage("db.ToDos.Remove--ToDo " + todo.Item + "-" + todo.ToDoID + "-" + todo.ProjectID);
                    db.ToDos.Remove(todo);
                }


                interimDelTrackBatch = new List<Track>();
                interimDelTrackBatch = (from c in db.Tracks where c.ProjectID == proj.ProjectID select c).ToList();
                NumTracks = NumTracks + interimDelTrackBatch.Count;
                mLogger.AddLogMessage("Checking for Tracks with ProjectID: " + proj.Item + "-" + proj.ProjectID + "-" + proj.PPartNum + " -> " + interimDelTrackBatch.Count);

                foreach (Track Track in interimDelTrackBatch)
                {
                    // delTrackBatch.Add(Track);
                    mLogger.AddLogMessage("db.Tracks.Remove--Track " + Track.Item + "-" + Track.TrackID + "-" + Track.ProjectID);
                    db.Tracks.Remove(Track);
                }

                mLogger.AddLogMessage("--Planning Delete: " + projToDelete.Count + "-" + NumToDos + "-" + NumTracks + " ====>" );
                //mLogger.AddLogMessage("+++=");
                //DeleteToDo(db, proj.ProjectID);  // delete any ToDo for that project
                //mLogger.AddLogMessage("Deleting project->" + proj.Item + "--" + proj.PPartNum);
                //db.Projects.Remove(proj);
            }

            ((PlansViewModel)this.DataContext).UpdateDB();

         //   UpdateDB(db);
            foreach (Project proj in projToDelete)
            {
                mLogger.AddLogMessage("db.Projects.Remove--Deleting project->" + proj.Item + "--" + proj.PPartNum);
                db.Projects.Remove(proj);
            }

      //      int NumChanges = db.SaveChanges();
      //      mLogger.AddLogMessage("NumChanges saved in projToDelete was " + NumChanges);
      ((PlansViewModel)this.DataContext).UpdateDB();
      //      UpdateDB(db);

            //}




            //System.Collections.Generic.List<int> projDelList = new List<int>();
            //Project headProj = null;
            ////int headID = 0;  // ID of top item to be deleted; remember everything not folder is project.
            //string srchString = "";
            //string theType = item.GetType().ToString();
            ////           Database theDB = new Database();
            //if (theType.Contains("Folder"))
            //{
            //    // Don't delete Folders
            //}
            //else if (theType.Contains("Project"))
            //{
            //    // Database theDB = new Database();
            //    headProj = ((dbProjectViewModel)item)._project;
            //    //headID = headProj.ProjectID;
            //    srchString = headProj.PPartNum.Substring(0, 3);// + "000000";

            //    //Project delProj = theDB.db.Projects.Find(proj.ProjectID);
            //    //theDB.db.Projects.Remove(delProj);
            //    //int savedMany = theDB.db.SaveChanges();


            //    //// Set FolderID based on the Parent

            //    //theTask.FolderID = ((dbProjectViewModel)parent)._project.FolderID;
            //    //theTask.PSortOrder = theTask.PPartNum = ((dbProjectViewModel)parent)._project.PPartNum.Substring(0, 3) + "001000";
            //    //dbTaskViewModel newDBItem = new dbTaskViewModel(theTask, (dbProjectViewModel)parent, false);
            //    ////item.DbID = ((dbProjectViewModel)item).ProjectName;
            //    //theDB.db.Projects.Add(theTask);
            //    //int savedMany = theDB.db.SaveChanges();

            //    //parent.Children.Add(newDBItem);
            //}
            //else if (theType.Contains("SubTask"))
            //{
            //    //Database theDB = new Database();
            //    headProj = ((dbSubTaskViewModel)item)._subTask;
            //    //headID = headProj.ProjectID;
            //    srchString = headProj.PPartNum.Substring(0, 3);// + "";

            //    //Project delTsk = theDB.db.Projects.Find(subTsk.ProjectID);
            //    //theDB.db.Projects.Remove(delTsk);
            //    //int savedMany = theDB.db.SaveChanges();


            //    //// Set FolderID based on the Parent
            //    //theTask.FolderID = ((dbSubTaskViewModel)parent)._subTask.FolderID;
            //    //dbTaskViewModel theParentTask = ((dbTaskViewModel)((dbSubTaskViewModel)parent).Parent);
            //    //theTask.PSortOrder = theTask.PPartNum = theParentTask._task.PPartNum.Substring(0, 6) + "000";
            //    //dbSubTaskViewModel newDBItem = new dbSubTaskViewModel(theTask, theParentTask, false);
            //    ////item.DbID = ((dbProjectViewModel)item).ProjectName;
            //    //theDB.db.Projects.Add(theTask);
            //    //int savedMany = theDB.db.SaveChanges();

            //    //parent.Children.Add(newDBItem);
            //}
            //else if (theType.Contains("Task"))
            //{
            //    //Database theDB = new Database();
            //    headProj = ((dbTaskViewModel)item)._task;
            //    //headID = headProj.ProjectID;
            //    srchString = headProj.PPartNum.Substring(0, 6);// + "000";

            //    //Project delTsk = theDB.db.Projects.Find(tsk.ProjectID);
            //    //theDB.db.Projects.Remove(delTsk);
            //    //int savedMany = theDB.db.SaveChanges();

            //}
            //headProj = theDB.db.Projects.Find(projDelList[0]);
            //int headID = headProj.ProjectID;
            //projDelList.Add(headID);
            //// Need search string to get children

            //System.Collections.Generic.List<Project> sourceBatch2 = new List<Project>();
            //sourceBatch2 = (from c in db.Projects
            //                where (c.PPartNum.Substring(0, srchString.Length) == srchString
            //                &&
            //                c.FolderID == headProj.FolderID)
            //                orderby c.PPartNum descending
            //                select c).ToList();

            //foreach (Project proj in projToDelete)
            //{
            //    mLogger.AddLogMessage("+++=");
            //    DeleteToDo(db, proj.ProjectID);  // delete any ToDo for that project
            //    mLogger.AddLogMessage("Deleting project->" + proj.Item + "--" + proj.PPartNum);
            //    db.Projects.Remove(proj);
            //}

            ////throw new NotImplementedException();

            //int NumChanges = db.SaveChanges();
            //mLogger.AddLogMessage("NumChanges saved in DeleteSet was " + NumChanges);
            //UpdateDB(db);

        }

        private void DeleteToDo(TDTDbContext db, int theProjectID)
        {
            System.Collections.Generic.List<ToDo> delToDoBatch = new List<ToDo>();
            delToDoBatch = (from c in db.ToDos where c.ProjectID == theProjectID select c).ToList();

            if (delToDoBatch.Count != 0)
            {
                foreach (ToDo todo in delToDoBatch)
                {
                    mLogger.AddLogMessage("Deleting todo->" + todo.Item + "-" + todo.ToDoID + "--" + todo.ProjectID);
                    db.ToDos.Remove(todo);
                }

            }
            else
            {
                mLogger.AddLogMessage("No ToDos for " + theProjectID);
            }
            int NumChanges = db.SaveChanges();
            mLogger.AddLogMessage("NumChanges saved in DeleteToDo was " + NumChanges);

        }

        private void ProcessChildren(ObservableCollection<TreeViewItemViewModel> children)
        {
            foreach (var item in children)
            {
                var x = item.DbID;
                //string grpID = GetParentID(item._project);
            }
            //string[] projList = (from c in children select pId).ToArray(); 
            //throw new NotImplementedException();
        }

        //public string GetParentID(string FID)
        //{
        //    string ParentID = "";
        //    //string FID = aFolder._folder.FSortOrder;
        //    if (FID == "000000000")
        //    {
        //        // this is root, assume always there
        //        ParentID = "";
        //    }
        //    else if (FID.Substring(3, 6) == "000000")
        //    {
        //        // this is project level, parent must be root
        //        ParentID = "000000000";
        //    }
        //    else if (FID.Substring(6, 3) == "000")
        //    {
        //        // this is task level
        //        ParentID = FID.Substring(0, 3) + "000000";
        //    }
        //    else
        //    {
        //        // Must be subtask
        //        ParentID = FID.Substring(0, 6) + "000";
        //    }
        //    mLogger.AddLogMessage("GetParentID:  " + FID + " - ParentID->" + ParentID);
        //    return ParentID;
        //}


        private void EvaluateAddFolder(object sender, CanExecuteRoutedEventArgs e)
        {
            //attention: this method is invoked all the time - also if the tree's context
            //menu has just been removed. if the latter is the case, GetCommandItem()
            //would fail, as it works on the context menu

            if (tv.ContextMenu != null)
            {
                //get the processed item
                TreeViewItemViewModel item = GetCommandItem();
                string theType = item.GetType().ToString();
                // Only add Folder to Folder
                if (theType.Contains("Folder"))
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
            else
            {
                //we just removed our context menu assignment, while the menu is open...
                e.CanExecute = false;
            }

            e.Handled = true;
        }
        private void EvaluateAddToDoTDT(object sender, CanExecuteRoutedEventArgs e)
        {
            //attention: this method is invoked all the time - also if the tree's context
         
            if (tv.ContextMenu != null)
            {
                //get the processed item
                TreeViewItemViewModel item = GetCommandItem();
                string theType = item.GetType().ToString();
                // Anything except Folder
               // if (!theType.Contains("Folder") && (item.cStatus == " " || item.cStatus == "F" || item.cStatus != "D"))
                if (!theType.Contains("Folder") && item.cStatus != "O" && item.cStatus != "A" && item.cStatus != "W")
                {
                    e.CanExecute = true;
                    mLogger.AddLogMessage("EvaluateAddToDoTDT  CanExecute TRUE " + item.cStatus + " type=" + theType);
                }
                else
                {
                    e.CanExecute = false;
                    mLogger.AddLogMessage("EvaluateAddToDoTDT  CanExecute FALSE " + item.cStatus + " type=" + theType);
                }
            }
            else
            {
                //we just removed our context menu assignment, while the menu is open...
                e.CanExecute = false;
            }

            e.Handled = true;
        }

        private void AddFolder(object sender, ExecutedRoutedEventArgs e)
        {
            //get item
            TreeViewItemViewModel item = GetCommandItem();
            string theType = item.GetType().ToString();

            // Adjust base version, appropriately based on desired type 
            if (theType.Contains("Folder"))
            {
                // Normal path, should have a Folder or this won't be executed
                dbFolderViewModel parentFolder = ((dbFolderViewModel)item);
                AddingNewFolder(parentFolder);

                // Don't delete Folders

                //// Set FolderID based on the Parent
                //theTask.FolderID = ((dbFolderViewModel)parent)._folder.FolderID;
                //theTask.PSortOrder = theTask.PPartNum = "001000000";
                //dbProjectViewModel newDBItem = new dbProjectViewModel(theTask, (dbFolderViewModel)parent, false);
                ////item.DbID = ((dbProjectViewModel)item).ProjectName;
                //theDB.db.Projects.Add(theTask);
                //int savedMany = theDB.db.SaveChanges();

            }
            else if (theType.Contains("Project"))
            {
                //Database theDB = new Database();
                //Project proj = ((dbProjectViewModel)item)._project;
                //Project delProj = theDB.db.Projects.Find(proj.ProjectID);
                //theDB.db.Projects.Remove(delProj);
                //int savedMany = theDB.db.SaveChanges();


                //// Set FolderID based on the Parent

                //theTask.FolderID = ((dbProjectViewModel)parent)._project.FolderID;
                //theTask.PSortOrder = theTask.PPartNum = ((dbProjectViewModel)parent)._project.PPartNum.Substring(0, 3) + "001000";
                //dbTaskViewModel newDBItem = new dbTaskViewModel(theTask, (dbProjectViewModel)parent, false);
                ////item.DbID = ((dbProjectViewModel)item).ProjectName;
                //theDB.db.Projects.Add(theTask);
                //int savedMany = theDB.db.SaveChanges();

                //parent.Children.Add(newDBItem);
            }
            else if (theType.Contains("SubTask"))
            {
                //Database theDB = new Database();
                //Project subTsk = ((dbSubTaskViewModel)item)._subTask;
                //Project delTsk = theDB.db.Projects.Find(subTsk.ProjectID);
                //theDB.db.Projects.Remove(delTsk);
                //int savedMany = theDB.db.SaveChanges();


                //// Set FolderID based on the Parent
                //theTask.FolderID = ((dbSubTaskViewModel)parent)._subTask.FolderID;
                //dbTaskViewModel theParentTask = ((dbTaskViewModel)((dbSubTaskViewModel)parent).Parent);
                //theTask.PSortOrder = theTask.PPartNum = theParentTask._task.PPartNum.Substring(0, 6) + "000";
                //dbSubTaskViewModel newDBItem = new dbSubTaskViewModel(theTask, theParentTask, false);
                ////item.DbID = ((dbProjectViewModel)item).ProjectName;
                //theDB.db.Projects.Add(theTask);
                //int savedMany = theDB.db.SaveChanges();

                //parent.Children.Add(newDBItem);
            }
            else if (theType.Contains("Task"))
            {
                //Database theDB = new Database();
                //Project tsk = ((dbTaskViewModel)item)._task;
                //Project delTsk = theDB.db.Projects.Find(tsk.ProjectID);
                //theDB.db.Projects.Remove(delTsk);
                //int savedMany = theDB.db.SaveChanges();

            }

            //remove from parent in the UI
            //      item.Parent.Children.Remove(item);

            //string theType = item.GetType().ToString();
            ////   dbTaskViewModel newDBItem = null;
            //if (theType.Contains("Task"))
            //{
            //    Database theDB = new Database();
            //    Project tsk = ((dbTaskViewModel)item)._task;
            //    Project delTsk = theDB.db.Projects.Find(tsk.ProjectID);
            //    theDB.db.Projects.Remove(delTsk);
            //    int savedMany = theDB.db.SaveChanges();

            //}

            //item.
            //db.ToDos.Remove(SelectedFolder.TheEntity);


            //mark event as handled
            e.Handled = true;
        }

        /// <summary>
        /// Determines the item that is the source of a given command.
        /// As a command event can be routed from a context menu click
        /// or a short-cut, we have to evaluate both possibilities.
        /// </summary>
        /// <returns></returns>
        public TreeViewItemViewModel GetCommandItem()
        {
            //get the processed item
            ContextMenu menu = tv.ContextMenu;
            if (menu.IsVisible)
            {
                //a context menu was clicked
                //TreeViewItem treeNode = (TreeViewItem)menu.PlacementTarget;
                //return (TreeViewItemViewModel)treeNode.Header;
                return (TreeViewItemViewModel)tv.SelectedItem;
            }
            else
            {
                //the context menu is closed - the user has pressed a shortcut
                return (TreeViewItemViewModel)tv.SelectedItem;
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void AddToDoTDT(object sender, ExecutedRoutedEventArgs e)
        {
            //get item
            TreeViewItemViewModel item = GetCommandItem();
            string theType = item.GetType().ToString();

            //           Database theDB = new Database();
            Project newToDoAdd = db.Projects.Find(Convert.ToInt32(item.DbID));
            mLogger.AddLogMessage("--- AddToDoTDT->" + newToDoAdd.Item + "-" + newToDoAdd.ProjectID);
            ToDo newToDo = new ToDo();
            newToDo.Item = newToDoAdd.Item;
            newToDo.DetailedDesc = newToDoAdd.DetailedDesc;
            newToDo.DispLevel = newToDoAdd.DispLevel;
            newToDo.Done = newToDoAdd.Done;
            newToDo.StartDate = DateTime.Now; // newToDoAdd.StartDate;
            newToDo.DoneDate = null;  // newToDoAdd.DoneDate;
            newToDo.DueDate = newToDoAdd.DueDate;
            newToDo.RevDueDate = newToDoAdd.RevDueDate;
            newToDo.Priority = newToDoAdd.Priority;
            newToDo.ProjectID = newToDoAdd.ProjectID;
            //       newToDo.FolderID = newToDoAdd.FolderID;
            newToDo.RespPerson = newToDoAdd.RespPerson;
            newToDo.Status = "O";  // newToDoAdd.Status;
            newToDo.TDTSortOrder = "000000002";   //  Default for AddToTDT

            newToDoAdd.Status = "O";
            item.cStatus = "O";

            db.ToDos.Add(newToDo);
            ((PlansViewModel)this.DataContext).UpdateDB();
         //   UpdateDB(db);
            // theDB.db.SaveChanges(); 
            mLogger.AddLogMessage("end AddToDoTDT");
            //mark event as handled
            e.Handled = true;
        }
        #endregion
        ////       private void UpdateDB(Database theDB)   // Update the Database
        //private void UpdateDB(TDTDbContext db)   // Update the Database
        //{
        //    mLogger.AddLogMessage("==== UpdateDB ==== PlansView.xaml.cs l 989 =============>");

        //    ////theDB.db.ChangeTracker.Entries();
        //    ////  Database theDB = new Database();
        //    //foreach (var history in db.ChangeTracker.Entries()
        //    //              .Where(e => e.Entity is IModificationHistory && (e.State == System.Data.Entity.EntityState.Added ||
        //    //                      e.State == System.Data.Entity.EntityState.Modified))
        //    //               .Select(e => e.Entity as IModificationHistory)
        //    //              )
        //    //{
        //    //    //history.DateModified = DateTime.Now;
        //    //    //if (history.DateCreated == DateTime.MinValue)
        //    //    //{
        //    //    //    history.DateCreated = DateTime.Now;
        //    //    //}
        //    //    Project p = history as Project;
        //    //    if (p != null)
        //    //    {
        //    //        mLogger.AddLogMessage("PlansView.xaml.cs-ChangeTracker: " + p.ProjectID + "-" + p.ProjectID + "-" + history.ToString());
        //    //    }
        //    //}

        //    try
        //    {
        //        int nChanges = db.SaveChanges();
        //        ///  ShowUserMessage("Database Updated with " + nChanges.ToString() + " changes.");
        //        mLogger.AddLogMessage("UpdateDB successfully completed. with " + nChanges.ToString() + " changes.");
        //    }
        //    catch (DbEntityValidationException dbEx)
        //    {
        //        // mLogger.AddLogMessage("DbEntityValidationException: =====");
        //        foreach (var validationErrors in dbEx.EntityValidationErrors)
        //        {
        //            foreach (var validationError in validationErrors.ValidationErrors)
        //            {
        //                mLogger.AddLogMessage("Plans Validation. Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
        //                Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
        //            }
        //        }
        //    }
        //    catch (System.Data.Entity.Infrastructure.DbUpdateException deDbEx)
        //    {
        //        //foreach (var vErrors in deDbEx.InnerException)
        //        //{
        //        var theProblem = deDbEx.InnerException.Message;
        //        //}
        //        mLogger.AddLogMessage("UpdateDB DbUpdateException: " + deDbEx.InnerException.Message);
        //    }
        //    catch (Exception e)
        //    {
        //        if (System.Diagnostics.Debugger.IsAttached)
        //        {
        //            string ErrorMessage = e.InnerException.GetBaseException().ToString();
        //        }
        //        mLogger.AddLogMessage("UpdateDB Exception: " + e.InnerException.GetBaseException().ToString());
        //        //ShowUserMessage("There was a problem updating the database");
        //    }
        //    finally
        //    {
        //        mLogger.AddLogMessage("  ========= finally End UpdateDB in PlansView.xaml.cs l 1049=============");
        //    }
        //}

        private void tv_DragEnter(object sender, DragEventArgs e)
        {
            string name = sender.ToString();
        }

        private void AddNewOne(object sender, System.Windows.RoutedEventArgs e)
        {
            // Called by ContextMenu, get current tv Item which will be parent for NewOne
            TreeViewItemViewModel theBase = GetCommandItem();
            //TreeViewItemViewModel parent = theBase.Parent;
            TreeViewItemViewModel parent = theBase;

            // Call dialog to get user to import name
            string name = ShowInputDialog(null);

            //////((PlansViewModel)DataContext).DetailsVM = new ProjectVM();
            //////((PlansViewModel)DataContext).DetailsVM.IsNew = true;
            //////((PlansViewModel)DataContext).DetailsVM.Selection = "Adding New One";
            ////// ((PlansViewModel)DataContext).DetailsVM.TheEntity = ((PlansViewModel)DataContext).SelectedProject;
            ////////  DetailsVM.TheEntity = SelectedProject;
            //////((PlansViewModel)DataContext).DetailsVM.TheEntity.Item = name;  // "New Work Item";

            ////////((PlansViewModel)DataContext).DetailsVM.IsNew = true;
            
            ((PlansViewModel)DataContext).AddingNewOne(parent, name);
            e.Handled = true;
        }

        private void tv_Loaded(object sender, RoutedEventArgs e)
        {
            var theTV = ((TreeView)sender);
            var Helper = ((TreeView)sender).Items[0];
            var tvItemX = tv.ItemContainerGenerator.ContainerFromItem(((TreeView)sender).SelectedItem);

            TreeViewItem item = (TreeViewItem)(tv.ItemContainerGenerator.ContainerFromIndex(tv.Items.CurrentPosition));

            //TreeViewItem tvItem = ((TreeViewItem)((TreeView)sender).Items[0]);
            //item.IsSelected = true;
         //   item.Focus();
        }

        //private void DeleteCurrent(object sender, RoutedEventArgs e)
        //{
        //    //get item
        //    //TreeViewItemViewModel item = GetCommandItem();
        //    TreeViewItemViewModel item = tv.SelectedItem; // GetCommandItem();

        //    //remove from parent
        //    item.Parent.Children.Remove(item);
        //   // item.ParentCategory.SubCategories.Remove(item);

        //    //mark event as handled
        //    e.Handled = true;
        //}


        /// <summary>
        /// Determines the item that is the source of a given command.
        /// As a command event can be routed from a context menu click
        /// or a short-cut, we have to evaluate both possibilities.
        /// </summary>
        /// <returns></returns>
        //private TreeViewItemViewModel GetCommandItem()
        //{
        //    //get the processed item
        //    //ContextMenu menu = CategoryTree.NodeContextMenu;
        //    ContextMenu menu = PlansTree.NodeContextMenu;// ContextMenuProperty;
        //    if (menu.IsVisible)
        //    {
        //        //a context menu was clicked
        //        TreeViewItem treeNode = (TreeViewItem)menu.PlacementTarget;
        //        return (TreeViewItemViewModel)treeNode.Header;
        //    }
        //    else
        //    {
        //        //the context menu is closed - the user has pressed a shortcut
        //        return ()PlansTree.SelectedItem;
        //    }
        //}

        // =============================================


    }

   

}