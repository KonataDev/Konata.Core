using System;
using System.Text;
using System.Collections.Generic;
using Konata.Utils.IO;

namespace Konata.Utils.Protobuf
{
    using ProtoLeaves = SortedDictionary<string, ProtoLeaf>;

    internal struct ProtoLeaf
    {
        public byte[] data;
        public bool needLength;
    }

    internal class ProtoComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var xval = ByteConverter.VarintToNumber(ByteConverter.UnHex(x));
            var yval = ByteConverter.VarintToNumber(ByteConverter.UnHex(y));
            return xval < yval ? -1 : 1;
        }
    }

    public class ProtoTreeRoot
    {
        private ProtoLeaves leaves;
        public delegate void TreeRootWriter(ProtoTreeRoot tree);
        public delegate void TreeRootReader(ProtoTreeRoot tree);

        public ProtoTreeRoot()
        {
            leaves = new ProtoLeaves(new ProtoComparer());
        }

        public ProtoTreeRoot(byte[] data, bool recursion = false)
        {
            leaves = new ProtoLeaves();
            DeSerialize("", data, recursion);
        }

        #region Add Methods

        public void AddTree(ProtoTreeRoot value)
        {
            leaves = value.leaves;
        }

        public void AddTree(string treePath, ProtoTreeRoot value)
        {
            if (value != null)
                AddLeafByteBuffer(treePath, value.Serialize());
        }

        public void AddTree(string treePath, TreeRootWriter writer)
        {
            var newTree = new ProtoTreeRoot();
            {
                writer(newTree);
            }
            AddTree(treePath, newTree);
        }

        public void AddLeafString(string leafPath, string value)
        {
            if (value != null)
                AddLeafBytes(leafPath, Encoding.UTF8.GetBytes(value));
        }

        public void AddLeafFix32(string leafPath, int? value)
        {
            if (value != null)
                AddLeaf(leafPath, ByteConverter.Int32ToBytes((int)value), false);
        }

        public void AddLeafFix64(string leafPath, long? value)
        {
            if (value != null)
                AddLeaf(leafPath, ByteConverter.Int64ToBytes((long)value), false);
        }

        public void AddLeafVar(string leafPath, long? value)
        {
            if (value != null)
                AddLeaf(leafPath, ByteConverter.NumberToVarint((long)value), false);
        }

        public void AddLeafByteBuffer(string leafPath, ByteBuffer value)
        {
            if (value != null)
                AddLeafBytes(leafPath, value.GetBytes());
        }

        public void AddLeafBytes(string leafPath, byte[] value)
        {
            if (value != null)
                AddLeaf(leafPath, value, true);
        }

        public void AddLeafEmpty(string leafPath)
        {
            AddLeaf(leafPath, null, false);
        }

        private void AddLeaf(string leafPath, byte[] value, bool needLength)
        {
            leaves.Add(leafPath, new ProtoLeaf { data = value, needLength = needLength });
        }

        #endregion

        #region Get Methods

        public void GetTree(string treePath, TreeRootReader reader)
        {
            var newTree = new ProtoTreeRoot(leaves[treePath].data);
            {
                reader(newTree);
            }
        }

        public string GetLeafString(string leafPath, out string value)
        {
            return value = Encoding.UTF8.GetString(GetLeaf(leafPath, out var _));
        }

        public int GetLeafFix32(string leafPath, out int value)
        {
            return value = ByteConverter.BytesToInt32(GetLeaf(leafPath, out var _), 0, Endian.Little);
        }

        public long GetLeafFix64(string leafPath, out long value)
        {
            return value = ByteConverter.BytesToInt64(GetLeaf(leafPath, out var _), 0, Endian.Little);
        }

        public long GetLeafVar(string leafPath, out long value)
        {
            return value = ByteConverter.VarintToNumber(GetLeaf(leafPath, out var _));
        }

        public byte[] GetLeaf(string leafPath, out byte[] value)
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
                            buffer.TakeBytes(out pbData, (uint)buffer.TakeVarIntValueLE(out var _));

                            try
                            {
                                if (recursion && !DeSerialize(pbPath, pbData, recursion))
                                    break;
                            }
                            catch
                            {
                                break;
                            }

                            break;

                        // 發生錯誤 此bytearray不可解
                        default: return false;
                    }

                    leaves[pbPath] = new ProtoLeaf { data = pbData };
                }
            }
            return true;
        }
    }
}
