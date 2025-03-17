namespace BtmsScheduler;

public class RecurringTaskOption
{
    public string Schedule { get; set; }
    public string Url { get; set; }
}

public class ScheduleOptions
{
    public const string SectionName = nameof(ScheduleOptions);
    
    public Dictionary<string, RecurringTaskOption> RecurringTasks { get; set; }
}