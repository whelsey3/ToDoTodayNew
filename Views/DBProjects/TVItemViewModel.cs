using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using LoggerLib;

namespace Planner
{
    /// <summary>
    /// A UI-friendly wrapper around a TVItem object.
    /// </summary>
    public class TVItemViewModel : INotifyPropertyChanged
    {
        #region Data

        //readonly ReadOnlyCollection<TVItemViewModel> _children;
        ReadOnlyCollection<TVItemViewModel> _children;
        readonly TVItemViewModel _parent;
        readonly TVItem _tVItem;

        bool _isExpanded;
        bool _isSelected;

        #endregion // Data

        #region Constructors

        public TVItemViewModel(TVItem person) : this(person, null)
        {
        }

        public TVItemViewModel(TVItem person, TVItemViewModel parent)
        {
            Logger mLogger = Logger.Instance;
            string pName = (parent == null ? "NULL" : parent.Name);
            mLogger.AddLogMessage("== TVItemViewModel->" + person.Name + " and parent ->" + pName);

            _tVItem = person;
            _parent = parent;

            var L1 = _tVItem.Children;
            var _children0 =
                    (from child in _tVItem.Children
                     select child)
                     .ToList();

            _children = new ReadOnlyCollection<TVItemViewModel>(
                    (from child in _tVItem.Children
                     select new TVItemViewModel(child, this))
                     .ToList<TVItemViewModel>());
        }

        #endregion // Constructors

        #region Person Properties

        public ReadOnlyCollection<TVItemViewModel> Children
        {
            get { return _children; }
        }

        public string Name
        {
            get { return _tVItem.Name; }
        }

        #endregion // Person Properties

        #region Presentation Members

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
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;
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
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        #endregion // IsSelected

        #region NameContainsText

        public bool NameContainsText(string text)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(this.Name))
                return false;

            return this.Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        #endregion // NameContainsText

        #region Parent

        public TVItemViewModel Parent
        {
            get { return _parent; }
        }

        #endregion // Parent

        #endregion // Presentation Members        

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }
}