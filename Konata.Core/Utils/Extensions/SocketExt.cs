using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CA2012 // Use ValueTasks correctly
namespace Konata.Core.Utils.Extensions
{
    internal static class SocketExt
    {
        public static ValueTask ReceiveFullyAsync(this Socket socket, byte[] buffer, CancellationToken token = default)
            => socket.ReceiveFullyAsync(new Memory<byte>(buffer, 0, buffer.Length), token);

        public static ValueTask ReceiveFullyAsync(this Socket socket, byte[] buffer, int offset, int size, CancellationToken token = default)
        {
            if (offset + size > buffer.Length)
            {
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }
            Memory<byte> memory = new Memory<byte>(buffer, offset, size);
            return socket.ReceiveFullyAsync(memory, token);
        }

        public static ValueTask ReceiveFullyAsync(this Socket socket, Memory<byte> memory, CancellationToken token = default)
        {
            ValueTask<int> vt = socket.ReceiveAsync(memory, SocketFlags.None, token);
            if (vt.IsCompletedSuccessfully)
            {
                int recved = vt.Result;
                if (recved == memory.Length)
                {
                    return default;
                }
                vt = new ValueTask<int>(recved); // ValueTask<TResult>.Result may not be consumed twice, so we recreate a new ValueTask<int> to store the result.
            }
            return Await(socket, memory, vt, token);

            static async ValueTask Await(Socket socket, Memory<byte> memory, ValueTask<int> recvTask, CancellationToken token)
            {
                while (true)
                {
                    int n = await recvTask;
                    if (n < 1)
                    {
                        throw new SocketException(10054);
                    }
                    else if (n == memory.Length)
                    {
                        return;
                    }
                    memory = memory[n..];
                    recvTask = socket.ReceiveAsync(memory, SocketFlags.None, token);
                }
            }
        }
    }
}
