using CodeFirst.EFcf;
using GalaSoft.MvvmLight;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Data.Entity.Validation;
using System.Diagnostics;
using GalaSoft.MvvmLight.Messaging;

namespace Planner
{
    public partial class PlansViewModel : ViewModelBase, IDropTarget, IDragSource // CrudVMBaseTDT
    {
        private void UpdateDB()   // Update the Database
        {
            mLogger.AddLogMessage("==== UpdateDB ==PlansVMpartial===============>");

            //theDB.db.ChangeTracker.Entries();
            //  Database theDB = new Database();
            foreach (var history in db.ChangeTracker.Entries()
                          .Where(e => e.Entity is IModificationHistory && (e.State == System.Data.Entity.EntityState.Added ||
                                  e.State == System.Data.Entity.EntityState.Modified))
                           .Select(e => e.Entity )
                          )
            {
 //                           .Select(e => e.Entity as IModificationHistory)
               //history.DateModified = DateTime.Now;
                //if (history.DateCreated == DateTime.MinValue)
                //{
                //    history.DateCreated = DateTime.Now;
                //}
                Project p = history as Project;
                if (p != null)
                {
                    mLogger.AddLogMessage("PlansVM-ChangeTracker: " + p.ProjectID + "-" + p.ProjectID + "-" + history.ToString());
                }
            }
            try
            {
                int nChanges = db.SaveChanges();
                ///  ShowUserMessage("Database Updated with " + nChanges.ToString() + " changes.");
                mLogger.AddLogMessage("UpdateDB successfully completed. with " + nChanges.ToString() + " changes.");
            }
            catch (DbEntityValidationException dbEx)
            {
                // mLogger.AddLogMessage("DbEntityValidationException: =====");
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //        mLogger.AddLogMessage("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
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
                //  mLogger.AddLogMessage("DbUpdateException: " + deDbEx.InnerException.Message);
            }
            catch (Exception e)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    string ErrorMessage = e.InnerException.GetBaseException().ToString();
                }
                //   mLogger.AddLogMessage("Exception: " + e.InnerException.GetBaseException().ToString());
                ///  ShowUserMessage("There was a problem updating the database");
            }
            finally
            {
                mLogger.AddLogMessage("  ========= finally End UpdateDB in PlansView =============");
            }
        }


        // Used to control showing a pop up to edit an entity or StartTiming
        private bool isInEditMode = false;
        public bool IsInEditMode
        {
            get
            {
                return isInEditMode;
            }
            set
            {
                isInEditMode = value;
                InEdit inEdit = new InEdit { Mode = value };
                Messenger.Default.Send<InEdit>(inEdit);
                RaisePropertyChanged();
            }
        }

        // Used to indicate Adding or Editing a project
        private bool nowEditing = false;
        public bool NowEditing
        {
            get
            {
                return nowEditing;
            }
            set
            {
                nowEditing = value;
                //InEdit inEdit = new InEdit { Mode = value };
                //Messenger.Default.Send<InEdit>(inEdit);
                RaisePropertyChanged();
            }
        }

        private string titlePopUp;
        public string TitlePopUp
        {
            get
            {
                return titlePopUp;
            }
            set
            {
                titlePopUp = value;
                RaisePropertyChanged();
            }
        }

    //    protected bool isCurrentView = false;
        public bool isCurrentView = false;

        public CommandVM SaveCmd { get; set; }
        //public CommandVM CommitStartTimingCmd { get; set; }
        //public CommandVM StopWorkCmd { get; set; }
        public CommandVM QuitCmd { get; set; }

        private void CurrentUserControl(CurrentViewMessage nm)
        {
            string curObj = this.GetType().Name;
            if (this.GetType() == nm.ViewModelType)
            {
                mLogger.AddLogMessage("PlansVMpartial CurrentUserControl - isCurentView TRUE  " + curObj);
                isCurrentView = true;
            }
            else
            {
                mLogger.AddLogMessage("PlansVMpartial CurrentUserControl - isCurrentView FALSE  " + curObj);
                isCurrentView = false;
                //if (db.ChangeTracker.HasChanges())
                //{
                //    int numChanges = db.SaveChanges();
                //    mLogger.AddLogMessage("Leaving Tracks.  Saved " + numChanges);
                //}

            }
        }

        protected void HandleCommand(CommandMessage action)
        {
            if (isCurrentView)
            {
                switch (action.Command)
                {
                    case CommandType.Insert:
                        //InsertNew();
                        InsertNewFull();
                        break;
                    case CommandType.Edit:
                        if (GotSomethingSelected())
                        {
                            NowEditing = true;
                            EditCurrent();
                        }
                        break;
                    //case CommandType.StartTiming:
                    //    if (GotSomethingSelected())
                    //    {
                    //        StartTiming();
                    //    }
                    //    break;
                    //case CommandType.CommitStartTiming:
                    //    if (GotSomethingSelected())
                    //    {
                    //        CommitStartTiming();
                    //    }
                    //    break;
                    //case CommandType.QuitStartTiming:
                    //    QuitStartTiming();
                    //    break;
                    //case CommandType.StopWork:
                    //    if (GotSomethingSelected())
                    //    {
                    //        StopWork();
                    //    }
                    //    break;
                    //case CommandType.CommitStopWork:
                    //    if (GotSomethingSelected())
                    //    {
                    //        CommitStopWork();
                    //    }
                    //    break;
                    //case CommandType.QuitStopWork:
                    //    QuitStopWork();
                    //    break;
                    //case CommandType.Delete:
                    //    if (GotSomethingSelected())
                    //    {
                    //        DeleteCurrent();
                    //    }
                    //    break;
                    case CommandType.Commit:
                        CommitUpdates();
                        break;
                    case CommandType.Refresh:
                        mLogger.AddLogMessage("Refresh in PlansVMpartial %%%%%%");
                        RefreshData();
                     //   editEntity = null;
                     //   selectedEntity = null;
                        break;
                    case CommandType.Quit:
                        Quit();
                        break;
                    //case CommandType.CleanUp:
                    //    CleanUp();
                    //    break;
                    case CommandType.None:
                        break;
                    default:
                        break;
                }
            }
        }

        private void InsertNew()
        {
            //TreeViewItemViewModel parent = (TreeViewItemViewModel)tv.SelectedItem;  // GetCommandItem();

            DetailsVM = new ProjectVM();
            DetailsVM.IsNew = true;
            DetailsVM.TheEntity = SelectedProject;
            DetailsVM.TheEntity.Item = "New Work Item";
            //   DetailsVM.FolderID = SelectedProject.FolderID; // newAdHocProj.ProjectID;
            mLogger.AddLogMessage("Reached InsertNew in PlansViewModel with '" + DetailsVM.TheEntity.Item.Trim() + "' - " + DetailsVM.TheEntity.Status + "---->");
            TitlePopUp = "Adding new Planning Work Item";
            NowEditing = false;
            IsInEditMode = true;  // Opens EditPopUp
        }

        private void InsertNewFull()
        {
            // From AddNewOne
            TreeViewItemViewModel parent = selectedTVItem; // SelectedProject GetCommandItem();
            //string name = ShowInputDialog(null);
            // Set up DetailsVM to populate EditPopUp
            DetailsVM = new ProjectVM();
            DetailsVM.IsNew = true;
            DetailsVM.Selection = "Adding New Work Item";
            DetailsVM.TheEntity = SelectedProject;
            //  DetailsVM.TheEntity = SelectedProject;
            DetailsVM.TheEntity.Item = "New Work Item0";
            DetailsVM.TheEntity.DetailedDesc = "New Work Item Details0";
            RaisePropertyChanged("DetailsVM");

            //((PlansViewModel)DataContext).DetailsVM.IsNew = true;
            //      AddingNewOne(parent, "New Work Item");
            //e.Handled = true;

            //TreeViewItemViewModel parent = (TreeViewItemViewModel)tv.SelectedItem;  // GetCommandItem();

            //DetailsVM = new ProjectVM();
            //DetailsVM.IsNew = true;
            //DetailsVM.TheEntity = SelectedProject;
            //DetailsVM.TheEntity.Item = "New Work Item";
            ////   DetailsVM.FolderID = SelectedProject.FolderID; // newAdHocProj.ProjectID;
            mLogger.AddLogMessage("Reached InsertNewFull in PlansViewModel with '" + DetailsVM.TheEntity.Item.Trim() + "' - " + DetailsVM.TheEntity.Status + "--->");
            TitlePopUp = "Adding new Planning Work Item (full edit)";
            NowEditing = false;
            IsInEditMode = true;  // Opens EditPopUp
        }

        protected virtual void RefreshData()
        {
            mLogger.AddLogMessage("Reached RefreshData in PlansVM which will cause Data to be Refreshed!");
            GetData();
            Messenger.Default.Send<UserMessage>(new UserMessage { Message = "Data Refreshed" });
        }

        protected void EditCurrent()
        {
            DetailsVM = new ProjectVM();
            DetailsVM.IsNew = false;

            DetailsVM.TheEntity = SelectedProject;
            mLogger.AddLogMessage("Reached PlansView EditCurrent with '" + DetailsVM.TheEntity.Item.Trim() + "' - " + DetailsVM.TheEntity.Status + "----->");
            TitlePopUp = "Editing '" + DetailsVM.TheEntity.Item.Trim() + "' ?";
            DetailsVM.Selection = TitlePopUp;
            NowEditing = true;
            IsInEditMode = true;
        }

        private bool GotSomethingSelected()
        {
            bool OK = true;
            if (SelectedProject == null)
            {
                OK = false;
                ShowUserMessage("You must select a work item");
            }
            return OK;
        }

        private void Quit()
        {
            //if (EditVM == null || EditVM.TheEntity == null)
            //{
            mLogger.AddLogMessage("Reached Plans Edit Quit ------------------>");
            ////if (!EditVM.IsNew)
            ////{
            ////EditVM.TheEntity.ClearErrors();
            ////tryF
            ////{
            ////    //await db.Entry(EditVM.TheEntity).ReloadAsync();
            ////    db.Entry(EditVM.TheEntity).Reload();
            ////}
            ////catch (Exception ex)
            ////{
            ////    mLogger.AddLogMessage("DB Problem l 966 : " + ex.Message);
            ////}
            //////   await db.Entry(EditVM.TheEntity).ReloadAsync();
            //////db.Entry(EditVM.TheEntity).Reload();
            ////// Force the datagrid to realise the record has changed
            ////EditVM.TheEntity = EditVM.TheEntity;
            //////          EditVM = currentToDo;
            ////RaisePropertyChanged("EditVM");
            ////}
            //}
            //      mLogger.AddLogMessage("EditVM->" + EditVM.TheEntity.Item);
            //     SelectedToDo = EditVM;
            //     RefreshData();
            //RaisePropertyChanged("SelectedToDo");
            ////inWorkToDo = null;
            //////EditVM = null;
            IsInEditMode = false;  //Close the popup
            NowEditing = false;
            ////IsInStartTimingMode = false;
            ////IsInStopWorkMode = false;
        }

        private void CommitUpdates()
        {
            if (DetailsVM == null || DetailsVM.TheEntity == null)
            {
                mLogger.AddLogMessage("CommitUpdates  DetailsVM is null!!");
                if (db.ChangeTracker.HasChanges())
                {
                    UpdateDB();
                }
                return;
            }

            mLogger.AddLogMessage($"CommitUpdates  Plans Edit    DetailsVM->{DetailsVM.TheEntity.Item}");

            var x = DetailsVM.TheEntity.Item;
            //  ShowUserMessage("WORKING! in CommitUpdates");

            if (true)  //if (DetailsVM.TheEntity.IsValid())
            {
                if (DetailsVM.IsNew)
                {
                    //  Need to handle adding here:

               //     DetailsVM.IsNew = false;
                    //// Add to collection
                    //ToDos.Add(DetailsVM);
                    //// Added to collection, now to add to the database.
                    //db.ToDos.Add(DetailsVM.TheEntity);
                    //mLogger.AddLogMessage("Added new ToDo, calling UpdateDB. " + DetailsVM.TheEntity.Item);
                    //UpdateDB();

                    Project theProj = GetNewProj(DetailsVM.TheEntity.Item, selectedTVItem);
               //     Project theProj = db.Projects.Find(DetailsVM.TheEntity.ProjectID);

                    theProj.Item = DetailsVM.TheEntity.Item;
                    theProj.DetailedDesc = DetailsVM.TheEntity.DetailedDesc;
                    theProj.Status = "A";

                    DetailsVM.IsNew = false;
                    //////Project projEntry = GetProjectEntry(DetailsVM.TheEntity);
                    //////db.Projects.Add(projEntry);
                    //mLogger.AddLogMessage("Edited project, calling UpdateDB.");
                    //UpdateDB();

                    //SelectedToDo = DetailsVM;
                    //Quit();
                }
                else if (db.ChangeTracker.HasChanges())
                //else if (NowEditing)
                {
                    NowEditing = false;

                    Project theProj = db.Projects.Find(DetailsVM.TheEntity.ProjectID);
                    theProj.Item = DetailsVM.TheEntity.Item;
                    theProj.DetailedDesc = DetailsVM.TheEntity.DetailedDesc;
                    theProj.Status = "A";
                    ////Project projEntry = GetProjectEntry(DetailsVM.TheEntity);
                    ////db.Projects.Add(projEntry);
                    mLogger.AddLogMessage("Edited project, calling UpdateDB.");
                    UpdateDB();

                    SelectedProject = theProj;
                    Quit();


                    //UpdateDB();
                    ////  SelectedToDo = DetailsVM;
                    //Quit();  // IsInEditMode = false;
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

            if (db.ChangeTracker.HasChanges())
            {
                UpdateDB();
                //  SelectedToDo = DetailsVM;
                Quit();  // IsInEditMode = false;
            }

            IsInEditMode = false;
        }

        public void ShowUserMessage(string message)
        {
            mLogger.AddLogMessage("--UserMessage: '" + message + "'");
            UserMessage msg = new UserMessage { Message = message };
            Messenger.Default.Send<UserMessage>(msg);
        }

    }

}