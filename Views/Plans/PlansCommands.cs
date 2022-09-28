using System.Windows.Input;

namespace Planner   //.Views.Plans
{
    public class PlansCommands
    {
        static PlansCommands()
        {
            AddNewFolder = new RoutedUICommand(
                "Add New Folder", "AddNewFolder", typeof(PlansCommands),
                new InputGestureCollection
                    { new KeyGesture(Key.F, ModifierKeys.Control, "Ctrl+F") }
                );

            DeleteFolder = new RoutedUICommand(
               "Delete Folder", "DeleteFolder", typeof(PlansCommands),
               new InputGestureCollection
                   { new KeyGesture(Key.D, ModifierKeys.Control, "Ctrl+D") }
               );
        }

        public static RoutedUICommand AddNewFolder { get; private set; }
        public static RoutedUICommand DeleteFolder { get; private set; }
    }
}
