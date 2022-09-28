using BuildSqliteCF.Entity;
using GongSolutions.Wpf.DragDrop;
using System.Data.Entity;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;


namespace Planner
{
    //public partial class ToDosViewModel : CrudVMBaseTDT, IDragSource, IDropTarget // ViewModelBase
    public partial class ToDosViewModel : ViewModelBase, IDragSource, IDropTarget // ViewModelBase
    {
        public void ChkContext(string Header = " ***")
        {
            mLogger.AddLogMessage("============== ChkContext ==ToDDosViewModel============== " + Header);
            // ===========================================================
            string changeType = "";
            int historyCount = 0;

            foreach (var history in dbBase.ChangeTracker.Entries()
              .Where(e => e.Entity is IModificationHistory
              && (e.State != EntityState.Unchanged))
               .Select(e => e.Entity)
              )

            ////foreach (var history in dbBase.ChangeTracker.Entries()
            ////  .Where e => e.Entity is IModificationHistory &&
            ////   (e => e.State == EntityState.Added ||
            ////          e.State == EntityState.Modified
            ////          ||
            ////          e.State == EntityState.Unchanged))
            ////   .Select(e => e.Entity )
            ////  )
            {
                historyCount++;
                //history.DateModified = DateTime.Now;
                //if (history.DateCreated == DateTime.MinValue)
                //{
                //    history.DateCreated = DateTime.Now;
                //}
                mLogger.AddLogMessage("");
                changeType = history.GetType().ToString();
                if (changeType.Contains("Project"))
                {
                    mLogger.AddLogMessage("T2 ChkContext -" + historyCount + "  Project Changed- '" + ((Project)history).Item + "' -" + changeType);
                }
                else if (changeType.Contains("ToDo"))
                {
                    mLogger.AddLogMessage("T2 ChkContext -" + historyCount + "  ToDo Changed- '" + ((ToDo)history).Item + "' -" + changeType);
                }
                else
                {
                    mLogger.AddLogMessage("T2 ChkContext -" + historyCount + "  Track Changed- '" + ((Track)history).Item + "' -" + changeType);
                }
            }
            // ================================================================
        }
    }
}
