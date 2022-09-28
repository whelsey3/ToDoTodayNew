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
using System.Security.Cryptography;
using Microsoft.Reporting.WinForms;

namespace Planner
{
   // public partial class ToDosViewModel : CrudVMBaseTDT, IDragSource, IDropTarget // ViewModelBase
    public partial class ToDosViewModel : ViewModelBase, IDragSource, IDropTarget // 
    {
        protected void StartTimingButton2()  // Show popup to get started on work
        {
            // Selection required to get here
            EditVM = SelectedToDo;

            // Make sure not already in Active status
            if (EditVM.TheEntity.Status == "A")
            {
                ShowUserMessage("'" + EditVM.TheEntity.Item.Trim() + "' is already in work!");
                mLogger.AddLogMessage("'" + EditVM.TheEntity.Item.Trim() + "' is already in work!");
                return;
            }

            InWorkToDo = CheckForInWork();  // returns the ToDo, if any, with 'A' , i.e. in WORK

            if (inWorkToDo == null)
            {
                // Fresh Start, nothing already in work.
                TitlePopUp1 = "Starting work on '" + EditVM.TheEntity.Item.Trim() + "' ?";
                TitlePopUp = "";
                // Nominal start with nothing currently in work
                mLogger.AddLogMessage("Popup 0: '" + TitlePopUp + "'");
                mLogger.AddLogMessage("---DIALOG: IsInStartTimingMode, Fresh Start.  '" + EditVM.TheEntity.Item.Trim() + "'");

                // Display PopUp for Start Timing 
                IsInStartTimingMode = true;
            }
            else
            {
                // Have to stop inWorkToDo, then start work on EditVM.TheEntity.Item
                //StopWork();
                StopStartWork();
                IsInStopStartWorkMode = true;
            }

        }
        protected void StartTimingButton()
        {
            //StartTiming2();
            StartTimingUI();
        }

        protected void StartTimingUI()
        {
            // UI Selection of ToDo required to get here
            //   Could be SWST or simple ST
            // Show popup to get stdarted on work

            // UI Selection of ToDo required to get here
            //   Could be SWST or simple ST
            // Show popup to get started on work

            EditVM = SelectedToDo;   // NextToDo
            int n = SelectedToDoIndex;

            // Make sure this ToDo not already in Active status
            if (EditVM.TheEntity.Status == "A")
            {
                ShowUserMessage("'" + EditVM.TheEntity.Item.Trim() + "' is already in work!");
                mLogger.AddLogMessage("'" + EditVM.TheEntity.Item.Trim() + "' is already in work!");
                return;
            }

            // Check if eligible for starting work  === What is 'R' status? ===
            //if (EditVM.TheEntity.Status == "R")
            //{
            //    ShowUserMessage("'" + EditVM.TheEntity.Item.Trim() + "' is a Reminder!");
            //    mLogger.AddLogMessage("'" + EditVM.TheEntity.Item.Trim() + "' is a Reminder!");
            //    HandleReminder();
            //    return;
            //}

            int nIndexOfInWork = IndexOfA();  // Check for existing item in work
            int nIndexOfNextToDo = SelectedToDoIndex;
           
            ToDoVM ttt = CheckForInWork();  // Alternative approach, revisit later

            // Should now have W, I, C, F, O as Status ===========================
            mLogger.AddLogMessage("===== Reached StartTimingUI with '" + EditVM.TheEntity.Item.Trim() + "' - " + EditVM.TheEntity.Status + " ====>");
            
            //  Capture current time
            StopStartTime = DateTime.Now;

          //  int nIndexOfInWork = IndexOfA();
         //  int nIndexOfNextToDo = SelectedToDoIndex;
          //  ToDoVM ttt = CheckForInWork();

            // Should now have W, I, C, F, O as Status ===========================
            mLogger.AddLogMessage("===== Reached StartTimingUI with '" + EditVM.TheEntity.Item.Trim() + "' - " + EditVM.TheEntity.Status + " ====>");
            
            //  Capture current time
            StopStartTime = DateTime.Now;

            //if (InWorkToDo == null)
            if (nIndexOfInWork == -1)
            {
                // ST case, simple start timing (uses EditVM = SelectedToDo)
                // Fresh Starting selection, nothing already in work.
                TitlePopUp = "Starting work on '" + EditVM.TheEntity.Item.Trim() + "' ?";
                TitlePopUp1 = "";
                mLogger.AddLogMessage("Popup 0: '" + TitlePopUp + "'");
                mLogger.AddLogMessage("---DIALOG: IsInStartTimingMode, Fresh Start.  '" + EditVM.TheEntity.Item.Trim() + "'");
                EditVM.TheEntity.StartDate = StopStartTime;
                RaisePropertyChanged("EditVM");

                IsInStartTimingMode = true;  // Display PopUp for Start Timing}
            }
            else
            {
                // SWST case, StopStart case.  Have to stop inWorkToDo, then start work on NextToDo //EditVM.TheEntity.Item
                // InWorkToDo = ToDos[ (nIndexOfInWork == -1) ? SelectedToDoIndex : nIndexOfInWork ];
                NextToDo = SelectedToDo;  // binding in dg

                //   inWorkToDo = ToDos[nIndexOfInWork];

                InWorkToDo = ToDos[(nIndexOfInWork == -1) ? SelectedToDoIndex : nIndexOfInWork];
                InWorkToDo.TheEntity.DoneDate = StopStartTime; // DateTime.Now;
                RaisePropertyChanged("InWorkToDo");
                //  NextToDo = ToDos[nIndexOfNextToDo];  // SetNextToDo();
                mLogger.AddLogMessage("Stopping inWork before starting another.");

                mLogger.AddLogMessage("InWorkToDo.TheEntity.Item = '" + InWorkToDo.TheEntity.Item + "'");
                mLogger.AddLogMessage("EditVM.TheEntity.Item = '" + EditVM.TheEntity.Item + "'");
                mLogger.AddLogMessage("NextToDo.TheEntity.Item = '" + NextToDo.TheEntity.Item + "'");
                mLogger.AddLogMessage("SelectedToDo.TheEntity.Item = '" + SelectedToDo.TheEntity.Item + "'");

                TitlePopUp = "Confirm stoping work on '" + InWorkToDo.TheEntity.Item.Trim() + "' ?**";
                TitlePopUp1 = "Then starting work on '" + NextToDo.TheEntity.Item.Trim() + "' ?**";

                IsInStopStartWorkMode = true;
                // StopWork();
                //     StopStartWork();
            }


            //   //  Check for active ToDo
            //   InWorkToDo = CheckForInWork();  // returns the ToDo, if any, with 'A' , i.e. in WORK
            ////   int m = Array.FindIndex(ToDos, item => item. == InWorkToDo);
            //   int m1 = ToDos.IndexOf(InWorkToDo);
            //   bool m2 = ToDos.Contains(InWorkToDo);
            //   int m4 = IndexOfA();
            //   // Display StartTiming popup
            //   if (InWorkToDo == null)
            //   {

            //   }
            //   else
            //   {

            //   }

        }

        ToDo CreateNewToDO(ToDo TheMaster)
        {
            // Initialise the entity or inserts will fail
            ToDo TheNewToDo = new ToDo();
            //if (IsNew)   // Defaults to true in VMBase default constructor
            //{
            TheNewToDo.Status = TheMaster.Status;
            TheNewToDo.Priority = TheMaster.Priority;
            TheNewToDo.Item = TheMaster.Item;
            TheNewToDo.DispLevel = TheMaster.DispLevel;
            TheNewToDo.DetailedDesc = TheMaster.DetailedDesc;
            TheNewToDo.Done = TheMaster.Done;
            TheNewToDo.DueDate = TheMaster.DueDate;
            TheNewToDo.RevDueDate = TheMaster.RevDueDate;
            TheNewToDo.StartDate = TheMaster.StartDate;
            TheNewToDo.RespPerson = TheMaster.RespPerson;  // "ELS";
            TheNewToDo.TDTSortOrder = TheMaster.TDTSortOrder;    // Default for ADD
            TheNewToDo.Hide = TheMaster.Hide;
            //   TheNewToDo.ProjectID = 1127;  // Hardcoded for "AdHoc"
            //   TheNewToDo.ToDoID
            //   TheNewToDo.IsDirty
            TheNewToDo.ElapsedTime = 0.0M;
            return TheNewToDo;
        }


        //    protected void StartTiming2()  // Show popup to get started on work
        //{
        //    // UI ToDo Selection required to get here
        //    EditVM = SelectedToDo;
        //    int n = SelectedToDoIndex;

        //    // Make sure not already in Active status
        //    if (EditVM.TheEntity.Status == "A")
        //    {
        //        ShowUserMessage("'" + EditVM.TheEntity.Item.Trim() + "' is already in work!");
        //        mLogger.AddLogMessage("'" + EditVM.TheEntity.Item.Trim() + "' is already in work!");
        //        return;
        //    }

        //    // Check if eligible for starting work
        //    if (EditVM.TheEntity.Status == "R")
        //    {
        //        ShowUserMessage("'" + EditVM.TheEntity.Item.Trim() + "' is a Reminder!");
        //        mLogger.AddLogMessage("'" + EditVM.TheEntity.Item.Trim() + "' is a Reminder!");
        //        HandleReminder();
        //        return;
        //    }

        //    // Should now have W, I, C, F, O as Status ===========================
        //    mLogger.AddLogMessage("===== Reached StartTiming2 with '" + EditVM.TheEntity.Item.Trim() + "' - " + EditVM.TheEntity.Status + " ====>");
            
        //    curTDTSortOrder = EditVM.TheEntity.TDTSortOrder;
        //    if (SwitchDate == DateTime.MinValue)
        //    {
        //        //No Stop screen so need to set value
        //        EditVM.TheEntity.StartDate = DateTime.Now;
        //    }
        //    else
        //    {
        //        // SwitchDate set in StopWorkPopUp
        //        EditVM.TheEntity.StartDate = SwitchDate;
        //        SwitchDate = DateTime.MinValue;
        //    }
        //    EditVM.TheEntity.RaisePropertyChanged("StartDate");

        //    mLogger.AddLogMessage("<===============================");
        //    mLogger.AddLogMessage("In StopTiming2()  <===");
        //    mLogger.AddLogMessage("ToDos SelectedIndex: " + n);
        //    ToDoVM aToDo = ToDos[2];
        //    aToDo.TheEntity.DetailedDesc = "Will this work??";
        //    UpdateDB();
        //    SelectedToDo = aToDo;
        //    SelectedToDo.TheEntity = SelectedToDo.TheEntity;
        //    RaisePropertyChanged("ToDos");
        //    mLogger.AddLogMessage("===============================>");
        //    //ToDos.


        //    //  Check for active ToDo
        //    InWorkToDo = CheckForInWork();  // returns the ToDo, if any, with 'A' , i.e. in WORK

        //    // Display StartTiming popup
        //    if (InWorkToDo == null)
        //    {
        //        // Fresh Start, nothing already in work.
        //        TitlePopUp = "Starting work on '" + EditVM.TheEntity.Item.Trim() + "' ?";
        //        TitlePopUp1 = "";
        //        // Nominal start with nothing currently in work
        //        mLogger.AddLogMessage("Popup 0: '" + TitlePopUp + "'");
        //        mLogger.AddLogMessage("---DIALOG: IsInStartTimingMode, Fresh Start.  '" + EditVM.TheEntity.Item.Trim() + "'");
        //        //EditVM.TheEntity.StartDate = SwitchDate;

        //        IsInStartTimingMode = true;  // Display PopUp for Start Timing
        //    }
        //    else
        //    {
        //        // StopStart case.  Have to stop inWorkToDo, then start work on EditVM.TheEntity.Item

        //        NextToDo = SetNextToDo();
        //        mLogger.AddLogMessage("Stopping inWork before starting another.");

        //        mLogger.AddLogMessage("InWorkToDo.TheEntity.Item = '" + InWorkToDo.TheEntity.Item + "'");
        //        mLogger.AddLogMessage("EditVM.TheEntity.Item = '" + EditVM.TheEntity.Item + "'");
        //        mLogger.AddLogMessage("NextToDo.TheEntity.Item = '" + NextToDo.TheEntity.Item + "'");
        //        mLogger.AddLogMessage("SelectedToDo.TheEntity.Item = '" + SelectedToDo.TheEntity.Item + "'");

        //        TitlePopUp = "Confirm stoping work on '" + InWorkToDo.TheEntity.Item.Trim() + "' ?**";
        //        TitlePopUp1 = "Then starting work on '" + NextToDo.TheEntity.Item.Trim() + "' ?**";

        //        IsInStopStartWorkMode = true;
        //       // StopWork();
        //        //     StopStartWork();
        //    }
        //}

        private ToDoVM SetNextToDo()
        {
            ToDoVM NextToDoVM = new ToDoVM();
            NextToDoVM.TheEntity = SelectedToDo.TheEntity;
            NextToDoVM.IsNew = false;
            return NextToDoVM;
            //throw new NotImplementedException();
        }

        //protected void StopWork2()
        //{
        //    //if (EditVM.TheEntity.Status != "A")
        //    if (SelectedToDo.TheEntity.Status != "A")
        //    {
        //        mLogger.AddLogMessage("Reached StopWork2 erroneously with '" + SelectedToDo.TheEntity.Item.Trim() + "' - " +SelectedToDo.TheEntity.Status + " --should be A ");
        //        return;
        //    }

        //    mLogger.AddLogMessage("===== Reached StopWork with '" + EditVM.TheEntity.Item.Trim() + "' - " + EditVM.TheEntity.Status + " ====>");
        //    mLogger.AddLogMessage("Reached StopWork");
        //    // Need to either Stop current item
        //    //    OR  Stop active item and then start current item.
        //    //  What is work item to be stopped?   inWorkToDo = CheckForInWork();
        //    EditVM = SelectedToDo;  // WorkItem to start work on
        //                            //     SwitchDate = DateTime.MinValue;
        //    if (inWorkToDo == null)
        //    {
        //        // Just stop the current item
        //        mLogger.AddLogMessage("Stopping work on active WorkItem.");  //  starting only
        //        TitlePopUp1 = "Stopping work on '" + EditVM.TheEntity.Item.Trim() + "' ?";
        //        TitlePopUp = "";
        //        EditVM.TheEntity.DoneDate = DateTime.Now;
        //        EditVM.TheEntity.RaisePropertyChanged("DoneDate");
        //        EditVM.TheEntity.TDTSortOrder = "999999990";// curTDTSortOrder;  // stopping single
        //        RaisePropertyChanged();
        //    }
        //    else
        //    {
        //        mLogger.AddLogMessage("Stopping inWork before starting another.");

        //        TitlePopUp = "Confirm stopping work on '" + InWorkToDo.TheEntity.Item.Trim() + "' ?";
        //        TitlePopUp1 = "Then starting work on '" + EditVM.TheEntity.Item.Trim() + "' ?";
        //        // Switch EditVM to be inWorkToDo
        //        EditVM = new ToDoVM();  // Dialog elements bound to EditVM
        //        EditVM.TheEntity = InWorkToDo.TheEntity;
        //        EditVM.TheEntity.DoneDate = InWorkToDo.TheEntity.DoneDate;  //DateTime.Now;  // Adjust to current time
        //        EditVM.TheEntity.TDTSortOrder = "000000001";//curTDTSortOrder;  // stopping by interupt
        //                                                    //      inWorkToDo.Status = "I";
        //                                                    //    EditVM.TheEntity.RaisePropertyChanged();
        //        EditVM.TheEntity.RaisePropertyChanged("DoneDate");
        //        // Have to stop inWorkToDo, then start on EditVM.TheEntity.Item
        //        // IsInStopWorkMode = true;
        //        //IsInStartTimingMode = true;
        //    }
        //    // EditVM = SelectedToDo;
        //    mLogger.AddLogMessage("Title for Popup 0 '" + TitlePopUp + "'");
        //    mLogger.AddLogMessage("Title for Popup 1 '" + TitlePopUp1 + "'");
        //    //if (EditVM.TheEntity.Status != "A")
        //    //{
        //    //    mLogger.AddLogMessage("Reached StopWork erroneously with '" + EditVM.TheEntity.Item.Trim() + "' - " + EditVM.TheEntity.Status + " ------------------>");
        //    //    return;
        //    //}
        //    mLogger.AddLogMessage("Reached StopWork with '" + EditVM.TheEntity.Item.Trim() + "' - " + EditVM.TheEntity.Status + " ------------------>");
        //    mLogger.AddLogMessage("---DIALOG: IsInStopWorkMode = true;");
        //    mLogger.AddLogMessage(" --- ");
        //    // Display StopWorkPopUp
        //    IsInStopStartWorkMode = true;
        //}


        protected void CommitStartTiming()  // Popup is up and we want to process and close
        {
            mLogger.AddLogMessage("Reached CommitStartTiming started" + "------------------>");
            // Proceeding to start work with current item

            //   SwitchAtoI();  // Need to interrupt any active work Item with Status = 'A'
            // Back to working with the current work Item

            StartTimingCurrentToDo(EditVM.TheEntity.StartDate, EditVM);

            QuitStartTiming();  // Close popup and Refresh Data
            //  IsInEditMode = false;     // close popup
            IsInStartTimingMode = false;

            // SelectedToDo = EditVM;  // Set Selected Item
        }

        private void StartTimingCurrentToDo(DateTime DoneDate, ToDoVM theToDo)
        {
            theToDo.TheEntity.Done = false;
            theToDo.TheEntity.Status = "A";
            theToDo.TheEntity.TDTSortOrder = "000000001";
            //      theToDo.TheEntity.ElapsedTime = 0.0M;
            theToDo.TheEntity.StartDate = DoneDate;   // StopStartTime;
            if (inWorkToDo != null)
            {
                // Stopped an active (InWorkToDo) so need to capture end date from dialog.
    //            SwitchDate = (DateTime)theToDo.TheEntity.DoneDate;  // from dialog
            }

            mLogger.AddLogMessage("* Changed selected ToDo to active status: '" + theToDo.TheEntity.Item.Trim() + "' - " + theToDo.TheEntity.Status);
            //int nProjectID = this.SelectedToDo.TheEntity.ProjectID;  // int.Parse(listViewToDos.FocusedItem.SubItems[6].Text);
            //int nToDoID = this.SelectedToDo.TheEntity.ToDoID;  // int.Parse(listViewToDos.FocusedItem.SubItems[8].Text);

            // Build Track record and save it
            TrackVM = new Planner.TrackVM();
            TrackVM.TheEntity.Item = theToDo.TheEntity.Item;
            TrackVM.TheEntity.ProjectID = theToDo.TheEntity.ProjectID;
            TrackVM.TheEntity.StartDate = theToDo.TheEntity.StartDate;
            TrackVM.TheEntity.DetailedDesc = theToDo.TheEntity.DetailedDesc;
            TrackVM.TheEntity.ElapsedTime = 0.0M;
            TrackVM.TheEntity.BillTime = 0.0M;
            TrackVM.TheEntity.BillRef = "tBd";
            //     TrackVM.TheEntity.EndDate = DateTime.Now;
            TrackVM.TheEntity.Expenses = 0.0M;
            TrackVM.TheEntity.Mileage = 0.0M;
            //  TrackVM.TheEntity.Project = theToDo.TheEntity.Project;
            TrackVM.TheEntity.RespPerson = theToDo.TheEntity.RespPerson;
            TrackVM.TheEntity.Status = theToDo.TheEntity.Status;  // starts out as 'A'
                                                                 // TrackVM.TheEntity.SortOrder = "000001";  // theToDo.TheEntity.TDTSortOrder;

            Tracks.Add(TrackVM);
            dbBase.Tracks.Add(TrackVM.TheEntity);
            mLogger.AddLogMessage("Created New Track with '" + TrackVM.TheEntity.Item.Trim() + "' - " + TrackVM.TheEntity.Status);

            MarkProjectStatus(theToDo.TheEntity.ProjectID ?? 0, "A");

            UpdateDB();
        }

        protected void QuitStartTiming()
        {
            mLogger.AddLogMessage("Reached QuitStartTiming " + "------->" + EditVM.TheEntity.Item);

            // Don't want any edited values to stick.
            EditVM.TheEntity.ClearErrors();
            try
            {
                //await db.Entry(EditVM.TheEntity).ReloadAsync();
                dbBase.Entry(EditVM.TheEntity).Reload();
            }
            catch (Exception ex)
            {
                mLogger.AddLogMessage("DB Problem l 966 : " + ex.Message);
            }
            //   await db.Entry(EditVM.TheEntity).ReloadAsync();
            //db.Entry(EditVM.TheEntity).Reload();
            // Force the datagrid to realise the record has changed
            EditVM.TheEntity = EditVM.TheEntity;
            //          EditVM = currentToDo;
            RaisePropertyChanged("EditVM");

          //  IsInEditMode = false;     // close popup
            IsInStartTimingMode = false;
          //  IsInStopWorkMode = false;

          //====+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        
            //    RefreshData();  // In CrudVMBaseTDT  ??
            //  Calls GetData() and sets UserMessage to 'Data Refreshed'
        }

        protected void CommitStopStartWork()
        {
            //base.CommitStopWork();  // doesn't do anything!
            mLogger.AddLogMessage("Reached CommitStopStartWork with '" + InWorkToDo.TheEntity.Item + "'");

            DateTime StopStartDate = (DateTime)InWorkToDo.TheEntity.DoneDate;  // from Dialog
            //DateTime StopStartDate = DateTime.Now;   // 
            try
            {
                SelectedToDo = InWorkToDo;
                EditVM = InWorkToDo;
                StoppingActiveToDo(StopStartDate, InWorkToDo);//UpdateDB();
           //     dbBase.ToDos.
                mLogger.AddLogMessage("InWorkToDo Status: " + InWorkToDo.TheEntity.Status);
                //Quit();

                //dbBase.Entry(InWorkToDo.TheEntity).Reload();
           //     SelectedToDo = InWorkToDo;
                InWorkToDo.TheEntity = InWorkToDo.TheEntity;
         //       RaisePropertyChanged("ToDos");  //  "ToDos"

                SelectedToDo = NextToDo;
             StartTimingCurrentToDo(StopStartDate, NextToDo);
                //==============
                NextToDo.TheEntity = NextToDo.TheEntity;
                mLogger.AddLogMessage("InWorkToDo Status: " + InWorkToDo.TheEntity.Status);
                //dbBase.Entry(InWorkToDo.TheEntity).Reload();
                //dbBase.Entry(NextToDo.TheEntity).Reload();
                //RaisePropertyChanged("ToDos");
                mLogger.AddLogMessage("InWorkToDo Status: " + InWorkToDo.TheEntity.Status);

                FinishSSW();   // Quit();
                mLogger.AddLogMessage("InWorkToDo Status: " + InWorkToDo.TheEntity.Status);
          ///      SelectedToDo = NextToDo;

                //await db.Entry(EditVM.TheEntity).ReloadAsync();
              //  dbBase.Entry(InWorkToDo.TheEntity).Reload();
               // dbBase.Entry(NextToDo.TheEntity).Reload();


            }
            catch (Exception e)
            {
                // PROBLEM
                mLogger.AddLogMessage(" ========== EXCEPTION in CommitStopStartWork =================");
                mLogger.AddLogMessage(e.InnerException.Message);
                throw;
            }
            isInStopStartWorkMode = false;
        }

        private void FinishSSW()
        {
            IsInStopStartWorkMode = false;
            //throw new NotImplementedException();
        }

        private void StoppingActiveToDo(DateTime StopDate, ToDoVM theToDo)
        {
            //var theToDo1 = (from c in dbBase.ToDos
            //                    where c.ToDoID == theToDoRef.TheEntity.ToDoID
            //                    //orderby c.ProjectID, c.StartDate descending
            //                    select c).FirstOrDefault();
            //ToDoVM theToDo = (ToDoVM)theToDoRef;
            mLogger.AddLogMessage("StoppingActiveToDo->" + theToDo.TheEntity.Item);

            var workingTrack = (from c in dbBase.Tracks
                                where c.ProjectID == theToDo.TheEntity.ProjectID
                                orderby c.ProjectID, c.StartDate descending
                                select c).First();

            // Update ToDo Item and DetailedDesc just in case editing occurred
            workingTrack.Item = theToDo.TheEntity.Item;
            workingTrack.DetailedDesc = theToDo.TheEntity.DetailedDesc;

            // Calculate time worked
            DateTime StartDate = workingTrack.StartDate;
            TimeSpan WorkTime = StopDate - StartDate;
            decimal nnn = Math.Round((decimal)WorkTime.TotalMinutes, 2);
            workingTrack.EndDate = StopDate;
            ////if (workingTrack.ElapsedTime == 0.0M)
            ////{
            ////    workingTrack.ElapsedTime = nnn;
            ////}
            ////else
            ////{
            ////    workingTrack.ElapsedTime = workingTrack.ElapsedTime + nnn;
            ////}
            workingTrack.ElapsedTime = Math.Round((decimal)WorkTime.TotalMinutes, 2);
            //  workingTrack.DetailedDesc += "\n Finished after " + workingTrack.ElapsedTime.ToString() + " minutes. ";
            mLogger.AddLogMessage(" Track Finished after " + workingTrack.ElapsedTime.ToString() + " minutes. ");

            // Update track record with some defaults
            workingTrack.RespPerson = "Henry";
            workingTrack.EndDate = StopDate;
            workingTrack.Mileage = 0.01M;
            workingTrack.BillRef = "tbd";
            workingTrack.BillTime = workingTrack.ElapsedTime;
            //    string temp = Flag;
            workingTrack.Status = "F";

            // Update ToDo record, want ElapsedTime to accumuate over work intervals for the day
            theToDo.TheEntity.ElapsedTime = theToDo.TheEntity.ElapsedTime + (decimal)workingTrack.ElapsedTime;

            //  string F = StatusOpt.ToString();
            mLogger.AddLogMessage("Adjusting Status: " + StatusOpt.ToString());
            if (StatusOpt == StatusOptions.F)
            {
                theToDo.TheEntity.Status = "F";  // mark F for Finished
                theToDo.TheEntity.Done = true;
                MarkProjectStatus(theToDo.TheEntity.ProjectID ?? 0, "F");
            }
            else if (StatusOpt == StatusOptions.C)
            {
                EditVM.TheEntity.Status = "C";  // mark C for Continuing
                EditVM.TheEntity.Done = false;
                MarkProjectStatus(EditVM.TheEntity.ProjectID ?? 0, "C");
            }
            else if (StatusOpt == StatusOptions.I)
            {
                theToDo.TheEntity.Status = "I";  // mark I for interrupted
                theToDo.TheEntity.Done = false;
                MarkProjectStatus(theToDo.TheEntity.ProjectID ?? 0, "I");
            }
            else if (StatusOpt == StatusOptions.O)
            {
                theToDo.TheEntity.Status = "O";  // mark W for partially finished, done for today
                theToDo.TheEntity.Done = false;
                MarkProjectStatus(theToDo.TheEntity.ProjectID ?? 0, "O");
            }
            else if (StatusOpt == StatusOptions.W)
            {
                EditVM.TheEntity.Status = "W";  // mark C for Continuing
                EditVM.TheEntity.Done = false;
                MarkProjectStatus(EditVM.TheEntity.ProjectID ?? 0, "W");
            }
            // theToDo.TheEntity.Status = "C";  // mark F for Finished
            theToDo.TheEntity.RespPerson = "Johnny";
            theToDo.TheEntity.DoneDate = StopDate;
            // theToDo.TheEntity.Done = true;

            //if (InWorkToDo == null)
            //{
                mLogger.AddLogMessage("--- Normal Stop Work ----");
                // Normal stop work
                SwitchDate = DateTime.MinValue;
                UpdateDB();
              //  Quit();
            //}
            //else
            //{
            //    mLogger.AddLogMessage("Stopped previous item, Now calling StartWork");
            //    // Stop theToDo then start another todo
            //    //IsInStartTimingMode = true;
            //    SwitchDate = StopDate;
            //    mLogger.AddLogMessage("SwitchDate: " + SwitchDate.ToShortTimeString());
            //    UpdateDB();
            //    //   StartTiming();
            //    mLogger.AddLogMessage("Starting new item, Now calling StartTimingCurrentToDo()!");

            //    StartTimingCurrentToDo(DateTime.Now);
            //    Quit();
            //}
        }

        protected void QuitStopStartWork()
        {
            mLogger.AddLogMessage("Reached QuitStopStartWork");
            //base.QuitStopWork();
            EditVM = SelectedToDo;
            //Quit();
            try
            {
                //await db.Entry(EditVM.TheEntity).ReloadAsync();
                dbBase.Entry(InWorkToDo.TheEntity).Reload();
                dbBase.Entry(NextToDo.TheEntity).Reload();
            }
            catch (Exception ex)
            {
                mLogger.AddLogMessage("DB Problem in QuitStopStartWork : " + ex.Message);
            }
            IsInStopStartWorkMode = false;
        }
        protected void QuitEdit()
        {
            //if (EditVM == null || EditVM.TheEntity == null)
            //{
            mLogger.AddLogMessage("Reached ToDos Quit ------------------>");
            if (!EditVM.IsNew)
            {
                EditVM.TheEntity.ClearErrors();
                try
                {
                    //await db.Entry(EditVM.TheEntity).ReloadAsync();
                    dbBase.Entry(EditVM.TheEntity).Reload();
                }
                catch (Exception ex)
                {
                    mLogger.AddLogMessage("DB Problem l 966 : " + ex.Message);
                }
                //   await db.Entry(EditVM.TheEntity).ReloadAsync();
                //db.Entry(EditVM.TheEntity).Reload();
                // Force the datagrid to realise the record has changed
                EditVM.TheEntity = EditVM.TheEntity;
                //          EditVM = currentToDo;
                RaisePropertyChanged("EditVM");
            }
            //}
            mLogger.AddLogMessage("EditVM->" + EditVM.TheEntity.Item);
            //     SelectedToDo = EditVM;
            //     RefreshData();
            //RaisePropertyChanged("SelectedToDo");
            //   inWorkToDo = null;
            //EditVM = null;
            IsInEditMode = false;
            IsInStartTimingMode = false;
            IsInStopWorkMode = false;

            IsInStopStartWorkMode = false;

            //mLogger.AddLogMessage("InWorkToDo.TheEntity.Item = '" + InWorkToDo.TheEntity.Item + "'");
            mLogger.AddLogMessage("EditVM.TheEntity.Item = '" + EditVM.TheEntity.Item + "'");
            mLogger.AddLogMessage("SelectedToDo.TheEntity.Item = '" + SelectedToDo.TheEntity.Item + "'");
        }

        protected void CommitEdit()
        {
            string t = TitlePopUp;
            mLogger.AddLogMessage("$$CommitUpdates() TitlePopUp - " + t);
            if (EditVM == null || EditVM.TheEntity == null)
            {
                mLogger.AddLogMessage("(ToDosViewModel) CommitUpdates  EditVM is null!!");
                if (dbBase.ChangeTracker.HasChanges())  // Presumed carry over when dg was directly editable
                {
                    UpdateDB();
                }
                return;
            }
            mLogger.AddLogMessage("(ToDosViewModel) CommitUpdates  EditVM->" + EditVM.TheEntity.Item);
            var theItem = EditVM.TheEntity.Item;
            //  ShowUserMessage("WORKING! in CommitUpdates");

            mLogger.AddLogMessage("Reached CommitUpdates with '" + EditVM.TheEntity.Item.Trim() + "' - '" + EditVM.TheEntity.Status + "' ------------->");
            if (EditVM.TheEntity.IsValid())
            {
                if (EditVM.IsNew)
                {
                    // Adding a new ToDo, need to have a project added in AdHocFolder to cover in Planning
                    Project newAdHocProj = AddAdHocProject();
                    newAdHocProj.Item = theItem.Trim();
                    EditVM.IsNew = false;
                    EditVM.TheEntity.ProjectID = newAdHocProj.ProjectID;
                    ToDos.Add(EditVM);
                    // Added to collection, now to add to the database.
                    dbBase.ToDos.Add(EditVM.TheEntity);
                    mLogger.AddLogMessage("Added new ToDo, calling UpdateDB. " + EditVM.TheEntity.Item);
                    UpdateDB();

                    Project theProj = dbBase.Projects.Find(EditVM.TheEntity.ProjectID);
                    theProj.Item = EditVM.TheEntity.Item;
                    theProj.DetailedDesc = EditVM.TheEntity.DetailedDesc;
                    theProj.Status = "O";  // "W";
                    ////Project projEntry = GetProjectEntry(EditVM.TheEntity);
                    ////db.Projects.Add(projEntry);
                    mLogger.AddLogMessage("Edited project, calling UpdateDB.");
                    UpdateDB();

                    SelectedToDo = EditVM;
                    Quit();
                }
                else if (dbBase.ChangeTracker.HasChanges())
                {
                    UpdateDB();
                    //  SelectedToDo = EditVM;
                    Quit();  // IsInEditMode = false;
                }
                else
                {
                    ShowUserMessage("No changes to save");
                }

            }
            else
            {
                ShowUserMessage("There are validation problems with the data entered");
                mLogger.AddLogMessage("There are validation problems with the data entered");
            }
        }


    }
}