using System;
using Konata.Core.Utils;

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
    private TaskScheduler Instance { get; }

    /// <summary>
    /// Scheduler name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Scheduler action
    /// </summary>
    public Action<Bot> Action { get; }

    internal Scheduler(Bot bot, string name, Action<Bot> action)
    {
        Bot = bot;
        Name = name;
        Action = action;
        Instance = bot.Scheduler;
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
    /// <param name="action"><b>[In]</b> Task callback action</param>
    /// <returns></returns>
    public static Scheduler Create(Bot bot, string name, Action<Bot> action)
        => new(bot, name, action);

    /// <summary>
    /// Execute the task with a specific interval
    /// </summary>
    /// <param name="interval"><b>[In]</b> Interval in milliseconds</param>
    /// <param name="times"><b>[In]</b> Execute times</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public void Interval(int interval, int times)
        => Instance.Interval(Name, interval, times, () => Action(Bot));

    /// <summary>
    /// Execute the task with a specific interval infinity
    /// </summary>
    /// <param name="interval"><b>[In]</b> Interval in milliseconds</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public void Interval(int interval)
        => Instance.Interval(Name, interval, Infinity, () => Action(Bot));

    /// <summary>
    /// Execute the task once
    /// </summary>
    /// <param name="delay"><b>[In]</b> Delay time in milliseconds</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public void RunOnce(int delay)
        => Instance.RunOnce(Name, delay, () => Action(Bot));

    /// <summary>
    /// Execute the task once
    /// </summary>
    /// <param name="date"><b>[In]</b> Execute date</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public void RunOnce(DateTime date)
        => Instance.RunOnce(Name, date, () => Action(Bot));

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
