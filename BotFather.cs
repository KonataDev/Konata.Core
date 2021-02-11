using System;
using System.Text;
using System.Threading.Tasks.Dataflow;

using Konata.Utils;
using Konata.Core.Event;
using Konata.Core.Component;

namespace Konata.Core
{
    public static class BotFather
    {
        /// <summary>
        /// Instantiate a bot
        /// </summary>
        /// <param name="handler"><b>[In] </b>bot event handler</param>
        /// <param name="config"><b>[In] </b>bot configuration</param>
        /// <returns></returns>
        public static Bot CreateBot(ActionBlock<BaseEvent> handler, BotConfig config)
        {
            var entity = new Bot();
            {
                foreach (var type in Reflection.GetClassesByAttribute<ComponentAttribute>())
                {
                    entity.AddComponent((BaseComponent)Activator.CreateInstance(type));
                }

                entity.SetEventHandler(handler);
                entity.GetComponent<ConfigComponent>().LoadConfig(config);
            }

            return entity;
        }

        /// <summary>
        /// Instantiate a bot
        /// </summary>
        /// <param name="botUin"><b>[In] </b>bot uin number</param>
        /// <param name="botPassword"><b>[In] </b>bot password</param>
        /// <param name="handler"><b>[In] </b>bot event handler</param>
        /// <returns></returns>
        public static Bot CreateBot(uint botUin, string botPassword,
            ActionBlock<BaseEvent> handler)
            => CreateBot(handler, new BotConfig
            {
                Uin = botUin,
                Password = botPassword,
            });
    }
}
