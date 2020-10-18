using System;
using System.Text;
using System.Collections.Generic;
using Konata.Library.IO;

namespace Konata.Library.Protobuf
{
    using ProtoLeaves = List<ProtoLeaf>;

    public struct ProtoLeaf
    {
        public string path;
        public byte[] data;
        public bool needLength;
    }

    public class ProtoTreeRoot
    {
        internal ProtoLeaves leaves;
        public delegate void TreeRootWriter(ProtoTreeRoot tree);

        public ProtoTreeRoot()
        {
            leaves = new ProtoLeaves();
        }

        public void addTree(string treePath, ProtoTreeRoot value)
        {
            addLeafByteBuffer(treePath, ProtoSerializer.Serialize(value));
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
            addLeafBytes(leafPath, VariantConv.NumberToVariant(value), false);
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
            leaves.Add(new ProtoLeaf { path = leafPath, data = value, needLength = needLength });
        }
    }
}
