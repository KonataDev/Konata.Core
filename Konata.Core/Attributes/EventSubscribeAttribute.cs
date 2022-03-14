using System;

namespace Konata.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal class EventSubscribeAttribute : Attribute
{
    public Type Event { get; }

    public EventSubscribeAttribute(Type type)
    {
        Event = type;
    }
}
