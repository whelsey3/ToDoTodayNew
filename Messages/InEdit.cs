namespace Planner
{
    public class InEdit
    {
        public bool Mode { get; set; }
    }

    public class InStartTiming
    {
        public bool Mode { get; set; }
    }
    public class InStopWork
    {
        public bool Mode { get; set; }
    }

    public class InStopStartWork
    {
        public bool Mode { get; set; }
    }

    public enum StatusOptions
    {
        // ==================================>
        F, C, I, O, A, W    // Finished, Continuing, Interrupted, Outstanding, Active, Partially finished (done for today)
    }
}
