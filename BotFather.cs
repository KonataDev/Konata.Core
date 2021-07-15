using System;
using System.Text;

using Konata.Utils;
using Konata.Core.Event;
using Konata.Core.Component;
using Konata.Core.Service;

namespace Konata.Core
{
    public static class BotFather
    {
        /// <summary>
        /// Create a bot
        /// </summary>
        /// <param name="handler"><b>[In] </b>bot event handler</param>
        /// <param name="config"><b>[In] </b>bot configuration</param>
        /// <param name="device"><b>[In] </b>bot device definition</param>
        /// <returns></returns>
        public static Bot CreateBot(BotConfig config, BotDevice device,
            BotKeyStore keystore, Action<CoreEvent> handler)
        {
            var entity = new Bot();
            {
                // Load components
                foreach (var type in Reflection
                    .GetClassesByAttribute<ComponentAttribute>())
                {
                    entity.AddComponent((BaseComponent)Activator.CreateInstance(type));
                }

                // Setup event handler
                entity.SetEventHandler(handler);

                // Setup configs
                var component = entity.GetComponent<ConfigComponent>();
                {
                    component.LoadConfig(config);
                    component.LoadDeviceInfo(device);
                    component.LoadSignInfo(new SignInfo(keystore));
                }
            }

            return entity;
        }
    }
}
