using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata.Utils.Crypt
{
    public interface ICryptor
    {
        byte[] Encrypt(byte[] data);

        byte[] Encrypt(byte[] data, byte[] key);

        byte[] Decrypt(byte[] data);

        byte[] Decrypt(byte[] data, byte[] key);

    }
}
