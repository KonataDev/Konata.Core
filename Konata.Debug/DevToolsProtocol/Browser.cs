using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Text;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Konata.Debug.DevToolsProtocol
{
    public struct BrowserConfig
    {
        public string _executeName;
        public string _profilePrefix;
        public string _profile;
    }

    public class Browser
    {
        public static readonly BrowserConfig Chrome =
            new BrowserConfig
            {
                _executeName = "chrome",
                _profile = "Default",
                _profilePrefix = $"%LOCALAPPDATA%/Google/Chrome*/User Data/"
            };

        public static readonly BrowserConfig MsEdge =
            new BrowserConfig
            {
                _executeName = "msedge",
                _profile = "Default",
                _profilePrefix = $"%LOCALAPPDATA%/Microsoft/Edge*/User Data/"
            };

        private Process _browserInstance;
        private string _browserDevUrl;

        /// <summary>
        /// 啓動Chrome
        /// </summary>
        /// <param name="profileName"></param>
        /// <param name="userAgent"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        /// <param name="disableExtensions"></param>
        /// <returns></returns>
        public bool Open(BrowserConfig browser, string userAgent, uint windowWidth,
            uint windowHeight, bool disableExtensions)
        {
            string pathToBrowser = "";
            string argsToBrowser = "";
            string pathToProfile = "";
            ushort debugPort = FindAvailablePort();

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    pathToBrowser = browser._executeName;
                    pathToProfile = Path.GetFileName($"{browser._profilePrefix}{browser._profile}");
                    break;
                    //case PlatformID.Unix:
                    //    pathToBrowser = "chrome";
                    //    pathToProfile = "";
                    //    break;
            }

            if (pathToBrowser == "")
            {
                return false;
            }

            _browserDevUrl = $"http://127.0.0.1:{debugPort}";
            argsToBrowser += disableExtensions ? "--disable-extensions " : "";
            argsToBrowser += $"--remote-debugging-port={debugPort} ";
            argsToBrowser += $"--window-size={windowWidth},{windowHeight} ";
            argsToBrowser += $"--homepage=about:blank ";
            argsToBrowser += $"--enable-automation ";
            argsToBrowser += $"--no-sandbox ";
            argsToBrowser += $"--user-agent=\"{userAgent}\" ";
            argsToBrowser += $"--user-data-dir=\"{pathToProfile}\"";

            _browserInstance = Process.Start(pathToBrowser, argsToBrowser);

            return _browserInstance.Id != 0;
        }

        /// <summary>
        /// 關閉瀏覽器
        /// </summary>
        /// <returns></returns>
        public bool WaitForExit()
        {
            _browserInstance.WaitForExit();
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
            var http = WebRequest.CreateHttp($"{_browserDevUrl}/{endpoint}");
            var response = http.GetResponse();

            byte[] data = new byte[response.ContentLength];
            response.GetResponseStream().Read(data, 0, data.Length);

            return Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// 尋找可用端口
        /// </summary>
        /// <returns></returns>
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
