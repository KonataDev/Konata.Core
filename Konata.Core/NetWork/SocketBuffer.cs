using System.Collections.Generic;
using System.Net.Sockets;


namespace Konata.Core.NetWork
{
    /// <summary>
    /// socket缓存管理
    /// 针对SocketAsyncEventArgs分配缓存
    /// </summary>
    class SocketBuffer
    {
        int _numBytes;//缓冲池最大可控字节数
        byte[] _buffer;//缓冲池大小
        Stack<int> _freeIndexPool;
        int _currentIndex;
        int _bufferSize;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalBytes">缓冲池最大字节量</param>
        /// <param name="bufferSize">每次请求缓冲池时标准切片大小</param>

        public SocketBuffer(int totalBytes, int bufferSize)
        {
            _numBytes = totalBytes;
            _currentIndex = 0;
            _bufferSize = bufferSize;
            _freeIndexPool = new Stack<int>();
        }
        // 分配缓冲池大小
        public void InitBuffer()
        {
            _buffer = new byte[_numBytes];
        }

        /// <summary>
        /// 给每个SocketAsyncEventArgs分配缓存
        /// </summary>
        /// <param name="args">SocketAsyncEventArgs</param>
        /// <returns>分配成功返回true，否则返回false</returns>
        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            if (_freeIndexPool.Count > 0)
            {
                args.SetBuffer(_buffer, _freeIndexPool.Pop(), _bufferSize);
            }
            else
            {
                if ((_numBytes - _bufferSize) < _currentIndex)
                {
                    return false;
                }
                args.SetBuffer(_buffer, _currentIndex, _bufferSize);
                _currentIndex += _bufferSize;
            }
            return true;
        }

        /// <summary>
        /// 释放缓存
        /// </summary>
        /// <param name="args">新的Socket</param>
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            _freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }
    }
}
