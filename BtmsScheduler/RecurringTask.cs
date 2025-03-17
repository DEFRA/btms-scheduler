using System.Net;
using Hangfire.Logging;

namespace BtmsScheduler;

public class RecurringTask(ILogger<RecurringTask> logger)
{
    public async Task Invoke(string id, RecurringTaskOption task)
    {
        logger.LogInformation("Invoking Recurring Task {Id}, {Schedule}, {Url}", id, task.Schedule, task.Url);

        HttpClient client = new();

        try
        {
            var result = await client.GetAsync(task.Url);
            logger.LogInformation("Recurring Task {Id} call to {Url} returned StatusCode {StatusCode}", id, task.Url, result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Recurring Task {Id} call to {Url} threw exception. {Message}", id, task.Url, ex.Message);
        }
    }
    
}