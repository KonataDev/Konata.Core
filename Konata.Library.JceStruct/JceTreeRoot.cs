using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Konata.Library.IO;

namespace Konata.Library.JceStruct
{
    using JceLeaves = Dictionary<byte, JceLeaf>;
    using JceKvMap = Dictionary<byte[], byte[]>;

    public struct JceLeaf
    {
        public byte[] data;
        public JceKvMap map;
        public JceType type;
    }

    public class JceTreeRoot
    {
        internal JceLeaves leaves;
        public delegate void JceTreeRootWriter(JceTreeRoot tree);
        public delegate void JceTreeRootReader(JceTreeRoot tree);

        public JceTreeRoot()
        {
            leaves = new JceLeaves();
        }

        public JceTreeRoot(byte[] data)
        {
            leaves = new JceLeaves();
            DeSerialize(data);
        }

        #region Add Methods
        //Jason_Ren H 

        public void AddTree(byte treeIndex, JceTreeRoot value)
        {
            AddLeafObject(treeIndex, value.Serialize().GetBytes());
        }

        public void AddTree(byte treeIndex, JceTreeRootWriter writer)
        {
            var newTree = new JceTreeRoot();
            {
                writer(newTree);
            }
            AddTree(treeIndex, newTree);
        }

        public void AddStruct(byte treeIndex, JceTreeRootWriter writer)
        {
            var newTree = new JceTreeRoot();
            {
                writer(newTree);
            }

            var leafData = newTree.Serialize().GetBytes();
            AddLeafBytes(treeIndex, JceType.StructBegin,
                leafData.Concat(JceUtils.Tag(JceType.StructEnd, 0)).ToArray());
        }

        public void AddLeafString(byte leafIndex, string value)
        {
            JceUtils.StringToJce(value, out var leafType, out var leafData);
            AddLeafBytes(leafIndex, leafType, leafData);
        }

        public void AddLeafNumber(byte leafIndex, long value)
        {
            JceUtils.NumberToJce(value, out var leafType, out var leafData);
            AddLeafBytes(leafIndex, leafType, leafData);
        }

        public void AddLeafMap<K, V>(byte leafIndex, Dictionary<K, V> value)
        {
            byte[] leafData;
            {
                if (value != null)
                {
                    var buffer = new ByteBuffer();
                    {
                        JceUtils.NumberToJce(value.Count, out var t, out var b);
                        buffer.PutBytes(JceUtils.Tag(t, 0));
                        buffer.PutBytes(b);

                        foreach (var element in value)
                        {
                            JceUtils.ObjectToJce(element.Key, out t, out b);
                            buffer.PutBytes(JceUtils.Tag(t, 0));
                            buffer.PutBytes(b);

                            JceUtils.ObjectToJce(element.Value, out t, out b);
                            buffer.PutBytes(JceUtils.Tag(t, 1));
                            buffer.PutBytes(b);
                        }
                    }
                    leafData = buffer.GetBytes();
                }
                else
                {
                    leafData = JceUtils.NumberToJce(0, out var _, out var _);
                }
            }

            AddLeafBytes(leafIndex, JceType.Map, leafData);
        }

        public void AddLeafObject(byte leafIndex, byte[] value)
        {
            JceUtils.BytesToJce(value, out var leafType, out var leafData);
            AddLeafBytes(leafIndex, leafType, leafData);
        }

        public void AddLeafFloat(byte leafIndex, float value)
        {
            JceUtils.FloatToJce(value, out var leafType, out var leafData);
            AddLeafBytes(leafIndex, leafType, leafData);
        }

        public void AddLeafDouble(byte leafIndex, double value)
        {
            JceUtils.DoubleToJce(value, out var leafType, out var leafData);
            AddLeafBytes(leafIndex, leafType, leafData);
        }

        public void AddLeafBytes(byte leafIndex, byte[] value)
        {
            AddLeafBytes(leafIndex, JceType.Byte, value);
        }

        private void AddLeafBytes(byte leafIndex, JceType type, byte[] value)
        {
            leaves.Add(leafIndex, new JceLeaf { data = value ?? new byte[0], type = type });
        }

        #endregion

        #region Get Methods

        public void GetTree(byte leafIndex, JceTreeRootReader reader)
        {
            var newTree = new JceTreeRoot(leaves[leafIndex].data);
            {
                reader(newTree);
            }
        }

        public string GetLeafString(byte leafIndex, out string value)
        {
            GetLeafBytes(leafIndex, out var leafType, out var leafData);
            if (leafType == JceType.String1 || leafType == JceType.String4)
            {
                value = Encoding.UTF8.GetString(leafData);
            }
            else
            {
                value = "";
            }

            return value;
        }

        public long GetLeafNumber(byte leafIndex, out long value)
        {
            GetLeafBytes(leafIndex, out var leafType, out var leafData);
            switch (leafType)
            {
                case JceType.Byte:
                    value = ByteConverter.BytesToInt8(leafData, 0); break;
                case JceType.Short:
                    value = ByteConverter.BytesToInt16(leafData, 0, Endian.Big); break;
                case JceType.Int:
                    value = ByteConverter.BytesToInt32(leafData, 0, Endian.Big); break;
                case JceType.Long:
                    value = ByteConverter.BytesToInt64(leafData, 0, Endian.Big); break;

                default:
                    //case JceType.ZeroTag:
                    value = 0; break;
            }
            return value;
        }

        public byte[] GetLeafBytes(byte leafIndex, out JceType type, out byte[] value)
        {
            type = leaves[leafIndex].type;
            value = leaves[leafIndex].data;
            return value;
        }

        #endregion

        public ByteBuffer Serialize()
        {
            var buffer = new ByteBuffer();
            {
                foreach (var element in leaves)
                {
                    buffer.PutBytes(JceUtils.Tag(element.Value.type, element.Key));
                    buffer.PutBytes(element.Value.data);
                }
            }

            return buffer;
        }

        public bool DeSerialize(byte[] data)
        {
            var buffer = new ByteBuffer(data);
            {
                while (buffer.RemainLength > 0)
                {
                    // 讀取JceStruct類型和索引
                    buffer.EatBytes(JceUtils.UnTag(
                        buffer.PeekBytes(2, out var _), out var jceType, out var jceTag));
                    var jceData = new byte[0];

                    // 解析JceStruct數據類型
                    switch (jceType)
                    {
                        case JceType.ZeroTag:
                            jceData = null; break;
                        case JceType.Byte:
                            buffer.TakeBytes(out jceData, 1); break;
                        case JceType.Short:
                            buffer.TakeBytes(out jceData, 2); break;
                        case JceType.Int:
                            buffer.TakeBytes(out jceData, 4); break;
                        case JceType.Long:
                            buffer.TakeBytes(out jceData, 8); break;
                        case JceType.Float:
                            buffer.TakeBytes(out jceData, 4); break;
                        case JceType.Double:
                            buffer.TakeBytes(out jceData, 8); break;
                        case JceType.String1:
                            {
                                buffer.TakeByte(out var len);
                                buffer.TakeBytes(out jceData, len);
                            }
                            break;
                        case JceType.String4:
                            {
                                buffer.TakeIntBE(out var len);
                                buffer.TakeBytes(out jceData, (uint)len);
                            }
                            break;
                        case JceType.Map:
                            {
                                leaves[jceTag] = new JceLeaf { type = jceType, map = new JceKvMap() };

                                // 解析鍵值對長度
                                buffer.EatBytes(JceUtils.JceToNumber(buffer, out var _,
                                    out var _, out var length));

                                byte[] tempData = null;

                                // 嘗試解析鍵值對
                                // 但是這裡只需要把數據解析出來
                                // 而無需構造實際的對象
                                for (int i = 0; i < length * 2; ++i)
                                {
                                    buffer.EatBytes(JceUtils.UnJceStandardType(buffer, out var kvType,
                                    out var kvTag, out var kvData));

                                    // 臨時保存 鍵數據
                                    if (i % 2 == 0)
                                        tempData = kvData;
                                    else
                                        leaves[jceTag].map.Add(tempData, kvData);
                                }

                                continue;
                            }
                            break;
                        case JceType.List: break;
                        case JceType.StructBegin: break;
                        case JceType.StructEnd: break;
                        case JceType.SimpleList:
                            {
                                buffer.EatBytes(1);

                                // 因爲tag始終為0 所以忽略
                                buffer.EatBytes(JceUtils.UnTag(
                                    buffer.PeekBytes(2, out var _), out var lenType, out var _));

                                long length = 0;

                                switch (lenType)
                                {
                                    case JceType.ZeroTag:
                                        length = buffer.TakeByte(out var _); break;
                                    case JceType.Byte:
                                        length = buffer.TakeByte(out var _); break;
                                    case JceType.Short:
                                        length = buffer.TakeShortBE(out var _); break;
                                    case JceType.Int:
                                        length = buffer.TakeShortBE(out var _); break;
                                    case JceType.Long:
                                        length = buffer.TakeShortBE(out var _); break;
                                }

                                buffer.TakeBytes(out jceData, (uint)length);
                            }
                            break;
                    }

                    leaves[jceTag] = new JceLeaf { type = jceType, data = jceData };
                }
            }

            return true;
        }
    }
}
