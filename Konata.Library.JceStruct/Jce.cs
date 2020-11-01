using Konata.Library.IO;
using System;
using System.Collections.Generic;

namespace Konata.Library.JceStruct
{
    public static partial class Jce
    {
        public static byte[] Serialize(Struct jce)
        {
            Buffer buffer = new Buffer();

            void PutObject(byte tag, IObject obj, bool writeHead = true)
            {
                if (writeHead)
                {
                    buffer.PutJceHead(tag, obj.Type);
                }
                switch (obj.Type)
                {
                case Type.Byte:
                    buffer.PutSbyte(((Number)obj).ValueByte);
                    break;
                case Type.Short:
                    buffer.PutShortBE(((Number)obj).ValueShort);
                    break;
                case Type.Int:
                    buffer.PutIntBE(((Number)obj).ValueInt);
                    break;
                case Type.Long:
                    buffer.PutLongBE((Number)obj);
                    break;
                case Type.Float:
                    buffer.PutFloatBE((Float)obj);
                    break;
                case Type.Double:
                    buffer.PutDoubleBE((Double)obj);
                    break;
                case Type.String1:
                    buffer.PutString((String)obj, ByteBuffer.Prefix.Uint8);
                    break;
                case Type.String4:
                    buffer.PutString((String)obj, ByteBuffer.Prefix.Uint32);
                    break;
                case Type.Map:
                    {
                        Map map = (Map)obj;
                        PutObject(0, (Number)map.Count);
                        if (map.Count > 0)
                        {
                            buffer.PutJceHead(0, map.KeyType);
                            foreach (IObject key in map.Keys)
                            {
                                PutObject(0, key, false);
                            }
                            buffer.PutJceHead(1, map.ValueType);
                            foreach (IObject value in map.Values)
                            {
                                PutObject(1, value, false);
                            }
                        }
                    }
                    break;
                case Type.List:
                    {
                        List list = (List)obj;
                        PutObject(0, (Number)list.Count);
                        if (list.Count > 0)
                        {
                            buffer.PutJceHead(0, list.ValueType);
                            foreach (IObject value in list)
                            {
                                PutObject(0, value, false);
                            }
                        }
                    }
                    break;
                case Type.StructBegin:
                    buffer.PutBytes(Serialize((Struct)obj));
                    buffer.PutByte((byte)Type.StructEnd);
                    break;
                case Type.ZeroTag:
                    break;
                case Type.SimpleList:
                    buffer.PutByte(0);
                    PutObject(0, (Number)((SimpleList)obj).Length);
                    buffer.PutBytes(((SimpleList)obj).Value);
                    break;
                default:
                    throw new NotImplementedException();
                }
            }

            foreach (KeyValuePair<byte, IObject> pair in jce)
            {
                PutObject(pair.Key, pair.Value);
            }
            return buffer.GetBytes();
        }

        public static Struct Deserialize(byte[] data)
        {
            Buffer buffer = new Buffer(data);

            int TakeJceInt()
            {
                buffer.TakeJceHead(out Type type);
                switch (type)
                {
                case Type.Byte:
                    return buffer.TakeSbyte(out _);
                case Type.Short:
                    return buffer.TakeShortBE(out _);
                case Type.Int:
                    return buffer.TakeIntBE(out _);
                case Type.ZeroTag:
                    return 0;
                default:
                    throw new NotImplementedException();
                }
            }

            IObject TakeJceObject(out byte tag, Type type = Type.None)
            {
                tag = type == Type.None ? buffer.TakeJceHead(out type) : (byte)0;
                switch (type)
                {
                case Type.Byte:
                    return (Number)buffer.TakeSbyte(out _);
                case Type.Short:
                    return (Number)buffer.TakeShortBE(out _);
                case Type.Int:
                    return (Number)buffer.TakeIntBE(out _);
                case Type.Long:
                    return (Number)buffer.TakeLongBE(out _);
                case Type.Float:
                    return (Float)buffer.TakeFloatBE(out _);
                case Type.Double:
                    return (Double)buffer.TakeDoubleBE(out _);
                case Type.String1:
                    return (String)buffer.TakeString(out _, ByteBuffer.Prefix.Uint8);
                case Type.String4:
                    return (String)buffer.TakeString(out _, ByteBuffer.Prefix.Uint32);
                case Type.Map:
                    {
                        int count = TakeJceInt();
                        if (count > 0)
                        {
                            buffer.TakeJceHead(out Type keyType);
                            List keys = new List(keyType, count);
                            for (int i = 0; i < count; ++i)
                            {
                                keys.Add(TakeJceObject(out _, keyType));
                            }
                            buffer.TakeJceHead(out Type valueType);
                            List values = new List(valueType, count);
                            for (int i = 0; i < count; ++i)
                            {
                                values.Add(TakeJceObject(out _, valueType));
                            }
                            Map map = new Map(keyType, valueType);
                            for (int i = 0; i < count; ++i)
                            {
                                map.Add(keys[i], values[i]);
                            }
                            return map;
                        }
                        return new Map();
                    }
                case Type.List:
                    {
                        int count = TakeJceInt();
                        if (count > 0)
                        {
                            buffer.TakeJceHead(out Type valueType);
                            List list = new List(valueType, count);
                            for (int i = 0; i < count; ++i)
                            {
                                list.Add(TakeJceObject(out _, valueType));
                            }
                            return list;
                        }
                        return new List();
                    }
                case Type.StructBegin:
                    return TakeJceStruct();
                case Type.StructEnd:
                    return null;
                case Type.ZeroTag:
                    return default(Number);
                case Type.SimpleList:
                    buffer.EatBytes(1);
                    return (SimpleList)buffer.TakeBytes(out _, (uint)TakeJceInt());
                default:
                    throw new NotImplementedException();
                }
            }

            Struct TakeJceStruct()
            {
                Struct result = new Struct();
                while (buffer.RemainLength > 0)
                {
                    IObject obj = TakeJceObject(out byte tag);
                    if (obj == null) // Meets JceType.StructEnd
                    {
                        break;
                    }
                    else
                    {
                        result.Add(tag, obj);
                    }
                }
                return result;
            }

            return TakeJceStruct();
        }
    }
}