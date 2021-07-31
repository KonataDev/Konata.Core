using System;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

using Konata.Core.Events;
using Konata.Core.Entity;

namespace Konata.Core.Components
{
    public class BaseComponent
    {
        public BaseEntity Entity { get; set; }

        public ActionBlock<KonataTask> EventPipeline { get; set; }

        public BaseComponent()
            => EventPipeline = new ActionBlock<KonataTask>(EventHandler);

        internal virtual void EventHandler(KonataTask task) { }

        public void PostEventToEntity(BaseEvent anyEvent)
            => Entity.PostEventToEntity(anyEvent);

        public Task<BaseEvent> PostEvent<T>(BaseEvent anyEvent)
            where T : BaseComponent => Entity.PostEvent<T>(anyEvent);

        public async Task<TEvent> PostEvent<TEntity, TEvent>(BaseEvent anyEvent)
            where TEvent : BaseEvent where TEntity : BaseComponent
        {
            var task = Entity.PostEvent<TEntity>(anyEvent);
            return (TEvent)await task;
        }

        public void BroadcastEvent(BaseEvent anyEvent)
            => Entity.BroadcastEvent(anyEvent);

        public T GetComponent<T>()
            where T : BaseComponent => Entity.GetComponent<T>();

        #region Log Methods

        protected void Log(LogLevel logLevel, string tag, string content)
            => PostEventToEntity(new LogEvent
            {
                Tag = tag,
                Level = logLevel,
                EventMessage = content
            });

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
