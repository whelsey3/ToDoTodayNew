using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace Planner
{
    /// <summary>
    /// Base class for all ViewModel classes displayed by TreeViewItems.  
    /// This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary>
    //public class TreeViewItemViewModel : INotifyPropertyChanged
    //[DataContract(Name = "TreeViewItemViewModel", Namespace = "http://www.contoso.com")]
    public class TreeViewItemViewModel : ViewModelBase //CrudVMBaseTDT
    {
        #region Data

       // protected TDTDbContext db = new TDTDbContext();

        static readonly TreeViewItemViewModel DummyChild = new TreeViewItemViewModel();

       // [DataMember()]
        readonly ObservableCollection<TreeViewItemViewModel> _children;
       // [DataMember()]
        TreeViewItemViewModel _parent;

      //  [DataMember()]
        string _dbID;
        public string DbID
        {
            get { return _dbID; }
            set
            {
                Set(() => DbID, ref _dbID, value);
            }
        }

      //  [DataMember()]
        string _cStatus;
        public string cStatus
        {
            get { return _cStatus; }
            set
            {
                Set(() => cStatus, ref _cStatus, value);
            }
        }

        bool _isExpanded;
        bool _isSelected;

        #endregion // Data

        #region Constructors

        protected TreeViewItemViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
        {
            _parent = parent;

            _children = new ObservableCollection<TreeViewItemViewModel>();

            if (lazyLoadChildren)
            {
                if (Parent != null)
                {
                    DummyChild.DbID = "DUMMY" + Parent.DbID;
                    _children.Add(DummyChild);
                }
            }
            ////else
            ////{
            ////    LoadChildren();
            ////}
            cStatus = "D";
            
        }

        // This is used to create the DummyChild instance.
        private TreeViewItemViewModel()
        {
           // DbID = "DUMMY" + Parent.DbID;
        }
        //used for ContextMenu add
        public TreeViewItemViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren, string theName)
        {
            _parent = parent;

            _children = new ObservableCollection<TreeViewItemViewModel>();

            DbID = theName;

        }

        #endregion // Constructors

        #region Presentation Members

        #region Children

        /// <summary>
        /// Returns the logical child items of this object.
        /// </summary>
        public ObservableCollection<TreeViewItemViewModel> Children
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
       // protected virtual void LoadChildren()
        protected virtual void LoadChildren()
        {
        }

        #endregion // LoadChildren

        #region Parent

        public TreeViewItemViewModel Parent
        {
            set { _parent = value;  }
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