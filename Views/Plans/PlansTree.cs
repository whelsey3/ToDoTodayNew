using CodeFirst.EFcf;
using System;
//using Hardcodet.Wpf.GenericTreeView;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using LoggerLib;

namespace Planner
{
    public class PlansTree : TreeViewBase<TreeViewItemViewModel>
    {
        Logger mLogger;
        public PlansTree() : base()
        {
            mLogger = Logger.Instance;
        //    this.Name = "tv";
            //create a dummy root node
            //TreeViewItem rootNode = (TreeViewItem)FindResource("CustomRootNode");

          //  this.RootNode = rootNode;
            ////create a dummy root node
            //TreeViewItem rootNode = (TreeViewItem)FindResource("CustomRootNode");
            //CategoryTree.RootNode = rootNode;
            IsLazyLoading = false;
            //AllowDrop = true;
            ObserveChildItems = true;
        }

        /// <summary>
        /// Generates a unique identifier for a given
        /// item that is represented as a node of the
        /// tree.
        /// </summary>
        /// <param name="item">An item which is represented
        /// by a tree node.</param>
        /// <returns>A unique key that represents the item.</returns>
        public override string GetItemKey(TreeViewItemViewModel item)
        {
           // mLogger.AddLogMessage("GetItemKey->" + item.DbID + "--" + item.GetType().ToString());
            return item.DbID;
            //return item.FolderName;
        }

        /// <summary>
        /// Gets all child items of a given parent item. The
        /// tree needs this method to properly traverse the
        /// logic tree of a given item.<br/>
        /// Important: If you plan to have the tree automatically
        /// update itself if nested content is being changed, you
        /// the <see cref="TreeViewBase{T}.ObserveChildItems"/> property must be
        /// true, and the collection that is being returned
        /// needs to implement the <see cref="INotifyCollectionChanged"/>
        /// interface (e.g. by returning an collection of type
        /// <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <param name="parent">A currently processed item that
        /// is being represented as a node of the tree.</param>
        /// <returns>All child items to be represented by the
        /// tree.<br/>
        /// If setting the <see cref="TreeViewBase{T}.ObserveChildItems"/>
        /// to true is supposed to work, the returned collection must 
        /// implement <see cref="INotifyCollectionChanged"/> .
        /// </returns>
        /// <remarks>If this is an expensive operation, you should
        /// override <see cref="TreeViewBase{T}.HasChildItems"/> which
        /// invokes this method by default.</remarks>
        public override ICollection<TreeViewItemViewModel> GetChildItems(TreeViewItemViewModel parent)
        {
            if (parent != null)
            {
                if (parent.DbID != "DUMMY")
                {
                    return parent.Children;
                }
                else
                {
                    return new ObservableCollection<TreeViewItemViewModel>();
                }
            }
            else
            {
                return new ObservableCollection<TreeViewItemViewModel>();
            }
           // return parent.Children;
        }

        /// <summary>
        /// Gets the parent of a given item, if available. If
        /// the item is a top-level element, this method is supposed
        /// to return a null reference.
        /// </summary>
        /// <param name="item">The currently processed item.</param>
        /// <returns>The parent of the item, if available.</returns>
        public override TreeViewItemViewModel GetParentItem(TreeViewItemViewModel item)
        {
            return item.Parent;
        }

        protected override TreeViewItem CreateTreeViewItem(TreeViewItemViewModel item)
        {
            //return new TreeViewItem();
            TreeViewItem tvi = new TreeViewItem();
            string iName = "";
            if (item.DbID != null)
            {
                if (item.DbID.Contains("DUMMY"))
                {
                    bool chkType = (item is dbFolderViewModel);
                    ///string theType = item.GetType().ToString();
                }
                else
                {
                    string theType = item.GetType().ToString();
                    if (theType.Contains("Folder"))
                    {
                        iName = ((dbFolderViewModel)item).FolderName;
                        item.DbID = "F" + ((dbFolderViewModel)item)._folder.FolderID.ToString();
                    }
                    else if (theType.Contains("Project"))
                    {
                        iName = ((dbProjectViewModel)item).ProjectName;
                        item.DbID = ((dbProjectViewModel)item)._project.ProjectID.ToString();
                    }
                    else if (theType.Contains("SubTask"))
                    {
                        iName = ((dbSubTaskViewModel)item).SubTaskName;
                        item.DbID = ((dbSubTaskViewModel)item)._subTask.ProjectID.ToString();
                    }
                    else if (theType.Contains("Task"))
                    {
                        iName = ((dbTaskViewModel)item).TaskName;
                        item.DbID = ((dbTaskViewModel)item)._task.ProjectID.ToString();
                    }

                }
            }
            tvi.Header = item;
            mLogger.AddLogMessage("CreateTreeViewItem->" + item.DbID + "--" + iName + "--" + item.GetType().ToString());
            return tvi;
        }

    }
}
