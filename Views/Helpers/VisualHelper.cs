using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CodeFirst.EFcf;
using System.Linq;

namespace Planner
{
    public class VisualHelper
    {
        public static LoggerLib.Logger mLogger = LoggerLib.Logger.Instance;
        public static int prevIndex = 0;
        public static int curIndex = 0;
        public static int dragIndex = 0;
        public static int destIndex = 0;
        public static TDTcfEntities db = new TDTcfEntities();

        //public VisualHelper(TDTcfEntities _db)
        //{
        //    db = _db;
        //}

        //EnableRowsMoveProperty is used to enable rows moving by mouse drag and move in data grid
        //the only requirement is to ItemsSource collection of datagrid be a ObservableCollection or at least IList collection
        public static readonly DependencyProperty EnableRowsMoveProperty =
            DependencyProperty.RegisterAttached("EnableRowsMove", typeof(bool), typeof(VisualHelper), new PropertyMetadata(false, EnableRowsMoveChanged));

        //Private DraggedItemProperty attached property used only for EnableRowsMoveProperty
        private static readonly DependencyProperty DraggedItemProperty =
            DependencyProperty.RegisterAttached("DraggedItem", typeof(object), typeof(VisualHelper), new PropertyMetadata(null));

        public static bool GetEnableRowsMove(DataGrid obj)
        {
            return (bool)obj.GetValue(EnableRowsMoveProperty);
        }

        public static void SetEnableRowsMove(DataGrid obj, bool value)
        {
            obj.SetValue(EnableRowsMoveProperty, value);
        }

        private static object GetDraggedItem(DependencyObject obj)
        {
            return (object)obj.GetValue(DraggedItemProperty);
        }

        private static void SetDraggedItem(DependencyObject obj, object value)
        {
            obj.SetValue(DraggedItemProperty, value);
        }

        private static void EnableRowsMoveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = (d as DataGrid);
            if (grid == null) return;
            if ((bool)e.NewValue)
            {
                grid.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
                grid.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
                grid.PreviewMouseMove += OnMouseMove;
            }
            else
            {
                grid.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
                grid.PreviewMouseLeftButtonUp -= OnMouseLeftButtonUp;
                grid.PreviewMouseMove -= OnMouseMove;
            }
        }

        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // mLogger.AddLogMessage("Left Button Down " + startIndex + " to " + endIndex);
            mLogger.AddLogMessage("Left Button Down " + curIndex + " from " + prevIndex + " and " + dragIndex + "-" + destIndex);

            //find datagrid row by mouse point position
            var row = TryFindFromPoint<DataGridRow>((UIElement)sender, e.GetPosition((sender as DataGrid)));
            if (row == null || row.IsEditing) return;
            prevIndex = dragIndex = curIndex = row.GetIndex();
            VisualHelper.SetDraggedItem(sender as DataGrid, row.Item);
            mLogger.AddLogMessage("Left Button Down " + dragIndex + " and current " + curIndex);
        }

        private static void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mLogger.AddLogMessage("Left Button Up   " + curIndex + " from " + prevIndex + " and " + dragIndex + "-" + destIndex);
            if (((sender as DataGrid).SelectedIndex) == curIndex && dragIndex == 0)
            {
                // Haven't changed rows
                curIndex = prevIndex = destIndex = dragIndex = 0;
                mLogger.AddLogMessage("---Reset == Left Button Up, no drag");
                return;
            }
            mLogger.AddLogMessage("Left Button Up " + dragIndex + " to " + curIndex);
            var draggeditem = VisualHelper.GetDraggedItem(sender as DependencyObject);
            if (draggeditem == null) return;  // not dragging

            ExchangeItems(sender, (sender as DataGrid).SelectedItem);
            mLogger.AddLogMessage("End of Drag!!  Moved " + dragIndex + " to " + destIndex);
            
            //select the dropped item
            (sender as DataGrid).SelectedItem = draggeditem;
            //reset
            VisualHelper.SetDraggedItem(sender as DataGrid, null);
            // Adjust backing data
            var list = (sender as DataGrid).ItemsSource as IList;

            ListToDosGrid(mLogger, list);
            prevIndex = curIndex = dragIndex = destIndex = 0;
        }

        //endIndex = 0;
        //}

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (dragIndex != 0)
            {
                // mLogger.AddLogMessage("Mouse Move  " + curIndex + " from " + prevIndex + " and " + dragIndex + "-" + destIndex);
                var draggeditem = VisualHelper.GetDraggedItem(sender as DependencyObject);
                if (draggeditem == null)
                {
                    //mLogger.AddLogMessage("MouseMove - Not dragging");
                    return;
                }
                var row = TryFindFromPoint<DataGridRow>((UIElement)sender, e.GetPosition((sender as DataGrid)));
                prevIndex = curIndex;
                curIndex = row.GetIndex();
                if (curIndex == prevIndex || dragIndex == 0)
                {
                    // mLogger.AddLogMessage("MouseMove indexes not interesting!");
                    return;
                }
                //if (row == null || row.IsEditing || curIndex != dragIndex)
                if (row == null || row.IsEditing)
                {
                    mLogger.AddLogMessage("MouseMove - Not New Row");
                    return;
                }
                mLogger.AddLogMessage("Mouse Move  " + curIndex + " from " + prevIndex + " and " + dragIndex + "-" + destIndex);
                // Potential exchange needed with new row
                ExchangeItems(sender, row.Item);
            }
        }

        private static void ExchangeItems(object sender, object targetItem)
        {
            mLogger.AddLogMessage("ExchangeItems " + curIndex + " from " + prevIndex + " and " + dragIndex + "-" + destIndex);

            //LoggerLib.Logger mLogger = LoggerLib.Logger.Instance;
            //mLogger.AddLogMessage("Start ExchangeItems");
            var draggeditem = VisualHelper.GetDraggedItem(sender as DependencyObject);
            if (draggeditem == null) return;
            if (targetItem != null && !ReferenceEquals(draggeditem, targetItem))
            {
                var list = (sender as DataGrid).ItemsSource as IList;
                if (list == null)
                    throw new ApplicationException("EnableRowsMoveProperty requires the ItemsSource property of DataGrid to be at least IList inherited collection. Use ObservableCollection to have movements reflected in UI.");
                //get target index
                curIndex = list.IndexOf(targetItem);
                destIndex = curIndex;
                mLogger.AddLogMessage("Dragging: " + draggeditem.ToString() + " - " + list.IndexOf(draggeditem));
                prevIndex = list.IndexOf(draggeditem);
                mLogger.AddLogMessage("ExchangeItems: " + prevIndex + " goes to " + curIndex);
                // Adjust by reversing the two items
                //remove the source from the list
                list.Remove(draggeditem);  // prevIndex

                //move source at the target's location
                //       list.Insert(destIndex, draggeditem);
                list.Insert(curIndex, draggeditem);
            }
            //
        }

        //private static void ListToDosGrid(LoggerLib.Logger mLogger, IList list)
        public static void ListToDosGrid(LoggerLib.Logger mLogger, IList list)
        {
            //CodeFirst.EFcf.TDTcfEntities db = new CodeFirst.EFcf.TDTcfEntities();
       //     TDTcfEntities db = _db;  // (TDTcfEntities)App.Current.Resources["theData"];
            var toDos = (from c in db.ToDos

                         select c);
            int rNum = 2;
            foreach (var dgRow in list)
            {
                CodeFirst.EFcf.ToDo toDo = ((ToDoVM)dgRow).TheEntity;
                if (toDo.Status == "A")
                {
                    toDo.TDTSortOrder = "000000";// (rNum + 1000000).ToString().Substring(1, 6);
                }
                else
                {
                    toDo.TDTSortOrder = (rNum + 1000000).ToString().Substring(1, 6);
                }
                //toDo.
                ToDo thisOne = db.ToDos.Find(toDo.ToDoID);
                thisOne.TDTSortOrder = toDo.TDTSortOrder;

                mLogger.AddLogMessage(rNum.ToString() + "  " + toDo.ToDoID.ToString() + " - " + toDo.TDTSortOrder);
                rNum++;
            }
            // db.
            int nChanges = db.SaveChanges();
            //ShowUserMessage("Database Updated with " + nChanges.ToString() + " changes.");
            mLogger.AddLogMessage("ListToDosGrid->UpdateDB successfully completed with " + nChanges.ToString() + " changes.");
        }

        public static T FindVisualParent<T>(DependencyObject child)
          where T : DependencyObject
        {
            // get parent item
            var parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we’re looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                // use recursion to proceed with next level
                return FindVisualParent<T>(parentObject);
            }
        }

        public static T TryFindFromPoint<T>(UIElement reference, Point point)
          where T : DependencyObject
        {
            var element = reference.InputHitTest(point) as DependencyObject;
            if (element == null) return null;
            if (element is T) return (T)element;
            return FindVisualParent<T>(element);
        }

    }
    #region CustomSort
    public class CustomSort : IComparer<string>
    {
        public static string[] statusOrder = new[] { "A", "I", "O", "C", "F" };
        public CustomSort()
        { }
        public int Compare(string x, string y)
        //public int Compare(object x1, object y1)
        {
            //string x = (string)x1;
            //string y = (string)y1;
            if (x == y)
            {
                return 0;
            }
            else
            {
                if (statusOrder.Any(a => a == x) && statusOrder.Any(a => a == y))
                {
                    if (Array.IndexOf(statusOrder, x) < Array.IndexOf(statusOrder, y))
                        return -1;
                    return 1;
                }
                else if (statusOrder.Any(a => a == x)) // only one item in customordered array (and its x)
                    return -1;
                else if (statusOrder.Any(a => a == y)) // only one item in customordered array (and its y)
                    return 1;
                else
                    return string.Compare(x, y);
            }
        }
    }
    #endregion CustomSort

    //#region CustomSort
    //public static string[] statusOrder = new[] { "A", "I", "O", "C", "F" };

    //public static int compare(string x, string y)
    //{
    //    if (x == y)
    //    {
    //        return 0;
    //    }
    //    else
    //    {
    //        if (statusOrder.Any(a => a == x) && statusOrder.Any(a => a == y))
    //        {
    //            if (Array.IndexOf(statusOrder, x) < Array.IndexOf(statusOrder, y))
    //                return -1;
    //            return 1;
    //        }
    //        else if (statusOrder.Any(a => a == x)) // only one item in customordered array (and its x)
    //            return -1;
    //        else if (statusOrder.Any(a => a == y)) // only one item in customordered array (and its y)
    //            return 1;
    //        else
    //            return string.Compare(x, y);
    //    }
    //}
    //#endregion CustomSort

}
