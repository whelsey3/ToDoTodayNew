using CodeFirst.EFcf;

namespace Planner
{
    public class FolderViewModel : xTreeViewItemViewModel
    {
        Folder _folder;
        bool LazyLoading;
        string folderName;

        public FolderViewModel(Folder folder, bool _LazyLoading) 
            : base(null, _LazyLoading, folder)
        {
            _folder = folder;
            LazyLoading = _LazyLoading;
            FolderName = _folder.FolderName;
        }

        public string FolderName
        {
            get { return folderName; }
            set
            {
                folderName = value;
                RaisePropertyChanged();
            }
            //set { _folderName value  }
            //get { return _folder.FolderName; }
        }

        protected override void LoadChildren()
        {
            foreach (Project project in DatabaseHC.GetProjects(_folder))
            {
                base.Children.Add(new ProjectViewModel(project, this, LazyLoading));
            }
        }
    }
}