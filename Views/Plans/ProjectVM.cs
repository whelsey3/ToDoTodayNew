using BuildSqliteCF.Entity;
using System;

namespace Planner
{
    public class ProjectVM : VMBase
    {

        public Project TheEntity
        {
            get
            {
                return (Project)base.theEntity;
            }
            set
            {
                theEntity = value;
                RaisePropertyChanged();
            }
        }

        public string Selection { get; set; }
        //public string Item { get; set; }
        //public string DetailedDesc { get; set; }
        //public string Priority { get; set; }
        //public string Status { get; set; }
        //public System.DateTime RevDueDate { get; set; }
        //public string PPartNum { get; set; }
        //public string  { get; set; }
        //public string  { get; set; }
        //public string  { get; set; }
        //public string  { get; set; }
        //public string  { get; set; }

        public ProjectVM()
        {
            // Initialise the entity or inserts will fail
            Selection = "???";
            TheEntity = new Project();
            if (IsNew)
            {
                TheEntity = new Project();
                TheEntity.Status = "O";
                TheEntity.Priority = "B";
                TheEntity.Item = "namePVM";
                TheEntity.DispLevel = "1";
                TheEntity.DetailedDesc = "details";
                TheEntity.Done = false;
                TheEntity.DueDate = DateTime.Today.AddDays(5);
                TheEntity.RevDueDate = TheEntity.DueDate;
                TheEntity.StartDate = DateTime.Today.AddDays(1);
                TheEntity.RespPerson = "ELS";
                //TheEntity.TDTSortOrder = "000003";
                TheEntity.Hide = false;
                //   TheEntity.ProjectID = 1127;  // Hardcoded for "AdHoc" 
                //TheEntity.ElapsedTime = 0.00M;
            }
        }

    }
}

