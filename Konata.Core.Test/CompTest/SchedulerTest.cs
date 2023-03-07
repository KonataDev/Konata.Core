using System.Threading;
using Konata.Core.Utils;
using NUnit.Framework;

namespace Konata.Core.Test.CompTest;

public class SchedulerTest
{
    private const string ScheduleTestRunOnce = "Test.RunOnce";

    [Test]
    public void TestRunOnce()
    {
        using var scheduler = new TaskScheduler();
        {
            var task = new ManualResetEvent(false);
            scheduler.RunOnce(ScheduleTestRunOnce, 1000, () => task.Set());
            task.WaitOne();
        }

        Assert.Pass();
    }
}
