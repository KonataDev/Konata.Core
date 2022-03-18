using System.Threading;
using Konata.Core.Components;
using NUnit.Framework;

namespace Konata.Core.Test.CompTest;

public class SchedulerTest
{
    private ScheduleComponent _component;
    private const string ScheduleTestRunOnce = "Test.RunOnce";

    [SetUp]
    public void Setup()
    {
        _component = new();
    }

    [Test]
    public void TestRunOnce()
    {
        var _passed = false;
        _component.RunOnce(ScheduleTestRunOnce, 1000, () => _passed = true);

        for (;;)
        {
            Thread.Sleep(10);
            if (_passed) Assert.Pass();
        }
    }
}
