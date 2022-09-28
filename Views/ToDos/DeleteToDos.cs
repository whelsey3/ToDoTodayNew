//using TDT_EF;  // 
using BuildSqliteCF.Entity;
using GongSolutions.Wpf.DragDrop;
//using BuildSqliteCF.DbContexts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
//using LoggerLib;
using System.Windows.Controls;

namespace Planner
{
    public partial class ToDosViewModel : ViewModelBase, IDragSource, IDropTarget // 
    {
        protected void DeleteSelectedToDos()
        {
            int nGridPosition = SelectedToDoIndex;
            int DelCount = 1;
            if (TheDGToDos.SelectedItems.Count == 0)
            {
                // There are no selected items to delete
                return;  // should not be here
            }
            else
            {
                //Process list to get approved list of deletions
                ObservableCollection<ToDoVM> ToDoList = new ObservableCollection<ToDoVM>();
                // Process list of selected ToDos
                DelCount = 1;

                mLogger.AddLogMessage("DELETING ToDos " + TheDGToDos.SelectedItems.Count + " ToDos selected");

                foreach (ToDoVM ToDoItem in TheDGToDos.SelectedItems)
                {
                    //  Check for "A" status (Inwork)
                    if (ToDoItem.TheEntity.Status == "A")
                    {
                        // Can't delete active ToDo
                        ShowUserMessage("Can't delete the active ToDo.  Stop timing and then delete.");
                        mLogger.AddLogMessage("Can't delete the active ToDo.  Stop timing and then delete.");
                        //break;  //return;
                    }
                    else
                    {
                        // Confirm deletion and copy ToDo for deletion
                        if (DelCount == 1)
                        {
                            nGridPosition = SelectedToDoIndex;
                        }
                        mLogger.AddLogMessage("Confirming deletion: " + DelCount + " " + ToDoItem.TheEntity.Item);
                        if (Util.CheckDeleteOK("Are you sure you want to delete the current ToDo, '" + ToDoItem.TheEntity.Item.Trim() + "'?"))
                        {
                            // Process deletion list
                            ToDoVM DeleteItem = new ToDoVM();
                            DeleteItem.TheEntity = ToDoItem.TheEntity;
                            ToDoList.Add(DeleteItem);
                        //  nGridPosition = SelectedToDoIndex;
                            mLogger.AddLogMessage("Confirmed delete for " + DelCount + " " + DeleteItem.TheEntity.Item + " nGridPosition: " + nGridPosition);
                        }
                        else
                        {
                            mLogger.AddLogMessage("User declined approval for deletinng " + DelCount + " " + ToDoItem.TheEntity.Item.Trim() + " nGridPosition: " + nGridPosition);
                        }
                        DelCount++;
                    }
                    //mLogger.AddLogMessage("Processing deletions: " + DelCount + " " + ToDoItem.TheEntity.Item);
                }
                // Process list of selected items for deletion
                DelCount = 1;
                foreach (var DeleteToDoItem in ToDoList)
                {
                    mLogger.AddLogMessage("Processing deletion list: " + DelCount + " " + DeleteToDoItem.TheEntity.Item);

                    mLogger.AddLogMessage("Confirmed DeleteCurrent ToDo DeleteCurrent with '" + DeleteToDoItem.TheEntity.Item.Trim() + "' - " + DeleteToDoItem.TheEntity.Status + "------------------>");
                    // Need to adjust Project status or remove Project if AdHoc
                    Project toDoProj = dbBase.Projects.Find(DeleteToDoItem.TheEntity.ProjectID);
                    Util.AdjustProjStatusForToDoDelete(DeleteToDoItem.TheEntity, dbBase);
                    // Delete ToDo
                    dbBase.ToDos.Remove(DeleteToDoItem.TheEntity);
                    ToDos.Remove(DeleteToDoItem);
                    RaisePropertyChanged("ToDos");

                    DelCount++;
                }
                UpdateDB();

            }
            // Delete occurred set focus to previous value(n -1)
            nGridPosition = nGridPosition >= 1 ? nGridPosition - 1 : 0;

            // Delete did NOT occur, reset focus to n

            Util.SelectRowByIndex(TheDGToDos, nGridPosition);

            ////  Clean up and position current item in list
            //nGridPosition = 9;
            //// Delete did NOT occur, reset focus to n
            //Util.SelectRowByIndex(TheDGToDos, nGridPosition);
            //  
        }
    
        protected void DeleteCurrent() // Delete current selection
        {
            int n = SelectedToDoIndex;
            EditVM = SelectedToDo;
            if (EditVM.TheEntity.Status == "A")
            {
                // Can't delete active ToDo
                ShowUserMessage("Can't delete the active ToDo.  Stop timing and then delete.");
                return;
            }

            if (Util.CheckDeleteOK("Are you sure you want to delete the current ToDo, '" + EditVM.TheEntity.Item.Trim() + "'?"))
            {
                // Need to adjust Project status or remove Project if AdHoc
                Project toDoProj = dbBase.Projects.Find(SelectedToDo.TheEntity.ProjectID);
                Util.AdjustProjStatusForToDoDelete(EditVM.TheEntity, dbBase);
                // Delete ToDo
                dbBase.ToDos.Remove(SelectedToDo.TheEntity);
                ToDos.Remove(SelectedToDo);
                RaisePropertyChanged("ToDos");

                //  NEED TO REVIEW, multiple ToDos for a given project, don't delete without checking
                //if (toDoProj.FolderID == AdHocFolderID)
                //{
                //    //toDoProj.Status = SelectedToDo.TheEntity.Status;
                //    // other Project adjustments, then delete ToDo
                //    db.Projects.Remove(toDoProj);
                //}

                UpdateDB();
                //CommitUpdates();
                //selectedEntity = null;  // Nothing should be selected // Delete occurred set focus to previous value (n -1)
                // Delete occurred set focus to previous value (n -1)
                n = n >= 1 ? n - 1 : 0;
            }
            // Delete did NOT occur, reset focus to n

            Util.SelectRowByIndex(TheDGToDos, n);
        }

    }
}