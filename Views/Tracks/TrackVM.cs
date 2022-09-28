using BuildSqliteCF.Entity;

namespace Planner
{
    public class TrackVM : VMBase
    {

        public Track TheEntity
        {
            get
            {
                return (Track)base.theEntity;
            }
            set
            {
                theEntity = value;
                RaisePropertyChanged();
            }
        }

        public TrackVM()
        {
            // Initialise the entity or inserts will fail
            TheEntity = new Track();
        }
    }
}
