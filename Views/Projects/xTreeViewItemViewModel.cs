using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using CodeFirst.EFcf;

namespace Planner
{
    /// <summary>
    /// Base class for all ViewModel classes displayed by TreeViewItems.  
    /// This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary>
    //public class TreeViewItemViewModel : INotifyPropertyChanged
    public class xTreeViewItemViewModel : ViewModelBase //CrudVMBaseTDT
    {
        #region Data

        static readonly xTreeViewItemViewModel DummyChild = new xTreeViewItemViewModel();

        readonly ObservableCollection<xTreeViewItemViewModel> _children;
        readonly xTreeViewItemViewModel _parent;

        bool _isExpanded;
        bool _isSelected;
        public string Item { get; set; }
        public string DetailedDesc { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        #endregion // Data

        #region Constructors

        protected xTreeViewItemViewModel(xTreeViewItemViewModel parent, bool lazyLoadChildren, object theUnderLyer = null)
        {
            _parent = parent;

            _children = new ObservableCollection<xTreeViewItemViewModel>();

            if (lazyLoadChildren)
            {
                _children.Add(DummyChild);
            }
            //else
            //{
            //    LoadChildren();
            //}
            if (theUnderLyer != null)
            {
                if (theUnderLyer.GetType().Name.Contains("Folder"))
                {
                    string theName = theUnderLyer.GetType().Name;
                    Item = ((Folder)theUnderLyer).FolderName;
                }
                else if (theUnderLyer.GetType().Name.Contains("Project"))
                {
                    Project projIn = (Project)theUnderLyer;
                    Item = projIn.Item;
                    Status = projIn.Status;
                    DetailedDesc = projIn.DetailedDesc;
                    Priority = projIn.Priority;
                }
                else if (theUnderLyer.GetType().Name.Contains("SubTask"))
                {
                    SubTask projIn = (SubTask)theUnderLyer;
                    Item = projIn.Item;
                    Status = projIn.Status;
                    DetailedDesc = projIn.DetailedDesc;
                    Priority = projIn.Priority;
                }
                else if (theUnderLyer.GetType().Name.Contains("Task"))
                {
                    Task projIn = (Task)theUnderLyer;
                    Item = projIn.Item;
                    Status = projIn.Status;
                    DetailedDesc = projIn.DetailedDesc;
                    Priority = projIn.Priority;
                }
                
            }
        }

        // This is used to create the DummyChild instance.
        private xTreeViewItemViewModel()
        {
        }

        #endregion // Constructors

        #region Presentation Members

        #region Children

        /// <summary>
        /// Returns the logical child items of this object.
        /// </summary>
        public ObservableCollection<xTreeViewItemViewModel> Children
        {
            get { return _children; }
        }

        #endregion // Children

        #region HasLoadedChildren

        /// <summary>
        /// Returns true if this object's Children have not yet been populated.
        /// </summary>
        public bool HasDummyChild
        {
            get { return this.Children.Count == 1 && this.Children[0] == DummyChild; }
        }

        #endregion // HasLoadedChildren

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                //if (value != _isExpanded)
                //{
                //    _isExpanded = value;
                //    this.OnPropertyChanged("IsExpanded");
                //}
                Set(() => IsExpanded, ref _isExpanded, value);

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;

                // Lazy load the child items, if necessary.
                if (this.HasDummyChild)
                {
                    this.Children.Remove(DummyChild);
                    this.LoadChildren();
                }
            }
        }

        #endregion // IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                //if (value != _isSelected)
                //{
                //    _isSelected = value;
                //    this.OnPropertyChanged("IsSelected");
                //}
                Set(() => IsSelected, ref _isSelected, value);
            }
        }

        #endregion // IsSelected

        #region LoadChildren

        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// Subclasses can override this to populate the Children collection.
        /// </summary>
        protected virtual void LoadChildren()
        {
        }

        #endregion // LoadChildren

        #region Parent

        public xTreeViewItemViewModel Parent
        {
            get { return _parent; }
        }

        #endregion // Parent

        #endregion // Presentation Members

        #region INotifyPropertyChanged Members

        //public event PropertyChangedEventHandler PropertyChanged;

        //protected virtual void OnPropertyChanged(string propertyName)
        //{
        //    if (this.PropertyChanged != null)
        //        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //}

        #endregion // INotifyPropertyChanged Members
    }
}