using System;
using System.Net.Sockets;

namespace Konata.Runtime.Network
{
    public class SocketConfig
    {
        /// <summary>
        /// 目标IP
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 目标端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// socket单次接收字节切片大小
        /// </summary>
        public int BufferSize { get; set; } = 1024;

        /// <summary>
        /// 该socket分配的总字节缓冲大小
        /// </summary>
        public int TotalBufferSize { get; set; } = 2048;

        /// <summary>
        /// socket连接超时时间(ms)
        /// </summary>
        public int Timeout { get; set; } = 30000;

        /// <summary>
        /// 用于标记该socket通信中每个报文最小长度，一般指报文头N位表示包长度的长度
        /// 只有当接收到的byte累计超过该数值,才会进行报文读取与处理
        /// </summary>
        public int MinPackageLen { get; set; } = 1;

        public SocketType SocketType { get; set; } = SocketType.Stream;

        public ProtocolType ProtocolType { get; set; } = ProtocolType.Tcp;
    }
}
