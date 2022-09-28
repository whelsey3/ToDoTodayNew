using System.Windows.Controls;
using System.Windows;
using CodeFirst.EFcf;

namespace Planner
{
    /// <summary>
    /// Description for ProjectsView.
    /// </summary>
    public partial class ProjectView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the ProjectsView class.
        /// </summary>
        public ProjectView()
        {
            InitializeComponent();
            //aThrobber.t
            //ThrobberVisible  = Visibility.Visible;
            bool LazyLoading = true;

           
            Folder[] folders = DatabaseHC.GetFolders();
            CountryViewModel viewModel = new CountryViewModel(folders, LazyLoading);
            this.DataContext = viewModel;
            //base.DataContext = viewModel;
            //Throbber.Visibility = Visibility
        }

        private void tv_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            object current;
            current = tv.SelectedItem;
            xTreeViewItemViewModel curItem = (xTreeViewItemViewModel)current;
        }

        private void AddNewOne(object sender, RoutedEventArgs e)
        {

        }
    }
}