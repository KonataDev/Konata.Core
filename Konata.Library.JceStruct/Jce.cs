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

            void PutObject(IObject obj, byte tag = 0)
            {
                if (obj is null)
                {
                    return;
                }
                switch (obj.Type)
                {
                    case Type.Byte:
                        buffer.PutJceHead(tag, Type.Byte);
                        buffer.PutSbyte(((Number)obj).ValueByte);
                        break;
                    case Type.Short:
                        buffer.PutJceHead(tag, Type.Short);
                        buffer.PutShortBE(((Number)obj).ValueShort);
                        break;
                    case Type.Int:
                        buffer.PutJceHead(tag, Type.Int);
                        buffer.PutIntBE(((Number)obj).ValueInt);
                        break;
                    case Type.Long:
                        buffer.PutJceHead(tag, Type.Long);
                        buffer.PutLongBE(((Number)obj).Value);
                        break;
                    case Type.Float:
                        buffer.PutJceHead(tag, Type.Float);
                        buffer.PutFloatBE((Float)obj);
                        break;
                    case Type.Double:
                        buffer.PutJceHead(tag, Type.Double);
                        buffer.PutDoubleBE((Double)obj);
                        break;
                    case Type.String1:
                        buffer.PutJceHead(tag, Type.String1);
                        buffer.PutString((string)(String)obj, ByteBuffer.Prefix.Uint8);
                        break;
                    case Type.String4:
                        buffer.PutJceHead(tag, Type.String4);
                        buffer.PutString((string)(String)obj, ByteBuffer.Prefix.Uint32);
                        break;
                    case Type.Map:
                        buffer.PutJceHead(tag, Type.Map);
                        {
                            Map map = (Map)obj;
                            PutObject((Number)map.Count);
                            if (map.Count > 0)
                            {
                                foreach (KeyValuePair<IObject, IObject> pair in map)
                                {
                                    PutObject(pair.Key);
                                    PutObject(pair.Value, 1);
                                }
                            }
                        }
                        break;
                    case Type.List:
                        buffer.PutJceHead(tag, Type.List);
                        {
                            List list = (List)obj;
                            PutObject((Number)list.Count);
                            if (list.Count > 0)
                            {
                                foreach (IObject value in list)
                                {
                                    PutObject(value);
                                }
                            }
                        }
                        break;
                    case Type.StructBegin:
                        buffer.PutJceHead(tag, Type.StructBegin);
                        buffer.PutBytes(Serialize((Struct)obj));
                        buffer.PutByte((byte)Type.StructEnd);
                        break;
                    case Type.ZeroTag:
                        buffer.PutJceHead(tag, Type.ZeroTag);
                        break;
                    case Type.SimpleList:
                        buffer.PutJceHead(tag, Type.SimpleList);
                        buffer.PutJceHead(0, Type.Byte);
                        PutObject((Number)((SimpleList)obj).Length);
                        buffer.PutBytes(((SimpleList)obj).Value);
                        break;
                    case Type.Null:
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            foreach (KeyValuePair<byte, IObject> pair in jce)
            {
                PutObject(pair.Value, pair.Key);
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

            IObject TakeJceObject(out byte tag)
            {
                tag = buffer.TakeJceHead(out Type type);
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
                            Map map = new Map();
                            for (int i = 0; i < count; ++i)
                            {
                                map.Add(TakeJceObject(out _), TakeJceObject(out _));
                            }
                            return map;
                        }
                    case Type.List:
                        {
                            int count = TakeJceInt();
                            List list = new List(count);
                            for (int i = 0; i < count; ++i)
                            {
                                list.Add(TakeJceObject(out _));
                            }
                            return list;
                        }
                    case Type.StructBegin:
                        return TakeJceStruct();
                    case Type.StructEnd:
                        return null; // Null object is only allowed here.
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
                    if (obj is null) // Meets JceType.StructEnd.
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