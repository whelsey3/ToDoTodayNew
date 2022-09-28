using BuildSqliteCF.Entity;
using System.Data.Entity;
//using BuildSqliteCF.DbContexts;
using System.Linq;

namespace Planner
{
    public class dbTaskViewModel : TreeViewItemViewModel
    {
        public readonly Project _task;
        bool LazyLoading;
        LoggerLib.Logger mLogger = LoggerLib.Logger.Instance;
        public TDTDbContext db;

       public dbTaskViewModel(Project task, dbProjectViewModel parentProject, bool _LazyLoading, TDTDbContext _db)
            : base(parentProject, _LazyLoading)
        {
            _task = task;
            db = _db;
            LazyLoading = _LazyLoading;
        //    DbID = task.Item;
            DbID = task.ProjectID.ToString();
            cStatus = task.Status;
            if (!LazyLoading)
            {
                LoadChildren();
            }
      ///      mLogger.AddLogMessage("--dbTaskViewModel: " + this.TaskName + " " + Children.Count);

        }

        public string TaskName
        {
            get { return _task.Item; }
        }

        protected override void LoadChildren()
        {
  //          Database theDB = new Database();
            foreach (Project subtask in GetSubTasks(_task))
                base.Children.Add(new dbSubTaskViewModel(subtask, this, LazyLoading));
        }

        #region GetSubTasks

        public Project[] GetSubTasks(Project task)
        {
            // project is actually a task
            IQueryable<Project> theSubTasks = (from c in db.Projects
                                               where c.FolderID == task.FolderID &&
                                               c.PPartNum.Substring(0, 6) == task.PPartNum.Substring(0, 6) &&
                                               //c.PPartNum.Substring(6, 3) == "000" &&
                                               c.PPartNum.Substring(6, 3) != "000"
                                               orderby c.PPartNum
                                               select c).AsNoTracking();
            int i = 0;
            Project[] subTasks = theSubTasks.ToArray();  // 
            //int t = projects.Count();
            int t = theSubTasks.Count();
            //LmLogger.AddLogMessage("GetSubTasks count is " + t);
            Project[] subTsk = new Project[t];
            foreach (var item in subTasks)
            {
                subTsk[i] = new Project(item.Item);//.p(CodeFirst.EFcf.Folder)theFolders[i];
                subTsk[i].PPartNum = item.PPartNum;
                subTsk[i].DetailedDesc = item.DetailedDesc;
                subTsk[i].FolderID = item.FolderID;
                subTsk[i].PSortOrder = item.PSortOrder;
                subTsk[i].ProjectID = item.ProjectID;
                subTsk[i].Item = item.Item;
                subTsk[i].Status = item.Status;
                subTsk[i].Priority = item.Priority;
                i++;
            }

            //CmLogger.AddLogMessage("GetSubTasks dbTVM " + task.Item + "-" + subTsk.Count());

            return subTsk;

        }

        #endregion // GetSubTasks

    }
}