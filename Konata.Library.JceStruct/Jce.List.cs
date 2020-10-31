using System;
using System.Collections.Generic;

namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public sealed class List : List<IObject>, IIndexable
        {
            public Type Type
            {
                get
                {
                    return Type.List;
                }
            }

            public Type ValueType { get; }

            public IObject this[string path]
            {
                get
                {
                    int dot = path.IndexOf('.');
                    if (dot >= 0)
                    {
                        int i = int.Parse(path.Substring(0, dot));
                        if (i < Count)
                        {
                            if (this[i] is IIndexable ind)
                            {
                                return ind[path.Substring(dot + 1)];
                            }
                            else
                            {
                                throw new InvalidCastException();
                            }
                        }
                        else
                        {
                            throw new KeyNotFoundException();
                        }
                    }
                    else
                    {
                        int i = int.Parse(path);
                        if (i < Count)
                        {
                            return this[i];
                        }
                        else
                        {
                            throw new KeyNotFoundException();
                        }
                    }
                }
            }

            public List(Type valueType = Type.None) : base()
            {
                ValueType = valueType;
            }

            public List(Type valueType, int count) : base(count)
            {
                ValueType = valueType;
            }

            public List(Type valueType, IEnumerable<IObject> list) : base(list)
            {
                ValueType = valueType;
            }
        }
    }
}