using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSqliteCF.Entity;
//using BuildSqliteCF.DbContexts;
using System.Windows;

namespace Planner
{
    public class dispTrack : Track
    {
        public string FSortOrder { get; set; }
        public string FolderName { get; set; }
        public string PSortOrder { get; set; }
        public string ProjectName { get; set; }
    }
    public class reptTracks
    {
        private List<dispTrack> theTracks;
        public reptTracks()
        {
            //TDTDbContext db = new TDTDbContext("BillWork.db");
         //   string theFileName = (string)App.Current.Properties["destFilePath"];
            ///        TDTDbContext db = new TDTDbContext(theFileName);
            DateTime chkDate = Convert.ToDateTime("7/1/2019 12:00:00 AM");
            chkDate = DateTime.Today.AddDays(-8.0);

            TDTDbContext db = ((App)Application.Current).db;  // new TDTDbContext();

            // theTracks = (from c in db.Tracks orderby c.EndDate descending select c).ToList();
            var rTracks0 = (from c in db.Tracks
                           orderby c.ProjectID, c.Item, c.EndDate descending
                           where c.EndDate.Value > chkDate
                           select new { T = c }).ToList();

            var rTracks = (from c in db.Tracks
                           join d in db.Projects on c.ProjectID equals d.ProjectID
                           join e in db.Folders on d.FolderID equals e.FolderID

                           orderby e.FSortOrder, d.PSortOrder, c.Item, c.EndDate descending
                           where c.EndDate.Value > chkDate
                           select new {T = c, P = d, F = e }).ToList();

            theTracks = new List<dispTrack>();
            foreach (var fp in rTracks)
            {
                dispTrack d = new dispTrack();
                // d = (dispTrack)item;
                //d.Item = fp.T.Item;
                d.TrackID = fp.T.TrackID;
                d.ProjectID = fp.T.ProjectID;
                d.Item = fp.T.Item;
                d.StartDate = fp.T.StartDate;
                d.DetailedDesc = fp.T.DetailedDesc;
                d.ElapsedTime = fp.T.ElapsedTime;
                d.BillRef = fp.T.BillRef;
                d.BillTime = fp.T.BillTime;
                d.EndDate = fp.T.EndDate;
                d.Mileage = fp.T.Mileage;
                d.Expenses = fp.T.Expenses;
                d.RespPerson = fp.T.RespPerson;
                d.Status = fp.T.Status;
                d.ProjectName = fp.P.Item;
                d.FolderName = fp.F.FolderName;
                //d = fp.T;
                //d = fp.T;
                theTracks.Add(d);
            }

        }

        public List<dispTrack> GetTracks()
        {

            //List<dispTrack> theSummary;
            //theSummary = (dispTrack)theTracks.GroupBy(i => i.Item);
            //this.TrackSummarized();
            reptSumTracks temp = new reptSumTracks();
            return theTracks;
        }

        public void TrackSummarized()
        {
            List<dispTrack> theSummary;
            theSummary = theTracks;

            var theSummary2 = (from p in theSummary
                group p by (p.Item) into g
                select new
                {
                    // Work = Item,
                    theItem = g.Key,
                   SumTotal = g.Sum(x => x.ElapsedTime),
                   SumCount = g.Count()
                }
                );

            //theSummary = theSummary.GroupBy(i => i.Item)
            //    .Select
            return;

        }
    }

    //public class reptProjects
    //{
    //    private List<Project> theProjects;
    //    public reptProjects()
    //    {
    //        string theFileName = (string)App.Current.Properties["destFilePath"];
    //        ///        TDTDbContext db = new TDTDbContext(theFileName);
    //        TDTDbContext db = ((App)Application.Current).db;  // new TDTDbContext();

    //        theProjects = (from c in db.Projects
    //                       orderby c.FolderID, c.PPartNum
    //                       select c).ToList();
    //    }

    //    public List<Project> GetProjects()
    //    {
    //        return theProjects;
    //    }
    //}
    
    public class dispProject : Project
    {
        public string FSortOrder { get; set; }
        public string FolderName { get; set; }
    }

    public class dispProjects
    {
        private List<dispProject> theProjects;
        public dispProjects()
        {
            string theFileName = (string)App.Current.Properties["destFilePath"];
            ///         TDTDbContext db = new TDTDbContext(theFileName);
            TDTDbContext db = ((App)Application.Current).db;  // new TDTDbContext();

            //  theProjects = (from c in db.Projects select c).ToList();

            var rProjects = (from c in db.Projects
                             join d in db.Folders on c.FolderID equals d.FolderID
                             orderby d.FSortOrder, c.PPartNum
                             // into projFolders
                             select new { P = c, F = d }).ToList();
            //select projFolders).ToList();

            //theProjects = rProjects;
            theProjects = new List<dispProject>();
            foreach (var fp in rProjects)
            {
                Type t = fp.GetType();
                dispProject d = new dispProject();
                ///d = fp;
                d.Item = fp.P.Item;
                d.PPartNum = fp.P.PPartNum;
                d.ProjectID = fp.P.ProjectID;
                d.FolderID = fp.P.FolderID;
                d.FolderName = fp.F.FolderName;
                d.FSortOrder = fp.F.FSortOrder;
                d.Status = fp.P.Status;
                d.DueDate = fp.P.DueDate;
                d.Priority = fp.P.Priority;
                d.DetailedDesc = fp.P.DetailedDesc;
                theProjects.Add(d);
            }

        }

        public List<dispProject> GetProjects()
        {
            return theProjects;
        }
    }

    public class reptToDos
    {
        private List<ToDo> theToDos;
        public reptToDos()
        {
            string theFileName = (string)App.Current.Properties["destFilePath"];
            ///          TDTDbContext db = new TDTDbContext(theFileName);
            TDTDbContext db = ((App)Application.Current).db;  // new TDTDbContext();

            theToDos = (from c in db.ToDos orderby c.TDTSortOrder select c).ToList();
            var x = App.Current.Properties["dbFileName"];
        }

        public List<ToDo> GetToDos()
        {
            return theToDos;
        }
    }
}
