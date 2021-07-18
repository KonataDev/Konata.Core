using System;
using System.Text;

namespace Konata.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class EventDependsAttribute : Attribute
    {
        public Type Event { get; set; }

        public EventDependsAttribute(Type type)
        {
            Event = type;
        }
    }
}
