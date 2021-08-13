using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Utils.JceStruct.Model
{
    public interface IIndexable : IObject
    {
        IObject this[string path] { get; }
    }
}
