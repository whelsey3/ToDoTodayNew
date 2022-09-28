using BuildSqliteCF.Entity;
using System.Data.Entity;
using System.Linq;
//using BuildSqliteCF.DbContexts;
using nuT3;

namespace Planner
{
    //[DataContract(Name = "aFolders", Namespace = "http://www.contoso.com")]
    public class dbFolderViewModel : TreeViewItemViewModel
    {
        //[DataMember()]
        public readonly Folder _folder;
        //[DataMember()]
        bool LazyLoading;
        TDTDbContext db;
        Logger mLogger = Logger.Instance;

        public dbFolderViewModel(Folder folder, bool _LazyLoading, TDTDbContext _db) 
            : base(null, _LazyLoading)
        {
            _folder = folder;
            db = _db;
            this.DbID = "F" + folder.FolderID.ToString();
            LazyLoading = _LazyLoading;
            if (!LazyLoading)
            {
                LoadChildren();
            }
        ///    mLogger.AddLogMessage("dbFolderViewModel: " + this.FolderName + " Children: " + Children.Count);
        }

       // [DataMember()]
        public string FolderName
        {
            get { return _folder.FolderName; }
        }

        protected override void LoadChildren()
        {
            //           Database theDB = new Database();
            //if (HasSubFolders)
            //{

            //}
            foreach (Project project in GetProjects(_folder))
            {
                base.Children.Add(new dbProjectViewModel(project, this, LazyLoading, db));
            }
        }

        #region GetProjects

        protected Project[] GetProjects(Folder folder)
        {
            IQueryable<Project> theProjs = (from c in db.Projects
                                            where c.FolderID == folder.FolderID &&
                                                  c.PPartNum.Substring(3, 6) == "000000"
                                            orderby c.PPartNum
                                            select c).AsNoTracking();
            Project[] projects = theProjs.ToArray();

            //CmLogger.AddLogMessage("GetProjects dbFVM " + folder.FolderName + "-" + projects.Count());

            return projects;
        }

        #endregion // GetProjects

    }
}