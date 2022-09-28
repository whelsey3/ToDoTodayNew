using BuildSqliteCF.Entity;
//using BuildSqliteCF.DbContexts;
using GalaSoft.MvvmLight;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Data.Entity;

namespace Planner
{
    public partial class PlansViewModel : ViewModelBase, IDropTarget, IDragSource // CrudVMBaseTDT
    {
        private void AddNewOne(object sender, System.Windows.RoutedEventArgs e)
        {
            TreeViewItemViewModel theBase = GetCommandItem();
            //TreeViewItemViewModel parent = theBase.Parent;
            TreeViewItemViewModel parent = theBase;
            string name = ShowInputDialog(null);

            //////((PlansViewModel)DataContext).DetailsVM = new ProjectVM();
            //////((PlansViewModel)DataContext).DetailsVM.IsNew = true;
            //////((PlansViewModel)DataContext).DetailsVM.Selection = "Adding New One";
            ////// ((PlansViewModel)DataContext).DetailsVM.TheEntity = ((PlansViewModel)DataContext).SelectedProject;
            ////////  DetailsVM.TheEntity = SelectedProject;
            //////((PlansViewModel)DataContext).DetailsVM.TheEntity.Item = name;  // "New Work Item";

            ////////((PlansViewModel)DataContext).DetailsVM.IsNew = true;
            ((PlansViewModel)DataContext).AddingNewOne(parent, name);
            e.Handled = true;
        }
    }
}