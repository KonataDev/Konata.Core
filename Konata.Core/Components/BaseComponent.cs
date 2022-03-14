using System;
using System.Threading.Tasks;
using Konata.Core.Events;
using Konata.Core.Entity;

// ReSharper disable MemberCanBeProtected.Global

namespace Konata.Core.Components;

public class BaseComponent
{
    internal BaseEntity Entity { get; set; }
 
    internal virtual Task<bool> OnHandleEvent(BaseEvent anyEvent)
        => Task.FromResult(false);

    internal virtual Task<bool> OnHandleEvent(KonataTask anyTask)
        => OnHandleEvent(anyTask.EventPayload);

    internal void PostEventToEntity(BaseEvent anyEvent)
        => Entity?.PostEventToEntity(anyEvent);

    /// <summary>
    /// Send event (async with a return value)
    /// </summary>
    /// <param name="anyEvent"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    internal Task<BaseEvent> SendEvent<TEvent>(BaseEvent anyEvent)
        where TEvent : BaseComponent => Entity?.SendEvent<TEvent>(anyEvent);

    /// <summary>
    /// Post event (async with none return value)
    /// </summary>
    /// <param name="anyEvent"></param>
    /// <typeparam name="TEvent"></typeparam>
    internal void PostEvent<TEvent>(BaseEvent anyEvent)
        where TEvent : BaseComponent => Entity?.PostEvent<TEvent>(anyEvent);

    internal void BroadcastEvent(BaseEvent anyEvent)
        => Entity?.BroadcastEvent(anyEvent);

    internal T GetComponent<T>()
        where T : BaseComponent => Entity.GetComponent<T>();

    internal virtual void OnInit()
    {
    }

    internal virtual void OnDestroy()
    {
    }

    #region Log Methods

    private void Log(LogLevel logLevel, string tag, string content)
        => PostEventToEntity(LogEvent.Create(tag, logLevel, content));

    internal void LogV(string tag, string content)
        => Log(LogLevel.Verbose, tag, content);

    internal void LogI(string tag, string content)
        => Log(LogLevel.Information, tag, content);

    internal void LogW(string tag, string content)
        => Log(LogLevel.Warning, tag, content);

    internal void LogE(string tag, string content)
        => Log(LogLevel.Exception, tag, content);

    internal void LogE(string tag, Exception e)
        => LogE(tag, $"{e.Message}\n{e.StackTrace}");

    internal void LogF(string tag, string content)
        => Log(LogLevel.Fatal, tag, content);

    #endregion
}
