namespace Konata.Core.Events;

internal class PacketEvent : BaseEvent
{
    public enum Type
    {
        Send,
        Receive,
    }

    public Type EventType { get; }

    public byte[] Buffer { get; }

    private PacketEvent(Type eventType, byte[] buffer)
    {
        EventType = eventType;
        Buffer = buffer;
    }

    /// <summary>
    /// Construct packet event
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    internal static PacketEvent Create(byte[] buffer)
        => new(Type.Send, buffer);

    /// <summary>
    /// Construct packet push event
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    internal static PacketEvent Push(byte[] buffer)
        => new(Type.Receive, buffer);
}
