using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Konata.Core.Utils;
using Konata.Core.Builder;
using Konata.Core.Extensions;

namespace Konata.Core.MQ
{
    /// <summary>
    /// Konata内部内存消息队列实现
    /// 一对多
    /// 未实现FIFO
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KonataMemMQ<T> : IMQ<T>, IDisposable
    {
        private Type msgtype = typeof(T);
        private int readtimeout = -1;
       
        public Type MsgType
        {
            get => msgtype;
        }

        public String TaskQueueID
        {
            get;
            private set;
        }

        public bool Running
        {
            get; private set;
        }

        public int MQCount
        {
            get => this._queue.Count;
        }

        public bool Closed
        {
            get => this._queue.IsCompleted;
        }

        private BlockingCollection<T> _queue = null;
        private CancellationTokenSource _source = null;
        private event Action<T> _processItemEvent = null;
        private TaskQueue _processqueue = null;

        public KonataMemMQ(IMQBuilder<T> builder)
        {
            MQConfig config = builder.GetMQConfig();
            List<Action<T>> processitemmethods = builder.GetMQReceiver();
            this.readtimeout = config.ReadTimeout;
            this.TaskQueueID = IdGenerater.GenerateGUID();
            this._processqueue = builder.GetExternalTaskQueue() ??
                TaskQueue.CreateGlobalQueue(TaskQueueID, (config.MaxProcessMTask > 0) ? config.MaxProcessMTask : 8); ;
            if (config.MaxMQLenth > 0)
            {
                this._queue = new BlockingCollection<T>(config.MaxMQLenth);
            }
            else
            {
                this._queue = new BlockingCollection<T>();
            }
            this._source = new CancellationTokenSource();

            foreach (var e in processitemmethods)
            {
                this._processItemEvent += e;
            }
        }
        public void Add(T data)
        {
            this._queue.Add(data);
        }

        public void Add(T data, CancellationToken token)
        {
            try
            {
                this._queue.Add(data, token);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        public bool TryAdd(T data, int timeout)
        {
            return this._queue.TryAdd(data, timeout);
        }

        public void StartTakeProcess()
        {
            if (!this.Running)
            {
                if (this._source.IsCancellationRequested)
                {
                    this._source = new CancellationTokenSource();
                }
                this.Running = true;
                this._processqueue.RunAsync(TakeProcess);
            }
        }

        public void StopTakeProcess()
        {
            this._source.Cancel();
        }

        private void TakeProcess()
        {
            if (this._queue != null && !this._queue.IsCompleted && !this._source.Token.IsCancellationRequested)
            {
                while (!this._queue.IsCompleted && !this._source.Token.IsCancellationRequested)
                {
                    try
                    {
                        T data = default(T);
                        if (this.readtimeout > 0)
                        {
                            if (!this._queue.TryTake(out data, this.readtimeout))
                            {
                                continue;
                            }
                            else
                            {
                                if (this._source.Token.IsCancellationRequested)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            data = this._queue.Take(this._source.Token);
                        }
                        this._processqueue.RunAsync(() =>
                        {
                            this._processItemEvent?.Invoke(data);
                        });
                    }
                    catch (OperationCanceledException)
                    {
                        continue;
                    }

                }
            }
            this.Running = false;
        }

        public void Dispose()
        {
            this._source?.Cancel();
            this._queue?.CompleteAdding();
            this._queue?.Dispose();
            this._processItemEvent = null;
        }

        ~KonataMemMQ()
        {
            this._source?.Cancel();
            this._queue?.CompleteAdding();
            this._queue?.Dispose();
            this._processItemEvent = null;
        }
    }
}
