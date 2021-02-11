using System;
using System.Text;

using Konata.Core.Service;

namespace Konata.Core.Component
{
    [Component("ConfigComponent", "Konata Config Management Component")]
    public class ConfigComponent : BaseComponent
    {
        public SignInfo SignInfo { get; private set; }

        public void LoadConfig(BotConfig config)
        {
            SignInfo = new SignInfo(config.Uin, config.Password);
        }
    }
}
