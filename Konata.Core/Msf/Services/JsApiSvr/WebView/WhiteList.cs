using System;

namespace Konata.Msf.Services.JsApiSvr.WebView
{
    internal class WhiteList : Service
    {
        private WhiteList()
        {
            Register("JsApiSvr.webview.whitelist", this);
        }

        public static Service Instance { get; } = new WhiteList();

        protected override bool OnRun(Core core, string method, params object[] args)
        {

            return false;
        }

        protected override bool OnHandle(Core core, params object[] args)
        {

            return false;
        }

    }
}
