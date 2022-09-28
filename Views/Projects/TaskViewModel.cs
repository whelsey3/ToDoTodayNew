using CodeFirst.EFcf;

namespace Planner
{
    public class TaskViewModel : xTreeViewItemViewModel
    {
        readonly Task _task;
        bool LazyLoading;

        public TaskViewModel(Task task, ProjectViewModel parentProject, bool _LazyLoading)
            : base(parentProject, _LazyLoading, task)
        {
            _task = task;
            LazyLoading = _LazyLoading;
        }

        public string TaskName
        {
            get { return _task.Item; }
        }

        protected override void LoadChildren()
        {
            foreach (SubTask subtask in DatabaseHC.GetSubTasks(_task))
            {
                base.Children.Add(new SubTaskViewModel(subtask, this, LazyLoading));
            }
        }
    }
}