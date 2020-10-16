using System;
using System.Collections.Generic;
using System.Text;
using Konata.Library.IO;

namespace Konata.Library.Protobuf
{
    using ProtoLeaves = List<ProtoLeaf>;

    public struct ProtoLeaf
    {
        public string _path;
        public byte[] _data;
    }

    public class ProtoTreeRoot
    {
        internal ProtoLeaves _leaves;
        public delegate void TreeRootWriter(ProtoTreeRoot tree);

        public ProtoTreeRoot()
        {
            _leaves = new ProtoLeaves();
        }

        public void addTree(string treePath, ProtoTreeRoot value)
        {
            addLeaf(treePath, ProtoSerializer.Serialize(value));
        }

        public void addTree(string treePath, TreeRootWriter writer)
        {
            var newTree = new ProtoTreeRoot();
            {
                writer(newTree);
            }
            addTree(treePath, newTree);
        }

        public void addLeaf(string leafPath, string value)
        {
            addLeaf(leafPath, Encoding.UTF8.GetBytes(value));
        }

        public void addLeaf(string leafPath, long value)
        {
            addLeaf(leafPath, ByteConverter.Int64ToBytes(value));
        }

        public void addLeaf(string leafPath)
        {
            addLeaf(leafPath, new byte[0]);
        }

        private void addLeaf(string leafName, byte[] leafData)
        {
            _leaves.Add(new ProtoLeaf { _path = leafName, _data = leafData });
        }
    }
}
