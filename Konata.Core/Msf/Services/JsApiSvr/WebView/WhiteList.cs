using System;

namespace Konata.Msf.Services.JsApiSvr.WebView
{
    public class WhiteList : Service
    {
        private WhiteList()
        {
            Register("JsApiSvr.webview.whitelist", this);
        }

        public static Service Instance { get; } = new WhiteList();

        public override bool OnRun(Core core, string method, params object[] args)
        {

            return false;
        }

        public override bool OnHandle(Core core, params object[] args)
        {

            return false;
        }

    }
}
