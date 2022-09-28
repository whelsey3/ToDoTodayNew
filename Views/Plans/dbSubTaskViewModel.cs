using BuildSqliteCF.Entity;
using nuT3;

namespace Planner
{

    public class dbSubTaskViewModel : TreeViewItemViewModel
    {
        // SubTask is actually a Project, per database schema
        public readonly Project _subTask;
        bool LazyLoading;
        Logger mLogger = Logger.Instance;

        public dbSubTaskViewModel(Project SubTask, dbTaskViewModel parentTask, bool _LazyLoading)
            : base(parentTask, _LazyLoading)
        {
            _subTask = SubTask;
       //     DbID = SubTask.Item;
            DbID = SubTask.ProjectID.ToString();
            cStatus = SubTask.Status;
            LazyLoading = _LazyLoading;
            mLogger.AddLogMessage("---dbSubTaskViewModel: " + this.SubTaskName + " " + Children.Count);

        }

        public string SubTaskName
        {
            get { return _subTask.Item; }
        }
    }
}