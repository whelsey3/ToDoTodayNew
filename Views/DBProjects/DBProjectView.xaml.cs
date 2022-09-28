using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Planner
{
    /// <summary>
    /// Description for ProjectsView.
    /// </summary>
    public partial class DBProjectView : UserControl
    {
        readonly TopViewModel _familyTree;

        /// <summary>
        /// Initializes a new instance of the ProjectsView class.
        /// </summary>
        public DBProjectView()
        {
            InitializeComponent();

            // Get raw family tree data from a database.
            TVItem rootPerson = DatabaseHC.GetFamilyTree();

            // Create UI-friendly wrappers around the 
            // raw data objects (i.e. the view-model).
            _familyTree = new TopViewModel(rootPerson);

            // Let the UI bind to the view-model.
            base.DataContext = _familyTree;

            //string test = tv.SelectedItem.ToString();
       //     ((TreeViewItem)tv.Items[0]).IsSelected = true;



            ////InitializeComponent();
            //////aThrobber.t
            //////ThrobberVisible  = Visibility.Visible;
            ////bool LazyLoading = true;
            ////Folder[] folders = Database.GetFolders();
            ////TopViewModel viewModel = new TopViewModel(folders, LazyLoading);
            ////this.DataContext = viewModel;
            //////base.DataContext = viewModel;
            //////Throbber.Visibility = Visibility
        }

        //private void tv_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        //{
        //    object current;
        //    current = tv.SelectedItem;
        //    // ((TreeViewItem)tv.Items[0]).IsSelected = true;
        //    ((TVItemViewModel)tv.Items[0]).IsExpanded = true;
        //    ((TVItemViewModel)tv.SelectedItem).IsExpanded = true;
        //}

        //private void tv_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    TreeViewItem treeViewItem = (TreeViewItem)SearchTreeView<TreeViewItem>((DependencyObject)e.OriginalSource);
        //    if (treeViewItem != null)
        //    {
        //        treeViewItem.IsSelected = true;
        //        e.Handled = true;
        //    }

        //}

        private static DependencyObject SearchTreeView<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
            {
                source = VisualTreeHelper.GetParent(source);
            }
            return source;
            //throw new NotImplementedException();
        }

        //private void tv_CMI1_Click(object sender, RoutedEventArgs e)
        //{
        //    MenuItem item = (MenuItem)e.OriginalSource;
        //    TreeViewItem selected = (TreeViewItem)tv.SelectedValue;
        //    if (item.Name.Equals("CMI1"))
        //    {
        //        string ID = selected.Name;
        //        // . . .  program specific logic
        //        ////TVItem newTVItem = new TVItem();
        //        ////newTVItem.Name = "NewOne";
        //        ////TreeViewItem newOne = new TreeViewItem(newTVItem,selected);
        //        ////selected.Children.Add(newOne);
        //    }
        //}

        private void tvDBP_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = (TreeViewItem)SearchTreeView<TreeViewItem>((DependencyObject)e.OriginalSource);
            if (treeViewItem != null)
            {
                treeViewItem.IsSelected = true;
                e.Handled = true;
            }
        }

        private void tv_CMI2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tvDBP_CMI1_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)e.OriginalSource;
            TVItemViewModel selected = (TVItemViewModel)tvDBP.SelectedValue;
            if (item.Name.Equals("CMI1"))
            {
                string ID = selected.Name;

                // . . .  program specific logic
                TVItem newTVItem = new TVItem();
                newTVItem.Name = "NewOne";
                TVItemViewModel newOne = new TVItemViewModel(newTVItem, selected);
                //selected.n
                //selected.Children.Add(newOne);
            }

        }

       
        private void tvDBP_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void tvDBP_CMI2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OnItemSelected(object sender, RoutedEventArgs e)
        {
            tvDBP.Tag = e.OriginalSource;
        }
    }
}