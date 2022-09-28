using CodeFirst.EFcf;

namespace Planner
{
    public class SubTaskViewModel : xTreeViewItemViewModel
    {
        readonly SubTask _subTask;
        bool LazyLoading;

        public SubTaskViewModel(SubTask SubTask, TaskViewModel parentTask, bool _LazyLoading)
            : base(parentTask, _LazyLoading, SubTask)
        {
            _subTask = SubTask;
            LazyLoading = _LazyLoading;
        }

        public string SubTaskName
        {
            get { return _subTask.Item; }
        }
    }
}