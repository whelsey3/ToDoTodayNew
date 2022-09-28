using System.Windows;

namespace Planner
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        public string CategoryName
        {
            get { return txtCategory.Text; }
            set { txtCategory.Text = value; }
        }

        public InputDialog()
        {
            InitializeComponent();
            txtCategory.Focus();
        }


        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
