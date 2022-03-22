using System;
using System.Threading.Tasks;
using Konata.Core.Events;
using Konata.Core.Entity;

// ReSharper disable MemberCanBeProtected.Global

namespace Konata.Core.Components;

internal class BaseComponent
{
    public BaseEntity Entity { get; set; }
 
    public virtual Task<bool> OnHandleEvent(BaseEvent anyEvent)
        => Task.FromResult(false);

    public virtual Task<bool> OnHandleEvent(KonataTask anyTask)
        => OnHandleEvent(anyTask.EventPayload);

    public void PostEventToEntity(BaseEvent anyEvent)
        => Entity?.PostEventToEntity(anyEvent);

    /// <summary>
    /// Send event (async with a return value)
    /// </summary>
    /// <param name="anyEvent"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    public Task<BaseEvent> SendEvent<TEvent>(BaseEvent anyEvent)
        where TEvent : BaseComponent => Entity?.SendEvent<TEvent>(anyEvent);

    /// <summary>
    /// Post event (async with none return value)
    /// </summary>
    /// <param name="anyEvent"></param>
    /// <typeparam name="TComp"></typeparam>
    public void PostEvent<TComp>(BaseEvent anyEvent)
        where TComp : BaseComponent => Entity?.PostEvent<TComp>(anyEvent);

    public void BroadcastEvent(BaseEvent anyEvent)
        => Entity?.BroadcastEvent(anyEvent);

    public TComp GetComponent<TComp>()
        where TComp : BaseComponent => Entity.GetComponent<TComp>();

    public virtual void OnInit()
    {
    }

    public virtual void OnDestroy()
    {
    }

    #region Log Methods

    private void Log(LogLevel logLevel, string tag, string content)
        => PostEventToEntity(LogEvent.Create(tag, logLevel, content));

    public void LogV(string tag, string content)
        => Log(LogLevel.Verbose, tag, content);

    public void LogI(string tag, string content)
        => Log(LogLevel.Information, tag, content);

    public void LogW(string tag, string content)
        => Log(LogLevel.Warning, tag, content);

    public void LogE(string tag, string content)
        => Log(LogLevel.Exception, tag, content);

    public void LogE(string tag, Exception e)
        => LogE(tag, $"{e.Message}\n{e.StackTrace}");

    public void LogF(string tag, string content)
        => Log(LogLevel.Fatal, tag, content);

    #endregion
}

internal class InternalComponent : BaseComponent
{
    public Bot Bot
        => (Bot) Entity;

    public ConfigComponent ConfigComponent
        => Bot.ConfigComponent;

    public BusinessComponent BusinessComponent
        => Bot.BusinessComponent;

    public PacketComponent PacketComponent
        => Bot.PacketComponent;

    public SocketComponent SocketComponent
        => Bot.SocketComponent;

    public ScheduleComponent ScheduleComponent
        => Bot.ScheduleComponent;

    public HighwayComponent HighwayComponent
        => Bot.HighwayComponent;
}
