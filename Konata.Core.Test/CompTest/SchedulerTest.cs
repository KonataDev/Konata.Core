using System;
using System.Threading;
using Konata.Core.Components;
using NUnit.Framework;

namespace Konata.Core.Test.CompTest;

public class SchedulerTest
{
    private const string ScheduleTestRunOnce = "Test.RunOnce";

    [Test]
    public void TestRunOnce()
    {
        var component = new ScheduleComponent();
        {
            var task = new ManualResetEvent(false);
            component.RunOnce(ScheduleTestRunOnce, 1000, () => task.Set());
            task.WaitOne();
        }
        component.OnDestroy();
        Assert.Pass();
    }
}
