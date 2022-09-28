using BuildSqliteCF.Entity;
using System;

namespace Planner
{
    public class ToDoVM : VMBase
    {

        public ToDo TheEntity
        {
            get
            {
                return (ToDo)base.theEntity;
            }
            set
            {
                theEntity = value;
                RaisePropertyChanged();
            }
        }

        public ToDoVM()
        {
            // Initialise the entity or inserts will fail
            TheEntity = new ToDo();
            if (IsNew)   // Defaults to true in VMBase default constructor
            {
                TheEntity.Status = "O";
                TheEntity.Priority = "B";
                TheEntity.Item = "new ToDo";
                TheEntity.DispLevel = "1";
                TheEntity.DetailedDesc = "details";
                TheEntity.Done = false;
                TheEntity.DueDate = DateTime.Today.AddDays(5);
                TheEntity.RevDueDate = TheEntity.DueDate;
                TheEntity.StartDate = DateTime.Today.AddDays(1);
                TheEntity.RespPerson = "????";  // "ELS";
                TheEntity.TDTSortOrder = "000000003";    // Default for ADD
                TheEntity.Hide = false;
                //   TheEntity.ProjectID = 1127;  // Hardcoded for "AdHoc" 
                TheEntity.ElapsedTime = 0.0M;
            }
        }

    }
}
