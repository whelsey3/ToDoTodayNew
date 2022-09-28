using CodeFirst.EFcf;

namespace Planner
{
    public class ProjectViewModel : xTreeViewItemViewModel
    {
        readonly Project _project;
        bool LazyLoading;

        public ProjectViewModel(Project project, FolderViewModel parentFolder, bool _LazyLoading)
            : base(parentFolder, _LazyLoading, project)
        {
            _project = project;
            LazyLoading = _LazyLoading;
        }

        public string ProjectName
        {
            get { return _project.Item; }
        }

        protected override void LoadChildren()
        {
            Task[] tempTasks = DatabaseHC.GetTasks(_project);
            foreach (Task task in DatabaseHC.GetTasks(_project))
            {
                base.Children.Add(new TaskViewModel(task, this, LazyLoading));
            }
        }
    }
}