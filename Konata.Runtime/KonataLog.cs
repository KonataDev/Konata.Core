using System;
using Microsoft.Extensions.Logging;

namespace Konata.Runtime
{
    /// <summary>
    /// Log管理器
    /// <para>对应平台需要提供具体的Provider来规范日志输出</para>
    /// <para>不同名组件等内容将会使用单一log对象</para>
    /// </summary>
    public class KonataLog
    {
        private KonataLog instance;

        public KonataLog Instance
        {
            get => instance ?? (instance = new KonataLog());
        }

        private KonataLog()
        {

        }

        private ILoggerFactory loggerFactory = null;

        /// <summary>
        /// 日志工厂是否已经被初始化
        /// </summary>
        public bool FactoryInited
        {
            get => (this.loggerFactory != null);
        }

        public void AddNewFactory(ILoggingBuilder builderconfig)
        {
            ILoggerFactory factory = LoggerFactory
                .Create(config => config = builderconfig);
            //ILogger<Program> logger = factory.CreateLogger<Program>();
            //logger.LogInformation("Hello World!");
        }

        public ILogger<T> GetLogger<T>()
        {
            if (FactoryInited)
            {
                return this.loggerFactory.CreateLogger<T>();
            }
            return null;
        }
    }
}
