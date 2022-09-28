using CodeFirst.EFcf;
//using Hardcodet.Wpf.GenericTreeView;
using System;
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
    public partial class Plans2View : UserControl
    {
        //readonly PlansViewModel _familyTree;
        LoggerLib.Logger mLogger;

        /// <summary>
        /// Initializes a new instance of the ProjectsView class.
        /// </summary>
        public Plans2View()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                string tt = e.InnerException.Message;
                throw;
            }
            //InitializeComponent();
    //        ShowUserMessage("There was a problem updating the database");

            mLogger = LoggerLib.Logger.Instance;

            ////     // Get raw family tree data from a database.
            Database theDB = new Database();
            ///       Folder[] folders = theDB.GetFolders();
            var theFolders = (from c in theDB.db.Folders
                              orderby c.FSortOrder
                              select c).ToList();
            CodeFirst.EFcf.Folder[] folders = new CodeFirst.EFcf.Folder[theFolders.Count];
            int i = 0;
            foreach (var item in theFolders)
            {
                folders[i] = (CodeFirst.EFcf.Folder)theFolders[i];
                folders[i].Projects = null;
                i++;
            }
            ///return fs; //  theFolders.ToArray<Folder>;








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
            bool LazyLoading = false;
            ////Folder[] folders = DatabaseHC.GetFolders();
            Plans2ViewModel viewModel = new Plans2ViewModel(folders, LazyLoading);
            
            System.Collections.ObjectModel.ObservableCollection<dbFolderViewModel> saveIt = new System.Collections.ObjectModel.ObservableCollection<dbFolderViewModel>(viewModel.Folders);
        //   SaveData(saveIt);

            this.DataContext = viewModel;
            //base.DataContext = viewModel;
            //Throbber.Visibility = Visibility

            //tv.RootNode = (TreeViewItem)((TreeViewItemViewModel)(viewModel.Folders[0]));
        }

        private void tv_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            object current;
            current = tv.SelectedItem;
            // ((TreeViewItem)tv.Items[0]).IsSelected = true;
           // ((TVItemViewModel)tv.Items[0]).IsExpanded = true;
          //  ((TVItemViewModel)tv.SelectedItem).IsExpanded = true;
        }

        //private void tv_SelectedItemChanged(object sender, Hardcodet.Wpf.GenericTreeView.RoutedTreeItemEventArgs<TreeViewItemViewModel> e)
        //{
        //    object current;
        //    current = tv.SelectedItem;
        //}

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
            tv.Tag = e.OriginalSource;
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

        #region context menu command handling

        /// <summary>
        /// Creates a sub category for the clicked item
        /// and refreshes the tree.
        /// </summary>
        private void AddNewOne(object sender, ExecutedRoutedEventArgs e)
        {
            //get the processed item
            TreeViewItemViewModel parent = GetCommandItem();
            AddingNewOne(parent);

            //create a sub category
         //   string name = ShowInputDialog(null);

            //check if we already have such a category
            //if (GetShop().TryFindCategoryByName(name) != null)
            //{
            //    string msg = "We already have a category with name '{0}'";
            //    msg = String.Format(msg, name);
            //    MessageBox.Show(msg, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    e.Handled = true;
            //    return;
            //}

            ////ShopCategory subCategory = new ShopCategory(name, parent);
            ////parent.SubCategories.Add(subCategory);
         //   string theType = parent.GetType().ToString();
         ////   dbTaskViewModel newDBItem = null;
         //   if (theType.Contains("Project"))
         //   {
         //       Database theDB = new Database();
         //       //   Project[] theProjs = theDB.GetProjects();
         //       Project theTask = GetNewProject(name);   // new Project(name);
         //       theTask.DetailedDesc = "Details";
         //       theTask.FolderID = ((dbProjectViewModel)parent)._project.FolderID;

         //       dbTaskViewModel newDBItem = new dbTaskViewModel(theTask, (dbProjectViewModel)parent, false);
         //       //item.DbID = ((dbProjectViewModel)item).ProjectName;
         //       theDB.db.Projects.Add(theTask);
         //       int savedMany = theDB.db.SaveChanges();

         //       parent.Children.Add(newDBItem);
         //   }
        //    TreeViewItemViewModel newOne = new TreeViewItemViewModel(parent, false, name);// (name, parent);

            //make sure the parent is expanded
    //       tv.TryFindNode(parent).IsExpanded = true;

            //NOTE this would be an alternative to force layout preservation
            //even if the PreserveLayoutOnRefresh property was false:
         //   TreeLayout layout = tv.GetTreeLayout();
         
     //       tv.Refresh(layout);

            //Important - mark the event as handled
            e.Handled = true;
        }

        
        /// <summary>
        /// Displays an input dialog and returns the entered
        /// value.
        /// </summary>
        private string ShowInputDialog(string defaultValue)
        {
            InputDialog dlg = new InputDialog();
            dlg.CategoryName = defaultValue;
            Window parentWindow = Window.GetWindow(this);
            dlg.Owner = parentWindow;  // (Window)(this.Parent);
           // dlg.DialogResult = false;
            dlg.ShowDialog();
            //if (dlg.ShowDialog())
            //{
            //    return dlg.CategoryName;
            //}
            //else
            //{
            //    return dlg.CategoryName;
            //}
            return dlg.CategoryName;
        }


        /// <summary>
        /// Checks whether it is allowed to delete a category, which is only
        /// allowed for nested categories, but not the root items.
        /// </summary>
        private void EvaluateCanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            //attention: this method is invoked all the time - also if the tree's context
            //menu has just been removed. if the latter is the case, GetCommandItem()
            //would fail, as it works on the context menu
           
            if (tv.ContextMenu != null)
            {
                // ensure the processed item is NOT root.
                TreeViewItemViewModel item = GetCommandItem();
                // This applies to work items!
                e.CanExecute = item.Parent != null;
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
            //get item
            TreeViewItemViewModel item = GetCommandItem();
            string theType = item.GetType().ToString();

            // Adjust base version, appropriately based on desired type 
            if (theType.Contains("Folder"))
            {
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
                Database theDB = new Database();
                Project proj = ((dbProjectViewModel)item)._project;
                Project delProj = theDB.db.Projects.Find(proj.ProjectID);
                theDB.db.Projects.Remove(delProj);
                int savedMany = theDB.db.SaveChanges();


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
                Database theDB = new Database();
                Project subTsk = ((dbSubTaskViewModel)item)._subTask;
                Project delTsk = theDB.db.Projects.Find(subTsk.ProjectID);
                theDB.db.Projects.Remove(delTsk);
                int savedMany = theDB.db.SaveChanges();


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
                Database theDB = new Database();
                Project tsk = ((dbTaskViewModel)item)._task;
                Project delTsk = theDB.db.Projects.Find(tsk.ProjectID);
                theDB.db.Projects.Remove(delTsk);
                int savedMany = theDB.db.SaveChanges();

            }

            //remove from parent in the UI
            mLogger.AddLogMessage("Removing " + item.DbID);
            item.Parent.Children.Remove(item);

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
        private TreeViewItemViewModel GetCommandItem()
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

            Database theDB = new Database();
            Project newToDoAdd = theDB.db.Projects.Find(Convert.ToInt32(item.DbID));
            ToDo newToDo = new ToDo();
            newToDo.Item = newToDoAdd.Item;
            newToDo.DetailedDesc = newToDoAdd.DetailedDesc;
            newToDo.DispLevel = newToDoAdd.DispLevel;
            newToDo.Done = newToDoAdd.Done;
            newToDo.StartDate = newToDoAdd.StartDate;
            newToDo.DoneDate = null;  // newToDoAdd.DoneDate;
            newToDo.DueDate = newToDoAdd.DueDate;
            newToDo.RevDueDate = newToDoAdd.RevDueDate;
            newToDo.Priority = newToDoAdd.Priority;
            newToDo.ProjectID = newToDoAdd.ProjectID;
     //       newToDo.FolderID = newToDoAdd.FolderID;
            newToDo.RespPerson = newToDoAdd.RespPerson;
            newToDo.Status = newToDoAdd.Status;
            newToDo.TDTSortOrder = "000002";

            newToDoAdd.Status = "W";
            item.cStatus = "W";
            mLogger.AddLogMessage("Adding to TDT ->" + newToDoAdd.Item);
            theDB.db.ToDos.Add(newToDo);
            UpdateDB(theDB);
           // theDB.db.SaveChanges(); 

            //mark event as handled
            e.Handled = true;
        }
        #endregion
        private void UpdateDB(Database theDB)   // Update the Database
        {
            mLogger.AddLogMessage("==== UpdateDB =================>");
          //  Database theDB = new Database();

            try
            {
                int nChanges = theDB.db.SaveChanges();
              ///  ShowUserMessage("Database Updated with " + nChanges.ToString() + " changes.");
                mLogger.AddLogMessage("UpdateDB successfully completed. with " + nChanges.ToString() + " changes.");
            }
            catch (DbEntityValidationException dbEx)
            {
                mLogger.AddLogMessage("DbEntityValidationException: =====");
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        mLogger.AddLogMessage("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
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
                mLogger.AddLogMessage("Exception in UpdateDB: " + e.Message);
                if (System.Diagnostics.Debugger.IsAttached && e.InnerException != null)
                {
                    string ErrorMessage = e.InnerException.GetBaseException().ToString();
                    mLogger.AddLogMessage("Exception: " + e.InnerException.GetBaseException().ToString());
                }
                // mLogger.AddLogMessage("Exception: " + e.InnerException.GetBaseException().ToString());
                //ShowUserMessage("There was a problem updating the database");
            }
        }

        private void tv_DragEnter(object sender, DragEventArgs e)
        {
            string name = sender.ToString();
        }

        private void tv_Drop(object sender, DragEventArgs e)
        {
            //GongSolutions.Wpf.DragDrop.DefaultDropHandler.. ..DragOver(e.Data);
            //var dInfo = GongSolutions.Wpf.DragDrop.DragInfo;
            var s = e.Data;
            var a = GongSolutions.Wpf.DragDrop.DefaultDropHandler.ExtractData(e.Data);
            // e.GetPosition.
            var row = VisualHelper.TryFindFromPoint<TreeView>((UIElement)sender, e.GetPosition((sender as TreeView)));
            var t = VisualHelper.TryFindFromPoint<TreeViewItem>((UIElement)sender, e.GetPosition((sender as UIElement)));
            TreeViewItem obj = e.Data.GetData(typeof(TreeViewItem)) as TreeViewItem;
          //  var a1 = (GongSolutions.Wpf.DragDrop.DropInfo)(e.Data);

            // source
            var so = e.OriginalSource;
            //  Want target of drop, i.e., the sender here
            var target = VisualHelper.TryFindFromPoint<TreeViewItem>((UIElement)sender, e.GetPosition((sender as UIElement)));
            //string tarType = target.DataContext.GetType().ToString();
        }

        //void GongSolutions.Wpf.DragDrop.IDropTarget.Drop(IDropInfo dropInfo)
        //{
        ////    // I know this example is called DefaultsExample, but the default handlers don't know how
        ////    // to set an item's group. You need to explicitly set the group on the dropped item like this.
        //    DragDrop.DefaultDropHandler.Drop(dropInfo);
        //    var data = DefaultDropHandler.ExtractData(dropInfo.Data).OfType<GroupedItem>().ToList();
        ////    foreach (var groupedItem in data)
        ////    {
        ////        groupedItem.Group = dropInfo.TargetGroup.Name.ToString();
        ////    }

        ////    // Changing group data at runtime isn't handled well: force a refresh on the collection view.
        ////    if (dropInfo.TargetCollection is ICollectionView)
        ////    {
        ////        ((ICollectionView)dropInfo.TargetCollection).Refresh();
        ////    }
        //}
        //private void AddNewOne(object sender, System.Windows.RoutedEventArgs e)
        //{

        //}

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

    }

    //public class PlansCommands
    //{
    //    static PlansCommands()
    //    {
    //        AddNewFolder = new RoutedUICommand(
    //            "Add New Folder", "AddNewFolder", typeof(PlansCommands),
    //            new InputGestureCollection
    //                { new KeyGesture(Key.F, ModifierKeys.Control, "Ctrl+F") }
    //            );
    //    }

    //    public static RoutedUICommand AddNewFolder { get; private set; }
    //}

}