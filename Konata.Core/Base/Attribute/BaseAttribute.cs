using System;

namespace Konata.Core.Base
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =true,Inherited =false)]
    public class BaseAttribute:Attribute
    {
        public Type AttributeType { get; private set; }

        public BaseAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }
}
