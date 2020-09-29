using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Text;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Konata.Debug.DevToolsProtocol
{
    public class Browser
    {
        private Process _chromeInstance;
        private string _chromeDevUrl;

        public bool Open(string profileName, uint windowWidth,
            uint windowHeight, bool disableExtensions)
        {
            string pathToChrome = "";
            string pathToProfile = "";
            string argsToChrome = "";
            ushort debugPort = FindAvailablePort();

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    pathToChrome = "chrome.exe";
                    pathToProfile = Path.GetFileName($"%LOCALAPPDATA%/Google/Chrome*/User Data/{profileName}");
                    break;
                case PlatformID.Unix:
                    pathToChrome = "chrome";
                    pathToProfile = "";
                    break;
            }

            if (pathToChrome == "")
            {
                return false;
            }

            _chromeDevUrl = $"http://127.0.0.1:{debugPort}";
            argsToChrome += disableExtensions ? "--disable-extensions " : "";
            argsToChrome += $"--remote-debugging-port={debugPort} ";
            argsToChrome += $"--window-size={windowWidth},{windowHeight} ";
            argsToChrome += $"--homepage=about:blank ";
            argsToChrome += $"--enable-automation ";
            argsToChrome += $"--no-sandbox ";
            argsToChrome += $"--user-data-dir=\"{pathToProfile}\"";

            _chromeInstance = Process.Start(pathToChrome, argsToChrome);

            return _chromeInstance.Id != 0;
        }

        /// <summary>
        /// 關閉瀏覽器
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            _chromeInstance.WaitForExit();
            return false;
        }

        /// <summary>
        /// 連接到指定標籤頁 返回WebSocket會話
        /// </summary>
        /// <returns></returns>
        public Session ConnectToTab(string tab)
        {
            return new Session(tab);
        }

        /// <summary>
        /// 連接到第一個標籤頁 返回WebSocket會話
        /// </summary>
        /// <returns></returns>
        public Session ConnectToFirstTab()
        {
            var tabs = ListAllTabs();

            if (tabs.Length <= 0)
                return null;

            return new Session(tabs[0]);
        }

        /// <summary>
        /// 列出所有標籤頁
        /// </summary>
        /// <returns></returns>
        public string[] ListAllTabs()
        {
            var json = RequestEndpoint("json/list");

            // "type":\s"page",\s*.*\s*"webSocketDebuggerUrl"*.\s*"(ws:\/\/\S*?)"
            var regex = new Regex("\"type\":\\s\"page\",\\s*.*\\s*\"webSocketDebuggerUrl\"*.\\s*\"(ws:\\/\\/\\S*?)\"",
                RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matches = regex.Matches(json);

            var tabs = new string[matches.Count];
            for (int i = 0; i < matches.Count; ++i)
            {
                tabs[i] = matches[i].Groups[1].Value;
            }

            return tabs;
        }

        /// <summary>
        /// 打開新的標籤頁
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Session OpenNewTab(string url)
        {
            var json = RequestEndpoint($"json/new?{url}");
            return null;
        }

        /// <summary>
        /// 請求接口
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public string RequestEndpoint(string endpoint)
        {
            var http = WebRequest.CreateHttp($"{_chromeDevUrl}/{endpoint}");
            var response = http.GetResponse();

            byte[] data = new byte[response.ContentLength];
            response.GetResponseStream().Read(data, 0, data.Length);

            return Encoding.UTF8.GetString(data);
        }

        private static ushort FindAvailablePort()
        {
            var ipProp = IPGlobalProperties.GetIPGlobalProperties();
            var portList = ipProp.GetActiveTcpListeners();

            for (ushort i = 65534; i > 0; --i)
            {
                foreach (var element in portList)
                {
                    if (element.Port == i)
                    {
                        goto next;
                    }
                }
                return i;
            next:
                continue;
            }

            return 0;
        }
    }
}
