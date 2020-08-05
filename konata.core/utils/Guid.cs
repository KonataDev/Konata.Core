using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata.Utils
{
    public static class Guid
    {
        public static byte[] Generate()
        {
            return new System.Guid().ToByteArray();
        }
    }
}
