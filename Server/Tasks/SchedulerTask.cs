using Quartz;
using Quartz.Impl;

namespace Server.Tasks;

public static class SchedulerTask
{
    private static readonly string ScheduleCronExpression = "* * * ? * * *";

    public static async Task StartAsync()
    {
        try
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            if (!scheduler.IsStarted)
            {
                await scheduler.Start();
            }

            var job1 = JobBuilder.Create<AdafruitDataLoggingJob>()
                .WithIdentity("ExecuteTaskServiceCallJob1", "group1")
                .Build();
            
            var trigger1 = TriggerBuilder.Create()
                .WithIdentity("ExecuteTaskServiceCallTrigger1", "group1")
                .WithCronSchedule(ScheduleCronExpression)
                .Build();
            
            await scheduler.ScheduleJob(job1, trigger1);
        }
        catch (Exception ex)
        {
        }
    }
}