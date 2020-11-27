using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Konata.Utils
{
    /// <summary>
    /// 配置文件加载器
    /// </summary>
    public static class ConfigurationReader
    {
        public static string CurrentPath { get; private set; } = Directory.GetCurrentDirectory();
        /// <summary>
        /// 装载配置文件跟踪器
        /// </summary>
        /// <param name="basepath">文件路径</param>
        /// <param name="filename">文件名</param>
        /// <param name="reloadOnChange">配置重载</param>
        /// <returns></returns>
        public static IConfigurationRoot LoadConfig(
            string basepath=null,
            string filename="appsettings.json",
            bool reloadOnChange = false
            )
        {
            if (String.IsNullOrEmpty(basepath))
            {
                basepath = CurrentPath;
            }
            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(basepath)
                    .AddJsonFile(filename, true, reloadOnChange);
                return config.Build();
            }catch(Exception)
            {
                return null;
            }
        }
    }
}
