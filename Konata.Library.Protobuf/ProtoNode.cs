using System;
using System.Collections.Generic;

namespace Konata.Library.Protobuf
{
    using ProtoDict = Dictionary<string, object>;

    public class ProtoNode
    {
        protected ProtoDict _dict;

        public ProtoNode()
        {
            _dict = new ProtoDict();
        }

        public void addTreePath(string treeName)
        {
            
        }

        public void addLeaf(string leafName, string value)
        {

        }

        public void addLeaf(string leafName, long value)
        {

        }

        public void addLeaf(string leafName, ProtoNode value)
        {
            
        }
    }
}
