using BuildSqliteCF.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
//using BuildSqliteCF.DbContexts;
using System.Windows;

namespace Planner
{
    public class dispTrackSum : Track
    {
        public string theItem { get; set; }
        public int SumCount { get; set; }
        public decimal? SumTotal { get; set; }
        public string Billing { get; set; }
    }
    public class reptSumTracks
    {
        private List<dispTrackSum> theTracks2;
        private List<dispTrackSum> theTracks3;
        public reptSumTracks()
        {
            DateTime chkDate = Convert.ToDateTime("7/1/2019 12:00:00 AM");
            chkDate = DateTime.Today.AddDays(-8.0);

            TDTDbContext db = ((App)Application.Current).db;  // new TDTDbContext();

            // theTracks = (from c in db.Tracks orderby c.EndDate descending select c).ToList();
            var rTracks = (from c in db.Tracks
                           orderby c.ProjectID, c.Item, c.EndDate descending
                           where c.EndDate.Value > chkDate
                           select new { T = c }).ToList();

            theTracks2 = new List<dispTrackSum>();
            foreach (var fp in rTracks)
            {
                dispTrackSum d = new dispTrackSum();
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
                theTracks2.Add(d);
            }

            //var theSummary2 = (from p in theTracks2
            var theSummary2a = (from p in theTracks2
                               group p by (p.Item) into g
                               //orderby SumTotal
                               select new
                               {
                                   // Work = Item,
                                   theItem = g.Key,
                                   //Bill = (x => x.BillRef),
                                   //Bill = x.BillRef,
                                   SumTotal = g.Sum(x => x.ElapsedTime),
                                   SumCount = g.Count()
                               }
                               );


            var theSummary2 = (from p in theSummary2a
                               orderby p.SumTotal descending
                               select p);

            theTracks3 = new List<dispTrackSum>();
            foreach (var fp in theSummary2)
            {
                dispTrackSum t = new dispTrackSum();
                t.theItem = fp.theItem;
                t.SumCount = fp.SumCount;
                t.SumTotal = fp.SumTotal;
                //t.BillRef = fp.
                // t.Billing = fp.b
                theTracks3.Add(t);
            }

        }
        public List<dispTrackSum> GetTracks()
        {
            return theTracks3;
        }
    }
}
