namespace Konata.Core.Events.Model;

public class GroupMessageBlockedEvent : BaseEvent
{
    private GroupMessageBlockedEvent() { }
    
    internal static GroupMessageBlockedEvent Push() => new();
}
