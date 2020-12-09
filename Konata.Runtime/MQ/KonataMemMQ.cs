using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Konata.Runtime.Utils;
using Konata.Runtime.Builder;
using Konata.Runtime.Extensions;

namespace Konata.Runtime.MQ
{
    /// <summary>
    /// Konata内部内存消息队列实现
    /// 一对多
    /// 未实现FIFO
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KonataMemMQ<T> : IMQ<T>, IDisposable
    {
        private int _readTimeout = -1;
        private Type _messageType;

        private BlockingCollection<T> _queue;
        private CancellationTokenSource _cancelToken;

        private event Action<T> _processItemEvent;
        private TaskQueue _processQueue;

        private bool _isRunning;
        private string _taskQueueGuid;

        public Type MsgType
        {
            get => _messageType;
        }

        public string TaskQueueId
        {
            get => _taskQueueGuid;
        }

        public bool Running
        {
            get => _isRunning;
        }

        public int MQCount
        {
            get => _queue.Count;
        }

        public bool Closed
        {
            get => _queue.IsCompleted;
        }

        public KonataMemMQ(IMQBuilder<T> builder)
        {
            MQConfig config = builder.GetMQConfig();
            List<Action<T>> processitemmethods = builder.GetMQReceiver();

            _messageType = default;
            _readTimeout = config.ReadTimeout;
            _processQueue = builder.GetExternalTaskQueue() ??
                TaskQueue.CreateGlobalQueue(TaskQueueId, (config.MaxProcessMTask > 0) ? config.MaxProcessMTask : 8); ;
            _taskQueueGuid = IdGenerater.GenerateGUID();

            if (config.MaxMQLenth > 0)
            {
                _queue = new BlockingCollection<T>(config.MaxMQLenth);
            }
            else
            {
                _queue = new BlockingCollection<T>();
            }
            _cancelToken = new CancellationTokenSource();

            foreach (var e in processitemmethods)
            {
                _processItemEvent += e;
            }
        }

        public void Add(T data)
        {
            _queue.Add(data);
        }

        public void Add(T data, CancellationToken token)
        {
            try
            {
                _queue.Add(data, token);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        public bool TryAdd(T data, int timeout)
        {
            return _queue.TryAdd(data, timeout);
        }

        public void StartTakeProcess()
        {
            if (!Running)
            {
                if (_cancelToken.IsCancellationRequested)
                {
                    _cancelToken = new CancellationTokenSource();
                }
                _isRunning = true;
                _processQueue.RunAsync(TakeProcess);
            }
        }

        public void StopTakeProcess()
        {
            _cancelToken.Cancel();
        }

        private void TakeProcess()
        {
            if (_queue != null && !_queue.IsCompleted && !_cancelToken.Token.IsCancellationRequested)
            {
                while (!_queue.IsCompleted && !_cancelToken.Token.IsCancellationRequested)
                {
                    try
                    {
                        T data = default(T);
                        if (_readTimeout > 0)
                        {
                            if (!_queue.TryTake(out data, _readTimeout))
                            {
                                continue;
                            }
                            else
                            {
                                if (_cancelToken.Token.IsCancellationRequested)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            data = _queue.Take(_cancelToken.Token);
                        }
                        _processQueue.RunAsync(() =>
                        {
                            _processItemEvent?.Invoke(data);
                        });
                    }
                    catch (OperationCanceledException)
                    {
                        continue;
                    }

                }
            }
            _isRunning = false;
        }

        public void Dispose()
        {
            _cancelToken?.Cancel();
            _queue?.CompleteAdding();
            _queue?.Dispose();
            _processItemEvent = null;
        }

        ~KonataMemMQ()
            => Dispose();
    }
}
