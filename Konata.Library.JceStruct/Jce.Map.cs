using System;
using System.Collections.Generic;

namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public sealed class Map : SortedList<IObject, IObject>, IIndexable
        {
            public Type Type
            {
                get
                {
                    return Type.Map;
                }
            }

            public Type KeyType { get; }

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
                            path = path.Substring(dot + 1);
                            int kvdot = path.IndexOf('.');
                            if (kvdot >= 0)
                            {
                                int kv = int.Parse(path.Substring(0, kvdot));
                                path = path.Substring(kvdot + 1);
                                switch (kv)
                                {
                                case 0:
                                    if (Keys[i] is IIndexable key)
                                    {
                                        return key[path];
                                    }
                                    else
                                    {
                                        throw new InvalidCastException();
                                    }
                                case 1:
                                    if (Values[i] is IIndexable value)
                                    {
                                        return value[path];
                                    }
                                    else
                                    {
                                        throw new InvalidCastException();
                                    }
                                default:
                                    throw new ArgumentOutOfRangeException();
                                }
                            }
                            else
                            {
                                int kv = int.Parse(path);
                                switch (kv)
                                {
                                case 0:
                                    return Keys[i];
                                case 1:
                                    return Values[i];
                                default:
                                    throw new ArgumentOutOfRangeException();
                                }
                            }
                        }
                        else
                        {
                            throw new IndexOutOfRangeException();
                        }
                    }
                    else
                    {
                        int i = int.Parse(path);
                        if (i < Count)
                        {
                            return new KeyValuePair(Keys[i], Values[i]);
                        }
                        else
                        {
                            throw new IndexOutOfRangeException();
                        }
                    }
                }
            }

            public Map(Type keyType = Type.None, Type valueType = Type.None) : base()
            {
                KeyType = keyType;
                ValueType = valueType;
            }

            public Map(Type keyType, Type valueType, IDictionary<IObject, IObject> map) : base(map)
            {
                KeyType = keyType;
                ValueType = valueType;
            }

            //public long TakeNumber(IObject key)
            //{

            //}

            //public string TakeString(IObject key)
            //{

            //}

            //public Map TakeMap(IObject key)
            //{

            //}
        }
    }
}