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

            var browser = new Browser();
            Console.WriteLine($"Opening browser...");

            var userAgent = "useragent";
            if (!browser.Open(Browser.Chrome, userAgent, 480, 720, true))
            {
                Console.WriteLine($"Your device haven't installed chromium kernel family web browser. (Need DevTools Protocol support.)");
                return false;
            }

            string sigTicket;
            var session = browser.ConnectToFirstTab();
            {
                session.EnableNetwork(65536);
                session.EnableTouchSimulation(1, "mobile");
                session.NavigateTo(sigUrl);

                sigTicket = session.WaitForTicket();
            }
            session.Close();
            browser.WaitForExit();

            if (sigTicket == null || sigTicket == "")
            {
                return false;
            }

            // 提交驗證
            Console.WriteLine($"Ticket got => \n{sigTicket}");
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
