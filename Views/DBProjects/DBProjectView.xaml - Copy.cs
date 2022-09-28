using System.Windows.Controls;
using System.Windows;

namespace Starter
{
    /// <summary>
    /// Description for ProjectsView.
    /// </summary>
    public partial class DBProjectView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the ProjectsView class.
        /// </summary>
        public DBProjectView()
        {
            InitializeComponent();
            //aThrobber.t
            //ThrobberVisible  = Visibility.Visible;
            bool LazyLoading = true;
            Folder[] folders = Database.GetFolders();
            TopViewModel viewModel = new TopViewModel(folders, LazyLoading);
            this.DataContext = viewModel;
            //base.DataContext = viewModel;
            //Throbber.Visibility = Visibility
        }

        private void tv_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            object current;
            current = tv.SelectedItem;
        }
    }
}