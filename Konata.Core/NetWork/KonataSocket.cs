using Konata.Core.Builder;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Konata.Core.Extensions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Konata.Core.NetWork
{
    /// <summary>
    /// Konata内部Socket实例,默认使用该Socket对象创建socket
    /// 该Socket对象暂不支持重新连接 失败即抛弃
    /// </summary>
    public sealed class KonataSocket : ISocket, IDisposable
    {
        private int minpackagelen=1;
        private Socket socket = null;
        private int conntimeoutms = 30000;
        private ProtocolType ptype = ProtocolType.Unspecified;
        public bool Connected
        {
            get => socket != null && socket.Connected;
        }

        public ProtocolType ProtocolType
        {
            get => ptype;
        }

        private IPEndPoint hostEndPoint;

        private ManualResetEvent timeoutConnObj = new ManualResetEvent(false);

        SocketBuffer socketBuffer = null;
        List<Byte> m_buffer;

        private List<SocketEventHandler> listSendHandler = new List<SocketEventHandler>();
        private SocketEventHandler receiveHandler = new SocketEventHandler();
        private int tagcount = 0;

        private int recvpackagelen = -1;
        private object recvpacklock = new object();

        private Func<List<Byte>, int> recvlencalcer = null;
        private Action<Byte[]> receiveAction = null;
        private Action serverCloseAction = null;
        private Action<Exception> exceptionHandler = null;



        public KonataSocket(ISocketBuilder builder)
        {
            SocketConfig config = builder.GetSocketConfig();

            this.receiveAction = builder.GetServerDataReceiver();
            this.serverCloseAction = builder.GetServerCloseListener();
            this.recvlencalcer = builder.GetRecvLenCaler();

            if (this.recvlencalcer == null)
            {
                throw new ArgumentException("必须提供报文长度计算委托用于截取需要的报文");
            }

            if (config == null||!IPAddress.TryParse(config.Ip,out IPAddress address))
            {
                throw new ArgumentException("Socket初始化必须携带config");
            }

            this.conntimeoutms = config.Timeout;
            this.minpackagelen = config.MinPackageLen;

            this.hostEndPoint = new IPEndPoint(address, config.Port);
            this.ptype = config.ProtocolType;
            this.socket = new Socket(this.hostEndPoint.AddressFamily, config.SocketType, this.ptype);
            this.m_buffer = new List<byte>();
            int maxbsize = config.TotalBufferSize >= config.BufferSize ? config.TotalBufferSize : config.BufferSize;
            int eachbsize= config.TotalBufferSize < config.BufferSize ? config.TotalBufferSize : config.BufferSize;
            this.socketBuffer = new SocketBuffer(maxbsize, eachbsize);
        }

        public SocketError Connect()
        {
            SocketAsyncEventArgs connectArgs = new SocketAsyncEventArgs
            {
                UserToken = this.socket,
                RemoteEndPoint = hostEndPoint
            };
            connectArgs.Completed+=new EventHandler<SocketAsyncEventArgs>(OnConnect);
            socket.ConnectAsync(connectArgs);

            if (timeoutConnObj.WaitOne(this.conntimeoutms, false))
            {
                return connectArgs.SocketError;
            }
            else
            {
                this.socket.Close();
                throw new TimeoutException("Socket Connection Timeout");
            }
        }

        //连接操作的回调函数
        private void OnConnect(object sender, SocketAsyncEventArgs e)
        {
            // 连接完成信号
            timeoutConnObj.Set();
            if (e.SocketError == SocketError.Success && this.Connected)
            {
                InitArgs(e);
            }
        }

        #region Init R/S Handler
        /// <summary>  
        /// 初始化收发参数  
        /// </summary>  
        /// <param name="e"></param>  
        private void InitArgs(SocketAsyncEventArgs e)
        {
            socketBuffer.InitBuffer();

            //接收参数  
            receiveHandler.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
            receiveHandler.UserToken = e.UserToken;
            receiveHandler.EventTagId = 0;
            receiveHandler.Using = true;

            socketBuffer.SetBuffer(receiveHandler);

            //启动接收监视
            if (!e.ConnectSocket.ReceiveAsync(receiveHandler))
                ProcessReceive(receiveHandler);
            //发送参数  
            InitSendArgs();
        }


        /// <summary>  
        /// 初始化发送参数MySocketEventArgs  
        /// </summary>  
        /// <returns></returns>  
        SocketEventHandler InitSendArgs()
        {
            SocketEventHandler sendArg = new SocketEventHandler()
            {
                UserToken = socket,
                RemoteEndPoint = hostEndPoint,
                Using = false,
                EventTagId = tagcount
            };
            sendArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);

            Interlocked.Increment(ref tagcount);

            lock (listSendHandler)
            {
                listSendHandler.Add(sendArg);
            }
            return sendArg;
        }
        #endregion
       
        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            SocketEventHandler seh = (SocketEventHandler)e;
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    seh.Using = false; //数据发送已完成.状态设为False  
                    ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("超出预估的事件");
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            try
            {
                Socket token = (Socket)e.UserToken;
                //只有主机未关闭且连接正常时才处理
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    //读取数据
                    byte[] data = new byte[e.BytesTransferred];
                    Array.Copy(e.Buffer, e.Offset, data, 0, e.BytesTransferred);
                    
                    lock (m_buffer)
                    {
                        m_buffer.AddRange(data);
                    }

                    do
                    {
                        byte[] recv = null;
                        lock (recvpacklock)
                        {
                            if (this.recvpackagelen <= 0)
                            {
                                this.recvpackagelen = recvlencalcer.Invoke(this.m_buffer);
                            }
                            if (this.recvpackagelen > m_buffer.Count)
                            {
                                break;
                            }
                            recv = m_buffer.GetRange(0, this.recvpackagelen).ToArray();
                            lock (m_buffer)
                            {
                                m_buffer.RemoveRange(0, this.recvpackagelen);
                            }
                            this.recvpackagelen = -1;
                        }

                        //byte[] lenBytes = m_buffer.GetRange(1, 3).ToArray();
                        //int packageLen = BitConverter.ToInt16(lenBytes, 0);
                        //if (packageLen > m_buffer.Count - 5)
                        //{
                        //    break;
                        //}
                        Task.Run(()=> { receiveAction(recv); });

                    } while (m_buffer.Count > this.minpackagelen);
                    //继续接收  
                    if (!token.ReceiveAsync(e))
                        this.ProcessReceive(e);
                }
                else
                {
                    ProcessError(e);
                }
            }
            catch (Exception ex)
            {
                exceptionHandler?.Invoke(ex);
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                ProcessError(e);
            }
        }

        //发送失败注销整个socket
        private void ProcessError(SocketAsyncEventArgs e)
        {
            Socket s = (Socket)e.UserToken;
            if (s.Connected)
            {

                try
                {
                    s.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    exceptionHandler?.Invoke(ex);
                }
                finally
                {
                    if (s.Connected)
                    {
                        s.Close();
                    }
                }
            }
            //注销事件 
            foreach (SocketEventHandler handler in listSendHandler)
                handler.Completed -= IO_Completed;

            receiveHandler.Completed -= IO_Completed;
            this.serverCloseAction?.Invoke();
        }

        public void DisConnect()
        {
            this.socket.Disconnect(false);
        }

        public void Send(byte[] sendBuffer)
        {
            if (Connected)
            {
                //协议一定要一致 否则无法收包
                //查找有没有空闲的发送MySocketEventArgs,有就直接拿来用,没有就创建新的
                SocketEventHandler sendArgs = listSendHandler.Find(a => a.Using == false);
                if (sendArgs == null)
                {
                    sendArgs = InitSendArgs();
                }
                lock (sendArgs) //防止其他线程取走即将发送的包
                {
                    sendArgs.Using = true;
                    sendArgs.SetBuffer(sendBuffer, 0, sendBuffer.Length);
                }
                //support tcp/udp send
                switch (ProtocolType)
                {
                    case ProtocolType.Tcp:
                        socket.SendAsync(sendArgs);
                        break;
                    case ProtocolType.Udp:
                        socket.SendToAsync(sendArgs);
                        break;
                    default:
                        return;
                }
                
            }
        }

        public void Dispose()
        {
            timeoutConnObj.Close();
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            socket.Close();
            timeoutConnObj.Dispose();
            socket = null;
            timeoutConnObj = null;
        }

        ~KonataSocket()
        {
            socket?.Dispose();
            timeoutConnObj.Dispose();
        }


    }
}
