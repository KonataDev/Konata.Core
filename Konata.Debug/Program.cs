using System;
using Konata.Msf;
using Konata.Debug.DevToolsProtocol;

namespace Konata.Debug
{
    internal class Program
    {
        static Bot bot;

        public static void Main()
        {
            bot = new Bot(2051118019, "12345678");
            bot.RegisterDelegate(EventProc);
            bot.Run();

            Console.Write("Exit.");
        }

        private static bool EventProc(EventType type, params object[] args)
        {
            switch (type)
            {
                case EventType.BotStart:
                    return OnBootstrap();
                case EventType.WtLoginVerifySliderCaptcha:
                    return OnSliderCaptcha((string)args[0], (string)args[1]);
                case EventType.WtLoginVerifyImageCaptcha:
                    return OnImageCaptcha();
                case EventType.GroupMessage:
                    return OnGroupMessage();
                case EventType.PrivateMessage:
                    return OnPrivateMessage();
            }

            return false;
        }

        /// <summary>
        /// 框架啓動事件
        /// </summary>
        /// <returns></returns>
        private static bool OnBootstrap()
        {
            bot.Login();
            return true;
        }

        private static bool OnSliderCaptcha(string sigSission, string sigUrl)
        {
            Console.WriteLine($"  SigSession => {sigSission}");
            Console.WriteLine($"  CaptchaUrl => {sigUrl}");

            var chrome = new Browser();
            Console.WriteLine($"Opening chrome browser...");

            if (!chrome.Open("Default", 480, 720, true))
            {
                Console.WriteLine($"Your device have not installed chrome.");
                return false;
            }

            string sigTicket = "";
            var tab = chrome.ConnectToFirstTab();
            {
                tab.EnableNetwork(65536);
                tab.EnablePage();
                tab.EnableRuntime();
                tab.EnableDOM();
                tab.EnableCSS();
                tab.EnableOverlay();
                tab.EnableTouchSimulation(1, "mobile");

                tab.NavigateTo(sigUrl);

                sigTicket = tab.WaitForTicket();
            }
            chrome.Close();

            if (sigTicket == null || sigTicket == "")
            {
                return false;
            }

            // 提交驗證
            bot.SubmitSliderTicket(sigSission, sigTicket);
            return true;
        }

        private static bool OnImageCaptcha()
        {
            return false;
        }

        private static bool OnPrivateMessage()
        {
            return false;
        }

        private static bool OnGroupMessage()
        {
            return false;
        }
    }
}
