namespace Planner
{
    public class PlanDetails : BuildSqliteCF.Entity.Project //: VMBase
    {
        public string Selection { get; set; }
        public string DAction { get; set; }
        //public string Item { get; set; }
        //public string DetailedDesc { get; set; }
        //public string Priority { get; set; }
        //public string Status { get; set; }
        //public System.DateTime RevDueDate { get; set; }
        //public string PPartNum { get; set; }

        //public int ProjectID { get; set; }
        //public int FolderID { get; set; }
        //public string PSortOrder { get; set; }
        //public System.DateTime StartDate { get; set; }
        //public System.DateTime DueDate { get; set; }
        //public Nullable<System.DateTime> DoneDate { get; set; }
        //public string RespPerson { get; set; }
        //public bool Hide { get; set; }
        //public string DispLevel { get; set; }
        //public bool Done { get; set; }


        public PlanDetails()
        {

        }

    }

    //public Plan
    //private ToDoVM selectedToDo;
    //public ToDoVM SelectedToDo
    //{
    //    get
    //    {
    //        return selectedToDo;
    //    }
    //    set
    //    {
    //        selectedToDo = value;
    //        selectedEntity = value;
    //        if (value != null)
    //        {
    //            mLogger.AddLogMessage("set in ToDosViewModel SelectedToDo->" + ((ToDoVM)value).TheEntity.Item);
    //        }
    //        // RaisePropertyChanged();
    //    }
    //}

}
