using Konata.Utils.IO;
using Konata.Utils.JceStruct.Model;
using System.Collections.Generic;

namespace Konata.Utils.JceStruct
{
    public static class Jce
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jce"></param>
        /// <returns></returns>
        public static byte[] Serialize(JStruct jce)
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
                        buffer.TakeJceHead(tag, Type.Byte);
                        buffer.PutSbyte(((JNumber)obj).ValueByte);
                        break;
                    case Type.Short:
                        buffer.TakeJceHead(tag, Type.Short);
                        buffer.PutShortBE(((JNumber)obj).ValueShort);
                        break;
                    case Type.Int:
                        buffer.TakeJceHead(tag, Type.Int);
                        buffer.PutIntBE(((JNumber)obj).ValueInt);
                        break;
                    case Type.Long:
                        buffer.TakeJceHead(tag, Type.Long);
                        buffer.PutLongBE(((JNumber)obj).Value);
                        break;
                    case Type.Float:
                        buffer.TakeJceHead(tag, Type.Float);
                        buffer.PutFloatBE((JFloat)obj);
                        break;
                    case Type.Double:
                        buffer.TakeJceHead(tag, Type.Double);
                        buffer.PutDoubleBE((JDouble)obj);
                        break;
                    case Type.String1:
                        buffer.TakeJceHead(tag, Type.String1);
                        buffer.PutString((string)(JString)obj, ByteBuffer.Prefix.Uint8);
                        break;
                    case Type.String4:
                        buffer.TakeJceHead(tag, Type.String4);
                        buffer.PutString((string)(JString)obj, ByteBuffer.Prefix.Uint32);
                        break;
                    case Type.Map:
                        buffer.TakeJceHead(tag, Type.Map);
                        {
                            JMap map = (JMap)obj;
                            PutObject((JNumber)map.Count);
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
                        buffer.TakeJceHead(tag, Type.List);
                        {
                            JList list = (JList)obj;
                            PutObject((JNumber)list.Count);
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
                        buffer.TakeJceHead(tag, Type.StructBegin);
                        buffer.PutBytes(Serialize((JStruct)obj));
                        buffer.PutByte((byte)Type.StructEnd);
                        break;
                    case Type.ZeroTag:
                        buffer.TakeJceHead(tag, Type.ZeroTag);
                        break;
                    case Type.SimpleList:
                        buffer.TakeJceHead(tag, Type.SimpleList);
                        buffer.PutByte(0);
                        PutObject((JNumber)((JSimpleList)obj).Length);
                        buffer.PutBytes(((JSimpleList)obj).Value);
                        break;
                    case Type.Null:
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
            }

            foreach (KeyValuePair<byte, IObject> pair in jce)
            {
                PutObject(pair.Value, pair.Key);
            }
            return buffer.GetBytes();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static JStruct Deserialize(byte[] data)
        {
            Buffer buffer = new Buffer(data);

            int TakeJceInt()
            {
                buffer.PutJceHead(out Type type);
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
                        throw new System.NotImplementedException();
                }
            }

            IObject TakeJceObject(out byte tag)
            {
                tag = buffer.PutJceHead(out Type type);
                switch (type)
                {
                    case Type.Byte:
                        return (JNumber)buffer.TakeSbyte(out _);
                    case Type.Short:
                        return (JNumber)buffer.TakeShortBE(out _);
                    case Type.Int:
                        return (JNumber)buffer.TakeIntBE(out _);
                    case Type.Long:
                        return (JNumber)buffer.TakeLongBE(out _);
                    case Type.Float:
                        return (JFloat)buffer.TakeFloatBE(out _);
                    case Type.Double:
                        return (JDouble)buffer.TakeDoubleBE(out _);
                    case Type.String1:
                        return (JString)buffer.TakeString(out _, ByteBuffer.Prefix.Uint8);
                    case Type.String4:
                        return (JString)buffer.TakeString(out _, ByteBuffer.Prefix.Uint32);
                    case Type.Map:
                        {
                            int count = TakeJceInt();
                            JMap map = new JMap();
                            for (int i = 0; i < count; ++i)
                            {
                                map.Add(TakeJceObject(out _), TakeJceObject(out _));
                            }
                            return map;
                        }
                    case Type.List:
                        {
                            int count = TakeJceInt();
                            JList list = new JList(count);
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
                        return default(JNumber);
                    case Type.SimpleList:
                        buffer.EatBytes(1);
                        return (JSimpleList)buffer.TakeBytes(out _, (uint)TakeJceInt());
                    default:
                        throw new System.NotImplementedException();
                }
            }

            JStruct TakeJceStruct()
            {
                JStruct result = new JStruct();
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
