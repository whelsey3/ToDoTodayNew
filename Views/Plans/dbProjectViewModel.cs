using BuildSqliteCF.Entity;
using System.Data.Entity;
//using BuildSqliteCF.DbContexts;
using System.Linq;
using nuT3;

namespace Planner
{
    public class dbProjectViewModel : TreeViewItemViewModel
    {
        public readonly Project _project;
        bool LazyLoading;
        Logger mLogger = Logger.Instance;
        public TDTDbContext db;

        public dbProjectViewModel(Project project, dbFolderViewModel parentFolder, bool _LazyLoading, TDTDbContext _db)
            : base(parentFolder, _LazyLoading)
        {
            _project = project;
            db = _db;
          ///  this.DbID = project.FolderID.ToString();   
            this.DbID = project.ProjectID.ToString();   
            this.cStatus = project.Status;
            //DbID = project.Item;
            LazyLoading = _LazyLoading;
            if (!LazyLoading)
            {
                LoadChildren();
            }
    ///        mLogger.AddLogMessage("-dbProjectViewModel: " + this.ProjectName + " " + Children.Count);

        }

        public string ProjectName
        {
            get { return _project.Item; }
        }

        protected override void LoadChildren()
        {
 //           Database theDB = new Database();
         //   Project[] tempTasks = theDB.GetTasks(_project);
            foreach (Project task in GetTasks(_project))
            {
                base.Children.Add(new dbTaskViewModel(task, this, LazyLoading, db));
            }
        }

        #region GetTasks

        protected Project[] GetTasks(Project project)
        {
            // project is actually a project
            IQueryable<Project> theTasks = (from c in db.Projects
                                            where c.FolderID == project.FolderID &&
                                            c.PPartNum.Substring(0, 3) == project.PPartNum.Substring(0, 3) &&
                                            c.PPartNum.Substring(6, 3) == "000" &&
                                            c.PPartNum.Substring(3, 3) != "000"
                                            orderby c.PPartNum
                                            select c).AsNoTracking();
            int i = 0;
            Project[] tasks = theTasks.ToArray();  // 
            //int t = projects.Count();
            int t = theTasks.Count();
            //CmLogger.AddLogMessage("GetTasks count is " + t);
            Project[] tsk = new Project[t];
            foreach (var item in tasks)
            {
                tsk[i] = new Project(item.Item);//.p(CodeFirst.EFcf.Folder)theFolders[i];
                tsk[i].PPartNum = item.PPartNum;
                tsk[i].DetailedDesc = item.DetailedDesc;
                tsk[i].FolderID = item.FolderID;
                tsk[i].PSortOrder = item.PSortOrder;
                tsk[i].ProjectID = item.ProjectID;
                tsk[i].Item = item.Item;
                tsk[i].Priority = item.Priority;
                tsk[i].Status = item.Status;
                // Add other details later.
                i++;
            }
            //Task[] tasks = theProjs.ToArray();

            //LmLogger.AddLogMessage("GetTasks dbPVM " + project.Item + "-" + tsk.Count());

            return tsk;
        }

        #endregion // GetTasks

    }
}