using System;
using Konata.Core.Components.Model;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Konata.Core.Common;

/// <summary>
/// Task Scheduler
/// </summary>
public class Scheduler
{
    public const int Infinity = int.MaxValue;

    /// <summary>
    /// Bot
    /// </summary>
    private Bot Bot { get; }

    /// <summary>
    /// Scheduler entity
    /// </summary>
    private ScheduleComponent Instance { get; }

    /// <summary>
    /// Scheduler name
    /// </summary>
    public string Name { get; }

    internal Scheduler(Bot bot, string name)
    {
        Bot = bot;
        Name = name;
        Instance = bot.ScheduleComponent;
    }

    /// <summary>
    /// Cancel the task
    /// </summary>
    ~Scheduler() => Cancel();
    
    /// <summary>
    /// Create a task scheduler
    /// </summary>
    /// <param name="bot"><b>[In]</b> Bot instance</param>
    /// <param name="name"><b>[In]</b> Task identity name</param>
    /// <returns></returns>
    public static Scheduler Create(Bot bot, string name)
        => new(bot, name);

    /// <summary>
    /// Execute the task with a specific interval
    /// </summary>
    /// <param name="interval"><b>[In]</b> Interval in milliseconds</param>
    /// <param name="times"><b>[In]</b> Execute times</param>
    /// <param name="action"><b>[In]</b> Callback action</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public void Interval(int interval, int times, Action action)
        => Instance.Interval(Name, interval, times, action);

    /// <summary>
    /// Execute the task with a specific interval infinity
    /// </summary>
    /// <param name="interval"><b>[In]</b> Interval in milliseconds</param>
    /// <param name="action"><b>[In]</b> Callback action</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public void Interval(int interval, Action action)
        => Instance.Interval(Name, interval, Infinity, action);

    /// <summary>
    /// Execute the task once
    /// </summary>
    /// <param name="delay"><b>[In]</b> Delay time in milliseconds</param>
    /// <param name="action"><b>[In]</b> Callback action</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public void RunOnce(int delay, Action action)
        => Instance.RunOnce(Name, delay, action);

    /// <summary>
    /// Execute the task once
    /// </summary>
    /// <param name="date"><b>[In]</b> Execute date</param>
    /// <param name="action"><b>[In]</b> Callback action</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public void RunOnce(DateTime date, Action action)
        => Instance.RunOnce(Name, date, action);

    /// <summary>
    /// Trigger a task to run
    /// </summary>
    public void Trigger()
        => Instance.Trigger(Name);

    /// <summary>
    /// Cancel the task
    /// </summary>
    /// <exception cref="ObjectDisposedException"></exception>
    public void Cancel()
        => Instance.Cancel(Name);
}
