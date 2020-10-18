using System;
using System.Text;
using System.Collections.Generic;
using Konata.Library.IO;

namespace Konata.Library.Protobuf
{
    using ProtoLeaves = Dictionary<string, ProtoLeaf>;

    public struct ProtoLeaf
    {
        public byte[] data;
        public bool needLength;
    }

    public class ProtoTreeRoot
    {
        internal ProtoLeaves leaves;
        public delegate void TreeRootWriter(ProtoTreeRoot tree);
        public delegate void TreeRootReader(ProtoTreeRoot tree);

        public ProtoTreeRoot()
        {
            leaves = new ProtoLeaves();
        }

        public ProtoTreeRoot(byte[] data, bool recursion = false)
        {
            leaves = new ProtoLeaves();
            DeSerialize("", data, recursion);
        }

        #region Add Methods

        public void addTree(string treePath, ProtoTreeRoot value)
        {
            addLeafByteBuffer(treePath, value.Serialize());
        }

        public void addTree(string treePath, TreeRootWriter writer)
        {
            var newTree = new ProtoTreeRoot();
            {
                writer(newTree);
            }
            addTree(treePath, newTree);
        }

        public void addLeafString(string leafPath, string value)
        {
            addLeafBytes(leafPath, Encoding.UTF8.GetBytes(value));
        }

        public void addLeafFix32(string leafPath, int value)
        {
            addLeafBytes(leafPath, ByteConverter.Int32ToBytes(value), false);
        }

        public void addLeafFix64(string leafPath, long value)
        {
            addLeafBytes(leafPath, ByteConverter.Int64ToBytes(value), false);
        }

        public void addLeafVar(string leafPath, long value)
        {
            addLeafBytes(leafPath, ByteConverter.NumberToVarint(value), false);
        }

        public void addLeafEmpty(string leafPath)
        {
            addLeafBytes(leafPath, null, false);
        }

        public void addLeafByteBuffer(string leafPath, ByteBuffer value)
        {
            addLeafBytes(leafPath, value.GetBytes());
        }

        public void addLeafBytes(string leafPath, byte[] value)
        {
            addLeafBytes(leafPath, value ?? new byte[0], true);
        }

        public void addLeafBytes(string leafPath, byte[] value, bool needLength)
        {
            leaves.Add(leafPath, new ProtoLeaf { data = value, needLength = needLength });
        }

        #endregion

        #region Get Methods

        public void getTree(string treePath, TreeRootReader reader)
        {
            var newTree = new ProtoTreeRoot(leaves[treePath].data);
            {
                reader(newTree);
            }
        }

        public string getLeafString(string leafPath, out string value)
        {
            return value = Encoding.UTF8.GetString(getLeafBytes(leafPath, out var _));
        }

        public int getLeafFix32(string leafPath, out int value)
        {
            return value = ByteConverter.BytesToInt32(getLeafBytes(leafPath, out var _), 0, Endian.Little);
        }

        public long getLeafFix64(string leafPath, out long value)
        {
            return value = ByteConverter.BytesToInt64(getLeafBytes(leafPath, out var _), 0, Endian.Little);
        }

        public long getLeafVar(string leafPath, out long value)
        {
            return value = ByteConverter.VarintToNumber(getLeafBytes(leafPath, out var _));
        }

        public byte[] getLeafBytes(string leafPath, out byte[] value)
        {
            return value = leaves[leafPath].data;
        }

        #endregion

        public ByteBuffer Serialize()
        {
            var buffer = new ByteBuffer();
            {
                foreach (var element in leaves)
                {
                    var split = element.Key.Split('.');
                    if (split.Length <= 0)
                        continue;

                    buffer.PutBytes(ByteConverter.UnHex(split[split.Length - 1]));
                    if (element.Value.needLength)
                        buffer.PutBytes(ByteConverter.NumberToVarint(element.Value.data.Length));
                    buffer.PutBytes(element.Value.data);
                }
            }

            return buffer;
        }

        private bool DeSerialize(string treePath, byte[] data, bool recursion)
        {
            var buffer = new ByteBuffer(data);
            {
                while (buffer.RemainLength > 0)
                {
                    var pbTag = buffer.TakeVarIntBytes(out var _);
                    var pbType = ProtoUtils.VarintGetPbType(pbTag);
                    var pbPath = (treePath == "" ? "" : treePath + ".") + ByteConverter.Hex(pbTag);
                    var pbData = new byte[0];

                    switch (pbType)
                    {
                        case ProtoType.VarInt: buffer.TakeVarIntBytes(out pbData); break;
                        case ProtoType.Bit32: buffer.TakeBytes(out pbData, 4); break;
                        case ProtoType.Bit64: buffer.TakeBytes(out pbData, 8); break;
                        case ProtoType.LengthDelimited:
                            buffer.TakeBytes(out pbData, (uint)buffer.TakeVarIntValue(out var _));
                            if (recursion && DeSerialize(pbPath, pbData, recursion))
                                continue;
                            break;
                        default: return false; // 發生錯誤 此bytearray不可解
                    }

                    leaves[pbPath] = new ProtoLeaf { data = pbData };
                }
            }
            return true;
        }
    }
}
