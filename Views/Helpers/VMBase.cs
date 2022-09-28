using GalaSoft.MvvmLight;

namespace Planner
{
    public class VMBase : ViewModelBase  // NotifyUIBase
    {
        private bool isNew = true;

        public bool IsNew
        {
            get { return isNew; }
            set
            {
                isNew = value;
                RaisePropertyChanged();
            }
        }
        private bool isSelected = false;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                RaisePropertyChanged();
            }
        }
        private bool isDeleted = false;

        public bool IsDeleted
        {
            get { return isDeleted; }
            set
            {
                isDeleted = value;
                RaisePropertyChanged();
            }
        }
        protected object theEntity;

    }
}
