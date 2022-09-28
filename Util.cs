using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BuildSqliteCF.Entity;
//using LoggerLib;
using nuT3;
//using BuildSqliteCF.DbContexts;

namespace Planner
{
    public static partial class Util
    {
        //public static TDTDbContext db ;
        public static Logger mLogger;
        static Util()
        {
            //db = new TDTDbContext();
            ////db = (TDTDbContext)App.Current.Resources["theData"];
            mLogger = Logger.Instance;

        }
        public static bool CheckDeleteOK(string msg = "Message")
        {
            if (MessageBox.Show(msg, "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                return true;
            }
            else
                return false;
        }

        internal static void AdjustProjStatusForToDoDelete(ToDo toDo, TDTDbContext db)
        {
            Project toDoProj = db.Projects.Find(toDo.ProjectID);
            if (toDoProj != null)
            {
                mLogger.AddLogMessage("Changing project '" + toDoProj.Item + "' status to '" + toDo.Status + "'");
                toDoProj.Status = " ";  // toDo.Status;
            }
        }

//        public static string newPPartNumForProj(int theFolderID, TDTDbContext theDB)
        public static string newPPartNumForProj(int? theFolderID, TDTDbContext db)
        {
            string PartNum = "";
            TDTDbContext theDB = db;
            //using (TDTDbContext theDB = new TDTDbContext())
            //{
                string theProjs = (from c in theDB.Projects
                                   where c.FolderID == theFolderID &&
                                         c.PPartNum.Substring(3, 6) == "000000"
                                   orderby c.PPartNum descending
                                   select c.PPartNum.Substring(0, 3)).FirstOrDefault();
                //Project[] projects = theProjs.ToArray();

                 PartNum = newNum(theProjs) + "000000";
            //}
            mLogger.AddLogMessage("NewProject.PPartNum: " + PartNum + " FolderID: " + theFolderID);
            return PartNum;
        }

        public static string newNum(string lastNum)
        {
            // Increment the old maximum and format with leading zeros
            int nNew = Convert.ToInt32(lastNum);
            nNew++;
            nNew = nNew + 1000;
            string newNumStr = nNew.ToString().Substring(1, 3);
            return newNumStr;
            //throw new NotImplementedException();
        }

        public static void DownTheTree()
        {

        }

    }
}
