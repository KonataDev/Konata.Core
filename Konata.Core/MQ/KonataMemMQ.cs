using Konata.Core.Builder;
using Konata.Core.Extensions;
using Konata.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Guid = Konata.Utils.Guid;

namespace Konata.Core.MQ
{
    /// <summary>
    /// Konata内部内存消息队列实现
    /// 一对多
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KonataMemMQ<T>:IMQ<T>,IDisposable
    {
        private Type msgtype=typeof(T);
        public Type MsgType
        {
            get => msgtype;
        }

        private BlockingCollection<T> _queue = null;
        private CancellationTokenSource _source = null;
        private event Action<T> _processItemEvent = null;
        private TaskQueue _processqueue = null;

        public KonataMemMQ(IMQBuilder<T> builder)
        {
            MQConfig config=builder.GetMQConfig();
            List<Action<T>> processitemmethods = builder.GetServerDataReceiver();

            this._processqueue = TaskQueue.CreateGlobalQueue(Guid.Generate().ToString(), (config.MaxProcessMTask>0)?config.MaxProcessMTask:8);


        }
        public void Add(T data)
        {
            throw new NotImplementedException();
        }

        public void StartTakeProcess(int timeout)
        {
            throw new NotImplementedException();
        }

        public void StopTakeProcess()
        {
            throw new NotImplementedException();
        }

        public bool TryAdd(T data, int timeout)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this._source?.Cancel();
            this._queue?.CompleteAdding();
            this._queue?.Dispose();
            this._queue?.Dispose();
            
        }
    }
}
