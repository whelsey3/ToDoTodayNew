using BuildSqliteCF.Entity;
using System;
using System.Data.Entity;
using System.Linq;

namespace Planner
{
    public static partial class Util
    {

        public static void ChkContext(TDTDbContext aContext, string Header = " ***" )
        {
            mLogger.AddLogMessage("====== ChkContext ===Util=== " + Header);
            // ===========================================================
            string changeType = "";
            int historyCount = 0;
            var temp = aContext.ChangeTracker.Entries();

            foreach (var item in temp)
            {
                string s = item.Entity.ToString();
                //item.Entity.
            }  


            foreach (var history in aContext.ChangeTracker.Entries()
              .Where(e => e.Entity is IModificationHistory
              && (e.State != EntityState.Unchanged))
               .Select(e => e.Entity as IModificationHistory)
              )

            ////foreach (var history in aContext.ChangeTracker.Entries()
            ////  .Where e => e.Entity is IModificationHistory &&
            ////   (e => e.State == EntityState.Added ||
            ////          e.State == EntityState.Modified
            ////          ||
            ////          e.State == EntityState.Unchanged))
            ////   .Select(e => e.Entity )
            ////  )
            {
                historyCount++;
                history.DateModified = DateTime.Now;
                if (history.DateCreated == DateTime.MinValue)
                {
                    history.DateCreated = history.DateModified;  // DateTime.Now;
                }
                mLogger.AddLogMessage("@ " + history);
                changeType = history.GetType().ToString();
                if (changeType.Contains("Project"))
                {
                    mLogger.AddLogMessage("T1 ChkContext -" + historyCount + "  Project Changed- '" + ((Project)history).Item + "' -" + changeType);
                }
                else if (changeType.Contains("ToDo"))
                {
                    mLogger.AddLogMessage("T2 ChkContext -" + historyCount + "  ToDo Changed- '" + ((ToDo)history).Item + "' -" + changeType);
                }
                else if (changeType.Contains("Track"))
                {
                    mLogger.AddLogMessage("T3 ChkContext -" + historyCount + "  Track Changed- '" + ((Track)history).Item + "' -" + changeType);
                }
                else
                {
                    mLogger.AddLogMessage("T4 ChkContext -" + historyCount + "  Track Changed- '" + ((Track)history).Item + "' -" + changeType);
                }

            }
            // ================================================================
        }

    }
}
