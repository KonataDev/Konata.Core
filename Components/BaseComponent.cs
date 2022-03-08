using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Konata.Core.Events;
using Konata.Core.Entity;

// ReSharper disable MemberCanBeProtected.Global

namespace Konata.Core.Components
{
    public class BaseComponent
    {
        public BaseEntity Entity { get; set; }

        // public ActionBlock<KonataTask> EventPipeline { get; }

        public Action<KonataTask> EventPipeline { get; }

        public BaseComponent()
        {
            // EventPipeline = new ActionBlock<KonataTask>(async t =>
            // {
            //     try
            //     {
            //         // Force finish the tasks if the
            //         // handler does not save the event by itself.
            //         if (!await OnHandleEvent(t) && !t.Complected) t.Finish();
            //     }
            //     catch (Exception e)
            //     {
            //         if (!t.Complected) t.Exception(e);
            //     }
            // });

            EventPipeline = t =>
            {
                ThreadPool.QueueUserWorkItem(async _ =>
                {
                    try
                    {
                        // Force finish the tasks if the
                        // handler does not save the event by itself.
                        if (!await OnHandleEvent(t) && !t.Complected) t.Finish();
                    }
                    catch (Exception e)
                    {
                        if (!t.Complected) t.Exception(e);
                    }
                });
            };
        }

        internal virtual Task<bool> OnHandleEvent(KonataTask task)
        {
            return Task.FromResult(false);
        }

        public void PostEventToEntity(BaseEvent anyEvent)
            => Entity.PostEventToEntity(anyEvent);

        public Task<BaseEvent> PostEvent<T>(BaseEvent anyEvent)
            where T : BaseComponent => Entity.PostEvent<T>(anyEvent);

        public async Task<TEvent> PostEvent<TEntity, TEvent>(BaseEvent anyEvent)
            where TEvent : BaseEvent where TEntity : BaseComponent
        {
            var task = Entity.PostEvent<TEntity>(anyEvent);
            return (TEvent) await task;
        }

        public void BroadcastEvent(BaseEvent anyEvent)
            => Entity.BroadcastEvent(anyEvent);

        public T GetComponent<T>()
            where T : BaseComponent => Entity.GetComponent<T>();

        public virtual void OnInit()
        {
        }

        public virtual void OnDestroy()
        {
        }

        #region Log Methods

        private void Log(LogLevel logLevel, string tag, string content)
            => PostEventToEntity(LogEvent.Create(tag, logLevel, content));

        public void LogV(string tag, string content)
            => Log(LogLevel.Verbose, tag, content);

        public void LogI(string tag, string content)
            => Log(LogLevel.Information, tag, content);

        public void LogW(string tag, string content)
            => Log(LogLevel.Warning, tag, content);

        public void LogE(string tag, string content)
            => Log(LogLevel.Exception, tag, content);

        public void LogE(string tag, Exception e)
            => LogE(tag, $"{e.Message}\n{e.StackTrace}");

        public void LogF(string tag, string content)
            => Log(LogLevel.Fatal, tag, content);

        #endregion
    }
}
