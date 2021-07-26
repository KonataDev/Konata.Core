using System;
using System.Text;

namespace Konata.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class EventSubscribeAttribute : Attribute
    {
        public Type Event { get; set; }

        public EventSubscribeAttribute(Type type)
        {
            Event = type;
        }
    }
}
