namespace Planner
{
    public class CommandMessage
    {
        public CommandType Command { get; set; }
    }
    public enum CommandType
    {
        Insert,
        Edit,
        Delete,
        Commit,
        QuitEdit,
        CommitEdit, 
        StartTiming,
        CommitStartTiming,
        QuitStartTiming,
        Refresh,
        StopWork,
        CommitStopWork,
        QuitStopWork,
        CommitStopStartWork,
        QuitStopStartWork,
        Quit,
        CleanUp,
        Report1,
        None
    }

    public class TrackCommandMessage : CommandMessage
    { }

}
